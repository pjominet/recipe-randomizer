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

        public int GetPublishedRecipeCount()
        {
            return Context.Recipes
                .Where(r => r.UserId.HasValue)
                .Count(r => r.DeletedOn == null);
        }

        public IEnumerable<Recipe> GetRecipes(bool deleted = false)
        {
            return Context.Recipes
                .Where(r => r.UserId.HasValue)
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
                recipes = GetRecipesFromTagsQueryable(tagIds);

            return recipes
                .Skip(rnd.Next(0, total))
                .FirstOrDefault()?.Id;
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IList<int> tagIds)
        {
            return GetRecipesFromTagsQueryable(tagIds).AsEnumerable();
        }

        public IEnumerable<Recipe> GetLikedRecipesForUser(int userId)
        {
            var recipes = Context.RecipeLikes
                .Where(rl => rl.UserId == userId)
                .Select(rl => rl.Recipe)
                .Where(r => r.DeletedOn == null)
                .Distinct();

            if (!recipes.Any())
                return null;

            return recipes
                .Include(r => r.User)
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.QuantityUnit)
                .Include(r => r.RecipeTagAssociations)
                .ThenInclude(rta => rta.Tag);
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

        private IQueryable<Recipe> GetRecipesFromTagsQueryable(ICollection<int> tagIds)
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
                .Distinct();

            if (!recipes.Any())
                return null;

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
