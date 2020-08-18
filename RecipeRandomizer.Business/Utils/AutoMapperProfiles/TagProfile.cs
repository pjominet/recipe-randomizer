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
                    opt.MapFrom(src => src.RecipeTagAssociations.Select(rta => rta.Recipe)));
            CreateMap<Models.Nomenclature.Tag, Entities.Nomenclature.Tag>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TagCategory, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeTagAssociations, opt => opt.Ignore());

            CreateMap<Entities.Nomenclature.TagCategory, Models.Nomenclature.TagCategory>();
            CreateMap<Models.Nomenclature.TagCategory, Entities.Nomenclature.TagCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
        }
    }
}
