using System;
using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Identity
{
    public class User
    {
        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
            Recipes = new HashSet<Recipe>();
            RecipeLikes = new HashSet<RecipeLike>();
        }

        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool HasAcceptedTerms { get; set; }
        public string ProfileImageUri { get; set; }
        public string VerificationToken { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public bool IsVerified => VerifiedOn.HasValue;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpiresOn { get; set; }
        public DateTime? PasswordResetOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? LockedOn { get; set; }
        public int? LockedById { get; set; }
        public bool IsLocked => LockedOn.HasValue;

        public virtual Role Role { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<RecipeLike> RecipeLikes { get; set; }
        public virtual User LockedBy { get; set; }
        public virtual User Locker { get; set; }
    }
}
