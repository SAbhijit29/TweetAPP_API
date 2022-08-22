using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;

namespace TweetAPP.Service
{
    public interface IUserRegistrationService
    {
        List<User> GetAllUsers();

        Task<UserRegistration> CreateUserAsync(UserRegistration user);

        Task<Login> GetByEmailAsync(Login login);

        string forgotPassword(string username, string password);

        User searchUserbyUsername(string username);
    }
}
