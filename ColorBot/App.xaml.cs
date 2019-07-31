using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace ColorBot
{
	public partial class App : Application
	{
		public event EventHandler VoteDictChanged;

		public Dictionary<String, ColorCount> colorCounts = new Dictionary<String, ColorCount>();

		public App()
		{
			MainWindow window = new MainWindow();
			window.Show();
			TestWindow test = new TestWindow();
			test.Show();
		}
		public void LogColor(String ColorName)
		{
			if (colorCounts.ContainsKey(ColorName))
			{
				colorCounts[ColorName].VoteCount++;
			}
			else
			{
				colorCounts.Add(ColorName, new ColorCount()
				{
					color = Color.FromName(ColorName),
					VoteCount = 1
				});
			}
			VoteDictChanged?.Invoke(this, new EventArgs());
		}
		public void ResetColorDict()
		{
			colorCounts = new Dictionary<String, ColorCount>();
			VoteDictChanged?.Invoke(this, new EventArgs());
		}
		public class ColorCount
		{
			public Color color { get; set; }
			public int VoteCount { get; set; }
			public ColorCount()
			{
				color = Color.White;
				VoteCount = 0;
			}
		}
	}
}
