using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Gamut;
using Q42.HueApi.ColorConverters.HSB;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Api;
using TwitchLib.PubSub;
using ColorBotCore.Views;
using Microsoft.Extensions.Configuration;
using TwitchLib.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using ColorBotCore.Model;
using Microsoft.EntityFrameworkCore;
using ColorBotCore.Model.Database;
using ColorBotCore.Model.Viewmodel;

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
		services.AddSingleton<Controls>();
		services.AddSingleton<BridgeCollection>();
		services.AddSingleton<TwitchSender>();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		ColorbotDB db = serviceProvider.GetService<ColorbotDB>();
		db.Database.Migrate();

		serviceProvider.GetRequiredService<MainWindow>().Show();
		serviceProvider.GetRequiredService<TestWindow>().Show();
		serviceProvider.GetRequiredService<Controls>().Show();

		serviceProvider.GetRequiredService<TwitchSender>();
	}
}
