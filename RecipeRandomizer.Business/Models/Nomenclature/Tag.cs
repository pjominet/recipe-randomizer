using System.Collections.Generic;
using RecipeRandomizer.Data.Entities.Shared;

namespace RecipeRandomizer.Business.Models.Nomenclature
{
    public class Tag
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public IList<Recipe> Recipes { get; set; }
        public TagCategory TagCategory { get; set; }
    }
}
