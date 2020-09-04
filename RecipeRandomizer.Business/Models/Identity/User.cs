using System;
using System.Collections.Generic;
using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfileImageUri { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }
        public DateTime? LockedOn { get; set; }

        public IList<Recipe> Recipes { get; set; }
        public IList<Recipe> LikedRecipes { get; set; }
        public User LockedBy { get; set; }
    }
}
