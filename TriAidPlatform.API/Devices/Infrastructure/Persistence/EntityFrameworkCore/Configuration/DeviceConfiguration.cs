using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TriAidPlatform.API.Devices.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.SerialNumber)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.HasIndex(d => d.SerialNumber)
            .IsUnique();

        builder.Property(d => d.Model)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.RegisteredAt)
            .IsRequired();
    }
}