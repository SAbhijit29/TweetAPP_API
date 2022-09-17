using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.MongoDbContext;
using TweetAPP.Repository;

namespace TweetAPP.Service
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRegistrationRepository  userRepo;
      
        public UserRegistrationService(IUserRegistrationRepository _userRepository)
        {
            userRepo = _userRepository;
        }

        public Task<UserRegistration> CreateUserAsync(UserRegistration user)
        {
            return userRepo.CreateUserAsync(user);
        }

        public List<User> GetAllUsers()
        {
            List<User> allUsers = userRepo.GetAllUsers(); ;
            return allUsers;
        }

        public Task<Login> GetByEmailAsync(Login login)
        {
            return userRepo.GetByEmailAsync(login);
        }

        public string forgotPassword(ForgotPassword login)
        {
            return userRepo.forgotPassword(login);
        }

        public User searchUserbyUsername(string username)
        {
            return userRepo.searchUserbyUsername(username);
        }

        public string UploadImage(IFormFile formFile, string username)
        {
            return userRepo.UploadImage(formFile, username);
        }

    }
}
