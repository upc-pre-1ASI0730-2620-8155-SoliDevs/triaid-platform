using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Services;

/// <summary>Error types produced by the specialty assignment use cases.</summary>
public enum ESpecialtyError
{
    /// <summary>The referenced resource does not exist.</summary>
    NotFound,

    /// <summary>The operation conflicts with the referral lifecycle or rules.</summary>
    Conflict,

    /// <summary>A mandatory input is missing or invalid.</summary>
    Validation
}

/// <summary>Structured error of a specialty assignment failure.</summary>
/// <param name="Type">Error category.</param>
/// <param name="Message">Human readable explanation.</param>
public sealed record SpecialtyError(ESpecialtyError Type, string Message);

/// <summary>Suggestion outcome including the demographic admission verdict (US32 - Escenario 2).</summary>
/// <param name="SpecialtyKey">Suggested specialty key.</param>
/// <param name="Rule">Strategy that produced the suggestion.</param>
/// <param name="DemographicRestriction">True when the suggested specialty rejects the patient demographics.</param>
public sealed record SpecialtySuggestionResult(string SpecialtyKey, string Rule, bool DemographicRestriction);

/// <summary>Contract of the Specialty Assignment use cases.</summary>
public interface ISpecialtyAssignmentService
{
    /// <summary>Computes the suggested specialty for an episode (US30).</summary>
    Task<Result<SpecialtySuggestionResult, SpecialtyError>> SuggestAsync(
        SuggestSpecialtyQuery query, CancellationToken cancellationToken = default);

    /// <summary>Routes a classified episode to a specialty (US31).</summary>
    Task<Result<Referral, SpecialtyError>> ReferAsync(
        ReferEpisodeCommand command, string symptom, CancellationToken cancellationToken = default);

    /// <summary>Reassigns a referral with justification (US32).</summary>
    Task<Result<Referral, SpecialtyError>> ReassignAsync(
        ReassignReferralCommand command, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the referral of an episode (US33, US37).</summary>
    Task<Result<Referral, SpecialtyError>> GetByEpisodeAsync(
        int episodeId, CancellationToken cancellationToken = default);

    /// <summary>Lists the specialty catalog.</summary>
    Task<IEnumerable<Specialty>> ListSpecialtiesAsync(
        bool onlyActive = false, CancellationToken cancellationToken = default);
}
