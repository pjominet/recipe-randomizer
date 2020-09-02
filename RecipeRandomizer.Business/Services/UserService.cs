using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using RecipeRandomizer.Business.Interfaces;
using RecipeRandomizer.Business.Models.Identity;
using RecipeRandomizer.Business.Utils.Exceptions;
using RecipeRandomizer.Data.Contexts;
using RecipeRandomizer.Data.Repositories;
using Entities = RecipeRandomizer.Data.Entities.Identity;
using BC = BCrypt.Net.BCrypt;

namespace RecipeRandomizer.Business.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(RRContext context, IMapper mapper)
        {
            _userRepository = new UserRepository(context);
            _mapper = mapper;
        }

        public IEnumerable<User> GetUsers()
        {
            return _mapper.Map<IEnumerable<User>>(_userRepository.GetAll<Entities.User>());
        }

        public User GetUser(int id)
        {
            return _mapper.Map<User>(_userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id));
        }

        public User Update(int id, UpdateUserRequest request)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User not found");

            // validate
            if (user.Email != request.Email && _userRepository.Exists<Entities.User>(u => u.Email == user.Email))
                throw new BadRequestException($"Email '{request.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(request.Password))
                user.PasswordHash = BC.HashPassword(request.Password);

            _mapper.Map(request, user);
            user.UpdatedOn = DateTime.UtcNow;
            _userRepository.Update(user);
            if(!_userRepository.SaveChanges())
                throw new ApplicationException("Database error: Changes could not be saved correctly");

            return _mapper.Map<User>(user);
        }

        public bool UploadUserAvatar(Stream imageStream, int id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            var user = _userRepository.GetFirstOrDefault<Entities.User>(u => u.Id == id);

            if (user == null)
                throw new BadRequestException("User to delete could not be found.");

            _userRepository.Delete(user);
            return _userRepository.SaveChanges();
        }
    }
}
