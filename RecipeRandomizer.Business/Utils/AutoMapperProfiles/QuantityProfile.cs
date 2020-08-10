using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class QuantityProfile : Profile
    {
        public QuantityProfile()
        {
            CreateMap<Entities.Nomenclature.Quantity, Models.Nomenclature.Quantity>();
            CreateMap<Models.Nomenclature.Quantity, Entities.Nomenclature.Quantity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
        }
    }
}
