using EAScraperConnector.Dtos;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace EAScraperConnector.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Property> Properties { get; set; }

        public DbSet<Audit> Audit { get; set; }
        public DbSet<Log> Log { get; set; }
    }
}
