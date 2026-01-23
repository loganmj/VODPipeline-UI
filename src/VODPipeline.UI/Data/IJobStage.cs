namespace VODPipeline.UI.Data
{
    public interface IJobStage
    {
        string? Name { get; }
        string? Status { get; }
        DateTime? StartTime { get; }
        DateTime? EndTime { get; }
        TimeSpan? Duration { get; }
        int? ProgressPercent { get; }
    }
}
