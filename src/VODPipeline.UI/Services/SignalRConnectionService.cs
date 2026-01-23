using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace VODPipeline.UI.Services
{
    public class SignalRConnectionService : IAsyncDisposable
    {
        private readonly ILogger<SignalRConnectionService> _logger;
        private readonly string _hubUrl;
        private HubConnection? _hubConnection;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public event Func<Exception?, Task>? Reconnecting;
        public event Func<string?, Task>? Reconnected;
        public event Func<Exception?, Task>? Closed;
        public event Func<Task>? Connected;

        public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public SignalRConnectionService(IConfiguration configuration, ILogger<SignalRConnectionService> logger)
        {
            _logger = logger;
            var hubUrl = configuration["SignalR:HubUrl"];
            
            if (string.IsNullOrEmpty(hubUrl))
            {
                throw new InvalidOperationException("Missing required 'SignalR:HubUrl' configuration value.");
            }

            _hubUrl = hubUrl;
        }

        public async Task StartAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                // If connection is active or connecting, don't start a new one
                if (_hubConnection is not null && _hubConnection.State != HubConnectionState.Disconnected)
                {
                    _logger.LogWarning("SignalR connection already started or in progress. Current state: {State}", _hubConnection.State);
                    return;
                }

                // Dispose existing disconnected connection if present before creating a new one
                if (_hubConnection is not null)
                {
                    try
                    {
                        await _hubConnection.DisposeAsync();
                    }
                    catch (Exception disposeEx)
                    {
                        _logger.LogWarning(disposeEx, "Error disposing existing SignalR connection before creating new one.");
                    }
                    finally
                    {
                        _hubConnection = null;
                    }
                }

                _logger.LogInformation("Initializing SignalR connection to {HubUrl}", _hubUrl);

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .WithAutomaticReconnect()
                    .Build();

                // Wire up connection lifecycle events
                _hubConnection.Reconnecting += async (error) =>
                {
                    _logger.LogWarning(error, "SignalR connection lost. Attempting to reconnect...");
                    if (Reconnecting != null)
                    {
                        await InvokeEventHandlersAsync(Reconnecting, error);
                    }
                };

                _hubConnection.Reconnected += async (connectionId) =>
                {
                    _logger.LogInformation("SignalR connection re-established. Connection ID: {ConnectionId}", connectionId);
                    if (Reconnected != null)
                    {
                        await InvokeEventHandlersAsync(Reconnected, connectionId);
                    }
                };

                _hubConnection.Closed += async (error) =>
                {
                    if (error != null)
                    {
                        _logger.LogError(error, "SignalR connection closed with error.");
                    }
                    else
                    {
                        _logger.LogInformation("SignalR connection closed.");
                    }
                    
                    if (Closed != null)
                    {
                        await InvokeEventHandlersAsync(Closed, error);
                    }
                };

                await _hubConnection.StartAsync();
                _logger.LogInformation("SignalR connection established successfully. Connection ID: {ConnectionId}", _hubConnection.ConnectionId);

                if (Connected != null)
                {
                    await InvokeEventHandlersAsync(Connected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish SignalR connection to {HubUrl}", _hubUrl);
                
                // Cleanup on failure
                if (_hubConnection is not null)
                {
                    try
                    {
                        await _hubConnection.DisposeAsync();
                    }
                    catch (Exception disposeEx)
                    {
                        _logger.LogError(disposeEx, "Error disposing SignalR connection after failed start.");
                    }
                    finally
                    {
                        _hubConnection = null;
                    }
                }
                
                throw;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task StopAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (_hubConnection is not null)
                {
                    _logger.LogInformation("Stopping SignalR connection...");
                    
                    try
                    {
                        await _hubConnection.StopAsync();
                        _logger.LogInformation("SignalR connection stopped successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while stopping SignalR connection.");
                    }

                    try
                    {
                        await _hubConnection.DisposeAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while disposing SignalR connection.");
                    }
                    finally
                    {
                        _hubConnection = null;
                    }
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public IDisposable On<T>(string methodName, Func<T, Task> handler)
        {
            if (_hubConnection is null)
            {
                throw new InvalidOperationException("Cannot register event handler before connection is started. Call StartAsync first.");
            }

            return _hubConnection.On(methodName, handler);
        }

        public IDisposable On<T1, T2>(string methodName, Func<T1, T2, Task> handler)
        {
            if (_hubConnection is null)
            {
                throw new InvalidOperationException("Cannot register event handler before connection is started. Call StartAsync first.");
            }

            return _hubConnection.On(methodName, handler);
        }

        public IDisposable On(string methodName, Func<Task> handler)
        {
            if (_hubConnection is null)
            {
                throw new InvalidOperationException("Cannot register event handler before connection is started. Call StartAsync first.");
            }

            return _hubConnection.On(methodName, handler);
        }

        public async Task<T?> InvokeAsync<T>(string methodName, params object[] args)
        {
            if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("Cannot invoke method on disconnected hub connection.");
            }

            return await _hubConnection.InvokeAsync<T>(methodName, args);
        }

        public async Task InvokeAsync(string methodName, params object[] args)
        {
            if (_hubConnection is null || _hubConnection.State != HubConnectionState.Connected)
            {
                throw new InvalidOperationException("Cannot invoke method on disconnected hub connection.");
            }

            await _hubConnection.InvokeAsync(methodName, args);
        }

        private static async Task InvokeEventHandlersAsync<T>(Func<T, Task> eventHandler, T arg)
        {
            foreach (var handler in eventHandler.GetInvocationList().Cast<Func<T, Task>>())
            {
                try
                {
                    await handler(arg);
                }
                catch
                {
                    // Log but don't throw - one handler shouldn't break others
                    // The logger is not available in static context, so handlers should handle their own errors
                }
            }
        }

        private static async Task InvokeEventHandlersAsync(Func<Task> eventHandler)
        {
            foreach (var handler in eventHandler.GetInvocationList().Cast<Func<Task>>())
            {
                try
                {
                    await handler();
                }
                catch
                {
                    // Log but don't throw - one handler shouldn't break others
                    // The logger is not available in static context, so handlers should handle their own errors
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
            _connectionLock.Dispose();
        }
    }
}
