# Detalles de la plataforma blockchain — IERAHKWA

**Sovereign Government of Ierahkwa Ne Kanienke · Ierahkwa Sovereign Blockchain (ISB)**

**Todos los gobiernos con sus detalles blockchain:** [GOBIERNOS-TODOS-DETALLES-BLOCKCHAIN.md](./GOBIERNOS-TODOS-DETALLES-BLOCKCHAIN.md)

---

## 1. Nodo principal (Mamey Node)

| Dato | Valor |
|------|--------|
| **Nombre** | Ierahkwa Futurehead Mamey Node |
| **Tipo** | Primary Sovereign Node |
| **Símbolo red** | ISB |
| **Node ID** | IFMN-001 |
| **Puerto RPC** | 8545 |
| **Puerto WebSocket** | 8546 |
| **Puerto GraphQL** | 8547 |
| **P2P** | 30303 |
| **Chain ID** | 77777 (o 777777 en algunos endpoints) |
| **Consenso** | Sovereign Proof of Authority (SPoA) |
| **Block time** | 500 ms (config) / ~3 s (docs) |
| **Gas price** | 0 (gratis) |
| **Finality** | Instantánea |

**Ubicación en código:**
- Servidor: `RuddieSolution/node/server.js` (Express + RPC en 8545)
- Config nodo: `RuddieSolution/node/ierahkwa-futurehead-mamey-node.json`
- Config red: `RuddieSolution/node/config.toml`
- Genesis: `RuddieSolution/node/genesis.json`
- Estado en disco: `RuddieSolution/node/data/blockchain-state.json`, `node/data/sovereign-chain/`

---

## 2. Arquitectura del nodo (capas)

```
┌─────────────────────────────────────────────────────────────┐
│            IERAHKWA FUTUREHEAD MAMEY NODE                    │
├─────────────────────────────────────────────────────────────┤
│   RPC LAYER (8545) — eth_* + igt_*                           │
│         ↓                                                     │
│   Block Producer | Account Manager | Token Manager | API     │
│         ↓                                                     │
│   State Database (blocks, transactions, accounts)            │
└─────────────────────────────────────────────────────────────┘
```

- **RPC:** JSON-RPC compatible Ethereum (`eth_*`) + métodos propios IGT (`igt_*`).
- **REST:** `/api/v1/stats`, `/api/v1/tokens`, `/api/v1/blocks`, `/api/v1/transactions`.
- **Plataforma:** `/platform`, `/tokens` (dashboard y registro de tokens).

---

## 3. Tokens IGT (Ierahkwa Government Token)

- **Estándar:** IGT-20  
- **Decimales:** 9  
- **Supply por token:** 10 trillones (quemable)  
- **Total registrados:** 101 tokens en el nodo (103 en docs; genesis tiene alloc para varios)

**Bloques de tokens en `ierahkwa-futurehead-mamey-node.json`:**

| Rango | Uso |
|-------|-----|
| 01–40 | Gobierno (IGT-PM, IGT-MFA, IGT-MFT, IGT-BDET, IGT-NT, ministerios, corte, salud, educación, etc.) |
| 41–52 | Finanzas (IGT-MAIN, IGT-STABLE, IGT-GOV, IGT-STAKE, IGT-LIQ, IGT-BRIDGE, IGT-DEFI, etc.) |
| 53–101 | Servicios (Exchange, Trading, Casino, Social, Lotto, SWIFT, Clearhouse, Pay, Wallet, Insurance, Health, Edu, AI, VPN, NFT, DAO, IISB, etc.) |

Cuentas preasignadas en **genesis**: PM, MFA, MFT, BDET, MAIN, STABLE, IISB (direcciones tipo `0x1000...001`, `0x1000...041`, etc.).

---

## 4. Conexiones bancarias (desde el nodo)

| Banco | SWIFT | Estado en config |
|-------|--------|-------------------|
| Ierahkwa Futurehead BDET Bank | IERBDETXXX | CONNECTED |
| Ierahkwa International Settlement Bank (IISB) | IISBGLOB | CONNECTED |

---

## 5. Seguridad (config y docs)

- **Cifrado:** AES-256-GCM  
- **TLS:** 1.3  
- **Certificado:** Government Grade SSL  
- **Protección:** DDoS, firewall, intrusion detection, audit logging  
- **Validadores:** 21 (mínimo 15 para consenso SPoA)

---

## 6. Funcionalidades habilitadas

- Smart contracts  
- NFTs  
- DeFi  
- Cross-chain bridge  
- Oracle  
- Staking  
- Governance  
- Atomic swaps  

---

## 7. Servicios “blockchain” en la plataforma (puertos)

Además del nodo 8545, en `config/services-ports.json` están:

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| DeFi Soberano | 5140 | DeFiSoberano/DeFi.API |
| NFT Certificates | 5141 | NFTCertificates/NFT.API |
| Governance DAO | 5142 | GovernanceDAO/DAO.API |
| Multichain Bridge | 5143 | MultichainBridge/Bridge.API |

---

## 8. Integración con Mamey (Rust) y .NET

- **Mamey (Rust):** En el repo hay carpeta `Mamey/` con servicios .NET (Blog, Documents, Treasury SICB, Compliance, etc.) y SDKs (TypeScript, Go, Python). El nodo blockchain de *alto rendimiento* en Rust estaría en `Mamey/core/` (referencia en ESTADO-SOBERANIA: `Mamey/core/MameyNode/`). Para producción 100% blockchain: compilar y levantar ese nodo y conectar el Node.js a él (gateway en `node/routers/mamey-gateway-server.js`, config en `ierahkwa-futurehead-mamey-node.json`).
- **.NET:** `IerahkwaBanking.NET10` tiene `MameyNodeService` y `MameyNodeController` que hablan con el nodo (health, estado) para el sistema bancario.

---

## 9. Conectar MetaMask / cliente

```
Network Name: Ierahkwa Sovereign Blockchain
RPC URL:      http://localhost:8545/rpc
Chain ID:     777777
Symbol:       IGT
Explorer:     http://localhost:8545
```

---

## 10. Referencia rápida de archivos

| Archivo | Contenido |
|---------|-----------|
| `RuddieSolution/node/README.md` | Documentación del nodo |
| `RuddieSolution/node/ierahkwa-futurehead-mamey-node.json` | Config + lista de 101 IGT |
| `RuddieSolution/node/genesis.json` | Genesis block (chainId 77777, alloc) |
| `RuddieSolution/node/config.toml` | Red, RPC, consenso, tokens, bancos |
| `RuddieSolution/node/data/blockchain-state.json` | Estado actual (blockNumber, bloques recientes) |
| `RuddieSolution/node/data/sovereign-chain/node-state.json` | Estado alternativo (blockHeight, validators) |
| `RuddieSolution/config/services-ports.json` | blockchain_services (DeFi, NFT, DAO, Bridge) |
| `RuddieSolution/ESTADO-SOBERANIA-COMPLETO.md` | Mamey Rust + Node.js + flujo producción |

---

*Generado para la plataforma IERAHKWA · Sovereign Government of Ierahkwa Ne Kanienke*
