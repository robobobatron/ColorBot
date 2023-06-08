using ColorBotCore.Model.Viewmodel;
using ColorBotCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ColorBotCore.Model;
using System.Timers;
using Microsoft.Extensions.Configuration;

namespace ColorBotCore
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public List<ColorCount> colorCounts = new();
		public BridgeCollection _bridgeCollection;

		private int TimerLengthInSeconds = 90;
		private DateTime sinceOneHit = DateTime.Now;
		private DateTime sinceReset = DateTime.Now;

		public Timer pulse = new() { Interval = 250 };

		public MainWindow(BridgeCollection bs, IConfiguration configuration)
		{
			InitializeComponent();
			TimerLengthInSeconds = int.Parse(configuration["TimerLength"]);
			_bridgeCollection = bs;
			timeBar.TimerLength = TimeSpan.FromSeconds(TimerLengthInSeconds);
			pulse.Elapsed += PulseHit;
			pulse.Start();
		}

		private void PulseHit(object o, ElapsedEventArgs e)
		{
			if (colorCounts.Count > 0)
			{
				Application.Current.Dispatcher.Invoke(new Action(() => timeBar.ChangeColor(colorCounts.First().color)));
			}

			if (Math.Floor((DateTime.Now - sinceOneHit).TotalSeconds) >= 1)
			{
				sinceOneHit = DateTime.Now;
				Application.Current.Dispatcher.Invoke(new Action(() => timeBar.TimerBarWidthChange(new TimeUpdate() { ratioToShow = (DateTime.Now - sinceReset).TotalSeconds / TimerLengthInSeconds, timeToShow = sinceReset.AddSeconds(TimerLengthInSeconds) - DateTime.Now })));
			}

			if ((DateTime.Now - sinceReset).TotalSeconds >= TimerLengthInSeconds)
			{
				sinceReset = DateTime.Now;
				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					if (colorCounts.Count > 0)
					{
						_bridgeCollection.DoColorChange(colorCounts.First().color);
					}
					colorCounts = new();
					timeBar.ChangeColor(Colors.Gray);
					ColorSquareStack.Children.Clear();
				}));
			}
		}

		public String LogColor(String ColorName)
		{
			ColorCount c = colorCounts.Where(x => x.color == (Color)ColorConverter.ConvertFromString(ColorName.ToLower().Replace(" ", ""))).FirstOrDefault();

			if (c != null)
			{
				c.VoteCount++;
			}
			else
			{
				try
				{
					if (ColorName.ToLower() == "lawngreen")
					{
						throw new Exception("Reserved color");
					}
					colorCounts.Add(new ColorCount()
					{
						color = (Color)ColorConverter.ConvertFromString(ColorName),
						VoteCount = 1
					});
				}
				catch
				{
					return "Sorry, bruh. I dont know that one. I know that one.";
				}
			}

			Application.Current.Dispatcher.Invoke(new Action(() => ColorSquareStack.Children.Clear()));
			colorCounts = colorCounts.OrderByDescending(cc => cc.Birthdate).ToList();
			colorCounts = colorCounts.OrderByDescending(cc => cc.VoteCount).ToList();

			foreach (ColorCount cc in colorCounts.Take(3).ToList())
			{
				Application.Current.Dispatcher.Invoke(new Action(() => {
					ColorSquare colorSquare = new ColorSquare();
					colorSquare.Numeral.Text = cc.VoteCount.ToString();
					colorSquare.TopLevelBorder.BorderBrush = new SolidColorBrush(cc.color);
					colorSquare.Height = ActualWidth;
					ColorSquareStack.Children.Add(colorSquare);
				}));
			}
			return "Noice. I know that one.";
		}
	}
}
