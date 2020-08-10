using System.Collections.Generic;
using RecipeRandomizer.Data.Entities.Shared;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class Tag
    {
        public Tag()
        {
            RecipeTagAssociations = new HashSet<RecipeTagAssociation>();
        }

        public int Id { get; set; }
        public string Label { get; set; }
        public int TagCategoryId { get; set; }

        public virtual ICollection<RecipeTagAssociation> RecipeTagAssociations { get; set; }
        public virtual TagCategory TagCategory { get; set; }
    }
}
