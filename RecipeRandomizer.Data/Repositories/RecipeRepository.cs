using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Repositories
{
    public class RecipeRepository : BaseRepository<RRContext>
    {
        public RecipeRepository(RRContext context) : base(context) { }

        public Recipe GetRandomRecipe(IList<int> tagIds)
        {
            var total = Context.Recipes.Count();
            var rnd = new Random();

            var recipes = Context.Recipes.Select(r => r);
            if (tagIds.Any())
                recipes = Context.RecipeTagAssociations
                    .Where(rta => tagIds.Contains(rta.TagId))
                    .Select(rta => rta.Recipe);

            return recipes
                .Skip(rnd.Next(0, total))
                .Include(r => r.Cost)
                .Include(r => r.Difficulty)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.Quantity)
                .Include(r => r.RecipeTagAssociations)
                .FirstOrDefault();
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds)
        {
            return Context.RecipeTagAssociations
                .Where(rta => tagIds.Contains(rta.TagId))
                .Select(rta => rta.Recipe)
                .Include(r => r.Cost)
                .Include(r => r.Difficulty)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.Quantity)
                .Include(r => r.RecipeTagAssociations)
                .ToList();
        }

        public void HardDeleteRecipe(int id)
        {
            foreach (var tagAssociation in Context.RecipeTagAssociations.Where(rta => rta.RecipeId == id))
                Delete(tagAssociation);

            Delete(Context.Recipes.Where(r => r.Id == id));
        }
    }
}
