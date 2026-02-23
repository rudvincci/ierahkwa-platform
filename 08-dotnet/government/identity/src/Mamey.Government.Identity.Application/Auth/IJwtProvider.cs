using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Auth;

internal interface IJwtProvider
{
    AuthDto Create(Guid userId, string name, string email, string role, UserType type, UserStatus status, Constants.User.Permission permissions, string? audience = null,
        IDictionary<string, string>? claims = null);
}