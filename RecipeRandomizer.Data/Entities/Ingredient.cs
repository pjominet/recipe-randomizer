using RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Data.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int QuantityUnitId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public virtual QuantityUnit QuantityUnit { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
