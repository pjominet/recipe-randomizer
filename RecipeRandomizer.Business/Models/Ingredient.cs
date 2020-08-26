using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int QuantityUnitId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public QuantityUnit QuantityUnit { get; set; }
        public Recipe Recipe { get; set; }
    }
}
