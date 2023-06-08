using System.Windows;
using System.Windows.Controls;

namespace ColorBotCore.Views;

/// <summary>
/// Interaction logic for Controls.xaml
/// </summary>
public partial class Controls : Window
{
	private readonly TestWindow _testWindow;
	private readonly RulesWindow _rulesWindow;
	public Controls(TestWindow t, RulesWindow r)
	{
		_testWindow = t;
		_rulesWindow = r;
		InitializeComponent();
	}
	private void ToggleDrunkRules(object sender, RoutedEventArgs e)
	{
		Button button = (Button)sender;
		if (_rulesWindow.IsVisible) _rulesWindow .Hide();
		else _rulesWindow.Show();
		button.Content = _rulesWindow.IsVisible ? "Showing" : "Hidden";
	}
	private void ToggleTestWindow(object sender, RoutedEventArgs e)
	{
		Button button = (Button)sender;
		if (_testWindow.IsVisible) _testWindow.Hide();
		else _testWindow.Show();
		button.Content = _testWindow.IsVisible ? "Showing" : "Hidden";
	}
}
