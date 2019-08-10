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
	public partial class TestWindow : Window
	{
		private App AppLevel = (App)Application.Current;
		public TestWindow()
		{
			InitializeComponent();
		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				AppLevel.LogColor(DebugWindow.Text);
				DebugWindow.Text = "";
			}
		}
	}
}
