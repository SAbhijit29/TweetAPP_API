using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.Repository;

namespace TweetAPP.Service
{
    public class tweetService : ItweetService
    {
        private readonly ItweetRepository tweetRepo;

        public tweetService(ItweetRepository _tweetRepository)
        {
            tweetRepo = _tweetRepository;
        }
        public List<Tweet> GetAllTweets()
        {
            List<Tweet> allUsers = tweetRepo.GetAllTweets(); ;
            return allUsers;
        }

        public async Task<List<Tweet>> GetTweetsByUsername(string username)
        {
            return await tweetRepo.GetTweetsByUsername(username);
        }

        public async Task<List<Reply>> GetReplyonTweet(string tweetId)
        {
            return await tweetRepo.GetReplyonTweet(tweetId);
        }
        public string likes(string Id,string res)
        {
           return tweetRepo.likes(Id,res);
        }

        public bool PostTweet(string username, Tweet tweet)
        {
           return tweetRepo.PostTweet(username, tweet);
        }

        public bool UpdateTweet(string tweetId, Tweet tweet)
        {
            return tweetRepo.UpdateTweet(tweetId, tweet);
        }

         public bool DeleteTweet(string tweetId, string username)
        {
            return tweetRepo.DeleteTweet(tweetId, username);
        }

        public string replyPost(string Id, Reply reply, string username)
        {
            return tweetRepo.replyPost(Id, reply, username);
        }
        public string replyToPost(string Id, Tweet reply, string username)
        {
            return tweetRepo.replyToPost(Id, reply, username);
        }

        public Tweet getPostbyID(string Id)
        {
            return tweetRepo.getPostbyID(Id);
        }
    }

}
