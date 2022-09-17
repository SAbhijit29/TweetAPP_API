using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;

namespace TweetAPP.Repository
{
    public interface IUserRegistrationRepository
    {
        List<User> GetAllUsers();
        Task<UserRegistration> CreateUserAsync(UserRegistration user);
        Task<Login> GetByEmailAsync(Login login);
        UserRegistration getUsertbyUserName(string username);
        string forgotPassword(ForgotPassword login);

        User searchUserbyUsername(string username);
        string UploadImage(IFormFile formFile, string username);
    }
}
