using DmhyAutoDownload.Core.Data.Models;

namespace DmhyAutoDownload.Core.Interfaces;

public interface IBangumiRepository
{
    Task<ICollection<Bangumi>> GetAllBangumisAsync();
    
    Task<ICollection<Bangumi>> GetBangumisAsync(bool finished);
}