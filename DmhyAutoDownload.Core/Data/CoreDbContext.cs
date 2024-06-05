using System.Reflection;
using DmhyAutoDownload.Core.Data.Attributes;
using DmhyAutoDownload.Core.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DmhyAutoDownload.Core.Data;

internal class CoreDbContext: DbContext
{
    public CoreDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Bangumi> Bangumis { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var bangumiProps = modelBuilder.Entity<Bangumi>().Metadata.ClrType.GetRuntimeProperties();
        var props = from p in bangumiProps
            let attribute = p.GetCustomAttribute<MappedAttribute>()
            where attribute is not null && p.GetMethod != null && !p.GetMethod.IsStatic && !p.GetMethod.IsPublic
            select (p, attribute);
        
        foreach (var (prop, attribute) in props)
        {
            modelBuilder.Entity<Bangumi>().Property(prop.Name).IsRequired(attribute.IsRequired);
        }
    }
}