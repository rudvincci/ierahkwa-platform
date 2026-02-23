# Stack de software libre — Ejemplos, ventajas/desventajas y uso en IERAHKWA

```
═══════════════════════════════════════════════════════════════════════
    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
    "Todo propio · Nada de terceros" — Software libre alineado
═══════════════════════════════════════════════════════════════════════
```

Este documento alinea el principio **todo propio** con ejemplos populares de software libre, sus ventajas y desventajas, y qué usa o recomienda la plataforma en cada categoría.

---

## 1. Ejemplos populares por categoría

| Categoría   | Ejemplos populares        | Uso típico                          |
|------------|---------------------------|-------------------------------------|
| **Sistemas operativos** | Linux, Ubuntu             | Servidores, escritorio, despliegue   |
| **Navegadores**        | Mozilla Firefox           | Navegación, pruebas de compatibilidad |
| **Ofimática**          | LibreOffice, Apache OpenOffice | Documentos, hojas de cálculo, presentaciones |
| **Bases de datos**     | MySQL, PostgreSQL         | Datos relacionales, APIs, backend   |
| **CMS**                | WordPress                 | Sitios web, blogs, contenido público |

---

## 2. Ventajas y desventajas del software libre

### Ventajas

| Ventaja | Cómo lo aprovechamos en IERAHKWA |
|---------|-----------------------------------|
| **Costes reducidos** | Sin licencias de pago; infra y herramientas propias o open source. |
| **Alta seguridad (revisión comunitaria)** | Código auditable; usamos crypto nativo Node.js, PostgreSQL, Redis, estándares abiertos. |
| **Flexibilidad** | Stack propio (Node, .NET, Rust, Go, Python según servicio); sin vendor lock-in. |
| **Mayor innovación** | Integración de proyectos como Jitsi, FreeSWITCH, Mojaloop, Hyperledger (ver `platform/docs/OPEN-SOURCE-GLOBAL-PARA-PLATAFORMA.md`). |

### Desventajas y mitigación

| Desventaja | Mitigación en la plataforma |
|------------|-----------------------------|
| **Soporte técnico oficial rápido** | Runbooks propios (`RUNBOOK-24-7.md`), playbook de incidentes (`PLAYBOOK-RESPUESTA-INCIDENTES.md`), documentación interna y equipo técnico soberano. |
| **Conocimientos técnicos avanzados** | Documentación en `docs/`, checklists de producción (`PRODUCTION-LIVE-CHECKLIST.md`), guías de deploy y `.env.example` en cada componente. |
| **Compatibilidad o fragmentación** | Stack fijo documentado (Node LTS, PostgreSQL, Redis, MongoDB); pruebas en Firefox y Chrome; despliegue recomendado en Linux/Ubuntu. |

---

## 3. Qué usa o recomienda el proyecto

| Categoría | En IERAHKWA | Notas |
|-----------|-------------|--------|
| **SO** | **Linux / Ubuntu** | Despliegue en servidores (Docker, VPS, racks). Ver `DEPLOY-SERVERS/`, `docs/DEPLOYMENT-GUIDE.md`, `INFRASTRUCTURE-SETUP.md`. |
| **Navegador** | **Mozilla Firefox** (recomendado para pruebas) | Probar dashboards, platform, BDET, Security Fortress en Firefox; compatibilidad con estándares web. Ver checklist producción: “Navegadores y SO”. |
| **Ofimática** | **LibreOffice / OpenOffice** (opcional) | Para escritorio y, si se desea, conversiones servidor (DOCX/ODT → PDF) con LibreOffice headless. Ver `docs/LIBREOFFICE-CONVERSION.md`. Generación de PDF en la plataforma: `pdfkit` (Node) en `node/services/pdf-generator.js`. |
| **Bases de datos** | **PostgreSQL** (principal relacional), **MongoDB** (documentos/logs), **Redis** (caché/cola) | Config: `RuddieSolution/node/database/db-config.js`, `config/services-ports.json`. **MySQL/MariaDB** permitidos como alternativa relacional si se prefiere (mismo principio: software libre). |
| **CMS** | **Contenido propio** (HTML/estáticos, platform) | Sitios institucionales y dashboards desde `platform/` y Node. **WordPress** es software libre; si se desea un blog o sitio público con CMS, puede desplegarse en servidor propio (self-hosted) sin depender de empresas externas. |

---

## 4. Resumen de implementación

- **Ya en uso:** Linux/Ubuntu en deploy, PostgreSQL + MongoDB + Redis, Node.js (crypto nativo), generación PDF con pdfkit.
- **Recomendado:** Probar en **Firefox** antes de producción; documentar en checklist “SO y navegador recomendados”.
- **Opcional:** LibreOffice headless para conversiones DOCX/ODT → PDF (guía en `LIBREOFFICE-CONVERSION.md`); MySQL/MariaDB como alternativa a PostgreSQL; WordPress self-hosted para sitios CMS si se requiere.

---

## 5. Referencias

- `PRINCIPIO-TODO-PROPIO.md` — Regla fundamental, nada de 3ra compañía.
- `.cursor/rules/todo-propio-soberania.mdc` — Criptografía y dependencias.
- `RuddieSolution/platform/docs/OPEN-SOURCE-GLOBAL-PARA-PLATAFORMA.md` — Lista de proyectos open source por dominio (gobierno, finanzas, comunicación, identidad, documentos, salud, votación).
- `RuddieSolution/node/PRODUCTION-LIVE-CHECKLIST.md` — Checklist producción (incl. SO y navegador).
