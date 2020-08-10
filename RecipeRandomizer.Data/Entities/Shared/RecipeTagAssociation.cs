using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Entities.Shared
{
    public class RecipeTagAssociation
    {
        public int RecipeId { get; set; }
        public int TagId { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
