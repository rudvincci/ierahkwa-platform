using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumIdClient : IFutureWampumIdClient
{
    private readonly Faker _faker = new();
    private readonly List<IdentityVerificationInfo> _verifications = new();
    private readonly List<DigitalCredentialInfo> _credentials = new();
    private readonly List<IdentityWalletInfo> _wallets = new();
    private readonly List<AttestationInfo> _attestations = new();
    private readonly List<RecoveryInfo> _recoveries = new();
    private readonly List<DIDDocumentInfo> _didDocuments = new();

    public MockFutureWampumIdClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var verificationFaker = new Faker<IdentityVerificationInfo>()
            .RuleFor(v => v.VerificationId, f => $"VERIFY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(v => v.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10)}")
            .RuleFor(v => v.VerificationType, f => f.PickRandom("Document", "Biometric", "MultiFactor", "Blockchain"))
            .RuleFor(v => v.Verified, f => f.Random.Bool(0.9f))
            .RuleFor(v => v.VerificationResult, f => f.Lorem.Sentence())
            .RuleFor(v => v.VerifiedAt, f => f.Date.Recent(60))
            .RuleFor(v => v.VerifierId, f => $"VER-{f.Random.AlphaNumeric(6)}")
            .RuleFor(v => v.Status, f => f.PickRandom("Pending", "Verified", "Rejected", "Expired"));

        _verifications.AddRange(verificationFaker.Generate(300));

        var credentialFaker = new Faker<DigitalCredentialInfo>()
            .RuleFor(c => c.CredentialId, f => $"CRED-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10)}")
            .RuleFor(c => c.CredentialType, f => f.PickRandom("Passport", "DriversLicense", "Education", "Professional", "Membership"))
            .RuleFor(c => c.Issuer, f => f.Company.CompanyName())
            .RuleFor(c => c.IssuedAt, f => f.Date.Past(1))
            .RuleFor(c => c.ExpiryDate, (f, c) => f.Date.Between(c.IssuedAt, c.IssuedAt.AddYears(5)))
            .RuleFor(c => c.IsRevoked, f => f.Random.Bool(0.05f))
            .RuleFor(c => c.Status, f => f.PickRandom("Active", "Expired", "Revoked"))
            .RuleFor(c => c.Claims, f => new Dictionary<string, string> { { "name", f.Person.FullName }, { "dob", f.Person.DateOfBirth.ToString("yyyy-MM-dd") } });

        _credentials.AddRange(credentialFaker.Generate(500));

        var walletFaker = new Faker<IdentityWalletInfo>()
            .RuleFor(w => w.WalletId, f => $"WALLET-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(w => w.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10)}")
            .RuleFor(w => w.WalletType, f => f.PickRandom("Standard", "Hardware", "Mobile", "Web"))
            .RuleFor(w => w.DIDs, f => new List<string> { $"did:futurewampum:{f.Random.AlphaNumeric(16)}" })
            .RuleFor(w => w.Credentials, f => _credentials.Take(f.Random.Int(1, 5)).Select(c => c.CredentialId).ToList())
            .RuleFor(w => w.CreatedAt, f => f.Date.Past(1))
            .RuleFor(w => w.IsActive, true);

        _wallets.AddRange(walletFaker.Generate(200));

        var attestationFaker = new Faker<AttestationInfo>()
            .RuleFor(a => a.AttestationId, f => $"ATTEST-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(a => a.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10)}")
            .RuleFor(a => a.AttestationType, f => f.PickRandom("Identity", "Credential", "Reputation", "Skill"))
            .RuleFor(a => a.AttesterId, f => $"ATTR-{f.Random.AlphaNumeric(8)}")
            .RuleFor(a => a.Content, f => f.Lorem.Paragraph())
            .RuleFor(a => a.CreatedAt, f => f.Date.Recent(90))
            .RuleFor(a => a.IsVerified, f => f.Random.Bool(0.95f))
            .RuleFor(a => a.Status, f => f.PickRandom("Active", "Revoked", "Expired"));

        _attestations.AddRange(attestationFaker.Generate(400));

        var recoveryFaker = new Faker<RecoveryInfo>()
            .RuleFor(r => r.RecoveryId, f => $"RECOV-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.IdentityId, f => $"ID-{f.Random.AlphaNumeric(10)}")
            .RuleFor(r => r.RecoveryType, f => f.PickRandom("Social", "Backup", "Guardian", "MultiSig"))
            .RuleFor(r => r.Status, f => f.PickRandom("Pending", "InProgress", "Completed", "Failed"))
            .RuleFor(r => r.RequestedAt, f => f.Date.Recent(30))
            .RuleFor(r => r.CompletedAt, (f, r) => r.Status == "Completed" ? f.Date.Between(r.RequestedAt, DateTime.Now) : null)
            .RuleFor(r => r.RecoveryMethods, f => f.Lorem.Words(f.Random.Int(1, 3)).ToList());

        _recoveries.AddRange(recoveryFaker.Generate(25));

        var didDocFaker = new Faker<DIDDocumentInfo>()
            .RuleFor(d => d.DID, f => $"did:futurewampum:{f.Random.AlphaNumeric(16)}")
            .RuleFor(d => d.DIDDocument, (f, d) => $"{{\"id\":\"{d.DID}\",\"@context\":[\"https://www.w3.org/ns/did/v1\"]}}")
            .RuleFor(d => d.Created, f => f.Date.Past(1))
            .RuleFor(d => d.Updated, f => f.Date.Recent(30))
            .RuleFor(d => d.VersionId, f => (ulong)f.Random.Long(1, 10))
            .RuleFor(d => d.Deactivated, f => f.Random.Bool(0.05f))
            .RuleFor(d => d.CanonicalId, f => $"did:futurewampum:{f.Random.AlphaNumeric(16)}");

        _didDocuments.AddRange(didDocFaker.Generate(150));
    }

    public Task<List<IdentityVerificationInfo>> GetIdentityVerificationsAsync() => Task.FromResult(_verifications);
    public Task<List<DigitalCredentialInfo>> GetDigitalCredentialsAsync() => Task.FromResult(_credentials);
    public Task<List<IdentityWalletInfo>> GetIdentityWalletsAsync() => Task.FromResult(_wallets);
    public Task<List<AttestationInfo>> GetAttestationsAsync() => Task.FromResult(_attestations);
    public Task<List<RecoveryInfo>> GetRecoveriesAsync() => Task.FromResult(_recoveries);
    public Task<List<DIDDocumentInfo>> GetDIDDocumentsAsync() => Task.FromResult(_didDocuments);
    public Task<List<DIDHistoryEntryInfo>> GetDIDHistoryAsync(string did)
    {
        var history = new List<DIDHistoryEntryInfo>();
        for (int i = 0; i < 10; i++)
        {
            history.Add(new DIDHistoryEntryInfo
            {
                Version = (ulong)(i + 1),
                DIDDocument = $"{{\"id\":\"{did}\",\"version\":{i + 1}}}",
                Action = i == 0 ? "created" : _faker.PickRandom("updated", "deactivated"),
                Actor = $"USER-{_faker.Random.AlphaNumeric(6)}",
                Timestamp = _faker.Date.Past(1).AddDays(-i),
                TransactionHash = $"0x{_faker.Random.AlphaNumeric(64)}",
                Reason = _faker.Lorem.Sentence()
            });
        }
        return Task.FromResult(history);
    }
}

