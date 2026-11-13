using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST;

/**
 * <summary>
 *     Symptom catalog entry served to the web client intake form.
 * </summary>
 * <param name="Id">Catalog identifier of the symptom.</param>
 * <param name="Es">Spanish label.</param>
 * <param name="En">English label.</param>
 * <param name="Specialty">Specialty key the symptom suggests.</param>
 */
public record SymptomResource(string Id, string Es, string En, string Specialty);

/**
 * <summary>
 *     Symptom catalog REST facade. The catalog is static and matches
 *     the seed data of the web client (ADR-010 backend swap contract).
 * </summary>
 */
[ApiController]
[Route("symptoms")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Symptoms")]
public class SymptomsController : ControllerBase
{
    private static readonly SymptomResource[] Catalog =
    [
        new("RFF10", "Dolor abdominal agudo", "Acute abdominal pain", "CirugiaGeneral"),
        new("RFF11", "Dolor abdominal crónico", "Chronic abdominal pain", "CirugiaGeneral"),
        new("TRAU-002", "Fractura de tobillo", "Ankle fracture", "Traumatologia"),
        new("TRAU-005", "Esguince de rodilla", "Knee sprain", "Traumatologia"),
        new("TRAU-009", "Traumatismo de cráneo", "Head injury", "Neurologia"),
        new("CARD-001", "Dolor torácico", "Chest pain", "Cardiologia"),
        new("CARD-004", "Palpitaciones", "Palpitations", "Cardiologia"),
        new("NEU-003", "Cefalea intensa", "Severe headache", "Neurologia"),
        new("NEU-007", "Convulsiones", "Seizures", "Neurologia"),
        new("GIN-001", "Dolor pélvico femenino", "Female pelvic pain", "Ginecologia"),
        new("GIN-004", "Sangrado vaginal anormal", "Abnormal vaginal bleeding", "Ginecologia"),
        new("PED-001", "Fiebre en niño", "Fever in child", "Pediatria"),
        new("PED-005", "Vómitos en lactante", "Vomiting in infant", "Pediatria"),
        new("MED-002", "Fiebre persistente en adulto", "Persistent fever in adult", "MedicinaInterna"),
        new("MED-006", "Deshidratación", "Dehydration", "MedicinaInterna"),
        new("MED-009", "Crisis asmática", "Asthma attack", "MedicinaInterna"),
    ];

    /**
     * <summary>
     *     Lists the symptom catalog
     * </summary>
     * <returns>The symptom catalog.</returns>
     */
    [HttpGet]
    [EndpointSummary("List the symptom catalog")]
    public IActionResult GetAll()
    {
        return Ok(Catalog);
    }
}
