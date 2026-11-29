using TriAidPlatform.API.Shared.Infrastructure.Localization;
﻿using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Errors;
using TriAidPlatform.API.Shared.Application.Model;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

public static class IncidentsActionResultAssembler
{
    private static int ToStatusCodeFromIncidentsError(IncidentsError error)
    {
        return error switch
        {
            IncidentsError.IncidentNotFound => StatusCodes.Status404NotFound,
            IncidentsError.InvalidIncidentData => StatusCodes.Status400BadRequest,
            IncidentsError.InvalidStateTransition => StatusCodes.Status409Conflict,
            IncidentsError.UnauthorizedClinicAccess => StatusCodes.Status403Forbidden,
            IncidentsError.OperationCancelled => StatusCodes.Status409Conflict,
            IncidentsError.DatabaseError => StatusCodes.Status500InternalServerError,
            IncidentsError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromCommandResult(
        ControllerBase controller,
        Result<Incident> result,
        Func<Incident, IActionResult> onSuccess,
        IApiMessages? messages = null)
    {
        if (result.IsSuccess) return onSuccess(result.Value!);

        var statusCode = Enum.TryParse<IncidentsError>(result.DomainError.Code, out var error)
            ? ToStatusCodeFromIncidentsError(error)
            : StatusCodes.Status400BadRequest;

        return controller.Problem(
            title: string.IsNullOrWhiteSpace(result.DomainError.Code) ? "IncidentError" : result.DomainError.Code,
            // Follows the client's language when the catalog has the error; the domain message otherwise.
            detail: messages?.Find("Incident", result.DomainError.Code) ?? result.Message,
            statusCode: statusCode
        );
    }

    public static IActionResult ToActionResultFromGetIncidentByIdResult(
        ControllerBase controller,
        Incident? incident,
        Func<Incident, IActionResult> onSuccess)
    {
        if (incident is null)
        {
            return controller.Problem(
                title: nameof(IncidentsError.IncidentNotFound),
                detail: "Incident not found.",
                statusCode: ToStatusCodeFromIncidentsError(IncidentsError.IncidentNotFound)
            );
        }

        return onSuccess(incident);
    }
}