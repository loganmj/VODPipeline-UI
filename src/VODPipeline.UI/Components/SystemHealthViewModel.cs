using VODPipeline.UI.Data;

namespace VODPipeline.UI.Components
{
    /// <summary>
    /// View model for system health status in the UI.
    /// Unifies system health-related UI state into a single authoritative object.
    /// Supports real-time updates via SignalR events.
    /// </summary>
    public class SystemHealthViewModel
    {
        // ===== Overall System Status =====
        /// <summary>
        /// Indicates whether the overall system is healthy.
        /// True if all subsystems are Healthy, false otherwise.
        /// </summary>
        public bool IsHealthy { get; private set; }
        
        /// <summary>
        /// Timestamp of when the health status was last updated.
        /// </summary>
        public DateTime LastUpdatedAt { get; private set; }

        // ===== Subsystem Statuses =====
        public Data.SystemHealth Api { get; private set; }
        public Data.SystemHealth Function { get; private set; }
        public Data.SystemHealth Database { get; private set; }
        public Data.SystemHealth FileShare { get; private set; }

        // ===== UI Helper Properties =====
        /// <summary>
        /// Indicates whether any subsystem has errors.
        /// Equivalent to !IsHealthy for convenience.
        /// </summary>
        public bool HasErrors => !IsHealthy;

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        private SystemHealthViewModel(
            Data.SystemHealth api,
            Data.SystemHealth function,
            Data.SystemHealth database,
            Data.SystemHealth fileShare,
            DateTime lastUpdatedAt)
        {
            Api = api;
            Function = function;
            Database = database;
            FileShare = fileShare;
            LastUpdatedAt = lastUpdatedAt;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Creates a new SystemHealthViewModel from a SystemHealthResponse DTO
        /// </summary>
        public static SystemHealthViewModel FromSystemHealthResponse(SystemHealthResponse healthResponse)
        {
            if (healthResponse == null)
                throw new ArgumentNullException(nameof(healthResponse));

            // Extract subsystem health data from dictionary
            var api = GetOrCreateSystemHealth(healthResponse.Systems, "API");
            var function = GetOrCreateSystemHealth(healthResponse.Systems, "Function");
            var database = GetOrCreateSystemHealth(healthResponse.Systems, "Database");
            var fileShare = GetOrCreateSystemHealth(healthResponse.Systems, "FileShare");
            
            var lastUpdatedAt = healthResponse.LastUpdated ?? DateTime.UtcNow;

            return new SystemHealthViewModel(
                api: api,
                function: function,
                database: database,
                fileShare: fileShare,
                lastUpdatedAt: lastUpdatedAt
            );
        }

        /// <summary>
        /// Updates the API subsystem health status.
        /// This method is called when receiving apiHeartbeat SignalR events.
        /// </summary>
        public void UpdateApiHealth(Data.SystemHealth systemHealth)
        {
            if (systemHealth == null)
                throw new ArgumentNullException(nameof(systemHealth));

            Api = CopySystemHealth(systemHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the Function subsystem health status.
        /// This method is called when receiving functionHeartbeat SignalR events.
        /// </summary>
        public void UpdateFunctionHealth(Data.SystemHealth systemHealth)
        {
            if (systemHealth == null)
                throw new ArgumentNullException(nameof(systemHealth));

            Function = CopySystemHealth(systemHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the Database subsystem health status.
        /// This method is called when receiving dbStatusChanged SignalR events.
        /// </summary>
        public void UpdateDatabaseHealth(Data.SystemHealth systemHealth)
        {
            if (systemHealth == null)
                throw new ArgumentNullException(nameof(systemHealth));

            Database = CopySystemHealth(systemHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the FileShare subsystem health status.
        /// This method is called when receiving fileShareStatusChanged SignalR events.
        /// </summary>
        public void UpdateFileShareHealth(Data.SystemHealth systemHealth)
        {
            if (systemHealth == null)
                throw new ArgumentNullException(nameof(systemHealth));

            FileShare = CopySystemHealth(systemHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Applies a full system health update from a SystemHealthResponse DTO.
        /// This method is called when receiving SystemHealthUpdated SignalR events.
        /// </summary>
        public void ApplyFullUpdate(SystemHealthResponse healthResponse)
        {
            if (healthResponse == null)
                throw new ArgumentNullException(nameof(healthResponse));

            Api = GetOrCreateSystemHealth(healthResponse.Systems, "API");
            Function = GetOrCreateSystemHealth(healthResponse.Systems, "Function");
            Database = GetOrCreateSystemHealth(healthResponse.Systems, "Database");
            FileShare = GetOrCreateSystemHealth(healthResponse.Systems, "FileShare");
            LastUpdatedAt = healthResponse.LastUpdated ?? DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Calculates whether the overall system is healthy based on all subsystems.
        /// The system is considered healthy only if all subsystems have a Healthy status.
        /// </summary>
        private bool CalculateIsHealthy()
        {
            return new[] { Api, Function, Database, FileShare }
                .All(subsystem => subsystem.Status == HealthStatus.Healthy);
        }

        /// <summary>
        /// Gets a system health from the dictionary or creates a new one if not found.
        /// </summary>
        private static Data.SystemHealth GetOrCreateSystemHealth(Dictionary<string, Data.SystemHealth> systems, string key)
        {
            if (systems.TryGetValue(key, out var systemHealth))
            {
                return CopySystemHealth(systemHealth);
            }
            return new Data.SystemHealth();
        }

        /// <summary>
        /// Creates a copy of a SystemHealth to ensure immutability.
        /// </summary>
        private static Data.SystemHealth CopySystemHealth(Data.SystemHealth systemHealth)
        {
            return new Data.SystemHealth
            {
                Status = systemHealth.Status,
                LastHeartbeat = systemHealth.LastHeartbeat,
                Message = systemHealth.Message
            };
        }
    }
}
