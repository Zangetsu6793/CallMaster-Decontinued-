using Microsoft.EntityFrameworkCore;
using CallMaster.Models;
using CallMaster.Models.CallMaster.Models;

namespace CallMaster.Data
{
    public class CallMasterDbContext : DbContext
    {
        // Constructor that takes DbContextOptions
        public CallMasterDbContext(DbContextOptions<CallMasterDbContext> options)
            : base(options)
        {
        }

        // Define your DbSets (tables)
        //Users
        public DbSet<User> Users { get; set; }
        public DbSet<VoIPLoginDetails> VoIPLoginDetails { get; set; }
        //Campains
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignPhoneNumber> CampaignPhoneNumbers { get; set; }
        public DbSet<Team> Teams { get; set; }

        //LicenceKeys
        public DbSet<LicenceKeys> LicenceKeys { get; set; }

        // Configure the one-to-zero-or-one relationship between User and VoIPLoginDetails
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.VoIPDetails) // A User has one VoIPDetails
                .WithOne(v => v.User) // A VoIPDetails is linked to one User
                .HasForeignKey<VoIPLoginDetails>(v => v.UserId) // Foreign key in VoIPLoginDetails
                .OnDelete(DeleteBehavior.SetNull); // If User is deleted, set VoIPLoginDetails.UserId to null (no cascade delete)

            // Ensure UserId is nullable in VoIPLoginDetails
            modelBuilder.Entity<VoIPLoginDetails>()
                .Property(v => v.UserId)
                .IsRequired(false); // Ensure this is nullable

            modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();


            // Campaign ↔ PhoneNumbers Relationship
            modelBuilder.Entity<CampaignPhoneNumber>()
                .HasOne(p => p.Campaign)
                .WithMany(c => c.PhoneNumbers)
                .HasForeignKey(p => p.CampaignId)
                .OnDelete(DeleteBehavior.Cascade); // When a campaign is deleted, remove all its numbers

            // Campaign ↔ Users (Agents)
            modelBuilder.Entity<Campaign>()
                .HasMany(c => c.AssignedAgents)
                .WithMany() // No explicit navigation back from User to Campaign
                .UsingEntity(j => j.ToTable("CampaignAgents")); // Join table for many-to-many relationship

        }
    }
}
