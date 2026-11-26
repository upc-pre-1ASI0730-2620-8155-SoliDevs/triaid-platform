using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;

namespace TriAidPlatform.API.Incidents.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Provides Entity Framework Core mapping extensions for the Incidents bounded context.
/// </summary>
/// <remarks>
/// Defines the relational mapping of the <see cref="Incident"/> aggregate. Value objects are
/// persisted through value conversions instead of owned types so that every incident stays on a
/// single row keyed by <c>Id</c>, matching the physical schema already deployed for alerting
/// (see ADR-010 and the 1.0.0 schema). The legacy <c>alert</c> table is renamed to
/// <c>incidents</c> at startup by <see cref="Schema.IncidentsSchema"/>, so the column names
/// below intentionally keep the snake_case names of that table.
/// </remarks>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies the relational configuration for the <see cref="Incident"/> aggregate.
    /// </summary>
    /// <remarks>
    /// Configures the strongly typed identifier, the scalar columns shared with the legacy
    /// alert table, and the conversions for the clinic, status and metric value objects.
    /// </remarks>
    /// <param name="builder">The model builder for the shared application database context.</param>
    public static void ApplyIncidentsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Incident>(incident =>
        {
            incident.ToTable("incidents");
            incident.HasKey(i => i.Id);
            incident.Property(i => i.Id)
                .HasConversion(new StronglyTypedIdConverter<IncidentId, int>(id => id.Value, value => new IncidentId(value)))
                .IsRequired()
                .ValueGeneratedOnAdd();

            incident.Property(i => i.ReadingId).IsRequired().HasColumnName("reading_id");
            incident.Property(i => i.EpisodeId).IsRequired().HasColumnName("episode_id");
            incident.Property(i => i.Institution).IsRequired().HasMaxLength(150).HasColumnName("institution");
            incident.Property(i => i.PatientName).HasMaxLength(150).HasColumnName("patient_name");
            incident.Property(i => i.Dni).HasMaxLength(20).HasColumnName("dni");
            incident.Property(i => i.Priority).HasMaxLength(20).HasColumnName("priority");
            incident.Property(i => i.Level).IsRequired().HasColumnName("level");
            incident.Property(i => i.RaisedAt).IsRequired().HasColumnName("raised_at");
            incident.Property(i => i.EscalatedTo).HasMaxLength(100).HasColumnName("escalated_to");
            incident.Property(i => i.AcknowledgedAt).HasColumnName("acknowledged_at");
            incident.Property(i => i.EscalatedAt).HasColumnName("escalated_at");
            incident.Property(i => i.CreatedAt).HasColumnName("created_at");
            incident.Property(i => i.UpdatedAt).HasColumnName("updated_at");

            incident.Property(i => i.ClinicId)
                .HasConversion(c => c.Value, v => new ClinicId(v))
                .HasColumnName("clinic_id")
                .IsRequired();

            incident.Property(i => i.Status)
                .HasConversion(s => s.StatusValue, v => new IncidentStatus(v))
                .HasColumnName("status")
                .IsRequired();

            incident.Property(i => i.Metric)
                .HasConversion(
                    m => System.Text.Json.JsonSerializer.Serialize(m, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<IncidentMetric>(v, (System.Text.Json.JsonSerializerOptions?)null)!)
                .HasColumnName("metric")
                .IsRequired();
        });
    }
}
