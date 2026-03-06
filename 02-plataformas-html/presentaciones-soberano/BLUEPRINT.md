# Presentaciones Soberano — Technical Blueprint

**Version**: 1.0.0 | **Date**: March 2026

## Architecture Diagram

```
┌──────────────────────────────────────────────────────────┐
│                    CLIENT LAYER                           │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────────┐ │
│  │  Canvas   │ │   CRDT   │ │ Offline  │ │    AI      │ │
│  │ Renderer  │ │  Engine  │ │ Storage  │ │ Assistant  │ │
│  │ (WASM)   │ │ (Yjs)    │ │(IndexDB) │ │ (ONNX.js)  │ │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └─────┬──────┘ │
│       └──────┬──────┘            │              │        │
│              │ WebSocket         │ Service      │ HTTP   │
│              │                   │ Worker       │        │
└──────────────┼───────────────────┼──────────────┼────────┘
               │                   │              │
┌──────────────┼───────────────────┼──────────────┼────────┐
│         API GATEWAY              │              │        │
│  ┌───────────────────────────────┼──────────────┼──────┐ │
│  │  Kong / Traefik               │              │      │ │
│  │  JWT Auth · Rate Limit · mTLS │              │      │ │
│  │  Circuit Breaker · CORS       │              │      │ │
│  └───────────┬───────────────────┘              │      │ │
│              │                                  │      │ │
└──────────────┼──────────────────────────────────┼──────┘ │
               │                                  │        │
┌──────────────┼──────────────────────────────────┼────────┐
│         MICROSERVICES LAYER                              │
│  ┌─────────┐ ┌──────────┐ ┌─────────┐ ┌──────────────┐ │
│  │ Slide   │ │ Template │ │ Export  │ │ Collaboration│ │
│  │ Engine  │ │ Service  │ │ Service │ │ Service      │ │
│  │ (Rust)  │ │ (Go)     │ │ (Rust)  │ │ (Go)         │ │
│  └────┬────┘ └────┬─────┘ └────┬────┘ └──────┬───────┘ │
│       └──────┬────┘             │              │        │
│              │                  │              │        │
└──────────────┼──────────────────┼──────────────┼────────┘
               │                  │              │
┌──────────────┼──────────────────┼──────────────┼────────┐
│         DATA LAYER                                       │
│  ┌──────────┐ ┌───────┐ ┌─────┐ ┌──────────────────┐   │
│  │PostgreSQL│ │ Redis │ │ S3  │ │ MameyNode Chain  │   │
│  │(slides)  │ │(cache)│ │(img)│ │ (audit trail)    │   │
│  └──────────┘ └───────┘ └─────┘ └──────────────────┘   │
└──────────────────────────────────────────────────────────┘
```

## Component Interaction

```
User creates slide
       │
       ▼
Canvas Renderer ──→ CRDT Engine ──→ WebSocket ──→ Collaboration Service
       │                                                    │
       ▼                                                    ▼
  IndexedDB ◄──── Service Worker                    PostgreSQL + Redis
  (offline)         (sync)                          (persistent)
       │                                                    │
       └────────── Sync on reconnect ──────────────────────→┘
```

## Export Pipeline

```
Presentation Data
       │
       ▼
┌──────────────────┐
│  Export Service   │
│  ┌────────────┐  │
│  │ PDF Engine │  │──→ PDF (Typst renderer)
│  ├────────────┤  │
│  │PPTX Engine │  │──→ PPTX (OpenXML SDK)
│  ├────────────┤  │
│  │HTML Engine │  │──→ HTML (static site)
│  ├────────────┤  │
│  │ PNG Engine │  │──→ PNG (per-slide raster)
│  ├────────────┤  │
│  │MP4 Engine  │  │──→ MP4 (FFmpeg + narration)
│  └────────────┘  │
└──────────────────┘
```

## API Design

### REST Endpoints

```
POST   /api/v1/presentaciones                    Create presentation
GET    /api/v1/presentaciones/{id}               Get presentation
PUT    /api/v1/presentaciones/{id}               Update metadata
DELETE /api/v1/presentaciones/{id}               Delete presentation

POST   /api/v1/presentaciones/{id}/slides        Add slide
PUT    /api/v1/presentaciones/{id}/slides/{num}  Update slide
DELETE /api/v1/presentaciones/{id}/slides/{num}  Remove slide
POST   /api/v1/presentaciones/{id}/slides/reorder Reorder slides

POST   /api/v1/presentaciones/{id}/export/{fmt}  Export presentation
GET    /api/v1/presentaciones/{id}/export/status  Check export status

GET    /api/v1/presentaciones/templates           List templates
GET    /api/v1/presentaciones/templates/{id}      Get template

POST   /api/v1/presentaciones/{id}/ai/generate   AI slide generation
POST   /api/v1/presentaciones/{id}/ai/translate   AI translation

POST   /api/v1/presentaciones/{id}/share         Share with users
GET    /api/v1/presentaciones/{id}/collaborators  List collaborators
```

### WebSocket Events

```
ws://api/v1/presentaciones/{id}/collab

Events:
  slide:update    — Slide content changed
  cursor:move     — Collaborator cursor moved
  user:join       — User joined session
  user:leave      — User left session
  comment:add     — New comment added
```

## Deployment Topology

```
┌─────────────────────────────────────────┐
│          Kubernetes Cluster              │
│  ┌───────────────────────────────────┐  │
│  │     Sovereign Infrastructure      │  │
│  │  ┌─────────┐ ┌─────────────────┐ │  │
│  │  │ Ingress │ │ Cert Manager    │ │  │
│  │  └────┬────┘ └─────────────────┘ │  │
│  │       │                           │  │
│  │  ┌────┴────┐  Replicas: 3        │  │
│  │  │ Gateway │                      │  │
│  │  └────┬────┘                      │  │
│  │       │                           │  │
│  │  ┌────┴────────────────────┐      │  │
│  │  │  slide-engine (x3)     │      │  │
│  │  │  template-svc (x2)     │      │  │
│  │  │  export-svc (x2)       │      │  │
│  │  │  collab-svc (x3)       │      │  │
│  │  └────┬────────────────────┘      │  │
│  │       │                           │  │
│  │  ┌────┴────────────────────┐      │  │
│  │  │ PostgreSQL (HA)        │      │  │
│  │  │ Redis Sentinel (x3)    │      │  │
│  │  │ MinIO S3 (x3)          │      │  │
│  │  │ MameyNode (x5 validators)│    │  │
│  │  └─────────────────────────┘      │  │
│  └───────────────────────────────────┘  │
└─────────────────────────────────────────┘
```

## Security Architecture

```
Client ──TLS 1.3──→ Gateway ──mTLS──→ Services
  │                    │                  │
  │ E2E Kyber-768      │ JWT + RBAC       │ Service mesh
  │ encryption         │ validation       │ (Istio)
  │                    │                  │
  └── Zero-knowledge ──┘──── Audit ───────→ MameyNode
      architecture           logging        Blockchain
```
