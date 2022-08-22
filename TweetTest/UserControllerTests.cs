using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetAPP.Controllers;
using TweetAPP.Models;
using TweetAPP.MongoDbContext;
using TweetAPP.Repository;
using TweetAPP.Service;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace TweetTest
{
    class UserControllerTests
    {
        private Mock<IUserRegistrationRepository> _UserRepository;
        private Mock<ITweetDatabaseSettings> _tweetdb;
        private RegistrationController _userRegistrationController;
        private UserRegistrationService _userRegistrationService;
        private LoginController _loginController;
        private readonly IConfiguration config;

        public UserControllerTests()
        {
            _tweetdb = new Mock<ITweetDatabaseSettings>();
            _UserRepository = new Mock<IUserRegistrationRepository>();
            _userRegistrationService = new UserRegistrationService(_UserRepository.Object);
            _userRegistrationController = new RegistrationController(_userRegistrationService);
            _loginController = new LoginController(config,_userRegistrationService);

            
        }

        [Test]
        public void getAllTweetsTest()
        {
            List<User> testTweet = new List<User>();

            _UserRepository.Setup(x => x.GetAllUsers()).Returns(testTweet);
            var res = _userRegistrationController.GetAllUsers();

            Assert.AreEqual(testTweet, res);
        }

        [Test]
        public async Task RegistrationofUser()
        {
            UserRegistration user = new UserRegistration();

            _UserRepository.Setup(x => x.CreateUserAsync(It.IsAny<UserRegistration>())).ReturnsAsync(user);
            var res = await _userRegistrationController.Register(user) as OkObjectResult;
         
            Assert.AreEqual((int)HttpStatusCode.OK, res.StatusCode);
           
        }

        [Test]
        public void ForgotPassword()
        {

            _UserRepository.Setup(x => x.forgotPassword(It.IsAny<string>(),It.IsAny<string>())).Returns("updated");
            var res = _loginController.UpdatePassword("Abhi51","Test") as OkObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, res.StatusCode);

        }


        [Test]
        public void getUserbyUsername()
        {
            User user = new User();

            _UserRepository.Setup(x => x.searchUserbyUsername(It.IsAny<string>())).Returns(user);
            var res = _loginController.searchUserbyUsername("Abhi51");

            Assert.AreEqual(user, res);

        }

        [Test]
        public void AuthenticateUser()
        {
            Login user = new Login();
            user.Username = "Abhi51";
            user.Password = "abhi@21";

            var res = _loginController.Login(user) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, res.StatusCode);

        }

    }
}
