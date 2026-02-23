using System;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Commands;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.RequestResponses;
using Mamey.Government.Modules.CitizenshipApplications.Core.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CitizenshipApplications.Api.Controllers;

[ApiController]
[Route("api/citizenship-applications")]
public class ApplicationsController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICitizenApplicationsService _citizenApplicationsService;

    public ApplicationsController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher, ICitizenApplicationsService citizenApplicationsService)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _citizenApplicationsService = citizenApplicationsService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApplicationDto>> Get(Guid id)
    {
        var application = await _queryDispatcher.QueryAsync(new GetApplication(id));
        if (application is null)
        {
            return NotFound();
        }
        return Ok(application);
    }

    [HttpGet("by-number/{applicationNumber}")]
    [ProducesResponseType(typeof(ApplicationDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ApplicationDto>> GetByNumber(string applicationNumber)
    {
        var application = await _queryDispatcher.QueryAsync(new GetApplicationByNumber(applicationNumber));
        if (application is null)
        {
            return NotFound();
        }
        return Ok(application);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(typeof(PagedResult<ApplicationSummaryDto>), 200)]
    public async Task<ActionResult<PagedResult<ApplicationSummaryDto>>> Browse(
        [FromQuery] Guid tenantId,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _queryDispatcher.QueryAsync(new BrowseApplications(
            tenantId, status, searchTerm, fromDate, toDate, page, pageSize));
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
    {
        var applicationId = Guid.NewGuid();
        await _commandDispatcher.SendAsync(
            new CreateApplication(
                applicationId,
                request.TenantId,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Email,
                request.Phone,
                request.Street,
                request.City,
                request.State,
                request.PostalCode,
                request.Country));
        
        return CreatedAtAction(nameof(Get), new { id = applicationId }, 
            new { ApplicationId = applicationId });
    }

    [HttpPost("start")]
    [ProducesResponseType(202)]
    public async Task<IActionResult> Start([FromBody] StartApplicationRequest request)
    {
        await _commandDispatcher.SendAsync(new StartApplication(request.Email));
        return Accepted();
    }

    [HttpPost("resume")]
    [ProducesResponseType(typeof(ResumeApplicationResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Resume([FromBody] ResumeApplicationRequest request)
    {
        try
        {

            var response = await _citizenApplicationsService.ResumeApplicationAsync(new ResumeApplication(
                request.Token,
                request.Email));
            if (response is null)
            {
                return BadRequest(new { error = "Failed to resume application." });
            }
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while resuming the application." });
        }
    }

    [HttpPost("{id:guid}/submit")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Submit(Guid id)
    {
        await _commandDispatcher.SendAsync(new SubmitApplication(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/documents")]
    [ProducesResponseType(201)]
    public async Task<IActionResult> UploadDocument(Guid id, [FromForm] UploadDocumentRequest request)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest("File is required");
        }

        await using var stream = request.File.OpenReadStream();
        await _commandDispatcher.SendAsync(
            new UploadDocument(
                id,
                request.DocumentType,
                request.File.FileName,
                request.File.ContentType,
                stream));
        
        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Approve(Guid id)
    {
        await _commandDispatcher.SendAsync(new ApproveApplication(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin,GovernmentAgent")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectApplicationRequest request)
    {
        await _commandDispatcher.SendAsync(new RejectApplication(id, request.Reason));
        return NoContent();
    }
}

public record CreateApplicationRequest(
    Guid TenantId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? Email,
    string? Phone,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country);

