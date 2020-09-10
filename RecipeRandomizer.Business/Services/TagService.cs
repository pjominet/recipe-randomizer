using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Nomenclature;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Nomenclature;

namespace RecipeRandomizer.Business.Services
{
    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagService(RRContext context, IMapper mapper)
        {
            _tagRepository = new TagRepository(context);
            _mapper = mapper;
        }

        public async Task<IEnumerable<Tag>> GetTags()
        {
            return _mapper.Map<IEnumerable<Tag>>(await _tagRepository.GetAllAsync<Entities.Tag>());
        }

        public async Task<Tag> GetTag(int id)
        {
            string[] includes =
            {
                $"{nameof(Entities.Tag.TagCategory)}"
            };

            return _mapper.Map<Tag>(await _tagRepository.GetFirstOrDefaultAsync<Entities.Tag>(t => t.Id == id, includes));
        }

        public async Task<Tag> CreateTag(Tag tag)
        {
            var newTag = _mapper.Map<Entities.Tag>(tag);
            _tagRepository.Insert(newTag);
            return await _tagRepository.SaveChangesAsync()
                ? _mapper.Map<Tag>(newTag)
                : null;
        }

        public async Task<bool> UpdateTag(Tag tag)
        {
            var existingRecipe = await _tagRepository.GetFirstOrDefaultAsync<Entities.Tag>(t => t.Id == tag.Id);
            _mapper.Map(tag, existingRecipe);
            _tagRepository.Update(existingRecipe);
            return await _tagRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteTag(int id)
        {
            _tagRepository.Delete(await _tagRepository.GetFirstOrDefaultAsync<Entities.Tag>(t => t.Id == id));
            return await _tagRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TagCategory>> GetTagCategories()
        {
            return _mapper.Map<IEnumerable<TagCategory>>(
                await _tagRepository.GetAllAsync<Entities.TagCategory>(
                    null, $"{nameof(Entities.TagCategory.Tags)}"));
        }

        public async Task<TagCategory> GetTagCategory(int id)
        {
            return _mapper.Map<TagCategory>(
                await _tagRepository.GetFirstOrDefaultAsync<Entities.TagCategory>(
                    tc => tc.Id == id, $"{nameof(Entities.TagCategory.Tags)}"));
        }
    }
}
