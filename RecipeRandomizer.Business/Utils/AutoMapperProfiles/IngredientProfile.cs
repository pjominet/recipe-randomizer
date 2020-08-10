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
                .ForMember(dest => dest.Quantity, opt => opt.Ignore())
                .ForMember(dest => dest.Recipe, opt => opt.Ignore());
        }
    }
}
