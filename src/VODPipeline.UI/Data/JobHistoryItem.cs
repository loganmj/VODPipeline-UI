namespace VODPipeline.UI.Data
{
    public class JobHistoryItem
    {
        public string? JobId { get; set; }
        public string? FileName { get; set; }
        public string? Status { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
