using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;

namespace TweetAPP.Service
{
    public interface ItweetService
    {
        List<Tweet> GetAllTweets();

        Task<List<Tweet>> GetTweetsByUsername(string username);
        Task<List<Reply>> GetReplyonTweet(string tweetId);

        string likes(string Id);

        bool PostTweet(string username, Tweet tweet);

        bool UpdateTweet(string tweetId, Tweet tweet);

        bool DeleteTweet(string tweetId, string username);

        string replyPost(string Id, Reply reply, string username);

    }
}
