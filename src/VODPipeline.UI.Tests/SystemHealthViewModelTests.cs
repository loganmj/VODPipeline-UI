using VODPipeline.UI.Components;
using VODPipeline.UI.Data;
using SystemHealth = VODPipeline.UI.Data.SystemHealth;

namespace VODPipeline.UI.Tests;

public class SystemHealthViewModelTests
{
    #region FromSystemHealthResponse Tests

    [Fact]
    public void FromSystemHealthResponse_WithValidHealthStatus_CreatesViewModel()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "API is running"
                },
                ["Function"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Function is running"
                },
                ["Database"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Database is running"
                },
                ["FileShare"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "FileShare is running"
                }
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

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
    public void FromSystemHealthResponse_WithNullHealthStatus_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SystemHealthViewModel.FromSystemHealthResponse(null!));
    }

    [Fact]
    public void FromSystemHealthResponse_WithNullLastUpdated_UsesUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy }
            },
            LastUpdated = null
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(viewModel.LastUpdatedAt, before, after);
    }

    [Fact]
    public void FromSystemHealthResponse_WithOneDegradedSubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Degraded }, // Degraded
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy }
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthResponse_WithOneUnhealthySubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Database"] = new SystemHealth { Status = HealthStatus.Unhealthy }, // Unhealthy
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy }
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthResponse_WithOneUnknownSubsystem_IsNotHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Unknown } // Unknown
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert
        Assert.False(viewModel.IsHealthy);
        Assert.True(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthResponse_WithAllHealthySubsystems_IsHealthy()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy }
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);
    }

    [Fact]
    public void FromSystemHealthResponse_WithMissingSubsystemKeys_DefaultsToUnknown()
    {
        // Arrange - Only include some subsystems in the dictionary
        var healthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                // Function is missing
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy }
                // FileShare is missing
            },
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert
        Assert.NotNull(viewModel.Api);
        Assert.Equal(HealthStatus.Healthy, viewModel.Api.Status);
        
        Assert.NotNull(viewModel.Function);
        Assert.Equal(HealthStatus.Unknown, viewModel.Function.Status); // Should default to Unknown
        
        Assert.NotNull(viewModel.Database);
        Assert.Equal(HealthStatus.Healthy, viewModel.Database.Status);
        
        Assert.NotNull(viewModel.FileShare);
        Assert.Equal(HealthStatus.Unknown, viewModel.FileShare.Status); // Should default to Unknown
        
        Assert.False(viewModel.IsHealthy); // Not healthy because Function and FileShare are Unknown
    }

    [Fact]
    public void FromSystemHealthResponse_WithNullSystems_CreatesDefaultSubsystems()
    {
        // Arrange
        var healthStatus = new SystemHealthResponse
        {
            Systems = null!, // Null dictionary - intentionally testing null handling
            LastUpdated = DateTime.UtcNow
        };

        // Act
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Assert - All subsystems should be created with Unknown status
        Assert.NotNull(viewModel.Api);
        Assert.Equal(HealthStatus.Unknown, viewModel.Api.Status);
        
        Assert.NotNull(viewModel.Function);
        Assert.Equal(HealthStatus.Unknown, viewModel.Function.Status);
        
        Assert.NotNull(viewModel.Database);
        Assert.Equal(HealthStatus.Unknown, viewModel.Database.Status);
        
        Assert.NotNull(viewModel.FileShare);
        Assert.Equal(HealthStatus.Unknown, viewModel.FileShare.Status);
        
        Assert.False(viewModel.IsHealthy);
    }

    [Fact]
    public void ApplyFullUpdate_WithNullSystems_CreatesDefaultSubsystems()
    {
        // Arrange
        var initialHealthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(initialHealthStatus);
        
        var updatedHealthStatus = new SystemHealthResponse
        {
            Systems = null!, // Null dictionary - intentionally testing null handling
            LastUpdated = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyFullUpdate(updatedHealthStatus);

        // Assert - All subsystems should be Unknown
        Assert.Equal(HealthStatus.Unknown, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Unknown, viewModel.Function.Status);
        Assert.Equal(HealthStatus.Unknown, viewModel.Database.Status);
        Assert.Equal(HealthStatus.Unknown, viewModel.FileShare.Status);
        Assert.False(viewModel.IsHealthy);
    }

    #endregion

    #region UpdateApiHealth Tests

    [Fact]
    public void UpdateApiHealth_WithValidComponentHealth_UpdatesApiSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;

        var updatedApiHealth = new SystemHealth
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
        Assert.True(viewModel.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateApiHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateApiHealth(null!));
    }

    #endregion

    #region UpdateFunctionHealth Tests

    [Fact]
    public void UpdateFunctionHealth_WithValidComponentHealth_UpdatesFunctionSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;

        var updatedFunctionHealth = new SystemHealth
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
        Assert.True(viewModel.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateFunctionHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateFunctionHealth(null!));
    }

    #endregion

    #region UpdateDatabaseHealth Tests

    [Fact]
    public void UpdateDatabaseHealth_WithValidComponentHealth_UpdatesDatabaseSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;

        var updatedDatabaseHealth = new SystemHealth
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
        Assert.True(viewModel.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateDatabaseHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateDatabaseHealth(null!));
    }

    #endregion

    #region UpdateFileShareHealth Tests

    [Fact]
    public void UpdateFileShareHealth_WithValidComponentHealth_UpdatesFileShareSubsystem()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;

        var updatedFileShareHealth = new SystemHealth
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
        Assert.True(viewModel.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateFileShareHealth_WithNullComponentHealth_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.UpdateFileShareHealth(null!));
    }

    #endregion

    #region ApplyFullUpdate Tests

    [Fact]
    public void ApplyFullUpdate_WithValidHealthStatus_UpdatesAllSubsystems()
    {
        // Arrange
        var initialHealthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(initialHealthStatus);
        var originalLastUpdated = viewModel.LastUpdatedAt;

        var updatedHealthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth
                {
                    Status = HealthStatus.Degraded,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "API slow"
                },
                ["Function"] = new SystemHealth
                {
                    Status = HealthStatus.Unhealthy,
                    LastHeartbeat = DateTime.UtcNow.AddMinutes(-5),
                    Message = "Function down"
                },
                ["Database"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Database OK"
                },
                ["FileShare"] = new SystemHealth
                {
                    Status = HealthStatus.Degraded,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Low storage"
                }
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
        Assert.True(viewModel.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void ApplyFullUpdate_WithNullHealthStatus_ThrowsArgumentNullException()
    {
        // Arrange
        var healthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(healthStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.ApplyFullUpdate(null!));
    }

    [Fact]
    public void ApplyFullUpdate_WithNullLastUpdated_UsesUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var initialHealthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(initialHealthStatus);

        var updatedHealthStatus = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy }
            },
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
        var initialHealthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(initialHealthStatus);

        // Assert initial state
        Assert.True(viewModel.IsHealthy);
        Assert.False(viewModel.HasErrors);

        // Act - Function becomes degraded
        viewModel.UpdateFunctionHealth(new SystemHealth
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
        viewModel.UpdateApiHealth(new SystemHealth
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
        viewModel.UpdateFunctionHealth(new SystemHealth
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
        viewModel.UpdateApiHealth(new SystemHealth
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
        var initialHealthStatus = CreateHealthySystemHealthResponse();
        var viewModel = SystemHealthViewModel.FromSystemHealthResponse(initialHealthStatus);

        // Act - Update individual subsystems
        viewModel.UpdateApiHealth(new SystemHealth
        {
            Status = HealthStatus.Degraded,
            LastHeartbeat = DateTime.UtcNow,
            Message = "API slow"
        });
        viewModel.UpdateFunctionHealth(new SystemHealth
        {
            Status = HealthStatus.Unhealthy,
            LastHeartbeat = DateTime.UtcNow,
            Message = "Function down"
        });

        // Assert intermediate state
        Assert.Equal(HealthStatus.Degraded, viewModel.Api.Status);
        Assert.Equal(HealthStatus.Unhealthy, viewModel.Function.Status);

        // Act - Apply full update that overrides everything
        var fullUpdate = new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth { Status = HealthStatus.Healthy, Message = "All good" },
                ["Function"] = new SystemHealth { Status = HealthStatus.Healthy, Message = "All good" },
                ["Database"] = new SystemHealth { Status = HealthStatus.Healthy, Message = "All good" },
                ["FileShare"] = new SystemHealth { Status = HealthStatus.Healthy, Message = "All good" }
            },
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

    private static SystemHealthResponse CreateHealthySystemHealthResponse()
    {
        return new SystemHealthResponse
        {
            Systems = new Dictionary<string, SystemHealth>
            {
                ["API"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "API is healthy"
                },
                ["Function"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Function is healthy"
                },
                ["Database"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "Database is healthy"
                },
                ["FileShare"] = new SystemHealth
                {
                    Status = HealthStatus.Healthy,
                    LastHeartbeat = DateTime.UtcNow,
                    Message = "FileShare is healthy"
                }
            },
            LastUpdated = DateTime.UtcNow
        };
    }

    #endregion
}
