using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities.Shared;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeTagAssociation> RecipeTagAssociations { get; set; }

        protected void OnModelCreatingShared(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "RecipeRandomizer");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(d => d.Quantity)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.QuantityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ingredient_Quantity");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Ingredients)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ingredient_Recipe");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe", "RecipeRandomizer");

                entity.Property(e => e.Description).HasMaxLength(512);

                entity.Property(e => e.ImageUri).HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Preparation).IsRequired();
            });

            modelBuilder.Entity<RecipeTagAssociation>(entity =>
            {
                entity.HasKey(e => new {e.RecipeId, e.TagId});

                entity.ToTable("RecipeTag", "RecipeRandomizer");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeTagAssociations)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeTag_Recipe");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.RecipeTagAssociations)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeTag_Tag");
            });
        }
    }
}
