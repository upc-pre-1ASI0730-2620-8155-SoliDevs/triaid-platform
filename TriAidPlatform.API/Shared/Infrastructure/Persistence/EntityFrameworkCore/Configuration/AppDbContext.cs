using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using TriAidPlatform.API.Subscription.Domain.Model;
using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    public DbSet<Payment> Payments { get; set; }

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

        // Primitive collections are stored as JSON text.
        builder.Entity<Plan>()
            .Property(p => p.Features)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
    }
}
