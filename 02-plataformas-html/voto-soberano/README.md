# Voto Soberano — Ierahkwa

> Plataforma de votacion digital en blockchain para 574 naciones tribales - Ierahkwa Ne Kanienke

## Resumen

**Voto Soberano** es una plataforma del ecosistema **Ierahkwa Ne Kanienke**, parte de **NEXUS Consejo (Gobierno Digital)**. Disenada para la soberania electoral de 72 millones de personas indigenas en 19 naciones y 574 tribus.

Cada voto se registra como transaccion inmutable en MameyChain, con encriptacion post-quantum Kyber-768, balotaje anonimo y verificacion publica completa.

## Caracteristicas Principales

1. **Verificacion en Blockchain** — Cada voto registrado inmutablemente en MameyChain
2. **Una Persona, Un Voto** — Identidad biometrica descentralizada con pruebas de conocimiento cero
3. **Balotaje Anonimo** — Protocolo de mezcla criptografica para privacidad total
4. **Resultados en Tiempo Real** — Conteo instantaneo y transparente en vivo
5. **Encriptacion Post-Quantum** — CRYSTALS-Kyber-768 contra ataques cuanticos
6. **Pista de Auditoria** — Trazabilidad forense completa sin comprometer anonimato
7. **Multi-Idioma** — 200+ lenguas indigenas soportadas
8. **Listo para Movil** — PWA offline-first, vota desde cualquier dispositivo
9. **Gobernanza DAO** — Organizacion autonoma descentralizada con voto proporcional
10. **Democracia Liquida** — Delegacion flexible con revocacion instantanea

## Secciones de la Plataforma

| Seccion | Descripcion |
|---------|-------------|
| Login Soberano | Autenticacion con identidad soberana Ierahkwa |
| Elecciones Activas | Lista de votaciones abiertas con estado y progreso |
| Modal de Votacion | Interfaz de emision de voto con confirmacion en blockchain |
| Resultados | Graficos de barras en tiempo real verificables en cadena |
| Mi Historial | Registro personal de votos con hashes de transaccion |
| Crear Eleccion | Herramienta administrativa para nuevas votaciones |

## Arquitectura

```
┌─────────────────────────────────────┐
│          voto-soberano              │
├─────────────────────────────────────┤
│  Frontend    │  HTML5 + CSS3 + JS   │
│  Design      │  ierahkwa.css        │
│  Security    │  Post-Quantum        │
│  AI Agents   │  7 Agentes Activos   │
│  Blockchain  │  MameyChain          │
│  Network     │  PWA + Offline-First │
│  NEXUS       │  consejo             │
└─────────────────────────────────────┘
```

## Tecnologias

- **Frontend**: HTML5 semantico, CSS3, JavaScript vanilla
- **Design System**: `ierahkwa.css` (shared)
- **API SDK**: `ierahkwa-api.js` — conexion con backend soberano
- **Seguridad**: `ierahkwa-security.js` — encriptacion post-quantum Kyber-768
- **AI**: `ierahkwa-agents.js` — 7 agentes autonomos anti-fraude
- **Quantum**: `ierahkwa-quantum.js` — criptografia resistente a quantum
- **Protocolos**: `ierahkwa-protocols.js` — comunicacion soberana P2P
- **Red**: `ierahkwa-interconnect.js` — interconexion entre plataformas
- **PWA**: Service Worker + manifest.json — funciona offline
- **Blockchain**: MameyChain — registro inmutable de votos

## Instalacion

```bash
# Clonar el repositorio
git clone https://github.com/rudvincci/ierahkwa-platform.git

# Navegar a la plataforma
cd 02-plataformas-html/voto-soberano

# Abrir en navegador (no requiere servidor)
open index.html
```

## NEXUS

Esta plataforma pertenece a **NEXUS Consejo (Gobierno Digital)** del ecosistema Ierahkwa.

- **Color**: #1565c0
- **Ambito**: Gobernanza, democracia, elecciones, legislacion soberana
- **Plataformas hermanas**: 27 plataformas en NEXUS Consejo

## Seguridad

- Encriptacion post-quantum (CRYSTALS-Kyber-768)
- 7 Agentes AI de vigilancia continua
- Balotaje anonimo con mezcla criptografica
- Zero dependencias externas
- Sin tracking ni cookies de terceros
- Datos almacenados localmente (IndexedDB)
- Verificacion de voto en MameyChain

## API

```javascript
// Autenticacion
IerahkwaAPI.auth.login(id, password)
IerahkwaAPI.auth.logout()
IerahkwaAPI.auth.isLoggedIn()

// Votacion
IerahkwaAPI.votes.active()        // Elecciones activas
IerahkwaAPI.votes.cast(id, choice) // Emitir voto
IerahkwaAPI.votes.create(data)    // Crear eleccion (admin)
IerahkwaAPI.votes.results(id)     // Resultados
IerahkwaAPI.votes.history()       // Historial personal
```

## Licencia

Propiedad de Ierahkwa Ne Kanienke — Nacion Digital Soberana.

## Contacto

- **Proyecto**: [Ierahkwa Platform](https://github.com/rudvincci/ierahkwa-platform)
- **NEXUS**: NEXUS Consejo (Gobierno Digital)
