using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class QuantityUnit
    {
        public QuantityUnit()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }
}
