using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>Push notifications - send, schedule, images, delivery status. Appy: From dashboard to app users.</summary>
public interface IPushNotificationService
{
    PushNotification Create(string appProjectId, string title, string body, string? imageUrl, DateTime? scheduledAt, string? userId);
    PushNotification? GetById(string id);
    IReadOnlyList<PushNotification> GetByProject(string appProjectId);
    IReadOnlyList<PushNotification> GetAll();
    void MarkSent(string id, int? deliveredCount = null);
    void MarkFailed(string id);
}
