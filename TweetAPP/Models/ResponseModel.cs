using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPP.Models
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public T Result { get; set; }
    }
}
