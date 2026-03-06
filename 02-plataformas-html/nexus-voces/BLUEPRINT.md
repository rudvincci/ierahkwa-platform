# NEXUS Voces — Technical Blueprint

**Version**: 1.0.0 | **Date**: March 2026

## Architecture Diagram

```
┌─────────────────────────────────────────────┐
│          CLIENT APPS                         │
│  Web PWA · iOS · Android · Desktop           │
└──────────┬──────────────────────────────────┘
           │ WebSocket + REST + Matrix
┌──────────┴──────────────────────────────────┐
│       COMMUNICATION SERVICES                 │
│  ┌────────┐ ┌────────┐ ┌────────┐          │
│  │Social  │ │Message │ │Video   │          │
│  │Network │ │Service │ │Confrnc │          │
│  │(Activ.)│ │(Matrix)│ │(Jitsi) │          │
│  └────────┘ └────────┘ └────────┘          │
└──────────┬──────────────────────────────────┘
           │
┌──────────┴──────────────────────────────────┐
│       DATA + FEDERATION                      │
│  PostgreSQL · Redis · S3 · ActivityPub       │
│  MameyNode (content hashes) · IPFS (media)   │
└──────────────────────────────────────────────┘
```

## API Design

```
POST /api/v1/voces/post/create           Create social post
GET  /api/v1/voces/feed                  Get user feed
POST /api/v1/voces/message/send          Send encrypted message
POST /api/v1/voces/conference/start      Start video call
GET  /api/v1/voces/community/{id}        Get community
POST /api/v1/voces/stream/start          Start live stream
```
