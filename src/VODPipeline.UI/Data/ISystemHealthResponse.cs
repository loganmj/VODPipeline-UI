namespace VODPipeline.UI.Data
{
    public interface ISystemHealthResponse
    {
        Dictionary<string, SystemHealth> Systems { get; }
        DateTime? LastUpdated { get; }
    }
}
