# REPORTE: Producción — Nada Trabaja

**Fecha:** Febrero 2026  
**Solicitud:** Reporte de por qué nada funciona en producción / test

---

## RESUMEN EJECUTIVO

El sistema está configurado para **localhost** y URLs de desarrollo. En producción real (dominio externo, servidor remoto) muchas partes fallan porque:

1. **URLs hardcodeadas** a `localhost:8545` en ~200+ archivos
2. **Variables de entorno** no configuradas para el host real
3. **CORS** sin dominios de producción
4. **Login y UI** con problemas de loading bloqueante (ya corregidos parcialmente)
5. **Servicios externos** (Rust, Go, Python) apuntan a 127.0.0.1

---

## 1. URLs Y ORIGEN HARDCODEADO

| Problema | Ubicación | Impacto |
|----------|-----------|---------|
| `http://localhost:8545` | ~200 archivos (platform, tokens, node, scripts) | APIs, fetch, links usan localhost en vez del dominio real |
| `ierahkwa_unified_origin` | localStorage — fallback `http://localhost:8545` | Si el usuario nunca visita desde el servidor correcto, queda localhost |
| `BANKING_BRIDGE_URL=http://localhost:3001` | .env | El Node no conecta al Bridge si está en otro host |
| `RUST_SERVICE_URL=http://127.0.0.1:8590` | .env | SWIFT, Queue, ML fallan si servicios no están en el mismo servidor |

**Archivos críticos con localhost:**
- `RuddieSolution/platform/index.html`
- `RuddieSolution/platform/login.html`
- `RuddieSolution/platform/config.json`
- `RuddieSolution/platform/assets/*.js`
- `RuddieSolution/node/server.js`
- Todos los tokens (`tokens/*/index.html`)

---

## 2. VARIABLES DE ENTORNO PARA PRODUCCIÓN

| Variable | Estado | Acción |
|----------|--------|--------|
| `NODE_ENV=production` | En .env.example | Verificar que esté en .env real |
| `JWT_ACCESS_SECRET` | Placeholder en ejemplo | **OBLIGATORIO:** generar con `openssl rand -hex 32` |
| `JWT_REFRESH_SECRET` | Placeholder en ejemplo | **OBLIGATORIO:** igual |
| `CORS_ORIGIN` | Comentado | Definir dominios permitidos: `https://tudominio.gov` |
| `BANKING_BRIDGE_URL` | `http://localhost:3001` | Si Bridge está en otro host: `http://HOST:3001` |
| `PLATFORM_USERS_JSON` o `PLATFORM_ADMIN_PASSWORD` | Opcional | Sin esto, login con usuarios reales falla |

---

## 3. LOGIN Y UI — Problemas Detectados y Corregidos

| Problema | Causa | Estado |
|----------|-------|--------|
| Pantalla "Verificando identidad..." bloqueando todo | `unified-styles.css` `.loading { display: flex }` sobrescribía `display: none` del login | **Corregido** — `#loadingScreen` con `!important` |
| Botones no respondían | Mismo overlay de loading encima | **Corregido** |
| Login "se queda pensando" | Fetch i18n sin timeout; API lenta | **Corregido** — AbortController 3s |
| platform-api-client cargado en login | Script innecesario | **Corregido** — no se carga en login |

---

## 4. PUERTOS Y SERVICIOS NECESARIOS

| Puerto | Servicio | Requerido |
|--------|----------|-----------|
| 8545 | Node Mamey (API + Platform) | Sí |
| 3001 | Banking Bridge | Sí (BDET, Depository, etc.) |
| 8080 | Platform static (alternativa) | No (Node sirve /platform) |
| 8590 | Rust SWIFT | Opcional |
| 8591 | Go Queue | Opcional |
| 8592 | Python ML | Opcional |
| 5054, 5061, 5071... | TradeX, FarmFactory, NET10... | Opcional |

Si solo Node (8545) y Bridge (3001) están arriba, el core debería funcionar. El resto son extras.

---

## 5. CHECKLIST PARA QUE FUNCIONE EN PRODUCCIÓN

```
[ ] 1. .env con:
       - JWT_ACCESS_SECRET, JWT_REFRESH_SECRET (32+ chars)
       - CORS_ORIGIN=https://tudominio.gov
       - BANKING_BRIDGE_URL=http://HOST_BRIDGE:3001 (si aplica)
       - NODE_ENV=production

[ ] 2. Usuarios: PLATFORM_USERS_JSON o PLATFORM_ADMIN_PASSWORD en .env

[ ] 3. Node (8545) y Banking Bridge (3001) corriendo

[ ] 4. Reverse proxy (nginx/Caddy) con HTTPS delante de 8545

[ ] 5. Acceso a la plataforma con la URL real (https://tudominio.gov/platform)
       — NO abrir file:// ni desde otro dominio que no esté en CORS

[ ] 6. localStorage: borrar ierahkwa_unified_origin si quedó localhost
       (o visitar primero la URL correcta para que se guarde)
```

---

## 6. DIAGNÓSTICO RÁPIDO

Ejecutar en el servidor:

```bash
# ¿Node responde?
curl -s http://localhost:8545/health | head -5

# ¿Bridge responde?
curl -s http://localhost:3001/api/health | head -5

# ¿Login existe?
curl -s -o /dev/null -w "%{http_code}" http://localhost:8545/platform/login.html
```

Si `health` devuelve JSON y login devuelve `200`, el backend está bien. El fallo suele ser:
- Frontend cargado desde dominio distinto (CORS)
- Origen guardado en localStorage como localhost
- JWT/usuarios mal configurados

---

## 7. ACCIONES RECOMENDADAS

1. **Crear script de configuración para producción** que reemplace `localhost:8545` por variable de entorno (SERVER_URL o similar) en los puntos críticos.
2. **Documentar** que el usuario debe acceder siempre desde la URL final (ej. `https://app.ierahkwa.gov/platform`) para que `window.location.origin` sea correcto.
3. **Revisar CORS** en `server.js` para incluir el dominio de producción.
4. **Probar** con `NODE_ENV=production` y `.env` real antes de exponer.

---

*Reporte generado a partir del análisis del codebase Ierahkwa Futurehead.*
