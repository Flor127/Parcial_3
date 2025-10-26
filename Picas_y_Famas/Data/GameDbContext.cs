using Microsoft.EntityFrameworkCore;
using Picas_y_Famas.Models;

namespace Picas_y_Famas.Data
{
        public class GameDbContext : DbContext
        {
            public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

            public DbSet<Player> Players { get; set; }
            public DbSet<Game> Games { get; set; }
            public DbSet<Attempt> Attempts { get; set; }
        }
    
}