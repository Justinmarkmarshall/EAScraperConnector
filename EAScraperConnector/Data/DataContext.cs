using EAScraperConnector.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EAScraperConnector.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }

        public DbSet<Audit> Audit { get; set; }
    }
}
