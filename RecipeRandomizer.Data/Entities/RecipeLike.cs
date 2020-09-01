using RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Data.Entities
{
    public class RecipeLike
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual User User { get; set; }
    }
}
