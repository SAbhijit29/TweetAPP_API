using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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
                /* for Local host connection */
                // var client = new MongoClient(settings.ConnectionString);
                //var database = client.GetDatabase(settings.DatabaseName);

                //_tweet = database.GetCollection<Tweet>("TweetCollection");
                //_users = database.GetCollection<UserRegistration>("UserRegistration");
                //_reply = database.GetCollection<Reply>("ReplyCollection");


                string connectionString =
  @"mongodb://abhijit-dbtweet:Kc7C2jJcvMdoz0uV51WGZt1SJoiWT7QW98QF0NaY83sLgAFSy8DkwE0CIvdELVSxS0nz12FZkbZPGsOl1tWlwg==@abhijit-dbtweet.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@abhijit-dbtweet@";
                MongoClientSettings settings2 = MongoClientSettings.FromUrl(
                  new MongoUrl(connectionString)
                );
                settings2.SslSettings =
                  new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                var client = new MongoClient(settings2);

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


        public string replyToPost(string id, Tweet reply, string Username)
        {
            try
            {
                if (id.Length != 24)
                {
                    _log4net.Error("Not a valid tweet Id");
                    return "Post not found";
                }

                var filter = Builders<Tweet>.Filter.Eq(x => x.Id, id);
                Tweet oldtweet = _tweet.Find(filter).FirstOrDefault();

                if (oldtweet != null && reply.TweetText != null)
                {
                    Tweet tweet = new Tweet();

                    tweet.Username = Username;
                    var x = getUsertbyUserName(Username);
                    if (x != null && reply.TweetText != null)
                    {
                       
                        tweet.TweetText = reply.TweetText;
                        tweet.TweetTime = DateTime.UtcNow;
                        tweet.Tags = reply.Tags;
                        tweet.Fullname = x.FirstName + " " + x.LastName;
                        tweet.LikedBy = new List<string>();
                        tweet.ReplyID = new List<string>();
                        tweet.Likes = tweet.LikedBy.Count();
                        _tweet.InsertOneAsync(tweet);
                        _log4net.Info("reply added as new tweet ");


                        oldtweet.ReplyID.Add(tweet.Id);
                        _tweet.FindOneAndReplace(x => x.Id == oldtweet.Id, oldtweet);
                        _log4net.Info("Repository called from Service to post tweet ");
                        return "Replied";

                    }
                      return "Cannot reply";
                }

                _log4net.Error("Invalid tweet/ tweet not found");
                return "inValid tweet";
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while posting reply to tweet :" + ex.Message);
                throw ex.InnerException;
            }
        }

        public string likes(string id,string username)
        {
            List<string> likedBy = new List<string>();
            try
            {
                var tweet = _tweet.Find(x => x.Id == id).FirstOrDefault();
                if (tweet != null)
                {
                    if (tweet.LikedBy == null)
                    {
                        tweet.LikedBy.Add(username);
                        tweet.Likes = tweet.LikedBy.Count();
                        _tweet.ReplaceOne(x => x.Id == id, tweet);
                        _log4net.Info("Repository called from Service to decrease likes of tweet");
                        return "liked Successfully for" + username;
                    }
                    else if(tweet.LikedBy.Contains(username) == true)
                    {
                        tweet.LikedBy.Remove(username);
                        tweet.Likes = tweet.LikedBy.Count();
                        _tweet.ReplaceOne(x => x.Id == id, tweet);
                        _log4net.Info("Repository called from Service to decrease likes of tweet");
                        return "removed liked for" + username;
                    }
                    else
                    {
                        tweet.LikedBy.Add(username);
                        tweet.Likes = tweet.LikedBy.Count();
                        _tweet.ReplaceOne(x => x.Id == id, tweet);
                        _log4net.Info("Repository called from Service to decrease likes of tweet");
                        return "liked Successfully for" + username;
                    }
                }

                return "tweet not found";
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
                   
                    tweet.TweetText = twt.TweetText;
                    tweet.TweetTime = DateTime.UtcNow;
                    tweet.Tags = twt.Tags;
                    tweet.Fullname = x.FirstName +" "+ x.LastName;
                    tweet.LikedBy = new List<string>() ;
                    tweet.ReplyID = new List<string>();
                    tweet.Likes = tweet.LikedBy.Count();
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
               
                twt.Username = twt.Username;
                twt.TweetTime =  DateTime.UtcNow;
                twt.TweetText = tweet.TweetText;
                twt.Tags = tweet.Tags;
                twt.LikedBy = twt.LikedBy;
                twt.ReplyID = twt.ReplyID;
                twt.Likes = tweet.LikedBy.Count();
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
                    _log4net.Error("tweet can't be deleted ");
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
