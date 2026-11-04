using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Extensions;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseSnakeCaseNamingConvention();
        builder.ApplyIamConfiguration();
    }
}
