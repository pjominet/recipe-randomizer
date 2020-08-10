using RecipeRandomizer.Data.Contexts;

namespace RecipeRandomizer.Data.Repositories
{
    public class QuantityRepository : BaseRepository<RRContext>
    {
        public QuantityRepository(RRContext context) : base(context) { }
    }
}
