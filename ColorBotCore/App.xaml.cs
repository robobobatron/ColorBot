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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Api;
using TwitchLib.PubSub;
using ColorBotCore.Views;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using ColorBotCore.Model;
using Microsoft.EntityFrameworkCore;
using ColorBotCore.Model.Database;
using ColorBotCore.Model.Viewmodel;

namespace ColorBotCore;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public event EventHandler VoteDictChanged;
	public event EventHandler<TimeSpan> ResetTimerElapsed;
	public event EventHandler<TimeUpdate> OneSecondUpdate;

	public Dictionary<String, ColorCount> colorCounts = new Dictionary<String, ColorCount>();

	public DateTime CountDownStart = new();
	public readonly int TimerLengthInSeconds = 90;

	public Timer pulse = new()
	{
		Interval = 250
	};

	private List<ILocalHueClient> avaialableBridges = new();

	public App()
	{
		IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

		ServiceCollection services = new ServiceCollection();
		services.AddDbContext<ColorbotDB>(options =>
		{
			options.UseSqlite("Data Source=ColorBot.sqlite");
		});
		services.AddSingleton<TwitchClient>();
		services.AddSingleton<TwitchPubSub>();
		services.AddSingleton<List<ILocalHueClient>>();
		services.AddSingleton<MainWindow>();
		services.AddSingleton<TestWindow>();
		services.AddSingleton<Controls>();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		ColorbotDB db = serviceProvider.GetService<ColorbotDB>();
		db.Database.Migrate();

		TwitchClient twitchClient = serviceProvider.GetRequiredService<TwitchClient>();
		twitchClient.Initialize(new ConnectionCredentials("huecolorbot", configuration["TwitchApiKey"]), "Robobobatron");
		twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
		//twitchClient.OnMessageReceived += TwitchClient_OnMessageReceived;
		twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		twitchClient.Connect();

		TwitchPubSub twitchPubSub = serviceProvider.GetRequiredService<TwitchPubSub>();
		twitchPubSub.OnPubSubServiceConnected += TwitchPubSub_OnPubSubServiceConnected;
		twitchPubSub.OnBitsReceived += TwitchPubSub_OnBitsReceived;
		twitchPubSub.Connect();

		serviceProvider.GetRequiredService<MainWindow>().Show();
		serviceProvider.GetRequiredService<TestWindow>().Show();
		serviceProvider.GetRequiredService<Controls>().Show();

		pulse.Elapsed += PulseHit;
		pulse.Start();
		CountDownStart = DateTime.Now;

		BackgroundWorker bw = new();
		bw.DoWork += async (object o, DoWorkEventArgs e) =>
		{
			IBridgeLocator locator = new HttpBridgeLocator();
			List<LocatedBridge> bridgeIPs = new();
			foreach (LocatedBridge lb in await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)))
			{
				bridgeIPs.Add(lb);
				avaialableBridges.Add(new LocalHueClient(lb.IpAddress));
			}

			List<String> ApiKeys = db.Bridges.Select(x => x.Key).ToList();

			for (int j = 0; j < avaialableBridges.Count; j++)
			{
				foreach (String u in ApiKeys)
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
							if(!db.Bridges.Any(x => x.Key == ApiKey))
							{
								db.Bridges.Add(new HueBridge() { Key = ApiKey });
							}
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

	private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs? e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		if (e?.Command.CommandText.ToLower() == "color")
		{
			twitchClient.SendMessage(e.Command.ChatMessage.Channel, LogColor(e.Command.ArgumentsAsString));
		}
		else if (e?.Command.CommandText.ToLower() == "addrule")
		{

		}
	}

	private void TwitchPubSub_OnBitsReceived(object sender, TwitchLib.PubSub.Events.OnBitsReceivedArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		twitchClient.SendMessage(e.ChannelId, String.Format("DUDE! {0} just donated {1} bits! They have donated {2} bits so far", e.Username, e.BitsUsed, e.TotalBitsUsed));
		twitchClient.SendMessage(e.ChannelId, "Did you know that you can use your bits to buy rules additions?");
		//e.UserId
	}

	private void TwitchPubSub_OnPubSubServiceConnected(object sender, EventArgs? e)
	{
		TwitchPubSub twitchPubSub = (TwitchPubSub)sender;
		twitchPubSub.ListenToBitsEventsV2("Robobobatron");
	}

	private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs? e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		twitchClient.SendMessage(e?.Channel, "Colorbot joined");
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

			LightCommand command = new();
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
		//new SQLiteCommand(String.Format("insert into 'DrinkingRules' (GameName, RuleText, isShot) values ('{0}', '{1}', {2})", GameName, RuleText, isShot ? 1 : 0), DBConnect).ExecuteNonQuery();
	}
	public List<DrinkingRule> GetAllRules(String GameName)
	{
		//SQLiteDataReader reader = new SQLiteCommand(String.Format("Select * from DrinkingRules Where GameName = '{0}';", GameName), DBConnect).ExecuteReader();
		List<DrinkingRule> Rules = new();
		//while (reader.Read())
		//{
		//	DrinkingRule d = new();
		//	d.GameName = reader["GameName"].ToString();
		//	d.RuleText = reader["RuleText"].ToString();
		//	d.isShot = (bool)reader["isShot"];
		//	Rules.Add(d);
		//}
		return Rules;
	}
}
