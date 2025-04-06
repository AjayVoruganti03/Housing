using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
namespace WebAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
          
        }

        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;
    }
    
}
