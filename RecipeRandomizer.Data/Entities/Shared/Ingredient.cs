using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Entities.Shared
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int QuantityId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }

        public virtual Quantity Quantity { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
