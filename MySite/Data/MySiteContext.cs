using Microsoft.EntityFrameworkCore;
using MySite.Models;

namespace MySite.Data
{
    public class MySiteContext:DbContext
    {
        public MySiteContext(DbContextOptions options):base(options) {
        
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Links> Links { get; set; }
    }
}
