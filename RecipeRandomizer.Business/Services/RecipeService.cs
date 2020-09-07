using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models;
using RecipeRandomizer.Business.Utils.Exceptions;
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
        private readonly IFileService _fileService;

        public RecipeService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService)
        {
            _recipeRepository = new RecipeRepository(context);
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _fileService = fileService;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipes());
        }

        public int GetPublishedRecipeCount()
        {
            return _recipeRepository.GetPublishedRecipeCount();
        }

        public IEnumerable<Recipe> GetDeletedRecipes()
        {
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetRecipes(true));
        }

        public IEnumerable<Recipe> GetOrphanRecipes()
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}",
                $"{nameof(Entities.Recipe.RecipeLikes)}"
            };
            return _mapper.Map<IEnumerable<Recipe>>(_recipeRepository.GetAll<Entities.Recipe>(r => !r.UserId.HasValue, includes));
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

        public bool UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id)
        {
            var recipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == id);
            if (recipe == null)
                throw new KeyNotFoundException("Recipe to add image to could not be found");

            try
            {
                var proposedFileExtension = Path.GetExtension(untrustedFileName);
                _fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

                // delete old recipe image (if any) to avoid file clutter
                var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot");
                if(!string.IsNullOrWhiteSpace(recipe.ImageUri))
                    _fileService.DeleteExistingFile(Path.Combine(physicalRoot, recipe.ImageUri));

                // save new recipe image
                var trustedFileName = Guid.NewGuid() + proposedFileExtension;
                _fileService.SaveFileToDisk(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

                recipe.ImageUri = Path.Combine(_appSettings.RecipeImagesFolder, trustedFileName);
                recipe.OriginalImageName = untrustedFileName;
                recipe.UpdatedOn = DateTime.UtcNow;
                _recipeRepository.Update(recipe);
                return _recipeRepository.SaveChanges();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw new BadRequestException(e.Message);
            }
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
            else
                _recipeRepository.Delete(
                    _recipeRepository.GetFirstOrDefault<Entities.RecipeLike>(
                        rl => rl.UserId == userId && rl.RecipeId == recipeId));

            return _recipeRepository.SaveChanges();
        }

        public bool AttributeRecipe(AttributionRequest request)
        {
            var recipe = _recipeRepository.GetFirstOrDefault<Entities.Recipe>(r => r.Id == request.RecipeId);

            if (recipe == null)
                throw new KeyNotFoundException("Recipe does not exist");

            recipe.UserId = request.UserId;
            recipe.UpdatedOn = DateTime.UtcNow;
            _recipeRepository.Update(recipe);
            return _recipeRepository.SaveChanges();
        }
    }
}
