using System.Collections.Generic;
using RecipeRandomizer.Data.Entities.Shared;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class Cost
    {
        public Cost()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int Id { get; set; }
        public string Label { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
