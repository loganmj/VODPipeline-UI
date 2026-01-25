namespace VODPipeline.UI.Data
{
    public interface ISystemHealth
    {
        HealthStatus Status { get; }
        DateTime? LastHeartbeat { get; }
        string? Message { get; }
    }
}
