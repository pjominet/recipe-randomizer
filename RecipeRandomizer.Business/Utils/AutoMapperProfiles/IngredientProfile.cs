using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile()
        {
            CreateMap<Entities.Ingredient, Models.Ingredient>();
            CreateMap<Models.Ingredient, Entities.Ingredient>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.QuantityUnit, opt => opt.Ignore())
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());

            CreateMap<Entities.Nomenclature.QuantityUnit, Models.Nomenclature.QuantityUnit>();
            CreateMap<Models.Nomenclature.QuantityUnit, Entities.Nomenclature.QuantityUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ingredients, opt => opt.Ignore());
        }
    }
}
