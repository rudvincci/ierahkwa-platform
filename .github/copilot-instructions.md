# Copilot Instructions — Ierahkwa Sovereign Digital Platform

## Project Context

This is the Ierahkwa Sovereign Digital Platform — a complete sovereign technology ecosystem serving 72 million indigenous people worldwide. It replaces Big Tech dependencies with 49 self-hosted platforms and 20 backend services.

**Stack:** Node.js 22, .NET 10, Rust 1.80+, Go 1.22, PostgreSQL 16, Redis 7, Docker 27
**Blockchain:** MameyNode (Rust-based, EVM Shanghai, Chain ID 777777, 3s blocks, 12,847 TPS)
**Token:** WAMPUM (WMP) — 720M total supply, 0% tax

## Architecture Patterns

### Service Structure
All Node.js backend services follow this pattern:
```
03-backend/<service-name>/
├── server.js          # Express app with routes
├── package.json       # Dependencies
├── Dockerfile         # Multi-stage Node 22 Alpine
└── __tests__/
    ├── health.test.js # Health endpoint tests
    └── api.test.js    # API endpoint tests
```

### Security Middleware (MANDATORY)
Every Node.js service MUST import and use the shared security middleware:
```javascript
const { corsConfig, applySecurityMiddleware, errorHandler } = require('../shared/security');
applySecurityMiddleware(app);
app.use(errorHandler);
```

### Accessibility Middleware
Services serving HTML SHOULD import accessibility middleware:
```javascript
const { applyAccessibilityMiddleware } = require('../shared/accessibility');
applyAccessibilityMiddleware(app);
```

### Naming Conventions
- Platforms use sovereign naming: `*-soberano` (masculine) or `*-soberana` (feminine)
- Backend services match their platform name
- Port assignments: 3000-3099 range for Node.js services
- All user-facing text in English (backend), Spanish names for directories

### Express Patterns
- Use `express.json({ limit: '10mb' })`
- Use `helmet()` for security headers
- Use `compression()` for response compression
- Rate limiting via shared security middleware
- Health endpoint at `GET /health` (required)
- API prefix: `/api/v1/` for versioned endpoints

### Error Handling
```javascript
app.use((err, req, res, next) => {
  const status = err.status || 500;
  res.status(status).json({
    error: { status, message: status < 500 ? err.message : 'Internal server error', code: err.code || `ERR_${status}` }
  });
});
```

### In-Memory Storage Pattern
For services without database dependency:
```javascript
const store = new Map();
const { v4: uuid } = require('uuid');
// CRUD operations use Map.set(), Map.get(), Map.delete()
```

### Testing Patterns
- Framework: Jest with `testEnvironment: 'node'`
- Test structure: `describe()` blocks per endpoint group
- Validate module exports, route definitions, config values
- Tests should NOT require running server (validate code structure)
- Use `require()` to import and inspect modules

### Docker Patterns
- Base: `node:22-alpine`
- Multi-stage builds for production
- Non-root user: `node`
- Health check: `CMD ["node", "-e", "require('http').get('http://localhost:PORT/health')"]`
- Expose only necessary ports

## Code Quality Rules

1. **Always use `'use strict'`** at the top of every JS file
2. **Never hardcode secrets** — use environment variables
3. **Never use `*` for CORS origins** — use whitelist
4. **Always validate input** — sanitize user data
5. **Never expose stack traces** in production errors
6. **Always include Content-Security-Policy** headers
7. **Use UUID v4** for all generated IDs
8. **Sovereign values**: Zero tracking, zero ads, zero taxes, 92% revenue to providers

## Accessibility Requirements (GAAD Pledge)

- All HTML must have `lang` attribute
- All images must have `alt` text
- All pages need skip navigation links
- Use semantic HTML (`<main>`, `<nav>`, `<header>`, `<footer>`)
- Minimum 4.5:1 color contrast ratio
- Keyboard navigable — no keyboard traps
- Respect `prefers-reduced-motion`
- Focus indicators on all interactive elements

## Sovereign Branding

- Primary background: `#09090d` (dark)
- Gold accent: `#d4a853`
- Teal accent: `#2dd4a8`
- Purple accent: `#9b6dff`
- Red accent: `#ff4d6a`
- Fonts: DM Sans, IBM Plex Mono, Bebas Neue, Syne

## Do NOT

- Never suggest analytics or tracking code
- Never recommend third-party SaaS dependencies
- Never use `console.log()` in production code (use structured logging)
- Never suggest `npm install` for packages already in shared infrastructure
- Never generate code that violates OWASP Top 10 2025
- Never create accounts, OAuth flows, or payment integrations without security review
