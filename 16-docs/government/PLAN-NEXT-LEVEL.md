# 🚀 PLAN NEXT LEVEL — Take Care of All

**IERAHKWA Sovereign Platform — Roadmap to Production & Excellence**  
**Fecha:** Febrero 2026

---

## ✅ APLICADO (Feb 2026)

| Item | Estado |
|------|--------|
| Origen dinámico | index, login, telecom guardan `location.origin` en localStorage cuando se visita desde dominio real |
| unified-styles .loading | `display: none` por defecto; `.loading.active` para mostrar (evita overlays bloqueantes) |
| setup-production-env.sh | Script que genera JWT y crea .env desde .env.example |
| inject-server-url.js | `SERVER_URL=https://... node scripts/inject-server-url.js` inyecta apiOrigin en config.json |
| GO-LIVE-PRODUCTION.sh | Llama setup-production-env.sh si falta .env |
| AI Platform | Exo 2, fondo fortress, header neon-green |
| Quantum Platform | Header fortress-style, fuentes Orbitron+Exo 2 |
| Telecom Platform | Persistencia de origen real |

---

## RESUMEN

Este plan ordena las acciones para llevar la plataforma al siguiente nivel: que funcione en producción, tenga diseño unificado, y sea mantenible y escalable.

---

## FASE 1 — QUE FUNCIONE (Producción)  
**Prioridad: CRÍTICA · Plazo: 1–2 semanas**

### 1.1 Variables de entorno reales
- [ ] Crear `.env` desde `.env.example` (nunca commitear .env)
- [ ] Generar JWT secrets: `openssl rand -hex 32` para ACCESS y REFRESH
- [ ] Definir `CORS_ORIGIN=https://tudominio.gov` (dominio real)
- [ ] Configurar `PLATFORM_USERS_JSON` o `PLATFORM_ADMIN_PASSWORD` para login

### 1.2 Origen dinámico (quitar localhost hardcodeado)
- [ ] Crear variable `SERVER_URL` o `API_BASE` en config/env
- [ ] Script que inyecte el origen correcto en los HTML críticos (index, login, config.json)
- [ ] Fallback: `window.location.origin` cuando esté en el servidor correcto
- [ ] Documentar: usuario debe acceder siempre desde la URL real (no file://)

### 1.3 Servicios core
- [ ] Node (8545) + Banking Bridge (3001) corriendo y verificados
- [ ] `curl http://HOST:8545/health` y `curl http://HOST:3001/api/health` OK
- [ ] Reverse proxy (nginx/Caddy) con HTTPS delante de 8545

### 1.4 Checklist pre-live
- [ ] JWT secrets reales
- [ ] CORS configurado
- [ ] Usuarios admin/leader configurados
- [ ] HTTPS activo

---

## FASE 2 — DISEÑO UNIFICADO  
**Prioridad: ALTA · Plazo: 2–3 semanas**

*(Ver AUDITORIA-MEJOR-VISTA.md para detalle)*

### 2.1 Una sola fuente de estilos
- [ ] Consolidar todo en `unified-styles.css` (incluir lo bueno de Security Fortress)
- [ ] Eliminar variables `:root` duplicadas en páginas HTML
- [ ] Fuentes únicas: Orbitron (títulos) + Exo 2 (cuerpo) + JetBrains Mono (código)

### 2.2 index.html como hub coherente
- [ ] Usar `unified-styles.css` + `unified-header`
- [ ] Mismo fondo: `#0a0e17 → #001a00 → #0a0e17`
- [ ] Reducir HTML (mover lógica a JS)

### 2.3 Migrar páginas prioritarias
Orden:
1. ai-platform.html  
2. quantum-platform.html  
3. telecom-platform.html  
4. dashboard.html  
5. bdet-bank.html  

Para cada una: quitar estilos inline, usar unified-header + header-bar.

### 2.4 node/public (BDET, Treasury, etc.)
- [ ] Alinear paleta con gold/neon-green/neon-cyan
- [ ] Usar Orbitron + Exo 2

### 2.5 Regla de oro
- Nueva página = partir de `template-unified.html`
- Una sola fuente: unified-styles.css
- Variaciones solo por variables o data-attrs

---

## FASE 3 — CALIDAD TÉCNICA  
**Prioridad: MEDIA · Plazo: 3–4 semanas**

### 3.1 URLs y configuración
- [ ] Centralizar API base en un solo lugar (config.json o env)
- [ ] Script de build/pre-deploy que reemplace placeholders por `SERVER_URL`
- [ ] Tokens (100+): revisar si todos usan origen dinámico

### 3.2 Scripts y dependencias
- [ ] Evitar cargar scripts innecesarios en login (ya hecho: platform-api-client)
- [ ] Timeouts en fetches críticos (ya hecho: i18n 3s)
- [ ] Manejo de errores en llamadas API (loading states, mensajes claros)

### 3.3 Testing
- [ ] Smoke tests: login, health, platform load
- [ ] Script `scripts/production-ready-check.sh` ejecutado antes de cada deploy

---

## FASE 4 — SEGURIDAD  
**Prioridad: ALTA · Plazo: 1–2 semanas**

- [ ] JWT secrets fuertes y rotables
- [ ] HTTPS obligatorio en producción
- [ ] Rate limiting en endpoints sensibles (ya aplicado en varios)
- [ ] Revisar CORS para no dejar `*` en producción
- [ ] `LIVE_REQUIRE_AUTH=1` si se usan canales sensibles (kms, ml)

---

## FASE 5 — EXPERIENCIA DE USUARIO  
**Prioridad: MEDIA · Plazo: continuo**

### 5.1 Login
- [x] Loading no bloqueante (corregido)
- [x] Timeout i18n (corregido)
- [ ] Mensajes claros cuando falla (credenciales, red)
- [ ] Opción “recordar sesión” bien documentada

### 5.2 Navegación
- [ ] Breadcrumbs consistentes
- [ ] “Regresar al Dashboard” siempre visible
- [ ] Mobile: menú colapsable, touch-friendly

### 5.3 Performance
- [ ] Lazy load de plataformas no críticas
- [ ] Comprimir assets (gzip/brotli)
- [ ] Reducir HTML inline donde sea posible

---

## FASE 6 — ESCALABILIDAD  
**Prioridad: BAJA · Plazo: futuro**

- [ ] Clustering (USE_CLUSTER=true) en producción
- [ ] Redis para sesiones si hay múltiples instancias
- [ ] Logs centralizados (ELK o similar)
- [ ] Health checks automáticos (monitoring)

---

## CRONOGRAMA SUGERIDO

| Semana | Foco | Entregables |
|--------|------|-------------|
| 1 | Fase 1.1–1.3 | .env listo, servicios verificados, health OK |
| 2 | Fase 1.4 + Fase 4 | Checklist pre-live, HTTPS, seguridad básica |
| 3 | Fase 2.1–2.2 | unified-styles consolidado, index unificado |
| 4 | Fase 2.3–2.4 | 5 plataformas migradas, node/public alineado |
| 5+ | Fase 3, 5, 6 | URLs dinámicas, UX, escalabilidad |

---

## COMANDOS ÚTILES

```bash
# Verificar producción
./scripts/production-ready-check.sh

# Arrancar todo
./GO-LIVE-PRODUCTION.sh

# Estado
./status.sh
pm2 status
```

---

## DOCUMENTOS DE REFERENCIA

- `REPORTE-PRODUCCION-NADA-TRABAJA.md` — Por qué falla en producción
- `REPORTE-PLATAFORMA-POR-PLATAFORMA.md` — Listado de 103+ plataformas
- `platform/docs/AUDITORIA-MEJOR-VISTA.md` — Diseño visual unificado
- `GO-LIVE-CHECKLIST.md` — Checklist go-live
- `node/ENV-PRODUCTION-SETUP.md` — Setup .env para producción

---

*Plan Next Level — Take care of all. Sovereign Government of Ierahkwa Ne Kanienke.*
