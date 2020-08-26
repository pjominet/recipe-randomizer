using System.Collections.Generic;

namespace RecipeRandomizer.Business.Models.Nomenclature
{
    public class QuantityUnit
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public IList<Ingredient> Ingredients { get; set; }
    }
}
