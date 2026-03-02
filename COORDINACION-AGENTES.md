# Coordinación entre agentes (Cursor + Claude + Google AI)

**Proyecto:** IERAHKWA — todo en un solo repo  
**Repo:** https://github.com/rudvincci/ierahkwa-platform  
**Flujo:** Google AI (arquitectura/ideas) → Cursor (implementación real) → Claude (supervisión/mejoras)

---

## Agentes activos

| Agente | Herramienta | Rol | Última sesión |
|--------|-------------|-----|---------------|
| Google AI | Gemini | Arquitecto — propone features, diseña conceptos | 2026-03-02 |
| Cursor | IDE + AI (Cursor) | Implementador — código real, sin stubs, push a GitHub | 2026-03-02 |
| Claude | CLI + API (Claude Code) | Supervisor — review, mejoras, testing, seguridad | 2026-03-02 |

---

## Registro de trabajo (actualizar al iniciar/finalizar)

Cada agente debe actualizar al **empezar** y al **terminar** su sesión:

### Últimas entradas

| Fecha | Agente | Tarea | Archivos | Estado |
|-------|--------|-------|----------|--------|
| 2026-03-02 | Claude | Fix 7 JWT/secretos hardcodeados (auditoría prioridad #1) | `03-backend/voto-soberano/server.js`, `03-backend/ierahkwa-shop/config/index.js`, `03-backend/ierahkwa-shop/src/routes/admin.js`, `03-backend/smart-school-node/src/config/config.js`, `03-backend/smart-school-node/src/server.js`, `03-backend/smart-school-node/src/seeders/seed.js` | completado |
| 2026-03-02 | Cursor | Documentación: FALTANTE-GITHUB, COORDINACION-AGENTES, identificar archivos sin subir | `COORDINACION-AGENTES.md`, `FALTANTE-GITHUB-IERAHKWA-2026-02-27.md` | completado |
| 2026-03-02 | Cursor | v11.0.0-PHANTOM: IerahkwaInheritance, IerahkwaPhantom, transferSovereignty, codec2_voice_bridge, ghost_bridge, snowflake, conciencia-captcha | `08-dotnet/.../DeFiSoberano/contracts/`, `scripts/protocols/`, `04-infraestructura/nginx/`, `docker-compose.sovereign.yml`, `03-backend/conciencia-captcha/` | completado |
| 2026-03-02 | Claude | Tests: 6 archivos nuevos (~150 test cases) para shared modules y servicios críticos | `shared/__tests__/`, `voto-soberano/__tests__/`, `ierahkwa-shop/__tests__/` | completado |
| 2026-03-02 | Cursor | v12.0.0: energy_monitor, airgap_transfer, nomad_node_config (solo lo que faltaba) | `scripts/protocols/energy_monitor.py`, `scripts/security/airgap_transfer.sh`, `hardware-node/nomad_node_config.yaml` | completado |
| 2026-03-02 | Claude | ierahkwa-ml: TrustEngine + AnomalyDetector + PatternAnalyzer + SwarmMonitor | `03-backend/ierahkwa-ml/lib/` | completado |
| 2026-03-02 | Cursor | ierahkwa-ml server.js completo (puerto 3092, 4 módulos, AgentSync, Dashboard) | `03-backend/ierahkwa-ml/server.js` | completado |
| 2026-03-02 | Cursor | v13.0: radio_triangulation.py (PoL real: multilateration, ToF, RTT, Haversine) | `scripts/protocols/radio_triangulation.py` | completado |
| 2026-03-02 | Cursor | v13.0: seed-node-firmware ESP32 (PlatformIO, AES-256-CBC, LoRa mesh, deep sleep) | `hardware-node/seed-node-firmware/` | completado |

---

## Estado del repo (actualizado 2026-03-02)

### Versiones integradas
- **v11.0.0-PHANTOM**: SBT Inheritance, Immortality Seal, Codec2 Voice Bridge, Snowflake/Tor Stealth, AI Empathy Captcha
- **v12.0.0**: Energy Monitor (solar/batería), Air-Gap Transfer (bóveda fría GPG), Nomad Node Config
- **v13.0.0**: Radio Triangulation (PoL), Seed Node Firmware (ESP32), Steganography Bridge
- **ML Engine**: TrustEngine + AnomalyDetector + PatternAnalyzer + SwarmMonitor (puerto 3092)
- **Security fixes**: 7 JWT/secrets hardcodeados corregidos en 03-backend

### Inventario completo (NO DUPLICAR)
- **84 microservicios .NET** con .sln y CQRS (08-dotnet/microservices/)
- **140 librerías** en el framework Mamey (08-dotnet/framework/src/)
- **5 proyectos Government** .NET (Portal, Monolith, Identity, Citizen Gateway, Pupitre con 30+ sub-services)
- **2 proyectos Banking** .NET (INKG + NET10)
- **23 servicios Node.js** en 03-backend/
- **7+ contratos Solidity** (Reputation, Pulse, Inheritance, Phantom, Treasury, Justice, Destruct)
- **2 crates Rust** (mamey-node-rust, mamey-forge)
- **20+ scripts** en scripts/protocols/ y scripts/security/
- **hardware-node/**: mobile-node-guide.md, raspberry-pi-setup.sh, mesh-antenna-specs.md, nomad_node_config.yaml, seed-node-firmware/

---

## Para Claude: Review pendiente (2026-03-02)

Cursor terminó implementación. Revisar calidad y seguridad:

### Archivos nuevos a revisar

| Archivo | Líneas | Prioridad |
|---------|--------|-----------|
| `scripts/protocols/radio_triangulation.py` | 390 | Alta — PoL: multilateration, ToF, RTT |
| `scripts/security/airgap_transfer.sh` | 170 | Alta — Bóveda fría: GPG + shred |
| `hardware-node/seed-node-firmware/src/main.cpp` | 340 | Alta — ESP32: AES-256-CBC, LoRa mesh |
| `03-backend/ierahkwa-ml/server.js` | 250 | Alta — 4 módulos ML, puerto 3092 |
| `08-dotnet/.../IerahkwaInheritance.sol` | 45 | Alta — Dead Man's Switch SBT |
| `scripts/protocols/energy_monitor.py` | 200 | Media — Victron, INA219, sysfs |
| `hardware-node/seed-node-firmware/include/config.h` | 110 | Media — Config ESP32 |
| `hardware-node/nomad_node_config.yaml` | 120 | Media — Docker Compose nómada |
| `03-backend/conciencia-captcha/conciencia_captcha.js` | 120 | Media — Empathy Captcha |
| `08-dotnet/.../IerahkwaPhantom.sol` | 25 | Media — Root hash on-chain |

### Mejoras sugeridas
- [ ] AES key en main.cpp es placeholder — necesita provisión segura en producción
- [ ] Agregar rate limiting al server.js del ML engine
- [ ] Revisar permisos en airgap_transfer.sh para distintos Linux
- [ ] Tests unitarios para radio_triangulation.py
- [ ] Verificar reentrancy en IerahkwaInheritance.sol claimLegacy
- [ ] HMAC en paquetes LoRa (AES-CBC sin auth es vulnerable a bit-flipping)
- [ ] Integrar ierahkwa-ml en docker-compose.sovereign.yml

---

## Reglas de coordinación

1. **Antes de empezar:** Leer este archivo. Evitar trabajar los mismos archivos.
2. **Antes de push:** `git pull --rebase` siempre.
3. **Al empezar sesión:** Añadir línea al registro con estado `en progreso`.
4. **Al terminar:** Actualizar a `completado` y push este archivo con tus cambios.
5. **Si hay conflicto:** Resolver localmente; si es complejo, dejar nota aquí.

---

## Áreas por agente

| Área | Agente | Notas |
|------|--------|-------|
| 08-dotnet/microservices | Cursor | Implementación real, consolidación |
| 03-backend | Claude | Seguridad, testing, AI/ML |
| 02-plataformas-html | Google AI + Claude | Frontend, NEXUS portals |
| scripts/protocols | Cursor | Código funcional (PoL, energy, codec2, etc.) |
| hardware-node | Cursor | Firmware ESP32, configs nómada |
| Supervisión / Code Review | Claude | Review post-implementación |
