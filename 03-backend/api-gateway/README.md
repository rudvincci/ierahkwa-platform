# API Gateway

> Gateway central de la Red Soberana Ierahkwa con enrutamiento, autenticacion JWT y circuit breakers para 22 grupos de rutas API.

## Descripcion

El API Gateway es el punto de entrada unificado para todos los servicios de la plataforma Ierahkwa Ne Kanienke. Gestiona el enrutamiento de 22 grupos de rutas que cubren finanzas (BDET), blockchain, comercio, comunicacion, gobernanza, contenido y servicios comunitarios. Opera sobre MameyNode v4.2 y esta disenado para produccion con alta disponibilidad.

Implementa autenticacion JWT con soporte dual HS256/RS256, control de acceso basado en tiers (member, resident, citizen, admin), rate limiting por IP (200 req/60s), sanitizacion de entradas contra prototype pollution e inyeccion, y un trail de auditoria inmutable con cadena de hashes para operaciones financieras y de gobernanza. Todas las respuestas de error siguen el estandar RFC 7807 Problem Details.

El gateway protege los servicios downstream mediante circuit breakers independientes para BDET, Chain, Invest y Commerce, con umbrales configurables y recuperacion automatica. Soporta graceful shutdown con drenaje de conexiones hasta 30 segundos.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **Autenticacion**: JWT HS256 / RS256 (zero-dependency)
- **Rate Limiting**: rate-limiter-flexible (memoria)
- **Seguridad**: Helmet, CORS whitelist, sanitizacion global
- **Logging**: Pino-compatible (JSON estructurado)
- **Auditoria**: Hash-chain inmutable (SHA-256)
- **Resiliencia**: Circuit Breaker (CLOSED/OPEN/HALF_OPEN)
- **Puerto**: 3000

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con version y uptime |
| GET | /ready | Readiness probe con estado de circuit breakers |
| GET | /metrics | Metricas de memoria, uptime y circuitos |
| POST | /v1/auth/login | Autenticacion (ruta publica) |
| POST | /v1/auth/register | Registro de usuarios (ruta publica) |
| * | /v1/bdet/* | Operaciones financieras BDET Bank (circuit-protected) |
| * | /v1/invest/* | Inversiones y ordenes (circuit-protected) |
| * | /v1/commerce/* | Comercio electronico (circuit-protected) |
| * | /v1/chain/* | Operaciones blockchain (circuit-protected) |
| * | /v1/mail/* | Correo soberano |
| * | /v1/social/* | Red social |
| * | /v1/voice/* | Comunicacion de voz |
| * | /v1/search/* | Motor de busqueda |
| * | /v1/video/* | Plataforma de video |
| * | /v1/music/* | Plataforma de musica |
| * | /v1/docs/* | Documentos colaborativos |
| * | /v1/news/* | Noticias |
| * | /v1/wiki/* | Wiki soberana |
| * | /v1/edu/* | Plataforma educativa |
| * | /v1/lodging/* | Alojamiento |
| * | /v1/artisan/* | Artesanias |
| * | /v1/jobs/* | Bolsa de empleo |
| * | /v1/renta/* | Alquiler de bienes |
| * | /v1/map/* | Mapa soberano |
| * | /v1/atabey/* | Gobernanza y propuestas |
| * | /v1/ai/* | Inferencia IA |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3000 |
| NODE_ENV | Entorno de ejecucion | production |
| JWT_SECRET | Clave secreta para HS256 | (min 32 chars) |
| JWT_PUBLIC_KEY | Clave publica para RS256 | -----BEGIN PUBLIC KEY----- |
| JWT_ISSUER | Emisor esperado del token | ierahkwa |
| CORS_ORIGIN | Origenes permitidos (separados por coma) | https://ierahkwa.gov |
| RATE_LIMIT_POINTS | Peticiones por ventana | 200 |
| RATE_LIMIT_DURATION | Duracion de ventana en segundos | 60 |
| LOG_LEVEL | Nivel minimo de log | info |

## Instalacion

```bash
npm install
npm start
```

### Desarrollo

```bash
npm run dev   # --watch mode
npm test      # Jest
```

## Docker

```bash
docker build -t api-gateway .
docker run -p 3000:3000 \
  -e JWT_SECRET=your-secret-min-32-chars \
  -e NODE_ENV=production \
  api-gateway
```

## Arquitectura

```
Cliente → API Gateway (Express)
            ├── Helmet (security headers)
            ├── CORS (whitelist-based)
            ├── Compression (gzip)
            ├── Body Parser (10mb limit)
            ├── Request Logger (Pino-compatible)
            ├── Global Sanitizer (prototype pollution defense)
            ├── Rate Limiter (200 req/60s per IP)
            ├── JWT Auth (HS256/RS256, tier-based)
            ├── Audit Middleware (write ops, hash-chain)
            └── 22 Route Groups → Downstream Services
                 ├── BDET ←── Circuit Breaker
                 ├── Chain ←── Circuit Breaker
                 ├── Invest ←── Circuit Breaker
                 └── Commerce ←── Circuit Breaker
```

Los circuit breakers operan con un umbral de 5 fallos consecutivos antes de abrir, un timeout de reset de 30 segundos, y estados CLOSED → OPEN → HALF_OPEN para recuperacion gradual.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
