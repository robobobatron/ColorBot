using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.PubSub;
using TwitchLib.Api;

namespace ColorBotBlazor.Services;

public class TwitchSender
{
	private TwitchAPI twitchAPI = new();
	private TwitchClient twitchClient = new();
	private TwitchPubSub twitchPubSub = new();

	private readonly IConfiguration config;

	public TwitchSender(IConfiguration configuration)
	{
		try
		{
			config = configuration;
			var a = configuration["CustomRobotName"];
			var b = configuration["TwitchApiKey"];
			var c = configuration["YourChannelName"];
			twitchClient.Initialize(new ConnectionCredentials(a, b), c);
			twitchClient.OnJoinedChannel += TwitchClient_OnJoinedChannel;
			twitchClient.OnChatCommandReceived += TwitchClient_OnChatCommandReceived;
			twitchClient.Connect();

			twitchPubSub.OnPubSubServiceConnected += TwitchPubSub_OnPubSubServiceConnected;
			twitchPubSub.OnBitsReceived += TwitchPubSub_OnBitsReceived;
			twitchPubSub.Connect();
		}
		catch
		{

		}
		
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
			//twitchClient.SendMessage(e.Command.ChatMessage.Channel, _mainWindow.LogColor(e.Command.ArgumentsAsString));
		}
		else if (e.Command.CommandText.ToLower() == "rules")
		{
			string GameName = e.Command.ArgumentsAsList.FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(GameName))
			{

			}
			else
			{
				twitchClient.SendMessage(e.Command.ChatMessage.Channel, "Sorry! You must specify a game.");
			}
		}
		else if (e.Command.CommandText.ToLower() == "addrule")
		{

		}
		else
		{
			twitchClient.SendMessage(e.Command.ChatMessage.Channel, "Sorry! I dont seem to know this command.");
		}
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
