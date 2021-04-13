using Bagual.Services.Shithead.Models;
using Microsoft.EntityFrameworkCore;

namespace Bagual.Services.Shithead.DataContext
{
    public class ShitheadDBContext : DbContext
    {
        public ShitheadDBContext(DbContextOptions<ShitheadDBContext> options)  : base(options) { }

        public ShitheadDBContext() : base() { }

        public DbSet<GameDbModel> ShitheadGames { get; set; }
    }
}
