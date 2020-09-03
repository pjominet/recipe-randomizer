using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IUserService
    {
        public IEnumerable<User> GetUsers();
        public User GetUser(int id);
        User Update(int id, User userUpdates);
        public Task<bool> UploadUserAvatar(Stream imageStream, string untrustedFileName, int id);
        bool Delete(int id);
    }
}
