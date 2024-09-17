using hoslog.signalr.api.Models.CustomerNotification;
using hoslog.signalr.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace hoslog.signalr.api.Controller;

[ApiController]
[Route("api")]
public class CustomerNotificationController : ControllerBase
{
    private readonly CustomerNotificationServices _notificationServices;
    public CustomerNotificationController(CustomerNotificationServices notificationServices)
    {
        _notificationServices = notificationServices;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationManagementModel request)
    {
        await _notificationServices.SendNotificationAsync(request);
        return Ok();
    }

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCustomerNotifications(string customerId)
    {
        var notifications = await _notificationServices.GetCustomerNotificationAsync(customerId);
        return Ok(notifications);   
    }

    [HttpPatch("{notificationId}/mark-read")]
    public async Task<IActionResult> MarkNotificationAsRead(string customerId, string notificationId)
    {
        await _notificationServices.MarkNotificationAsReadAsync(customerId, notificationId);
        return Ok();
    }
}