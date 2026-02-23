namespace Mamey.Portal.Shared.Storage;

public static class ObjectStorageKeys
{
    public static string TenantBucket(string tenantId) => $"tenant-{tenantId}".ToLowerInvariant();
}




