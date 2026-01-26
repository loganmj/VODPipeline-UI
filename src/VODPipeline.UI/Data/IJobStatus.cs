namespace VODPipeline.UI.Data
{
    public interface IJobStatus
    {
        public bool IsRunning { get; }
        public string? JobId { get; }
        public string? FileName { get; }
        public string? CurrentFile { get; }
        public string? Stage { get; }
        public int? Percent { get; }
        public DateTime? Timestamp { get; }
    }
}
