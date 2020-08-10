using RecipeRandomizer.Data.Contexts;

namespace RecipeRandomizer.Data.Repositories
{
    public class TagRepository : BaseRepository<RRContext>
    {
        public TagRepository(RRContext context) : base(context) { }
    }
}
