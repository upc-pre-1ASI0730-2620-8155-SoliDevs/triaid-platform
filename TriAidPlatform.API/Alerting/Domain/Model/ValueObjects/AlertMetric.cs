namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
/// Value object representing an anomalous vital sign reading within the Alert bounded context.
/// Encapsulates the immutable clinical data to describe the exact metric that triggered the alert.
/// </summary>
public sealed record AlertMetric
{
    /// <summary>
    /// Human-readable name of the altered vital sign.
    /// </summary>
    public string SignName { get; init; }

    /// <summary>
     ///  Exact anomalous value captured by the device during a triage.
     /// </summary>
    public string MeasuredValue {get; init; }
    /// <summary>
    /// Normative parameter indicating the healthy levels for the vital sign 
    /// </summary>
    public string SafeRange {get; init; }

    /// <summary>
    /// Protected parameterless constructor for materialization
    /// </summary>
    protected AlertMetric() {}

    
    /// <summary>
    /// Initializes a new instance of the <see cref="AlertMetric"/> value object.
    /// </summary>
    /// <param name="signName">Name of the vital sign.</param>
    /// <param name="measuredValue">Exact value captured.</param>
    /// <param name="safeRange">Safe reference range.</param>
    /// <exception cref="ArgumentException">Thrown when signName or measuredValue are null or empty.</exception>
    public AlertMetric(string signName, string measuredValue, string safeRange)
    {
        if (string.IsNullOrWhiteSpace(signName)) throw new ArgumentException("The vital sign's name is required.");
        if (string.IsNullOrWhiteSpace(measuredValue)) throw new ArgumentException("The vital measured value is required.");
        
        SignName = signName;
        MeasuredValue = measuredValue;
        SafeRange = safeRange;
    }
}