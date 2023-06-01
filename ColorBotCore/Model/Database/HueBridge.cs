using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorBotCore.Model.Database;


public class HueBridge
{
	[Key]
	public string Key { get; set; }
}
