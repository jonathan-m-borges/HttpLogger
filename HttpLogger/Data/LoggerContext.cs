using HttpLogger.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpLogger.Data
{
    public class LoggerContext : DbContext
    {
        public LoggerContext(DbContextOptions<LoggerContext> options)
            : base(options)
        {
        }

        public DbSet<Logger> Loggers { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Header> Headers { get; set; }
    }
}