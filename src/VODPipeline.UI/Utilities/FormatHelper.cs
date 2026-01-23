namespace VODPipeline.UI.Utilities
{
    public static class FormatHelper
    {
        public static string FormatDuration(TimeSpan? duration)
        {
            if (!duration.HasValue || duration.Value < TimeSpan.Zero)
            {
                return "N/A";
            }

            return FormatDuration(duration.Value);
        }

        public static string FormatDuration(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                return "N/A";
            }
            
            if (duration.TotalHours >= 1)
            {
                return $"{(int)duration.TotalHours}h {duration.Minutes}m {duration.Seconds}s";
            }
            else if (duration.TotalMinutes >= 1)
            {
                return $"{duration.Minutes}m {duration.Seconds}s";
            }
            else if (duration.TotalSeconds >= 1)
            {
                return $"{(int)duration.TotalSeconds}s";
            }
            else if (duration.TotalMilliseconds >= 1)
            {
                return $"{(int)duration.TotalMilliseconds}ms";
            }
            else
            {
                return "<1ms";
            }
        }

        public static string FormatTimestamp(DateTime? timestamp)
        {
            if (!timestamp.HasValue)
            {
                return "N/A";
            }

            return timestamp.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz");
        }

        public static string GetStatusBadgeClass(string? status)
        {
            return status?.ToLower() switch
            {
                "completed" => "badge-success",
                "success" => "badge-success",
                "failed" => "badge-danger",
                "error" => "badge-danger",
                "running" => "badge-primary",
                "in progress" => "badge-primary",
                "processing" => "badge-primary",
                "pending" => "badge-warning",
                "queued" => "badge-warning",
                _ => "badge-secondary"
            };
        }
    }
}
