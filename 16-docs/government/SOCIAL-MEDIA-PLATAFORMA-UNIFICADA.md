# Social Media — Todo lo que hay y plataforma unificada

**Sovereign Government of Ierahkwa Ne Kanienke**  
Resumen de todo lo relacionado con redes sociales y la plataforma unificada.

---

## 1. Resumen rápido

| Qué | Dónde | Descripción |
|-----|--------|-------------|
| **Plataforma unificada (Social)** | `social-platform.html` | IERAHKWA SOCIAL PLATFORM: Feed, Chat, Video, Streaming, Groups, Marketplace, Events |
| **Red social (feed/clásica)** | `social-media.html` | IERAHKWA SOCIAL: feed, publicar, amigos, tendencias |
| **Códigos redes** | `social-media-codes.html` | Códigos reutilizables (compartir Facebook, Twitter, etc.) |
| **Backend social** | `node/modules/social-network.js` | API soberana: usuarios, posts, stories, reels, monetización |
| **Chat seguro** | `secure-chat.html` | Mensajería encriptada |
| **Video llamadas** | `video-call.html` | Videollamadas |
| **Chat (Shop)** | `ierahkwa-shop/public/chat/` | Chat de la tienda |
| **Notificaciones** | `notifications.html` | Centro de notificaciones |

La **plataforma unificada** es **`social-platform.html`**: “Social Media • Encrypted Chat • Video Calls • Streaming • Community”.  
El **dashboard principal** y el header apuntan por defecto a **`social-media.html`** (feed clásico).

---

## 2. Archivos HTML (frontend)

| Archivo | Ruta corta | Contenido |
|---------|------------|------------|
| **social-platform.html** | `/platform/social-platform.html` | **Plataforma unificada**: Overview, Feed, Stories, Reels, Encrypted Chat, Video Calls, Groups, Live Streaming, Marketplace, Events. Usa `assets/unified-core.js`. |
| **social-media.html** | `/social-media`, `/platform/social-media.html` | Red social: inicio, amigos, mensajes, notificaciones, guardados, feed con publicaciones, crear post, tendencias. |
| **social-media-codes.html** | `/social-codes`, `/platform/social-media-codes.html` | Códigos para botones de compartir (Facebook, Twitter, Instagram, LinkedIn, YouTube, TikTok, WhatsApp, Telegram, Discord, etc.). |
| **secure-chat.html** | `/platform/secure-chat.html` | Chat encriptado (badge CHAT en header → `securechat`). |
| **video-call.html** | `/platform/video-call.html` | Videollamadas (badge VIDEO en header → `videocall`). |
| **chat.html** | `/platform/chat.html` | Página de chat genérica. |
| **notifications.html** | `/platform/notifications.html` | Centro de notificaciones. |

En **config.json**:
- `videocall` → `social-platform.html#video`
- `securechat` → `social-platform.html#chat`

Es decir: desde la config, Video y Chat encriptado pueden abrirse como paneles dentro de la plataforma unificada.

---

## 3. Rutas del nodo (platform-routes.js)

```text
/social-media   → social-media.html
/social-codes   → social-media-codes.html
```

No hay ruta dedicada para `social-platform.html`; se abre por path completo `/platform/social-platform.html`.

---

## 4. Backend y APIs

| Componente | Ubicación | Función |
|------------|-----------|---------|
| **Social Network (Node)** | `RuddieSolution/node/modules/social-network.js` | IERAHKWA Sovereign Social Network: usuarios, posts, stories, reels, likes, comments, monetización (95% creator), privacidad, cero ads. |
| **Social (.NET)** | `platform-dotnet/` | `SocialController.cs`, `SocialService.cs`, `SocialModels.cs` — servicios de red social en .NET. |

---

## 5. Dónde se enlaza (index, config, dashboard)

- **index.html**: sección “SOCIAL MEDIA & COMUNICACIÓN” → botón abre `/social-media`.  
- **platform-links.json**: “📊 Open Social Media Dashboard” → `/social-media`; “📱 SOCIAL” → `/platform/social-media.html`.  
- **config.json**: categoría “comunicacion”, ítem “SOCIAL” → `/platform/social-media.html`.  
- **admin.html**: card “SOCIAL MEDIA” → `/social-media`; también botón “Abrir” para `social-platform.html`.  
- **user-dashboard.html**: “Social Media” → `/social-media.html`.  
- **americas-communication-platform.html**: iframe “Redes sociales” → `social-media.html`.

Conclusión: el **punto de entrada oficial** desde dashboard y menús es **social-media.html**; la **unificada** es **social-platform.html** (más completa: chat, video, streaming, etc.) y está enlazada desde admin y desde config para videocall/securechat.

---

## 6. Token IGT-SOCIAL

- **ID 56**, símbolo **IGT-SOCIAL**  
- Uso: “Social media platform token for content creation and community engagement”  
- Definiciones: `tokens/56-IGT-SOCIAL.json`, `tokens/56-IGT-SOCIAL/whitepaper*.md`  
- Registrado en `platform-tokens.json` e `ierahkwa-futurehead-mamey-node.json`.

---

## 7. Documentación de referencia

| Doc | Contenido |
|-----|------------|
| **docs/MAPA-DEL-MUNDO-SEGURIDAD-CASINO-SOCIAL-ETC.md** | Mapa “Mundo SOCIAL MEDIA”: social-media, social-platform, social-media-codes, secure-chat, video-call, notifications. |
| **VERIFICACION-LOTTO-RAFFLE-SOCIAL-CASINO-CHAT-VIDEO.md** | Verificación de Lotto, Raffle, Social Media, Casino, Chat, Video; menciona `/tradex/#social` para Social Media y rutas de chat/video. |
| **REPORTE-EJECUTIVO-COMPLETO-2026.md** | Parte 9: “Kanien Social”, Kanien Chat, Kanien Video, etc. (visión de dominios). |

---

## 8. Resumen: “plataforma unificada”

- **La plataforma unificada de social** es **`social-platform.html`** (“IERAHKWA SOCIAL PLATFORM”): una sola página con Overview, Feed, Stories, Reels, Encrypted Chat, Video Calls, Groups, Live Streaming, Marketplace y Events.  
- **`social-media.html`** es la red social tipo feed (inicio, amigos, publicar, tendencias) y es la que se usa como destino principal desde el dashboard y el menú SOCIAL.  
- Para tener **una sola entrada** que sea la unificada, habría que cambiar enlaces de dashboard/config/header de `social-media.html` a `social-platform.html`, o hacer que `/social-media` redirija o sirva `social-platform.html`.

Si quieres, el siguiente paso puede ser: (1) unificar la entrada para que “Social” abra siempre `social-platform.html`, o (2) dejar `social-media.html` como feed rápido y añadir en la unificada un enlace claro a “Abrir feed clásico” a `social-media.html`.
