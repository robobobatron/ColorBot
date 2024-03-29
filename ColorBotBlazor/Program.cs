using ColorBotBlazor;
using ColorBotBlazor.Model;
using ColorBotBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddDbContext<ColorbotDB>(options => { options.UseSqlite("Data Source=ColorBot.sqlite"); });

builder.Services.AddSingleton<BridgeCollection>();
builder.Services.AddSingleton<ColorbotDB>();
builder.Services.AddSingleton<TwitchSender>();

var app = builder.Build();
