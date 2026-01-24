using VODPipeline.UI.Components;
using VODPipeline.UI.Data;

namespace VODPipeline.UI.Tests;

public class SystemHealthViewModelTests
{
    #region FromSystemHealthStatus Tests

    [Fact]
    public void FromSystemHealthStatus_WithValidHealthStatus_CreatesViewModel()
    {
        // Arrange
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "API is running"
            },
            Function = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Function is running"
            },
            Database = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Database is running"
            },
            FileShare = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "FileShare is running"
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal(HealthStatus.Healthy, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Healthy, viewModel.Function.Status);
        Assert.Equal(HealthStatus.Healthy, viewModel.Database.Status);
        Assert.Equal(HealthStatus.Healthy, viewModel.FileShare.Status);
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);
        Assert.Equal(healthStatus.LastUpdated.Value, viewModel.LastUpdatedAt);
    }

    [Fact]
    public void FromSystemHealthStatus_WithNullHealthStatus_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SystemHealthViewModel.FromSystemHealthStatus(null!));
    }

    [Fact]
    public void FromSystemHealthStatus_WithNullLastUpdated_UsesUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Healthy },
            Database = new ComponentHealth { Status = HealthStatus.Healthy },
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy },
            LastUpdated = null
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(viewModel.LastUpdatedAt, before, after);
    }

    [Fact]
    public void FromSystemHealthStatus_WithOneDegradedSubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Degraded }, // Degraded
            Database = new ComponentHealth { Status = HealthStatus.Healthy },
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthStatus_WithOneUnhealthySubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Healthy },
            Database = new ComponentHealth { Status = HealthStatus.Unhealthy }, // Unhealthy
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthStatus_WithOneUnknownSubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Healthy },
            Database = new ComponentHealth { Status = HealthStatus.Healthy },
            FileShare = new ComponentHealth { Status = HealthStatus.Unknown }, // Unknown
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthStatus_WithAllHealthySubsystems_IsHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Healthy },
            Database = new ComponentHealth { Status = HealthStatus.Healthy },
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Assert
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);
    }

    #endregion

    #region UpdateApiHealth Tests

    [Fact]
    public void UpdateApiHealth_WithValidComponentHealth_UpdatesApiSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;
        
        // Wait a bit to ensure timestamp changes
        System.Threading.Thread.Sleep(10);

        var updatedApiHealth = new ComponentHealth
        {
            Status = HealthStatus.Degraded,
            LastHeartbeat = DateTime.UtcNow,
            Message = "API is slow"
        };

        // Act
        viewModel.UpdateApiHealth(updatedApiHealth);

        // Assert
        Assert.Equal(HealthStatus.Degraded, viewModel.Api.Status);
        Assert.Equal("API is slow", viewModel.Api.Message);
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.True(viewModel.LastUpdatedAt > originalLastUpdated);
    }

    [Fact]
    public void UpdateApiHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateApiHealth(null!));
    }

    #endregion

    #region UpdateFunctionHealth Tests

    [Fact]
    public void UpdateFunctionHealth_WithValidComponentHealth_UpdatesFunctionSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;
        
        // Wait a bit to ensure timestamp changes
        System.Threading.Thread.Sleep(10);

        var updatedFunctionHealth = new ComponentHealth
        {
            Status = HealthStatus.Unhealthy,
            LastHeartbeat = DateTime.UtcNow.AddMinutes(-5),
            Message = "Function not responding"
        };

        // Act
        viewModel.UpdateFunctionHealth(updatedFunctionHealth);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Function.Status);
        Assert.Equal("Function not responding", viewModel.Function.Message);
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.True(viewModel.LastUpdatedAt > originalLastUpdated);
    }

    [Fact]
    public void UpdateFunctionHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateFunctionHealth(null!));
    }

    #endregion

    #region UpdateDatabaseHealth Tests

    [Fact]
    public void UpdateDatabaseHealth_WithValidComponentHealth_UpdatesDatabaseSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;
        
        // Wait a bit to ensure timestamp changes
        System.Threading.Thread.Sleep(10);

        var updatedDatabaseHealth = new ComponentHealth
        {
            Status = HealthStatus.Degraded,
            LastHeartbeat = DateTime.UtcNow,
            Message = "High latency detected"
        };

        // Act
        viewModel.UpdateDatabaseHealth(updatedDatabaseHealth);

        // Assert
        Assert.Equal(HealthStatus.Degraded, viewModel.Database.Status);
        Assert.Equal("High latency detected", viewModel.Database.Message);
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.True(viewModel.LastUpdatedAt > originalLastUpdated);
    }

    [Fact]
    public void UpdateDatabaseHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateDatabaseHealth(null!));
    }

    #endregion

    #region UpdateFileShareHealth Tests

    [Fact]
    public void UpdateFileShareHealth_WithValidComponentHealth_UpdatesFileShareSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;
        
        // Wait a bit to ensure timestamp changes
        System.Threading.Thread.Sleep(10);

        var updatedFileShareHealth = new ComponentHealth
        {
            Status = HealthStatus.Unhealthy,
            LastHeartbeat = DateTime.UtcNow,
            Message = "Storage full"
        };

        // Act
        viewModel.UpdateFileShareHealth(updatedFileShareHealth);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, viewModel.FileShare.Status);
        Assert.Equal("Storage full", viewModel.FileShare.Message);
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.True(viewModel.LastUpdatedAt > originalLastUpdated);
    }

    [Fact]
    public void UpdateFileShareHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateFileShareHealth(null!));
    }

    #endregion

    #region ApplyFullUpdate Tests

    [Fact]
    public void ApplyFullUpdate_WithValidHealthStatus_UpdatesAllSubsystems()
    {
        // Arrange
        var initialHealthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(initialHealthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;
        
        // Wait a bit to ensure timestamp changes
        System.Threading.Thread.Sleep(10);

        var updatedHealthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth
            {
                Status = HealthStatus.Degraded,
                LastHeartbeat = DateTime.UtcNow,
                Message = "API slow"
            },
            Function = new ComponentHealth
            {
                Status = HealthStatus.Unhealthy,
                LastHeartbeat = DateTime.UtcNow.AddMinutes(-5),
                Message = "Function down"
            },
            Database = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Database OK"
            },
            FileShare = new ComponentHealth
            {
                Status = HealthStatus.Degraded,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Low storage"
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyFullUpdate(updatedHealthStatus);

        // Assert
        Assert.Equal(HealthStatus.Degraded, viewModel.Api.Status);
        Assert.Equal("API slow", viewModel.Api.Message);
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Function.Status);
        Assert.Equal("Function down", viewModel.Function.Message);
        Assert.Equal(HealthStatus.Healthy, viewModel.Database.Status);
        Assert.Equal("Database OK", viewModel.Database.Message);
        Assert.Equal(HealthStatus.Degraded, viewModel.FileShare.Status);
        Assert.Equal("Low storage", viewModel.FileShare.Message);
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.True(viewModel.LastUpdatedAt > originalLastUpdated);
    }

    [Fact]
    public void ApplyFullUpdate_WithNullHealthStatus_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.ApplyFullUpdate(null!));
    }

    [Fact]
    public void ApplyFullUpdate_WithNullLastUpdated_UsesUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var initialHealthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(initialHealthStatus);

        var updatedHealthStatus = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy },
            Function = new ComponentHealth { Status = HealthStatus.Healthy },
            Database = new ComponentHealth { Status = HealthStatus.Healthy },
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy },
            LastUpdated = null
        };

        // Act
        viewModel.ApplyFullUpdate(updatedHealthStatus);
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(viewModel.LastUpdatedAt, before, after);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void RealTimeUpdateScenario_MultipleSubsystemUpdates()
    {
        // Arrange - Start with healthy system
        var initialHealthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(initialHealthStatus);

        // Assert initial state
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);

        // Act - Function becomes degraded
        System.Threading.Thread.Sleep(10);
        viewModel.UpdateFunctionHealth(new ComponentHealth
        {
            Status = HealthStatus.Degraded,
            LastHeartbeat = DateTime.UtcNow,
            Message = "Function experiencing delays"
        });

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
        Assert.Equal(HealthStatus.Degraded, viewModel.Function.Status);

        // Act - API becomes unhealthy
        System.Threading.Thread.Sleep(10);
        viewModel.UpdateApiHealth(new ComponentHealth
        {
            Status = HealthStatus.Unhealthy,
            LastHeartbeat = DateTime.UtcNow.AddMinutes(-10),
            Message = "API not responding"
        });

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Degraded, viewModel.Function.Status);

        // Act - Function recovers
        System.Threading.Thread.Sleep(10);
        viewModel.UpdateFunctionHealth(new ComponentHealth
        {
            Status = HealthStatus.Healthy,
            LastHeartbeat = DateTime.UtcNow,
            Message = "Function recovered"
        });

        // Assert - Still not healthy because API is unhealthy
        Assert.False(viewModel.IsHealthy);
        Assert.Equal(HealthStatus.Healthy, viewModel.Function.Status);
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Api.Status);

        // Act - API recovers
        System.Threading.Thread.Sleep(10);
        viewModel.UpdateApiHealth(new ComponentHealth
        {
            Status = HealthStatus.Healthy,
            LastHeartbeat = DateTime.UtcNow,
            Message = "API recovered"
        });

        // Assert - Now healthy again
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);
    }

    [Fact]
    public void FullUpdateOverridesIndividualUpdates()
    {
        // Arrange
        var initialHealthStatus = CreateHealthySystemHealthStatus();
        var viewModel = SystemHealthViewModel.FromSystemHealthStatus(initialHealthStatus);

        // Act - Update individual subsystems
        viewModel.UpdateApiHealth(new ComponentHealth
        {
            Status = HealthStatus.Degraded,
            LastHeartbeat = DateTime.UtcNow,
            Message = "API slow"
        });
        viewModel.UpdateFunctionHealth(new ComponentHealth
        {
            Status = HealthStatus.Unhealthy,
            LastHeartbeat = DateTime.UtcNow,
            Message = "Function down"
        });

        // Assert intermediate state
        Assert.Equal(HealthStatus.Degraded, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Function.Status);

        // Act - Apply full update that overrides everything
        System.Threading.Thread.Sleep(10);
        var fullUpdate = new SystemHealthStatus
        {
            API = new ComponentHealth { Status = HealthStatus.Healthy, Message = "All good" },
            Function = new ComponentHealth { Status = HealthStatus.Healthy, Message = "All good" },
            Database = new ComponentHealth { Status = HealthStatus.Healthy, Message = "All good" },
            FileShare = new ComponentHealth { Status = HealthStatus.Healthy, Message = "All good" },
            LastUpdated = DateTime.UtcNow
        };
        viewModel.ApplyFullUpdate(fullUpdate);

        // Assert - Everything is healthy now
        Assert.True(viewModel.IsHealthy);
        Assert.Equal(HealthStatus.Healthy, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Healthy, viewModel.Function.Status);
        Assert.Equal("All good", viewModel.Api.Message);
        Assert.Equal("All good", viewModel.Function.Message);
    }

    #endregion

    #region Helper Methods

    private static SystemHealthStatus CreateHealthySystemHealthStatus()
    {
        return new SystemHealthStatus
        {
            API = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "API is healthy"
            },
            Function = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Function is healthy"
            },
            Database = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "Database is healthy"
            },
            FileShare = new ComponentHealth
            {
                Status = HealthStatus.Healthy,
                LastHeartbeat = DateTime.UtcNow,
                Message = "FileShare is healthy"
            },
            LastUpdated = DateTime.UtcNow
        };
    }

    #endregion
}
