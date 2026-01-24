namespace VODPipeline.UI.Data
{
    /// <summary>
    /// Represents the health status of an individual subsystem.
    /// </summary>
    public class SubsystemHealth : IComponentHealth
    {
        public HealthStatus Status { get; set; } = HealthStatus.Unknown;
        public DateTime? LastHeartbeat { get; set; }
        public string? Message { get; set; }
    }
}
