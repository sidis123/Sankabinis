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

        public DbSet<Sankabinis.Models.Car> Car { get; set; } = default!;
        public DbSet<User> Users { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<Distance> Distance { get; set; }
        public DbSet <Race> Race { get; set; }
        public DbSet <Track> Track {get; set; }
    }
}
