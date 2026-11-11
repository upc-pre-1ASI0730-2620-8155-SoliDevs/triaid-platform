namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Services;

/// <summary>Outcome of the specialty recommendation.</summary>
/// <param name="SpecialtyKey">Suggested specialty key, or null when the input is insufficient.</param>
/// <param name="Rule">Strategy that produced the suggestion ('symptom' | 'priority').</param>
public sealed record SpecialtySuggestion(string? SpecialtyKey, string Rule)
{
    /// <summary>Whether a specialty was proposed (US30 - Escenario 1).</summary>
    public bool HasSuggestion => SpecialtyKey is not null;
}

/// <summary>
/// Domain service with the clinical rules of specialty recommendation
/// (US30): main-symptom keyword matching first, then the classified
/// priority as fallback (levels I-II to General Surgery, the rest to
/// Internal Medicine).
/// </summary>
public static class SpecialtyRules
{
    /// <summary>Main symptom keyword table. The first matching rule wins.</summary>
    private static readonly (string Key, string[] Words)[] SymptomRules =
    [
        ("CirugiaGeneral", ["abdomen", "abdominal", "apendice", "apendicitis", "hernia", "dolor abdominal"]),
        ("Traumatologia", ["fractura", "esguince", "luxacion", "trauma", "caida", "golpe", "torcedura"]),
        ("Cardiologia", ["toracico", "pecho", "corazon", "cardiaco", "palpitaciones", "opresion"]),
        ("Neurologia", ["cefalea", "migraña", "migrana", "convulsion", "mareo", "entumecimiento", "neurolog"]),
        ("Ginecologia", ["embarazo", "menstru", "pelvico", "vaginal", "gestac"]),
        ("Pediatria", ["niño", "niña", "bebe", "pediatric", "lactante", "infantil"]),
        ("MedicinaInterna", ["fiebre", "tos", "gripe", "dolor de garganta", "vomito", "diarrea", "infeccion", "debilidad", "fatiga"])
    ];

    /// <summary>
    /// Resolves the suggested specialty (US30 - Escenario 1). An empty
    /// symptom stops the suggestion and requires the main condition (US30 - Escenario 2).
    /// </summary>
    /// <param name="symptom">Free-text main symptom.</param>
    /// <param name="triageLevel">Confirmed triage level (1-5), or null.</param>
    /// <returns>The suggestion with the strategy that produced it.</returns>
    public static SpecialtySuggestion Suggest(string symptom, int? triageLevel)
    {
        var text = (symptom ?? string.Empty).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(text)) return new SpecialtySuggestion(null, "symptom-required");

        foreach (var (key, words) in SymptomRules)
            if (words.Any(text.Contains)) return new SpecialtySuggestion(key, "symptom");

        return triageLevel is 1 or 2
            ? new SpecialtySuggestion("CirugiaGeneral", "priority")
            : new SpecialtySuggestion("MedicinaInterna", "priority");
    }
}
