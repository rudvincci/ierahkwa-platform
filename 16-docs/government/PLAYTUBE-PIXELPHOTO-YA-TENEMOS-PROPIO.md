# PlayTube / PixelPhoto / FLAME / WoWonder / QuickDate — Ya lo tenemos en código propio

**Sovereign Government of Ierahkwa Ne Kanienke**  
Principio: **TODO PROPIO** — No usamos productos de CodeCanyon ni de terceros. **Todo sin certificado ajeno** (sin licencia WoWonder, PlayTube, etc.).

---

## No usamos

| Producto | Tipo | Motivo |
|----------|------|--------|
| **PlayTube** (CodeCanyon) | PHP Video CMS, video sharing tipo YouTube | Código de 3ra compañía; PHP externo; principio TODO PROPIO. |
| **PixelPhoto** (CodeCanyon) | PHP Image sharing, photo social network | Mismo motivo: 3ra compañía, no propio. |
| **FLAME** (CodeCanyon) | PHP Social media: news, lists, quizzes, videos, polls, music | Mismo motivo: 3ra compañía; además FLAME usa Amazon S3, Stripe, WoWonder, etc. — nosotros no. |
| **WoWonder** (CodeCanyon) | PHP Social Network Platform (red social completa, API, apps nativas) | Mismo motivo: 3ra compañía; nosotros tenemos red social soberana propia, API propia, sin certificado/licencia WoWonder. |
| **QuickDate** (CodeCanyon) | PHP Dating Platform (perfiles, match, likes, créditos, regalos, WoWonder integration) | Mismo motivo: 3ra compañía; nosotros tenemos **Dating Platform** propia, API `/api/v1/dating`, token IGT-DATING, sin QuickDate ni WoWonder. |

**Actualizaciones:** No dependemos de sus updates; nosotros mantenemos y actualizamos nuestro propio código.

---

## Lo que ya tenemos (propio)

### Video / streaming (equivalente a PlayTube)

| Componente | Ubicación | Función |
|------------|-----------|---------|
| **IERAHKWA Stream** | `RuddieSolution/node/modules/streaming-platform.js` | Video (películas, series, documentales, originals), calidades hasta 8K, música, podcasts, live events. API propia. |
| **Social: posts con video** | `RuddieSolution/node/modules/social-network.js` | Posts tipo `image`/`video`, media en publicaciones. |
| **Reels** | Mismo módulo | Short videos (tipo TikTok), con likes, comentarios, shares; datos propios. |
| **Stories** | Mismo módulo | Stories con media, 24h; sin depender de Instagram. |
| **Plataforma unificada social** | `RuddieSolution/platform/social-platform.html` | Live Streaming, Reels, Feed, Chat, Video Calls, Marketplace. |
| **VOD / IPTV** | `RuddieSolution/node/data/iptv/vod.json` + rutas IPTV | Catálogo y streaming de video bajo demanda. |
| **Token streaming** | **IGT-STREAM** (ID 74) | Token oficial para la plataforma de streaming. |
| **VMS / gestión de video** | `RuddieSolution/platform/vms-gestion-video.html`, Frigate, go2rtc | Gestión de video (vigilancia, análisis, almacenamiento). Ver `docs/SISTEMA-GESTION-VIDEO-VMS.md`. |

### Imágenes / fotos (equivalente a PixelPhoto)

| Componente | Ubicación | Función |
|------------|-----------|---------|
| **Ierahkwa Image Upload** | `image-upload/` | Subida de imágenes (single/múltiple), drag & drop, progreso, vista previa, **galería**, **thumbnails**, listado y borrado. Node + Express + Multer. Puerto 3500. |
| **Ruta en plataforma** | `/image-upload/public/index.html` | Acceso desde dashboard (key `images`). |
| **Social: posts con imagen** | `RuddieSolution/node/modules/social-network.js` | Posts con `type: 'image'` y `media[]`; likes, comentarios. |
| **Stories con media** | Mismo módulo | Stories con fotos/videos. |
| **Almacenamiento** | `RuddieSolution/node/services/storage-soberano.js` | Sin AWS; archivos en nuestro sistema. |

### News, listas, quizzes, videos, polls, música (equivalente a FLAME)

| Funcionalidad FLAME | Lo que tenemos (propio) |
|---------------------|--------------------------|
| **News** | Posts tipo `article` en **social-network.js**; **SOV-SPAN** (sovereign public affairs): noticias/cobertura en vivo, canales, archivo — `sovereign-public-affairs.html`, `/api/v1/public-affairs`. Ver `docs/SOVEREIGN-PUBLIC-AFFAIRS-NETWORK.md`. |
| **Videos** | **streaming-platform.js**, posts/reels con video en **social-network.js**, **social-platform.html** (Live Streaming, Reels), IPTV/VOD. |
| **Lists** (viral lists, buzzfeed-style) | Contenido agrupado por categoría en config/combos; listas en plataforma (platform-links, unified combos); extensible como tipo de post o sección. |
| **Polls** | Posts tipo `poll` en **social-network.js**; **VotingSystem** (encuestas, votaciones) — `voting.html`, IGT-VOTE. |
| **Music** | **streaming-platform.js** (música, podcasts, playlists); token **IGT-MUSIC** (76). |
| **Quizzes** | Extensible con **FormBuilder** o lógica en social (p. ej. post tipo quiz); sin dependencia de FLAME. |
| **Reactions** | Likes en posts; `reactions` en stories; comentarios en posts/reels. |
| **Admin panel** | **admin.html**, **leader-control.html**, gestión de plataformas, usuarios y servicios. |
| **API** | APIs propias: platform-auth, banking, IPTV, public-affairs, casino, platform routes, etc. |
| **Breaking news / live** | SOV-SPAN (canales en vivo); **social-platform.html** (Live Streaming). |
| **Almacenamiento** | **storage-soberano.js** — sin Amazon S3; todo en nuestra infraestructura. |
| **Pagos** | Sin Stripe/2checkout en flujo propio; banca BDET, wallet, tokens IGT. |

### Red social completa (equivalente a WoWonder)

WoWonder es una red social PHP (posts, stories, mensajes, páginas, grupos, API para apps nativas). Nosotros **ya tenemos todo** en código propio, sin certificado WoWonder:

| Funcionalidad WoWonder | Lo que tenemos (propio) |
|------------------------|--------------------------|
| **Red social** | **IERAHKWA Sovereign Social** — `RuddieSolution/node/modules/social-network.js` — “Better than Facebook/Instagram/Twitter/TikTok • Zero Ads • User Owns Data”. |
| **Usuarios, perfiles** | `users` Map, perfiles con bio, avatar, verified, followers/following, privacidad, monetización (95% creator). |
| **Posts** | Posts tipo text, image, video, poll, article; likes, comments, shares, views, hashtags, mentions. |
| **Stories** | Stories 24h con media, stickers, music, views, reactions. |
| **Reels / short video** | Reels con video, descripción, música, efectos, likes, comentarios, shares. |
| **Mensajería** | Conversaciones E2E (Signal Protocol), mensajes encriptados en **social-network.js**; **secure-chat.html**, **video-call.html** en plataforma. |
| **Grupos / comunidad** | Extensible en social; **social-platform.html** tiene panels Groups, Marketplace, Events. |
| **API para apps** | APIs propias (platform-auth, platform routes, servicios); sin depender de la API WoWonder. |
| **UI / temas** | **social-media.html**, **social-platform.html**, estilos unificados (`unified-styles.css`), sin temas de terceros. |
| **Admin** | **admin.html**, **leader-control.html**; gestión de plataformas y servicios. |
| **Almacenamiento** | **storage-soberano.js** — sin S3 ni CDN de terceros. |
| **Token social** | **IGT-SOCIAL** (ID 56) — token oficial de la plataforma social. |

Todo lo anterior es **código propio**, instalable y operable **sin certificado ni licencia de WoWonder**.

### Dating (equivalente a QuickDate)

QuickDate es un script PHP de citas (perfiles, find match, like, créditos, regalos, integración WoWonder). Nosotros **ya lo tenemos** implementado en código propio:

| Funcionalidad QuickDate | Lo que tenemos (propio) |
|-------------------------|--------------------------|
| **Perfiles** | **Dating Platform** — `RuddieSolution/node/modules/dating-platform.js`: crear/editar perfil (nombre, bio, imágenes), descubrir perfiles. |
| **Like** | API `POST /api/v1/dating/like` (fromId, toId); like mutuo = match. |
| **Match** | API `GET /api/v1/dating/matches/:userId`; lista de matches. |
| **Find match / Discover** | API `GET /api/v1/dating/discover/:userId`; perfiles que aún no has likeado. |
| **Créditos** | Sistema de créditos IGT-DATING en el módulo; API `GET/POST /api/v1/dating/credits/:userId` y `POST /api/v1/dating/credits`. |
| **Regalos** | API `POST /api/v1/dating/gift` (fromId, toId, amount); gasta créditos y registra regalo. |
| **Admin panel** | Mismo **admin.html** / **leader-control**; gestión de plataforma. |
| **UI** | **dating-platform.html** — Descubrir, Mi perfil, Matches, Créditos IGT-DATING, Regalos. Ruta: `/dating`. |
| **Token** | **IGT-DATING** (ID 87) — token oficial. |
| **Sin WoWonder** | Cero integración con WoWonder; todo propio. |

Ruta en plataforma: **/dating** (dashboard: “💕 Open Dating (IGT-DATING)”). Servicio en `config.json` y `platform-links.json` como `dating`.

---

## Resumen

- **PlayTube (video CMS / sharing):** no lo usamos; tenemos **streaming-platform.js**, **social-network.js** (posts/reels/stories con video), **social-platform.html** (live + reels), **IPTV/VOD**, **IGT-STREAM** y VMS.
- **PixelPhoto (photo sharing / red social de fotos):** no lo usamos; tenemos **image-upload** (subida, galería, thumbnails) y **social-network** (posts/stories con imágenes, likes, comentarios).
- **FLAME (news, listas, quizzes, videos, polls, music):** no lo usamos; tenemos **SOV-SPAN** (news/live), **social-network** (posts tipo article/poll, reels, reactions), **streaming-platform** (video + música), **VotingSystem** (polls/encuestas), **admin/leader-control**, APIs propias y **storage-soberano** (sin S3).
- **WoWonder (red social PHP, API, apps nativas):** no lo usamos; tenemos **IERAHKWA Sovereign Social** (social-network.js: usuarios, posts, stories, reels, mensajes E2E), **social-platform.html** / **social-media.html**, **secure-chat**, **video-call**, APIs propias, **admin/leader-control**, **IGT-SOCIAL**. Todo sin certificado WoWonder.
- **QuickDate (dating PHP, match, créditos, regalos):** no lo usamos; tenemos **Dating Platform** (dating-platform.js + dating-platform.html): perfiles, like, match, discover, créditos IGT-DATING, regalos; API `/api/v1/dating`, ruta `/dating`. Sin QuickDate ni WoWonder.

Todo esto es **código propio**, mantenido por nosotros; las “updates” son las que nosotros hacemos. **Sin certificado ajeno.** Ver [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) y [CODIGO-PROPIO.md](../CODIGO-PROPIO.md).
