using System.Linq;
using System.Windows;
using System.Windows.Input;

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
