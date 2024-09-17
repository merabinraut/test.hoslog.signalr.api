using Dapper;
using hoslog.signalr.api.Models.Common;
using hoslog.signalr.api.Models.CustomerNotification;

namespace hoslog.signalr.api.Repository.CustomerNotification;
public class CustomerNotificationRepository : ICustomerNotificationRepository
{
    private readonly RepositoryDao _dao;
    public CustomerNotificationRepository(IConfiguration configuration)
    {
        _dao = new RepositoryDao(configuration);
    }

    public async Task<CommonDBResponse> InsertNotificationAsync(NotificationManagementModel request)
    {
        var parameters = new DynamicParameters(new
        {
            request.agentId,
            request.notificationType,
            request.notificationSubject,
            request.notificationBody,
            request.notificationImageURL,
            request.notificationURL,
            request.additionalDetail1,
            request.actionUser
        });
        string proc = "service_apiproc_insert_customer_notification";
        var dbResponse = await _dao.ExecuteAsync<CommonDBResponse>(proc, parameters);
        return dbResponse;
    }
}