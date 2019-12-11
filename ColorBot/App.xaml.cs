using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Gamut;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ColorBot
{
	public partial class App : Application
	{
		public event EventHandler VoteDictChanged;
		public event EventHandler<TimeSpan> ResetTimerElapsed;
		public event EventHandler<TimeUpdate> OneSecondUpdate;

		SQLiteConnection DBConnect;
		TwitchClient twitchClient;

		public Dictionary<String, ColorCount> colorCounts = new Dictionary<String, ColorCount>();

		public DateTime CountDownStart = new DateTime();
		public int TimerLengthInSeconds = 90;

		private List<ILocalHueClient> avaialableBridges = new List<ILocalHueClient>();

		public Timer pulse = new Timer()
		{
			Interval = 250
		};

		public MainWindow window;
		public TestWindow test;

		public App()
		{
			ConnectionCredentials connectionCredentials = new ConnectionCredentials("huecolorbot", ConfigurationManager.AppSettings["TwitchApiKey"]);

			twitchClient = new TwitchClient();
			twitchClient.Initialize(connectionCredentials, "Robobobatron");

			twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;

			twitchClient.Connect();

			if(!File.Exists(Directory.GetCurrentDirectory() + @"\ColorBot.sqlite"))
			{
				SQLiteConnection.CreateFile("ColorBot.sqlite");
				DBConnect = new SQLiteConnection("Data Source=ColorBot.sqlite");
				DBConnect.Open();
				new SQLiteCommand("CREATE TABLE 'HueBridge' ( 'key'  TEXT ); ", DBConnect).ExecuteNonQuery();
				new SQLiteCommand("CREATE TABLE 'DrinkingRules' ( 'GameName'  TEXT, 'RuleText' Text, 'isShot' bit); ", DBConnect).ExecuteNonQuery();
			}
			else
			{
				DBConnect = new SQLiteConnection("Data Source=ColorBot.sqlite");
				DBConnect.Open();
			}

			window = new MainWindow();
			window.Show();
			test = new TestWindow();
			test.Show();
			Controls controls = new Controls();
			controls.Show();

			pulse.Elapsed += PulseHit;
			pulse.Start();
			CountDownStart = DateTime.Now;

			BackgroundWorker bw = new BackgroundWorker();
			bw.DoWork += async (object o, DoWorkEventArgs e) =>
			{
				IBridgeLocator locator = new HttpBridgeLocator();
				List<LocatedBridge> bridgeIPs = new List<LocatedBridge>();
				foreach(LocatedBridge lb in await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)))
				{
					bridgeIPs.Add(lb);
					avaialableBridges.Add(new LocalHueClient(lb.IpAddress));
				}
				SQLiteDataReader reader = new SQLiteCommand("Select * from HueBridge;", DBConnect).ExecuteReader();
				List<String> ApiKeys = new List<String>();
				while (reader.Read())
				{
					ApiKeys.Add(reader["key"].ToString());
				}
				for(int j = 0; j < avaialableBridges.Count; j++)
				{
					foreach(String u in ApiKeys)
					{
						try
						{
							avaialableBridges[j].Initialize(u);
							Bridge br = await avaialableBridges.First().GetBridgeAsync();
							break;
						}
						catch
						{
							avaialableBridges[j] = new LocalHueClient(bridgeIPs[j].IpAddress);
						}
					}
					if (!await avaialableBridges[j].CheckConnection())
					{
						RetryHueTimer timer = new RetryHueTimer();
						timer.index = j;
						timer.Interval = 500;
						timer.Elapsed += async (object sender, ElapsedEventArgs ev) =>
						{
							RetryHueTimer ii = (RetryHueTimer)sender;
							try
							{
								String ApiKey = await avaialableBridges[ii.index].RegisterAsync("ColorBot", "ColorBot");
								new SQLiteCommand("insert into 'HueBridge' (key) values ('" + ApiKey + "')", DBConnect).ExecuteNonQuery();
								avaialableBridges[ii.index].Initialize(ApiKey);
								ii.Stop();
								ii.Dispose();
							}
							catch
							{
								Console.WriteLine("excepted");
							}
						};
						timer.Start();
					}
				}
			};
			bw.RunWorkerAsync();
		}

		private void TwitchClient_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
		{
			if(e.ChatMessage.Message[0] == '!')
			{
				twitchClient.SendMessage(e.ChatMessage.Channel, LogColor(e.ChatMessage.Message.Substring(1)));
			}
			
		}

		private void TwitchClient_OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
		{
			twitchClient.SendMessage(e.Channel, "Colorbot joined");
		}

		DateTime sinceOneHit = DateTime.Now;
		DateTime sinceReset = DateTime.Now;
		private void PulseHit(object o, ElapsedEventArgs e)
		{
			if (Math.Floor((DateTime.Now - sinceOneHit).TotalSeconds) >= 1)
			{
				sinceOneHit = DateTime.Now;
				TimeUpdate timeUpdate = new TimeUpdate()
				{
					ratioToShow = (DateTime.Now - sinceReset).TotalSeconds / TimerLengthInSeconds,
					timeToShow = sinceReset.AddSeconds(TimerLengthInSeconds) - DateTime.Now
				};
				OneSecondUpdate?.Invoke(this, timeUpdate);
			}

			if ((DateTime.Now - sinceReset).TotalSeconds >= TimerLengthInSeconds)
			{
				sinceReset = DateTime.Now;
				DoColorChange();
				colorCounts = new Dictionary<String, ColorCount>();
				ResetTimerElapsed?.Invoke(this, TimeSpan.FromSeconds(TimerLengthInSeconds));
			}
		}
		public void DoColorChange()
		{
			List<ColorCount> colors = new List<ColorCount>();
			foreach (KeyValuePair<string, ColorCount> kvp in colorCounts)
			{
				colors.Add(kvp.Value);
			}

			ColorCount ctp = colors.OrderByDescending(ccc => ccc.VoteCount).FirstOrDefault();
			Color c;
			if (ctp != null)
			{
				c = ctp.color;

				LightCommand command = new LightCommand();
				command.Brightness = 100;
				command.TurnOn().SetColor(new RGBColor(c.ScR * 100, c.ScG * 100, c.ScB * 100));

				foreach (ILocalHueClient Hue in avaialableBridges)
				{
					Hue.SendCommandAsync(command);
				}
			}
		}
		public String LogColor(String ColorName)
		{
			ColorName = ColorName.ToLower().Replace(" ", "");

			if (colorCounts.ContainsKey(ColorName))
			{
				colorCounts[ColorName].VoteCount++;
			}
			else
			{
				try
				{
					if (ColorName.ToLower() == "lawngreen")
					{
						throw new Exception("Reserved color");
					}
					colorCounts.Add(ColorName, new ColorCount()
					{
						color = (Color)ColorConverter.ConvertFromString(ColorName),
						VoteCount = 1
					});
				}
				catch
				{
					return "Sorry, bruh. I dont know that one. I know that one.";
				}
			}
			VoteDictChanged?.Invoke(this, new EventArgs());
			return "Noice. I know that one.";
		}
		public void AddRule(String GameName, String RuleText, bool isShot)
		{
			new SQLiteCommand(String.Format("insert into 'DrinkingRules' (GameName, RuleText, isShot) values ('{0}', '{1}', {2})", GameName, RuleText, isShot ? 1 : 0), DBConnect).ExecuteNonQuery();
		}
		public List<DrinkingRule> GetAllRules(String GameName)
		{
			SQLiteDataReader reader = new SQLiteCommand(String.Format("Select * from DrinkingRules Where GameName = '{0}';", GameName), DBConnect).ExecuteReader();
			List<DrinkingRule> Rules = new List<DrinkingRule>();
			while (reader.Read())
			{
				DrinkingRule d = new DrinkingRule();
				d.GameName = reader["GameName"].ToString();
				d.RuleText = reader["RuleText"].ToString();
				d.isShot = (bool)reader["isShot"];
				Rules.Add(d);
			}
			return Rules;
		}
		public void ResetColorDict()
		{
			colorCounts = new Dictionary<String, ColorCount>();
			VoteDictChanged?.Invoke(this, new EventArgs());
		}
		public class DrinkingRule
		{
			public String GameName { get; set; }
			public String RuleText { get; set; }
			public bool isShot { get; set; }
		}
		public class ColorCount
		{
			public Color color { get; set; }
			public int VoteCount { get; set; }
			public DateTime Birthdate { get; set; }
			public ColorCount()
			{
				color = Colors.White;
				VoteCount = 0;
				Birthdate = DateTime.Now;
			}
		}
		public class TimeUpdate
		{
			public TimeSpan timeToShow { get; set; }
			private double _ratioToShow = 0;
			public double ratioToShow
			{
				get
				{
					return _ratioToShow;
				}
				set
				{
					if(value > 1)
					{
						_ratioToShow = 1;
					}
					else if (value < 0)
					{
						_ratioToShow = 0;
					}
					else
					{
						_ratioToShow = value;
					}
				}
			}
		}
		public class RetryHueTimer : Timer
		{
			public int index { get; set; }
			public RetryHueTimer() : base()
			{

			}
		}
	}
}
