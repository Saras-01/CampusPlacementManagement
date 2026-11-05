using CampusPlacementManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<College> Colleges { get; set; }
        public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
        public DbSet<Company> Companies { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();
            builder.Entity<College>()
    .HasIndex(c => c.CollegeCode)
    .IsUnique()
    .HasFilter("[CollegeCode] IS NOT NULL");

            builder.Entity<PlacementOfficer>()
    .HasOne(p => p.User)
    .WithOne(u => u.PlacementOfficer)
    .HasForeignKey<PlacementOfficer>(p => p.UserId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlacementOfficer>()
                .HasOne(p => p.College)
                .WithMany(c => c.PlacementOfficers)
                .HasForeignKey(p => p.CollegeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PlacementOfficer>()
                .HasOne(p => p.Approver)
                .WithMany(u => u.ApprovedPlacementOfficers)
                .HasForeignKey(p => p.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}