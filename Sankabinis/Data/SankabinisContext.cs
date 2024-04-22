using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Models;

namespace Sankabinis.Data
{
    public class SankabinisContext : DbContext
    {
        public SankabinisContext (DbContextOptions<SankabinisContext> options)
            : base(options)
        {
        }

        public DbSet<Sankabinis.Models.Automobilis> Automobilis { get; set; } = default!;
    }
}
