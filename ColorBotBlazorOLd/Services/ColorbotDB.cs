using ColorBotBlazor.Model.Database;
using Microsoft.EntityFrameworkCore;

namespace ColorBotBlazor.Model;

public class ColorbotDB : DbContext
{
	public DbSet<BitCounts> Bit { get; set; } 
	public DbSet<DrinkingRules> Rules { get; set; }
	public DbSet<HueBridge> Bridges { get; set; }

	public ColorbotDB(DbContextOptions<ColorbotDB> options) : base(options)
	{
		Database.Migrate();
		Database.EnsureCreated();
	}
}
