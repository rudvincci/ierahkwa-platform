using Microsoft.AspNetCore.Http;

namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.RequestResponses;


public record UploadDocumentRequest(
    string DocumentType,
    IFormFile? File);

public record RejectApplicationRequest(
    string Reason);

public record StartApplicationRequest(
    string Email,
    string? FirstName,
    string? LastName);

public record ResumeApplicationRequest(
    string Token,
    string Email);