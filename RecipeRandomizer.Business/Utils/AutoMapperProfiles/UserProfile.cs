using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.Identity.User, Models.Identity.User>()
                .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
                .ForMember(dest => dest.JwtToken, opt => opt.Ignore())
                .DisableCtorValidation();

            CreateMap<Entities.Identity.User, Models.Identity.AuthRequest>();

            CreateMap<Models.Identity.RegisterRequest, Entities.Identity.User>();

            CreateMap<Models.Identity.UpdateUserRequest, Entities.Identity.User>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        switch (prop)
                        {
                            // ignore null & empty string properties
                            case null:
                            case string arg when string.IsNullOrEmpty(arg):
                                return false;
                            default:
                                // ignore null role
                                return x.DestinationMember.Name != "Role" || src.Role != null;
                        }
                    }
                ));
        }
    }
}
