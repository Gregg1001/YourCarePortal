using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YourCarePortal.Models
{
    public partial class tp1coastContext : DbContext
    {
        public tp1coastContext()
        {
        }

        public tp1coastContext(DbContextOptions<tp1coastContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tblCompanies> tblCompanies { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=backupblockssql.tpcare.com.au;TrustServerCertificate=true;Initial Catalog=tp1coast;User ID=tp_app_login;Password=BU4atxHT5rR;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<tblCompanies>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.ToTable("tblCompanies");

                entity.Property(e => e.CompanyId).HasColumnName("companyID");

                entity.Property(e => e.CompanyAbn)
                    .HasMaxLength(255)
                    .HasColumnName("companyABN");

                entity.Property(e => e.CompanyAccountsConfig)
                    .IsUnicode(false)
                    .HasColumnName("companyAccountsConfig");

                entity.Property(e => e.CompanyAccountsEmail)
                    .IsUnicode(false)
                    .HasColumnName("companyAccountsEmail");

                entity.Property(e => e.CompanyAccountsInstructions)
                    .IsUnicode(false)
                    .HasColumnName("companyAccountsInstructions");

                entity.Property(e => e.CompanyAccountsRestricted).HasColumnName("companyAccountsRestricted");

                entity.Property(e => e.CompanyAddress)
                    .HasMaxLength(255)
                    .HasColumnName("companyAddress");

                entity.Property(e => e.CompanyAllowanceKm1).HasColumnName("companyAllowanceKM1");

                entity.Property(e => e.CompanyAllowanceKm2).HasColumnName("companyAllowanceKM2");

                entity.Property(e => e.CompanyAvailabilityLockDate)
                    .HasColumnType("datetime")
                    .HasColumnName("companyAvailabilityLockDate");

                entity.Property(e => e.CompanyBankAccountBsb1)
                    .IsUnicode(false)
                    .HasColumnName("companyBankAccountBSB1");

                entity.Property(e => e.CompanyBankAccountName1)
                    .IsUnicode(false)
                    .HasColumnName("companyBankAccountName1");

                entity.Property(e => e.CompanyBankAccountNumber1)
                    .IsUnicode(false)
                    .HasColumnName("companyBankAccountNumber1");

                entity.Property(e => e.CompanyBillingConfig)
                    .IsUnicode(false)
                    .HasColumnName("companyBillingConfig");

                entity.Property(e => e.CompanyCareOrganisation).HasColumnName("companyCareOrganisation");

                entity.Property(e => e.CompanyCarePlanFormat)
                    .HasColumnType("ntext")
                    .HasColumnName("companyCarePlanFormat");

                entity.Property(e => e.CompanyCoords)
                    .HasColumnType("ntext")
                    .HasColumnName("companyCoords");

                entity.Property(e => e.CompanyCrmid).HasColumnName("companyCRMID");

                entity.Property(e => e.CompanyDateModified)
                    .HasColumnType("datetime")
                    .HasColumnName("companyDateModified");

                entity.Property(e => e.CompanyDefaultMobile)
                    .HasColumnType("ntext")
                    .HasColumnName("companyDefaultMobile");

                entity.Property(e => e.CompanyDisabled).HasColumnName("companyDisabled");

                entity.Property(e => e.CompanyDvanursingSenderCode)
                    .IsUnicode(false)
                    .HasColumnName("companyDVANursingSenderCode");

                entity.Property(e => e.CompanyDvanursingServicingDoctor)
                    .IsUnicode(false)
                    .HasColumnName("companyDVANursingServicingDoctor");

                entity.Property(e => e.CompanyEmail)
                    .HasMaxLength(255)
                    .HasColumnName("companyEmail");

                entity.Property(e => e.CompanyGeneralConfig)
                    .IsUnicode(false)
                    .HasColumnName("companyGeneralConfig");

                entity.Property(e => e.CompanyGeoFencing).HasColumnName("companyGeoFencing");

                entity.Property(e => e.CompanyGpswarningsEnforced).HasColumnName("companyGPSWarningsEnforced");

                entity.Property(e => e.CompanyHaccproviderNumber).HasColumnName("companyHACCProviderNumber");

                entity.Property(e => e.CompanyInvoiceFormat).HasColumnName("companyInvoiceFormat");

                entity.Property(e => e.CompanyInvoiceTerms)
                    .HasColumnType("ntext")
                    .HasColumnName("companyInvoiceTerms");

                entity.Property(e => e.CompanyLastInvoiceNumber).HasColumnName("companyLastInvoiceNumber");

                entity.Property(e => e.CompanyLogo)
                    .HasMaxLength(255)
                    .HasColumnName("companyLogo");

                entity.Property(e => e.CompanyMobileTitle)
                    .HasColumnType("ntext")
                    .HasColumnName("companyMobileTitle");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("companyName");

                entity.Property(e => e.CompanyNdisproviderNumber)
                    .IsUnicode(false)
                    .HasColumnName("companyNDISProviderNumber");

                entity.Property(e => e.CompanyPackageServiceRestrictions)
                    .IsUnicode(false)
                    .HasColumnName("companyPackageServiceRestrictions");

                entity.Property(e => e.CompanyPayrollConfig)
                    .HasColumnType("ntext")
                    .HasColumnName("companyPayrollConfig");

                entity.Property(e => e.CompanyPayrollFormat).HasColumnName("companyPayrollFormat");

                entity.Property(e => e.CompanyPhone)
                    .HasMaxLength(255)
                    .HasColumnName("companyPhone");

                entity.Property(e => e.CompanyPlannerVersion).HasColumnName("companyPlannerVersion");

                entity.Property(e => e.CompanyPostcode)
                    .HasMaxLength(4)
                    .HasColumnName("companyPostcode");

                entity.Property(e => e.CompanyPublishedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("companyPublishedDate");

                entity.Property(e => e.CompanyRateBase).HasColumnName("companyRateBase");

                entity.Property(e => e.CompanyRateDefinitions)
                    .HasColumnType("ntext")
                    .HasColumnName("companyRateDefinitions");

                entity.Property(e => e.CompanyReferrer).HasColumnName("companyReferrer");

                entity.Property(e => e.CompanyRosterType)
                    .HasColumnType("ntext")
                    .HasColumnName("companyRosterType");

                entity.Property(e => e.CompanyShiftBreakMins).HasColumnName("companyShiftBreakMins");

                entity.Property(e => e.CompanyShiftDefinitions)
                    .HasColumnType("ntext")
                    .HasColumnName("companyShiftDefinitions");

                entity.Property(e => e.CompanyState)
                    .HasMaxLength(255)
                    .HasColumnName("companyState");

                entity.Property(e => e.CompanySuburb)
                    .HasMaxLength(255)
                    .HasColumnName("companySuburb");

                entity.Property(e => e.CompanyWebsite)
                    .HasMaxLength(255)
                    .HasColumnName("companyWebsite");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
