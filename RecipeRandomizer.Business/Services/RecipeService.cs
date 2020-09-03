using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities;

namespace RecipeRandomizer.Business.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public RecipeService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _recipeRepository = new RecipeRepository(context);
            _mapper = mapper;
            _appSettings = appSettings.Value;
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
                $"{nameof(Entities.Recipe.User)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}",
                $"{nameof(Entities.Recipe.RecipeLikes)}"
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
            newRecipe.UpdatedOn = DateTime.UtcNow;
            foreach (var tag in recipe.Tags)
                _recipeRepository.Insert(_mapper.Map(new Entities.RecipeTagAssociation(), tag));

            _recipeRepository.Insert(newRecipe);
            return _recipeRepository.SaveChanges() ? newRecipe.Id : -1;
        }

        public async Task<bool> UploadRecipeImage(Stream imageStream, string untrustedFileName, int id)
        {
            var recipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id);
            if(recipe == null)
                throw new KeyNotFoundException("Recipe to add image to could not be found");

            var trustedFilePath = _appSettings.RecipeImagesFolder + Guid.NewGuid() + Path.GetExtension(untrustedFileName);
            await using var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), trustedFilePath), FileMode.Create);

            if (!imageStream.CopyToAsync(fileStream).IsCompletedSuccessfully)
                throw new ApplicationException("File copy failed");

            recipe.ImageUri = trustedFilePath;
            recipe.OriginalImageName = untrustedFileName;
            _recipeRepository.Update(recipe);
            return _recipeRepository.SaveChanges();
        }

        public bool UpdateRecipe(Recipe recipe)
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}"
            };
            var existingRecipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == recipe.Id, includes);
            _mapper.Map(recipe, existingRecipe);
            existingRecipe.UpdatedOn = DateTime.UtcNow;
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

        public Recipe RestoreDeletedRecipe(int id)
        {
            var recipeToRestore = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id);
            recipeToRestore.DeletedOn = null;
            _recipeRepository.Update(recipeToRestore);

            return _recipeRepository.SaveChanges() ? _mapper.Map<Recipe>(recipeToRestore) : null;
        }

        public bool ToggleRecipeLike(int recipeId, int userId, bool like)
        {
            if (like)
            {
                _recipeRepository.Insert(new Entities.RecipeLike
                {
                    RecipeId = recipeId,
                    UserId = userId
                });
            }
            else _recipeRepository.Delete(
                _recipeRepository.GetFirstOrDefault<Entities.RecipeLike>(
                    rl => rl.UserId == userId && rl.RecipeId == recipeId));

            return _recipeRepository.SaveChanges();
        }
    }
}
