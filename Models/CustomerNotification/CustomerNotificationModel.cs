namespace hoslog.signalr.api.Models.CustomerNotification;
public class NotificationManagementModel
{
    public string agentId { get; set; } = null!;
    public string notificationType { get; set; } = null!;
    public string notificationSubject { get; set; } = null!;
    public string notificationBody { get; set; } = null!;
    public string notificationImageURL { get; set; } = null!;
    public string notificationURL { get; set; } = null!;
    public string additionalDetail1 { get; set; } = null!;
    public string actionUser { get; set; } = null!;
}

public class NotificationModel
{
    public string notificationId { get; set; } = null!;
    public string NotificationTo { get; set; } = null!;
    public string NotificationType { get; set; } = null!;
    public string NotificationSubject { get; set; } = null!;
    public string NotificationBody { get; set; } = null!;
    public string NotificationStatus { get; set; } = null!;
    public string NotificationReadStatus { get; set; } = null!;
    public string DateCategory { get; set; } = null!;
    public string FormattedDate { get; set; } = null!;
    public string NotificationURL { get; set; } = null!;
    public string NotificationImage { get; set; } = null!;
    public string UnReadNotification { get; set; } = null!;
}