using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IUserService
    {
        public User Authenticate(AuthenticateRequest model, string ipAddress);
        public User RefreshToken(string token, string ipAddress);
        public bool RevokeToken(string token, string ipAddress);
        public IEnumerable<User> GetUsers();
        public User GetUser(int id);
        public IEnumerable<RefreshToken> GetUserRefreshTokens(int id);
        int Create(User model, string password);
        bool Update(User model, string password = null);
        bool Delete(int id);
    }
}
