using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Subscription.Domain.Model;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Extensions;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
   
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);   
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseSnakeCaseNamingConvention();
    }

   
}