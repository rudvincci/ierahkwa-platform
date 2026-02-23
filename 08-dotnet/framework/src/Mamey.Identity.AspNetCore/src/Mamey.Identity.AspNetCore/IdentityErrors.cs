using Microsoft.AspNetCore.Identity;

namespace Mamey.Identity.AspNetCore;

public static class IdentityErrors
{
    public static IdentityResult FromException(Exception ex)
        => IdentityResult.Failed(new IdentityError { Description = ex.Message });
}

