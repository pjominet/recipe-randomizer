using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Identity;

namespace RecipeRandomizer.Business.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<User> GetUsers()
        {
            return _mapper.Map<IEnumerable<User>>(_userRepository.GetAll<Entities.User>(null, $"{nameof(Entities.User.Role)}"));
        }

        public User GetUser(int id)
        {
            return _mapper.Map<User>(_userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id, $"{nameof(Entities.User.Role)}"));
        }

        public User Update(int id, UpdateRequest updateRequest)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User not found");

            // check if email is not already taken
            if (user.Email != updateRequest.Email && _userRepository.Exists<Entities.User>(u => u.Email == user.Email))
                throw new BadRequestException($"Email '{updateRequest.Email}' is already taken");

            // map any other value according to the mapping profile
            _mapper.Map(updateRequest, user);
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return _mapper.Map<User>(user);
        }

        public async Task<bool> UploadUserAvatar(Stream imageStream, string untrustedFileName, int id)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);
            if(user == null)
                throw new KeyNotFoundException("Recipe to add image to could not be found");

            var trustedFilePath = _appSettings.UserAvatarsFolder + Guid.NewGuid() + Path.GetExtension(untrustedFileName);
            await using var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "@wwwroot", trustedFilePath), FileMode.Create);

            if (!imageStream.CopyToAsync(fileStream).IsCompletedSuccessfully)
                throw new ApplicationException("File copy failed");

            user.ProfileImageUri = trustedFilePath;
            _userRepository.Update(user);
            return _userRepository.SaveChanges();
        }

        public bool Delete(int id)
        {
            if(_userRepository.AdminCount() <= 1)
                throw new BadRequestException("The last admin can't delete his account!");

            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User to delete could not be found.");

            _userRepository.Delete(user);
            return _userRepository.SaveChanges();
        }

        public bool ToggleUserLock(int id, LockRequest lockRequest)
        {
            return false;
        }
    }
}
