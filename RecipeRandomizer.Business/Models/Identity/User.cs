using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Models.Identity
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }

        // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public User(Entities.User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
