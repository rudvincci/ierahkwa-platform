using System;

namespace Mamey.ApplicationName.Bootstrapper;

public static class FhgRole
{
    // Admin Role
    public const FHGPermission Admin =
        FHGPermission.All;
    

    // Customer Service Representative Role
    public const FHGPermission User =
        FHGPermission.None;
}
[Flags]
public enum FHGPermission : long
{
    None = 0L,

    // Account Management
    ViewAllAccounts = 1L << 0,
    ViewIndividualAccounts = 1L << 1,
    All = 1L << 999
}