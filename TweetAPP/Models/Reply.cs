using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPP.Models
{
    public class Reply
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TweetId { get; set; }
        public DateTime ReplyTime { get; set; }

        [MaxLength(144, ErrorMessage = "Limit reached for reply tweet message")]
        public string ReplyText { get; set; }
        public string ReplyUsername { get; set; }

        [MaxLength(50, ErrorMessage = "Limit reached for tweet message")]
        public string Tags { get; set; }
    }
}
