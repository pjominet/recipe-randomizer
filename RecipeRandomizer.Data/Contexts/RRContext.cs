using Microsoft.EntityFrameworkCore;

namespace RecipeRandomizer.Data.Contexts
{
    public partial class RRContext : DbContext
    {
        private readonly DbContextOptions<RRContext> _options;

        public RRContext() { }

        public RRContext(DbContextOptions<RRContext> options)
            : base(options)
        {
            _options = options;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) { }

#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingShared(modelBuilder);
            OnModelCreatingNomenclature(modelBuilder);
        }
    }
}
