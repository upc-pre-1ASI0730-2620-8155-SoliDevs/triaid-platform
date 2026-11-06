using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

namespace TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Extension methods to configure the Entity Framework Core mappings of
/// the Specialty Assignment bounded context.
/// </summary>
public static class SpecialtyAssignmentContextConfigurationExtensions
{
    /// <summary>
    /// Applies the database configuration of the Specialty catalog and the
    /// Referral aggregate with snake_case columns.
    /// </summary>
    /// <param name="builder">The model builder being configured.</param>
    /// <returns>The same builder to allow chaining.</returns>
    public static ModelBuilder ApplySpecialtyAssignmentConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Specialty>(specialty =>
        {
            specialty.ToTable("specialties");
            specialty.HasKey(s => s.Id);
            specialty.Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
            specialty.Property(s => s.Key).HasColumnName("key").IsRequired();
            specialty.Property(s => s.Name).HasColumnName("name").IsRequired();
            specialty.Property(s => s.Room).HasColumnName("room").IsRequired();
            specialty.Property(s => s.Active).HasColumnName("active").IsRequired();
            specialty.Property(s => s.MinAge).HasColumnName("min_age");
            specialty.Property(s => s.MaxAge).HasColumnName("max_age");
            specialty.Property(s => s.RequiredSex).HasColumnName("required_sex");
            specialty.Property(s => s.CreatedAt).HasColumnName("created_at");
            specialty.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            specialty.HasIndex(s => s.Key).IsUnique();
        });

        builder.Entity<Referral>(referral =>
        {
            referral.ToTable("referrals");
            referral.HasKey(r => r.Id);
            referral.Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
            referral.Property(r => r.EpisodeId).HasColumnName("episode_id").IsRequired();
            referral.Property(r => r.SpecialtyId).HasColumnName("specialty_id").IsRequired();
            referral.Property(r => r.Room).HasColumnName("room").IsRequired();
            referral.Property(r => r.QueuePosition).HasColumnName("queue_position").IsRequired();
            referral.Property(r => r.State).HasColumnName("state").IsRequired();
            referral.Property(r => r.ChangeReason).HasColumnName("change_reason");
            referral.Property(r => r.ReferredAt).HasColumnName("referred_at").IsRequired();
            referral.Property(r => r.CreatedAt).HasColumnName("created_at");
            referral.Property(r => r.UpdatedAt).HasColumnName("updated_at");
            referral.HasIndex(r => r.EpisodeId).IsUnique();
        });

        return builder;
    }
}
