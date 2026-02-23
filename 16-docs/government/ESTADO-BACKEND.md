# Estado del backend propio — IERAHKWA

**Sovereign Government of Ierahkwa Ne Kanienke**

Resumen en una página: qué es nuestro backend, dónde está y cómo comprobarlo.

---

## Qué es

- **Backend propio:** IERAHKWA Futurehead Mamey Node.
- **Código:** `RuddieSolution/node/server.js` (+ `node/modules/`, `node/routes/`, `node/services/`).
- **Puerto:** `8545` (o `process.env.PORT`).
- **Protocolo:** **HTTP** por defecto (sin certificado). Opcional: `USE_HTTPS=true` + `node/ssl/ssl-config.js`.

---

## Dónde está

| | |
|--|--|
| Script | `RuddieSolution/node/server.js` |
| Config puertos | `RuddieSolution/config/services-ports.json` → `core_services.node_mamey` |
| Documentación detallada | [SERVICIOS-NUESTRO-NODE.md](SERVICIOS-NUESTRO-NODE.md) |
| Node sin certificado | Ver sección "Node sin certificado — Dónde va" en ese mismo doc |

---

## Cómo arrancar

- Desde raíz del repo: `./start.sh`
- Solo Node: `cd RuddieSolution/node && node server.js`

---

## Cómo comprobar estado

- **Script:** `./status.sh` — comprueba Node (8545), Banking Bridge (3001), AI Hub, operatividad, PM2, SSL.
- **Health:** `http://localhost:8545/health`
- **Plataforma:** `http://localhost:8545/platform`
- **Banco:** `http://localhost:8545/bdet-bank`

---

## Principio

Todo propio: infraestructura y código en nuestro repo. Sin dependencias de backend de terceros. Ver [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md).
