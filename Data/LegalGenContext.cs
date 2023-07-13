using LegalGenApi.Models;
using Microsoft.AspNetCore.Authentication;

using Microsoft.EntityFrameworkCore;

namespace LegalGenApi.Data
{
   
    public class LegalGenContext:DbContext
    {
        public LegalGenContext(DbContextOptions<LegalGenContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<ResearchBook> ResearchBooks { get; set; }
        public DbSet<LegalInformation> LegalInformation { get; set; }
        public DbSet<SearchQuery> SearchQueries { get; set; }
        public DbSet<ChatInteraction> ChatInteractions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the relationships using Fluent API if needed

            modelBuilder.Entity<ResearchBook>()
            .HasOne(rb => rb.User)
            .WithMany(u => u.ResearchBooks)
            .HasForeignKey(rb => rb.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LegalInformation>()
                .HasOne(li => li.ResearchBook)
                .WithMany(rb => rb.LegalInformation)
                .HasForeignKey(li => li.ResearchBookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SearchQuery>()
                .HasOne(sq => sq.User)
                .WithMany(u => u.SearchQueries)
                .HasForeignKey(sq => sq.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatInteraction>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.ChatInteractions)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }


}
