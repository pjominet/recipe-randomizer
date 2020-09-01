using System;
using System.Collections.Generic;
using System.Linq;
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

        public RecipeService(RRContext context, IMapper mapper)
        {
            _recipeRepository = new RecipeRepository(context);
            _mapper = mapper;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipes());
        }

        public IEnumerable<Recipe> GetDeletedRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipes(true));
        }

        public Recipe GetRecipe(int id)
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}"
            };
            return _mapper.Map<Recipe>(_recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id, includes));
        }

        public int? GetRandomRecipe(IEnumerable<int> tagIds)
        {
            return _recipeRepository.GetRandomRecipe(tagIds.ToList());
        }

        public IEnumerable<Recipe> GetRecipesFromTags(IEnumerable<int> tagIds)
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipesFromTags(tagIds.ToList()));
        }

        public IEnumerable<Recipe> GetRecipesForUser(int userId)
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}"
            };
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => r.UserId == userId, includes));
        }

        public IEnumerable<Recipe> GetLikedRecipesForUser(int userId)
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetLikedRecipesForUser(userId));
        }

        public int CreateRecipe(Recipe recipe)
        {
            var newRecipe = _mapper.Map<Entities.Recipe>(recipe);
            newRecipe.CreatedOn = DateTime.UtcNow;
            _recipeRepository.Insert(newRecipe);
            return _recipeRepository.SaveChanges() ? newRecipe.Id : -1;
        }

        public bool UpdateRecipe(Recipe recipe)
        {
            var existingRecipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == recipe.Id);
            _mapper.Map(recipe, existingRecipe);
            existingRecipe.LastUpdatedOn = DateTime.UtcNow;
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
                recipeToDelete.DeletedOn = DateTime.UtcNow;
                _recipeRepository.Update(recipeToDelete);
            }

            return _recipeRepository.SaveChanges();
        }
    }
}
