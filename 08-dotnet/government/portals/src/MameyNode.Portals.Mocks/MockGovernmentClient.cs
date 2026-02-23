using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockGovernmentClient : IGovernmentClient
{
    private readonly Faker _faker = new();
    private readonly List<IdentityInfo> _identities = new();
    private readonly List<DIDInfo> _dids = new();
    private readonly List<DocumentInfo> _documents = new();
    private readonly List<VoteInfo> _votes = new();
    private readonly List<ComplianceCheckInfo> _complianceChecks = new();
    private readonly List<CitizenshipApplicationInfo> _citizenshipApplications = new();
    private readonly List<CitizenInfo> _citizens = new();
    private readonly List<PassportInfo> _passports = new();
    private readonly List<TravelIdentityInfo> _travelIdentities = new();
    private readonly List<DiplomatInfo> _diplomats = new();
    private readonly List<PaymentPlanInfo> _paymentPlans = new();
    private readonly List<AMLFlagInfo> _amlFlags = new();
    private readonly List<KYCStatusInfo> _kycStatuses = new();
    private readonly List<FraudReportInfo> _fraudReports = new();
    private readonly List<AuditEntryInfo> _auditEntries = new();
    private readonly List<RedFlagInfo> _redFlags = new();
    private readonly List<RoleInfo> _roles = new();
    private readonly List<PermissionInfo> _permissions = new();
    private readonly List<RoleHierarchyInfo> _roleHierarchies = new();

    public MockGovernmentClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var identityFaker = new Faker<IdentityInfo>()
            .RuleFor(i => i.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(i => i.CitizenId, f => $"CIT-{f.Random.AlphaNumeric(8)}")
            .RuleFor(i => i.FirstName, f => f.Person.FirstName)
            .RuleFor(i => i.LastName, f => f.Person.LastName)
            .RuleFor(i => i.DateOfBirth, f => f.Person.DateOfBirth.ToString("yyyy-MM-dd"))
            .RuleFor(i => i.Nationality, f => f.Address.Country())
            .RuleFor(i => i.Status, f => f.PickRandom<IdentityStatus>())
            .RuleFor(i => i.BlockchainAccount, f => $"0x{f.Random.AlphaNumeric(40)}")
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(2))
            .RuleFor(i => i.VerifiedAt, (f, i) => i.Status == IdentityStatus.Verified ? f.Date.Between(i.CreatedAt, DateTime.Now) : null);

        _identities.AddRange(identityFaker.Generate(200));

        var didFaker = new Faker<DIDInfo>()
            .RuleFor(d => d.DID, f => $"did:futurewampum:{f.Random.AlphaNumeric(16)}")
            .RuleFor(d => d.Status, f => f.PickRandom<DIDStatus>())
            .RuleFor(d => d.Created, f => f.Date.Past(1))
            .RuleFor(d => d.Updated, f => f.Date.Recent(30))
            .RuleFor(d => d.Version, f => (ulong)f.Random.Long(1, 10))
            .RuleFor(d => d.Controller, f => $"ACC-{f.Random.AlphaNumeric(8)}");

        _dids.AddRange(didFaker.Generate(150));

        var documentFaker = new Faker<DocumentInfo>()
            .RuleFor(d => d.DocumentId, f => $"DOC-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(d => d.IdentityId, f => _identities[f.Random.Int(0, _identities.Count - 1)].IdentityId)
            .RuleFor(d => d.DocumentType, f => f.PickRandom("Passport", "ID", "BirthCertificate", "DriverLicense"))
            .RuleFor(d => d.DocumentNumber, f => f.Random.AlphaNumeric(12).ToUpper())
            .RuleFor(d => d.Status, f => f.PickRandom<DocumentStatus>())
            .RuleFor(d => d.IssuedAt, f => f.Date.Past(2))
            .RuleFor(d => d.ExpiryDate, (f, d) => f.Date.Between(d.IssuedAt, d.IssuedAt.AddYears(10)))
            .RuleFor(d => d.IssuingAuthority, f => f.Company.CompanyName());

        _documents.AddRange(documentFaker.Generate(250));

        var voteFaker = new Faker<VoteInfo>()
            .RuleFor(v => v.VoteId, f => $"VOTE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(v => v.Title, f => f.Lorem.Sentence())
            .RuleFor(v => v.Description, f => f.Lorem.Paragraph())
            .RuleFor(v => v.Status, f => f.PickRandom<VoteStatus>())
            .RuleFor(v => v.StartDate, f => f.Date.Past(30))
            .RuleFor(v => v.EndDate, (f, v) => f.Date.Between(v.StartDate, v.StartDate.AddDays(30)))
            .RuleFor(v => v.TotalVotes, f => f.Random.Int(0, 10000))
            .RuleFor(v => v.OptionVotes, f => new Dictionary<string, int> { { "Option A", f.Random.Int(0, 5000) }, { "Option B", f.Random.Int(0, 5000) } });

        _votes.AddRange(voteFaker.Generate(20));

        // Add remaining mock data generators...
        var complianceFaker = new Faker<ComplianceCheckInfo>()
            .RuleFor(c => c.CheckId, f => $"CHECK-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.EntityId, f => _identities[f.Random.Int(0, _identities.Count - 1)].IdentityId)
            .RuleFor(c => c.EntityType, f => f.PickRandom("Identity", "Document", "Transaction"))
            .RuleFor(c => c.CheckType, f => f.PickRandom("AML", "KYC", "Sanctions", "PEP"))
            .RuleFor(c => c.Status, f => f.PickRandom<ComplianceStatus>())
            .RuleFor(c => c.Passed, f => f.Random.Bool(0.9f))
            .RuleFor(c => c.Result, f => f.Lorem.Sentence())
            .RuleFor(c => c.CheckedAt, f => f.Date.Recent(60))
            .RuleFor(c => c.CheckedBy, f => $"AGENT-{f.Random.AlphaNumeric(6)}");

        _complianceChecks.AddRange(complianceFaker.Generate(300));

        var applicationFaker = new Faker<CitizenshipApplicationInfo>()
            .RuleFor(a => a.ApplicationId, f => $"APP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(a => a.ApplicantId, f => $"APPL-{f.Random.AlphaNumeric(8)}")
            .RuleFor(a => a.Status, f => f.PickRandom("Pending", "UnderReview", "Approved", "Rejected"))
            .RuleFor(a => a.SubmittedAt, f => f.Date.Recent(90))
            .RuleFor(a => a.ReviewedAt, (f, a) => a.Status != "Pending" ? f.Date.Between(a.SubmittedAt, DateTime.Now) : null)
            .RuleFor(a => a.ApprovedAt, (f, a) => a.Status == "Approved" ? f.Date.Between(a.SubmittedAt, DateTime.Now) : null)
            .RuleFor(a => a.ReviewerId, (f, a) => a.Status != "Pending" ? $"REV-{f.Random.AlphaNumeric(6)}" : string.Empty)
            .RuleFor(a => a.Notes, f => f.Lorem.Sentence());

        _citizenshipApplications.AddRange(applicationFaker.Generate(100));

        var citizenFaker = new Faker<CitizenInfo>()
            .RuleFor(c => c.CitizenId, f => $"CIT-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(c => c.FirstName, f => f.Person.FirstName)
            .RuleFor(c => c.LastName, f => f.Person.LastName)
            .RuleFor(c => c.Email, f => f.Person.Email)
            .RuleFor(c => c.DateOfBirth, f => f.Person.DateOfBirth.ToString("yyyy-MM-dd"))
            .RuleFor(c => c.Status, f => f.PickRandom("Active", "Suspended", "Deceased"))
            .RuleFor(c => c.RegisteredAt, f => f.Date.Past(5))
            .RuleFor(c => c.IdentityId, f => _identities[f.Random.Int(0, _identities.Count - 1)].IdentityId);

        _citizens.AddRange(citizenFaker.Generate(500));

        var passportFaker = new Faker<PassportInfo>()
            .RuleFor(p => p.PassportId, f => $"PASS-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.CitizenId, f => _citizens[f.Random.Int(0, _citizens.Count - 1)].CitizenId)
            .RuleFor(p => p.PassportNumber, f => f.Random.AlphaNumeric(9).ToUpper())
            .RuleFor(p => p.IssuedAt, f => f.Date.Past(2))
            .RuleFor(p => p.ExpiryDate, (f, p) => f.Date.Between(p.IssuedAt, p.IssuedAt.AddYears(10)))
            .RuleFor(p => p.IssuingCountry, f => f.Address.Country())
            .RuleFor(p => p.Status, f => f.PickRandom("Active", "Expired", "Revoked"));

        _passports.AddRange(passportFaker.Generate(400));

        // Add remaining generators for travel identities, diplomats, payment plans, AML flags, KYC, fraud reports, audit entries, red flags, roles, permissions, role hierarchies...
        var travelFaker = new Faker<TravelIdentityInfo>()
            .RuleFor(t => t.TravelId, f => $"TRAVEL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(t => t.CitizenId, f => _citizens[f.Random.Int(0, _citizens.Count - 1)].CitizenId)
            .RuleFor(t => t.IdentityType, f => f.PickRandom("Visa", "TravelPermit", "Diplomatic"))
            .RuleFor(t => t.Status, "Active")
            .RuleFor(t => t.IssuedAt, f => f.Date.Past(1))
            .RuleFor(t => t.ExpiryDate, (f, t) => f.Date.Between(t.IssuedAt, t.IssuedAt.AddYears(5)))
            .RuleFor(t => t.AuthorizedCountries, f => f.Lorem.Words(f.Random.Int(1, 10)).ToList());

        _travelIdentities.AddRange(travelFaker.Generate(150));

        var diplomatFaker = new Faker<DiplomatInfo>()
            .RuleFor(d => d.DiplomatId, f => $"DIPL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(d => d.CitizenId, f => _citizens[f.Random.Int(0, _citizens.Count - 1)].CitizenId)
            .RuleFor(d => d.DiplomaticRank, f => f.PickRandom("Ambassador", "Consul", "Attache", "Secretary"))
            .RuleFor(d => d.AssignmentCountry, f => f.Address.Country())
            .RuleFor(d => d.AssignmentStart, f => f.Date.Past(2))
            .RuleFor(d => d.AssignmentEnd, (f, d) => f.Date.Between(d.AssignmentStart, d.AssignmentStart.AddYears(4)))
            .RuleFor(d => d.Status, f => f.PickRandom("Active", "Completed", "Terminated"));

        _diplomats.AddRange(diplomatFaker.Generate(50));

        var planFaker = new Faker<PaymentPlanInfo>()
            .RuleFor(p => p.PlanId, f => $"PLAN-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.CitizenId, f => _citizens[f.Random.Int(0, _citizens.Count - 1)].CitizenId)
            .RuleFor(p => p.PlanType, f => f.PickRandom("Tax", "Fine", "Fee", "Installment"))
            .RuleFor(p => p.Amount, f => f.Finance.Amount(100, 10000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.Frequency, f => f.PickRandom("Monthly", "Quarterly", "Annually"))
            .RuleFor(p => p.StartDate, f => f.Date.Recent(60))
            .RuleFor(p => p.EndDate, (f, p) => f.Date.Between(p.StartDate, p.StartDate.AddYears(2)))
            .RuleFor(p => p.Status, f => f.PickRandom("Active", "Completed", "Cancelled"));

        _paymentPlans.AddRange(planFaker.Generate(200));

        var amlFaker = new Faker<AMLFlagInfo>()
            .RuleFor(a => a.FlagId, f => $"FLAG-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(a => a.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(a => a.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(a => a.Reason, f => f.Lorem.Sentence())
            .RuleFor(a => a.RiskLevel, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(a => a.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(a => a.Status, f => f.PickRandom<FlagStatus>());

        _amlFlags.AddRange(amlFaker.Generate(75));

        var kycFaker = new Faker<KYCStatusInfo>()
            .RuleFor(k => k.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(k => k.KYCLevel, f => f.PickRandom("basic", "standard", "enhanced"))
            .RuleFor(k => k.Status, f => f.PickRandom("Pending", "Verified", "Rejected"))
            .RuleFor(k => k.VerifiedAt, f => f.Date.Recent(180))
            .RuleFor(k => k.ExpiresAt, (f, k) => f.Date.Between(k.VerifiedAt, k.VerifiedAt.AddYears(2)))
            .RuleFor(k => k.VerifiedAttributes, f => f.Lorem.Words(f.Random.Int(1, 5)).ToList());

        _kycStatuses.AddRange(kycFaker.Generate(300));

        var fraudFaker = new Faker<FraudReportInfo>()
            .RuleFor(f => f.FraudReportId, f => $"FRAUD-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(f => f.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(f => f.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(f => f.FraudType, f => f.PickRandom("IdentityTheft", "CardFraud", "AccountTakeover", "Phishing"))
            .RuleFor(f => f.Description, f => f.Lorem.Paragraph())
            .RuleFor(f => f.ReportedAt, f => f.Date.Recent(30))
            .RuleFor(f => f.Status, f => f.PickRandom<FraudReportStatus>());

        _fraudReports.AddRange(fraudFaker.Generate(40));

        var auditFaker = new Faker<AuditEntryInfo>()
            .RuleFor(a => a.AuditEntryId, f => $"AUDIT-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(a => a.EntityType, f => f.PickRandom("Identity", "Document", "Transaction", "Account"))
            .RuleFor(a => a.EntityId, f => $"ENT-{f.Random.AlphaNumeric(8)}")
            .RuleFor(a => a.Action, f => f.PickRandom("Create", "Update", "Delete", "View", "Approve", "Reject"))
            .RuleFor(a => a.Actor, f => $"USER-{f.Random.AlphaNumeric(6)}")
            .RuleFor(a => a.Timestamp, f => f.Date.Recent(90))
            .RuleFor(a => a.Details, f => new Dictionary<string, string> { { "ip", f.Internet.Ip() }, { "userAgent", f.Internet.UserAgent() } });

        _auditEntries.AddRange(auditFaker.Generate(1000));

        var redFlagFaker = new Faker<RedFlagInfo>()
            .RuleFor(r => r.RedFlagId, f => $"RED-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.FlagType, f => f.PickRandom("SuspiciousActivity", "ComplianceViolation", "RiskIndicator"))
            .RuleFor(r => r.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(r => r.Description, f => f.Lorem.Sentence())
            .RuleFor(r => r.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(r => r.Status, f => f.PickRandom<RedFlagStatus>());

        _redFlags.AddRange(redFlagFaker.Generate(60));

        var roleFaker = new Faker<RoleInfo>()
            .RuleFor(r => r.RoleId, f => $"ROLE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.RoleName, f => f.PickRandom("Admin", "Agent", "Citizen", "Auditor", "ComplianceOfficer"))
            .RuleFor(r => r.Description, f => f.Lorem.Sentence())
            .RuleFor(r => r.Permissions, f => f.Lorem.Words(f.Random.Int(3, 10)).ToList())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past(1))
            .RuleFor(r => r.IsActive, true);

        _roles.AddRange(roleFaker.Generate(10));

        var permissionFaker = new Faker<PermissionInfo>()
            .RuleFor(p => p.PermissionId, f => $"PERM-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.PermissionName, f => f.PickRandom("Read", "Write", "Delete", "Approve", "Reject"))
            .RuleFor(p => p.Resource, f => f.PickRandom("Identity", "Document", "Transaction", "Account"))
            .RuleFor(p => p.Action, f => f.PickRandom("View", "Create", "Update", "Delete"))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence());

        _permissions.AddRange(permissionFaker.Generate(25));

        var hierarchyFaker = new Faker<RoleHierarchyInfo>()
            .RuleFor(h => h.HierarchyId, f => $"HIER-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(h => h.ParentRoleId, f => _roles[f.Random.Int(0, _roles.Count - 1)].RoleId)
            .RuleFor(h => h.ChildRoleId, f => _roles[f.Random.Int(0, _roles.Count - 1)].RoleId)
            .RuleFor(h => h.Level, f => f.Random.Int(1, 5));

        _roleHierarchies.AddRange(hierarchyFaker.Generate(15));
    }

    public Task<List<IdentityInfo>> GetIdentitiesAsync() => Task.FromResult(_identities);
    public Task<List<DIDInfo>> GetDIDsAsync() => Task.FromResult(_dids);
    public Task<List<DocumentInfo>> GetDocumentsAsync() => Task.FromResult(_documents);
    public Task<List<VoteInfo>> GetVotesAsync() => Task.FromResult(_votes);
    public Task<List<ComplianceCheckInfo>> GetComplianceChecksAsync() => Task.FromResult(_complianceChecks);
    public Task<List<CitizenshipApplicationInfo>> GetCitizenshipApplicationsAsync() => Task.FromResult(_citizenshipApplications);
    public Task<List<CitizenInfo>> GetCitizensAsync() => Task.FromResult(_citizens);
    public Task<List<PassportInfo>> GetPassportsAsync() => Task.FromResult(_passports);
    public Task<List<TravelIdentityInfo>> GetTravelIdentitiesAsync() => Task.FromResult(_travelIdentities);
    public Task<List<DiplomatInfo>> GetDiplomatsAsync() => Task.FromResult(_diplomats);
    public Task<List<PaymentPlanInfo>> GetPaymentPlansAsync() => Task.FromResult(_paymentPlans);
    public Task<List<AMLFlagInfo>> GetAMLFlagsAsync() => Task.FromResult(_amlFlags);
    public Task<List<KYCStatusInfo>> GetKYCStatusesAsync() => Task.FromResult(_kycStatuses);
    public Task<List<FraudReportInfo>> GetFraudReportsAsync() => Task.FromResult(_fraudReports);
    public Task<List<AuditEntryInfo>> GetAuditEntriesAsync() => Task.FromResult(_auditEntries);
    public Task<List<RedFlagInfo>> GetRedFlagsAsync() => Task.FromResult(_redFlags);
    public Task<List<RoleInfo>> GetRolesAsync() => Task.FromResult(_roles);
    public Task<List<PermissionInfo>> GetPermissionsAsync() => Task.FromResult(_permissions);
    public Task<List<RoleHierarchyInfo>> GetRoleHierarchiesAsync() => Task.FromResult(_roleHierarchies);
}

