namespace VODPipeline.UI.Data
{
    public class SystemHealthResponse : ISystemHealthResponse
    {
        public Dictionary<string, SystemHealth> Systems { get; set; } = new Dictionary<string, SystemHealth>();
        public DateTime? LastUpdated { get; set; }
    }
}
