namespace TriAidPlatform.API.Incidents.Domain.Model.Errors;

/// <summary>
/// Defines the domain and infrastructure errors that can occur while processing an incident.
/// </summary>
public enum IncidentsError
{
    /// <summary>The requested incident could not be found.</summary>
    IncidentNotFound,

    /// <summary>The supplied incident data is invalid or incomplete.</summary>
    InvalidIncidentData,

    /// <summary>The requested lifecycle transition is not valid for the incident current state.</summary>
    InvalidStateTransition,

    /// <summary>The clinic does not have permission to access or modify the incident.</summary>
    UnauthorizedClinicAccess,

    /// <summary>The incident operation was canceled before it completed.</summary>
    OperationCancelled,

    /// <summary>A database operation failed while processing the incident.</summary>
    DatabaseError,

    /// <summary>An unexpected internal server error occurred while processing the incident.</summary>
    InternalServerError
}