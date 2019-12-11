using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorBot
{
	public partial class MainWindow : Window
    {
		private readonly App AppLevel = Application.Current as App;
        public MainWindow()
        {
            InitializeComponent();
			AppLevel.VoteDictChanged += DoListRefresh;
			AppLevel.OneSecondUpdate += timeBar.TimerBarWidthChange;
			AppLevel.ResetTimerElapsed += timeBar.TimerBarReset;
			AppLevel.ResetTimerElapsed += DoReset;
			timeBar.TimerLength = TimeSpan.FromSeconds(AppLevel.TimerLengthInSeconds);
        }
		private void DoListRefresh(object sender, EventArgs e)
		{
			this.Dispatcher.Invoke(() =>
			{
				ColorSquareStack.Children.Clear();
				List<App.ColorCount> colorCounts = AppLevel.colorCounts.Values.ToList();
				colorCounts = colorCounts.OrderByDescending(cc => cc.Birthdate).ToList();
				colorCounts = colorCounts.OrderByDescending(cc => cc.VoteCount).ToList();
				colorCounts = colorCounts.Take(3).ToList();

				timeBar.ChangeColor(colorCounts.First().color);

				foreach (App.ColorCount cc in colorCounts)
				{
					ColorSquare colorSquare = new ColorSquare();
					colorSquare.Numeral.Text = cc.VoteCount.ToString();
					colorSquare.TopLevelBorder.BorderBrush = new SolidColorBrush(cc.color);
					colorSquare.Height = ActualWidth;
					ColorSquareStack.Children.Add(colorSquare);
				}
			});
		}
		private void DoReset(object sender, TimeSpan dateTime)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				timeBar.ChangeColor(Colors.Gray);
				ColorSquareStack.Children.Clear();
			}));
		}
	}
}
