using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttpLogger.Models
{
    public class Logger
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("([a-z]|[A-Z]|[0-9]|-|/.)+", ErrorMessage = "Type letters or numbers")]
        public string Name { get; set; }

        public IList<Request> Requests { get; set; }
    }
}