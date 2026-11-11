using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Shared.Application.Internal.SeedService;

/// <summary>
///     Idempotent seed of the specialty catalog. Runs at startup so the
///     referral rules always have their destination specialties.
/// </summary>
public static class SpecialtySeed
{
    /// <summary>Inserts the base specialties when the catalog is empty.</summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService
            <TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.AppDbContext>();
        if (context.Set<Specialty>().Any()) return;

        context.Set<Specialty>().AddRange(
            new Specialty("MedicinaInterna", "Medicina Interna", "MI-2", minAge: 0),
            new Specialty("CirugiaGeneral", "Cirugía General", "CG-1", minAge: 0),
            new Specialty("Traumatologia", "Traumatología", "TT-3", minAge: 0),
            new Specialty("Cardiologia", "Cardiología", "CA-2", minAge: 0),
            new Specialty("Ginecologia", "Ginecología", "GI-1", requiredSex: "F"),
            new Specialty("Pediatria", "Pediatría", "PD-1", maxAge: 14),
            new Specialty("Neurologia", "Neurología", "NE-1", active: false, minAge: 0));
        await context.SaveChangesAsync();
    }
}
