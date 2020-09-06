using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class UserUpdateRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
