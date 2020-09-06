using System.Linq;
using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Entities.Identity.User, Models.Identity.User>()
                .ForMember(dest => dest.JwtToken, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.LikedRecipes, opt =>
                    opt.MapFrom(src => src.RecipeLikes.Select(rl => rl.Recipe)));

            CreateMap<Models.Identity.User, Entities.Identity.User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => (int) src.Role))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpiresOn, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetOn, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.HasAcceptedTerms, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Recipes, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeLikes, opt => opt.Ignore())
                .ForMember(dest => dest.LockedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Locker, opt => opt.Ignore());

            CreateMap<Models.Identity.RegisterRequest, Entities.Identity.User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageUri, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationToken, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedOn, opt => opt.Ignore())
                .ForMember(dest => dest.ResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.ResetTokenExpiresOn, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetOn, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Recipes, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeLikes, opt => opt.Ignore())
                .ForMember(dest => dest.LockedOn, opt => opt.Ignore())
                .ForMember(dest => dest.LockedById, opt => opt.Ignore())
                .ForMember(dest => dest.LockedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Locker, opt => opt.Ignore());
        }
    }
}
