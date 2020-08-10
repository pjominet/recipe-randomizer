using Quantity = RecipeRandomizer.Business.Models.Nomenclature.Quantity;
using Recipe = RecipeRandomizer.Business.Models.Recipe;

namespace RecipeRandomizer.Business.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public int QuantityId { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }

        public Quantity Quantity { get; set; }
        public Recipe Recipe { get; set; }
    }
}
