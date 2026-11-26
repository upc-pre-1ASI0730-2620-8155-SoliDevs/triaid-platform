using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace TriAidPlatform.API.Incidents.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Provides Entity Framework Core mapping extensions for the Incidents bounded context.
/// </summary>
/// <remarks>
/// This class defines the database mapping of the <see cref="Incident"/> aggregate,
/// including scalar properties and its owned value objects.
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies the relational configuration for the <see cref="Incident"/> aggregate.
    /// </summary>
    /// <remarks>
    /// Configures the strongly typed incident identifier, required and optional scalar properties,
    /// and the owned value objects for clinic, metric and incident status.
    /// </remarks>
    /// <param name="builder">The model builder for the shared application database context.</param>
    public static void ApplyIncidentsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Incident>().HasKey(i => i.Id);
        builder.Entity<Incident>().Property(i => i.Id)
            .HasConversion(new StronglyTypedIdConverter<IncidentId, int>(id => id.Value, value => new IncidentId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Entity<Incident>().Property(i => i.ReadingId).IsRequired();
        builder.Entity<Incident>().Property(i => i.EpisodeId).IsRequired();
        builder.Entity<Incident>().Property(i => i.Institution).IsRequired().HasMaxLength(150);
        builder.Entity<Incident>().Property(i => i.PatientName).HasMaxLength(150);
        builder.Entity<Incident>().Property(i => i.Dni).HasMaxLength(20);
        builder.Entity<Incident>().Property(i => i.Priority).HasMaxLength(20);
        builder.Entity<Incident>().Property(i => i.Level).IsRequired();
        builder.Entity<Incident>().Property(i => i.RaisedAt).IsRequired();
        builder.Entity<Incident>().Property(i => i.EscalatedTo).HasMaxLength(100);

        // Value Object: ClinicId
        builder.Entity<Incident>().OwnsOne(i => i.ClinicId, c =>
        {
            c.WithOwner().HasForeignKey("Id");
            c.Property(p => p.Value).HasColumnName("ClinicId").IsRequired();
        });

        // Value Object: IncidentMetric
        builder.Entity<Incident>().OwnsOne(i => i.Metric, m =>
        {
            m.WithOwner().HasForeignKey("Id");
            m.Property(p => p.SignName).HasColumnName("MetricSignName").IsRequired();
            m.Property(p => p.MeasuredValue).HasColumnName("MetricMeasuredValue").IsRequired();
            m.Property(p => p.SafeRange).HasColumnName("MetricSafeRange").IsRequired();
        });

        // Value Object: IncidentStatus
        builder.Entity<Incident>().OwnsOne(i => i.Status, s =>
        {
            s.WithOwner().HasForeignKey("Id");
            s.Property(p => p.StatusValue).HasColumnName("Status").IsRequired();
        });
    }
}