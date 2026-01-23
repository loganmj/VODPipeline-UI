namespace VODPipeline.UI.Data
{
    public class JobEvent : IJobEvent
    {
        public string? EventId { get; set; }
        public string? JobId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? EventType { get; set; }
        public string? Message { get; set; }
        public string? Stage { get; set; }
        public Dictionary<string, object>? Details { get; set; }
    }
}
