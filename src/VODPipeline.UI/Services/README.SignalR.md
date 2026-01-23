# SignalRConnectionService

## Overview
The `SignalRConnectionService` is a centralized service for managing SignalR connections throughout the VODPipeline UI application. It provides a single connection to the `/hubs/pipeline` endpoint with automatic reconnection and event handling capabilities.

## Configuration
Configure the SignalR hub URL in `appsettings.json`:

```json
{
  "SignalR": {
    "HubUrl": "http://localhost:5000/hubs/pipeline"
  }
}
```

## Usage in Components

### 1. Inject the Service
```csharp
@inject SignalRConnectionService SignalRService
```

### 2. Start the Connection
```csharp
protected override async Task OnInitializedAsync()
{
    try
    {
        await SignalRService.StartAsync();
    }
    catch (Exception ex)
    {
        // Handle connection failure
    }
}
```

### 3. Subscribe to Hub Events
```csharp
private IDisposable? eventSubscription;

protected override async Task OnInitializedAsync()
{
    await SignalRService.StartAsync();
    
    // Subscribe to events from the hub
    eventSubscription = SignalRService.On<JobStatus>("jobProgressUpdated", async (status) =>
    {
        await InvokeAsync(() =>
        {
            // Update component state
            StateHasChanged();
        });
    });
}
```

### 4. Monitor Connection Status
```csharp
// Check connection state
var isConnected = SignalRService.IsConnected;
var state = SignalRService.ConnectionState;

// Subscribe to connection lifecycle events
SignalRService.Reconnecting += async (error) =>
{
    // Handle reconnection attempt
};

SignalRService.Reconnected += async (connectionId) =>
{
    // Handle successful reconnection
};

SignalRService.Closed += async (error) =>
{
    // Handle connection closed
};

SignalRService.Connected += async () =>
{
    // Handle initial connection
};
```

### 5. Invoke Server Methods
```csharp
// Invoke a method on the server
await SignalRService.InvokeAsync("SendMessage", "Hello, Server!");

// Invoke a method and get a result
var result = await SignalRService.InvokeAsync<string>("GetData", "parameter");
```

### 6. Cleanup
```csharp
public async ValueTask DisposeAsync()
{
    eventSubscription?.Dispose();
    // Note: The service is a singleton and will be disposed by the DI container
}
```

## Features

- **Automatic Reconnection**: Uses SignalR's built-in automatic reconnection with exponential backoff
- **Connection State Tracking**: Provides `IsConnected` and `ConnectionState` properties
- **Event Lifecycle Hooks**: Exposes `Reconnecting`, `Reconnected`, `Closed`, and `Connected` events
- **Thread-Safe**: Uses semaphore locks to prevent concurrent connection attempts
- **Comprehensive Logging**: Logs all connection lifecycle events for debugging
- **Error Handling**: Gracefully handles connection failures with proper cleanup

## Thread Safety

The service uses a `SemaphoreSlim` to ensure thread-safe connection management. Multiple concurrent calls to `StartAsync()` will be serialized, and only one connection will be established.

## Best Practices

1. **Singleton Lifetime**: The service is registered as a singleton in `Program.cs`, ensuring a single connection across the application
2. **Early Initialization**: Start the connection early in the application lifecycle (e.g., in `App.razor` or main layout)
3. **Event Subscriptions**: Always dispose of event subscriptions when components are disposed
4. **Error Handling**: Always wrap `StartAsync()` in try-catch blocks and handle connection failures gracefully
5. **Connection Status**: Display connection status to users for better UX when connection is lost
