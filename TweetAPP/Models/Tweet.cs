using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TweetAPP.Models
{
    public class Tweet
    {
        #region
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [MaxLength(144, ErrorMessage ="Limit reached for tweet message")]
        public string TweetText { get; set; }
        public DateTime TweetTime { get; set; }

        public string Username { get; set; }
        public int Likes { get; set; }

        [MaxLength(50, ErrorMessage = "Limit reached for tweet message")]
        public string Tags { get; set; }
        #endregion
    }
}
