using Microsoft.EntityFrameworkCore;
using YourCarePortal.Models;

namespace YourCarePortal.Data
{
    /// <summary>
    /// Represents the database context used for interacting with the database using Entity Framework.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        // The field seems unused. Consider removing it or add a meaningful purpose for its existence.
        // If it's a placeholder for future use, add a relevant comment.
        internal readonly object Session;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // DbSet properties represent tables in the database.

        public DbSet<tblAddress> tblAddress { get; set; }
        public DbSet<tblAppointments> tblAppointments { get; set; }
        public DbSet<tblClients> tblClients { get; set; }
        public DbSet<tblCompanies> tblCompanies { get; set; }
        public DbSet<tblUsers> tblUsers { get; set; }
        public DbSet<tblAppointmentServiceTypes> tblAppointmentServiceTypes { get; set; }
        public DbSet<tblClientPackages> tblClientPackages { get; set; }
        public DbSet<TblPortalUser> TblPortalUser { get; set; }
        public DbSet<QueryPortalUser> QueryPortalUser { get; set; }
        public DbSet<QueryClient> QueryClient { get; set; }
        public DbSet<QueryClientDetails> QueryClientDetails { get; set; }
        public DbSet<QueryProviderID> QueryProviderID { get; set; }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Uncomment the line below if you wish to map the Appointments entity to the "tblAppointments" table.
            // modelBuilder.Entity<Appointments>().ToTable("tblAppointments");
        }
    }
}
