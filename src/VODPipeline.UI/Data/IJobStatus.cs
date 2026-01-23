namespace VODPipeline.UI.Data
{
    public interface IJobStatus
    {
        public bool IsRunning { get; }
        public string? JobID { get; }
        public string? FileName { get; }
        public string? CurrentFile { get; }
        public string? Stage { get; }
        public int? Percent { get; }
        public DateTime? TimeStamp { get; }
        public DateTime? LastUpdated { get; }
        public TimeSpan? EstimatedTimeRemaining { get; }
        public TimeSpan? ElapsedTime { get; }
    }
}
