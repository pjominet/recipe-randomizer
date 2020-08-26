using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class QuantityProfile : Profile
    {
        public QuantityProfile()
        {
            CreateMap<Entities.Nomenclature.QuantityUnit, Models.Nomenclature.QuantityUnit>();
            CreateMap<Models.Nomenclature.QuantityUnit, Entities.Nomenclature.QuantityUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
        }
    }
}
