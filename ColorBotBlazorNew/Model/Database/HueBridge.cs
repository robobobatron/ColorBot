using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColorBotBlazor.Model.Database;


public class HueBridge
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int ID { get; set; }
	[Required]
	public string Key { get; set; }
	[Required]
	public string IP { get; set; }
}
