using System.Linq;
using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Entities.Nomenclature.Tag, Models.Nomenclature.Tag>()
                .ForMember(dest => dest.Recipes, opt =>
                    opt.MapFrom(src => src.RecipeTagAssociations.Select(rta => rta.Recipe)))
                .ForMember(dest => dest.TagCategory, opt => opt.MapFrom(src => src.TagCategoryId));
            CreateMap<Models.Nomenclature.Tag, Entities.Nomenclature.Tag>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TagCategory, opt => opt.Ignore())
                .ForMember(dest => dest.TagCategoryId, opt => opt.MapFrom(src => (int) src.TagCategory))
                .ForMember(dest => dest.RecipeTagAssociations, opt => opt.Ignore());
        }
    }
}
