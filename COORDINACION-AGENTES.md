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
| 2026-03-02 | Claude | Tests: 6 archivos nuevos (~150 test cases) para shared modules y servicios críticos | `shared/__tests__/{security,error-handler,logger,audit}.test.js`, `voto-soberano/__tests__/voting-logic.test.js`, `ierahkwa-shop/__tests__/admin-auth.test.js` | completado |
| 2026-03-02 | Cursor | v12.0.0: energy_monitor, airgap_transfer, nomad_node_config (solo lo que faltaba, sin duplicar) | `scripts/protocols/energy_monitor.py`, `scripts/security/airgap_transfer.sh`, `hardware-node/nomad_node_config.yaml` | completado |
| 2026-03-02 | Claude | ierahkwa-ml: TrustEngine + AnomalyDetector (trust-engine.js, anomaly-detector.js, package.json) | `03-backend/ierahkwa-ml/lib/trust-engine.js`, `03-backend/ierahkwa-ml/lib/anomaly-detector.js`, `03-backend/ierahkwa-ml/package.json` | completado |
| 2026-03-02 | Cursor | ierahkwa-ml server.js (entry point faltante) + subir todo pendiente a GitHub | `03-backend/ierahkwa-ml/server.js`, `COORDINACION-AGENTES.md` | completado |
| 2026-03-02 | Cursor | Fix server.js: puerto 3092, rutas PatternAnalyzer + SwarmMonitor + AgentSync + Dashboard | `03-backend/ierahkwa-ml/server.js` | completado |
| 2026-03-02 | Cursor | v13.0: radio_triangulation.py (PoL real: multilateration, ToF, RTT, Haversine, CLI) | `scripts/protocols/radio_triangulation.py` | completado |
| 2026-03-02 | Cursor | v13.0: seed-node-firmware ESP32 real (PlatformIO, AES-256, LoRa mesh, deep sleep, vote, PoL pong) | `hardware-node/seed-node-firmware/` (platformio.ini, config.h, main.cpp) | completado |

---

## Estado del repo (actualizado 2026-03-02)

### Versiones integradas
- **v11.0.0-PHANTOM**: SBT Inheritance, Immortality Seal, Codec2 Voice Bridge, Snowflake/Tor Stealth, AI Empathy Captcha
- **v12.0.0**: Energy Monitor (solar/batería), Air-Gap Transfer (bóveda fría GPG), Nomad Node Config (Docker Compose ligero)
- **ML Engine**: TrustEngine (scoring multi-factor) + AnomalyDetector (Z-Score, EMA, IQR ensemble)
- **Security fixes**: 7 JWT/secrets hardcodeados corregidos en 03-backend

### Archivos verificados que YA EXISTEN (no duplicar)
- `scripts/protocols/`: mediator, sentinel, codec2, lora, satellite_uplink, chaos_scheduler, peace_oracle, survival_sync, shamir, bio_ledger, dna_encoder, ipfs, js8call, welcome_bot, notify_guardians, energy_monitor
- `scripts/security/`: harden-server.sh, tactical-wipe.sh, airgap_transfer.sh
- `hardware-node/`: mobile-node-guide.md, raspberry-pi-setup.sh, mesh-antenna-specs.md, nomad_node_config.yaml
- `sovereign-dns/`: setup_hns.sh, handshake-config.json
- `08-dotnet/contracts/`: IerahkwaReputation, IerahkwaPulse, IerahkwaInheritance, IerahkwaPhantom, IerahkwaDestruct, IerahkwaTreasury, SovereignJustice
- `03-backend/conciencia-captcha/`: conciencia_captcha.js, Dockerfile, package.json
- `03-backend/ierahkwa-ml/`: server.js, lib/trust-engine.js, lib/anomaly-detector.js, package.json

### Próximos pasos sugeridos
- [ ] Compilar contratos Solidity: `cd 08-dotnet/microservices/DeFiSoberano && npx hardhat compile`
- [ ] Tests de ierahkwa-ml: `cd 03-backend/ierahkwa-ml && npm test`
- [ ] Verificar tests: `cd 03-backend/shared && npm test`
- [ ] Integrar ierahkwa-ml en docker-compose.sovereign.yml
- [ ] Añadir [Authorize] faltantes en páginas .NET (auditoría SEC-02)

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
