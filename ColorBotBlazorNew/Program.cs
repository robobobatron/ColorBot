using ColorBotBlazor.Model;
using ColorBotBlazorNew;
using ColorBotBlazorNew.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

builder.Services.AddDbContext<ColorbotDB>(options => { options.UseSqlite("Data Source=ColorBot.sqlite"); });
ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
ColorbotDB db = serviceProvider.GetService<ColorbotDB>();
db.Database.Migrate();
builder.Services.AddScoped<ColorbotDB>();
builder.Services.AddScoped<BridgeCollection>();
builder.Services.AddScoped<TwitchSender>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>();

app.UseWebSockets();

app.Run();
