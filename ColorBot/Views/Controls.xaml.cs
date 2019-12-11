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
using System.Windows.Shapes;

namespace ColorBot
{
	public partial class Controls : Window
	{
		private App AppLevel = (App)Application.Current;

		public Controls()
		{
			InitializeComponent();
		}
		private void ToggleDrunkRules(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			RulesWindow rw = new RulesWindow();
			rw.Show();
		}
		private void ToggleTestWindow(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			if (AppLevel.test.IsVisible)
			{
				AppLevel.test.Hide();
				button.Content = "Hidden";
			}
			else
			{
				AppLevel.test.Show();
				button.Content = "Showing";
			}

		}
	}
}
