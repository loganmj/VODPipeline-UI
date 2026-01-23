namespace VODPipeline.UI.Data
{
    public interface IComponentHealth
    {
        public HealthStatus Status { get; }
        public DateTime? LastHeartbeat { get; }
        public string? Message { get; }
    }
}
