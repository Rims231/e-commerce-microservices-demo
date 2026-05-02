using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationApiDbContext(DbContextOptions<AuthenticationApiDbContext> options)
        : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.TelephoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.Address)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Password)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}