using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IRecipeService
    {
        public Task<IEnumerable<Recipe>> GetRecipesAsync(IList<int> tagIds);
        public Task<int> GetPublishedRecipeCount();
        public Task<IEnumerable<Recipe>> GetDeletedRecipes();
        public Task<IEnumerable<Recipe>> GetOrphanRecipes();
        public Task<Recipe> GetRecipe(int id);
        public Task<int?> GetRandomRecipeId(IList<int> tagIds);
        public Task<IEnumerable<Recipe>> GetRecipesForUser(int userId);
        public Task<IEnumerable<Recipe>> GetLikedRecipesForUser(int userId);
        public Task<Recipe> CreateRecipe(Recipe recipe);
        public bool UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id);
        public bool UpdateRecipe(Recipe recipe);
        public bool DeleteRecipe(int id, bool hard = false);
        public Recipe RestoreDeletedRecipe(int id);
        public bool ToggleRecipeLike(int recipeId, int userId, bool like);
        public bool AttributeRecipe(AttributionRequest request);
    }
}
