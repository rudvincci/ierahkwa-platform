# Authentik OIDC Configuration for Mamey Government Portal

This guide provides complete instructions for setting up Authentik as the OpenID Connect (OIDC) identity provider for the Mamey Government Portal.

## Quick Setup (Automated)

If you already have Authentik running with an admin user, use the automated setup script:

```bash
cd Mamey.Government/Monolith

# Set your Authentik API token (or the script will prompt for it)
export AUTHENTIK_TOKEN="your-api-token-here"

# Run the setup script
./scripts/setup-authentik.sh
```

The script will:
- Create required groups (portal-admin, portal-citizen, portal-agent, etc.)
- Create property mappings for roles and tenant claims
- Create the OAuth2/OIDC provider
- Create the application
- Create test users with passwords
- Output the configuration for appsettings.json

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Starting Authentik](#starting-authentik)
3. [Initial Authentik Setup](#initial-authentik-setup)
4. [Creating the OIDC Provider](#creating-the-oidc-provider)
5. [Creating the Application](#creating-the-application)
6. [Configuring Roles (Property Mappings)](#configuring-roles-property-mappings)
7. [Creating Test Users](#creating-test-users)
8. [Portal Configuration](#portal-configuration)
9. [Testing the Integration](#testing-the-integration)
10. [Troubleshooting](#troubleshooting)

---

## Prerequisites

- Docker and Docker Compose installed
- Mamey Government Portal project cloned
- `authentik.env` file configured (see below)

### Create authentik.env

Create a file at `Mamey.Government/Monolith/authentik.env`:

```bash
# PostgreSQL credentials
AUTHENTIK_PG_USER=authentik
AUTHENTIK_PG_PASS=supersecretpassword
AUTHENTIK_PG_DB=authentik

# Secret key for signing (generate with: openssl rand -hex 32)
AUTHENTIK_SECRET_KEY=your-super-secret-key-min-32-chars

# Optional: Error reporting
AUTHENTIK_ERROR_REPORTING__ENABLED=false

# Authentik version tag
AUTHENTIK_TAG=2025.10.0

# Port configuration (avoid conflicts with MinIO)
AUTHENTIK_HTTP_PORT=9100
AUTHENTIK_HTTPS_PORT=9444
```

---

## Starting Authentik

```bash
cd Mamey.Government/Monolith

# Start Authentik services
docker compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d

# Check status
docker compose -f docker-compose.authentik.console.yml ps
```

### Authentik URLs

| Service | URL |
|---------|-----|
| Admin Console | http://localhost:9100/if/admin/ |
| Initial Setup | http://localhost:9100/if/flow/initial-setup/ |
| User Dashboard | http://localhost:9100/if/user/ |

---

## Initial Authentik Setup

1. Navigate to: http://localhost:9100/if/flow/initial-setup/
2. Create the initial admin account:
   - **Email**: `admin@mamey.local`
   - **Password**: Choose a strong password
3. Complete the setup wizard

---

## Creating the OIDC Provider

1. Log into Authentik Admin Console: http://localhost:9100/if/admin/
2. Navigate to **Applications → Providers**
3. Click **Create** and select **OAuth2/OpenID Provider**
4. Configure:

| Field | Value |
|-------|-------|
| **Name** | `mamey-government` |
| **Authorization flow** | `default-provider-authorization-implicit-consent` |
| **Client type** | `Confidential` |
| **Client ID** | `mamey-government` (auto-generated or set manually) |
| **Client Secret** | Click to reveal/copy (save this!) |
| **Redirect URIs** | `https://localhost:7295/signin-oidc` |
| **Signing Key** | `authentik Self-signed Certificate` |
| **Subject mode** | `Based on the User's ID` |

### Advanced Protocol Settings

| Field | Value |
|-------|-------|
| **Access token validity** | `minutes=5` |
| **Refresh token validity** | `days=30` |
| **Include claims in id_token** | ✅ Checked |

5. Click **Create**
6. **Copy the Client Secret** - you'll need this for portal configuration

---

## Creating the Application

1. Navigate to **Applications → Applications**
2. Click **Create**
3. Configure:

| Field | Value |
|-------|-------|
| **Name** | `Mamey Government Portal` |
| **Slug** | `mamey-government` |
| **Provider** | Select `mamey-government` (the provider you just created) |
| **Launch URL** | `https://localhost:7295/` |

4. Click **Create**

---

## Configuring Roles (Property Mappings)

The portal expects a `roles` claim with values like: `Admin`, `Citizen`, `GovernmentAgent`, `ContentEditor`, `LibraryEditor`.

### Step 1: Create a Custom Scope for Roles

1. Navigate to **Customization → Property Mappings**
2. Click **Create** and select **Scope Mapping**
3. Configure:

| Field | Value |
|-------|-------|
| **Name** | `Government Portal Roles` |
| **Scope name** | `roles` |
| **Expression** | See below |

**Expression:**

```python
# Return user's groups as roles
# Map Authentik groups to portal roles
group_to_role = {
    "portal-admin": "Admin",
    "portal-citizen": "Citizen",
    "portal-agent": "GovernmentAgent",
    "portal-content-editor": "ContentEditor",
    "portal-library-editor": "LibraryEditor",
}

roles = []
for group in user.ak_groups.all():
    if group.name in group_to_role:
        roles.append(group_to_role[group.name])

# Return as list
return roles
```

4. Click **Create**

### Step 2: Create Tenant Property Mapping

1. Navigate to **Customization → Property Mappings**
2. Click **Create** and select **Scope Mapping**
3. Configure:

| Field | Value |
|-------|-------|
| **Name** | `Government Portal Tenant` |
| **Scope name** | `tenant` |
| **Expression** | `return "default"` |

4. Click **Create**

### Step 3: Add Mappings to Provider

1. Navigate to **Applications → Providers**
2. Edit `mamey-government`
3. In **Property mappings** section, add:
   - `Government Portal Roles`
   - `Government Portal Tenant`
4. Click **Update**

---

## Creating Test Users

### Create Required Groups

1. Navigate to **Directory → Groups**
2. Create the following groups:

| Group Name | Description |
|------------|-------------|
| `portal-admin` | Full administrative access |
| `portal-citizen` | Registered citizen access |
| `portal-agent` | Government agent access |
| `portal-content-editor` | CMS content editing access |
| `portal-library-editor` | Library document management |

### Create Test Users

1. Navigate to **Directory → Users**
2. Click **Create**
3. Create users for each role:

#### Admin User
| Field | Value |
|-------|-------|
| **Username** | `admin` |
| **Name** | `Admin User` |
| **Email** | `admin@mamey.local` |
| **Groups** | `portal-admin` |
| **Is active** | ✅ |

Set password via **Users → [user] → Set Password**

#### Citizen User
| Field | Value |
|-------|-------|
| **Username** | `citizen` |
| **Name** | `John Citizen` |
| **Email** | `citizen@mamey.local` |
| **Groups** | `portal-citizen` |

#### Government Agent User
| Field | Value |
|-------|-------|
| **Username** | `agent` |
| **Name** | `Jane Agent` |
| **Email** | `agent@mamey.local` |
| **Groups** | `portal-agent` |

---

## Portal Configuration

### Update appsettings.Development.json

```json
{
  "Auth": {
    "Mode": "Oidc",
    "TenantClaimType": "tenant",
    "Oidc": {
      "Authority": "http://localhost:9100/application/o/mamey-government/",
      "ClientId": "mamey-government",
      "ClientSecret": "PASTE_CLIENT_SECRET_FROM_AUTHENTIK",
      "RoleClaimType": "roles",
      "NameClaimType": "preferred_username",
      "RequireHttpsMetadata": false
    }
  }
}
```

### Important Notes

- The `Authority` URL must end with a trailing `/`
- `RequireHttpsMetadata: false` is only for development
- In production, use HTTPS and set `RequireHttpsMetadata: true`

---

## Testing the Integration

### Start the Portal

```bash
cd Mamey.Government/Monolith/src/Bootstrapper/Mamey.Government.BlazorServer
dotnet run --urls "https://localhost:7295;http://localhost:5075"
```

### Test Login Flow

1. Navigate to: https://localhost:7295/
2. Click **Login** or try accessing a protected page
3. You should be redirected to Authentik login page
4. Log in with one of the test users
5. After successful authentication, you'll be redirected back to the portal
6. Verify:
   - User name appears in the header
   - Role-based navigation items are visible
   - Protected pages are accessible based on role

### Verify Claims

Access the debug endpoint (development only):

```
https://localhost:7295/dev/whoami
```

This shows the current user's claims including roles.

---

## Troubleshooting

### Common Issues

#### 1. "The correlation cookie is missing"

**Cause**: Cookie settings not compatible with HTTP/HTTPS mix

**Solution**: Ensure you're accessing the portal via HTTPS:
```
https://localhost:7295
```

#### 2. "Unable to retrieve document from authority"

**Cause**: Authentik not accessible or wrong Authority URL

**Solution**:
1. Verify Authentik is running: `docker ps | grep authentik`
2. Check Authority URL ends with `/`
3. Test OpenID configuration: `curl http://localhost:9100/application/o/mamey-government/.well-known/openid-configuration`

#### 3. "Invalid client" error

**Cause**: Client ID or Secret mismatch

**Solution**:
1. Verify Client ID matches in both Authentik and portal config
2. Copy the Client Secret again from Authentik provider settings

#### 4. Roles not appearing / Access denied

**Cause**: Property mappings not configured or user not in groups

**Solution**:
1. Verify property mappings are added to the provider
2. Verify user is in the correct groups
3. Check the `/dev/whoami` endpoint to see actual claims

#### 5. Redirect loop

**Cause**: Cookie/session issues

**Solution**:
1. Clear browser cookies
2. Try incognito/private browsing mode
3. Restart the portal application

### Viewing Authentik Logs

```bash
# Server logs
docker logs mamey-authentik-server -f

# Worker logs
docker logs mamey-authentik-worker -f
```

### Verifying OpenID Configuration

```bash
curl -s http://localhost:9100/application/o/mamey-government/.well-known/openid-configuration | jq .
```

---

## Portal Roles Reference

| Role | Description | Access Level |
|------|-------------|--------------|
| `Admin` | Full system administration | All features + admin panel |
| `Citizen` | Registered citizen | Citizen portal, personal documents |
| `GovernmentAgent` | Government staff | Government portal, process applications |
| `ContentEditor` | CMS content manager | CMS, news, pages management |
| `LibraryEditor` | Document librarian | Library document management |

---

## Quick Reference

### URLs

| Service | URL |
|---------|-----|
| Portal (HTTPS) | https://localhost:7295 |
| Portal (HTTP) | http://localhost:5075 |
| Authentik Admin | http://localhost:9100/if/admin/ |
| OIDC Discovery | http://localhost:9100/application/o/mamey-government/.well-known/openid-configuration |

### Docker Commands

```bash
# Start Authentik
docker compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d

# Stop Authentik
docker compose -f docker-compose.authentik.console.yml down

# View logs
docker compose -f docker-compose.authentik.console.yml logs -f

# Reset Authentik (⚠️ destroys data)
docker compose -f docker-compose.authentik.console.yml down -v
```

---

## Next Steps

After successful integration:

1. Configure additional identity sources (LDAP, SAML, etc.)
2. Set up Multi-Factor Authentication (MFA)
3. Configure password policies
4. Set up audit logging
5. Configure session timeout policies
