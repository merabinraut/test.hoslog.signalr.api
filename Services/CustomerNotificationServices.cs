using System.Net;
using hoslog.signalr.api.Hubs;
using hoslog.signalr.api.Models.Cache;
using hoslog.signalr.api.Models.Common.CommonAPIResponse;
using hoslog.signalr.api.Models.CustomerNotification;
using hoslog.signalr.api.Repository.CustomerNotification;
using hoslog.signalr.api.Repository.DBModels.CustomerNotification;
using hoslog.signalr.api.Utilities.Helper;
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

    public async Task<(HttpStatusCode statusCode, CommonAPIResponse response)> SendNotificationAsync(NotificationManagementModel request)
    {
        try
        {
            var dbRequest = request.MapObject<NotificationManagementCommon>();
            var dbResponse = await _customerNotificationRepository.InsertNotificationAsync(request);
            if (!string.IsNullOrEmpty(dbResponse.code) && dbResponse.code != "0")
                return (HttpStatusCode.BadRequest, new CommonAPIResponse
                {
                    code = "1",
                    message = "Failed to insert notification"
                });

            var dbResponseObject = dbResponse.MapObject<NotificationModel>();
            var connectionIds = await _redisConnectionService.GetConnectionsAsync(request.agentId);

            var sendTasks = new List<Task>();
            foreach (var connectionId in connectionIds)
            {
                sendTasks.Add(_hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveNotification", dbResponseObject));
                sendTasks.Add(_hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveNotificationCount", dbResponseObject.notificationUnReadCount));
            }
            await Task.WhenAll(sendTasks);

            return (HttpStatusCode.OK, new CommonAPIResponse
            {
                code = "0",
                message = "Success"
            });
        }
        catch (Exception ex)
        {
            return (HttpStatusCode.BadRequest, new CommonAPIResponse
            {
                code = "1",
                message = $"An error occurred while sending notification: {ex.Message}"
            });
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