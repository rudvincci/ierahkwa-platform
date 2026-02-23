# Authentik (local) – Phase 4.5

We run authentik locally via Docker Desktop as a separate stack from:
- the portal app (run with `dotnet run`)
- the portal infrastructure stack (`docker-compose.infrastructure.console.yml`)

This matches authentik’s recommended Docker Compose install flow for test/small setups.  
See authentik docs: https://docs.goauthentik.io/install-config/install/docker-compose/

## Why ports are different

Our portal infra compose already uses:
- MinIO: `9000` (API) and `9001` (console)

authentik defaults to `9000/9443`, so we expose it as:
- `9100 -> 9000` (HTTP)
- `9444 -> 9443` (HTTPS)

## Start authentik

From `Mamey.Government/Portal/`:

1) Create your env file:

- Copy `authentik.env.example` → `authentik.env`
- Generate secrets (per authentik docs):

```bash
cp authentik.env.example authentik.env
echo "AUTHENTIK_PG_PASS=$(openssl rand -base64 36 | tr -d '\n')" >> authentik.env
echo "AUTHENTIK_SECRET_KEY=$(openssl rand -base64 60 | tr -d '\n')" >> authentik.env
```

2) Start containers:

```bash
docker compose -f docker-compose.authentik.console.yml --env-file authentik.env up -d
```

3) Initial setup (note the trailing `/`):

`http://localhost:9100/if/flow/initial-setup/`

Authentik docs note: you will get **Not Found** if you omit the trailing `/`.  
See: https://docs.goauthentik.io/install-config/install/docker-compose/

## Next (wiring the portal)

Phase 4.5 will switch the portal from mock auth to OIDC:
- Create an authentik **OAuth2/OpenID Provider** + **Application**
- Configure redirect URIs for the portal (Blazor Server)
- Configure role/tenant claims mapping

See: `docs/authentik-portal-oidc.md`

## Test logins

See: `docs/testing-logins.md`


