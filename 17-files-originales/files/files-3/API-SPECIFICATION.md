# 🌐 API Soberana — Especificación REST v1.0
## Red Soberana Digital de las Américas · MameyNode

**Base URL:** `https://api.soberano.bo/v1`
**Auth:** Bearer token (Sovereign ID JWT)
**Formato:** JSON
**Rate Limit:** 1000 req/min (autenticado), 100 req/min (público)
**Cifrado:** TLS 1.3 + Post-quantum overlay

---

## 🔐 Autenticación

```
POST /auth/register          → Crear cuenta Sovereign ID
POST /auth/login             → Login (face + password)
POST /auth/refresh           → Refresh token
POST /auth/verify            → Verificar nivel soberano
GET  /auth/me                → Perfil actual
```

## 🏦 BDET Bank API

```
POST /bdet/payment           → Procesar pago (auto-split por plataforma)
POST /bdet/escrow/create     → Crear escrow
POST /bdet/escrow/{id}/release → Liberar escrow
POST /bdet/escrow/{id}/dispute → Disputar escrow
GET  /bdet/balance           → Balance Wampum
GET  /bdet/transactions      → Historial
POST /bdet/transfer          → Transferir WMP
POST /bdet/forex/quote       → Cotización WMP↔USD/BTC/ETH
POST /bdet/forex/execute     → Ejecutar cambio
POST /bdet/microloan/apply   → Solicitar microcrédito
GET  /bdet/microloan/{id}    → Estado microcrédito
```

## ✉️ 01 — CorreoSoberano API

```
POST /mail/send              → Enviar email cifrado
GET  /mail/inbox             → Bandeja entrada
GET  /mail/sent              → Enviados
GET  /mail/message/{id}      → Leer mensaje
DELETE /mail/message/{id}    → Eliminar
POST /mail/encrypt           → Cifrar adjunto
POST /mail/bdet-inline       → Insertar pago BDET en email
GET  /mail/search?q=         → Buscar emails
```

## 🌐 02 — Red Soberana API (Social)

```
POST /social/post            → Crear post
GET  /social/feed            → Feed cronológico
GET  /social/feed/ai         → Feed AI (sin manipulación)
POST /social/story           → Crear story
POST /social/poll            → Crear encuesta blockchain
POST /social/poll/{id}/vote  → Votar (verificado MameyNode)
POST /social/event           → Crear evento
GET  /social/groups          → Listar grupos
POST /social/tip             → Enviar Wampum tip
GET  /social/trending        → Tendencias
```

## 🔍 03 — BúsquedaSoberana API

```
GET  /search?q=&type=&lang=  → Buscar (web|img|video|news|market|academic)
GET  /search/suggest?q=      → Autocompletar
GET  /search/knowledge/{id}  → Knowledge panel
POST /search/ai-answer       → Respuesta MameyAI
GET  /search/trending        → Búsquedas tendencia
```

## 📺 04 — CanalSoberano API (Video)

```
POST /video/upload           → Subir video
GET  /video/feed             → Feed de videos
GET  /video/{id}             → Ver video
GET  /video/{id}/stream      → Stream HLS/DASH
POST /video/{id}/like        → Like
POST /video/{id}/comment     → Comentar
POST /video/live/start       → Iniciar live
POST /video/live/{id}/chat   → Chat en vivo
POST /video/{id}/tip         → Wampum tip a creador
GET  /video/channel/{id}     → Canal de creador
GET  /video/categories       → Categorías
GET  /video/trending         → Trending
```

## 🎵 05 — MúsicaSoberana API

```
GET  /music/stream/{id}      → Stream audio
GET  /music/search?q=        → Buscar canciones/artistas
GET  /music/playlist/{id}    → Playlist
POST /music/playlist/create  → Crear playlist
GET  /music/artist/{id}      → Perfil artista
GET  /music/genres           → Géneros
POST /music/{id}/tip         → Propina BDET a artista
GET  /music/radio/{genre}    → Radio por género
GET  /music/new-releases     → Nuevos lanzamientos
```

## 🏠 06 — HospedajeSoberano API

```
GET  /lodging/search?dest=&checkin=&checkout=&guests= → Buscar
GET  /lodging/{id}           → Detalle hospedaje
POST /lodging/{id}/book      → Reservar (escrow BDET)
GET  /lodging/experiences    → Experiencias
POST /lodging/{id}/review    → Review (blockchain verified)
GET  /lodging/host/{id}      → Perfil anfitrión
POST /lodging/host/register  → Registrar hospedaje
```

## 🏺 07 — ArtesaníaSoberana API

```
GET  /artisan/products?cat=  → Buscar productos
GET  /artisan/product/{id}   → Detalle producto
POST /artisan/product/{id}/buy → Comprar (escrow BDET)
GET  /artisan/product/{id}/cert → Certificado blockchain NFT
GET  /artisan/{id}           → Perfil artesano
POST /artisan/register       → Registrar como artesano
POST /artisan/product/create → Listar producto
GET  /artisan/categories     → Categorías
```

## 📱 08 — CortosIndígenas API

```
POST /shorts/upload          → Subir video corto
GET  /shorts/feed            → Feed infinito
GET  /shorts/{id}            → Ver corto
POST /shorts/{id}/like       → Like
POST /shorts/{id}/comment    → Comentar
POST /shorts/{id}/tip        → Propina BDET
POST /shorts/live/start      → Iniciar live
GET  /shorts/trending        → Trending
GET  /shorts/sounds          → Sonidos MúsicaSoberana
```

## 🛍 09 — ComercioSoberano API

```
POST /commerce/store/create  → Crear tienda
GET  /commerce/store/{id}    → Mi tienda
POST /commerce/product/create → Agregar producto
PUT  /commerce/product/{id}  → Editar producto
POST /commerce/order/create  → Crear orden
GET  /commerce/orders        → Mis órdenes
GET  /commerce/analytics     → Analytics
POST /commerce/shipping/create → Crear envío SoberanoFreight
GET  /commerce/templates     → Plantillas
```

## 📈 10 — InvertirSoberano API

```
GET  /invest/portfolio       → Mi portfolio
GET  /invest/assets          → Activos disponibles
POST /invest/order/buy       → Comprar activo
POST /invest/order/sell      → Vender activo
GET  /invest/price/{symbol}  → Precio actual
GET  /invest/chart/{symbol}  → Datos de chart
GET  /invest/funds           → Fondos soberanos comunitarios
POST /invest/fund/{id}/invest → Invertir en fondo
GET  /invest/news            → Noticias del mercado
```

## 📄 11 — DocsSoberanos API

```
POST /docs/create            → Crear documento
GET  /docs/{id}              → Abrir documento
PUT  /docs/{id}              → Guardar (cifrado blockchain)
POST /docs/{id}/share        → Compartir
GET  /docs/{id}/history      → Historial de cambios
POST /docs/{id}/comment      → Agregar comentario
POST /docs/{id}/translate    → Traducir via Atabey
POST /docs/{id}/ai-assist    → MameyAI redacción
GET  /docs/{id}/export?fmt=  → Exportar (pdf|docx|md)
```

## 🗺 12 — MapaSoberano API

```
GET  /map/communities        → Comunidades indígenas
GET  /map/nodes              → Nodos soberanos
GET  /map/search?q=          → Buscar lugares
GET  /map/community/{id}     → Detalle comunidad
GET  /map/services?type=&lat=&lng= → Servicios cercanos
GET  /map/route?from=&to=    → Ruta
GET  /map/satellite/{lat}/{lng} → Vista satelital
```

## 📢 13 — VozSoberana API

```
POST /voice/post             → Publicar (hasta 500 chars)
GET  /voice/feed             → Timeline
GET  /voice/feed/following   → Solo siguiendo
POST /voice/{id}/repost      → Repost
POST /voice/{id}/reply       → Responder
POST /voice/{id}/tip         → Propina BDET
GET  /voice/trending         → Tendencias
GET  /voice/user/{handle}    → Perfil
POST /voice/user/{id}/follow → Seguir
GET  /voice/search?q=        → Buscar
```

## 💼 14 — TrabajoSoberano API

```
GET  /jobs/search?q=&loc=    → Buscar empleos
GET  /jobs/{id}              → Detalle empleo
POST /jobs/{id}/apply        → Aplicar
POST /jobs/create            → Publicar empleo
GET  /jobs/profile/{id}      → Perfil profesional
PUT  /jobs/profile           → Actualizar perfil
GET  /jobs/connections       → Mis conexiones
POST /jobs/connect/{id}      → Conectar
```

## 💰 15 — RentaSoberano API

```
POST /renta/gig/create       → Crear trabajo
GET  /renta/gigs?cat=        → Buscar trabajos
POST /renta/gig/{id}/apply   → Aplicar
POST /renta/gig/{id}/assign  → Asignar worker
POST /renta/gig/{id}/complete → Completar (release escrow)
POST /renta/gig/{id}/dispute → Disputar
GET  /renta/worker/{id}      → Perfil worker
GET  /renta/reputation/{id}  → Reputación blockchain
```

## 📚 18 — SabiduríaSoberana API

```
GET  /wiki/article/{slug}    → Leer artículo
GET  /wiki/search?q=         → Buscar
POST /wiki/article/create    → Crear artículo
PUT  /wiki/article/{id}      → Editar
GET  /wiki/article/{id}/history → Historial (blockchain)
POST /wiki/article/{id}/translate → Traducir Atabey
GET  /wiki/categories        → Categorías
GET  /wiki/featured          → Artículos destacados
```

## 🎓 19 — UniversidadSoberana API

```
GET  /edu/courses?cat=       → Buscar cursos
GET  /edu/course/{id}        → Detalle curso
POST /edu/course/{id}/enroll → Inscribirse
GET  /edu/course/{id}/lessons → Lecciones
POST /edu/lesson/{id}/complete → Completar lección
GET  /edu/certificates       → Mis certificados (NFT)
GET  /edu/certificate/{id}   → Verificar certificado
POST /edu/scholarship/apply  → Solicitar beca
```

## 📰 20 — NoticiaSoberana API

```
GET  /news/feed?cat=         → Feed de noticias
GET  /news/{id}              → Artículo
GET  /news/trending          → Trending
GET  /news/sources           → Fuentes verificadas
GET  /news/search?q=         → Buscar noticias
POST /news/{id}/verify       → Verificar fuente (blockchain)
POST /news/alert/subscribe   → Suscribir alertas
```

---

## 🌐 APIs Transversales

### Atabey Translator
```
POST /atabey/translate       → Traducir texto
POST /atabey/detect          → Detectar idioma
GET  /atabey/languages       → Idiomas soportados (14 indígenas + 6)
POST /atabey/tts             → Text-to-speech
POST /atabey/stt             → Speech-to-text
```

### MameyAI
```
POST /ai/complete            → Completar texto
POST /ai/summarize           → Resumir
POST /ai/generate-image      → Generar imagen
POST /ai/classify            → Clasificar contenido
POST /ai/moderate            → Moderar (AI Fortress)
POST /ai/recommend           → Recomendaciones
```

### MameyNode Blockchain
```
GET  /chain/block/{number}   → Bloque
GET  /chain/tx/{hash}        → Transacción
GET  /chain/status           → Estado de la red
POST /chain/contract/deploy  → Deploy smart contract
POST /chain/contract/call    → Llamar función
GET  /chain/gas              → Gas price actual
```

---

## Webhooks
```
POST /webhooks/register      → Registrar webhook
Events: payment.completed, escrow.created, escrow.released,
        order.created, review.posted, certificate.minted,
        vote.cast, message.received, tip.received
```

## Rate Limits
| Nivel | Requests/min | Concurrent |
|-------|-------------|------------|
| Público | 100 | 10 |
| Autenticado | 1,000 | 50 |
| Premium | 10,000 | 200 |
| Plataforma | 100,000 | 1,000 |

## SDK
```bash
npm install @mameynode/sdk@4.2.0
pip install mameynode-sdk==4.2.0
```

---
*API Soberana v1.0 · MameyNode · BDET Bank · Soberanía siempre 🌿*
