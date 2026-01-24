namespace VODPipeline.UI.Data
{
    public class SystemHealthStatus : ISystemHealthStatus
    {
        public SubsystemHealth Function { get; set; } = new SubsystemHealth();
        public SubsystemHealth API { get; set; } = new SubsystemHealth();
        public SubsystemHealth Database { get; set; } = new SubsystemHealth();
        public SubsystemHealth FileShare { get; set; } = new SubsystemHealth();
        public DateTime? LastUpdated { get; set; }
    }
}
