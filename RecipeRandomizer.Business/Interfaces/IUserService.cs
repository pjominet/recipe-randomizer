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
        User Update(int id, UpdateRequest updateRequest);
        public Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, int id);
        public bool Delete(int id);
        public bool ToggleUserLock(int id, LockRequest lockRequest);

    }
}
