using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Identity;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUsersAsync();
        public Task<User> GetUserAsync(int id);
        public Task<User> UpdateAsync(int id, UserUpdateRequest userUpdateRequest);
        public Task<User> UpdateAsync(int id, RoleUpdateRequest roleUpdateRequest);
        public Task<bool> UploadUserAvatar(Stream sourceStream, string untrustedFileName, int id);
        public Task<bool> Delete(int id);
        public Task<bool> ToggleUserLock(int id, LockRequest lockRequest);

    }
}
