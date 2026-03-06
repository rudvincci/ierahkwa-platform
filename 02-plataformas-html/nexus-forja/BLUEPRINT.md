# NEXUS Forja — Technical Blueprint

**Version**: 1.0.0 | **Date**: March 2026

## Architecture Diagram

```
┌──────────────────────────────────────────────────┐
│         DEVELOPER INTERFACES                      │
│  IDE Soberano · Git UI · CI Dashboard · Terminal  │
└──────────┬───────────────────────────────────────┘
           │
┌──────────┴───────────────────────────────────────┐
│       DEV SERVICES (101 platforms)                │
│  ┌────────┐ ┌────────┐ ┌────────┐ ┌──────────┐ │
│  │  Git   │ │ CI/CD  │ │Testing │ │ Registry │ │
│  │ Server │ │ Engine │ │ Suite  │ │Container │ │
│  └────────┘ └────────┘ └────────┘ └──────────┘ │
│  ┌────────┐ ┌────────┐ ┌────────┐ ┌──────────┐ │
│  │  IDE   │ │ Agent  │ │  SAST  │ │ Docs Gen │ │
│  │Backend │ │Soberano│ │Scanner │ │ Engine   │ │
│  └────────┘ └────────┘ └────────┘ └──────────┘ │
└──────────┬───────────────────────────────────────┘
           │
┌──────────┴───────────────────────────────────────┐
│       DATA STORES                                 │
│  PostgreSQL · Redis · S3 (artifacts) · MinIO      │
│  MameyNode (commit hashes) · Elasticsearch        │
└──────────────────────────────────────────────────┘
```

## API Design

```
POST /api/v1/forja/repo/create           Create git repository
POST /api/v1/forja/ci/trigger            Trigger CI pipeline
GET  /api/v1/forja/ci/{id}/status        Pipeline status
POST /api/v1/forja/scan/sast             Run SAST scan
POST /api/v1/forja/registry/push         Push container image
GET  /api/v1/forja/packages              List packages
POST /api/v1/forja/agent/assist          AI coding assistance
```
