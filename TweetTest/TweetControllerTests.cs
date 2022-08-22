using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetAPP.Controllers;
using TweetAPP.Repository;
using TweetAPP.Service;
using TweetAPP.MongoDbContext;
using Moq;
using NUnit.Framework;
using TweetAPP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TweetTest
{
    public class TweetControllerTests
    {
        private Mock<ItweetRepository> _tweetRepository;
        private Mock<ITweetDatabaseSettings> _tweetdb;
        private TweetController _tweetController;
        private tweetService _tweetService;

        public TweetControllerTests()
        {
            _tweetdb = new Mock<ITweetDatabaseSettings>();
           _tweetRepository = new Mock<ItweetRepository>();
           _tweetService = new tweetService(_tweetRepository.Object);
            _tweetController = new TweetController(_tweetService);
        }
        
        [Test]
        public void getAllTweetsTest()
        {
            List<Tweet> testTweet = new List<Tweet>();

            _tweetRepository.Setup(x => x.GetAllTweets()).Returns(testTweet);
            var res = _tweetController.GetAllTweets();

            Assert.AreEqual(testTweet,res);
        }

        [Test]
        public async Task GetTweetbyUsernameTest()
        {
          IEnumerable<Tweet> testTweet = new List<Tweet>();

            _tweetRepository.Setup<Task<IEnumerable<Tweet>>>(x=>x.GetTweetsByUsername("Abhi51")).Returns(Task.FromResult(testTweet));
            var res = await _tweetController.GetTweetsByUsername("Abhi51");

            Assert.AreEqual(testTweet, res); //will see
        }

        [Test]
        public async Task GetReplyForTweet()
        {
            IEnumerable<Reply> testReply = new List<Reply>();

            _tweetRepository.Setup<Task<IEnumerable<Reply>>>(x => x.GetReplyonTweet("62e659168d449abb8b98640e")).Returns(Task.FromResult(testReply));
            var res = await _tweetController.GetRepliesforTweet("62e659168d449abb8b98640e");

            Assert.AreEqual(testReply, res); 
        }

        [Test]
        public void Like()
        {
            string str = "liked Successfully";
            string twtId = "62e659168d449abb8b98640e";
            _tweetRepository.Setup(x => x.likes(It.IsAny<string>())).Returns(str);
            var res = _tweetController.like(twtId);

            Assert.AreEqual(str, res);
        }

        [Test]
        public void PostTweet()
        {
            Tweet testTweet = new Tweet();
            testTweet.TweetText = "hi";
           

            _tweetRepository.Setup(x => x.PostTweet(It.IsAny<string>(),It.IsAny<Tweet>())).Returns(true);
            var res = _tweetController.Add("Abhi51",testTweet) as OkObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK,res.StatusCode);
        }

        [Test]
        public void PostTweet_exception()
        {
            Tweet testTweet = new Tweet();
            testTweet.TweetText = "hi";


            _tweetRepository.Setup(x => x.PostTweet(It.IsAny<string>(), It.IsAny<Tweet>())).Returns(false);
            var res = _tweetController.Add("xwz", testTweet) as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Test]
        public void updateTweet()
        {
            Tweet testTweet = new Tweet();

            _tweetRepository.Setup(x => x.UpdateTweet(It.IsAny<string>(), It.IsAny<Tweet>())).Returns(true);
            var res = _tweetController.Update("62e659168d449abb8b98640e", testTweet, "Abhi51") as OkObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public void updateTweet_false()
        {
            Tweet testTweet = new Tweet();

            _tweetRepository.Setup(x => x.UpdateTweet(It.IsAny<string>(), It.IsAny<Tweet>())).Returns(false);
            var res = _tweetController.Update("62e659168d449abb8b98640e", testTweet, "Abhi51") as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Test]
        public void updateTweet_exception()
        {
            Tweet testTweet = new Tweet();

            _tweetRepository.Setup(x => x.UpdateTweet(It.IsAny<string>(), It.IsAny<Tweet>())).Throws(new Exception("Tweet not found"));
            var res = _tweetController.Update("62e659168d449abb8b98640e", testTweet, "Abhi51") as ObjectResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, res.StatusCode);
        }


        [Test]
        public void deleteTweet()
        {
            _tweetRepository.Setup(x => x.DeleteTweet(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            var res = _tweetController.Delete("Abhi51", "62e659978d449abb8b986413") as OkObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, res.StatusCode);
        }

        [Test]
        public void Postreply()
        {
            Reply testReply = new Reply();

            _tweetRepository.Setup(x => x.replyPost(It.IsAny<string>(), It.IsAny<Reply>(), It.IsAny<string>())).Returns("Valid Tweet");
            var res = _tweetController.PostReply("62e659168d449abb8b98640e", testReply, "Abhi51") as OkObjectResult;

            Assert.AreEqual((int)HttpStatusCode.OK, res.StatusCode);
        }


    }
}
