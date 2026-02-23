namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record CitizenUploadDto(
    Guid UploadId,
    string Kind,
    string FileName,
    string ContentType,
    long Size,
    DateTimeOffset UploadedAt);




