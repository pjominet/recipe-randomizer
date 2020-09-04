using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models
{
    public class AttributionRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RecipeId { get; set; }
    }
}
