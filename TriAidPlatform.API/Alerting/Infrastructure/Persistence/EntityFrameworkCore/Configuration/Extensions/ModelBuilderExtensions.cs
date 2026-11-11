using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ModelBuilder"/> to configure Entity Framework Core 
/// mappings for the Alerting bounded context.
/// </summary>
public static class AlertingContextConfigurationExtensions
{
    /// <summary>
    /// Applies the database configuration mappings for the <see cref="Alert"/> aggregate root 
    /// and its associated Value Objects to ensure correct persistence.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder ApplyAlertingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Alert>(alert =>
        {
            alert.HasKey(a => a.Id);
            alert.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
            
            
            alert.Property(a => a.ClinicId).HasConversion(
                c => c.Value, v => new ClinicId(v)).HasColumnName("clinic_id").IsRequired();

            alert.Property(a => a.Status).HasConversion(
                s => s.StatusValue, v => new AlertStatus(v)).HasColumnName("status").IsRequired();

            alert.Property(a => a.Metric).HasConversion(
                m => System.Text.Json.JsonSerializer.Serialize(m, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<AlertMetric>(v, (System.Text.Json.JsonSerializerOptions?)null)!)
                .HasColumnName("metric").IsRequired();
        });

        return builder;
    }
}