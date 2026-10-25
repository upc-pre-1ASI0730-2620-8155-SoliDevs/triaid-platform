using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Extensions;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseSnakeCaseNamingConvention();
    }
}