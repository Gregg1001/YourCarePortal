using Microsoft.EntityFrameworkCore;
using YourCarePortal.Models;

namespace YourCarePortal.Data
{
    public class AppointmentsContext : DbContext
    {
        public AppointmentsContext(DbContextOptions<AppointmentsContext> options)
            : base(options)
        {

        }

        public DbSet<Appointments> Appointments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointments>().ToTable("Table_3");
        }
    }
}