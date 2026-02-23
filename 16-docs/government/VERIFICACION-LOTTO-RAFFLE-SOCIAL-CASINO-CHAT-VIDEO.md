# Verificación: Lotto, Raffle, Social Media, Casino, Chat, Video Call

**Fecha:** 19 Enero 2026  
**Sovereign Government of Ierahkwa Ne Kanienke**

---

## Resumen

| Servicio      | Destino                         | Cómo abrirlo                          | Estado |
|---------------|----------------------------------|----------------------------------------|--------|
| **Lotto**     | `/tradex/#lotto`                | Botón LOTTO & RAFFLE, card Social     | OK     |
| **Raffle**    | `/tradex/#lotto` (mismo que Lotto) | `openPlatform('raffle')` — alias  | OK     |
| **Social Media** | `/tradex/#social`            | Botón SOCIAL MEDIA, card Social       | OK     |
| **Casino**    | `/tradex/#casino`               | Botón CASINO                           | OK     |
| **Chat**      | Dos opciones:                   |                                        | OK     |
|               | • `securechat` → `/platform/secure-chat.html` | Badge CHAT en header     | OK     |
|               | • `chat` → `/ierahkwa-shop/public/chat/index.html` | Card Ierahkwa Chat  | OK     |
| **Video Call**| `/platform/video-call.html`     | Badge VIDEO en header                  | OK     |

---

## Cambios realizados

### 1. `platform/index.html`

- **`platforms`:**
  - `lotto`: `/tradex/#lotto`
  - `raffle`: `/tradex/#lotto` (alias; IGT-LOTTO = Lotto & Raffle)
  - `social`: `/tradex/#social`
  - `casino`: `/tradex/#casino`
  - `chat`: `/ierahkwa-shop/public/chat/index.html`
  - `videocall`: `/platform/video-call.html`
  - `securechat`: `/platform/secure-chat.html` (nuevo)

- **Badge CHAT:** pasa a `openPlatform('securechat')` → `/platform/secure-chat.html` (y en `file://` → `http://localhost:8545/platform/secure-chat.html`).

- **Badge VIDEO:** pasa a `openPlatform('videocall')` → `/platform/video-call.html` (y en `file://` → `http://localhost:8545/platform/video-call.html`).

- **Botón LOTTO:** texto a «LOTTO & RAFFLE» y `title` actualizado.

### 2. `node/server.js`

- **`/tradex`** (ya existía): sirve `TradeX/TradeX.API/wwwroot` (Casino, Social, Lotto).
- **`/chat`** (nuevo): `express.static` a `ierahkwa-shop/public/chat` para `manifest.json`, `sw.js`, etc., usados por la app de chat.

### 3. `openPlatform` (lógica previa mantenida)

- Si `url.startsWith('/')` y `protocol === 'file:'` → `url = 'http://localhost:8545' + url` para que `/tradex`, `/platform/...`, `/ierahkwa-shop/...` funcionen al abrir `index.html` por `file://`.

---

## Cómo probar

1. **Arrancar el nodo:**
   ```bash
   ./start.sh
   ```

2. **Abrir la plataforma desde el nodo:**
   ```
   http://localhost:8545/platform
   ```

3. **Comprobar uno por uno:**
   - **Lotto & Raffle:** botón «LOTTO & RAFFLE» o `http://localhost:8545/tradex/#lotto`
   - **Social Media:** botón «SOCIAL MEDIA» o `http://localhost:8545/tradex/#social`
   - **Casino:** botón «CASINO» o `http://localhost:8545/tradex/#casino`
   - **Chat (encriptado):** badge «CHAT» en la barra → `http://localhost:8545/platform/secure-chat.html`
   - **Chat (Ierahkwa Shop):** card «Ierahkwa Chat» → `http://localhost:8545/ierahkwa-shop/public/chat/index.html`
   - **Video Call:** badge «VIDEO» → `http://localhost:8545/platform/video-call.html`

---

## Archivos involucrados

| Ruta / servicio            | Origen del HTML/UI                               |
|----------------------------|--------------------------------------------------|
| `/tradex/`                 | `TradeX/TradeX.API/wwwroot/index.html`           |
| `/platform/secure-chat.html` | `platform/secure-chat.html`                   |
| `/platform/video-call.html`  | `platform/video-call.html`                    |
| `/ierahkwa-shop/public/chat/` | `ierahkwa-shop/public/chat/index.html`       |

---

*Verificación 19 Enero 2026*
