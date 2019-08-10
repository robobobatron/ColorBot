using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorBot
{
	public partial class CountDownTimerBar : UserControl
	{
		public TimeSpan TimerLength { get; set; }

		public CountDownTimerBar()
		{
			InitializeComponent();
			timerText.Text = TimerLength.ToString(@"mm\:ss");
		}
		public void TimerBarWidthChange(object o, App.TimeUpdate TU)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				innerBorder.Width = outerBorder.ActualWidth - (outerBorder.ActualWidth * TU.ratioToShow);
				timerText.Text = TimeSpan.FromSeconds(Math.Ceiling(TU.timeToShow.TotalSeconds)).ToString(@"mm\:ss");
			}));
		}
		public void TimerBarReset(object sender, TimeSpan resetTime)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				innerBorder.Width = outerBorder.ActualWidth;
				timerText.Text = resetTime.ToString(@"mm\:ss");
			}));
		}
		public void ChangeColor(Color colorToChangeTo)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				((SolidColorBrush)innerBorder.Background).Color = colorToChangeTo;
				((SolidColorBrush)outerBorder.BorderBrush).Color = colorToChangeTo;
			}));
		}
	}
}
