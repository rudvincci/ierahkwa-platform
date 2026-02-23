# Endpoint Security Soberano — EPP/EDR en el Node

**Referencia:** [Singularity™ Endpoint](https://www.sentinelone.com/platform/endpoint-security/) (SentinelOne). Implementación **propia** en código; sin integración con terceros. Todo propio · Sovereign Government of Ierahkwa Ne Kanienke.

---

## Implementación en el Node

| Concepto (ref. SentinelOne) | En nuestro stack (código/datos) |
|-----------------------------|----------------------------------|
| **Protección/detección a velocidad de máquina** | Wazuh (atabey_guardian), Anomaly AI, Nginx PM, Security Fortress; visibilidad endpoint + identidad (platform-auth, roles) |
| **Respuesta y remediación rápida** | atabey_panic.sh, atabey_listener.sh; PLAYBOOK-RESPUESTA-INCIDENTES; Wazuh logs, vigilance-log; /api/operativity |
| **Agente unificado (EDR + identidad)** | Wazuh en hosts; Node 8545 + auth/roles; Security Fortress como consola; health por servicio |
| **IA generativa para equipos** | AI Hub, Atabey Master Controller; estado-final, reportes; sin NL externo; salvaguardas en repo |
| **Riesgos evolutivos** | Ataques → Wazuh + Anomaly + Phantom; superficies fragmentadas → Atabey + Security Fortress + estado-final; equipos sobrecargados → checklist-status, playbooks, scripts |

---

## API (Node)

| Método y ruta | Descripción |
|---------------|-------------|
| **GET /api/v1/security/endpoint** | Payload completo: proteccionDeteccion, respuestaRemediacion, agenteUnificado, iaGenerativa, riesgosEvolutivos, apis, documentos. Fuente: `node/data/endpoint-security-soberano.json`. |
| **GET /api/v1/security/endpoint/mapa** | Solo bloques para dashboards. |
| **GET /api/v1/security/endpoint/status** | Estado en vivo: pings a Node (8545), Baserow (8080), Nextcloud (8081), Nginx PM (81). Opcional: `?host=127.0.0.1`. |
| **GET /api/v1/security/endpoint/health** | Salud del módulo (existencia del JSON). |

---

## Datos

- **Fuente de verdad:** `RuddieSolution/node/data/endpoint-security-soberano.json`
- **Router:** `RuddieSolution/node/routes/endpoint-security-api.js`
- **Montaje en server.js:** `/api/v1/security/endpoint`

---

## Referencias

- [Singularity Endpoint](https://www.sentinelone.com/platform/endpoint-security/) — EPP/EDR, protección/detección/respuesta, agente unificado (referencia externa; no se integra).
- [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) — Checklist y mapa 101↔stack.
- [CLOUD-SECURITY-SOBERANO.md](./CLOUD-SECURITY-SOBERANO.md) — CNAPP/CSPM/CWPP en Node.
