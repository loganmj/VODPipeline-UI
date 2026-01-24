namespace VODPipeline.UI.Data
{
    public interface ISystemHealthStatus
    {
        SubsystemHealth Function { get; }
        SubsystemHealth API { get; }
        SubsystemHealth Database { get; }
        SubsystemHealth FileShare { get; }
        DateTime? LastUpdated { get; }
    }
}
