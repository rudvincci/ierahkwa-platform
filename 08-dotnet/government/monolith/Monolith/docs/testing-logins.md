# Testing logins (dev)

This repo supports **two** ways to log in during development:

- **Mock mode** (default): cookie-backed dev session via `/dev/mock-login`
- **OIDC mode**: authentik OIDC login via `/auth/oidc/login`

Portal default URL: `https://localhost:7295` (HTTPS) or `http://localhost:5075` (HTTP)

## Mock mode (fastest for UI/dev)

Mock mode is active when `Auth:Mode` is **not** `Oidc` (default is `Mock`).

Use the dev helper endpoint (no password):

`/dev/mock-login?tenant=default&user=you@example.com&role=Citizen`

### Ready-to-copy examples

- **Citizen**: `https://localhost:7295/dev/mock-login?tenant=default&user=citizen@example.com&role=Citizen`
- **Government Agent**: `https://localhost:7295/dev/mock-login?tenant=default&user=agent@example.com&role=GovernmentAgent`
- **Admin**: `https://localhost:7295/dev/mock-login?tenant=default&user=admin@example.com&role=Admin`
- **Content Editor**: `https://localhost:7295/dev/mock-login?tenant=default&user=editor@example.com&role=ContentEditor`
- **Library Editor**: `https://localhost:7295/dev/mock-login?tenant=default&user=library@example.com&role=LibraryEditor`

After setting the cookie, hit:

- **Who am I** (dev): `https://localhost:7295/dev/whoami`

## OIDC mode (authentik)

OIDC mode is active when `Auth:Mode` is `Oidc`.

Login entrypoint:

- `https://localhost:7295/auth/login` (Blazor login page)
  - will force-load to `https://localhost:7295/auth/oidc/login` in OIDC mode

Logout:

- `https://localhost:7295/auth/logout` (Blazor logout)
  - will hit `https://localhost:7295/auth/oidc/logout` in OIDC mode

### Roles used by the portal

These role names must match what authentik emits (typically from **group membership**):

- `Admin`
- `GovernmentAgent`
- `Citizen`
- `ContentEditor`
- `LibraryEditor`

### Suggested test users (create in authentik)

Create users in authentik and assign them to the matching group(s):

- **portal-admin** → `Admin`  
  - Password: `PortalAdmin!2026`
- **portal-agent** → `GovernmentAgent`  
  - Password: `PortalAgent!2026`
- **portal-citizen** → `Citizen`  
  - Password: `PortalCitizen!2026`
- **portal-editor** → `ContentEditor`  
  - Password: `PortalEditor!2026`
- **portal-library** → `LibraryEditor`  
  - Password: `PortalLibrary!2026`

These are **dev-only** passwords. If you changed them in authentik, use your updated values instead.

#### Reset all authentik dev passwords (scripted)

If you want to reset them back to the defaults above, run:

```bash
docker exec -i mamey-authentik-server ak shell <<'PY'
from authentik.core.models import User
from django.contrib.auth.models import Group

ROLE_GROUPS = ["Admin", "GovernmentAgent", "Citizen", "ContentEditor", "LibraryEditor"]
def ensure_group(name: str) -> Group:
    g, _ = Group.objects.get_or_create(name=name)
    return g
groups = {name: ensure_group(name) for name in ROLE_GROUPS}

def ensure_user(username: str, email: str, password: str, group_names: list[str]):
    u, _ = User.objects.get_or_create(username=username, defaults={"email": email, "name": username})
    u.email = email
    u.name = username
    u.is_active = True
    u.set_password(password)
    u.save()
    u.groups.clear()
    for gn in group_names:
        u.groups.add(groups[gn])
    return u

ensure_user("portal-admin", "portal-admin@example.com", "PortalAdmin!2026", ["Admin"])
ensure_user("portal-agent", "portal-agent@example.com", "PortalAgent!2026", ["GovernmentAgent"])
ensure_user("portal-citizen", "portal-citizen@example.com", "PortalCitizen!2026", ["Citizen"])
ensure_user("portal-editor", "portal-editor@example.com", "PortalEditor!2026", ["ContentEditor"])
ensure_user("portal-library", "portal-library@example.com", "PortalLibrary!2026", ["LibraryEditor"])

print("OK")
PY
```

### Tenant selection in OIDC

Tenant can come from:

- an OIDC claim (default claim type is `tenant`), **or**
- an admin-managed mapping in the portal: `https://localhost:7295/gov/user-tenant-mappings`
  - use **"Use my identity"** while logged in to prefill (Issuer + Subject)

If your resolved tenant doesn't exist:

- **Admin** users are redirected to: `https://localhost:7295/gov/tenants?prefillTenantId=...`
- non-admin users are redirected to: `/auth/login?error=tenant_unknown`

### Quick verification

After a successful OIDC login, verify:

- `https://localhost:7295/dev/whoami`

You should see:

- `roles`: includes the expected roles
- `tenant`: matches an existing tenant id in Postgres


