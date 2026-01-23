namespace VODPipeline.UI.Data
{
    public class SystemHealthStatus
    {
        public ComponentHealth Function { get; set; } = new ComponentHealth();
        public ComponentHealth API { get; set; } = new ComponentHealth();
        public ComponentHealth Database { get; set; } = new ComponentHealth();
        public ComponentHealth FileShare { get; set; } = new ComponentHealth();
        public DateTime? LastUpdated { get; set; }
    }

    public class ComponentHealth
    {
        public HealthStatus Status { get; set; } = HealthStatus.Unknown;
        public DateTime? LastHeartbeat { get; set; }
        public string? Message { get; set; }
    }

    public enum HealthStatus
    {
        Unknown = 0,
        Healthy = 1,
        Degraded = 2,
        Unhealthy = 3
    }
}
