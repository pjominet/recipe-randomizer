using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeTagAssociation> RecipeTagAssociations { get; set; }

        protected void OnModelCreatingBase(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "RecipeRandomizer");

                entity.Property(i => i.Name)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(i => i.Quantity)
                    .WithMany(q => q.Ingredients)
                    .HasForeignKey(i => i.QuantityId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Ingredient_Quantity");

                entity.HasOne(i => i.Recipe)
                    .WithMany(r => r.Ingredients)
                    .HasForeignKey(i => i.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_Recipe");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe", "RecipeRandomizer");

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(r => r.Preparation).IsRequired();

                entity.HasOne(r => r.Cost)
                    .WithMany(c => c.Recipes)
                    .HasForeignKey(r => r.CostId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Recipe_Cost");

                entity.HasOne(r => r.Difficulty)
                    .WithMany(d => d.Recipes)
                    .HasForeignKey(r => r.DifficultyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Recipe_Difficulty");

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Recipes)
                    .HasForeignKey(r => r.UserId)
                    .HasConstraintName("FK_Recipe_User");
            });

            modelBuilder.Entity<RecipeTagAssociation>(entity =>
            {
                entity.HasKey(rta => new {rta.RecipeId, rta.TagId});

                entity.ToTable("RecipeTag", "RecipeRandomizer");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeTagAssociations)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeTag_Recipe");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.RecipeTagAssociations)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeTag_Tag");
            });
        }
    }
}
