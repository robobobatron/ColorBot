using ColorBotBlazor.Model;
using ColorBotBlazorNew.Services;
using Microsoft.AspNetCore.Components;

namespace ColorBotBlazorNew.Pages;

public partial class Index : ComponentBase
{
	[Inject]
	public TwitchSender TwitchSender { get; set; }
	//[Inject]
	//public ColorbotDB DB { get; set; }
	//[Inject]
	//public BridgeCollection BridgeCollection { get; set; }

	public Index()
	{

	}

}
