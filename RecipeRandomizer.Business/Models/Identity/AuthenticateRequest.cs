using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class AuthenticateRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
