# Cloud Data Security — Soberanía Ierahkwa

Alineación con **[Singularity™ Cloud Data Security](https://www.sentinelone.com/platform/singularity-cloud-data-security/)** de SentinelOne: detección de amenazas en almacenamiento, respuesta automatizada y escaneo local y conforme a cumplimiento. En nuestro stack soberano el “cloud storage” es la **Vault (Nextcloud)**, los **backups** y los **datos censales (Baserow)**. **Checklist operativo unificado:** [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) (no duplicar pasos).

---

## Enlace de referencia

| Recurso | URL |
|--------|-----|
| **Singularity Cloud Data Security** | https://www.sentinelone.com/platform/singularity-cloud-data-security/ |

*Detección con IA para S3, Azure Blob, NetApp; respuesta automatizada (cuarentena); escaneo local y cumplimiento (PCI-DSS, HIPAA, GLBA).*

---

## Equivalencia en nuestro stack

| Capacidad (Cloud Data Security) | En nuestro stack soberano |
|---------------------------------|----------------------------|
| **Detección sin demora** — malware y zero-day en almacenamiento | Cifrado servidor en Nextcloud (Vault); integridad de backups con `cloud-data-security-check.sh`; secret scanning en código (Gitleaks/TruffleHog). |
| **Respuesta automatizada** — cuarentena de objetos maliciosos | Nextcloud en contenedor aislado (red Docker); backups inmutables por fecha; procedimientos de restauración desde backup. |
| **Local y conforme** — escaneo en el propio almacenamiento, datos sensibles no salen | Datos en tu hardware (Nextcloud, Baserow, backups en `sovereign_backups` o bunker); sin envío a terceros; documentación de controles para alineación con marcos. |

No usamos S3/Azure Blob/NetApp; el equivalente funcional es: **Nextcloud (Vault)** + **sovereign_backup** + **Baserow (Census)** + **secret scanning** + **script de verificación** de backups.

---

## Implementación concreta

| Componente | Descripción |
|------------|-------------|
| **Nextcloud (Vault)** | Cifrado de servidor activado; carpeta “Grandfather's Archive” para documentos soberanos. Ver [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md). |
| **Backups** | `sovereign-backup.sh` — copia Census, Vault, Messenger, Gateway (y opcional bunker). |
| **Verificación de integridad** | `scripts/cloud-data-security-check.sh` — comprueba que exista el último backup y lista archivos/tamaños; opcional checksum. |
| **Secret scanning** | Gitleaks/TruffleHog en repo y CI; evita credenciales en código. Ver [SECRET-SCANNING.md](./SECRET-SCANNING.md). |
| **Wazuh (Guardian)** | Monitoreo y logs; detección de anomalías a nivel host/servicio. |

---

## Checklist Cloud Data Security

- [ ] **Vault cifrada:** Nextcloud con cifrado de servidor activado.
- [ ] **Backups automáticos:** `sovereign-backup.sh` en cron; opcional `SOVEREIGN_BUNKER_PATH` para clonar a bunker.
- [ ] **Verificación periódica:** ejecutar `./scripts/cloud-data-security-check.sh` (o en cron) para validar último backup.
- [ ] **Secretos:** CI y local con `./scripts/secret-scan.sh`; sin credenciales en repo.
- [ ] **Aislamiento:** Contenedores Atabey en red `atabey_fortress`; Nginx PM como único punto de entrada público si aplica.

---

## Documentos relacionados

| Documento | Contenido |
|-----------|-----------|
| [ATABEY-FORTRESS-LIVE.md](./ATABEY-FORTRESS-LIVE.md) | Vault, Census, Guardian, backup y pasos finales. |
| [SECRET-SCANNING.md](./SECRET-SCANNING.md) | Escaneo de secretos en código. |
| [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md) | Mapa general SentinelOne ↔ stack. |

---

*Alineado con [Singularity™ Cloud Data Security](https://www.sentinelone.com/platform/singularity-cloud-data-security/). Objetivo: proteger los datos en almacenamiento soberano (Vault, backups, Census) con detección, respuesta y controles locales.*
