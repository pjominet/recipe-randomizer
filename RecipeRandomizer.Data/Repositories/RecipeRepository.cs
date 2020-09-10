using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Repositories
{
    public class RecipeRepository : BaseRepository<RRContext>
    {
        public RecipeRepository(RRContext context) : base(context) { }

        public async Task<int> GetPublishedRecipeCountAsync()
        {
            return await Context.Recipes
                .Where(r => r.UserId.HasValue)
                .CountAsync(r => r.DeletedOn == null);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync(bool deleted = false)
        {
            return await Context.Recipes
                .Where(r => r.UserId.HasValue)
                .Include(r => r.Cost)
                .Include(r => r.Difficulty)
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag)
                .Where(r => (r.DeletedOn != null) == deleted)
                .ToListAsync();
        }

        public async Task<Recipe> GetRandomRecipeAsync(IList<int> tagIds)
        {
            var total = await Context.Recipes.CountAsync();
            var rnd = new Random();

            var recipes = Context.Recipes.Select(r => r);
            if (tagIds.Any())
                recipes = GetRecipesFromTagsAsQueryable(tagIds);

            return await recipes
                .Skip(rnd.Next(0, total))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesFromTagsAsync(IList<int> tagIds)
        {
            return await GetRecipesFromTagsAsQueryable(tagIds).ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetLikedRecipesForUserAsync(int userId)
        {
            var recipes = Context.RecipeLikes
                .Where(rl => rl.UserId == userId)
                .Select(rl => rl.Recipe)
                .Where(r => r.DeletedOn == null)
                .Distinct();

            if (!recipes.Any())
                return null;

            return await recipes
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag)
                .ToListAsync();
        }

        public void HardDeleteRecipe(int id)
        {
            foreach (var tagAssociation in Context.RecipeTagAssociations.Where(rta => rta.RecipeId == id))
                Delete(tagAssociation);

            foreach (var recipeLike in Context.RecipeLikes.Where(rl => rl.RecipeId == id))
                Delete(recipeLike);

            Delete(Context.Recipes.Where(r => r.Id == id));
        }

        #region helpers

        private IQueryable<Recipe> GetRecipesFromTagsAsQueryable(ICollection<int> tagIds)
        {
            var matchingRecipeIds = Context.RecipeTagAssociations
                .Where(rta => tagIds.Contains(rta.TagId))
                .GroupBy(rta => rta.RecipeId)
                .Where(grp => grp.Count() == tagIds.Count)
                .Select(grp => grp.Key);

            var recipes = Context.Recipes
                .Where(r => matchingRecipeIds.Contains(r.Id))
                .Where(r => r.DeletedOn == null)
                .Where(r => r.UserId.HasValue)
                .OrderBy(r => r.CreatedOn);

            if (!recipes.Any())
                return Enumerable.Empty<Recipe>().AsQueryable();

            return recipes
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag);
        }

        #endregion

    }
}
