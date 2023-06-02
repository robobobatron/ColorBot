using Microsoft.Extensions.Configuration;
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
	private MainWindow _mainWindow;

	public TwitchSender(IConfiguration configuration, MainWindow mw) 
	{
		_mainWindow = mw;

		twitchClient.Initialize(new ConnectionCredentials("huecolorbot", configuration["TwitchApiKey"]), "Robobobatron");
		twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
		twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
		twitchClient.Connect();
		
		twitchPubSub.OnPubSubServiceConnected += TwitchPubSub_OnPubSubServiceConnected;
		twitchPubSub.OnBitsReceived += TwitchPubSub_OnBitsReceived;
		twitchPubSub.Connect();
	}
	private void TwitchClient_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		if (e.Command.CommandText.ToLower() == "color")
		{
			twitchClient.SendMessage(e.Command.ChatMessage.Channel, _mainWindow.LogColor(e.Command.ArgumentsAsString));
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

	private void TwitchPubSub_OnPubSubServiceConnected(object sender, EventArgs e)
	{
		TwitchPubSub twitchPubSub = (TwitchPubSub)sender;
		twitchPubSub.ListenToBitsEventsV2("Robobobatron");
	}

	private void TwitchClient_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
	{
		TwitchClient twitchClient = (TwitchClient)sender;
		twitchClient.SendMessage(e?.Channel, "Colorbot joined");
	}
}
