# FutureWampumId TDD Compliance & Mamey.Blockchain Integration Guide

## Overview

This guide explains how to use Maestro to ensure all FutureWampumId microservices comply with the TDD specifications and integrate with Mamey.Blockchain libraries for MameyNode connectivity.

## Prerequisites

1. **Orchestrator Configured**: Ensure `.agent-orchestrator/config/orchestrator.config.yml` is properly configured
2. **TDD Document**: Located at `.designs/TDD/FutureWampum/FutureWampumID TDD.md`
3. **Plan Files**: Located at `.cursor/plans/FutureWampum/FutureWampumId/*.plan.md`
4. **Services**: Located at `/Volumes/Barracuda/mamey-io/code-final/FutureWampum/FutureWampumId/`
5. **Mamey.Blockchain Libraries**: Located at `Mamey/src/Mamey.Blockchain.*`
6. **MameyNode**: Located at `MameyNode/`

## Workflow Overview

The `fwid-compliance-workflow.yml` workflow performs the following phases:

### Phase 1: Analysis and Gap Identification
- Reads and analyzes the TDD document
- Reads all plan files
- Inventories existing microservices
- Checks existing Mamey.Blockchain references

### Phase 2: Service-by-Service Compliance Check
- Checks each microservice against TDD and plan requirements
- Verifies all layers (Domain, Application, Infrastructure, Contracts, API)
- Identifies missing components

### Phase 3: Mamey.Blockchain Integration Analysis
- Identifies required blockchain libraries
- Analyzes MameyNode integration points
- Verifies blockchain references in services

### Phase 4: Generate Compliance Report
- Compiles all findings
- Generates prioritized remediation plan

### Phase 5: Implementation Tasks
- Implements missing domain layer components
- Implements missing application layer components
- Implements missing infrastructure layer components
- Implements missing API layer components

### Phase 6: Mamey.Blockchain Integration Implementation
- Adds blockchain references to .csproj files
- Implements blockchain services
- Configures MameyNode connectivity

### Phase 7: Verification and Testing
- Runs build verification
- Verifies TDD compliance
- Verifies blockchain integration

### Phase 8: Documentation Update
- Updates service READMEs
- Creates integration guide

## Running the Workflow

### Step 1: Run Analysis Phase

```bash
cd .agent-orchestrator
npm run dev execute --workflow config/fwid-compliance-workflow.yml --phase analyze_tdd_and_plans
```

This will:
- Analyze the TDD document
- Read all plan files
- Inventory existing services
- Check blockchain references

Output files:
- `tdd_analysis.json`
- `plans_analysis.json`
- `services_inventory.json`
- `blockchain_references.json`

### Step 2: Check Compliance

```bash
npm run dev execute --workflow config/fwid-compliance-workflow.yml --phase check_service_compliance
```

This will check each service against TDD and plan requirements.

Output files:
- `identities_compliance.json`
- `dids_compliance.json`
- `zkps_compliance.json`
- `accesscontrols_compliance.json`
- `credentials_compliance.json`
- `apigateway_compliance.json`

### Step 3: Analyze Blockchain Integration

```bash
npm run dev execute --workflow config/fwid-compliance-workflow.yml --phase analyze_blockchain_integration
```

This will identify required blockchain libraries and integration points.

Output files:
- `required_blockchain_libraries.json`
- `mameynode_integration_points.json`
- `blockchain_verification.json`

### Step 4: Generate Compliance Report

```bash
npm run dev execute --workflow config/fwid-compliance-workflow.yml --phase generate_compliance_report
```

This will generate:
- `compliance_report.md` - Comprehensive compliance findings
- `remediation_plan.md` - Prioritized remediation plan

### Step 5: Review and Approve

Review the `compliance_report.md` and `remediation_plan.md` files to understand what needs to be fixed.

### Step 6: Run Full Workflow (Optional)

If you want to run the entire workflow including implementation:

```bash
npm run dev execute --workflow config/fwid-compliance-workflow.yml
```

**Warning**: This will make changes to your codebase. Review the remediation plan first!

## Service Mapping

### TDD/Plan Names → Actual Service Names

| TDD/Plan Name | Actual Service Name | Notes |
|--------------|---------------------|-------|
| Identity | Mamey.FWID.Identities | Pluralized |
| DID | Mamey.FWID.DIDs | Pluralized |
| ZKP | Mamey.FWID.ZKPs | Pluralized |
| AccessControl | Mamey.FWID.AccessControls | Pluralized |
| Credential | Mamey.FWID.Credentials | Pluralized |
| API | Mamey.FWID.ApiGateway | Different name |

### Additional Services (Not in Plans)

- Mamey.FWID.Notifications
- Mamey.FWID.Operations
- Mamey.FWID.Sagas

These services may need separate compliance checks.

## Mamey.Blockchain Libraries

### Required Libraries for FutureWampumId

1. **Mamey.Blockchain.Node** - Core MameyNode connectivity
2. **Mamey.Blockchain.Crypto** - Cryptographic operations
3. **Mamey.Blockchain.LedgerIntegration** - Ledger operations
4. **Mamey.Blockchain.Compliance** - Compliance checks
5. **Mamey.Blockchain.UniversalProtocolGateway** - Protocol gateway

### Integration Pattern

Each FutureWampumId service should:

1. **Reference Required Libraries** in `.csproj`:
```xml
<ProjectReference Include="../../Mamey/src/Mamey.Blockchain.Node/src/Mamey.Blockchain.Node/Mamey.Blockchain.Node.csproj" />
<ProjectReference Include="../../Mamey/src/Mamey.Blockchain.Crypto/src/Mamey.Blockchain.Crypto/Mamey.Blockchain.Crypto.csproj" />
```

2. **Create Blockchain Service Interface**:
```csharp
public interface IBlockchainService
{
    Task<string> SubmitTransactionAsync(TransactionRequest request);
    Task<BlockchainResponse> QueryLedgerAsync(QueryRequest request);
}
```

3. **Implement Blockchain Service**:
```csharp
public class BlockchainService : IBlockchainService
{
    private readonly IBlockchainNodeClient _nodeClient;
    
    public async Task<string> SubmitTransactionAsync(TransactionRequest request)
    {
        // Use Mamey.Blockchain.Node to connect to MameyNode
        return await _nodeClient.SubmitTransactionAsync(request);
    }
}
```

4. **Configure in Infrastructure**:
```csharp
services.AddBlockchainNode(options =>
{
    options.NodeUrl = configuration["MameyNode:Url"];
    options.ApiKey = configuration["MameyNode:ApiKey"];
});
```

## TDD Compliance Checklist

For each microservice, verify:

### Domain Layer
- [ ] Aggregate root matches TDD specification
- [ ] All value objects implemented per TDD
- [ ] Domain methods implemented per TDD
- [ ] Domain events defined per TDD

### Application Layer
- [ ] All commands from TDD Section 3 implemented
- [ ] All queries from TDD Section 4 implemented
- [ ] All event handlers from TDD Section 5 implemented
- [ ] Application services implemented per TDD

### Infrastructure Layer
- [ ] Repositories implemented per TDD
- [ ] External service integrations configured
- [ ] Mamey.Blockchain.* libraries integrated
- [ ] MameyNode connectivity configured

### Contracts Layer
- [ ] All commands have `[Contract]` attribute
- [ ] All events have `[Contract]` attribute
- [ ] Queries do NOT have `[Contract]` attribute
- [ ] DTOs match TDD specifications

### API Layer
- [ ] All endpoints from TDD implemented
- [ ] Routes match TDD specifications
- [ ] Authentication/authorization configured
- [ ] Validation implemented

## Troubleshooting

### Build Errors

If you encounter build errors after adding Mamey.Blockchain references:

1. Ensure all Mamey.Blockchain projects build successfully:
```bash
cd Mamey
dotnet build
```

2. Check project reference paths are correct
3. Verify .NET version compatibility

### Missing TDD References

If the workflow can't find TDD line references:

1. Verify TDD document path: `.designs/TDD/FutureWampum/FutureWampumID TDD.md`
2. Check that line numbers haven't changed
3. Use grep to find the actual line numbers

### MameyNode Connectivity Issues

If MameyNode connectivity fails:

1. Verify MameyNode is running:
```bash
cd MameyNode
cargo run
```

2. Check gRPC endpoint configuration
3. Verify authentication tokens
4. Check network connectivity

## Next Steps

After running the compliance workflow:

1. Review `compliance_report.md`
2. Prioritize fixes from `remediation_plan.md`
3. Implement fixes incrementally
4. Re-run compliance checks after each fix
5. Update documentation as you go

## Support

For issues or questions:
- Check `.agent-orchestrator/docs/` for more documentation
- Review TDD document: `.designs/TDD/FutureWampum/FutureWampumID TDD.md`
- Review plan files: `.cursor/plans/FutureWampum/FutureWampumId/*.plan.md`
