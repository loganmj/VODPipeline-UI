namespace VODPipeline.UI.Data
{
    public class JobStage : IJobStage
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? ProgressPercent { get; set; }
    }
}
