using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.Service;

namespace TweetAPP.Controllers
{
     
    [Route("api/v1.0/[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private readonly IUserRegistrationService _userService;
        public LoginController(IConfiguration config, IUserRegistrationService userSer)
        {
            _config = config;
            _userService = userSer;
        }

        [HttpPost]
        [Route("/api/v1.0/login")]
        public IActionResult Login([FromBody]Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user is null)
            {
                return BadRequest("Invalid Login Attempt");
            }

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString,username = login.Username });
            }
            return response;
        }

        private Login AuthenticateUser(Login login)
        {
            Login user = null;
            var username = _userService.GetByEmailAsync(login);

            //Validate the User Credentials    
            if (username.Result != null)
            {
                if (login.Username == username.Result.Username && login.Password == username.Result.Password)
                {
                    user = new Login { Username = login.Username, Password = login.Password };
                }
                return user;
            }
            else
            {
                return user;
            }
        }

        private string GenerateJSONWebToken(Login userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            ClaimsIdentity Subject = new ClaimsIdentity(new Claim[]  { 
                new Claim(ClaimTypes.Name, "admin")  
            });

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = Subject,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(token);
            return Token;
           
        }

        [HttpPut]
        [Route("/api/v1.0/login/forgotPassword")]
        public ActionResult UpdatePassword([FromBody]ForgotPassword forgot)
        {

            var responseModelobj = new ResponseModel<string>();
            try
            {
                var result = _userService.forgotPassword(forgot);
                if (result.Equals("updated"))
                {
                    responseModelobj.Result = result;
                    responseModelobj.Message = "Updated password successfully for " + forgot.Username;
                    responseModelobj.StatusCode = 200;
                }
                else
                {
                    responseModelobj.Result = "user not found";
                    responseModelobj.Message = "user not found with " + forgot.Username;
                    responseModelobj.StatusCode = 404;
                }
               
            }
            catch (Exception e)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = "Password updation failed for" + forgot.Username + "Error: "+e.Message;
                responseModelobj.StatusCode = 404;
            }
            return Ok(responseModelobj);
        }

        [HttpGet]
        [Route("/api/v1.0/user/search/{username}")]
        public ResponseModel<User> searchUserbyUsername([FromRoute]string username)
        {
            var responseModelobj = new ResponseModel<User>();
            try
            {
                User u = _userService.searchUserbyUsername(username);
                responseModelobj.Result = u;
                responseModelobj.Message ="user found with "+username;
                responseModelobj.StatusCode = 200;
            }
            catch (Exception ex)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = "user not found with " + username + "Error: "+ex.Message;
                responseModelobj.StatusCode = 404;
            }

            return responseModelobj ;
        }


    }
}
