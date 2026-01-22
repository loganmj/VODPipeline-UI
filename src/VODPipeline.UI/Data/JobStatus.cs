namespace VODPipeline.UI.Data
{
    public class JobStatus : IJobStatus
    {
        public bool IsRunning { get; set; }
        public string? CurrentFile { get; set; }
        public string? Stage { get; set; }
        public int? Percent { get; set; }
        public DateTime? LastUpdated { get; set; }
        public TimeSpan? EstimatedTimeRemaining { get; set; }
        public TimeSpan? ElapsedTime { get; set; }
    }
}
