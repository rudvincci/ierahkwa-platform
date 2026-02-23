# Próximos pasos — Producción y evolución

Lista única de tareas opcionales o posteriores al go-live. Cada ítem enlaza a la documentación donde se explica cómo hacerlo.

---

## 1. Publicar en producción (cuando quieras)

| Tarea | Dónde está | Acción |
|-------|------------|--------|
| **HTTPS** | [DEPLOY-SERVERS/HTTPS-REVERSE-PROXY-EXAMPLE.md](../DEPLOY-SERVERS/HTTPS-REVERSE-PROXY-EXAMPLE.md) | Configurar Nginx/Caddy con SSL delante del Node (8545). |
| **.env completo** | [RuddieSolution/platform/PRODUCTION-SETUP.md](../RuddieSolution/platform/PRODUCTION-SETUP.md) | Copiar `RuddieSolution/node/.env.example` → `.env` y rellenar PORT, CORS_ORIGIN, JWT_*, PLATFORM_*, LOG_DIR, etc. |
| **Dominio** | [PRODUCTION-SETUP.md](../RuddieSolution/platform/PRODUCTION-SETUP.md) + [PRODUCTION-CHECKLIST.md](../RuddieSolution/platform/PRODUCTION-CHECKLIST.md) | Configurar en proxy y en `CORS_ORIGIN` (ej. `https://app.ierahkwa.gov`). |
| **Proxy** | [HTTPS-REVERSE-PROXY-EXAMPLE.md](../DEPLOY-SERVERS/HTTPS-REVERSE-PROXY-EXAMPLE.md) | Reverse proxy con health check a `/health` y `/api/v1/atabey/status`. |

**Resumen:** Todo el detalle está en **PRODUCTION-SETUP.md** y **PRODUCTION-CHECKLIST.md** (misma carpeta `RuddieSolution/platform/`).

---

## 2. Más verticales en ATABEY

| Tarea | Dónde | Acción |
|-------|--------|--------|
| Añadir productos/servicios a la vista | `RuddieSolution/platform/data/platform-links.json` | Añadir entradas con `id`, `label`, `url`, `section`, etc. |
| Enlazar en ATABEY | `RuddieSolution/platform/atabey-platform.html` | Añadir pestaña o card que apunte a la nueva URL (p. ej. en "Plataformas seguridad" o en la pestaña correspondiente). |

Referencia: [LISTA-MAESTRA-PLATAFORMAS.md](LISTA-MAESTRA-PLATAFORMAS.md).

---

## 3. Usar i18n en más pantallas

| Tarea | Dónde | Acción |
|-------|--------|--------|
| API de idiomas | Node: `GET /api/v1/i18n/:lang`, `GET /api/v1/i18n/languages` | Ya disponible (en, es, moh, tai) con keys: welcome, dashboard, login, atabey, backup, security, income, whistleblower, compliance, etc. |
| En el HTML | Cualquier `.html` en `RuddieSolution/platform/` | Añadir `data-i18n="key"` a los elementos de texto; en el JS cargar `GET /api/v1/i18n/:lang` y reemplazar `el.textContent = translations[key]`. |
| Ejemplo | `RuddieSolution/platform/index.html` (líneas ~2978–3024) | Ver uso de `data-i18n` y `data-i18n-placeholder` y el script que aplica traducciones. |

Referencia: [I18N-IDIOMAS.md](I18N-IDIOMAS.md).

---

## 4. Concienciación en seguridad

| Tarea | Dónde | Acción |
|-------|--------|--------|
| Material escrito | [CONCIENCIACION-SEGURIDAD.md](CONCIENCIACION-SEGURIDAD.md) | Ya existe (buenas prácticas). |
| **Página web** | [RuddieSolution/platform/concienciacion-seguridad.html](../RuddieSolution/platform/concienciacion-seguridad.html) | ✅ Creada; enlazada desde ATABEY → Cumplimiento. |
| Taller o checklist interno | Opcional | Usar la página como base para sesión formativa o checklist para equipos. |

---

## 5. Mamey / SICB real

| Tarea | Dónde | Acción |
|-------|--------|--------|
| Stubs actuales | `RuddieSolution/node/routes/sicb-mamey-stubs.js` | Rutas `/api/v1/sicb/*` y `/api/v1/mamey/*` devuelven 501 o JSON con `stub: true`. |
| Integración real | Cuando existan componentes Rust/.NET (MameyNode, SICB) | Sustituir las respuestas de los stubs por llamadas a las APIs reales (misma URL o nuevo cliente HTTP dentro del Node). |

Referencia: [FALTANTES-PARA-PRODUCCION.md](../FALTANTES-PARA-PRODUCCION.md) (raíz del repo); [ROADMAP-ALCANCE-Y-NECESIDADES.md](ROADMAP-ALCANCE-Y-NECESIDADES.md) sección 2.

---

## 6. ZKP real

| Tarea | Dónde | Acción |
|-------|--------|--------|
| Placeholder actual | `RuddieSolution/node/modules/zkp-privacy.js` | `GET /api/v1/zkp/status`; `POST /prove` y `POST /verify` devuelven 501. |
| Documentación | [ZKP-PRIVACY.md](ZKP-PRIVACY.md) | Objetivo y referencias. |
| Integración real | Cuando exista Mamey.SICB.ZeroKnowledgeProofs | Implementar prove/verify en este módulo (llamando a la lib o servicio correspondiente). |

---

## Orden sugerido

1. **Para publicar:** 1 (HTTPS, .env, dominio, proxy).  
2. **Cuando añadas servicios:** 2 (verticales en ATABEY).  
3. **Para mejorar UX por idioma:** 3 (i18n en más pantallas).  
4. **Interno:** 4 (concienciación).  
5. **Cuando tengáis componentes:** 5 y 6 (Mamey/SICB y ZKP reales).
