namespace TriAidPlatform.API.Alerting.Domain.Model.Aggregates;

public class Alert
{
    //private set servía para proteger el estado sin que nadie  fuera de la clase pueda manipularla
    public int Id { get; private set; }
    public int ReadingId {get; private set;}
    public AlertLevel Level { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime RaisedAt { get; private set; }

    protected Alert() { }

    public Alert(int readingId, AlertLevel level)
    {
        ReadingId = readingId;
        Level = level;
        Status = AlertStatus.Active;
        RaisedAt= DateTime.UtcNow;
    }
    
    //Reglas de negocio (en progreso)
    
    //Esta función escalará la alerta a la zona de mayor prioridad
    public void Escalate(string destination = "Trauma Shock")
    {
        Status = AlertStatus.Escalated;
        EscalatedTo = destination;
    }
    
    //La alerta marcará el estado de la alerta como  resuelta
    public void Resolve()
    {
        Status = AlertStatus.Resolved;
    }
}