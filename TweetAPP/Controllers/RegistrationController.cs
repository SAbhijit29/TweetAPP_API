using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Repository;
using TweetAPP.MongoDbContext;
using TweetAPP.Models;
using TweetAPP.Service;
using Microsoft.AspNetCore.Authorization;

namespace TweetAPP.Controllers
{
    [Authorize]
    [Route("api/v1.0/[controller]/[action]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        private readonly IUserRegistrationService _userService;

       
        public RegistrationController(IUserRegistrationService userSer)
        {
            _userService = userSer;
        }


        [HttpGet]
        [Route("/api/v1.0/users/allUsers")]
        public ResponseModel<List<User>> GetAllUsers()
        {
            var responseModelobj = new ResponseModel<List<User>>();
            try
            {
                List<User> allUsers = _userService.GetAllUsers();
                responseModelobj.Result = allUsers;
                responseModelobj.StatusCode = 200;
                responseModelobj.Message = "ALl user list";
            }
            catch (Exception ex)
            {
                responseModelobj.Result = null;
                responseModelobj.StatusCode = 404;
                responseModelobj.Message = "No user found";
                throw ex.InnerException;
            }
            return responseModelobj;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/v1.0/users/register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration user)
        {
             var responseModelobj = new ResponseModel<string>();

            if (!ModelState.IsValid)
            {
                responseModelobj.StatusCode = 400;
                responseModelobj.Message = "Validation error";
                responseModelobj.Result = null;
              //  return responseModelobj;
            }
            var usr = await _userService.CreateUserAsync(user);
            if (usr!=null)
            {
                responseModelobj.StatusCode = 200;
                responseModelobj.Message = "user registered successfully";
                responseModelobj.Result = user.Id;
              //  return responseModelobj;
                // return Ok(user.Id);
            }
            else
            {
                responseModelobj.StatusCode = 400;
                responseModelobj.Message = "Username/Email already exists!";
                responseModelobj.Result = null;
               // return responseModelobj;
               // return BadRequest("Username/Email already exists!");
            }

            return Ok(responseModelobj);
        }


    }
}
