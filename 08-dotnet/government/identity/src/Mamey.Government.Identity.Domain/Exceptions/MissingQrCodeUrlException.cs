using Mamey.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Exceptions;

internal class MissingQrCodeUrlException : DomainException
{
    public override string Code { get; } = "missing_qr_code_url";

    public MissingQrCodeUrlException() : base("QR code URL is missing.")
    {
    }
}
