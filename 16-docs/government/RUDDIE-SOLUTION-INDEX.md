# Dónde está todo (RuddieSolution)

Todo lo que usas está en **RuddieSolution/**, bien acomodado.

→ **Índice completo:** [RuddieSolution/INDICE.md](RuddieSolution/INDICE.md)  
→ **Lista de plataformas (URLs):** [RuddieSolution/PLATAFORMAS-8545.md](RuddieSolution/PLATAFORMAS-8545.md)

---

## Arrancar (desde la raíz del repo)

| Comando | Qué hace |
|---------|----------|
| `./up` | Arranca Node :8545 (si hace falta) y abre Chrome: /platform, /, /bdet-bank, /forex |
| `./start.sh` | Node en primer plano (:8545) |
| `./abre-plataformas.sh` | Abre 18 plataformas en Chrome |
| `./start-full-stack.sh` | Node :8545 + .NET Banking :5000 |

Los scripts delegan a **RuddieSolution/scripts/**.

---

## Dónde está cada cosa

- **Node (Mamey, API, BDET, AI):** `RuddieSolution/node/` → http://localhost:8545
- **Plataformas HTML:** `RuddieSolution/platform/`
- **Banking .NET:** `RuddieSolution/IerahkwaBanking.NET10/` → :5000
- **Scripts de arranque:** `RuddieSolution/scripts/`
- **Rust / Go / Python:** `RuddieSolution/services/`
- **Servidores (BDET, TradeX, SIIS, Gov, etc.):** `RuddieSolution/servers/`
- **Deploy, config, nginx, monitoring, backup, data:** `RuddieSolution/deploy/`, `config/`, `nginx/`, `monitoring/`, `backup-system/`, `data/`
- **Docs y listas:** `RuddieSolution/INDICE.md`, `RuddieSolution/PLATAFORMAS-8545.md`, `RuddieSolution/LEEME`

---

## Archivos migrados a RuddieSolution/

- `commerce-business-dashboard.html` → /commerce-business-dashboard.html
- `RECIBIR_CRYPTOHOST_CONVERTIR_USDT.html` → /RECIBIR_CRYPTOHOST_CONVERTIR_USDT.html
- `platform-services.json` → /platform-services.json
- `PLATAFORMAS-8545.md` → lista de todas las URLs
