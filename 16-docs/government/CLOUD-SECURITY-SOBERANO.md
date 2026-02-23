# Cloud Security Soberano — CNAPP/CSPM/CWPP en el Node

**Referencia:** [Singularity™ Cloud Security](https://www.sentinelone.com/platform/cloud-security/) (SentinelOne). Implementación **propia** en código; sin integración con terceros. Todo propio · Sovereign Government of Ierahkwa Ne Kanienke.

---

## Implementación en el Node

| Concepto (ref. SentinelOne) | En nuestro stack (código/datos) |
|-----------------------------|----------------------------------|
| **CNAPP** (plataforma unificada, visibilidad, detección/respuesta, automatización) | Atabey Command Center, Security Fortress, Wazuh, Anomaly AI, estado-final-sistema.json, scripts soberanos |
| **CSPM** (postura, misconfiguraciones, cumplimiento) | Docker Fortress, cloud-data-security-check.sh, secret-scan, checklist Ciberseguridad 101 |
| **CWPP** (protección cargas de trabajo) | Docker (Baserow, Nextcloud, Wazuh, Nginx PM), Node 8545, health checks |
| **CDR** (detección y respuesta) | Wazuh logs, Atabey vigilance-log, PLAYBOOK-RESPUESTA-INCIDENTES, panic/listener, clone-from-backup |
| **DevSecOps** (shift-left, CI, escaneo) | Secret scanning en CI, SECRET-SCANNING.md, hardening nodo 8545 |
| **KSPM** (contenedores/postura) | Docker Compose, cloud-data-security-check, checklist-status por puerto |

---

## API (Node)

| Método y ruta | Descripción |
|---------------|-------------|
| **GET /api/v1/security/cloud** | Payload completo: cnapp, cspm, cwpp, cdr, devsecops, kspm, cobertura, apis, documentos. Fuente: `node/data/cloud-security-soberano.json`. |
| **GET /api/v1/security/cloud/mapa** | Solo bloques CNAPP/CSPM/CWPP/CDR/DevSecOps/KSPM y cobertura (para dashboards). |
| **GET /api/v1/security/cloud/status** | Estado en vivo de workloads: pings a Node (8545), Baserow (8080), Nextcloud (8081), Nginx PM (81). Query opcional: `?host=127.0.0.1`. |
| **GET /api/v1/security/cloud/health** | Salud del módulo (existencia del JSON). |

---

## Datos

- **Fuente de verdad:** `RuddieSolution/node/data/cloud-security-soberano.json`
- **Router:** `RuddieSolution/node/routes/cloud-security-api.js`
- **Montaje en server.js:** `/api/v1/security/cloud`

---

## Referencias

- [Singularity Cloud Security](https://www.sentinelone.com/platform/cloud-security/) — CNAPP, CSPM, CWPP, CDR, DevSecOps, KSPM (referencia externa; no se integra).
- [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) — Checklist y mapa 101↔stack.
- [CLOUD-DATA-SECURITY.md](./CLOUD-DATA-SECURITY.md) — Vault, backups, integridad.
