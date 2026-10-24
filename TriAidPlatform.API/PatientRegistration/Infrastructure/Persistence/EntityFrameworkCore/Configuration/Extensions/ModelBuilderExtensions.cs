using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;

namespace TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Entity Framework Core mapping of the Patient Registration bounded context.
/// Follows the patients table of the database diagram.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies the Patient Registration entity configuration to the model.
    /// </summary>
    /// <param name="builder">The model builder of the shared AppDbContext.</param>
    public static void ApplyPatientRegistrationConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Patient>().ToTable("patients");
        builder.Entity<Patient>().HasKey(p => p.Id);
        builder.Entity<Patient>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Patient>().Property(p => p.Dni).HasMaxLength(12);
        builder.Entity<Patient>().HasIndex(p => p.Dni).IsUnique();
        builder.Entity<Patient>().Property(p => p.FirstName).IsRequired().HasMaxLength(80);
        builder.Entity<Patient>().Property(p => p.LastName).IsRequired().HasMaxLength(80);
        builder.Entity<Patient>().Property(p => p.BirthDate).IsRequired();
        builder.Entity<Patient>().Property(p => p.Sex).IsRequired().HasConversion<int>().HasColumnName("SexId");
        builder.Entity<Patient>().Property(p => p.Phone).HasMaxLength(20);
        builder.Entity<Patient>().Property(p => p.Address).HasMaxLength(200);
        builder.Entity<Patient>().Property(p => p.IsTemporary).IsRequired();
        builder.Entity<Patient>().Property(p => p.TemporaryCode).HasMaxLength(12);
        builder.Entity<Patient>().HasIndex(p => p.TemporaryCode).IsUnique();
    }
}
