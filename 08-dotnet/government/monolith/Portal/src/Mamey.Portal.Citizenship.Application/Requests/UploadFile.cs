namespace Mamey.Portal.Citizenship.Application.Requests;

public sealed record UploadFile(
    string FileName,
    string ContentType,
    long Size,
    Stream Content);




