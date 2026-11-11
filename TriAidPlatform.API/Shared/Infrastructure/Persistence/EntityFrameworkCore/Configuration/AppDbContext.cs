using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // The MySQL connector stores DateOnly as datetime on this setup.
        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>();
        configurationBuilder.Properties<DateOnly?>()
            .HaveConversion<NullableDateOnlyConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyPatientRegistrationConfiguration();
        builder.ApplyAlertingConfiguration();
        builder.ApplyTriageClassificationConfiguration();
        builder.ApplySpecialtyAssignmentConfiguration();
        builder.ApplyDevicesConfiguration();
        builder.ApplyIamConfiguration();
        builder.UseSnakeCaseNamingConvention();

        // Primitive collections are stored as JSON text.
        builder.Entity<Plan>()
            .Property(p => p.Features)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
            .Metadata.SetValueComparer(
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<string>>(
                    (a, b) => a!.SequenceEqual(b!),
                    list => list.Aggregate(0, (hash, item) => HashCode.Combine(hash, item)),
                    list => list.ToList()));
    }
}

/// <summary>Converts DateOnly properties to DateTime for the MySQL provider.</summary>
public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime)) { }
}

/// <summary>Converts nullable DateOnly properties to DateTime for the MySQL provider.</summary>
public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
{
    public NullableDateOnlyConverter() : base(
        dateOnly => dateOnly.HasValue ? dateOnly.Value.ToDateTime(TimeOnly.MinValue) : null,
        dateTime => dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null) { }
}
