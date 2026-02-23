# Migration Guide

This guide helps you migrate between versions of Mamey.Authentik.

## Version 1.0.0

Initial release of Mamey.Authentik library.

### Breaking Changes

None - this is the initial release.

### New Features

- Complete Authentik API v3 support (after code generation)
- API token authentication
- OAuth2 client credentials authentication
- Retry policies with exponential backoff
- Circuit breaker support
- In-memory and distributed caching
- Comprehensive error handling
- Logging support

## Upgrading from Pre-Release Versions

If you were using a pre-release version:

1. Update package reference to `1.0.0`
2. Review any breaking changes in the changelog
3. Update your code to use the new API if needed
4. Run your tests to ensure compatibility

## Code Generation Updates

When Authentik releases new API versions:

1. Fetch the latest schema:
   ```bash
   ./scripts/update-schema.sh https://your-authentik-instance.com
   ```

2. Regenerate the client:
   ```bash
   ./scripts/generate-client.sh https://your-authentik-instance.com
   ```

3. Review breaking changes:
   - Check for removed endpoints
   - Check for changed request/response models
   - Update service implementations if needed

4. Update tests:
   - Update test data if models changed
   - Add tests for new endpoints
   - Remove tests for deprecated endpoints

5. Update documentation:
   - Update API reference
   - Update examples if needed
   - Update this migration guide

## Common Migration Scenarios

### Updating Service Implementations

After regenerating the client, you may need to update service implementations:

```csharp
// Old (example)
public async Task<User> GetUserAsync(string userId)
{
    // Implementation
}

// New (if API changed)
public async Task<UserDto> GetUserAsync(string userId)
{
    // Updated implementation with new model
}
```

### Handling Deprecated Endpoints

If an endpoint is deprecated:

1. Check Authentik release notes for replacement
2. Update service implementation to use new endpoint
3. Add deprecation warnings if needed
4. Plan removal in next major version

## Getting Help

If you encounter issues during migration:

1. Check the [CHANGELOG.md](../CHANGELOG.md) for detailed changes
2. Review [API_REFERENCE.md](API_REFERENCE.md) for current API
3. Check [EXAMPLES.md](EXAMPLES.md) for usage examples
4. Open an issue on GitHub for support
