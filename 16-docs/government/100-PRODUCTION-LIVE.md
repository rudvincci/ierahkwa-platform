# 100% Production Live — Checklist

Checklist para dejar el ecosistema Futurehead Trust y el Node **listo para producción en vivo**: datos, endpoints y verificación.

---

## 1. Pre-vuelo (antes de arrancar)

| Paso | Acción |
|------|--------|
| 1 | `cp RuddieSolution/node/.env.example RuddieSolution/node/.env` |
| 2 | Rellenar en `.env`: `NODE_ENV=production`, `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` (≥32 caracteres) |
| 3 | Comprobar que existan todos los archivos de datos en `RuddieSolution/node/data/` (ver lista abajo) |

---

## 2. Archivos de datos requeridos (production ready)

En `RuddieSolution/node/data/`:

- `resumen-soberano.json`
- `ciberseguridad-101.json`
- `security-tools-recommended.json`
- `formacion-datacamp.json`
- `ecosistema-futurehead.json`
- `whitepaper-futurehead.json`
- `plan-implementacion-futurehead.json`
- `beneficios-empleados.json`
- `ofertas-corporativas.json`
- `bank-registry.json` (BDET back — registro de bancos)
- `casino/promo-codes.json`
- `casino/plataforma-mundial.json`
- `casino/apuestas-deportivas-algoritmos.json`

---

## 3. Arranque

```bash
./start.sh
```

Verificar que el Node responda en el puerto configurado (por defecto 8545).

---

## 4. Endpoints de producción

| Endpoint | Uso |
|----------|-----|
| `GET /health` | Salud básica del servicio |
| `GET /ready` | Readiness (Kubernetes/PM2): node, blockchain, memoria, dependencias opcionales |
| `GET /live` | Liveness: proceso vivo y uptime |
| `GET /api/v1/production/status` | Estado 100%: lista de archivos de datos presentes y endpoints críticos |
| `GET /api/v1/production/ready` | **200** solo si todos los archivos de datos existen; **503** si falta alguno (para load balancer o CI) |

---

## 5. Verificación 100% Production Live

Desde la raíz del repo:

```bash
cd RuddieSolution/node && node scripts/verificar-production-live.js
```

O con el servidor en otro host/puerto:

```bash
BASE_URL=https://api.tudominio.com node RuddieSolution/node/scripts/verificar-production-live.js
```

- **Exit 0:** todos los archivos de datos existen y todos los endpoints responden OK.
- **Exit 1:** hay fallos (archivos faltantes o endpoint no OK).

Usar en CI/CD o antes de cada despliegue para asegurar 100% production live.

---

## 6. Documentación relacionada

| Tema | Documento |
|------|-----------|
| **100% Production (checklist definitivo)** | `docs/PLATAFORMA-100-PRODUCTION.md` |
| Producción (resumen) | `docs/PRODUCTION-LISTO.md` |
| Ecosistema Futurehead | `docs/ECOSISTEMA-MODULAR-FUTUREHEAD.md` |
| Whitepaper | `docs/WHITEPAPER-FUTUREHEAD-TRUST-ECOSYSTEM-2026.md` |
| Plan implementación | `docs/PLAN-IMPLEMENTACION-FUTUREHEAD-2026.md` |
| Negocio independiente (carpeta) | `futurehead-trust-negocio/README.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke.*
