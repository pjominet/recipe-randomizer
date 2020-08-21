using System.Collections.Generic;
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

        public IEnumerable<Tag> GetTags()
        {
            return _mapper.Map<IEnumerable<Tag>>(_tagRepository.GetAll<Entities.Tag>());
        }

        public Tag GetTag(int id)
        {
            string[] includes =
            {
                $"{nameof(Entities.Tag.TagCategory)}"
            };

            return _mapper.Map<Tag>(_tagRepository.GetFirstOrDefault<Entities.Tag>(t => t.Id == id, includes));
        }

        public int CreateTag(Tag tag)
        {
            var newTag = _mapper.Map<Entities.Tag>(tag);
            _tagRepository.Insert(newTag);
            return _tagRepository.SaveChanges() ? newTag.Id : -1;
        }

        public bool UpdateTag(Tag tag)
        {
            var existingRecipe = _tagRepository.GetFirstOrDefault<Entities.Tag>(t => t.Id == tag.Id);
            _mapper.Map(tag, existingRecipe);
            _tagRepository.Update(existingRecipe);
            return _tagRepository.SaveChanges();
        }

        public bool DeleteTag(int id)
        {
            _tagRepository.Delete(_tagRepository.GetFirstOrDefault<Entities.Tag>(t => t.Id == id));
            return _tagRepository.SaveChanges();
        }

        public IEnumerable<TagCategory> GetTagCategories()
        {
            string[] includes =
            {
                $"{nameof(Entities.TagCategory.Tags)}"
            };
            return _mapper.Map<IEnumerable<TagCategory>>(_tagRepository.GetAll<Entities.TagCategory>(null, includes));
        }

        public TagCategory GetTagCategory(int id)
        {
            string[] includes =
            {
                $"{nameof(Entities.TagCategory.Tags)}"
            };

            return _mapper.Map<TagCategory>(_tagRepository.GetFirstOrDefault<Entities.TagCategory>(tc => tc.Id == id, includes));
        }
    }
}
