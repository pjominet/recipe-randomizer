using System;
using System.Collections.Generic;
using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfileImageUri { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsVerified { get; set; }
        public string JwtToken { get; set; }

        // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public IList<Recipe> Recipes { get; set; }
        public IList<Recipe> LikedRecipes { get; set; }
    }
}
