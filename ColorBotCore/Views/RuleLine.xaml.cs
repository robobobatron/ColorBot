using System.Windows.Controls;

namespace ColorBotCore.Views
{
	/// <summary>
	/// Interaction logic for RuleLine.xaml
	/// </summary>
	public partial class RuleLine : UserControl
    {
        public RuleLine(int Number, string Text)
        {
			RuleNumber.Text = Number.ToString();
			RuleText.Text = Text;
			InitializeComponent();
        }
    }
}
