using Mamey.Templates.Authorization;
using Mamey.Types;

namespace Mamey.Templates.Registries;

public class DocumentTemplate : AggregateRoot<TemplateId>, IAuditable<UserId>
{
    private readonly string? _agencyCode;
    private DocumentTemplate() {}

    public DocumentTemplate(TemplateId id, DocumentTemplateStatus status, string storageRef, string contentType, string sha256,
        int size, DateTimeOffset effectiveDate, string? classification = null, string? agencyCode = null,
        DateTimeOffset? effectiveTo = null, int version = 0)
        :base(id, version)
    {
        _agencyCode = agencyCode;
        Status = status;
        StorageRef = storageRef;
        ContentType = contentType;
        Sha256 = sha256;
        Size = size;
        EffectiveDate = effectiveDate;
        Classification = classification;
        EffectiveTo = effectiveTo;
    }

    public DocumentTemplateStatus Status { get; set; } = DocumentTemplateStatus.Draft;

    /// <summary>Provider-specific storage reference (e.g., GridFS ObjectId string).</summary>
    public string StorageRef { get; set; } = default!;

    public string ContentType { get; set; } = "application/octet-stream"; // application/vnd.openxmlformats-officedocument
    public string Sha256 { get; set; } = default!;
    public int Size { get; set; }
    public DateTimeOffset EffectiveDate { get; }

    // optional governance fields
    public string? Classification { get; set; }
    public string? AgencyCode { get; set; }

    public DateTimeOffset EffectiveFrom { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EffectiveTo { get; set; }
    public UserId CreatedBy { get; private set; }
    public UserId? ModifiedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }
    
    public virtual ICollection<TemplatePolicy> TemplatePolicies { get; private set; } = new List<TemplatePolicy>();
    
    
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