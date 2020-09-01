using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext
    {
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeLike> RecipeLikes { get; set; }
        public virtual DbSet<RecipeTagAssociation> RecipeTagAssociations { get; set; }

        protected void OnModelCreatingBase(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "RecipeRandomizer");

                entity.Property(i => i.Name)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.HasOne(i => i.QuantityUnit)
                    .WithMany(q => q.Ingredients)
                    .HasForeignKey(i => i.QuantityUnitId)
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

            modelBuilder.Entity<RecipeLike>(entity =>
            {
                entity.HasKey(rta => new {rta.RecipeId, rta.UserId});

                entity.ToTable("RecipeLike", "RecipeRandomizer");

                entity.HasOne(rl => rl.Recipe)
                    .WithMany(r => r.RecipeLikes)
                    .HasForeignKey(rl => rl.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeLike_Recipe");

                entity.HasOne(rl => rl.User)
                    .WithMany(u => u.RecipeLikes)
                    .HasForeignKey(rl => rl.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeLike_User");
            });

            modelBuilder.Entity<RecipeTagAssociation>(entity =>
            {
                entity.HasKey(rta => new {rta.RecipeId, rta.TagId});

                entity.ToTable("RecipeTag", "RecipeRandomizer");

                entity.HasOne(rta => rta.Recipe)
                    .WithMany(r => r.RecipeTagAssociations)
                    .HasForeignKey(rta => rta.RecipeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeTag_Recipe");

                entity.HasOne(rta => rta.Tag)
                    .WithMany(t => t.RecipeTagAssociations)
                    .HasForeignKey(rta => rta.TagId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RecipeTag_Tag");
            });
        }
    }
}
