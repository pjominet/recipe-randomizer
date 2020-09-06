using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class RoleUpdateRequest
    {
        [Required]
        public Role Role { get; set; }
    }
}
