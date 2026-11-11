using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
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
        builder.UseSnakeCaseNamingConvention();
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
