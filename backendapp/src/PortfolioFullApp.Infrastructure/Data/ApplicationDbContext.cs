using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PortfolioFullApp.Core.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioFullApp.Core.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PortfolioFullApp.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Hackathon> Hackathons { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<SocialMedia> SocialMedias { get; set; }
        public DbSet<ProjectLink> ProjectLinks { get; set; }
        public DbSet<HackathonLink> HackathonLinks { get; set; }
        public DbSet<NavbarItem> NavbarItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Id'leri string olarak yapılandır
            modelBuilder.Entity<Profile>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Contact>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Work>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Project>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<ProjectLink>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Education>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Hackathon>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<HackathonLink>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<Skill>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<NavbarItem>().Property(x => x.Id).HasMaxLength(36);
            modelBuilder.Entity<SocialMedia>().Property(x => x.Id).HasMaxLength(36);

            // List<string> type configurations
            modelBuilder.Entity<Work>()
                .Property(w => w.Badges)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

            // İlişkileri güncelle - artık int yerine string Id kullanıyoruz
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Links)
                .WithOne(l => l.Project)
                .HasForeignKey("ProjectId");

            modelBuilder.Entity<Hackathon>()
                .HasMany(h => h.Links)
                .WithOne(l => l.Hackathon)
                .HasForeignKey("HackathonId");

            // Default sıralama için indexler
            modelBuilder.Entity<Work>().HasIndex(w => w.Order);
            modelBuilder.Entity<Education>().HasIndex(e => e.Order);
            modelBuilder.Entity<Project>().HasIndex(p => p.Order);
            modelBuilder.Entity<ProjectLink>().HasIndex(p => p.Order);
            modelBuilder.Entity<Hackathon>().HasIndex(h => h.Order);
            modelBuilder.Entity<HackathonLink>().HasIndex(h => h.Order);
            modelBuilder.Entity<Skill>().HasIndex(s => s.Order);
            modelBuilder.Entity<NavbarItem>().HasIndex(n => n.Order);
            modelBuilder.Entity<SocialMedia>().HasIndex(s => s.Order);
        }
    }
}