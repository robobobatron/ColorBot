using System.IO;
using System.Windows;
using ColorBotCore.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ColorBotCore.Model;
using Microsoft.EntityFrameworkCore;

namespace ColorBotCore;

public partial class App : Application
{
	public static IConfiguration Config { get; private set; }

	public App()
	{
		ServiceCollection services = new();
		services.AddTransient<IConfiguration>(sp =>
		{
			IConfigurationBuilder configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
			return configuration.Build();
		});
		services.AddDbContext<ColorbotDB>(options => { options.UseSqlite("Data Source=ColorBot.sqlite"); });
		services.AddSingleton<MainWindow>();
		services.AddSingleton<TestWindow>();
		services.AddSingleton<RulesWindow>();
		services.AddSingleton<Controls>();
		services.AddSingleton<BridgeCollection>();
		services.AddSingleton<TwitchSender>();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		ColorbotDB db = serviceProvider.GetService<ColorbotDB>();
		db.Database.Migrate();

		serviceProvider.GetRequiredService<MainWindow>().Show();
		serviceProvider.GetRequiredService<Controls>().Show();

		serviceProvider.GetRequiredService<TwitchSender>();
	}
}
