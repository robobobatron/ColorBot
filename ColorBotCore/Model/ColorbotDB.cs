using ColorBotCore.Model.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorBotCore.Model;

public class ColorbotDB : DbContext
{
	public DbSet<BitCounts> Bit { get; set; } 
	public DbSet<DrinkingRules> Rules { get; set; }
	public DbSet<HueBridge> Bridges { get; set; }

	public ColorbotDB(DbContextOptions<ColorbotDB> options) : base(options)
	{
		Database.EnsureCreated();
	}
}
