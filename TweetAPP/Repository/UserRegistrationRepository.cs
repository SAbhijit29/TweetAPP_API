using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetAPP.Models;
using TweetAPP.MongoDbContext;

namespace TweetAPP.Repository
{
    public class UserRegistrationRepository : IUserRegistrationRepository
    {
        private readonly IMongoCollection<UserRegistration> _users;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(UserRegistrationRepository));
        public UserRegistrationRepository(ITweetDatabaseSettings settings)
        {
            try
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);

                _users = database.GetCollection<UserRegistration>("UserRegistration");
                _log4net.Info("MongoDb connected with database:" +database);
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while connecting database :" + ex.Message);
                throw ex.InnerException;
            }

        }

        public List<User> GetAllUsers()
        {
            try
            {
                _log4net.Info("Repository called from Service to get the list of all users");
                List<UserRegistration> x =  _users.Find(usr => true).ToList();
                List<User> user = new List<User>();
                foreach (var item in x)
                {
                    User u = new User();
                    u.Fullname = item.FirstName+" "+ item.LastName;
                    u.Email = item.Email;
                    u.Username = item.Username;
                    u.Phone = item.Phone;

                    user.Add(u);
                }
                return user;
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while fetching all users list :" + ex.Message);
                throw ex.InnerException;
            }
         
        }

        public async Task<UserRegistration> CreateUserAsync(UserRegistration user)
        {
            UserRegistration usr = user;
            var x = _users.FindAsync<UserRegistration>(c => c.Username == user.Username).Result.FirstOrDefaultAsync();
            var y = _users.FindAsync<UserRegistration>(c => c.Email == user.Email).Result.FirstOrDefaultAsync();
            try
            {
                _log4net.Info("Repository called from Service to get register user");
                if (x.Result == null)
                {
                    if (y.Result == null)
                    {
                        var pwd = user.Password;
                        byte[] encode = new byte[usr.Password.Length];
                        encode = Encoding.UTF8.GetBytes(pwd);
                        usr.Password = Convert.ToBase64String(encode);
                        usr.ConfirmPassword = usr.Password;
                        await _users.InsertOneAsync(usr);
                    }
                    else
                        return null;
                }
                else
                {
                    if(x.Result.Username != user.Username)
                    {
                        if(x.Result.Email != user.Email)
                        {
                            if(y.Result.Username != user.Username)
                            {
                                if (y.Result.Email != user.Email)
                                {
                                    var pwd = user.Password;
                                    byte[] encode = new byte[usr.Password.Length];
                                    encode = Encoding.UTF8.GetBytes(pwd);
                                    usr.Password = Convert.ToBase64String(encode);
                                    usr.ConfirmPassword = usr.Password;
                                    await _users.InsertOneAsync(usr);
                                }
                                else
                                    return null;
                            }
                            return null;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while Creating user to db :" + ex.Message);
                throw ex.InnerException;
            }
            return user;
        }

        public async Task<Login> GetByEmailAsync(Login login)
        {
            Login user = new Login();
            try
            {
                _log4net.Info("Repository called from Service to get the username & passowrd for login purpose");
                var x = new UserRegistration();
                x = await _users.FindAsync<UserRegistration>(c => c.Username == login.Username).Result.FirstOrDefaultAsync();
                user.Username = x.Username;
                var base64EncodedBytes = System.Convert.FromBase64String(x.Password);
                var pwd = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                user.Password = pwd;
            }
            catch (Exception ex)
            {
                _log4net.Error("Exception occured while collecting username & password for forgot password :" + ex.Message);
                return user = null;
            }

            return user;
        }

        public UserRegistration getUsertbyUserName(string username)
        {
            _log4net.Info("Repository called from Service to get the userdetails from its username");
            var isUser = new UserRegistration();
            isUser = _users.Find<UserRegistration>(x => x.Username == username).FirstOrDefault();
            return isUser;
        }

        public User searchUserbyUsername(string username)
        {
            _log4net.Info("Repository called from Service to get the user from its username");
            var isUser = new UserRegistration();
            User u = new User();
            isUser = _users.Find<UserRegistration>(x => x.Username == username).FirstOrDefault();

            if (isUser != null)
            {
                u.Fullname = isUser.FirstName + " " + isUser.LastName;
                u.Email = isUser.Email;
                u.Username = isUser.Username;
                u.Phone = isUser.Phone;
                return u;
            }
             return null;
        }

        public string forgotPassword(string username,string password)
        {
            var usr = getUsertbyUserName(username);
            try
            {
                if (usr != null)
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
                    var pwd = System.Convert.ToBase64String(plainTextBytes);

                    _log4net.Info("Repository called from Service to implement forgot password");
                    var filterDefinition = Builders<UserRegistration>.Filter.Eq(u => u.Username, username);

                    var updateDefinition = Builders<UserRegistration>.Update.Set(u => u.Password, pwd);
                    _users.UpdateOne(filterDefinition, updateDefinition);

                    var updateDefinitiont = Builders<UserRegistration>.Update.Set(u => u.ConfirmPassword, pwd);
                    _users.UpdateOne(filterDefinition, updateDefinitiont);

                    return "updated";

                }
                else
                    return "not found";

            }
            catch (Exception ex)
            {

                _log4net.Error("Exception occured while implementing forgot password :" + ex.Message);
                return ex.Message;
            }
        }


    }
}
