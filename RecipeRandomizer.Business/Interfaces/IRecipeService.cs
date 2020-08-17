using System.Collections.Generic;
using RecipeRandomizer.Business.Models;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IRecipeService
    {
        public IEnumerable<Recipe> GetRecipes();
        public IEnumerable<Recipe> GetDeletedRecipes();
        public Recipe GetRecipe(int id);
        public Recipe GetRandomRecipe();
        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds);
        public IEnumerable<Recipe> GetRecipesFromUser(int userId);
        public IEnumerable<Recipe> GetRecipesFromUserAndTags(int userId, IEnumerable<int> tagIds);
        public int CreateRecipe(Recipe recipe);
        public bool UpdateRecipe(Recipe recipe);
        public bool DeleteRecipe(int id, bool hard = false);
    }
}
