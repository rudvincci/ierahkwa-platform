# Admin Dashboard — Technical Blueprint

**Version**: 1.0.0 | **Date**: March 2026

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                ADMIN DASHBOARD UI                        │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────┐ │
│  │ NEXUS    │ │ User     │ │ Security │ │ Blockchain│ │
│  │ Monitor  │ │ Manager  │ │ Center   │ │ Monitor   │ │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └─────┬─────┘ │
└───────┼─────────────┼───────────┼──────────────┼────────┘
        │             │           │              │
┌───────┼─────────────┼───────────┼──────────────┼────────┐
│  API Gateway (JWT + RBAC + mTLS + Rate Limit)           │
└───────┼─────────────┼───────────┼──────────────┼────────┘
        │             │           │              │
┌───────┼─────────────┼───────────┼──────────────┼────────┐
│  MICROSERVICES                                           │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────┐ │
│  │ Platform │ │ Identity │ │ Threat   │ │ Chain     │ │
│  │ Health   │ │ & Access │ │ Detector │ │ Explorer  │ │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └─────┬─────┘ │
└───────┼─────────────┼───────────┼──────────────┼────────┘
        │             │           │              │
┌───────┼─────────────┼───────────┼──────────────┼────────┐
│  DATA STORES                                             │
│  PostgreSQL · TimescaleDB · Redis · MameyNode Blockchain │
└──────────────────────────────────────────────────────────┘
```

## Data Flow

```
425+ Platforms → Health Probes → TimescaleDB → Dashboard UI
                                     │
7 AI Agents → Agent Metrics ─────────┘
                                     │
84 Microservices → Service Mesh ─────┘
                                     │
MameyNode → Block Explorer ──────────┘
```

## API Design

```
GET  /api/v1/admin/nexus                    List all 18 NEXUS status
GET  /api/v1/admin/nexus/{id}/platforms     Platforms in NEXUS
GET  /api/v1/admin/platforms/{id}/health    Platform health metrics
GET  /api/v1/admin/users                    List users (paginated)
POST /api/v1/admin/users/{id}/role          Assign role
GET  /api/v1/admin/security/threats         Active threats
GET  /api/v1/admin/security/audit           Audit trail
GET  /api/v1/admin/blockchain/status        MameyNode status
GET  /api/v1/admin/agents                   AI agent status
```

## Deployment

```
Kubernetes (3 replicas)
├── admin-dashboard-web (Nginx + SPA)
├── admin-api (Go, 3 replicas)
├── health-monitor (Rust, 1 replica per node)
├── identity-service (Go, 2 replicas)
├── threat-detector (Python/ML, 2 replicas)
└── chain-explorer (Rust, 1 replica)
```
