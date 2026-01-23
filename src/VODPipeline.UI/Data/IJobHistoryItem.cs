namespace VODPipeline.UI.Data
{
    public interface IJobHistoryItem
    {
        string? JobId { get; }
        string? FileName { get; }
        string? Status { get; }
        TimeSpan? Duration { get; }
        DateTime? Timestamp { get; }
    }
}
