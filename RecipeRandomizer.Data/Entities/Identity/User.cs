using System.Collections.Generic;

namespace RecipeRandomizer.Data.Entities.Identity
{
    public class User
    {
        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
