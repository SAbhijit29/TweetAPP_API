using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPP.MongoDbContext
{
    public class TweetDatabaseSettings : ITweetDatabaseSettings
    {
        public string TweetCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
