namespace VODPipeline.UI.Data
{
    public class ComponentHealth : IComponentHealth
    {
        public HealthStatus Status { get; set; } = HealthStatus.Unknown;
        public DateTime? LastHeartbeat { get; set; }
        public string? Message { get; set; }
    }
}
