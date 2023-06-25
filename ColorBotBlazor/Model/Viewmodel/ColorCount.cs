using System;
using System.Drawing;

namespace ColorBotCore.Model.Viewmodel;

public class ColorCount
{
	public Color color { get; set; }
	public int VoteCount { get; set; }
	public DateTime Birthdate { get; set; }
	public ColorCount()
	{
		color = Color.White;
		VoteCount = 0;
		Birthdate = DateTime.Now;
	}
}
