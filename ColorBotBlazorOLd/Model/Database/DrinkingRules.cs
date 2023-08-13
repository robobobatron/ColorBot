using Microsoft.EntityFrameworkCore;

namespace ColorBotBlazor.Model.Database;

[Keyless]
public class DrinkingRules
{
	public string GameName { get; set; }
	public string RuleText { get; set; }
	public bool isShot { get; set; }
}
