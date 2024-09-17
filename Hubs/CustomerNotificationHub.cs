using hoslog.signalr.api.Models.Cache;
using hoslog.signalr.api.Services;
using Microsoft.AspNetCore.SignalR;

namespace hoslog.signalr.api.Hubs;

public class CustomerNotificationHub : Hub
{
    private readonly RedisConnectionService _redisConnectionService;
    private readonly RedisCacheSetting _cacheSettings;
    public CustomerNotificationHub(RedisConnectionService redisConnectionService,
     RedisCacheSetting cacheSettings)
    {
        _redisConnectionService = redisConnectionService;
        _cacheSettings = cacheSettings;
    }

    public override async Task OnConnectedAsync()
    {
        var customerId = Context.GetHttpContext().Request.Query["customerId"];

        if (!string.IsNullOrEmpty(customerId))
        {
            await _redisConnectionService.AddConnectionAsync(customerId, Context.ConnectionId, TimeSpan.FromMinutes(_cacheSettings.minimumIntervalInMinutes));
            await Clients.Caller.SendAsync("OnConnected", Context.ConnectionId);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var customerId = Context.GetHttpContext().Request.Query["customerId"];

        await _redisConnectionService.RemoveConnectionAsync(customerId, Context.ConnectionId);
        await _redisConnectionService.RemoveConnectionFromAllAsync(Context.ConnectionId);

        var remainingConnections = await _redisConnectionService.GetConnectionsAsync(customerId);
        if (remainingConnections.Length == 0)
            await _redisConnectionService.DeleteCustomerConnectionsAsync(customerId);
    }

    public async Task SendNotificationAsync(string customerId, string message)
    {
        var connectionIds = await _redisConnectionService.GetConnectionsAsync(customerId);

        var sendTasks = connectionIds.Select(connectionId =>
                    Clients.Client(connectionId).SendAsync("ReceiveNotification", message));

        await Task.WhenAll(sendTasks);
    }
}