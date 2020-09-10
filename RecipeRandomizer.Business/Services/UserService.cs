using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IFileService _fileService;

        public UserService(RRContext context, IMapper mapper, IOptions<AppSettings> appSettings, IFileService fileService)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _fileService = fileService;
        }

        public IEnumerable<User> GetUsers()
        {
            return _mapper.Map<IEnumerable<User>>(_userRepository.GetAllAsync<Entities.User>(null, $"{nameof(Entities.User.Role)}"));
        }

        public User GetUser(int id)
        {
            return _mapper.Map<User>(_userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id, $"{nameof(Entities.User.Role)}"));
        }

        public User Update(int id, UserUpdateRequest userUpdateRequest)
        {
            var user = _userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // check if email is not already taken
            if (user.Email != userUpdateRequest.Email && _userRepository.Exists<Entities.User>(u => u.Email == user.Email))
                throw new BadRequestException($"Email '{userUpdateRequest.Email}' is already taken");

            // update user
            user.Username = userUpdateRequest.Username;
            user.Email = userUpdateRequest.Email;
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if (!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return _mapper.Map<User>(user);
        }

        public User Update(int id, RoleUpdateRequest roleUpdateRequest)
        {
            if (_userRepository.AdminCount() <= 1)
                throw new BadRequestException("The last admin can't demote himself!");

            var user = _userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // update role
            user.RoleId = (int) roleUpdateRequest.Role;
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if (!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return _mapper.Map<User>(user);
        }

        public bool UploadUserAvatar(Stream sourceStream, string untrustedFileName, int id)
        {
            var user = _userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id);
            if (user == null)
                throw new KeyNotFoundException("User to add avatar to could not be found");

            try
            {
                var proposedFileExtension = Path.GetExtension(untrustedFileName);
                 _fileService.CheckForAllowedSignature(sourceStream, proposedFileExtension);

                 // delete old avatar (if any) to avoid file clutter
                 var physicalRoot = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot");
                 if (!string.IsNullOrWhiteSpace(user.ProfileImageUri))
                    _fileService.DeleteExistingFile(Path.Combine(physicalRoot, user.ProfileImageUri));

                 // save new avatar
                var trustedFileName = Guid.NewGuid() + proposedFileExtension;
                _fileService.SaveFileToDisk(sourceStream, Path.Combine(physicalRoot, _appSettings.UserAvatarsFolder), trustedFileName);

                user.ProfileImageUri = Path.Combine(_appSettings.UserAvatarsFolder, trustedFileName);
                user.UpdatedOn = DateTime.UtcNow;
                _userRepository.Update(user);

                return _userRepository.SaveChanges();
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw new BadRequestException(e.Message);
            }
        }

        public bool Delete(int id)
        {
            if (_userRepository.AdminCount() <= 1)
                throw new BadRequestException("The last admin can't delete his account!");

            var user = _userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException("User to delete could not be found.");

            _userRepository.Delete(user);
            return _userRepository.SaveChanges();
        }

        public bool ToggleUserLock(int id, LockRequest lockRequest)
        {
            if (lockRequest.LockedById.HasValue && lockRequest.LockedById == id)
                throw new BadRequestException("Locking yourself is not allowed!");

            var user = _userRepository.GetFirstOrDefaultAsync<Entities.User>(u => u.Id == id);

            user.LockedOn = lockRequest.UserLock
                ? (DateTime?) DateTime.UtcNow
                : null;

            user.LockedById = lockRequest.LockedById;

            _userRepository.Update(user);
            return _userRepository.SaveChanges();
        }
    }
}
