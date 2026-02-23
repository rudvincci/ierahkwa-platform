# Authentik → Portal OIDC wiring (Phase 4.5)

This guide configures authentik as an OIDC provider for the portal.

## Prereqs

- Portal is running locally at `http://localhost:5180`
- Authentik is running locally at `http://localhost:9100`
  - Initial setup URL (must include trailing slash): `http://localhost:9100/if/flow/initial-setup/`
  - See authentik Docker Compose install docs: https://docs.goauthentik.io/install-config/install/docker-compose/

## 1) Create an OAuth2/OpenID Provider in authentik

In authentik Admin UI:

- **Applications → Providers → Create**
  - Type: **OAuth2/OpenID Provider**
  - **Name**: `mamey-portal`
  - **Slug**: `mamey-portal`
  - **Client type**: Confidential
  - **Redirect URIs**:
    - `http://localhost:5180/signin-oidc`
  - (Optional) **Post logout redirect URIs**:
    - `http://localhost:5180/`

Copy:
- **Client ID**
- **Client Secret**

## 2) Create an Application in authentik

- **Applications → Applications → Create**
  - Name: `Mamey Portal`
  - Slug: `mamey-portal`
  - Provider: the provider you created above

## 3) Add required claims (roles + tenant)

The portal expects:
- **role claims** (for `[Authorize(Roles="...")]`)
- a **tenant claim** (defaults to claim type `tenant`)

Configure the provider to include:
- `roles`: list of roles (`Admin`, `Citizen`, `GovernmentAgent`, `ContentEditor`, `LibraryEditor`)
- `tenant`: tenant id (e.g. `default`)

Implementation detail: `Auth:Oidc:RoleClaimType` defaults to `roles`, and `Auth:TenantClaimType` defaults to `tenant`.

## 4) Switch portal to OIDC mode

Edit:
- `src/Mamey.Portal.Web/appsettings.Development.json`

Set:

```json
{
  "Auth": {
    "Mode": "Oidc",
    "TenantClaimType": "tenant",
    "Oidc": {
      "Authority": "http://localhost:9100/application/o/mamey-portal/",
      "ClientId": "mamey-portal",
      "ClientSecret": "PASTE_FROM_AUTHENTIK",
      "RoleClaimType": "roles",
      "NameClaimType": "preferred_username",
      "RequireHttpsMetadata": false
    }
  }
}
```

Note: the Authority must end with a trailing `/` for authentik.

## 5) Verify login

- Open `http://localhost:5180/auth/login`
  - In OIDC mode the page will force-load to `/auth/oidc/login` and redirect you to authentik.

After login, you should land back at the portal and see your user menu populated.

## Test logins

See: `docs/testing-logins.md`


