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
