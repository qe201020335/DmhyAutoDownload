using DmhyAutoDownload.Core.Data.Models;
using DmhyAutoDownload.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DmhyAutoDownload.Core.Data;

internal class BangumiRepository: IBangumiRepository
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

    public async Task<Bangumi?> GetBangumiAsync(string name)
    {
        return await _db.Bangumis.FindAsync(name);
    }

    public async Task<Bangumi> AddOrUpdateBangumiAsync(Bangumi bangumi)
    {
        _db.Bangumis.Update(bangumi);
        await _db.SaveChangesAsync();
        return (await GetBangumiAsync(bangumi.Name))!;
    }

    public async Task<bool> TryAddBangumiAsync(Bangumi bangumi)
    {
        if (await GetBangumiAsync(bangumi.Name) is not null)
        {
            _logger.LogWarning("Bangumi {Name} already exists", bangumi.Name);
            return false;
        }
        
        _db.Bangumis.Add(bangumi);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryUpdateBangumiAsync(Bangumi bangumi)
    {
        if (await GetBangumiAsync(bangumi.Name) is null)
        {
            _logger.LogWarning("Existing bangumi {Name} not found", bangumi.Name);
            return false;
        }
        
        _db.Bangumis.Update(bangumi);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryDeleteBangumiAsync(string name)
    {
        var bangumi = await GetBangumiAsync(name);
        if (bangumi is null)
        {
            _logger.LogWarning("Bangumi {Name} not found", name);
            return false;
        }
        
        _db.Bangumis.Remove(bangumi);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryMarkAsFinishedAsync(string name, bool finished)
    {
        var bangumi = await GetBangumiAsync(name);
        if (bangumi is null)
        {
            _logger.LogWarning("Bangumi {Name} not found", name);
            return false;
        }
        
        bangumi.Finished = finished;
        await _db.SaveChangesAsync();
        return true;
    }
}