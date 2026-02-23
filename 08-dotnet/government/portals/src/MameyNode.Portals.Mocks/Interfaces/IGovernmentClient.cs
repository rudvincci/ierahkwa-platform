using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IGovernmentClient
{
    Task<List<IdentityInfo>> GetIdentitiesAsync();
    Task<List<DIDInfo>> GetDIDsAsync();
    Task<List<DocumentInfo>> GetDocumentsAsync();
    Task<List<VoteInfo>> GetVotesAsync();
    Task<List<ComplianceCheckInfo>> GetComplianceChecksAsync();
    Task<List<CitizenshipApplicationInfo>> GetCitizenshipApplicationsAsync();
    Task<List<CitizenInfo>> GetCitizensAsync();
    Task<List<PassportInfo>> GetPassportsAsync();
    Task<List<TravelIdentityInfo>> GetTravelIdentitiesAsync();
    Task<List<DiplomatInfo>> GetDiplomatsAsync();
    Task<List<PaymentPlanInfo>> GetPaymentPlansAsync();
    Task<List<AMLFlagInfo>> GetAMLFlagsAsync();
    Task<List<KYCStatusInfo>> GetKYCStatusesAsync();
    Task<List<FraudReportInfo>> GetFraudReportsAsync();
    Task<List<AuditEntryInfo>> GetAuditEntriesAsync();
    Task<List<RedFlagInfo>> GetRedFlagsAsync();
    Task<List<RoleInfo>> GetRolesAsync();
    Task<List<PermissionInfo>> GetPermissionsAsync();
    Task<List<RoleHierarchyInfo>> GetRoleHierarchiesAsync();
}

