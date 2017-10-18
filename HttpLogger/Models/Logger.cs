using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttpLogger.Models
{
    public class Logger
    {
        public string id => Name;

        [Required]
        [RegularExpression("([a-z]|[A-Z]|[0-9]|-|/.)+", ErrorMessage = "Type letters or numbers")]
        public string Name { get; set; }

        public IList<Request> Requests { get; set; }

        public Logger()
        {
            Requests = new List<Request>();
        }
    }
}