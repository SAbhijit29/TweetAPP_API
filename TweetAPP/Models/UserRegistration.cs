using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TweetAPP.Models
{
    public class UserRegistration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [MinLength(1,ErrorMessage ="Please enter valid first name")]
        public string FirstName { get; set; }

        [BsonRequired]
        [MinLength(1, ErrorMessage = "Please enter valid last name")]
        public string LastName { get; set; }

        [BsonRequired]
        [MinLength(3, ErrorMessage = "Please enter valid username")]
        [RegularExpression("^[A-Za-z][A-Za-z0-9_]{2,14}$" , ErrorMessage ="Minimum length 3 required")]
        public string Username { get; set; }

        [BsonRequired]
        [MinLength(8, ErrorMessage = "Please enter valid password with length minimum 8 letters including 1 small, 1 capital, 1 numeric & 1 special in it.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z]).{8,26}$")]
        public string Password { get; set; }

        [BsonRequired]
        [Compare("Password",ErrorMessage ="Password & Confirm Password does not match")]
        public string ConfirmPassword { get; set; }

        [BsonRequired]
        [RegularExpression("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage ="Not a valid email address")]
        public string Email { get; set; }

        [BsonRequired]
        [RegularExpression("^[0-9]{10}$" ,ErrorMessage ="Please enter 10 digit Mobile Number")]
        public double Phone { get; set; }

    }
}
