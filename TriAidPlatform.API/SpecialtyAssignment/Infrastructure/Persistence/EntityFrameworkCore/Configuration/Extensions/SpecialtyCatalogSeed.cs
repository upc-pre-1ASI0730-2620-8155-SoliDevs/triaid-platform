using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

// ReSharper disable once CheckNamespace

namespace TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>
/// Seeds the specialty catalog of the institution demo when the database
/// is created, mirroring the catalog used by the web client.
/// </summary>
public static class SpecialtyCatalogSeed
{
    /// <summary>Inserts the base specialties when the table is empty.</summary>
    /// <param name="context">Database context.</param>
    public static void SeedSpecialtyCatalog(global::TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.AppDbContext context)
    {
        if (context.Set<Specialty>().Any()) return;

        context.Set<Specialty>().AddRange(
            new Specialty("MedicinaInterna", "Medicina Interna", "MI-2", minAge: 0),
            new Specialty("CirugiaGeneral", "Cirugía General", "CG-1", minAge: 0),
            new Specialty("Traumatologia", "Traumatología", "TT-3", minAge: 0),
            new Specialty("Ginecologia", "Ginecología", "GI-1", requiredSex: "F"),
            new Specialty("Cardiologia", "Cardiología", "CA-2", minAge: 0),
            new Specialty("Pediatria", "Pediatría", "PD-1", maxAge: 14),
            new Specialty("Neurologia", "Neurología", "NE-1", active: false, minAge: 0));
        context.SaveChanges();
    }
}
