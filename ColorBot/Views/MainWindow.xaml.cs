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
	public partial class MainWindow : Window
    {
		private App AppLevel = (App)Application.Current;
        public MainWindow()
        {
            InitializeComponent();
			
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Random random = new Random();
			for (int i = 0; i < 10; i++)
			{
				ColorSquare colorSquare = new ColorSquare()
				{
					Height = ColorSquareStack.ActualWidth,
					Width = ColorSquareStack.ActualWidth
				};
				if (i % 2 == 0)
				{
					colorSquare.TopLevelBorder.Background = Brushes.Purple;
				}
				colorSquare.Numeral.Text = random.Next(0, 20).ToString();
				ColorSquareStack.Children.Add(colorSquare);
			}
		}
		private void DoListRefresh()
		{
			foreach(KeyValuePair<String, App.ColorCount> kvp in AppLevel.colorCounts)
			{

			}
		}
	}
}
