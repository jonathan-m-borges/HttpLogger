using System;
using System.Collections.Generic;

namespace HttpLogger.Models
{
    public class Request
    {
        public int Id { get; set; }
        public Logger Logger { get; set; }
        public DateTime DateTime { get; set; }
        public string Method { get; set; }
        public IList<Header> Headers { get; set; }
        public string Body { get; set; }
    }
}