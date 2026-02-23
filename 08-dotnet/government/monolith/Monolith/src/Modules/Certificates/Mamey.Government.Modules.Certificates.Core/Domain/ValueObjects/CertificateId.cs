using Mamey.Types;

namespace Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;

public class CertificateId : TypeId
{
    public CertificateId(Guid value) : base(value)
    {
    }

    public static implicit operator CertificateId(Guid id) => new(id);
    public static implicit operator Guid(CertificateId id) => id.Value;
}
