using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.Service;

namespace TweetAPP.Controllers
{
    [Route("api/v1.0/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TweetController : Controller
    {
        private readonly ItweetService _userService;
        public TweetController(ItweetService userSer)
        {
            _userService = userSer;
        }


        [HttpGet]
        [Route("/api/v1.0/tweets/all")]
        public ResponseModel<List<Tweet>> GetAllTweets()
        {
            var responseModelobj = new ResponseModel<List<Tweet>>();
            try
            {
                List<Tweet> alltweets = _userService.GetAllTweets();

                responseModelobj.Result = alltweets;
                responseModelobj.StatusCode = 200;
                responseModelobj.Message = "All tweets received";
            }
            catch (Exception ex)
            {
                responseModelobj.Result = null;
                responseModelobj.StatusCode = 404;
                responseModelobj.Message = ex.Message;
            }
           
            return responseModelobj;
            //return alltweets;
        }

        [HttpGet]
        [Route("/api/v/1.0/tweets/user/search/{username}")]
        public async Task<ResponseModel<List<Tweet>>> GetTweetsByUsername(string username)
        {
            var responseModelobj = new ResponseModel<List<Tweet>>();
            try
            {
                var t = await _userService.GetTweetsByUsername(username);
                responseModelobj.Result = t;
                responseModelobj.Message = "Tweet tweeted by " + username;
                responseModelobj.StatusCode = 200;
            }
            catch (Exception e)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = e.Message + username;
                responseModelobj.StatusCode = 404;
            }
            return responseModelobj;
        }

        [HttpGet]
        [Route("/api/v1.0/tweets/tweetreplies/{tweetId}")]
        public async Task<ResponseModel<List<Reply>>> GetRepliesforTweet([FromRoute]string tweetId)
        {
            var responseModelobj = new ResponseModel<List<Reply>>();
            try
            {
                var getReply = await _userService.GetReplyonTweet(tweetId);
                responseModelobj.Result = getReply;
                responseModelobj.Message = "get all replied to tweetId: "+ tweetId;
                responseModelobj.StatusCode = 200;
            }
            catch (Exception e)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = e.Message;
                responseModelobj.StatusCode = 404;
            }
            return responseModelobj;
        }

        [HttpPost]
        [Route("/api/v1.0/tweets/{username}/reply/{tweetId}")]
        public IActionResult PostReply([FromRoute]string tweetId, [FromBody]Reply reply, [FromRoute]string username)
        {
            var responseModelobj = new ResponseModel<string>();
            try
            {
                var postRply = _userService.replyPost(tweetId, reply, username);
                responseModelobj.Result = postRply;
                responseModelobj.Message = "Replied successfully to tweetId: " + tweetId;
                responseModelobj.StatusCode = 200;
            }
            catch (Exception e)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = e.Message;
                responseModelobj.StatusCode = 404;
            }
            return Ok(responseModelobj);
        }

        [HttpPatch]
        [Route("/api/v1.0/tweets/update/{tweetId}")]
        public ResponseModel<string> like(string tweetId)
        {
            var responseModelobj = new ResponseModel<string>();
            try
            {
                var like = _userService.likes(tweetId);
                responseModelobj.Result = like;
                responseModelobj.Message ="tweet "+tweetId+" liked successfully";
                responseModelobj.StatusCode = 200;
            }
            catch (Exception e)
            {
                responseModelobj.Result = null;
                responseModelobj.Message = e.Message;
                responseModelobj.StatusCode = 404;
            }
            return responseModelobj;
        }

        [HttpPost]
        [Route("/api/v1.0/tweets/{username}/add")]
        public IActionResult Add([FromRoute]string username, [FromBody]Tweet tweet)
        {
            var responseModelobj = new ResponseModel<bool>();
            try
            {
               bool x = _userService.PostTweet(username, tweet);

                if (x)
                {
                    responseModelobj.Result = x;
                    responseModelobj.Message ="Posted Tweet Successfully for " +username;
                    responseModelobj.StatusCode = 200;
                }
                   
                else
                {
                    responseModelobj.Result = x;
                    responseModelobj.Message = "Cannot post tweetfor "+username;
                    responseModelobj.StatusCode = 400;
                    return BadRequest(responseModelobj);
                }
            }
            catch (Exception e)
            {
                responseModelobj.Result = false;
                responseModelobj.Message = e.Message;
                responseModelobj.StatusCode = 404;
            }
            return Ok(responseModelobj);
        }

        [HttpPut]
        [Route("/api/v1.0/tweets/{username}/update/{tweetId}")]
        public IActionResult Update([FromRoute]string tweetId, [FromBody]Tweet tweet, [FromRoute]string username)
        {
            var responseModelobj = new ResponseModel<bool>();
            try
            {
                bool x = _userService.UpdateTweet(tweetId, tweet);
                if (x)
                {
                    responseModelobj.Result = x;
                    responseModelobj.Message = "Updated Tweet Successfully for " +username;
                    responseModelobj.StatusCode = 200;
                }

                else
                {
                    responseModelobj.Result = x;
                    responseModelobj.Message = "Tweet not found for " + username + "with TweetId: "+tweetId;
                    responseModelobj.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                responseModelobj.Result = false;
                responseModelobj.Message = ex.Message;
                responseModelobj.StatusCode = 404;
                return NotFound(responseModelobj);
            }
            return Ok(responseModelobj);
        }

       [HttpDelete]
        [Route("/api/v1.0/tweets/{username}/delete/{tweetId}")]
        public IActionResult Delete(string tweetId,[FromRoute]string username)
        {
            var responseModelobj = new ResponseModel<bool>();
            try
            {
                var dltTwet = _userService.DeleteTweet(tweetId, username); 
                responseModelobj.Result = false;
                responseModelobj.Message = "Tweet with "+tweetId+"deleted successfully for "+ username;
                responseModelobj.StatusCode = 200;
            }
            catch (Exception ex)
            {
                responseModelobj.Result = false;
                responseModelobj.Message = ex.Message;
                responseModelobj.StatusCode = 404;
            }
            return Ok(responseModelobj);
        }

     
    }
}
