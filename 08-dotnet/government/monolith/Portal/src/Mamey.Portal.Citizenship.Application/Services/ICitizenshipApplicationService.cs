using Mamey.Portal.Citizenship.Application.Requests;

namespace Mamey.Portal.Citizenship.Application.Services;

public interface ICitizenshipApplicationService
{
    Task<string> SubmitAsync(
        SubmitCitizenshipApplicationRequest request,
        IReadOnlyList<UploadFile> personalDocuments,
        UploadFile passportPhoto,
        UploadFile signatureImage,
        CancellationToken ct = default);
}




