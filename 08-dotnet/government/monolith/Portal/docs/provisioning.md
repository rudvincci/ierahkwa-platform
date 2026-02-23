## Provisioning (SaaS Tenant Bootstrap)

This repo ships a small provisioning console app to create/update **real go-live tenants** (e.g., `ink`, `bor`) in Postgres in an **idempotent** way.

### What it does

- Creates or updates:
  - `tenants`
  - `tenant_settings`
  - `tenant_document_naming`
  - optional `tenant_document_templates`
- Optionally creates a single **sample** citizenship application per tenant (for testing template previews).

### Run (dry-run)

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal
dotnet run --project src/Mamey.Portal.Provisioning -- --seed-file provisioning/seed-tenants.json
```

### Run (apply)

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal
dotnet run --project src/Mamey.Portal.Provisioning -- --seed-file provisioning/seed-tenants.json --apply
```

### Optional: create sample applications (per-tenant)

This is useful for `/gov/tenants` “Real” preview dropdowns.

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Portal
dotnet run --project src/Mamey.Portal.Provisioning -- --seed-file provisioning/seed-tenants.json --apply --seed-sample-applications
```

### Seeding tenant document templates

In `provisioning/seed-tenants.json`, use `templateFiles` (recommended) to reference HTML files on disk:

- Keys are template kinds, e.g. `CitizenshipCertificate`, `Passport`, `IdCard`, `VehicleTag`, or variants like `IdCard:MedicinalCannabis`.
- Values are paths **relative to the seed file**.

This repo includes baseline templates under `provisioning/templates/`.

### Seeding first-login tenant invites (OIDC)

For SaaS onboarding you often don’t know a user’s OIDC `sub` until their first login.
To avoid manual mapping, you can pre-provision **invites by email**.

- In the seed file, set `userInvites` for a tenant:
  - `[{ "email": "portal-admin@example.com" }, ...]`
- On the user’s **first login**, the portal will:
  - find a matching invite by `(issuer,email)`,
  - create the real `(issuer,sub) -> tenantId` mapping,
  - mark the invite as used.

The provisioning tool uses the OIDC issuer from web config by default (`Auth:Oidc:Authority`). You can override with `--issuer`.

### Connection string resolution

The tool resolves `PortalDb` in this order:

1. `--portaldb "..."` (explicit override)
2. `ConnectionStrings__PortalDb` env var
3. Web config files:
   - `src/Mamey.Portal.Web/appsettings.json`
   - `src/Mamey.Portal.Web/appsettings.{ENV}.json`
   - `src/Mamey.Portal.Web/appsettings.Local.json`
   - `src/Mamey.Portal.Web/appsettings.{ENV}.Local.json`


