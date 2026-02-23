# API Contracts Directory

This directory stores all contract and specification files that serve as the **single source of truth** for API definitions in the Mamey Framework.

## Directory Structure

```
api-contracts/
├── grpc/              # gRPC service contracts (.proto files)
├── openapi/           # OpenAPI REST API contracts (.yaml files)
├── graphql/           # GraphQL schema contracts (.graphql files)
└── README.md          # This file
```

## Contract Types

### gRPC Contracts
- **Location**: `api-contracts/grpc/{service-name}.proto`
- **Format**: Protocol Buffers v3
- **Purpose**: Service-to-service communication, high-performance requirements
- **Validation**: `protoc --proto_path=api-contracts/grpc --validate_out=. {file}.proto`
- **Template**: `.cursor/templates/contracts/grpc-service-template.proto`

### OpenAPI Contracts
- **Location**: `api-contracts/openapi/{service-name}.yaml`
- **Format**: OpenAPI 3.0.3
- **Purpose**: Public APIs, frontend-backend communication, REST endpoints
- **Standards**: Follow ADR-001 (OpenAPI 3.0) and ADR-007 (API Standards)
- **Validation**: `swagger-cli validate {file}.yaml` or `openapi-generator validate -i {file}.yaml`
- **Template**: `.cursor/templates/contracts/openapi-service-template.yaml`

### GraphQL Contracts
- **Location**: `api-contracts/graphql/{service-name}.graphql`
- **Format**: GraphQL Schema Definition Language (SDL)
- **Purpose**: Flexible query requirements, complex data relationships
- **Validation**: `graphql-schema-linter {file}.graphql`
- **Template**: `.cursor/templates/contracts/graphql-schema-template.graphql`

### CQRS Contracts
- **Location**: `{ServiceName}/src/{ServiceName}.Contracts/` (service-specific)
- **Format**: C# classes with `[Contract]` attribute
- **Purpose**: Internal application commands, queries, events
- **Pattern**: 
  - Commands and Events MUST have `[Contract]` attribute
  - Queries MUST NOT have `[Contract]` attribute (per Mamey Framework rules)
- **Template**: `.cursor/templates/contracts/cqrs-contracts-template.md`

## Spec-Driven Workflow

Contracts are defined **FIRST** before implementation:

1. **Define Contracts**: Use `/define-contracts` to create contract files
2. **Generate BDD/TDD**: BDD/TDD documents reference contracts
3. **Implement**: Code must match contract definitions
4. **Validate**: Use `/validate-contracts` to ensure compliance

## Commands

- `/define-contracts` - Define contracts/specs first
- `/validate-contracts` - Validate implementation against contracts
- `/plan-application` - Spec-driven application planning (includes contract definition)
- `/plan-feature` - Spec-driven feature planning (includes contract definition)

## Scripts

- `.cursor/scripts/contracts/define-{type}-contracts.sh` - Contract definition scripts
- `.cursor/scripts/contracts/validate-contracts.sh` - Contract validation script

## Templates

- `.cursor/templates/contracts/grpc-service-template.proto` - gRPC template
- `.cursor/templates/contracts/openapi-service-template.yaml` - OpenAPI template
- `.cursor/templates/contracts/cqrs-contracts-template.md` - CQRS template
- `.cursor/templates/contracts/graphql-schema-template.graphql` - GraphQL template
- `.cursor/templates/contracts/contract-validation-checklist.md` - Validation checklist

## Versioning

All contracts use semantic versioning (MAJOR.MINOR.PATCH):
- **Breaking changes**: Require MAJOR version bump
- **Non-breaking additions**: Require MINOR version bump
- **Bug fixes**: Require PATCH version bump

## Validation

Contracts are validated:
- **Syntax**: Contract syntax is valid
- **Completeness**: All operations have contract definitions
- **Implementation Match**: Code matches contract definitions
- **Breaking Changes**: Detect breaking changes before releases

## Related

- `.cursor/rules/workflow/spec-driven-development.md` - Complete spec-driven workflow guide
- `.cursor/commands/contracts/define-contracts.md` - Contract definition command
- `.cursor/commands/contracts/validate-contracts.md` - Contract validation command
- `.cursor/adr/adr-001-api-specification-format.md` - OpenAPI standard
- `.cursor/adr/adr-007-api-standards.md` - API standards
