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
        public SubsystemHealth Api { get; private set; }
        public SubsystemHealth Function { get; private set; }
        public SubsystemHealth Database { get; private set; }
        public SubsystemHealth FileShare { get; private set; }

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
            SubsystemHealth api,
            SubsystemHealth function,
            SubsystemHealth database,
            SubsystemHealth fileShare,
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
        /// Creates a new SystemHealthViewModel from a SystemHealthStatus DTO
        /// </summary>
        public static SystemHealthViewModel FromSystemHealthStatus(SystemHealthStatus healthStatus)
        {
            if (healthStatus == null)
                throw new ArgumentNullException(nameof(healthStatus));

            // Convert ComponentHealth to SubsystemHealth
            var api = ConvertToSubsystemHealth(healthStatus.API);
            var function = ConvertToSubsystemHealth(healthStatus.Function);
            var database = ConvertToSubsystemHealth(healthStatus.Database);
            var fileShare = ConvertToSubsystemHealth(healthStatus.FileShare);
            
            var lastUpdatedAt = healthStatus.LastUpdated ?? DateTime.UtcNow;

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
        public void UpdateApiHealth(ComponentHealth componentHealth)
        {
            if (componentHealth == null)
                throw new ArgumentNullException(nameof(componentHealth));

            Api = ConvertToSubsystemHealth(componentHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the Function subsystem health status.
        /// This method is called when receiving functionHeartbeat SignalR events.
        /// </summary>
        public void UpdateFunctionHealth(ComponentHealth componentHealth)
        {
            if (componentHealth == null)
                throw new ArgumentNullException(nameof(componentHealth));

            Function = ConvertToSubsystemHealth(componentHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the Database subsystem health status.
        /// This method is called when receiving dbStatusChanged SignalR events.
        /// </summary>
        public void UpdateDatabaseHealth(ComponentHealth componentHealth)
        {
            if (componentHealth == null)
                throw new ArgumentNullException(nameof(componentHealth));

            Database = ConvertToSubsystemHealth(componentHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Updates the FileShare subsystem health status.
        /// This method is called when receiving fileShareStatusChanged SignalR events.
        /// </summary>
        public void UpdateFileShareHealth(ComponentHealth componentHealth)
        {
            if (componentHealth == null)
                throw new ArgumentNullException(nameof(componentHealth));

            FileShare = ConvertToSubsystemHealth(componentHealth);
            LastUpdatedAt = DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Applies a full system health update from a SystemHealthStatus DTO.
        /// This method is called when receiving SystemHealthUpdated SignalR events.
        /// </summary>
        public void ApplyFullUpdate(SystemHealthStatus healthStatus)
        {
            if (healthStatus == null)
                throw new ArgumentNullException(nameof(healthStatus));

            Api = ConvertToSubsystemHealth(healthStatus.API);
            Function = ConvertToSubsystemHealth(healthStatus.Function);
            Database = ConvertToSubsystemHealth(healthStatus.Database);
            FileShare = ConvertToSubsystemHealth(healthStatus.FileShare);
            LastUpdatedAt = healthStatus.LastUpdated ?? DateTime.UtcNow;
            IsHealthy = CalculateIsHealthy();
        }

        /// <summary>
        /// Calculates whether the overall system is healthy based on all subsystems.
        /// The system is considered healthy only if all subsystems have a Healthy status.
        /// </summary>
        private bool CalculateIsHealthy()
        {
            return Api.Status == HealthStatus.Healthy &&
                   Function.Status == HealthStatus.Healthy &&
                   Database.Status == HealthStatus.Healthy &&
                   FileShare.Status == HealthStatus.Healthy;
        }

        /// <summary>
        /// Converts a ComponentHealth to SubsystemHealth.
        /// Creates a new instance to ensure immutability.
        /// </summary>
        private static SubsystemHealth ConvertToSubsystemHealth(ComponentHealth componentHealth)
        {
            return new SubsystemHealth
            {
                Status = componentHealth.Status,
                LastHeartbeat = componentHealth.LastHeartbeat,
                Message = componentHealth.Message
            };
        }
    }
}
