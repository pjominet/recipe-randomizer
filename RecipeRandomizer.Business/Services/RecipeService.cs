using System.Collections.Generic;
using AutoMapper;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly string[] _baseIncludes;

        public RecipeService(RRContext context, IMapper mapper)
        {
            _recipeRepository = new RecipeRepository(context);
            _mapper = mapper;
            _baseIncludes = new []
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.Quantity)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}"
            };
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => !r.IsDeleted, _baseIncludes));
        }

        public IEnumerable<Recipe> GetDeletedRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => r.IsDeleted, _baseIncludes));
        }

        public Recipe GetRecipe(int id)
        {
            return _mapper.Map<Recipe>(_recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id && !r.IsDeleted, _baseIncludes));
        }

        public Recipe GetRandomRecipe()
        {
            return _mapper.Map<Recipe>(_recipeRepository.GetRandomRecipe());
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds)
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipesFromTags(tagIds));
        }

        public int CreateRecipe(Recipe recipe)
        {
            var newRecipe = _mapper.Map<Entities.Recipe>(recipe);
            _recipeRepository.Insert(newRecipe);
            return _recipeRepository.SaveChanges() ? newRecipe.Id : -1;
        }

        public bool UpdateRecipe(Recipe recipe)
        {
            var existingRecipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == recipe.Id);
            _mapper.Map(recipe, existingRecipe);
            _recipeRepository.Update(existingRecipe);
            return _recipeRepository.SaveChanges();
        }

        public bool DeleteRecipe(int id, bool hard = false)
        {
            if (hard)
                _recipeRepository.HardDeleteRecipe(id);
            else
            {
                var recipeToDelete = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id);
                recipeToDelete.IsDeleted = true;
                _recipeRepository.Update(recipeToDelete);
            }

            return _recipeRepository.SaveChanges();
        }
    }
}
