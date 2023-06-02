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

namespace ColorBotCore.Views
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
		MainWindow _mainWindow;

		public TestWindow(MainWindow mw)
        {
            InitializeComponent();
			_mainWindow = mw;

		}

		private void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (DebugWindow.Text.First() == '!')
				{

				}
				else if (DebugWindow.Text.First() == '@')
				{

				}
				else
				{
					_mainWindow.LogColor(DebugWindow.Text);
				}

				DebugWindow.Text = "";
			}
		}
	}
}
