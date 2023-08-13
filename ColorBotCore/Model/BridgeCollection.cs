﻿using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Gamut;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.ColorConverters.Original;
using System;
using System.Collections.Generic;
using System.Linq;
using ColorBotCore.Model.Database;
using System.Windows.Media;

namespace ColorBotCore.Model;

public class BridgeCollection
{
	private List<LocalHueClient> avaialableBridges = new();

	public BridgeCollection(ColorbotDB db)
	{
		//foreach(var hc in new HttpBridgeLocator().LocateBridgesAsync(TimeSpan.FromSeconds(5)).Result.ToList())
		//{
		//	LocalHueClient lhc = new(hc.IpAddress);

		//	String ApiKey = lhc.RegisterAsync("ColorBot", "ColorBot").Result;
		//	lhc.Initialize(ApiKey);
		//	if (!db.Bridges.Any(x => x.Key == ApiKey))
		//	{
		//		db.Bridges.Add(new HueBridge() { IP = hc.IpAddress, Key = ApiKey });
		//	}
		//	avaialableBridges.Add(lhc);
		//}

		List<HueBridge> OldBridges =  db.Bridges.ToList();
		foreach (HueBridge ob in OldBridges)
		{
			LocalHueClient lhc = new(ob.IP);
			lhc.Initialize(ob.Key);
			avaialableBridges.Add(lhc);
		}
		db.SaveChanges();
	}

	public void DoColorChange(Color c)
	{
		LightCommand command = new();
		command.Brightness = 100;
		command.TurnOn().SetColor(new RGBColor(c.ScR * 100, c.ScG * 100, c.ScB * 100));

		foreach (LocalHueClient Hue in avaialableBridges)
		{
			Hue.SendCommandAsync(command);
		}
	}
}
