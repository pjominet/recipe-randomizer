using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class VerificationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
