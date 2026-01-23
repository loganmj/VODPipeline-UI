namespace VODPipeline.UI.Data
{
    public interface IJobDetailInfo
    {
        string? JobId { get; }
        string? FileName { get; }
        string? Status { get; }
        DateTime? StartTime { get; }
        DateTime? EndTime { get; }
        TimeSpan? Duration { get; }
        string? CurrentStage { get; }
        int? ProgressPercent { get; }
        List<IJobStage>? Stages { get; }
        int? HighlightCount { get; }
        int? SceneCount { get; }
        Dictionary<string, object>? Metadata { get; }
    }
}
