using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.Commands;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Queries;
using Mamey.MicroMonolith.Abstractions;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.Government.Modules.Certificates.Api.Controllers;

[Authorize]
internal class CertificatesController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IContext _context;

    public CertificatesController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        IContext context)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _context = context;
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation("Get certificate by ID")]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> GetAsync(Guid id)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetCertificate { CertificateId = id }));

    [HttpGet("by-number/{certificateNumber}")]
    [SwaggerOperation("Get certificate by certificate number")]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CertificateDto>> GetByNumberAsync(string certificateNumber)
        => OkOrNotFound(await _queryDispatcher.QueryAsync(new GetCertificateByNumber { CertificateNumber = certificateNumber }));

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Browse certificates")]
    [ProducesResponseType(typeof(PagedResult<CertificateSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CertificateSummaryDto>>> BrowseAsync(
        [FromQuery] Guid tenantId,
        [FromQuery] string? certificateType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new BrowseCertificates
        {
            TenantId = tenantId,
            CertificateType = certificateType,
            Status = status,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };
        return Ok(await _queryDispatcher.QueryAsync(query));
    }

    [HttpGet("by-citizen/{citizenId:guid}")]
    [SwaggerOperation("Get certificates for a citizen")]
    [ProducesResponseType(typeof(IEnumerable<CertificateSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CertificateSummaryDto>>> GetByCitizenAsync(Guid citizenId)
    {
        var certificates = await _queryDispatcher.QueryAsync(new GetCertificatesByCitizen { CitizenId = citizenId });
        return Ok(certificates);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Issue new certificate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IssueAsync([FromBody] IssueCertificateRequest request)
    {
        if (!Enum.TryParse<CertificateType>(request.CertificateType, true, out var certType))
        {
            return BadRequest($"Invalid certificate type: {request.CertificateType}");
        }

        var command = new IssueCertificate(
            request.TenantId,
            request.CitizenId,
            certType,
            request.SubjectName,
            request.Notes);

        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { CertificateId = command.Id });
    }

    [HttpPost("{id:guid}/revoke")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Revoke certificate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeAsync(Guid id, [FromBody] RevokeCertificateRequest request)
    {
        var revokedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new RevokeCertificate(id, request.Reason, revokedBy));
        return NoContent();
    }

    [HttpPost("{id:guid}/copy")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [SwaggerOperation("Issue certificate copy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> IssueCopyAsync(Guid id, [FromBody] IssueCertificateCopyRequest? request = null)
    {
        var requestedBy = _context.Identity?.Id.ToString() ?? "system";
        var command = new IssueCertificateCopy(id, requestedBy, request?.Notes);
        await _commandDispatcher.SendAsync(command);
        return CreatedAtAction(nameof(GetAsync), new { id = command.Id }, new { CertificateId = command.Id });
    }

    [HttpPost("{id:guid}/archive")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation("Archive certificate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveAsync(Guid id)
    {
        var archivedBy = _context.Identity?.Id.ToString() ?? "system";
        await _commandDispatcher.SendAsync(new ArchiveCertificate(id, archivedBy));
        return NoContent();
    }

    [HttpPost("validate")]
    [AllowAnonymous]
    [SwaggerOperation("Publicly validate a certificate")]
    [ProducesResponseType(typeof(CertificateValidationResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CertificateValidationResultDto>> ValidateAsync([FromBody] ValidateCertificateRequest request)
    {
        var result = await _queryDispatcher.QueryAsync(new ValidateCertificate 
        { 
            CertificateNumber = request.CertificateNumber
        });
        return Ok(result);
    }
}

// Request DTOs
public record IssueCertificateRequest(
    Guid TenantId,
    Guid? CitizenId,
    string CertificateType,
    string? SubjectName = null,
    string? Notes = null);

public record RevokeCertificateRequest(string Reason);

public record IssueCertificateCopyRequest(string? Notes = null);

public record ValidateCertificateRequest(string CertificateNumber);
