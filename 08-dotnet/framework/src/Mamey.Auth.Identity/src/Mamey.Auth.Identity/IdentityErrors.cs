using Microsoft.AspNetCore.Identity;

namespace Mamey.Auth.Identity;

public static class IdentityErrors
{
    public static IdentityResult FromException(Exception ex)
        => IdentityResult.Failed(new IdentityError { Description = ex.Message });
}