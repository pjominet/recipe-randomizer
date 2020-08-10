using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Shared;

namespace RecipeRandomizer.Business.Models.Nomenclature
{
    public class Quantity
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public IList<Ingredient> Ingredients { get; set; }
    }
}
