using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using TagHelpersWebSite.Models;

    public class EmployeeContext : DbContext
    {
        private static bool _created = false;

        public EmployeeContext()
        {
            if (!_created)
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                _created = true;
            }
        }

        public DbSet<Employee> Employee { get; set; }

        protected override void OnConfiguring(DbContextOptions options)
        {
            options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EmployeeContext;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
