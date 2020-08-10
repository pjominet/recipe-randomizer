using System.Collections.Generic;
using RecipeRandomizer.Business.Models.Nomenclature;

namespace RecipeRandomizer.Business.Interfaces
{
    public interface ITagService
    {
        public IEnumerable<Tag> GetTags();
        public Tag GetTag(int id);
        public int CreateTag(Tag tag);
        public bool UpdateTag(Tag tag);
        public bool DeleteTag(int id);

        public IEnumerable<TagCategory> GetTagCategories();
        public TagCategory GetTagCategory(int id);
    }
}
