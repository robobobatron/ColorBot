using Microsoft.EntityFrameworkCore;

namespace ColorBotCore.Model.Database;

[Keyless]
public class DrinkingRules
{
	public string GameName { get; set; }
	public string RuleText { get; set; }
	public bool isShot { get; set; }
}
