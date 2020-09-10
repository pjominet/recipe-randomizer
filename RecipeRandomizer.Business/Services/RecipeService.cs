using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Recipe>> GetRecipesAsync(IList<int> tagIds)
        {
            return _mapper.Map<IEnumerable<Recipe>>(tagIds.Any()
                ? await _recipeRepository.GetRecipesFromTagsAsync(tagIds)
                : await _recipeRepository.GetRecipesAsync());
        }

        public async Task<int> GetPublishedRecipeCountAsync()
        {
            return await _recipeRepository.GetPublishedRecipeCountAsync();
        }

        public async Task<IEnumerable<Recipe>> GetDeletedRecipesAsync()
        {
            return _mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetRecipesAsync(true));
        }

        public async Task<IEnumerable<Recipe>> GetAbandonedRecipesAsync()
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}",
                $"{nameof(Entities.Recipe.RecipeLikes)}"
            };
            return _mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetAllAsync<Entities.Recipe>(r => !r.UserId.HasValue, includes));
        }

        public async Task<Recipe> GetRecipeAsync(int id)
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
            return _mapper.Map<Recipe>(await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == id, includes));
        }

        public async Task<int?> GetRandomRecipeIdAsync(IList<int> tagIds)
        {
            return (await _recipeRepository.GetRandomRecipeAsync(tagIds))?.Id;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesForUserAsync(int userId)
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}"
            };
            return _mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetAllAsync<Entities.Recipe>(r => r.UserId == userId, includes));
        }

        public async Task<IEnumerable<Recipe>> GetLikedRecipesForUserAsync(int userId)
        {
            return _mapper.Map<IEnumerable<Recipe>>(await _recipeRepository.GetLikedRecipesForUserAsync(userId));
        }

        public async Task<Recipe> CreateRecipe(Recipe recipe)
        {
            var newRecipe = _mapper.Map<Entities.Recipe>(recipe);
            newRecipe.CreatedOn = DateTime.UtcNow;
            newRecipe.UpdatedOn = DateTime.UtcNow;

            _recipeRepository.Insert(newRecipe);
            var result = await _recipeRepository.SaveChangesAsync();

            if (!result)
                return null;

            foreach (var tag in recipe.Tags)
                _recipeRepository.Insert(new Entities.RecipeTagAssociation {TagId = tag.Id, RecipeId = newRecipe.Id});

            result &= await _recipeRepository.SaveChangesAsync();
            return result ? _mapper.Map<Recipe>(newRecipe) : null;
        }

        public async Task<bool> UploadRecipeImage(Stream sourceStream, string untrustedFileName, int id)
        {
            var recipe = await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == id);
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
                await _fileService.SaveFileToDiskAsync(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

                recipe.ImageUri = Path.Combine(_appSettings.RecipeImagesFolder, trustedFileName);
                recipe.OriginalImageName = untrustedFileName;
                recipe.UpdatedOn = DateTime.UtcNow;
                _recipeRepository.Update(recipe);
                return await _recipeRepository.SaveChangesAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw new BadRequestException(e.Message);
            }
        }

        public async Task<bool> UpdateRecipe(Recipe recipe)
        {
            string[] includes =
            {
                $"{nameof(Entities.Recipe.Cost)}",
                $"{nameof(Entities.Recipe.Difficulty)}",
                $"{nameof(Entities.Recipe.Ingredients)}.{nameof(Entities.Ingredient.QuantityUnit)}",
                $"{nameof(Entities.Recipe.RecipeTagAssociations)}.{nameof(Entities.RecipeTagAssociation.Tag)}"
            };
            var existingRecipe = await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == recipe.Id, includes);
            _mapper.Map(recipe, existingRecipe);
            existingRecipe.UpdatedOn = DateTime.UtcNow;
            _recipeRepository.Update(existingRecipe);
            return await _recipeRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteRecipe(int id, bool hard = false)
        {
            if (hard)
                _recipeRepository.HardDeleteRecipe(id);
            else
            {
                var recipeToDelete = await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == id);
                recipeToDelete.DeletedOn = DateTime.UtcNow;
                _recipeRepository.Update(recipeToDelete);
            }

            return await _recipeRepository.SaveChangesAsync();
        }

        public async Task<Recipe> RestoreDeletedRecipe(int id)
        {
            var recipeToRestore = await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == id);
            recipeToRestore.DeletedOn = null;
            _recipeRepository.Update(recipeToRestore);

            return await _recipeRepository.SaveChangesAsync() ? _mapper.Map<Recipe>(recipeToRestore) : null;
        }

        public async Task<bool> ToggleRecipeLike(int recipeId, LikeRequest request)
        {
            if (request.Like)
            {
                _recipeRepository.Insert(new Entities.RecipeLike
                {
                    RecipeId = recipeId,
                    UserId = request.LikedById
                });
            }
            else
                _recipeRepository.Delete(await _recipeRepository
                    .GetFirstOrDefaultAsync<Entities.RecipeLike>(rl => rl.UserId == request.LikedById && rl.RecipeId == recipeId));

            return await _recipeRepository.SaveChangesAsync();
        }

        public async Task<bool> AttributeRecipeAsync(AttributionRequest request)
        {
            var recipe = await _recipeRepository.GetFirstOrDefaultAsync<Entities.Recipe>(r => r.Id == request.RecipeId);

            if (recipe == null)
                throw new KeyNotFoundException("Recipe does not exist");

            recipe.UserId = request.UserId;
            recipe.UpdatedOn = DateTime.UtcNow;
            _recipeRepository.Update(recipe);
            return await _recipeRepository.SaveChangesAsync();
        }
    }
}
