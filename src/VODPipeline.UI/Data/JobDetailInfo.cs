namespace VODPipeline.UI.Data
{
    public class JobDetailInfo : IJobDetailInfo
    {
        public string? JobId { get; set; }
        public string? FileName { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? CurrentStage { get; set; }
        public int? ProgressPercent { get; set; }
        public List<IJobStage>? Stages { get; set; }
        public int? HighlightCount { get; set; }
        public int? SceneCount { get; set; }
        public Dictionary<string, object>? Metadata { get; set; }
    }
}
