namespace VODPipeline.UI.Data
{
    public class JobStatus : IJobStatus
    {
        public bool IsRunning { get; set; }
        public string? JobId { get; set; }
        public string? FileName { get; set; }
        public string? CurrentFile { get; set; }
        public string? Stage { get; set; }
        public int? Percent { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
