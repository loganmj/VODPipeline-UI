namespace VODPipeline.UI.Data
{
    public class SystemHealthStatus : ISystemHealthStatus
    {
        public ComponentHealth Function { get; set; } = new ComponentHealth();
        public ComponentHealth API { get; set; } = new ComponentHealth();
        public ComponentHealth Database { get; set; } = new ComponentHealth();
        public ComponentHealth FileShare { get; set; } = new ComponentHealth();
        public DateTime? LastUpdated { get; set; }
    }
}
