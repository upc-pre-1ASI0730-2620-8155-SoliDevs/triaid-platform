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
            .UseMySQL("Server=localhost;Port=3306;Database=triaid_platform;Uid=root;Pwd=triaid2026;")
            .Options;
        return new AppDbContext(options);
    }
}
