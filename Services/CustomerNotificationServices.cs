using hoslog.signalr.api.Hubs;
using hoslog.signalr.api.Models.Cache;
using hoslog.signalr.api.Models.CustomerNotification;
using hoslog.signalr.api.Repository.CustomerNotification;
using Microsoft.AspNetCore.SignalR;

namespace hoslog.signalr.api.Services;

public class CustomerNotificationServices
{
    private readonly IHubContext<CustomerNotificationHub> _hubContext;
    private readonly ICustomerNotificationRepository _customerNotificationRepository;
    private readonly RedisCacheSetting _cacheSettings;
    private string _cacheKey = "";
    private readonly RedisConnectionService _redisConnectionService;

    public CustomerNotificationServices(IHubContext<CustomerNotificationHub> hubContext,
     RedisCacheSetting cacheSettings,
     ICustomerNotificationRepository customerNotificationRepository,
     RedisConnectionService redisConnectionService)
    {
        _hubContext = hubContext;
        _cacheSettings = cacheSettings;
        _customerNotificationRepository = customerNotificationRepository;
        _redisConnectionService = redisConnectionService;
    }

    public async Task SendNotificationAsync(NotificationManagementModel request)
    {
        try
        {
            var dbResponse = _customerNotificationRepository.InsertNotificationAsync(request);
            var connectionIds = await _redisConnectionService.GetConnectionsAsync(request.agentId);
            var sendTasks = new List<Task>();

            foreach (var connectionId in connectionIds)
            {
                sendTasks.Add(_hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveNotification", request.agentId, request.notificationBody));
            }
            await Task.WhenAll(sendTasks);
        }
        catch (Exception ex)
        {
            throw;
        }
        // await _hubContext.Clients.All.SendAsync("ReceiveNotification", request.agentId, request.notificationBody);
    }

    public async Task<List<object>> GetCustomerNotificationAsync(string customerId)
    {
        //get customer notification
        return new List<object>();
    }

    public async Task MarkNotificationAsReadAsync(string customerId, string notificationId)
    {
        //update customer notification table
    }
}