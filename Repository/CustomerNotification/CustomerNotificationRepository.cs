using Dapper;
using hoslog.signalr.api.Models.CustomerNotification;
using hoslog.signalr.api.Repository.DBModels.CustomerNotification;

namespace hoslog.signalr.api.Repository.CustomerNotification;
public class CustomerNotificationRepository : ICustomerNotificationRepository
{
    private readonly RepositoryDao _dao;
    public CustomerNotificationRepository(IConfiguration configuration)
    {
        _dao = new RepositoryDao(configuration);
    }

    public async Task<NotificationCommon> InsertNotificationAsync(NotificationManagementModel request)
    {
        var parameters = new DynamicParameters(new
        {
            request.agentId,
            request.notificationType,
            request.actionUser
        });
        string proc = "service_apiproc_insert_customer_notification";
        var dbResponse = await _dao.ExecuteAsync<NotificationCommon>(proc, parameters);
        return dbResponse;
    }
}