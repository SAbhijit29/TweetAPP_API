using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;

namespace TweetAPP.Repository
{
    public interface ItweetRepository
    {
        List<Tweet> GetAllTweets();

        Task<List<Tweet>>GetTweetsByUsername(string username);

        string likes(string Id);

        bool PostTweet(string username, Tweet tweet);

        bool UpdateTweet(string tweetId, Tweet tweet);

        bool DeleteTweet(string tweetId, string username);

        Task<List<Reply>> GetReplyonTweet(string tweetId);

        string replyPost(string Id, Reply reply, string username);
        //string replyPost(string username, Reply reply, string id);
    }
}
