using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.User, Models.Identity.User>()
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.JwtToken, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .DisableCtorValidation();
            CreateMap<Models.Identity.User, Entities.User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());
            CreateMap<Entities.RefreshToken, Models.Identity.RefreshToken>();
        }
    }
}
