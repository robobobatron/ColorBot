using ColorBotCore.Views;
using Microsoft.Extensions.Configuration;
using Q42.HueApi.Interfaces;
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace ColorBotCore.Model;

public class TwitchSender
{
	private TwitchClient twitchClient = new();
	private TwitchPubSub twitchPubSub = new();
	
	private readonly MainWindow _mainWindow;
	private readonly RulesWindow _rulesWindow;
	private readonly IConfiguration config; 

	public TwitchSender(IConfiguration configuration, MainWindow mw, RulesWindow rw) 
	{
		_mainWindow = mw;
		_rulesWindow = rw;
		config = configuration;

		twitchClient.Initialize(new ConnectionCredentials(configuration["CustomRobotName"], configuration["TwitchApiKey"]), configuration["YourChannelName"]);
		twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
		twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		twitchClient.OnChannelStateChanged += TwitchClient_OnChannelStateChanged;
		twitchClient.Connect();
		
		twitchPubSub.OnPubSubServiceConnected += TwitchPubSub_OnPubSubServiceConnected;
		twitchPubSub.OnBitsReceived += TwitchPubSub_OnBitsReceived;
		twitchPubSub.Connect();
	}

	private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		twitchClient.SendMessage(e?.Channel, $"{config["CustomRobotName"]} joined");
	}

	private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		if (e.Command.CommandText.ToLower() == "color")
		{
			twitchClient.SendMessage(e.Command.ChatMessage.Channel, _mainWindow.LogColor(e.Command.ArgumentsAsString));
		}
		else if (e.Command.CommandText.ToLower() == "addrule")
		{

		}
		else
		{
			twitchClient.SendMessage(e.Command.ChatMessage.Channel, "Sorry! I dont seem to know this command.");
		}
	}

	private void TwitchClient_OnChannelStateChanged(object sender, OnChannelStateChangedArgs e)
	{

	}



	private void TwitchPubSub_OnBitsReceived(object sender, TwitchLib.PubSub.Events.OnBitsReceivedArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		twitchClient.SendMessage(e.ChannelId, $"DUDE! {e.Username} just donated {e.BitsUsed} bits! They have donated {e.TotalBitsUsed} bits so far");
		twitchClient.SendMessage(e.ChannelId, "Did you know that you can use your bits to buy rules additions?");
		twitchClient.SendMessage(e.ChannelId, "just type !addrule followed by your desired rule followed by \"shot\" or \"drink\"!");
	}

	private void TwitchPubSub_OnPubSubServiceConnected(object sender, EventArgs e)
	{
		TwitchPubSub twitchPubSub = (TwitchPubSub)sender;
		twitchPubSub.ListenToBitsEventsV2(config["YourChannelName"]);
	}

	
}
