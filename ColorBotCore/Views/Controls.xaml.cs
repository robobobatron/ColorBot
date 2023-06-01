using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
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

namespace ColorBotCore.Views;

/// <summary>
/// Interaction logic for Controls.xaml
/// </summary>
public partial class Controls : Window
{
	private readonly TestWindow _testWindow;
	public Controls(TestWindow t)
	{
		_testWindow = t;
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
		if (_testWindow.IsVisible)
		{
			_testWindow.Hide();
			button.Content = "Hidden";
		}
		else
		{
			_testWindow.Show();
			button.Content = "Showing";
		}

	}
}
