using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Extension methods to configure the Entity Framework Core mappings of
/// the Triage Classification bounded context.
/// </summary>
public static class TriageClassificationContextConfigurationExtensions
{
    /// <summary>
    /// Applies the database configuration of the Classification aggregate:
    /// snake_case columns, catalog-backed level and state columns.
    /// </summary>
    /// <param name="builder">The model builder being configured.</param>
    /// <returns>The same builder to allow chaining.</returns>
    public static ModelBuilder ApplyTriageClassificationConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Classification>(classification =>
        {
            classification.HasKey(c => c.Id);
            classification.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
            classification.ToTable("triage_classifications");

            classification.Property(c => c.EpisodeId).HasColumnName("episode_id").IsRequired();
            classification.Property(c => c.SuggestedLevel).HasColumnName("suggested_level_id").IsRequired();
            classification.Property(c => c.FinalLevel).HasColumnName("final_level_id").IsRequired();
            classification.Property(c => c.State).HasColumnName("state_id").IsRequired();
            classification.Property(c => c.SuggestedAt).HasColumnName("suggested_at").IsRequired();
            classification.Property(c => c.ConfirmedAt).HasColumnName("confirmed_at");
            classification.Property(c => c.Justification).HasColumnName("justification");
            classification.Property(c => c.MissingMetrics).HasColumnName("missing_metrics");
            classification.Property(c => c.OverriddenBy).HasColumnName("overridden_by");
            classification.Property(c => c.Institution).HasColumnName("institution").IsRequired(false);
            classification.Property(c => c.CreatedAt).HasColumnName("created_at");
            classification.Property(c => c.UpdatedAt).HasColumnName("updated_at");

            classification.HasIndex(c => c.EpisodeId).IsUnique();
        });

        return builder;
    }
}
