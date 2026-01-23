namespace VODPipeline.UI.Data
{
    public interface IComponentHealth
    {
        HealthStatus Status { get; }
        DateTime? LastHeartbeat { get; }
        string? Message { get; }
    }
}
