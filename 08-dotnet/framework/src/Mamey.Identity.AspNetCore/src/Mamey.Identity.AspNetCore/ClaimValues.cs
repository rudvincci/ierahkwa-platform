namespace Mamey.Identity.AspNetCore;

public static class ClaimValues
{
    public static class Permission
    {
        public const string IdentityRead     = "Identity.Read";
        public const string IdentityWrite    = "Identity.Write";
        public const string IdentityAdmin    = "Identity.Admin";
        public const string SystemAll        = "System.All";
        public const string DaemonExecute    = "Daemon.Execute";
    }

    public static class Role
    {
        public const string Admin      = "Admin";
        public const string TenantAdmin      = "TenantAdmin";
        public const string Support          = "Support";
        public const string User             = "User";
        public const string Daemon           = "Daemon";
    }

    public static class Feature
    {
        public const string BetaFeatureX         = "BetaFeatureX";
        public const string NewDashboardAccess   = "NewDashboardAccess";
        public const string ReportsPreview       = "ReportsPreview";
    }

    public static class Scope
    {
        public const string OpenId           = "openid";
        public const string Profile          = "profile";
        public const string Email            = "email";
        public const string OfflineAccess    = "offline_access";
    }

    public static class Department
    {
        public const string Sales            = "Sales";
        public const string Engineering      = "Engineering";
        public const string HR               = "HR";
        public const string Finance          = "Finance";
    }

    public static class System
    {
        public const string MaintenanceMode  = "SystemMaintenance";
        public const string ReadOnlyMode     = "ReadOnlyMode";
    }

    public static class Preference
    {
        public const string ThemeDark        = "Theme.Dark";
        public const string ThemeLight       = "Theme.Light";
        public const string ItemsPerPage20   = "ItemsPerPage=20";
        public const string ItemsPerPage50   = "ItemsPerPage=50";
    }

    public static class Group
    {
        public const string ProjectXTeam     = "ProjectXTeam";
        public const string AuditCommittee   = "AuditCommittee";
        public const string Developers       = "Developers";
    }

    public static class Policy
    {
        public const string DataExportAllowed   = "DataExportAllowed";
        public const string AccountLockPolicy   = "AccountLockPolicy";
        public const string PasswordComplexity  = "PasswordComplexity";
    }

    public static class Resource
    {
        public const string CustomerCreate   = "Customer.Create";
        public const string CustomerRead     = "Customer.Read";
        public const string CustomerUpdate   = "Customer.Update";
        public const string CustomerDelete   = "Customer.Delete";
        public const string InvoiceCreate    = "Invoice.Create";
    }

    public static class Environment
    {
        public const string Production       = "Environment=Production";
        public const string Staging          = "Environment=Staging";
        public const string Development      = "Environment=Development";
    }

    public static class Authentication
    {
        public const string Password         = "AuthMethod=Password";
        public const string TwoFactor        = "AuthMethod=2FA";
        public const string OAuth            = "AuthMethod=OAuth";
    }

    public static class IdentityProvider
    {
        public const string AzureAD          = "IdP=AzureAD";
        public const string Google           = "IdP=Google";
        public const string Facebook         = "IdP=Facebook";
    }

    public static class Locale
    {
        public const string EnUS             = "Locale=en-US";
        public const string EsPR             = "Locale=es-PR";
        public const string FrCA             = "Locale=fr-CA";
    }

    public static class Time
    {
        public const string IssuedAt         = "IssuedAt";
        public const string ExpiresAt        = "ExpiresAt";
    }
}
