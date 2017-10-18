using System.Collections.Generic;
using System.Threading.Tasks;
using HttpLogger.Models;

namespace HttpLogger.Data
{
    public interface ILoggerRepository
    {
        Task<IEnumerable<Logger>> GetAllAsync();
        Task CreateAsync(Logger logger);
        Task<Logger> GetByNameAsync(string name);
        Task UpdateAsync(Logger logger);
        Task RemoveAsync(string id);
        
    }
}