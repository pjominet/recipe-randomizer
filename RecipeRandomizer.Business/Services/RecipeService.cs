using System.Collections.Generic;
using System.Net;
using AutoMapper;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Business.Utils;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly string[] _includes;

        public RecipeService(RRContext context, IMapper mapper)
        {
            _recipeRepository = new RecipeRepository(context);
            _mapper = mapper;
            _includes = new []
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}"
            };
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => !r.IsDeleted, _includes));
        }

        public IEnumerable<Recipe> GetDeletedRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => r.IsDeleted, _includes));
        }

        public Recipe GetRecipe(int id)
        {
            return _mapper.Map<Recipe>(_recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id && !r.IsDeleted, _includes));
        }

        public Recipe GetRandomRecipe()
        {
            return _mapper.Map<Recipe>(_recipeRepository.GetRandomRecipe());
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds)
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipesFromTags(tagIds));
        }

        public RequestResult<int> CreateRecipe(Recipe recipe)
        {
            var result = new RequestResult<int>();
            var newRecipe = _mapper.Map<Entities.Recipe>(recipe);
            _recipeRepository.Insert(newRecipe);

            if (!_recipeRepository.SaveChanges())
                return result;

            result.IsSuccess = true;
            result.StatusCode = HttpStatusCode.OK;
            result.ReturnValue = newRecipe.Id;
            return result;
        }

        public bool UpdateRecipe(Recipe recipe)
        {
            return _recipeRepository.SaveChanges();
        }

        public bool DeleteRecipe(int id)
        {
            return _recipeRepository.DeleteRecipe(id);
        }
    }
}
