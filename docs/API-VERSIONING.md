# API Versioning

## Current Version
- API Version: 1.0.0
- Platform Version: 3.3.0

## Versioning Strategy
All APIs use URL path versioning: `/api/v1/resource`

## Endpoints by NEXUS

| NEXUS | Base Port | Services | OpenAPI Spec |
|-------|-----------|----------|-------------|
| Gateway | 5000 | 1 | gateway.yml |
| Orbital | 5100-5107 | 8 | orbital-api.yml |
| Escudo | 5200-5204 | 5 | escudo-api.yml |
| Cerebro | 5300-5303 | 4 | cerebro-api.yml |
| Tesoro | 5400-5407 | 8 | tesoro-api.yml |
| Voces | 5500-5504 | 5 | voces-api.yml |
| Consejo | 5600-5604 | 5 | consejo-api.yml |
| Tierra | 5700-5704 | 5 | tierra-api.yml |
| Forja | 5800-5804 | 5 | forja-api.yml |
| Urbe | 5900-5903 | 4 | urbe-api.yml |
| Raices | 6000-6003 | 4 | raices-api.yml |
| AI Models | 5050 | 1 | ai-models-api.yml |

## Common Headers
All API responses include:
- `X-Request-Id`: UUID correlation ID for tracing
- `X-RateLimit-Limit`: Max requests per window
- `X-RateLimit-Remaining`: Remaining requests
- `X-RateLimit-Reset`: Seconds until reset

## Rate Limits
| Tier | Requests/min | Burst |
|------|-------------|-------|
| Public | 30 | 50 |
| Citizen | 100 | 200 |
| Admin | 500 | 1000 |
| Service | 5000 | 10000 |

## Authentication
All endpoints (except /health) require JWT Bearer token:
```
Authorization: Bearer <token>
```
Tokens are RS256 signed with tier-based claims.

## Deprecation Policy
- Minimum 6 months notice before API version deprecation
- Sunset header added when version is deprecated
- Migration guide provided for each version change
