using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.DTO;

namespace Mamey.Government.Modules.Certificates.Core.Queries.Handlers;

internal sealed class ValidateCertificateHandler : IQueryHandler<ValidateCertificate, CertificateValidationResultDto?>
{
    private readonly ICertificateRepository _repository;

    public ValidateCertificateHandler(ICertificateRepository repository)
    {
        _repository = repository;
    }

    public async Task<CertificateValidationResultDto?> HandleAsync(ValidateCertificate query, CancellationToken cancellationToken = default)
    {
        var certificate = await _repository.GetByCertificateNumberAsync(query.CertificateNumber, cancellationToken);
        
        if (certificate is null)
        {
            return new CertificateValidationResultDto
            {
                IsValid = false,
                CertificateNumber = query.CertificateNumber,
                CertificateType = "Unknown",
                Status = "NotFound",
                VerifiedAt = DateTime.UtcNow,
                Message = "Certificate not found in the system"
            };
        }

        return new CertificateValidationResultDto
        {
            IsValid = certificate.IsActive,
            CertificateNumber = certificate.CertificateNumber,
            CertificateType = certificate.CertificateType.ToString(),
            Status = certificate.IsActive ? "Active" : "Revoked",
            IssueDate = certificate.IssuedDate,
            VerifiedAt = DateTime.UtcNow,
            Message = certificate.IsActive ? "Certificate is valid" : $"Certificate has been revoked: {certificate.RevocationReason}"
        };
    }
}
