using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorBotCore.Model.Database;

[Keyless]
public class DrinkingRules
{
	public string GameName { get; set; }
	public string RuleText { get; set; }
	public bool isShot { get; set; }
}
