# Authentik Cleanup Script

This script removes Authentik resources created for example tenants (Springfield, California) so you can start fresh with your actual government tenants.

## Usage

### Clean Up Example Resources

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith

export AUTHENTIK_TOKEN="your-api-token-here"
./scripts/cleanup-authentik.sh
```

### What Gets Deleted

The cleanup script removes:

1. **OAuth2 Providers** - Providers for Springfield/California
2. **Applications** - B2B and B2C applications for example tenants
3. **Flows** - Authentication and enrollment flows
4. **Prompt Stages** - Terms acceptance and profile prompts
5. **Groups** - Government-specific groups (springfield-*, california-*)
6. **Users** - Test users created for examples
7. **Property Mappings** - Tenant and role mappings
8. **Brands** - B2B and B2C brands
9. **Tenants** - Authentik native tenants (if tenancy enabled)

### After Cleanup

1. Update `config/government-tenants.json` with your actual government tenants
2. Run `./scripts/setup-authentik.sh` to create resources for your tenants

## Safety

- The script only deletes resources matching Springfield/California patterns
- Your other Authentik resources are not affected
- The script is idempotent - safe to run multiple times
