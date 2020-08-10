using RecipeRandomizer.Data.Entities.Shared;
using Quantity = RecipeRandomizer.Business.Models.Nomenclature.Quantity;

namespace RecipeRandomizer.Business.Models.Shared
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
