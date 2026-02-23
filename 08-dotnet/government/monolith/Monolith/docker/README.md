# Mamey Government Portal - Docker Setup

This directory contains the Docker configuration for running the Mamey Government Portal application.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        mamey network                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────┐    ┌──────────────────┐                  │
│  │  Government      │    │   Authentik      │                  │
│  │  Portal          │◄───│   (OIDC)         │                  │
│  │  :7295 (HTTPS)   │    │   :9444 (HTTPS)  │                  │
│  │  :5075 (HTTP)    │    │   :9100 (HTTP)   │                  │
│  └────────┬─────────┘    └──────────────────┘                  │
│           │                                                     │
│           ▼                                                     │
│  ┌──────────────────────────────────────────────────────┐      │
│  │              Infrastructure Services                  │      │
│  ├──────────┬──────────┬──────────┬──────────┬─────────┤      │
│  │ Postgres │  Mongo   │  Redis   │   Seq    │  Vault  │      │
│  │  :5432   │  :27017  │  :6379   │  :5341   │  :8200  │      │
│  └──────────┴──────────┴──────────┴──────────┴─────────┘      │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Quick Start

### 1. Create the shared network

```bash
docker network create mamey 2>/dev/null || true
```

### 2. Start Infrastructure Services

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith/docker
docker compose -f infrastructure.yml up -d
```

### 3. Start Authentik (Identity Provider)

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith
docker compose -f docker-compose.authentik.console.yml up -d
```

### 4. Start the Government Portal Application

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Mamey.Government/Monolith
docker compose -f docker-compose.app.yml up -d --build
```

## Access URLs

| Service | URL | Description |
|---------|-----|-------------|
| Government Portal (HTTPS) | https://localhost:7295 | Main application |
| Government Portal (HTTP) | http://localhost:5075 | HTTP redirect |
| Health Check | http://localhost:5075/health | Application health |
| Authentik | https://localhost:9444 | Identity Provider UI |
| Seq Logs | http://localhost:5341 | Logging dashboard |
| PostgreSQL | localhost:5432 | Database |
| MongoDB | localhost:27017 | Document store |
| Redis | localhost:6379 | Cache |
| Vault | http://localhost:8200 | Secrets management |
| MailHog | http://localhost:8025 | Email testing UI |

## HTTPS Certificates

The application uses self-signed certificates for HTTPS in development.

### Certificate Details

| Property | Value |
|----------|-------|
| **Subject** | `CN=localhost, O=Mamey Government, C=US` |
| **Valid From** | Jan 15, 2026 |
| **Valid Until** | Dec 22, 2125 (100 years) |
| **Password** | `supersecret` |
| **Location** | `certs/localhost.pfx` |

### Subject Alternative Names (SANs)

- `localhost`
- `*.localhost`
- `government.mamey.local`
- `*.mamey.local`
- `127.0.0.1`

### Certificate Files

```
certs/
├── localhost.crt   # X.509 Certificate
├── localhost.key   # Private Key
├── localhost.pfx   # PKCS#12 Bundle (password protected)
├── localhost.pem   # Combined cert + key (PEM format)
└── localhost.pub   # Public Key
```

### Regenerating Certificates

If you need to regenerate the certificates:

```bash
./scripts/generate-cert.sh
```

Or manually with OpenSSL:

```bash
cd certs

# Generate certificate (100 year validity)
openssl req -x509 -nodes -days 36500 -newkey rsa:2048 \
    -keyout localhost.key \
    -out localhost.crt \
    -subj "/CN=localhost/O=Mamey Government/C=US" \
    -addext "subjectAltName=DNS:localhost,DNS:*.localhost,IP:127.0.0.1"

# Convert to PFX
openssl pkcs12 -export -out localhost.pfx \
    -inkey localhost.key \
    -in localhost.crt \
    -passout pass:supersecret
```

## Docker Compose Files

| File | Description |
|------|-------------|
| `docker-compose.app.yml` | Main application container |
| `docker-compose.authentik.console.yml` | Authentik identity provider |
| `docker/infrastructure.yml` | Infrastructure services (Postgres, Mongo, Redis, etc.) |
| `docker/infrastructure.development.yml` | Development infrastructure (includes MailHog) |

## Environment Variables

Copy `docker/docker.env.example` to `.env` and configure:

```bash
# Application Ports
APP_HTTPS_PORT=7295
APP_HTTP_PORT=5075
POSTGRES_PORT=5432

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Docker

# Certificate Password
CERT_PASSWORD=supersecret

# Authentik OIDC Configuration
AUTHENTIK_AUTHORITY=https://localhost:9444/application/o/mamey-government/
AUTHENTIK_CLIENT_ID=mamey-government-portal
AUTHENTIK_CLIENT_SECRET=your-client-secret-here
```

## Useful Commands

### View logs

```bash
# Application logs
docker logs -f mamey-government-portal

# All services
docker compose -f docker-compose.app.yml logs -f
```

### Rebuild and restart

```bash
docker compose -f docker-compose.app.yml up -d --build --force-recreate
```

### Stop all services

```bash
docker compose -f docker-compose.app.yml down
docker compose -f docker-compose.authentik.console.yml down
docker compose -f docker/infrastructure.yml down
```

### Clean up volumes

```bash
docker compose -f docker-compose.app.yml down -v
```

### Check health

```bash
curl http://localhost:5075/health
```

## Troubleshooting

### Certificate not trusted

For development, you may need to trust the self-signed certificate:

**macOS:**
```bash
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain certs/localhost.crt
```

**Windows (PowerShell as Admin):**
```powershell
Import-Certificate -FilePath .\certs\localhost.crt -CertStoreLocation Cert:\LocalMachine\Root
```

### Connection refused to database

Ensure infrastructure services are running:
```bash
docker compose -f docker/infrastructure.yml ps
```

### Authentik not responding

Check Authentik logs:
```bash
docker logs mamey-authentik-server
docker logs mamey-authentik-worker
```

### Application won't start

Check the application logs:
```bash
docker logs mamey-government-portal
```

Common issues:
- Database not ready (wait for health check)
- Certificate password mismatch
- Network not created

## Database Connections

### PostgreSQL

```
Host: localhost (or postgres from Docker)
Port: 5432
Database: mamey_government
Username: admin
Password: secret
```

### MongoDB

```
Connection String: mongodb://root:secret@localhost:27017/?authSource=admin
Database: mamey_government
```

### Redis

```
Host: localhost (or redis from Docker)
Port: 6379
```
