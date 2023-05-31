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
    /// Interaction logic for RulesWindow.xaml
    /// </summary>
    public partial class RulesWindow : Window
    {
		private App AppLevel = (App)Application.Current;
		public RulesWindow()
        {
            InitializeComponent();
        }

		private void Window_ContentRendered(object sender, EventArgs e)
		{
			List<DrinkingRule> rules = AppLevel.GetAllRules("DMC5");
			List<DrinkingRule> shotRule = rules.Where(r => r.isShot).ToList();
			List<DrinkingRule> drinkRule = rules.Where(r => !r.isShot).ToList();
			int shot = 1;
			int drink = 1;
			foreach (DrinkingRule dr in shotRule)
			{
				RuleLine rl = new RuleLine(shot, dr.RuleText);
				rl.Height = DrinkRec.ActualHeight / shotRule.Count;
				ShotWhen.Children.Add(rl);
				shot++;
			}
			foreach (DrinkingRule dr in drinkRule)
			{
				RuleLine rl = new RuleLine(drink, dr.RuleText);
				rl.Height = DrinkRec.ActualHeight / drinkRule.Count;
				DrinkWhen.Children.Add(rl);
				drink++;
			}
		}
	}
}
