using System.Collections.Generic;

namespace HttpLogger.Models
{
    public class Header
    {
        public int Id { get; set; }
        public Request Request { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}