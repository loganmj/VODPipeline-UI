namespace VODPipeline.UI.Data
{
    public interface IJobEvent
    {
        string? EventId { get; }
        string? JobId { get; }
        DateTime? Timestamp { get; }
        string? EventType { get; }
        string? Message { get; }
        string? Stage { get; }
        Dictionary<string, object>? Details { get; }
    }
}
