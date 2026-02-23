# RUDDIE SOLUTION — Índice (todo bien acomodado)

Todo lo que usas está en **RuddieSolution/**. Base: **http://localhost:8545**

---

## Cómo arrancar

| Dónde | Comando | Qué hace |
|-------|---------|----------|
| Raíz del repo | `./up` | **Todo arriba:** prende Node (8545), Bridge (3001), Editor (3002), .NET, Platform vía start-all, y abre /platform, /, /bdet-bank, /forex, /editor, /admin |
| Raíz | `./start.sh` | Inicia Mamey Node en primer plano (Ctrl+C para parar) |
| Raíz | `./stop.sh` | Detiene el servidor node en :8545. Después de cambios en server.js o si /admin o /gaming dan 404: `./stop.sh` y luego `./start.sh` |
| Raíz | `./abre-plataformas.sh` | Abre 18 plataformas en Chrome (sin arrancar servidor) |
| Raíz | `./start-full-stack.sh` | Node :8545 + .NET Banking :5000 |
| RuddieSolution/node | `BRIDGE_PORT=3001 node banking-bridge.js` | Banking Bridge: cards, mobile, remittances, bills, ATM, insurance, investments, loyalty, forex, interbank, 2FA (200+ API) |
| RuddieSolution/platform | `./start.sh` | Editor AI (puerto 3002). También: `http://localhost:8545/editor` si 8545 + editor-api en 3002 |
| Raíz | `./start-all.sh` | **Prender todo:** Node (8545), Banking Bridge (3001), Editor API (3002); con PM2 usa `node/ecosystem.config.js` (incluye los 3) |
| RuddieSolution/node | `pm2 start ecosystem.config.js` | **Ecosystem (prender todo):** Node, Banking Bridge, Editor API en un solo comando |

Detalle: **REPORTE-TODO-ESTABA-LISTO.md**

Los scripts de la raíz delegan a **RuddieSolution/scripts/**.

---

## Estructura en RuddieSolution/

```
RuddieSolution/
├── INDICE.md                      ← estás aquí
├── LEEME
├── PLATAFORMAS-8545.md            ← lista de todas las URLs
├── ESTADO-PLATAFORMAS-SERVICIOS.md ← verificación: todas las plataformas operativas
├── REPORTE-TODO-ESTABA-LISTO.md   ← Sistema Bancario (200+ API) + Editor AI: dónde, puertos, cómo arrancar
├── REPORTE-LINKS-Y-BOTONES.md     ← inventario links/botones (Admin)
├── REPORTE-COMPLETO.md            ← reporte general del sistema
├── commerce-business-dashboard.html
├── RECIBIR_CRYPTOHOST_CONVERTIR_USDT.html
├── platform-services.json
│
├── scripts/               ← arranque y utilidades
│   ├── up.sh
│   ├── start.sh
│   ├── abre-plataformas.sh
│   └── start-full-stack.sh
│
├── node/                  ← Mamey Node (servidor :8545)
├── platform/              ← HTML de todas las plataformas
├── IerahkwaBanking.NET10/ ← API Banking .NET :5000
│
├── config/
├── data/
├── backup-system/
├── deploy/
├── monitoring/
├── nginx/
├── servers/
└── services/              ← Go, Python, Rust
```

---

## URLs principales (http://localhost:8545)

| Ruta | Qué es |
|------|--------|
| / | Dashboard Mamey Node |
| /platform | Plataforma principal |
| /admin | Admin (config, monitor, backup, links y botones) |
| /editor | Editor AI completo (Monaco, Files, Terminal, IA, Git) — requiere editor-api en 3002 |
| /bdet-bank | BDET Bank |
| /forex | Forex / AI Trading |
| /bank-worker | Banca global |
| /central-banks | 4 bancos centrales |
| /vip-transactions | VIP Transactions |
| /wallet | Wallet |
| /mamey-futures | Mamey Futures |
| /leader-control | Leader Control |
| /monitor | Monitor |
| /voting | Voting |
| /analytics | Analytics |
| /commerce-business-dashboard.html | Commerce Business Dashboard |
| /platform-services.json | Servicios (JSON) |

Lista completa: **PLATAFORMAS-8545.md**

---

## Dónde está cada cosa

- **Plataformas HTML:** `platform/*.html` → se sirven en `/platform/xxx.html` o en rutas cortas (ej. `/forex`, `/bdet-bank`).
- **Servidor:** `node/server.js` (puerto 8545).
- **Banking .NET:** `IerahkwaBanking.NET10` (puerto 5000, opcional).
- **Config:** `config/`, `platform/config.json`.
- **Datos / IA:** `data/ai-hub/`, `data/`.
- **Backups:** `backup-system/`.
- **Deploy / Nginx / monitoring:** `deploy/`, `nginx/`, `monitoring/`.
- **Estado de plataformas:** `ESTADO-PLATAFORMAS-SERVICIOS.md` (comprobación de que todas las plataformas se sirven correctamente).
- **Sistema Bancario (200+ API) y Editor AI:** `REPORTE-TODO-ESTABA-LISTO.md`. Banking Bridge en **3001** (cards, mobile, remittances, bills, ATM, insurance, investments, loyalty, forex, interbank, 2FA). Editor API en **3002**; ruta `/editor`. El servidor 8545 hace proxy a 3001 y 3002.
