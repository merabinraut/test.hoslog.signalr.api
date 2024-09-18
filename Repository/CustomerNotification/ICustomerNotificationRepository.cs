using hoslog.signalr.api.Models.CustomerNotification;
using hoslog.signalr.api.Repository.DBModels.CustomerNotification;

namespace hoslog.signalr.api.Repository.CustomerNotification;
public interface ICustomerNotificationRepository
{
    Task<NotificationCommon> InsertNotificationAsync(NotificationManagementModel request);
}