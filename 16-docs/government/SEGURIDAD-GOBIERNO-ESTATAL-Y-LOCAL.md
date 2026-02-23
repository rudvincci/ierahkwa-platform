# Ciberseguridad para gobierno estatal y local — Soberanía Ierahkwa

Alineación del mando soberano (Atabey Fortress / Ierahkwa) con el marco **[State and Local Government Cybersecurity](https://www.sentinelone.com/es/platform/state-and-local-government/)** de SentinelOne: protección de servicios críticos y ciudadanos, defensa proactiva y preparación operativa. **Checklist operativo unificado:** [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) (no duplicar pasos).

---

## Enlaces de referencia

| Recurso | URL |
|--------|-----|
| **Singularity Platform** (plataforma XDR unificada) | https://www.sentinelone.com/platform/ |
| **State and Local Government (SentinelOne)** | https://www.sentinelone.com/es/platform/state-and-local-government/ |

*Proteger servicios vitales y ciudadanos frente a interrupciones; pasar de reactivo a preparado; defensa autónoma en cada endpoint y visibilidad unificada.*

---

## Pilares: de reactivo a preparado

Según el marco State and Local, los pilares son: detener atacantes habilitados por IA, potenciar al equipo con GenAI, simplificar el stack y unificar datos y defensa. Aquí su traducción al stack soberano.

| Pilar (State/Local) | En nuestro stack soberano |
|---------------------|----------------------------|
| **Stop AI-enabled attackers** — EDR autónomo, remediación 1‑click, rollback | Wazuh (atabey_guardian) como SIEM/detección; Nginx PM como gateway/firewall; Security Fortress y monitoreo de servicios; backups con `sovereign-backup.sh` para recuperación. |
| **Level-up your team** — GenAI, respuestas rápidas, menos carga manual | Atabey Command Center como consola unificada; estado de nodos y servicios; documentación (Ciberseguridad 101, checklist) para procedimientos. |
| **Simplify your stack** — consolidar herramientas, sustituir tech obsoleta | Una plataforma: Matrix (comunicación), Baserow (censo), Nextcloud (bóveda), Wazuh (guardian), Nginx PM (gateway); secret scanning y CI en un solo flujo. |
| **Unify data & defense** — Whole-of-State, una consola, datos correlacionados | Atabey Command Center + Security Fortress; estado final del sistema (`estado-final-sistema.json`); logs y alertas en Wazuh. |

---

## Equivalencias funcionales (sin vendor lock-in)

| Capacidad State/Local | Equivalente soberano |
|------------------------|----------------------|
| **Singularity MDR / EDR autónomo** | Wazuh (detección, reglas, alertas); monitoreo 24/7 vía scripts y Docker health. |
| **Vigilance MDR (expertos 24/7)** | Procedimientos documentados; opción de integrar alertas (email, webhook) desde Wazuh. |
| **Purple AI / GenAI** | Documentación operativa (docs/), checklist y runbooks en este repo. |
| **AI SIEM / visibilidad** | Wazuh como núcleo SIEM; dashboard Atabey para estado de servicios y nodos. |
| **Un solo console** | Atabey Command Center (`/platform/atabey-platform.html`) + Security Fortress (`/platform/security-fortress.html`). |

---

## Preparación para financiación y cumplimiento

La página State and Local menciona **State and Local Cybersecurity Grant Program (SLCGP)**, **FedRAMP** y marcos de cumplimiento. Para entidades soberanas/tribales/estatales que opten a ayudas o quieran alinear controles:

| Tema | Referencia | Acción en nuestro stack |
|------|------------|--------------------------|
| **SLCGP (programa de subvenciones)** | Objetivos: detección y respuesta, visibilidad, resiliencia cibernética | Wazuh + gateway + backups + documentación de postura (este doc y CIBERSEGURIDAD-101). |
| **Cumplimiento (NIST, FISMA, HIPAA, ISO…)** | SentinelOne Trust Center / estándares del sector | Mantener inventario (estado final del sistema), controles de acceso (roles), cifrado (Nextcloud), escaneo de secretos (DevSecOps). |
| **Continuidad de servicios** | Proteger 911, infraestructura crítica, datos ciudadanos | Backups automáticos (`sovereign-backup.sh`), bóveda cifrada (Nextcloud), censo en Baserow; procedimientos en ATABEY-FORTRESS-LIVE. |

*(No sustituye asesoría legal o de cumplimiento; sirve como guía de alineación técnica.)*

---

## Checklist operativa (gobierno estatal/local / soberano)

- [ ] **Servicios críticos:** Census (Baserow), Vault (Nextcloud), Messenger (Matrix), Guardian (Wazuh), Gateway (Nginx PM) en marcha y verificados con `docker ps`.
- [ ] **Un solo punto de mando:** Atabey Command Center y Security Fortress accesibles y usados para estado y alertas.
- [ ] **Detección y logs:** Wazuh operativo; revisión periódica de alertas y reglas.
- [ ] **Backup y recuperación:** `sovereign-backup.sh` en cron; opcional bunker offline (`SOVEREIGN_BUNKER_PATH`).
- [ ] **Secretos y código:** Secret scanning en CI y local (`./scripts/secret-scan.sh`); ver [SECRET-SCANNING.md](./SECRET-SCANNING.md).
- [ ] **Identidad y acceso:** Roles (admin/operator/citizen) definidos y aplicados en la plataforma.
- [ ] **Documentación:** [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md), [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md) y este documento actualizados y accesibles para el equipo.

---

## Documentos relacionados

| Documento | Contenido |
|-----------|-----------|
| [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) | Mapa Ciberseguridad 101 ↔ stack; checklist y enlaces. |
| [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md) | Census, Vault, Guardian, backup y pasos finales. |
| [SECRET-SCANNING.md](./SECRET-SCANNING.md) | Escaneo de secretos (Gitleaks/TruffleHog). |
| [ESTADO-FINAL-SISTEMA.md](./ESTADO-FINAL-SISTEMA.md) | Estado final del sistema y fuentes de verdad. |

---

*Alineado con [State and Local Government Cybersecurity | SentinelOne](https://www.sentinelone.com/es/platform/state-and-local-government/). Objetivo: proteger servicios y datos ciudadanos con defensa proactiva y una plataforma unificada en infraestructura soberana.*
