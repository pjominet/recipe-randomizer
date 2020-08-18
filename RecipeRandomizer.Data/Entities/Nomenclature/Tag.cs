using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Nomenclature
{
    public class Tag
    {
        public Tag()
        {
            RecipeTagAssociations = new HashSet<RecipeTagAssociation>();
        }

        public int Id { get; set; }
        public int TagCategoryId { get; set; }
        public string Label { get; set; }

        public virtual ICollection<RecipeTagAssociation> RecipeTagAssociations { get; set; }
        public virtual TagCategory TagCategory { get; set; }
    }
}
