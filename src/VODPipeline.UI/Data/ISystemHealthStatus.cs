namespace VODPipeline.UI.Data
{
    public interface ISystemHealthStatus
    {
        public ComponentHealth Function { get; }
        public ComponentHealth API { get; }
        public ComponentHealth Database { get; }
        public ComponentHealth FileShare { get; }
        public DateTime? LastUpdated { get; }
    }
}
