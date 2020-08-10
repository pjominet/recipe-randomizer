using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class TagCategory
    {
        public TagCategory()
        {
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public string Label { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
