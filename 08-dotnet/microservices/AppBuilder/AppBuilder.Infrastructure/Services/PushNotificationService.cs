using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;

namespace AppBuilder.Infrastructure.Services;

/// <summary>Push notifications – send, schedule, images, status. IERAHKWA Appy.</summary>
public class PushNotificationService : IPushNotificationService
{
    private static readonly List<PushNotification> _list = new();
    private static readonly object _lock = new();

    public PushNotification Create(string appProjectId, string title, string body, string? imageUrl, DateTime? scheduledAt, string? userId)
    {
        lock (_lock)
        {
            var n = new PushNotification
            {
                Id = Guid.NewGuid().ToString(),
                AppProjectId = appProjectId,
                Title = title,
                Body = body,
                ImageUrl = imageUrl,
                ScheduledAt = scheduledAt,
                Status = scheduledAt.HasValue ? PushDeliveryStatus.Scheduled : PushDeliveryStatus.Pending,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _list.Add(n);
            return n;
        }
    }

    public PushNotification? GetById(string id)
    {
        lock (_lock) return _list.FirstOrDefault(x => x.Id == id);
    }

    public IReadOnlyList<PushNotification> GetByProject(string appProjectId)
    {
        lock (_lock) return _list.Where(x => x.AppProjectId == appProjectId).OrderByDescending(x => x.CreatedAt).ToList();
    }

    public IReadOnlyList<PushNotification> GetAll()
    {
        lock (_lock) return _list.OrderByDescending(x => x.CreatedAt).ToList();
    }

    public void MarkSent(string id, int? deliveredCount = null)
    {
        lock (_lock)
        {
            var n = _list.FirstOrDefault(x => x.Id == id);
            if (n != null) { n.Status = PushDeliveryStatus.Sent; n.SentAt = DateTime.UtcNow; n.DeliveredCount = deliveredCount; }
        }
    }

    public void MarkFailed(string id)
    {
        lock (_lock)
        {
            var n = _list.FirstOrDefault(x => x.Id == id);
            if (n != null) n.Status = PushDeliveryStatus.Failed;
        }
    }
}
