# Blockchain Dashboard — MameyNode Soberano

> Dashboard en tiempo real del blockchain soberano MameyNode, 109 tokens IGT
> y sistema de identidad FutureWampum para Ierahkwa Ne Kanienke.

## Descripcion

El Blockchain Dashboard es la plataforma de monitoreo centralizada para toda la
infraestructura blockchain de la nacion soberana digital Ierahkwa Ne Kanienke.
Proporciona visibilidad completa sobre el nodo MameyNode v4.2, los 109 tokens IGT
del ecosistema Ierahkwa Government Token, y el sistema de identidad descentralizada
FutureWampum (FWID).

## Proposito

- Monitorear el estado operativo del nodo MameyNode v4.2 (Chain ID 777777)
- Visualizar los 109 tokens IGT organizados por categoria en un grid interactivo
- Rastrear metricas de la red: TPS, altura de bloque, gas price, validadores
- Supervisar el sistema de identidad FutureWampum y su distribucion de trust score
- Mostrar el feed de transacciones en tiempo real con datos simulados
- Verificar el estado de la seguridad post-cuantica (CRYSTALS-Kyber-768)

## Caracteristicas Principales

### Grid de 109 Tokens IGT
Todos los tokens del ecosistema organizados en 9 categorias:
- **Mamey Framework** (01-30): Ministerios y oficinas gubernamentales
- **Financial** (31-50): Policia, fuerzas armadas, moneda principal, stablecoin
- **DeFi** (51-65): Exchange, trading, casino, SWIFT, clearhouse, loans
- **Social & Services** (66-80): Marketplace, health, education, AI, gaming
- **Infrastructure** (81-100): Energy, telecom, metaverse, NFT, DAO, oracle
- **Sovereign** (101-103): IISB, treaty, embassy
- **Social Services** (104-106): Housing, welfare, labor
- **Commerce & Trade** (107-108): Customs, export
- **Infrastructure New** (109): Transport

### MameyNode Status
Panel con metricas del nodo soberano primario:
- Altura de bloque con contador animado en tiempo real
- Gas price, validadores activos (574), consenso Proof-of-Sovereignty
- Tiempo de bloque (2.4s), peers conectados, epoch actual, salud de red

### FutureWampum Identity (FWID)
Sistema de identidad descentralizada con:
- Total de FWIDs emitidos y registros DID
- Verificaciones ZK proof acumuladas
- Distribucion de trust score con barras CSS (90-100, 70-89, 50-69, 0-49)

### Transaction Feed
Feed simulado de transacciones recientes con:
- Hash de transaccion, direcciones from/to, valor, tipo de token
- Nuevas transacciones generadas cada 4 segundos via JavaScript

### Seguridad Post-Cuantica
- CRYSTALS-Kyber-768 (encriptacion)
- SHA3-512 + BLAKE3 (hash)
- CRYSTALS-Dilithium-5 (firmas digitales)
- X25519 + Kyber-768 (key exchange hibrido)
- Groth16 + Plonk (ZK proofs)
- NIST Level 5 (maximo nivel)

## Arquitectura Tecnica

```
blockchain-dashboard/
├── index.html          ← UI completa (~65KB, self-contained)
├── README.md           ← Este archivo
├── WHITEPAPER.md       ← Documento tecnico completo
└── BLUEPRINT.md        ← Planos y diagramas tecnicos
```

### Dependencias
- `../shared/ierahkwa.css` — Sistema de diseno Ierahkwa v3.5.0
- `../shared/ierahkwa-agents.js` — 7 Agentes AI Soberanos

### Stack
- HTML5 semantico con ARIA labels y roles
- CSS inline con variables custom (tema oscuro, acento dorado #ffd600)
- JavaScript vanilla para animaciones y simulacion de datos
- Zero dependencias externas (solo shared CSS y AI agents)

## Accesibilidad (GAAD)

- Skip navigation link
- Roles ARIA: `banner`, `main`, `contentinfo`, `list`, `listitem`, `log`, `img`
- `aria-label` en todas las secciones
- `aria-live="polite"` en contadores animados
- `prefers-reduced-motion` respetado
- Todos los token cards son navegables con teclado (tabindex)
- Contraste adecuado en texto y elementos interactivos

## Uso

Abrir `index.html` en cualquier navegador moderno. No requiere servidor.

```bash
open blockchain-dashboard/index.html
```

## Version

- **Plataforma**: v1.0.0
- **MameyNode**: v4.2
- **Chain ID**: 777777
- **Consenso**: Proof-of-Sovereignty
- **Tokens**: 109 IGT

---

*Ierahkwa Ne Kanienke — Sovereign Digital Nation — 2026*
