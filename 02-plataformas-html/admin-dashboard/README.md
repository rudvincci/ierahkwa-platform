# Admin Dashboard — Ierahkwa Ne Kanienke

> Central administration panel for the sovereign digital nation

## Overview

The Admin Dashboard provides centralized monitoring and management of all 425+ platforms across 18 NEXUS mega-portals serving 72M+ indigenous people across 19 nations and 574 tribal nations.

## Features

- **Real-Time Monitoring** — Live metrics from all 18 NEXUS portals
- **Platform Health** — Uptime, latency, error rates per platform
- **User Management** — Roles, permissions, tribal nation assignments
- **Blockchain Monitor** — MameyNode validator status, IGT token metrics
- **AI Agent Dashboard** — Status of 7 sovereign AI agents across 400+ platforms
- **Security Center** — Threat detection, forensic logs, trust scores
- **NEXUS Map** — Visual topology of all interconnected portals
- **Audit Trail** — Immutable blockchain-based activity logging

## Architecture

```
Admin UI (PWA)
    │
API Gateway (JWT + RBAC + mTLS)
    │
┌───┼──────────────┐
│   │               │
Monitor  User Mgmt  Blockchain
Service  Service     Monitor
│   │               │
└───┼──────────────┘
    │
PostgreSQL + Redis + MameyNode + TimescaleDB
```

## Access Control

| Role | Permissions |
|------|-----------|
| Super Admin | Full system access, NEXUS management |
| NEXUS Admin | Manage platforms within assigned NEXUS |
| Tribal Admin | Manage users within tribal nation |
| Auditor | Read-only access to all logs and metrics |

## Deployment

```bash
docker-compose -f docker-compose.sovereign.yml up admin-dashboard
```

## License

Sovereign License — Ierahkwa Ne Kanienke Digital Nation
