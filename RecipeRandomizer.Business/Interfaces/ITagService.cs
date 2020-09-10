using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface ITagService
    {
        public Task<IEnumerable<Tag>> GetTags();
        public Task<Tag> GetTag(int id);
        public Task<Tag> CreateTag(Tag tag);
        public Task<bool> UpdateTag(Tag tag);
        public Task<bool> DeleteTag(int id);

        public Task<IEnumerable<TagCategory>> GetTagCategories();
        public Task<TagCategory> GetTagCategory(int id);
    }
}
