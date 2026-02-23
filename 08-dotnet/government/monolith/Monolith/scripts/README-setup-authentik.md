# Authentik Multi-Tenant Setup Script

This script configures Authentik for hierarchical multi-tenant support with B2B (government employees) and B2C (citizens) flows per government tenant.

## Prerequisites

1. Authentik running and accessible (default: `http://localhost:9100`)
2. Authentik API token with appropriate permissions
3. `jq` installed for JSON processing
4. `curl` installed for API calls

## Configuration

### Environment Variables

- `AUTHENTIK_TOKEN` - Authentik API token (required)
- `AUTHENTIK_URL` - Authentik base URL (default: `http://localhost:9100`)
- `PORTAL_URL` - Portal base URL (default: `https://localhost:7295`)
- `ENABLE_TENANCY` - Enable Authentik native tenancy (default: `true`)
- `TENANT_SOURCE` - Source for government tenants: `config`, `env`, `database`, or `api` (default: `config`)

### Tenant Sources

#### Config File (Recommended)
Create `config/government-tenants.json`:

```json
[
  {
    "id": "ink",
    "name": "Ierahkwa ne Kanienke Government",
    "domain": "ink.gov"
  }
]
```

#### Environment Variable
```bash
export GOVERNMENT_TENANTS='[{"id":"ink","name":"Ierahkwa ne Kanienke Government","domain":"ink.gov"}]'
export TENANT_SOURCE="env"
```

## Usage

### Basic Usage

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith

# Set your Authentik API token
export AUTHENTIK_TOKEN="your-api-token-here"

# Run the script
./scripts/setup-authentik.sh
```

### With Custom Configuration

```bash
export AUTHENTIK_TOKEN="your-api-token-here"
export AUTHENTIK_URL="http://localhost:9100"
export PORTAL_URL="https://localhost:7295"
export TENANT_SOURCE="config"
export TENANT_CONFIG_FILE="config/government-tenants.json"

./scripts/setup-authentik.sh
```

## What the Script Creates

For each government tenant, the script creates:

1. **Authentik Tenant** (if tenancy enabled)
   - Schema: `t_gov_{government-id}`
   - Domain association

2. **Groups**
   - `{gov-id}-admin` - B2B admin group
   - `{gov-id}-agent` - B2B agent group
   - `{gov-id}-manager` - B2B manager group
   - `{gov-id}-citizen` - B2C citizen group
   - Global groups: `portal-admin`, `portal-agent`, `citizen`

3. **Prompt Stages**
   - Terms Acceptance prompt
   - B2B Profile prompt (job title, department, phone)

4. **Authentication Flows**
   - B2B Authentication Flow: `{gov-name} B2B Authentication`
   - B2C Authentication Flow: `{gov-name} B2C Authentication`

5. **Brands**
   - B2B Brand for government domain
   - B2C Brand for citizens subdomain

6. **Property Mappings**
   - Tenant mapping: Returns government ID
   - Roles mapping: Maps groups to portal roles

7. **OAuth2/OIDC Provider**
   - Client ID: `mamey-government-{gov-id}`
   - Configured with tenant and role mappings

8. **Applications**
   - B2B Application: `{gov-name} Portal (B2B)`
   - B2C Application: `{gov-name} Citizen Portal (B2C)`

9. **Test Users**
   - `{gov-id}-admin@${gov-domain}` - Admin user
   - `{gov-id}-agent@${gov-domain}` - Agent user
   - `{gov-id}-citizen1@example.com` - Citizen user
   - Password: `Test123!`

## Output

The script outputs:
- OIDC configuration for each government tenant
- Client IDs and secrets
- Instructions for updating `appsettings.Development.json`

## Notes

- The script is idempotent - it can be run multiple times safely
- Existing resources are detected and skipped
- Some API calls may need adjustment based on Authentik version
- Prompt stages API structure may vary by Authentik version

## Troubleshooting

### "Failed to authenticate with Authentik API"
- Verify your API token is correct
- Check token has required permissions
- Ensure Authentik is running and accessible

### "No government tenants found"
- Verify tenant source configuration
- Check config file exists and is valid JSON
- Verify environment variables are set correctly

### "Could not create tenant"
- Check Authentik tenancy is enabled in `authentik.env`
- Verify API token has tenant management permissions
- Check Authentik logs for detailed errors
