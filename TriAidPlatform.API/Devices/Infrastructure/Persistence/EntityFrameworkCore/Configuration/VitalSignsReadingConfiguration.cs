using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TriAidPlatform.API.Devices.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
/// Entity Framework Core configuration for the <see cref="VitalSignsReading"/> aggregate.
/// </summary>
public class VitalSignsReadingConfiguration : IEntityTypeConfiguration<VitalSignsReading>
{
    public void Configure(EntityTypeBuilder<VitalSignsReading> builder)
    {
        // Table mapping
        builder.ToTable("VitalSignsReadings");

        // Primary Key
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedOnAdd();

        // Properties with explicit types to prevent compiler inference issues
        builder.Property<int>(r => r.DeviceId)
            .IsRequired();

        builder.Property<int>(r => r.EpisodeId)
            .IsRequired();

        builder.Property<int>(r => r.SystolicBloodPressure)
            .IsRequired();

        builder.Property<int>(r => r.DiastolicBloodPressure)
            .IsRequired();

        builder.Property<int>(r => r.HeartRate)
            .IsRequired();

        builder.Property<decimal>(r => r.Temperature)
            .IsRequired();

        builder.Property<int>(r => r.OxygenSaturation)
            .IsRequired();

        builder.Property<DateTime>(r => r.CapturedAt)
            .IsRequired();
    }
}