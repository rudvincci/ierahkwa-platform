# Coordinación entre agentes (Cursor + Claude)

**Proyecto:** IERAHKWA — todo en un solo repo  
**Repo:** https://github.com/rudvincci/ierahkwa-platform

---

## Agentes activos

| Agente | Herramienta | Última sesión |
|--------|-------------|---------------|
| Cursor | IDE + AI (Cursor) | 2026-03-02 |
| Claude | CLI + API (Claude Code) | 2026-03-02 |

---

## Registro de trabajo (actualizar al iniciar/finalizar)

Cada agente debe actualizar al **empezar** y al **terminar** su sesión:

```
[YYYY-MM-DD HH:MM] AGENTE: Cursor | TAREA: subir archivos faltantes | ARCHIVOS: 08-dotnet/... | ESTADO: en progreso
[YYYY-MM-DD HH:MM] AGENTE: Claude | TAREA: fix CORS backend | ARCHIVOS: 03-backend/ | ESTADO: en progreso
```

### Últimas entradas

| Fecha | Agente | Tarea | Archivos | Estado |
|-------|--------|-------|----------|--------|
| 2026-03-02 | Claude | Fix 7 JWT/secretos hardcodeados (auditoría prioridad #1) | `03-backend/voto-soberano/server.js`, `03-backend/ierahkwa-shop/config/index.js`, `03-backend/ierahkwa-shop/src/routes/admin.js`, `03-backend/smart-school-node/src/config/config.js`, `03-backend/smart-school-node/src/server.js`, `03-backend/smart-school-node/src/seeders/seed.js` | completado |
| 2026-03-02 | Cursor | Documentación: FALTANTE-GITHUB, COORDINACION-AGENTES, identificar archivos sin subir | `COORDINACION-AGENTES.md`, `FALTANTE-GITHUB-IERAHKWA-2026-02-27.md` | completado |
| 2026-03-02 | Cursor | v11.0.0-PHANTOM: IerahkwaInheritance, IerahkwaPhantom, transferSovereignty, codec2_voice_bridge, ghost_bridge, snowflake, conciencia-captcha | `08-dotnet/.../DeFiSoberano/contracts/`, `scripts/protocols/`, `04-infraestructura/nginx/`, `docker-compose.sovereign.yml`, `03-backend/conciencia-captcha/` | completado |

---

## Reglas de coordinación

1. **Antes de empezar:** Leer este archivo y el registro. Evitar trabajar los mismos archivos que el otro agente.
2. **Antes de push:** `git pull --rebase` siempre.
3. **Al empezar sesión:** Añadir tu línea al registro con estado `en progreso`.
4. **Al terminar:** Actualizar a `completado` y hacer push de este archivo junto con tus cambios.
5. **Si hay conflicto:** Resolver localmente; si es complejo, dejar nota aquí para el otro agente.

---

## Áreas / ramas por agente (opcional)

Si se divide trabajo por áreas, documentar aquí:

| Área | Agente asignado | Notas |
|------|-----------------|-------|
| 08-dotnet/microservices | Cursor | Stubs .NET, consolidación Platform Unificada |
| 03-backend | Claude | Seguridad, testing, AI/ML |
| 02-plataformas-html | Claude | Frontend, NEXUS portals |
| Documentación (16-docs) | Cursor | Auditorías, planes, faltantes |
| Consolidación repos | Cursor | Mamey-main, Platform Unificada → ierahkwa-platform |
| Arquitectura Mamey híbrida | Claude | Diseño integración MameyNode con backends |
