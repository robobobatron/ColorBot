using ColorBotBlazor;
using ColorBotBlazor.Model;
using ColorBotBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddDbContext<ColorbotDB>(options => { options.UseSqlite("Data Source=ColorBot.sqlite"); });

builder.Services.AddSingleton<BridgeCollection>();
builder.Services.AddSingleton<ColorbotDB>();
builder.Services.AddSingleton<TwitchSender>();

await builder.Build().RunAsync();
