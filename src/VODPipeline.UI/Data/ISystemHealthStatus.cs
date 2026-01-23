namespace VODPipeline.UI.Data
{
    public interface ISystemHealthStatus
    {
        ComponentHealth Function { get; }
        ComponentHealth API { get; }
        ComponentHealth Database { get; }
        ComponentHealth FileShare { get; }
        DateTime? LastUpdated { get; }
    }
}
