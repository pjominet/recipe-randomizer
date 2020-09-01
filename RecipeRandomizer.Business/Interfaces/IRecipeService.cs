using System.Collections.Generic;
using RecipeRandomizer.Business.Models;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IRecipeService
    {
        public IEnumerable<Recipe> GetRecipes();
        public IEnumerable<Recipe> GetDeletedRecipes();
        public Recipe GetRecipe(int id);
        public int? GetRandomRecipe(IEnumerable<int> tagIds);
        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds);
        public IEnumerable<Recipe> GetRecipesForUser(int userId);
        public IEnumerable<Recipe> GetLikedRecipesForUser(int userId);
        public int CreateRecipe(Recipe recipe);
        public bool UpdateRecipe(Recipe recipe);
        public bool DeleteRecipe(int id, bool hard = false);
    }
}
