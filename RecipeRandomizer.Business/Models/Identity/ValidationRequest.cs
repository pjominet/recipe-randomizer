using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class ValidationRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
