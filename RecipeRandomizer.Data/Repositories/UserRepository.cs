using System.Linq;
using RecipeRandomizer.Data.Contexts;

namespace RecipeRandomizer.Data.Repositories
{
    public class UserRepository : BaseRepository<RRContext>
    {
        public UserRepository(RRContext context) : base(context) { }

        public bool HasUsers()
        {
            return Context.Users.Any();
        }
    }
}
