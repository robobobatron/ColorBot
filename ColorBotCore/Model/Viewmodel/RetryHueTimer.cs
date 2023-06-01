using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ColorBotCore.Model.Viewmodel;

public class RetryHueTimer : Timer
{
	public int index { get; set; }
	public RetryHueTimer() : base()
	{

	}
}