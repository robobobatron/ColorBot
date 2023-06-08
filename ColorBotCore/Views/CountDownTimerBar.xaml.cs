using ColorBotCore.Model.Viewmodel;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorBotCore.Views;

/// <summary>
/// Interaction logic for CountDownTimerBar.xaml
/// </summary>
public partial class CountDownTimerBar : UserControl
{
	public TimeSpan TimerLength { get; set; }

	public CountDownTimerBar()
	{
		InitializeComponent();
		timerText.Text = TimerLength.ToString(@"mm\:ss");
	}

	public void TimerBarWidthChange(TimeUpdate TU)
	{
		innerBorder.Width = outerBorder.ActualWidth - (outerBorder.ActualWidth * TU.ratioToShow);
		timerText.Text = TimeSpan.FromSeconds(Math.Ceiling(TU.timeToShow.TotalSeconds)).ToString(@"mm\:ss");
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
		if (colorToChangeTo != ((SolidColorBrush)innerBorder.Background).Color)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				((SolidColorBrush)innerBorder.Background).Color = colorToChangeTo;
				((SolidColorBrush)outerBorder.BorderBrush).Color = colorToChangeTo;
			}));
		}
	}
}
