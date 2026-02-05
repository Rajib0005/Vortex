namespace Vortex.Contracts;

public record NotificationRequested(
    string Destination,
    string Subject,
    string Body,
    NotificationType Type
);

public enum NotificationType
{
    Email,
    Push
}

public static class NotificationTypeExtensions
{
    public static string ParseTemplate(
        this NotificationRequested notificationTemplate,
        IDictionary<string, string> values)
    {
        return values.Aggregate(
            notificationTemplate.Body,
            (curr, kv)=> curr.Replace($"{{{kv.Key}}}", kv.Value)
        );
    } 
}
