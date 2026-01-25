namespace VODPipeline.UI.Data
{
    /// <summary>
    /// Represents the health status of an individual system.
    /// </summary>
    public class SystemHealth : ISystemHealth
    {
        public HealthStatus Status { get; set; } = HealthStatus.Unknown;
        public DateTime? LastHeartbeat { get; set; }
        public string? Message { get; set; }
    }
}
