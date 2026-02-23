using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Identity.Core.Queries;

internal record GenerateAuthenticatorQrCode(Guid UserId) : IQuery<byte[]?>;