using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Data;

public class BangumiRepository: IBangumiRepository
{
    private readonly ILogger<BangumiRepository> _logger;
    
    private readonly CoreDbContext _db;
    
    public BangumiRepository(CoreDbContext db, ILogger<BangumiRepository> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<ICollection<Bangumi>> GetAllBangumisAsync()
    {
        return await _db.Bangumis.ToListAsync();
    }

    public async Task<ICollection<Bangumi>> GetBangumisAsync(bool finished)
    {
        return await _db.Bangumis.Where(b => b.Finished == finished).ToListAsync();
    }
}