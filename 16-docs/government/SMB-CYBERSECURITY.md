# Ciberseguridad para pequeña empresa / negocio — Soberanía Ierahkwa

Alineación con **[Cybersecurity for Small Business](https://www.sentinelone.com/platform/small-business/)** de SentinelOne: protección fácil de implementar, bloqueo de ransomware y malware, y checklist tipo "Ten Step" aplicado al stack soberano (Atabey Fortress, plataforma única). **Checklist operativo unificado:** [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) (no duplicar pasos).

---

## Enlace de referencia

| Recurso | URL |
|--------|-----|
| **Cybersecurity for Small Business (SentinelOne)** | https://www.sentinelone.com/platform/small-business/ |

*Protección de Windows, Mac, móviles y servidores; plataforma fácil de usar; bloqueo efectivo de amenazas; respaldo de analistas (Gartner, MITRE).*

---

## Pilares "More Protection. Less Worry" ↔ nuestro stack

| Pilar (Small Business) | En nuestro stack soberano |
|------------------------|----------------------------|
| **Easy to Use** — set-and-forget, enfocarse en el negocio | Una consola (Atabey Command Center), backup con un script (`sovereign-backup.sh`), checklist en un solo comando (`smb-security-check.sh`). |
| **Effective Threat Blocking** — ransomware y malware, siempre activo | Wazuh (Guardian), Nginx PM (gateway), cifrado en Nextcloud (Vault), secret scanning en código, backups automáticos. |
| **Trusted Industry-Wide** — líder en pruebas y cobertura de dispositivos | Controles documentados (Ciberseguridad 101, NIST/SLCGP referenciados), scripts repetibles y CI (secret scan, backup). |

---

## Checklist tipo "Ten Step" para negocio soberano

Adaptado al stack Atabey/Ierahkwa (equivalente funcional al [Ten Step Checklist](https://www.sentinelone.com/platform/small-business/) de SentinelOne para SMB):

1. **Backups automáticos** — `sovereign-backup.sh` en cron; opcional bunker (`SOVEREIGN_BUNKER_PATH`).
2. **Backup reciente verificado** — existe al menos un backup en `sovereign_backups/atabey_*` (ver `cloud-data-security-check.sh`).
3. **Vault cifrada** — Nextcloud con cifrado de servidor activado (Grandfather's Archive).
4. **Censo/configuración** — Baserow (Census) o datos críticos en lugar conocido y respaldado.
5. **Gateway/firewall** — Nginx Proxy Manager configurado (proxy, HTTPS donde aplique).
6. **Sin secretos en código** — `./scripts/secret-scan.sh` y CI (Gitleaks/TruffleHog); sin credenciales en repo.
7. **Servicios en marcha** — Contenedores Atabey (database, cloud, messenger, guardian, gateway) Up cuando se usa Docker (`docker ps`).
8. **Identidad y acceso** — Roles (admin/operator/citizen) definidos; login y sesión operativos.
9. **Visibilidad** — Atabey Command Center y/o Security Fortress accesibles; estado de nodos conocido.
10. **Documentación y respuesta** — Procedimientos en docs (ATABEY-FORTRESS-LIVE, CLONE-SETUP, CIBERSEGURIDAD-101); scripts de respuesta (panic/listener) conocidos.

**Comprobación rápida en código:** ejecutar `./scripts/smb-security-check.sh` (resume backup, datos, opcional Docker y recordatorio de secret scan).

---

## Implementación en código

| Componente | Descripción |
|------------|-------------|
| **scripts/smb-security-check.sh** | Comprueba backup reciente, datos (Vault/Census), opcional estado Docker Atabey; recuerda secret-scan. Una sola ejecución para "readiness" tipo SMB. |
| **scripts/cloud-data-security-check.sh** | Integridad y listado del último backup; ver [CLOUD-DATA-SECURITY.md](./CLOUD-DATA-SECURITY.md). |
| **scripts/secret-scan.sh** | Escaneo de secretos en repo; ver [SECRET-SCANNING.md](./SECRET-SCANNING.md). |
| **sovereign-backup.sh** | Backup de Census, Vault, Messenger, Gateway; opcional clone a bunker. |

---

## Recursos SentinelOne SMB (referencia)

- [Ten Step Checklist for Successful Small Business Cybersecurity](https://www.sentinelone.com/platform/small-business/)
- [NIST Cybersecurity Framework: A Proactive SMB Action Plan](https://www.sentinelone.com/platform/small-business/)
- [Cybersecurity Best Practices for SMBs](https://www.sentinelone.com/platform/small-business/)

Para alineación con NIST y buenas prácticas SMB en nuestro stack: ver [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) y [SEGURIDAD-GOBIERNO-ESTATAL-Y-LOCAL.md](./SEGURIDAD-GOBIERNO-ESTATAL-Y-LOCAL.md).

---

*Alineado con [Cybersecurity for Small Business \| SentinelOne](https://www.sentinelone.com/platform/small-business/). Objetivo: protección fácil de usar, efectiva y documentada para negocio/soberanía con una sola plataforma.*
