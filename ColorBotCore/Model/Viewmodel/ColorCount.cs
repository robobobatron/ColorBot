using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ColorBotCore.Model.Viewmodel;

public class ColorCount
{
	public Color color { get; set; }
	public int VoteCount { get; set; }
	public DateTime Birthdate { get; set; }
	public ColorCount()
	{
		color = Colors.White;
		VoteCount = 0;
		Birthdate = DateTime.Now;
	}
}
