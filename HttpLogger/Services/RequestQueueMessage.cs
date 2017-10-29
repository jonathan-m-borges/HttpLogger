using System;
using System.IO;
using HttpLogger.Models;

namespace HttpLogger.Services
{
    public class RequestQueueMessage
    {
        public string Logger { get; set; }
        public Guid Id { get; set; }
        public Request Request { get; set; }
    }
}