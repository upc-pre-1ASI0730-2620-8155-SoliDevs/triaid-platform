namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

public sealed record AlertMetric
{
    //Nombre del signo vital 
    public string SignName { get; init; }
    // Valor exacto que capturo el dispositivo destinado durante el triaje
    public string MeasuredValue {get; init; }
    //Nivel de seguridad para los signos vitales
    public string SafeRange {get; init; }

    //Constructor
    protected AlertMetric()
    {
    }
}