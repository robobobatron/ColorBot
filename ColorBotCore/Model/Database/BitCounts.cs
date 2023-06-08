using Microsoft.EntityFrameworkCore;

namespace ColorBotCore.Model.Database;

[Keyless]
public class BitCounts
{
	public string UserID { get; set; }
}
