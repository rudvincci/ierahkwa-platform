using Mamey.Templates.Registries;
using Mamey.Types;

namespace Mamey.Templates.Authorization;

/// <summary>Simple ABAC rule; version=null applies at family-level.</summary>
public class TemplatePolicy : IAuditable<UserId>
{
    
    public string TemplateId { get; set; } = default!;
    public int? Version { get; set; }

    public string Effect { get; set; } = "allow"; // allow|deny
    public string PrincipalType { get; set; } = "service"; // service|tenant|role|claim
    public string Principal { get; set; } = default!;      // service name / tenant / role / claim-type
    public string? ClaimValue { get; set; }                // used when PrincipalType == claim
    
    public virtual DocumentTemplate DocumentTemplate { get; set; }
    
    private TemplatePolicy() { }

    public TemplatePolicy(string templateId, int? version, string effect, string principalType, string principal,
        string? claimValue)
    {
        
    }

    public UserId CreatedBy { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    public static TemplatePolicy Insert(string templateId, int? version, string effect, string principalType,
        string principal, string? claimValue, UserId createdBy)
    {
        var policyRow = new TemplatePolicy(templateId, version, effect, principalType, principal, claimValue);
        policyRow.SetAudit(createdBy);
        return policyRow;
    }
    
    public void Touch(UserId modifier)
    {
        ModifiedBy = modifier ?? throw new ArgumentNullException(nameof(modifier));
        ModifiedAt = DateTime.UtcNow;
    }

    public void SetAudit(UserId createdBy)
    {
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedAt = DateTime.UtcNow;
    }
}