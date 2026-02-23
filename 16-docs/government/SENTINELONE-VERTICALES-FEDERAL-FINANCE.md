# Verticales Federal, Finance y Manufacturing — Soberanía Ierahkwa

Referencia a los marcos **[Federal Government](https://www.sentinelone.com/platform/federal-government/)**, **[Finance](https://www.sentinelone.com/platform/finance/)** y **[Manufacturing](https://www.sentinelone.com/platform/manufacturing/)** de SentinelOne. En este repo **no se duplica** checklist ni scripts: se reutiliza el mismo stack y el **checklist operativo único** de [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md). Para **Manufacturing**, el **código clone** (recuperación rápida, “keep producing”) es [CLONE-SETUP.md](./CLONE-SETUP.md) y los scripts `clone-repo.sh` y `clone-from-backup.sh`.

---

## Enlaces SentinelOne

| Recurso | URL |
|--------|-----|
| **Federal Government** | https://www.sentinelone.com/platform/federal-government/ |
| **Finance** | https://www.sentinelone.com/platform/finance/ |
| **Manufacturing** | https://www.sentinelone.com/platform/manufacturing/ |

---

## Equivalencia en nuestro stack (sin duplicar)

| Vertical | Conceptos clave SentinelOne | En nuestro stack (mismo que 101 / Estado local / SMB) |
|----------|----------------------------|-------------------------------------------------------|
| **Federal Government** | FedRAMP High, AWS GovCloud, CISA CDM, NIST SP 800-53; Cloud SaaS, on‑premise, air‑gapped; defensa de activos críticos | Wazuh (Guardian), Nginx PM (gateway), backups (`sovereign-backup.sh`), cifrado Nextcloud (Vault), secret scanning; despliegue on‑prem / bunker air‑gap; inventario en `estado-final-sistema.json`. Cumplimiento: alinear controles con NIST según [SEGURIDAD-GOBIERNO-ESTATAL-Y-LOCAL.md](./SEGURIDAD-GOBIERNO-ESTATAL-Y-LOCAL.md). |
| **Finance** | GLBA, PCI DSS, GDPR; detección/remediación autónoma; reducción de superficie de ataque; recuperación automatizada | Mismo stack: Wazuh (detección), gateway, Vault cifrada, backups, secret scanning; procedimientos en docs y [CLOUD-DATA-SECURITY.md](./CLOUD-DATA-SECURITY.md). Datos sensibles en infraestructura soberana (Nextcloud, Baserow). |
| **Manufacturing** | Uptime, “Stop threats. Keep producing.”; ransomware; recuperación automática/rollback; multi-site; visibilidad de red | Mismo stack + **código clone** para recuperación: [CLONE-SETUP.md](./CLONE-SETUP.md), `scripts/clone-repo.sh`, `scripts/clone-from-backup.sh`. Restaurar desde backup en otro host y levantar servicios (`docker-compose up -d`) para recuperar operación; backups con `sovereign-backup.sh`; Wazuh y gateway para detección. |

**Checklist operativo:** usar el de [CIBERSEGURIDAD-101.md](./CIBERSEGURIDAD-101.md)#checklist-rápido-alineado-a-101. No se añaden scripts nuevos; usar `smb-security-check.sh` y `cloud-data-security-check.sh` para comprobaciones rápidas. **Recuperación (clone):** [CLONE-SETUP.md](./CLONE-SETUP.md).

---

*Referencia a [Federal Government](https://www.sentinelone.com/platform/federal-government/), [Finance](https://www.sentinelone.com/platform/finance/) y [Manufacturing](https://www.sentinelone.com/platform/manufacturing/) \| SentinelOne. Sin duplicar contenido ni artefactos.*
