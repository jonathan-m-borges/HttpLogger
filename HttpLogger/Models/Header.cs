using System;
using System.Collections.Generic;

namespace HttpLogger.Models
{
    public class Header
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Header()
        {
        }

        public Header(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}