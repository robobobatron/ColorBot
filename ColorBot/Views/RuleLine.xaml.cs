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
	/// <summary>
	/// Interaction logic for RuleLine.xaml
	/// </summary>
	public partial class RuleLine : UserControl
	{
		public RuleLine(int Number, String Text)
		{
			InitializeComponent();
			RuleNumber.Text = Number.ToString();
			RuleText.Text = Text;
		}
	}
}
