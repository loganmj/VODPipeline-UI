using VODPipeline.UI.Components;
using VODPipeline.UI.Data;

namespace VODPipeline.UI.Tests;

public class CurrentJobViewModelTests
{
    #region FromJobStatus Tests

    [Fact]
    public void FromJobStatus_WithValidJobStatus_CreatesViewModel()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(jobId, viewModel.JobId);
        Assert.Equal("test.mp4", viewModel.FileName);
        Assert.Equal("Downloading", viewModel.Stage);
        Assert.Equal(50, viewModel.Percent);
        Assert.True(viewModel.IsRunning);
        Assert.Equal(jobStatus.Timestamp.Value, viewModel.StartedAt);
        Assert.Equal(jobStatus.Timestamp.Value, viewModel.LastUpdatedAt);
    }

    [Fact]
    public void FromJobStatus_WithNullJobStatus_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => CurrentJobViewModel.FromJobStatus(null!));
    }

    [Fact]
    public void FromJobStatus_WithInvalidGuid_ThrowsArgumentException()
    {
        // Arrange
        var jobStatus = new JobStatus
        {
            JobId = "not-a-guid",
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => CurrentJobViewModel.FromJobStatus(jobStatus));
        Assert.Contains("JobStatus.JobId must be a valid GUID", exception.Message);
    }

    [Fact]
    public void FromJobStatus_WithNullJobId_ThrowsArgumentException()
    {
        // Arrange
        var jobStatus = new JobStatus
        {
            JobId = null,
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => CurrentJobViewModel.FromJobStatus(jobStatus));
        Assert.Contains("JobStatus.JobId must be a valid GUID", exception.Message);
    }

    [Fact]
    public void FromJobStatus_WithEmptyGuid_ThrowsArgumentException()
    {
        // Arrange
        var jobStatus = new JobStatus
        {
            JobId = Guid.Empty.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => CurrentJobViewModel.FromJobStatus(jobStatus));
        Assert.Contains("JobStatus.JobId must be a valid GUID", exception.Message);
    }

    [Fact]
    public void FromJobStatus_WithNullTimestamp_UsesUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = null
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);
        var after = DateTime.UtcNow;

        // Assert
        Assert.InRange(viewModel.StartedAt, before, after);
        Assert.InRange(viewModel.LastUpdatedAt, before, after);
    }

    [Fact]
    public void FromJobStatus_WithTimestamp_UsesSameForBothStartedAtAndLastUpdatedAt()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = timestamp
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(timestamp, viewModel.StartedAt);
        Assert.Equal(timestamp, viewModel.LastUpdatedAt);
    }

    [Fact]
    public void FromJobStatus_WithNullOptionalFields_UsesDefaults()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = null,
            Stage = null,
            Percent = null,
            IsRunning = false,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(string.Empty, viewModel.FileName);
        Assert.Equal(string.Empty, viewModel.Stage);
        Assert.Equal(0, viewModel.Percent);
        Assert.False(viewModel.IsRunning);
    }

    [Fact]
    public void FromJobStatus_ClampsPercentToValidRange_UpperBound()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 150, // Over 100
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(100, viewModel.Percent);
    }

    [Fact]
    public void FromJobStatus_ClampsPercentToValidRange_LowerBound()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = -50, // Negative value
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(0, viewModel.Percent);
    }

    [Fact]
    public void FromJobStatus_WithoutEstimatedTimeRemaining_StartsWithNull()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Null(viewModel.EstimatedRemaining);
    }

    #endregion

    #region ApplyUpdate Tests

    [Fact]
    public void ApplyUpdate_WithNullUpdate_ThrowsArgumentNullException()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.ApplyUpdate(null!));
    }

    [Fact]
    public void ApplyUpdate_WithMatchingJobId_UpdatesFields()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 75,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = viewModel.ApplyUpdate(update);

        // Assert
        Assert.True(result);
        Assert.Equal("Processing", viewModel.Stage);
        Assert.Equal(75, viewModel.Percent);
        Assert.True(viewModel.IsRunning);
    }

    [Fact]
    public void ApplyUpdate_WithMismatchedJobId_ReturnsFalse()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var differentJobId = Guid.NewGuid();
        var update = new JobStatus
        {
            JobId = differentJobId.ToString(),
            Stage = "Processing",
            Percent = 75,
            IsRunning = true
        };

        // Act
        var result = viewModel.ApplyUpdate(update);

        // Assert
        Assert.False(result);
        Assert.Equal("Downloading", viewModel.Stage); // Should not change
        Assert.Equal(50, viewModel.Percent); // Should not change
    }

    [Fact]
    public void ApplyUpdate_WithNullJobIdInUpdate_UpdatesFieldsRegardlessOfJobId()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = null, // Null JobId parses to Guid.Empty, allowing update to proceed
            Stage = "Processing",
            Percent = 75,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = viewModel.ApplyUpdate(update);

        // Assert
        Assert.True(result);
        Assert.Equal("Processing", viewModel.Stage);
        Assert.Equal(75, viewModel.Percent);
    }

    [Fact]
    public void ApplyUpdate_WithNullStage_KeepsCurrentStage()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = null,
            Percent = 75,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal("Downloading", viewModel.Stage);
        Assert.Equal(75, viewModel.Percent);
    }

    [Fact]
    public void ApplyUpdate_WithNullPercent_KeepsCurrentPercent()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = null,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal("Processing", viewModel.Stage);
        Assert.Equal(50, viewModel.Percent);
    }

    [Fact]
    public void ApplyUpdate_ClampsPercentToValidRange_UpperBound()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 150, // Over 100
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal(100, viewModel.Percent);
    }

    [Fact]
    public void ApplyUpdate_ClampsPercentToValidRange_LowerBound()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = -20, // Negative value
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal(0, viewModel.Percent);
    }

    [Fact]
    public void ApplyUpdate_WhenComplete_SetsIsRunningFalseAndEstimatedRemainingZero()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 90,
            IsRunning = true,
            Timestamp = DateTime.UtcNow.AddMinutes(-10)
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Complete",
            Percent = 100,
            IsRunning = true, // Even if update says running
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal(100, viewModel.Percent);
        Assert.False(viewModel.IsRunning); // Should be set to false
        Assert.Equal(TimeSpan.Zero, viewModel.EstimatedRemaining);
    }

    [Fact]
    public void ApplyUpdate_CalculatesEstimatedTimeBasedOnProgress()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow.AddMinutes(-10)
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 60,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert - EstimatedRemaining should be calculated
        Assert.NotNull(viewModel.EstimatedRemaining);
    }

    [Fact]
    public void ApplyUpdate_CalculatesEstimatedTimeRemaining_BasedOnProgress()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddMinutes(-10);
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Downloading",
            Percent = 25,
            IsRunning = true,
            Timestamp = startTime
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var updateTime = DateTime.UtcNow;
        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Downloading",
            Percent = 50,
            IsRunning = true,
            Timestamp = updateTime
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.NotNull(viewModel.EstimatedRemaining);
        // At 50% with ~10 minutes elapsed, should estimate ~10 minutes remaining
        Assert.True(viewModel.EstimatedRemaining.Value.TotalMinutes > 8);
        Assert.True(viewModel.EstimatedRemaining.Value.TotalMinutes < 12);
    }

    [Fact]
    public void ApplyUpdate_WithZeroPercent_SetsEstimatedRemainingToNull()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddMinutes(-10);
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Starting",
            Percent = 0,
            IsRunning = true,
            Timestamp = startTime
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Starting",
            Percent = 0,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Null(viewModel.EstimatedRemaining);
    }

    #endregion

    #region SetError and ClearError Tests

    [Fact]
    public void SetError_SetsErrorMessageAndStopsJob()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Act
        viewModel.SetError("An error occurred");

        // Assert
        Assert.Equal("An error occurred", viewModel.ErrorMessage);
        Assert.False(viewModel.IsRunning);
        Assert.Null(viewModel.EstimatedRemaining);
    }

    [Fact]
    public void ClearError_RemovesErrorMessage()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);
        viewModel.SetError("An error occurred");

        // Act
        viewModel.ClearError();

        // Assert
        Assert.Null(viewModel.ErrorMessage);
    }

    #endregion

    #region Computed Properties Tests

    [Fact]
    public void Elapsed_ReturnsCorrectTimeSpan()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddMinutes(-10);
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = startTime
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.Equal(TimeSpan.Zero, viewModel.Elapsed);
    }

    [Fact]
    public void Elapsed_WithUpdatedTimestamp_ReturnsCorrectTimeSpan()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddMinutes(-10);
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = startTime
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Update with new timestamp
        var updateTime = DateTime.UtcNow;
        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 60,
            IsRunning = true,
            Timestamp = updateTime
        };
        viewModel.ApplyUpdate(update);

        // Assert
        var expectedElapsed = updateTime - startTime;
        Assert.Equal(expectedElapsed, viewModel.Elapsed);
    }

    [Fact]
    public void Elapsed_WithTimestampBeforeStartedAt_ReturnsZero()
    {
        // This test simulates a scenario where an update has a timestamp before the job started
        // which would be an unusual case but should be handled gracefully
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = startTime
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Apply an update with an earlier timestamp
        var earlierTime = DateTime.UtcNow.AddMinutes(-10);
        var update = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 60,
            IsRunning = true,
            Timestamp = earlierTime
        };
        viewModel.ApplyUpdate(update);

        // Assert
        Assert.Equal(TimeSpan.Zero, viewModel.Elapsed);
    }

    [Fact]
    public void IsIdle_ReturnsTrueWhenNotRunning()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Complete",
            Percent = 100,
            IsRunning = false,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.True(viewModel.IsIdle);
        Assert.False(viewModel.IsRunning);
    }

    [Fact]
    public void IsIdle_ReturnsFalseWhenRunning()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.False(viewModel.IsIdle);
        Assert.True(viewModel.IsRunning);
    }

    [Fact]
    public void IsComplete_ReturnsTrueWhenPercentIs100()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Complete",
            Percent = 100,
            IsRunning = false,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.True(viewModel.IsComplete);
    }

    [Fact]
    public void IsComplete_ReturnsFalseWhenPercentIsLessThan100()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 99,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.False(viewModel.IsComplete);
    }

    [Fact]
    public void HasError_ReturnsTrueWhenErrorMessageExists()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Act
        viewModel.SetError("An error occurred");

        // Assert
        Assert.True(viewModel.HasError);
    }

    [Fact]
    public void HasError_ReturnsFalseWhenNoErrorMessage()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "test.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Assert
        Assert.False(viewModel.HasError);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void CompleteJobScenario_FromStartToFinish()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;

        // Create initial job
        var initialStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "video.mp4",
            Stage = "Starting",
            Percent = 0,
            IsRunning = true,
            Timestamp = startTime
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(initialStatus);

        // Assert initial state
        Assert.Equal(0, viewModel.Percent);
        Assert.True(viewModel.IsRunning);
        Assert.False(viewModel.IsComplete);

        // Update to downloading
        var downloadingUpdate = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Downloading",
            Percent = 25,
            IsRunning = true,
            Timestamp = startTime.AddMinutes(2)
        };
        viewModel.ApplyUpdate(downloadingUpdate);
        Assert.Equal("Downloading", viewModel.Stage);
        Assert.Equal(25, viewModel.Percent);

        // Update to processing
        var processingUpdate = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = startTime.AddMinutes(5)
        };
        viewModel.ApplyUpdate(processingUpdate);
        Assert.Equal("Processing", viewModel.Stage);
        Assert.Equal(50, viewModel.Percent);

        // Update to complete
        var completeUpdate = new JobStatus
        {
            JobId = jobId.ToString(),
            Stage = "Complete",
            Percent = 100,
            IsRunning = true,
            Timestamp = startTime.AddMinutes(10)
        };
        viewModel.ApplyUpdate(completeUpdate);
        Assert.Equal("Complete", viewModel.Stage);
        Assert.Equal(100, viewModel.Percent);
        Assert.False(viewModel.IsRunning); // Should be set to false automatically
        Assert.True(viewModel.IsComplete);
        Assert.Equal(TimeSpan.Zero, viewModel.EstimatedRemaining);
    }

    [Fact]
    public void ErrorScenario_JobEncountersError()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId.ToString(),
            FileName = "video.mp4",
            Stage = "Processing",
            Percent = 45,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        // Act - Error occurs
        viewModel.SetError("Processing failed: Invalid format");

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Processing failed: Invalid format", viewModel.ErrorMessage);
        Assert.False(viewModel.IsRunning);
        Assert.Null(viewModel.EstimatedRemaining);

        // Clear error
        viewModel.ClearError();
        Assert.False(viewModel.HasError);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public void UpdateWithDifferentJobId_IgnoresUpdate()
    {
        // Arrange
        var jobId1 = Guid.NewGuid();
        var jobId2 = Guid.NewGuid();
        var jobStatus = new JobStatus
        {
            JobId = jobId1.ToString(),
            FileName = "video1.mp4",
            Stage = "Processing",
            Percent = 50,
            IsRunning = true,
            Timestamp = DateTime.UtcNow
        };
        var viewModel = CurrentJobViewModel.FromJobStatus(jobStatus);

        var wrongUpdate = new JobStatus
        {
            JobId = jobId2.ToString(),
            Stage = "Complete",
            Percent = 100,
            IsRunning = false
        };

        // Act
        var result = viewModel.ApplyUpdate(wrongUpdate);

        // Assert
        Assert.False(result);
        Assert.Equal("Processing", viewModel.Stage);
        Assert.Equal(50, viewModel.Percent);
        Assert.True(viewModel.IsRunning);
    }

    #endregion
}
