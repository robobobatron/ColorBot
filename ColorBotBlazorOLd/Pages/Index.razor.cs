using ColorBotBlazor.Model;
using ColorBotBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace ColorBotBlazor.Pages;

public partial class Index : ComponentBase
{
	[Inject]
	public TwitchSender TwitchSender { get; set; }
	//[Inject]
	//public ColorbotDB DB { get; set; }
	//[Inject]
	//public BridgeCollection BridgeCollection { get; set; }

}
