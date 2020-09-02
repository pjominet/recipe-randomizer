using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Repositories
{
    public class RecipeRepository : BaseRepository<RRContext>
    {
        public RecipeRepository(RRContext context) : base(context) { }

        public IEnumerable<Recipe> GetRecipes(bool deleted = false)
        {
            return Context.Recipes
                .Include(r => r.Cost)
                .Include(r => r.Difficulty)
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag)
                .AsEnumerable()
                .Where(r => r.IsDeleted == deleted);
        }

        public int? GetRandomRecipe(IList<int> tagIds)
        {
            var total = Context.Recipes.Count();
            var rnd = new Random();

            var recipes = Context.Recipes.Select(r => r);
            if (tagIds.Any())
                recipes = Context.RecipeTagAssociations
                    .Where(rta => tagIds.Contains(rta.TagId))
                    .Select(rta => rta.Recipe);

            if (!recipes.Any())
                return null;

            return recipes
                .Where(r => r.DeletedOn == null)
                .Skip(rnd.Next(0, total))
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag)
                .FirstOrDefault()?.Id;
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IList<int> tagIds)
        {
            var recipes = Context.RecipeTagAssociations
                .Where(rta => tagIds.Contains(rta.TagId))
                .Select(rta => rta.Recipe)
                .Where(r => r.DeletedOn == null);

            if (!recipes.Any())
                return null;

            foreach (var recipe in recipes.ToList())
            {
                recipe.Ingredients = Context.Ingredients.Where(i => i.RecipeId == recipe.Id).Include(i => i.QuantityUnit).ToList();
                recipe.RecipeTagAssociations = Context.RecipeTagAssociations.Where(rta => rta.RecipeId == recipe.Id).Include(rta => rta.Tag).ToList();
                recipe.User = Context.Users.Single(u => u.Id == recipe.UserId);
            }
            return recipes;
        }

        public IEnumerable<Recipe> GetLikedRecipesForUser(int userId)
        {
            var recipes = Context.RecipeLikes
                .Where(rl => rl.UserId == userId)
                .Select(rl => rl.Recipe)
                .Where(r => r.DeletedOn == null);

            if (!recipes.Any())
                return null;

            foreach (var recipe in recipes.ToList())
            {
                recipe.Ingredients = Context.Ingredients.Where(i => i.RecipeId == recipe.Id).Include(i => i.QuantityUnit).ToList();
                recipe.RecipeTagAssociations = Context.RecipeTagAssociations.Where(rta => rta.RecipeId == recipe.Id).Include(rta => rta.Tag).ToList();
                recipe.User = Context.Users.Single(u => u.Id == recipe.UserId);
            }
            return recipes;
        }

        public void HardDeleteRecipe(int id)
        {
            foreach (var tagAssociation in Context.RecipeTagAssociations.Where(rta => rta.RecipeId == id))
                Delete(tagAssociation);

            foreach (var recipeLike in Context.RecipeLikes.Where(rl => rl.RecipeId == id))
                Delete(recipeLike);

            Delete(Context.Recipes.Where(r => r.Id == id));
        }
    }
}
