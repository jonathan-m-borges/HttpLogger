using System;
using System.Collections.Generic;

namespace HttpLogger.Models
{
    public class Request
    {
        public DateTime DateTime { get; set; }
        public string Method { get; set; }
        public IList<Header> Headers { get; set; }
        public string Body { get; set; }

        public Request()
        {
            Headers = new List<Header>();
        }
    }
}