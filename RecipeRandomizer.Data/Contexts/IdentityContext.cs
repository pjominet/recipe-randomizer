using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected void OnModelCreatingIdentity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "RR_Identity");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.UserName).IsRequired();

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_User_Role");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken", "RR_Identity");

                entity.Property(e => e.Token).IsRequired();

                entity.Property(rt => rt.CreatedByIp)
                    .HasMaxLength(39)
                    .IsRequired();

                entity.Property(rt => rt.RevokedByIp).HasMaxLength(39);

                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RefreshToken_User");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "RR_Identity");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(d => d.Label)
                    .IsRequired()
                    .HasMaxLength(8);
            });
        }
    }
}
