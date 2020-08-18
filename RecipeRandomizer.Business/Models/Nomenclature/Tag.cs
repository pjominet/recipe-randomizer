using System.Collections.Generic;

namespace RecipeRandomizer.Business.Models.Nomenclature
{
    public class Tag
    {
        public int Id { get; set; }
        public int TagCategoryId { get; set; }
        public string Label { get; set; }

        public IList<Recipe> Recipes { get; set; }
        public TagCategory TagCategory { get; set; }
    }
}
