using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class MultiFactorAuthDto
{
    public MultiFactorAuthDto(Guid id, Guid userId, IEnumerable<string> enabledMethods, int requiredMethods, string status, DateTime createdAt, DateTime? activatedAt)
    {
        Id = id;
        UserId = userId;
        EnabledMethods = enabledMethods;
        RequiredMethods = requiredMethods;
        Status = status;
        CreatedAt = createdAt;
        ActivatedAt = activatedAt;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable<string> EnabledMethods { get; set; }
    public int RequiredMethods { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
}

