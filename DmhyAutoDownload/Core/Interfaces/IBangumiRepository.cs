using DmhyAutoDownload.Core.Data.Models;

namespace DmhyAutoDownload.Core.Interfaces;

public interface IBangumiRepository
{
    Task<ICollection<Bangumi>> GetAllBangumisAsync();
    
    Task<ICollection<Bangumi>> GetBangumisAsync(bool finished);
    
    Task<Bangumi?> GetBangumiAsync(string name);
    
    Task<Bangumi> AddOrUpdateBangumiAsync(Bangumi bangumi);
    
    Task<bool> TryAddBangumiAsync(Bangumi bangumi);
    
    Task<bool> TryUpdateBangumiAsync(Bangumi bangumi);
    
    Task<bool> TryDeleteBangumiAsync(string name);
    
    Task<bool> TryMarkAsFinishedAsync(string name, bool finished);
}