using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IRecipeService
    {
        public Task<IEnumerable<Recipe>> GetRecipesAsync(IList<int> tagIds);
        public Task<int> GetPublishedRecipeCountAsync();
        public Task<IEnumerable<Recipe>> GetDeletedRecipesAsync();
        public Task<IEnumerable<Recipe>> GetAbandonedRecipesAsync();
        public Task<Recipe> GetRecipeAsync(int id);
        public Task<int?> GetRandomRecipeIdAsync(IList<int> tagIds);
        public Task<IEnumerable<Recipe>> GetRecipesForUserAsync(int userId);
        public Task<IEnumerable<Recipe>> GetLikedRecipesForUserAsync(int userId);
        public Task<Recipe> CreateRecipe(Recipe recipe);
        public Task<bool> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id);
        public Task<bool> UpdateRecipe(Recipe recipe);
        public Task<bool> DeleteRecipe(int id, bool hard = false);
        public Task<Recipe> RestoreDeletedRecipe(int id);
        public Task<bool> ToggleRecipeLike(int recipeId, LikeRequest request);
        public Task<bool> AttributeRecipeAsync(AttributionRequest request);
    }
}
