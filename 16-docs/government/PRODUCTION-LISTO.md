# ✅ Producción lista — sin duplicaciones

**Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister**

Referencia única para dejar todo listo para producción.

---

## 1. Arranque (un solo comando)

```bash
./start.sh
```

- **No usar** varios scripts de inicio a la vez. `start.sh` detecta `node/` o `RuddieSolution/node`, PM2 o procesos en background, .NET y Platform.
- Alternativas documentadas: `./start-all-services.sh`, `./start-all.sh` — ver `COMMANDS.md`.

---

## 2. Parar / Estado

| Acción   | Comando        |
|----------|----------------|
| Detener  | `./stop-all.sh` |
| Estado   | `./status.sh`   |

---

## 3. Configuración (una sola fuente)

| Qué           | Dónde |
|---------------|--------|
| Puertos       | `RuddieSolution/config/services-ports.json` |
| Variables env | `RuddieSolution/node/.env` (copiar de `.env.example`) |

---

## 4. Monitoreo y reportes

| Qué                    | Comando / Ubicación |
|------------------------|----------------------|
| Test cada 5 min + reparación automática | `node scripts/test-cada-5min.js` (desde `RuddieSolution/node`) |
| Reporte automático     | `docs/REPORTE-AUTO-5MIN.md` |
| Test manual una vez    | `node scripts/test-toda-plataforma.js` |
| Verificación 100% prod | `node scripts/verificar-plataforma-100-production.js` |
| **Verificación 100% Production Live** (Futurehead + datos + APIs) | `cd RuddieSolution/node && node scripts/verificar-production-live.js` — comprueba archivos de datos y endpoints `/health`, `/ready`, `/live`, `/api/v1/production/*`, sovereignty y casino. |
| **Asegurar 100% y production (ambas verificaciones)** | `./scripts/asegurar-100-production.sh` — ejecuta las dos verificaciones; exit 0 solo si ambas pasan. Ver `docs/ASEGURAR-100-PRODUCTION-CADA-PLATAFORMA.md`. |
| **Smoke test** | `./scripts/smoke-test.sh [BASE_URL]` — curl a /health, /ready, /api/v1/production/ready, /api/v1/bdet/bank-registry; exit 0 solo si todos 200. |

**Endpoints de producción:** `GET /api/v1/production/status` (estado de archivos y endpoints), `GET /api/v1/production/ready` (200 solo si todos los datos presentes).

---

## 5. Documentación de referencia

| Tema        | Documento |
|-------------|------------|
| Comandos    | `COMMANDS.md` |
| Producción  | `PRODUCTION-READY.md` |
| Go live     | `GO-LIVE-CHECKLIST.md` |

---

## 6. Checklist rápido antes de producción

- [ ] `cp RuddieSolution/node/.env.example RuddieSolution/node/.env` y rellenar JWT_ACCESS_SECRET, JWT_REFRESH_SECRET (≥32 caracteres).
- [ ] Archivos de datos en `node/data/` (incl. `bank-registry.json` para BDET back). Ver lista en `docs/PLATAFORMA-100-PRODUCTION.md`.
- [ ] `./start.sh` — verificar que Node (8545) y Bridge (3001) respondan.
- [ ] `./status.sh` — sin errores.
- [ ] **Obligatorio para 100% prod:** `cd RuddieSolution/node && node scripts/verificar-production-live.js` → exit 0 (datos + endpoints + bank-registry).
- [ ] Opcional: `node scripts/verificar-plataforma-100-production.js` — 0 fallos.
- [ ] Opcional: `node scripts/test-cada-5min.js` en segundo plano para monitoreo continuo.

**Checklist definitivo 100% production:** `docs/PLATAFORMA-100-PRODUCTION.md`.

---

*Todo listo para producción. Sin duplicaciones.*
