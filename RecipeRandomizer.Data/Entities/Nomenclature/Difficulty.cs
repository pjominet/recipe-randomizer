using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class Difficulty
    {
        public Difficulty()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int Id { get; set; }
        public string Label { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
