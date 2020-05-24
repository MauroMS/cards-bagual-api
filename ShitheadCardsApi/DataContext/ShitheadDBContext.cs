using Microsoft.EntityFrameworkCore;
using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.DataContext
{
    public class ShitheadDBContext : DbContext
    {
        public ShitheadDBContext(DbContextOptions<ShitheadDBContext> options)  : base(options) { }

        public ShitheadDBContext() : base() { }

        public DbSet<GameDbModel> ShitheadGames { get; set; }
    }
}
