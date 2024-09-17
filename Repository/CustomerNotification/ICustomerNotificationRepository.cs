using hoslog.signalr.api.Models.Common;
using hoslog.signalr.api.Models.CustomerNotification;

namespace hoslog.signalr.api.Repository.CustomerNotification;
public interface ICustomerNotificationRepository
{
    Task<CommonDBResponse> InsertNotificationAsync(NotificationManagementModel request);
}