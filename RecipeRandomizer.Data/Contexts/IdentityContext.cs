using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected void OnModelCreatingIdentity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "RR_Identity");

                entity.Property(u => u.FirstName).IsRequired();

                entity.Property(u => u.LastName).IsRequired();

                entity.Property(u => u.Email).IsRequired();

                entity.Property(u => u.PasswordHash)
                    .HasColumnType("binary")
                    .HasMaxLength(64)
                    .IsRequired();

                entity.Property(u => u.PasswordSalt)
                    .HasColumnType("binary")
                    .HasMaxLength(128)
                    .IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken", "RR_Identity");

                entity.Property(rt => rt.Token).IsRequired();

                entity.Property(rt => rt.Expires)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.Property(rt => rt.Created)
                    .IsRequired()
                    .HasColumnType("datetime2");

                entity.Property(rt => rt.CreatedByIp)
                    .HasMaxLength(39)
                    .IsRequired();

                entity.Property(rt => rt.Revoked).HasColumnType("datetime2");

                entity.Property(rt => rt.RevokedByIp).HasMaxLength(39);

                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RefreshToken_User");
            });
        }
    }
}
