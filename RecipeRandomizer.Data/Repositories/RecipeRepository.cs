using System.Collections.Generic;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Data.Repositories
{
    public class RecipeRepository : BaseRepository<RRContext>
    {
        public RecipeRepository(RRContext context) : base(context) { }

        public Recipe GetRandomRecipe()
        {
            return null;
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds)
        {
            return null;
        }

        public bool DeleteRecipe(int id)
        {
            return false;
        }
    }
}
