using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Factories;

/// <summary>
///     Design-time factory so `dotnet ef` can create the DbContext without
///     booting the full host.
/// </summary>
public class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=triaid_platform;Username=postgres;Password=triaid2026;")
            .Options;
        return new AppDbContext(options);
    }
}
