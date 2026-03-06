# Presentaciones Soberano

> Sovereign Presentation Platform for the Ierahkwa Ne Kanienke Digital Nation

## Overview

Presentaciones Soberano is a sovereign, enterprise-grade presentation creation platform serving 72M+ indigenous people across 19 nations and 574 tribal nations. It replaces PowerPoint/Google Slides with zero Big Tech dependencies.

## Features

- **WYSIWYG Slide Editor** — Drag-and-drop with smart alignment, layers, and snapping
- **200+ Sovereign Templates** — Indigenous-themed professional, government, and educational designs
- **Animations & Transitions** — CSS/WebGL animation engine with editable keyframe timelines
- **Multi-Format Export** — PDF, PPTX, HTML, PNG, MP4 video export
- **Real-Time Collaboration** — CRDT-based co-editing with live cursors and comments
- **AI Slide Assistant** — Auto-generate slides from text, suggest layouts, translate to 14 indigenous languages
- **Charts & Data Visualization** — Bar, line, pie, radar, maps, live data from sovereign APIs
- **Multimedia Embedding** — Video, audio, GIF, iframe, and 3D content support
- **Presenter Mode** — Speaker notes, timer, laser pointer, mobile remote control
- **Offline Support** — Full Service Worker PWA with automatic sync on reconnect

## Architecture

```
Browser (PWA)
  ├── Canvas Rendering Engine (WebAssembly)
  ├── CRDT Collaboration Layer
  ├── Offline Storage (IndexedDB)
  └── AI Assistant (Local inference)
        │
   API Gateway (mTLS + JWT + Rate Limiting)
        │
   ┌────┼────────────────┐
   │    │                 │
Slide   Template    Export
Engine  Service     Service
   │    │                 │
   └────┼────────────────┘
        │
   PostgreSQL + Redis + S3 + MameyNode
```

## NEXUS

Part of **NEXUS Escritorio** (Desktop Apps) — Color: `#26C6DA`

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | HTML5 Canvas, WebAssembly, Service Worker, PWA |
| Backend | Rust + Go microservices, gRPC, WebSocket |
| Database | PostgreSQL, Redis, S3-compatible storage |
| Blockchain | MameyNode sovereign chain |
| Encryption | Kyber-768 post-quantum, E2E |
| AI | Local inference, sovereign ML models |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/presentaciones/create` | Create presentation |
| GET | `/api/v1/presentaciones/{id}` | Get presentation |
| POST | `/api/v1/presentaciones/{id}/slides` | Add slide |
| POST | `/api/v1/presentaciones/{id}/export/{format}` | Export to format |
| GET | `/api/v1/presentaciones/templates` | List templates |
| POST | `/api/v1/presentaciones/{id}/ai/generate` | AI slide generation |

## Deployment

```bash
docker-compose -f docker-compose.sovereign.yml up presentaciones-soberano
```

## Contributing

See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

## License

Sovereign License — Ierahkwa Ne Kanienke Digital Nation
