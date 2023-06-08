using System;

namespace ColorBotCore.Model.Viewmodel;

public class TimeUpdate
{
	public TimeSpan timeToShow { get; set; }
	private double _ratioToShow = 0;
	public double ratioToShow
	{
		get
		{
			return _ratioToShow;
		}
		set
		{
			if (value > 1)
			{
				_ratioToShow = 1;
			}
			else if (value < 0)
			{
				_ratioToShow = 0;
			}
			else
			{
				_ratioToShow = value;
			}
		}
	}
}
