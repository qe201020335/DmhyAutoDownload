using DmhyAutoDownload.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DmhyAutoDownload.Core.Data;

public class CoreDbContext: DbContext
{
    public CoreDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Bangumi> Bangumis { get; set; } = null!;
}