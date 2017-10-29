using System;
using System.Collections.Generic;

namespace HttpLoggerModel
{
    public class Request
    {
        public string Logger { get; set; }
        public Guid Id { get; set; }
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