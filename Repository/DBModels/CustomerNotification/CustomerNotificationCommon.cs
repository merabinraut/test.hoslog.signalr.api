using hoslog.signalr.api.Models.Common;

namespace hoslog.signalr.api.Repository.DBModels.CustomerNotification;

public class NotificationManagementCommon
{
    public string agentId { get; set; } = null!;
    public string notificationType { get; set; } = null!;
    public string actionUser { get; set; } = null!;
}

public class NotificationCommon : CommonDBResponse
{
    public string notificationId { get; set; } = null!;
    public string agentId { get; set; } = null!;
    public string notificationType { get; set; } = null!;
    public string notificationSubject { get; set; } = null!;
    public string notificationBody { get; set; } = null!;
    public string notificationImageURL { get; set; } = null!;
    public string notificationReadStatus { get; set; } = null!;
    public string notificationURL { get; set; } = null!;
    public string additionalDetail1 { get; set; } = null!;
    public string createdBy { get; set; } = null!;
    public string createdDate { get; set; } = null!;
    public string updatedBy { get; set; } = null!;
    public string updatedDate { get; set; } = null!;
    public int notificationUnReadCount { get; set; } = 0;
}