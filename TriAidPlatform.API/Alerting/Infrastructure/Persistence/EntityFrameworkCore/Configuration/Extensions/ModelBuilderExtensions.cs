using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;

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
            
            
            alert.OwnsOne(a => a.ClinicId, c =>
            {
                c.WithOwner().HasForeignKey("Id");
                c.Property(p => p.Value).HasColumnName("clinic_id").IsRequired();
            });

            alert.OwnsOne(a => a.Status, s =>
            {
                s.WithOwner().HasForeignKey("Id");
                s.Property(p => p.StatusValue).HasColumnName("status").IsRequired();
            });

            alert.OwnsOne(a => a.Metric, m =>
            {
                m.WithOwner().HasForeignKey("Id");
                m.Property(p => p.SignName).HasColumnName("metric_sign_name").IsRequired();
                m.Property(p => p.MeasuredValue).HasColumnName("metric_measured_value").IsRequired();
                m.Property(p => p.SafeRange).HasColumnName("metric_safe_range").IsRequired();
            });
        });

        return builder;
    }
}