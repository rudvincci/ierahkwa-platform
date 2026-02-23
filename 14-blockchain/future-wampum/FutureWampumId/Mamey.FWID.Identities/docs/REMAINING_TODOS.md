# Remaining TODOs - Identities Microservice

## Overview
This document tracks remaining TODOs and placeholder implementations that need to be completed when external dependencies are available.

## 1. BiometricClient - gRPC Integration

**Location**: `Mamey.FWID.Identities.Infrastructure/Clients/BiometricClient.cs`

**Status**: Placeholder implementation with TODO comments

**Description**: The `BiometricClient` is a placeholder implementation for integrating with the external Biometric Verification Microservice. It currently returns mock data.

**TODOs**:
- [ ] Implement gRPC call to Biometric Verification Microservice for `LivenessVerifyAsync`
- [ ] Implement gRPC call to Biometric Verification Microservice for `ExtractTemplateAsync`
- [ ] Implement gRPC call to Biometric Verification Microservice for `EnrollAsync`
- [ ] Implement gRPC call to Biometric Verification Microservice for `VerifyAsync`
- [ ] Implement gRPC call to Biometric Verification Microservice for `DeleteTemplateAsync`

**Dependencies**:
- Biometric Verification Microservice must be implemented and deployed
- gRPC proto definitions must be available
- Service endpoint configuration must be set in `appsettings.json`

**Impact**: 
- Low - Current placeholder allows the Identities service to function
- Biometric operations will use mock data until real service is integrated
- All other functionality works independently

## 2. BiometricEvidenceService - JWS Signing

**Location**: `Mamey.FWID.Identities.Application/Services/BiometricEvidenceService.cs`

**Status**: Partial implementation with TODO comments

**Description**: The `BiometricEvidenceService` creates evidence objects for biometric operations but needs JWS signing implementation.

**TODOs**:
- [ ] Implement JWS signing with service key (ES256/Ed25519 per spec §16) in `CreateEnrollmentEvidenceAsync`
- [ ] Implement JWS signing with service key (ES256/Ed25519 per spec §16) in `CreateVerificationEvidenceAsync`
- [ ] Implement JWS signature validation and structure validation in `ValidateEvidenceAsync`

**Dependencies**:
- Service signing key must be configured
- JWS library (e.g., `System.IdentityModel.Tokens.Jwt` or similar)
- Key management system for secure key storage

**Impact**:
- Medium - Evidence objects are created but not cryptographically signed
- Evidence validation will not work until signing is implemented
- Other functionality works independently

## 3. Test Coverage Enhancements

**Location**: Various test projects

**Status**: Core tests implemented, additional coverage recommended

**Description**: While core functionality is tested, additional test scenarios would improve coverage.

**Recommended Enhancements**:
- [ ] Device registration tests (when device registration is implemented)
- [ ] Additional boundary value tests
- [ ] Additional state transition tests
- [ ] Additional security penetration tests
- [ ] Additional performance tests for different endpoints

**Impact**:
- Low - Core functionality is well-tested
- Additional tests would improve confidence and catch edge cases

## 4. Documentation

**Status**: Good, but can be enhanced

**Recommended Enhancements**:
- [ ] API documentation with Swagger/OpenAPI annotations
- [ ] Deployment guide
- [ ] Integration guide for other microservices
- [ ] Troubleshooting guide
- [ ] Performance tuning guide

**Impact**:
- Low - Current documentation is sufficient for development
- Enhanced documentation would improve developer experience

## Priority Summary

### High Priority (Blocking Production)
None - All critical functionality is implemented

### Medium Priority (Should Complete Soon)
1. **BiometricEvidenceService JWS Signing** - Required for evidence validation
   - Estimated effort: 2-4 hours
   - Dependencies: Signing key configuration, JWS library

### Low Priority (Nice to Have)
1. **BiometricClient gRPC Integration** - Required when Biometric Verification Microservice is ready
   - Estimated effort: 4-8 hours
   - Dependencies: External service availability

2. **Test Coverage Enhancements** - Improve confidence
   - Estimated effort: Ongoing
   - Dependencies: None

3. **Documentation Enhancements** - Improve developer experience
   - Estimated effort: Ongoing
   - Dependencies: None

## Notes

- All TODOs are documented and intentional placeholders
- The service is fully functional with current implementations
- Placeholder implementations allow development to continue independently
- Integration points are clearly marked and ready for implementation


