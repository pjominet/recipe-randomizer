using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Business.Models
{
    public class LikeRequest
    {
        [Required]
        public bool Like { get; set; }
        [Required]
        public int LikedById { get; set; }
    }
}
