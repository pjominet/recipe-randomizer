using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public virtual DbSet<Quantity> Quantities { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected void OnModelCreatingNomenclature(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quantity>(entity =>
            {
                entity.ToTable("Quantity", "Nomenclature");

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag", "Nomenclature");

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasMaxLength(32);
            });
        }
    }
}
