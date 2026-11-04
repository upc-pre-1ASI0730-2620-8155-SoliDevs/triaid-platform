namespace TriAidPlatform.API.VitalSignsCaptureOla.Domain.Model.Aggregates
{
    /// <summary>
    /// Domain entity representing a medical measurement device in the TriAidPlatform, first version.
    /// </summary>
    public class Device
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = string.Empty; // pa, spo2, fc, temp
        public string Model { get; set; } = string.Empty;
        public bool Online { get; set; } = true;
        public string LastUse { get; set; } = string.Empty;
        public string? LinkedEpisodeId { get; set; } = null;
        public DateTime? LinkedAt { get; set; } = null;

        /// <summary>
        /// Indicates whether the device is available to be linked to a new triage episode.
        /// </summary>
        public bool IsFree => string.IsNullOrEmpty(LinkedEpisodeId);

        /// <summary>
        /// Links the device to a specific triage episode.
        /// </summary>
        /// <param name="episodeId">The identifier of the episode.</param>
        public void AssignTo(string episodeId)
        {
            LinkedEpisodeId = episodeId;
            LinkedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Unlinks the device, making it available for future uses.
        /// </summary>
        public void Release()
        {
            LinkedEpisodeId = null;
            LinkedAt = null;
        }
    }
}