using System.Text.RegularExpressions;

namespace Mamey.Azure.Blobs;

public class ResourceNameValidator
{
    // Container name validation rules
    public bool IsValidContainerName(string containerName)
    {
        if (string.IsNullOrEmpty(containerName)) return false;
        if (containerName.Length < 3 || containerName.Length > 63) return false;

        // Corrected regex to enforce the rule that the name must start with a lowercase letter
        if (!Regex.IsMatch(containerName, "^[a-z][a-z0-9-]*[a-z0-9]$")) return false;

        // Ensure no consecutive hyphens
        if (containerName.Contains("--")) return false;

        return true;
    }

    // Directory name validation rules
    public bool IsValidDirectoryName(string directoryName)
    {
        if (string.IsNullOrEmpty(directoryName)) return false;
        if (directoryName.Length > 1024) return false;
        if (directoryName.EndsWith(".") || directoryName.EndsWith("/") || directoryName.EndsWith("\\")) return false;

        return true;
    }

    // Blob name validation rules
    public bool IsValidBlobName(string blobName)
    {
        if (string.IsNullOrEmpty(blobName)) return false;
        if (blobName.Length > 1024) return false;
        if (blobName.Contains("\uE000") || Regex.IsMatch(blobName, @"[\u0000-\u001F\u0081]")) return false;
        if (blobName.EndsWith(".") || blobName.EndsWith("/") || blobName.EndsWith("\\")) return false;

        return true;
    }
}