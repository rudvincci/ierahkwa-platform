namespace Mamey.Portal.Library.Application.Requests;

public sealed record LibraryUploadFile(
    string FileName,
    string ContentType,
    long Size,
    Stream Content);




