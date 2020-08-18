using System.Collections.Generic;

namespace RecipeRandomizer.Business.Models.Nomenclature
{
    public class TagCategory
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public IList<Tag> Tags { get; set; }
    }
}
