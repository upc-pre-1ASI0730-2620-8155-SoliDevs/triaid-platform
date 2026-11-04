namespace TriAidPlatform.API.VitalSignsCaptureOla.Domain.Model.Aggregates
{
    /// <summary>
    /// Represents the catalog of supported vital sign measurement types and their clinical limits.
    /// </summary>
    public class DeviceType
    {
        public string Key { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Ph { get; set; } = string.Empty;
        public double MinLimit { get; set; }
        public double MaxLimit { get; set; }

        public static readonly List<DeviceType> SupportedTypes = new()
        {
            new DeviceType { Key = "pa", Label = "Presión arterial", Device = "Tensiómetro", Unit = "mmHg", Ph = "120", MinLimit = 50, MaxLimit = 250 },
            new DeviceType { Key = "spo2", Label = "Saturación O₂", Device = "Oxímetro", Unit = "%", Ph = "98", MinLimit = 50, MaxLimit = 100 },
            new DeviceType { Key = "fc", Label = "Frecuencia cardíaca", Device = "Monitor FC", Unit = "lpm", Ph = "75", MinLimit = 30, MaxLimit = 220 },
            new DeviceType { Key = "temp", Label = "Temperatura", Device = "Termómetro", Unit = "°C", Ph = "36.5", MinLimit = 30, MaxLimit = 45 }
        };
    }
}