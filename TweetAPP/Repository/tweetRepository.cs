using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.MongoDbContext;

namespace TweetAPP.Repository
{
    public class tweetRepository : ItweetRepository
    {
        private readonly IMongoCollection<Tweet> _tweet;
        private readonly IMongoCollection<UserRegistration> _users;
        private readonly IMongoCollection<Reply> _reply;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(tweetRepository));
        public tweetRepository(ITweetDatabaseSettings settings)
        {
            try
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);

                _tweet = database.GetCollection<Tweet>("TweetCollection");
                _users = database.GetCollection<UserRegistration>("UserRegistration");
                _reply = database.GetCollection<Reply>("ReplyCollection");
                _log4net.Info("MongoDb connected with database:" + database);
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while connecting database :" + ex.Message);
                throw ex.InnerException;
            }

        }
        public List<Tweet> GetAllTweets()
        {
            try
            {
                _log4net.Info("Repository called from Service to get the list of all tweets");
                return _tweet.Find(twt => true).ToList();
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while fetching all users tweets :" + ex.Message);
                throw;
            }
        }

        public async Task<List<Tweet>> GetTweetsByUsername(string username)
        {
            try
            {
                _log4net.Info("Repository called from Service to get tweets by username");
                return await _tweet.Find(x => x.Username == username).ToListAsync();
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while fetching all tweets by username :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public async Task<List<Reply>> GetReplyonTweet(string tweetId)
        {
            try
            {
                _log4net.Info("Repository called from Service to get the replies of tweets");
                return await _reply.Find(x => x.TweetId == tweetId).ToListAsync();
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while fetching replies of tweet :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public string replyPost(string id, Reply reply, string username)
        {
            try
            {
                if (id.Length != 24)
                {
                    _log4net.Error("Not a valid tweet Id");
                    return "Post not found";
                }

                var filter = Builders<Tweet>.Filter.Eq(x => x.Id, id);
                Tweet tweet = _tweet.Find(filter).FirstOrDefault();

                if (tweet != null && reply.ReplyText!=null)
                {
                    Reply twetReply = new Reply();
                    twetReply.TweetId = tweet.Id;
                    twetReply.ReplyText = reply.ReplyText;
                    twetReply.ReplyTime = DateTime.UtcNow;
                    twetReply.ReplyUsername = username;
                    twetReply.Tags = reply.Tags;

                    _reply.InsertOne(twetReply);
                    _log4net.Info("Repository called from Service to reply on tweet");
                    return "Replied";

                }

                else
                {
                    _log4net.Error("Invalid tweet/ tweet not found");
                    return "inValid tweet";
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while posting reply to tweet :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public string likes(string id)
        {
            try
            {
                var tweet = _tweet.Find(x => x.Id == id).FirstOrDefault();
                if (tweet != null){
                    tweet.Likes += 1;
                    _tweet.ReplaceOne(x => x.Id == id, tweet);
                    _log4net.Info("Repository called from Service to count on likes of tweet");
                    return "liked Successfully";
                }
                else
                {
                    _log4net.Error("Tweet not exists");
                    return "Invalid Tweet!";
                }
               
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while counting likes :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public UserRegistration getUsertbyUserName(string username)
        {
            _log4net.Info("Repository called from Service to get the userdetails from its username");
            var isUser = new UserRegistration();
            isUser = _users.Find<UserRegistration>(x => x.Username == username).FirstOrDefault();
            return isUser;
        }

        public bool PostTweet(string username, Tweet twt)
        {
            Tweet tweet = new Tweet();
            try
            {
                tweet.Username = username;
                var x = getUsertbyUserName(username);
                if (x!=null && twt.TweetText!=null)
                {
                    tweet.Likes = 0;
                    tweet.TweetText = twt.TweetText;
                    tweet.TweetTime = DateTime.UtcNow;
                    tweet.Tags = twt.Tags;
                    _tweet.InsertOne(tweet);
                    _log4net.Info("Repository called from Service to post tweet ");
                    return true;
                }
                else
                {
                    return false;
                }
               
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while posting tweet :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public Tweet getPostbyID(string Id)
        {
            try
            {
                Tweet isTweet = _tweet.Find(x => x.Id == Id).FirstOrDefault();
                _log4net.Info("Repository called from Service to get post via id");
                return isTweet;
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while collecting tweet by id :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public bool UpdateTweet(string tweetId, Tweet tweet)
        {
            try
            {
                Tweet twt = getPostbyID(tweetId);
                twt.Id = tweetId;
                twt.Likes = tweet.Likes;
                twt.Username = twt.Username;
                twt.TweetTime =  DateTime.UtcNow;
                twt.TweetText = tweet.TweetText;
                twt.Tags = tweet.Tags;
                if (twt.TweetText != null)
                {
                    _tweet.ReplaceOne(x => x.Id == tweetId, twt);
                    _log4net.Info("Repository called from Service to update tweet ");
                    return true;
                }
                else
                {
                    _log4net.Error("not a valid tweet response/ invalid tweet message");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while updating tweet :" + ex.Message);
                throw ex.InnerException;
            }

        }

        public bool DeleteTweet(string tweetId,string username)
        {
            try
            {
                Tweet twt = getPostbyID(tweetId);
                if (twt.Id != null && twt.Username == username)
                {
                    _tweet.DeleteOne(x => x.Id == tweetId);
                    _log4net.Info("Repository called from Service to delete tweet ");
                    return true;
                }
                else
                {
                    _log4net.Error("tweet delete not possible ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while deleting tweet :" + ex.Message);
                throw ex.InnerException;
            }
        }
    }
}
