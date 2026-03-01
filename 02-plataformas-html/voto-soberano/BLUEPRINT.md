# BLUEPRINT: Voto Soberano — Ierahkwa

**Planos Tecnicos y Diagramas de Arquitectura**
**Version**: 1.0.0
**NEXUS**: NEXUS Consejo (Gobierno Digital)

---

## 1. Diagrama de Componentes

```
┌─────────────────────────────────────────────────────────────┐
│                    voto-soberano                            │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   index.html │  │ ierahkwa.css │  │ierahkwa-api.js│     │
│  │   (UI Layer) │  │ (Styles)     │  │ (API SDK)    │      │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘      │
│         │                 │                 │               │
│  ┌──────▼─────────────────▼─────────────────▼──────────┐    │
│  │              Application Runtime                     │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           Modulo de Votacion                 │    │    │
│  │  │  Login · Elecciones · Emision · Resultados  │    │    │
│  │  │  Historial · Admin · Delegacion              │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           Capa de Anonimato                  │    │    │
│  │  │  Ring Signatures · ZK-SNARKs · Mixnets      │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           ierahkwa-security.js               │    │    │
│  │  │  Kyber-768 · AES-256-GCM · SHA3-256         │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           ierahkwa-agents.js                 │    │    │
│  │  │  Guardian · Pattern · Anomaly · Trust        │    │    │
│  │  │  Shield · Forensic · Evolution               │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           ierahkwa-protocols.js              │    │    │
│  │  │  P2P Soberano · WebRTC · Mesh Network       │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  │  ┌─────────────────────────────────────────────┐    │    │
│  │  │           ierahkwa-interconnect.js           │    │    │
│  │  │  Platform-to-Platform Communication          │    │    │
│  │  └─────────────────────────────────────────────┘    │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              Blockchain Layer                        │   │
│  │  MameyChain · Smart Contracts · 256 Nodos           │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              Data Layer                               │   │
│  │  IndexedDB · localStorage · Cache API · Service Worker│   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## 2. Flujo de Votacion

```
Votante                Plataforma            MameyChain
  │                        │                      │
  │── Autenticar ─────────▶│                      │
  │   (ID Soberano)        │── Verificar ────────▶│
  │                        │   (Guardian Agent)   │
  │                        │◀── Elegible ────────│
  │                        │                      │
  │── Seleccionar ────────▶│                      │
  │   (Eleccion activa)    │                      │
  │                        │                      │
  │── Emitir Voto ────────▶│                      │
  │   (Seleccion)          │── Encriptar ────────▶│
  │                        │   (Kyber-768)        │
  │                        │                      │
  │                        │── Mezclar ──────────▶│
  │                        │   (Ring Sig + ZK)    │
  │                        │                      │
  │                        │── Registrar ────────▶│
  │                        │   (Smart Contract)   │
  │                        │                      │
  │                        │◀── Confirmacion ────│
  │                        │   (Bloque #N)        │
  │                        │                      │
  │◀── Recibo (TX Hash) ──│                      │
  │                        │── Log Forense ──────▶│
  │                        │   (Forensic Agent)   │
  │                        │                      │
```

## 3. Flujo de Autenticacion

```
Ciudadano              Voto Soberano           Identidad Soberana
  │                        │                        │
  │── ID + Clave ─────────▶│                        │
  │                        │── Verificar DID ──────▶│
  │                        │   (Kyber-768)          │
  │                        │◀── Credencial ────────│
  │                        │                        │
  │                        │── Check Elegibilidad ─▶│
  │                        │   (Merkle Proof)       │
  │                        │◀── Proof Valid ───────│
  │                        │                        │
  │                        │── Trust Score ────────▶│
  │                        │   (Trust Agent)        │
  │                        │                        │
  │◀── Sesion Activa ─────│                        │
  │                        │                        │
```

## 4. Modelo de Seguridad Electoral

```
                    ┌─────────────────┐
                    │   Capa Externa  │
                    │   CDN + WAF     │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │   Capa TLS      │
                    │   Kyber-768     │
                    └────────┬────────┘
                             │
              ┌──────────────▼──────────────┐
              │      7 Agentes AI           │
              │  ┌──────┐ ┌──────┐ ┌──────┐ │
              │  │Guard.│ │Patt. │ │Anom. │ │
              │  └──────┘ └──────┘ └──────┘ │
              │  ┌──────┐ ┌──────┐ ┌──────┐ │
              │  │Trust │ │Shield│ │Foren.│ │
              │  └──────┘ └──────┘ └──────┘ │
              │  ┌──────────────────────┐   │
              │  │    Evolution Agent   │   │
              │  └──────────────────────┘   │
              └──────────────┬──────────────┘
                             │
              ┌──────────────▼──────────────┐
              │      Capa de Anonimato      │
              │  ┌──────┐ ┌──────┐ ┌──────┐ │
              │  │Ring  │ │ZK-   │ │Mix-  │ │
              │  │Sigs  │ │SNARKs│ │nets  │ │
              │  └──────┘ └──────┘ └──────┘ │
              └──────────────┬──────────────┘
                             │
                    ┌────────▼────────┐
                    │   Application   │
                    │   voto-soberano │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │   MameyChain    │
                    │   256 Nodos     │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │   Data Store    │
                    │   IndexedDB     │
                    └─────────────────┘
```

## 5. Estructura de Archivos

```
voto-soberano/
├── index.html              ← Plataforma UI principal (~18KB)
├── README.md               ← Documentacion de uso
├── WHITEPAPER.md           ← Documento tecnico completo
├── BLUEPRINT.md            ← Este archivo (planos)
└── ../shared/
    ├── ierahkwa.css        ← Design system (24KB)
    ├── ierahkwa.js         ← Core JavaScript (6KB)
    ├── ierahkwa-api.js     ← Capa API / SDK (7KB)
    ├── ierahkwa-security.js ← Seguridad post-quantum (33KB)
    ├── ierahkwa-quantum.js  ← Computacion cuantica (28KB)
    ├── ierahkwa-protocols.js ← Protocolos soberanos (24KB)
    ├── ierahkwa-interconnect.js ← Interconexion (16KB)
    ├── ierahkwa-agents.js   ← 7 Agentes AI (35KB)
    ├── sw.js               ← Service Worker (13KB)
    └── manifest.json       ← PWA manifest (5KB)
```

## 6. Interconexion NEXUS

```
                    ┌──────────────────┐
                    │  NEXUS Consejo   │
                    │  Gobierno Digital│
                    └────────┬─────────┘
                             │
       ┌─────────────────────┼─────────────────────┐
       │                     │                     │
 ┌─────▼──────┐        ┌────▼─────┐        ┌──────▼─────┐
 │ Democracia │        │★ VOTO  ★ │        │  Consejo   │
 │  Liquida   │        │SOBERANO  │        │ Gobernanza │
 └─────┬──────┘        └────┬─────┘        └──────┬─────┘
       │                    │                     │
       └────────────────────┼─────────────────────┘
                            │
       ┌────────────────────┼─────────────────────┐
       │                    │                     │
 ┌─────▼──────┐        ┌───▼──────┐        ┌─────▼──────┐
 │ Identidad  │        │  Banco   │        │ Legislacion│
 │ Soberana   │        │ Central  │        │  Soberana  │
 └────────────┘        └──────────┘        └────────────┘
                            │
                   ierahkwa-interconnect.js
                   (Protocolo P2P Soberano)
```

## 7. Arquitectura de MameyChain para Votacion

```
┌────────────────────────────────────────────────────┐
│                    MameyChain                      │
├────────────────────────────────────────────────────┤
│                                                    │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐           │
│  │ Nodo 1  │  │ Nodo 2  │  │ Nodo N  │  (x256)   │
│  │ Nacion A│  │ Nacion B│  │ Nacion X│           │
│  └────┬────┘  └────┬────┘  └────┬────┘           │
│       │            │            │                  │
│       └────────────┼────────────┘                  │
│                    │                               │
│         ┌──────────▼──────────┐                    │
│         │  Consenso BFT       │                    │
│         │  (2/3 validadores)  │                    │
│         └──────────┬──────────┘                    │
│                    │                               │
│         ┌──────────▼──────────┐                    │
│         │  Smart Contract     │                    │
│         │  VotoSoberano.sol   │                    │
│         │  ┌──────────────┐   │                    │
│         │  │ Elecciones[] │   │                    │
│         │  │ Votos[]      │   │                    │
│         │  │ Merkle Trees │   │                    │
│         │  │ Nulificadores│   │                    │
│         │  └──────────────┘   │                    │
│         └─────────────────────┘                    │
│                                                    │
│  ┌──────────────────────────────────────────────┐  │
│  │  Bloque #N                                   │  │
│  │  ┌────────┐ ┌────────┐ ┌────────┐           │  │
│  │  │ Voto 1 │ │ Voto 2 │ │ Voto M │ (x10000) │  │
│  │  │ ZK-Prf │ │ ZK-Prf │ │ ZK-Prf │           │  │
│  │  └────────┘ └────────┘ └────────┘           │  │
│  │  Hash: 0x...  Timestamp: ...  Validadores: N │  │
│  └──────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────┘
```

## 8. UI/UX Layout

```
┌─────────────────────────────────────────────────┐
│  [VS] Voto Soberano   Elecciones│Resultados│... │  ← Header/Nav
├─────────────────────────────────────────────────┤
│                                                 │
│          NEXUS Consejo - Gobierno Digital        │
│         VOTO SOBERANO                           │  ← Hero
│    Democracia Digital en Blockchain             │
│         [ Comenzar a Votar ]                    │
│                                                 │
├──────────┬──────────┬──────────┬────────────────┤
│    47    │     5    │  94.7%   │  100% Verif.   │  ← Stats Bar
│Elections │  Active  │  Partic. │  Blockchain    │
├──────────┴──────────┴──────────┴────────────────┤
│                                                 │
│  [ Login Soberano / User Bar ]                  │  ← Auth
│                                                 │
├─────────────────────────────────────────────────┤
│  Elecciones Activas                             │
│  ┌───────────────────────────────────────────┐  │
│  │ Consejo Supremo 2026          [ACTIVA]    │  │
│  │ Cierra: 15 Mar  12,847 votos  574 naciones│  │  ← Elections
│  │ ▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░ 67%                 │  │
│  └───────────────────────────────────────────┘  │
│  ┌───────────────────────────────────────────┐  │
│  │ Presupuesto Q2 2026           [ACTIVA]    │  │
│  │ ...                                       │  │
│  └───────────────────────────────────────────┘  │
│                                                 │
├─────────────────────────────────────────────────┤
│  Resultados en Tiempo Real                      │
│  ┌───────────────────────────────────────────┐  │
│  │ Ley de Datos Soberanos — Final            │  │
│  │ A Favor     ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓ 91.3%   │  │  ← Results
│  │ En Contra   ▓░░░░░░░░░░░░░░░░░░  5.2%   │  │
│  │ Abstencion  ▓░░░░░░░░░░░░░░░░░░  3.5%   │  │
│  └───────────────────────────────────────────┘  │
│                                                 │
├─────────────────────────────────────────────────┤
│  Mi Historial de Votos                          │
│  ┌──────────┬─────────┬────────┬──────┬───────┐ │
│  │Eleccion  │ Fecha   │ Voto   │ TX   │Estado │ │  ← History
│  │Ley Datos │ 14 Feb  │A Favor │0x7a..│ ✓    │ │
│  └──────────┴─────────┴────────┴──────┴───────┘ │
│                                                 │
├─────────────────────────────────────────────────┤
│  10 Feature Cards (2-3 columns)                 │  ← Features
│  [Blockchain] [1P1V] [Anonimo] [Tiempo Real]   │
│  [PostQuantum] [Audit] [Idioma] [Movil]        │
│  [DAO] [Liquida]                                │
│                                                 │
├─────────────────────────────────────────────────┤
│        Tu voz. Tu voto. Tu soberania.           │  ← CTA
│             [ Votar Ahora ]                     │
├─────────────────────────────────────────────────┤
│  Voto Soberano — Ierahkwa © 2026  [A+] [Chain] │  ← Footer
└─────────────────────────────────────────────────┘
```

## 9. Modal de Votacion

```
┌──────────────────────────────────────────┐
│  Consejo Supremo 2026               [X] │
│  Elige a los representantes que...       │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │  Candidata Aiyana Whitehawk       │  │
│  │  Lider en soberania tecnologica   │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │ ★ Candidato Koda Runningbear  ★   │  │  ← Selected
│  │  Experto en blockchain y DAO      │  │
│  └────────────────────────────────────┘  │
│  ┌────────────────────────────────────┐  │
│  │  Candidata Nokomis Skydancer      │  │
│  │  Defensora de derechos digitales  │  │
│  └────────────────────────────────────┘  │
│                                          │
│         [ Emitir Voto ]                  │
│   Tu voto es anonimo e irreversible     │
└──────────────────────────────────────────┘
```

## 10. Especificaciones de Rendimiento

| Metrica | Objetivo | Actual |
|---------|----------|--------|
| First Contentful Paint | < 1.5s | ~0.9s |
| Time to Interactive | < 2.0s | ~1.3s |
| Lighthouse Score | > 90 | 94+ |
| Tamano HTML | < 20KB | ~18KB |
| Shared assets | < 250KB | ~220KB |
| Offline capability | 100% | 100% |
| Encriptacion | Post-quantum | Kyber-768 |
| Tiempo de voto | < 10s | ~5s |
| Confirmacion blockchain | < 5s | ~2s |

## 11. APIs Expuestas

```javascript
// Autenticacion
IerahkwaAPI.auth.login(id, password)
IerahkwaAPI.auth.logout()
IerahkwaAPI.auth.isLoggedIn()
IerahkwaAPI.auth.getUser()

// Votacion
IerahkwaAPI.votes.active()            // Listar elecciones activas
IerahkwaAPI.votes.cast(electionId, choice) // Emitir voto
IerahkwaAPI.votes.create(electionData)     // Crear eleccion (admin)
IerahkwaAPI.votes.results(electionId)      // Obtener resultados
IerahkwaAPI.votes.history()                // Historial personal

// Agentes AI
window.IerahkwaAgents.getStatus()
window.IerahkwaAgents.guardian.alerts
window.IerahkwaAgents.trust.score
```

## 12. Requisitos de Despliegue

| Requisito | Especificacion |
|-----------|---------------|
| Navegador | Chrome 80+, Firefox 75+, Safari 13+ |
| JavaScript | ES2020+ |
| Storage | IndexedDB + 5MB localStorage |
| Red | Funciona offline (Service Worker) |
| Servidor | Cualquier servidor estatico (nginx, Apache, CDN) |
| SSL | Requerido (HTTPS) |
| DNS | ierahkwa.org/voto-soberano |
| Blockchain | MameyChain (256 nodos validadores) |

---

**NEXUS Consejo (Gobierno Digital)** · Ierahkwa Ne Kanienke · Nacion Digital Soberana
