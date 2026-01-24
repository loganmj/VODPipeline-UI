using VODPipeline.UI.Data;

namespace VODPipeline.UI.Components
{
    /// <summary>
    /// View model for current job status in the UI.
    /// Unifies job-related UI state into a single authoritative object.
    /// </summary>
    public class CurrentJobViewModel
    {
        // ===== Identity =====
        // These fields never change during a job
        public Guid JobId { get; private set; }
        public string FileName { get; private set; }

        // ===== Core Status =====
        // These fields update frequently via SignalR
        public string Stage { get; private set; }
        public int Percent { get; private set; }
        public bool IsRunning { get; private set; }

        // ===== Timing =====
        public DateTime StartedAt { get; private set; }
        public DateTime LastUpdatedAt { get; private set; }
        public TimeSpan? EstimatedRemaining { get; private set; }
        
        /// <summary>
        /// Computed elapsed time since job started.
        /// Returns TimeSpan.Zero if LastUpdatedAt is earlier than StartedAt.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                var elapsed = LastUpdatedAt - StartedAt;
                return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
            }
        }

        // ===== UI Helper Properties =====
        public bool IsIdle => !IsRunning;
        public bool IsComplete => Percent >= 100;
        public bool HasError => ErrorMessage != null;

        // ===== Optional =====
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        private CurrentJobViewModel(
            Guid jobId,
            string fileName,
            string stage,
            int percent,
            bool isRunning,
            DateTime startedAt,
            DateTime lastUpdatedAt,
            TimeSpan? estimatedRemaining = null,
            string? errorMessage = null)
        {
            JobId = jobId;
            FileName = fileName ?? string.Empty;
            Stage = stage ?? string.Empty;
            Percent = Math.Clamp(percent, 0, 100);
            IsRunning = isRunning;
            StartedAt = startedAt;
            LastUpdatedAt = lastUpdatedAt;
            EstimatedRemaining = estimatedRemaining;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a new CurrentJobViewModel from a JobStatus DTO
        /// </summary>
        public static CurrentJobViewModel FromJobStatus(JobStatus jobStatus)
        {
            if (jobStatus == null)
                throw new ArgumentNullException(nameof(jobStatus));

            var jobId = ParseJobId(jobStatus.JobId);
            
            // Validate that JobId was successfully parsed
            if (jobId == Guid.Empty)
                throw new ArgumentException("JobStatus.JobId must be a valid GUID", nameof(jobStatus));
            
            // Use Timestamp for StartedAt (job start time)
            var startedAt = jobStatus.Timestamp ?? DateTime.UtcNow;
            
            // Use LastUpdated for LastUpdatedAt if available, otherwise fall back to Timestamp
            var lastUpdatedAt = jobStatus.LastUpdated ?? startedAt;
            
            return new CurrentJobViewModel(
                jobId: jobId,
                fileName: jobStatus.FileName ?? string.Empty,
                stage: jobStatus.Stage ?? string.Empty,
                percent: jobStatus.Percent ?? 0,
                isRunning: jobStatus.IsRunning,
                startedAt: startedAt,
                lastUpdatedAt: lastUpdatedAt,
                estimatedRemaining: jobStatus.EstimatedTimeRemaining
            );
        }

        /// <summary>
        /// Applies an update from a JobStatus DTO received via SignalR
        /// </summary>
        public void ApplyUpdate(JobStatus update)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            // Verify this update is for the same job
            var updateJobId = ParseJobId(update.JobId);
            if (updateJobId != Guid.Empty && updateJobId != JobId)
            {
                throw new InvalidOperationException(
                    $"Cannot apply update for job {updateJobId} to view model for job {JobId}");
            }

            // Update core status fields
            Stage = update.Stage ?? Stage;
            Percent = Math.Clamp(update.Percent ?? Percent, 0, 100);
            // Note: IsRunning is assigned directly from update.IsRunning because it is a non-nullable bool in the DTO.
            // In partial updates, a missing IsRunning field would deserialize to false and incorrectly stop the job.
            // Consider using a separate update DTO with nullable fields if partial updates are required.
            IsRunning = update.IsRunning;

            // Update timing - prioritize LastUpdated, fallback to Timestamp, then UtcNow
            LastUpdatedAt = GetTimestampFromUpdate(update);

            // Recalculate estimated remaining time
            RecalculateEstimatedRemaining(update);

            // Handle completion
            if (Percent >= 100)
            {
                IsRunning = false;
                EstimatedRemaining = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Sets an error state on the view model
        /// </summary>
        public void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IsRunning = false;
            // Clear any stale ETA when entering an error state
            EstimatedRemaining = null;
        }

        /// <summary>
        /// Clears any error state
        /// </summary>
        public void ClearError()
        {
            ErrorMessage = null;
        }

        /// <summary>
        /// Recalculates the estimated remaining time based on current progress
        /// </summary>
        private void RecalculateEstimatedRemaining(JobStatus update)
        {
            // If the update provides an estimated time remaining, use it
            if (update.EstimatedTimeRemaining.HasValue)
            {
                EstimatedRemaining = update.EstimatedTimeRemaining;
                return;
            }

            // Otherwise, calculate based on progress
            var elapsed = Elapsed;
            
            // Can't calculate if no time has elapsed or no progress made
            if (elapsed.TotalSeconds <= 0 || Percent <= 0)
            {
                EstimatedRemaining = null;
                return;
            }

            // Calculate remaining percent
            int remainingPercent = 100 - Percent;
            
            // If complete, no time remaining
            if (remainingPercent <= 0)
            {
                EstimatedRemaining = TimeSpan.Zero;
                return;
            }

            // Calculate estimated time remaining
            // Formula: elapsed * (remaining / completed)
            // Use Math.Max to ensure we never divide by zero
            double estimatedRemainingSeconds = elapsed.TotalSeconds * ((double)remainingPercent / Math.Max(Percent, 1));
            EstimatedRemaining = TimeSpan.FromSeconds(estimatedRemainingSeconds);
        }

        /// <summary>
        /// Parses a JobId string to a Guid. Returns Guid.Empty if parsing fails or input is null.
        /// </summary>
        private static Guid ParseJobId(string? jobId)
        {
            return Guid.TryParse(jobId, out var parsedId) ? parsedId : Guid.Empty;
        }

        /// <summary>
        /// Extracts the timestamp from a JobStatus update.
        /// Prioritizes LastUpdated, falls back to Timestamp, then to UtcNow as a last resort.
        /// </summary>
        private DateTime GetTimestampFromUpdate(JobStatus update)
        {
            return update.LastUpdated ?? update.Timestamp ?? DateTime.UtcNow;
        }
    }
}
