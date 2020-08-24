using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
