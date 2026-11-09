using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyDevicesConfiguration(this ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DeviceConfiguration());
        builder.ApplyConfiguration(new VitalSignsReadingConfiguration());
        
        return builder;
    }
}