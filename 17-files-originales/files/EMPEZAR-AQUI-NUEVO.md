# Empezar aquí — Plataforma Soberana Unificada

**Una sola plataforma.** Akwesasne + Ierahkwa + Mamey reunidos. Nada eliminado.

---

## Paso 1 — Verificar que todo funciona

```bash
./03-SCRIPTS/health/verificar-todo.sh
```

Esto comprueba: enlaces simbólicos, carpetas, dependencias y puertos.

---

## Paso 2 — Entender la estructura

| Carpeta | Contenido |
|---------|-----------|
| **Akwesasne** | Fotos oficiales PM, soberanos natives, platform, logs, backups |
| **Ierahkwa** | BANCO BDET, BANCO_CENTRAL, CitizenCRM, 60+ plataformas, docs, scripts |
| **Mamey** | Framework técnico: Pupitre, SICB, BIIS, FWID Identities, infra Docker |
| **01-PLATAFORMAS-LIMPIO** | Todo organizado en 15 categorías (ver abajo) |
| **02-SEGURIDAD** | Certificados TLS, firewall, autenticación, backups |
| **03-SCRIPTS** | Arranque, parada, health checks, backups, despliegue |
| **04-CONFIG** | Nginx, Docker, variables de entorno |
| **05-MONITORING** | Prometheus, Grafana, alertas |

*Akwesasne, Ierahkwa y Mamey son enlaces simbólicos a las carpetas originales en el Desktop.*

---

## Paso 3 — Acceso rápido por sistema

### Akwesasne (contenido)
- Fotos y soberanos natives → `Akwesasne/soberanos natives`
- Platform → `Akwesasne/platform`
- Backup → `Akwesasne/BACKUP_IERAHKWA_PLATFORM_*.zip`

### Ierahkwa (sistema)
- Sistema bancario → `Ierahkwa/BANCO BDET`, `Ierahkwa/BANCO_CENTRAL`
- Ciudadanos → `Ierahkwa/CitizenCRM`, `Ierahkwa/CitizenPortal`
- Scripts → `Ierahkwa/start-all-services.sh`, `Ierahkwa/start-ierahkwa.sh`

### Mamey (backend técnico)
- Pupitre → `Mamey/Pupitre`
- SICB, BIIS → `Mamey/`
- Docker → `Mamey/docker-compose.infra.yml`

---

## Paso 4 — Plataformas organizadas

→ **[01-PLATAFORMAS-LIMPIO/](01-PLATAFORMAS-LIMPIO/README.md)** — 15 categorías:

| # | Categoría | Qué incluye |
|---|-----------|-------------|
| 01 | Gobierno | Office PM, documentos oficiales, actas |
| 02 | Bancos | BANCO BDET, BANCO_CENTRAL, sistema bancario indígena |
| 03 | Identidad | Mamey.Government.Identity, FWID, biometría |
| 04 | Blockchain | MameyNode, Chain 777777, contratos, nodos |
| 05 | Compliance ZKP | Zero-Knowledge Proofs, auditoría, regulación |
| 06 | Tesorería | SICB Treasury, SICBDC, WAMPUM, IGT |
| 07 | AI | Inteligencia artificial, modelos, automatización |
| 08 | Biometría | Reconocimiento facial, huellas, autenticación |
| 09 | DeFi | Finanzas descentralizadas, protocolos |
| 10 | ERP | Gestión empresarial, recursos, planificación |
| 11 | Mobile | Apps móviles, PWA |
| 12 | Educación | Capacitación, documentación técnica |
| 13 | Oficina | Herramientas administrativas |
| 14 | Infraestructura | Docker, servidores, redes, DNS |
| 15 | CRM Ciudadanos | CitizenCRM, CitizenPortal, membresía |

---

## Paso 5 — Arrancar servicios

```bash
# Primero aplicar seguridad (solo la primera vez)
./03-SCRIPTS/security/hardening.sh

# Arrancar
./03-SCRIPTS/start/start-mamey.sh

# Ver estado
./03-SCRIPTS/health/health-check.sh

# Parar
./03-SCRIPTS/stop/stop-mamey.sh
```

---

## Si algo falla

```bash
# Diagnóstico completo
./03-SCRIPTS/health/verificar-todo.sh

# Ver logs
ls -la Mamey/logs/

# Recrear enlaces simbólicos
./03-SCRIPTS/deploy/recrear-enlaces.sh
```

---

## Documentación completa

| Documento | Ubicación |
|-----------|-----------|
| Arquitectura | [00-DOCS/ARQUITECTURA.md](00-DOCS/ARQUITECTURA.md) |
| Seguridad | [00-DOCS/SEGURIDAD.md](00-DOCS/SEGURIDAD.md) |
| Sistema bancario | [00-DOCS/SISTEMA-BANCARIO.md](00-DOCS/SISTEMA-BANCARIO.md) |
| Disaster recovery | [00-DOCS/DISASTER-RECOVERY.md](00-DOCS/DISASTER-RECOVERY.md) |
| Auditoría | [00-DOCS/AUDITORIA.md](00-DOCS/AUDITORIA.md) |
