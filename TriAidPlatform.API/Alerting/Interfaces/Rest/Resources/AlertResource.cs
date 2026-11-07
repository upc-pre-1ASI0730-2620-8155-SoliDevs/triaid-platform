namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;
/// <summary>
/// Resource representation of an Alert, used to return data to the frontend or external clients.
/// Acts as a Data Transfer Object containing flattened alert information, including vital sign metrics.
/// </summary>
/// <param name="Id">The unique identifier of the alert.</param>
/// <param name="ClinicId">The unique identifier of the clinic to ensure data isolation.</param>
/// <param name="Status">The current lifecycle status of the alert (e.g., Active, Acknowledged, Escalated, Resolved).</param>
/// <param name="Level">The severity level of the alert.</param>
/// <param name="ReadingId">The identifier of the specific vital sign reading that triggered the alert.</param>
/// <param name="RaisedAt">The exact date and time (in ISO 8601 format) when the alert was triggered.</param>
/// <param name="AcknowledgedAt">The exact date and time (in ISO 8601 format) when the alert was acknowledged, if applicable.</param>
/// <param name="EscalatedTo">The destination to which the alert was escalated, if applicable.</param>
/// <param name="SignName">The name of the vital sign that triggered the alert.</param>
/// <param name="MeasuredValue">The anomalous measured value of the vital sign.</param>
/// <param name="SafeRange">The safe or expected range for the vital sign.</param>
public record AlertResource(
    int Id,
    int ClinicId,
    string Status,
    string Level,
    int ReadingId,
    string RaisedAt,
    string? AcknowledgedAt,
    string? EscalatedTo,
    string SignName,
    string MeasuredValue,
    string SafeRange
);