using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class UpdateRequest
    {
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
