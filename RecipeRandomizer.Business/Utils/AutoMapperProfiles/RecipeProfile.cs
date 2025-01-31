﻿using System.Linq;
using AutoMapper;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Utils.AutoMapperProfiles
{
    public class RecipeProfile : Profile
    {
        public RecipeProfile()
        {
            CreateMap<Entities.Recipe, Models.Recipe>()
                .ForMember(dest => dest.Tags, opt =>
                    opt.MapFrom(src => src.RecipeTagAssociations.Select(rta => rta.Tag)))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.CostId))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.DifficultyId))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.RecipeLikes.Select(rl => rl.UserId)))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.Username));
            CreateMap<Models.Recipe, Entities.Recipe>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Cost, opt => opt.Ignore())
                .ForMember(dest => dest.CostId, opt => opt.MapFrom(src => (int) src.Cost))
                .ForMember(dest => dest.Difficulty, opt => opt.Ignore())
                .ForMember(dest => dest.DifficultyId, opt => opt.MapFrom(src => (int) src.Difficulty))
                .ForMember(dest => dest.RecipeTagAssociations, opt => opt.Ignore())
                .ForMember(dest => dest.RecipeLikes, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
