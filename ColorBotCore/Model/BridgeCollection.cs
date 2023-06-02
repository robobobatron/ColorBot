using ColorBotCore.Model.Viewmodel;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Gamut;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ColorBotCore.Model.Database;
using System.Windows.Media;

namespace ColorBotCore.Model;

public class BridgeCollection
{
	private List<ILocalHueClient> avaialableBridges = new();

	public BridgeCollection(ColorbotDB db)
	{
		IBridgeLocator locator = new HttpBridgeLocator();

		List<LocatedBridge> bridgeIPs = new();
		foreach (LocatedBridge lb in locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult().ToList())
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
					Bridge br = avaialableBridges.First().GetBridgeAsync().GetAwaiter().GetResult();
					break;
				}
				catch
				{
					avaialableBridges[j] = new LocalHueClient(bridgeIPs[j].IpAddress);
				}
			}
			if (!avaialableBridges[j].CheckConnection().GetAwaiter().GetResult())
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
						if (!db.Bridges.Any(x => x.Key == ApiKey))
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
	}

	public void DoColorChange(Color c)
	{
		LightCommand command = new();
		command.Brightness = 100;
		command.TurnOn().SetColor(new RGBColor(c.ScR * 100, c.ScG * 100, c.ScB * 100));

		foreach (ILocalHueClient Hue in avaialableBridges)
		{
			Hue.SendCommandAsync(command);
		}
	}
}
