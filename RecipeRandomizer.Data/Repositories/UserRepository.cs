using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Data.Contexts;

namespace RecipeRandomizer.Data.Repositories
{
    public class UserRepository : BaseRepository<RRContext>
    {
        public UserRepository(RRContext context) : base(context) { }

        public async Task<bool> HasUsersAsync()
        {
            return await Context.Users.AnyAsync();
        }

        public async Task<int> AdminCountAsync()
        {
            return await Context.Users.CountAsync(r => r.RoleId == 1);
        }
    }
}
