using System.Collections.Generic;
using System.IO;
using RecipeRandomizer.Business.Models;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IRecipeService
    {
        public IEnumerable<Recipe> GetRecipes();
        public int GetPublishedRecipeCount();
        public IEnumerable<Recipe> GetDeletedRecipes();
        public IEnumerable<Recipe> GetOrphanRecipes();
        public Recipe GetRecipe(int id);
        public int? GetRandomRecipe(IEnumerable<int> tagIds);
        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds);
        public IEnumerable<Recipe> GetRecipesForUser(int userId);
        public IEnumerable<Recipe> GetLikedRecipesForUser(int userId);
        public int CreateRecipe(Recipe recipe);
        public bool UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id);
        public bool UpdateRecipe(Recipe recipe);
        public bool DeleteRecipe(int id, bool hard = false);
        public Recipe RestoreDeletedRecipe(int id);
        public bool ToggleRecipeLike(int recipeId, int userId, bool like);
        public bool AttributeRecipe(AttributionRequest request);
    }
}
