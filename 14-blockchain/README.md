# IERAHKWA Sovereign Blockchain

## Mamey Chain - Digital Sovereignty Infrastructure

> **Chain ID:** 574 | **Consensus:** Proof-of-Sovereignty | **Block Time:** 3s

---

## Overview

The Ierahkwa Sovereign Blockchain ("Mamey Chain") is the foundational Layer-1 infrastructure powering digital sovereignty for 72 million indigenous people across 19 nations and 574 tribal nations. Built on a modified EVM-compatible chain with post-quantum cryptographic primitives, the Mamey Chain provides the backbone for governance, identity, finance, and all sovereign digital services.

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                    MAMEY CHAIN ARCHITECTURE                             │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                    QUANTUM SECURITY LAYER                       │   │
│  │    Post-Quantum Signatures · QKD · Lattice-Based Encryption     │   │
│  │                  128 Qubits · 99.9% Fidelity                    │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                               │                                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │ WAMPUM   │  │ FutureID │  │ DAO      │  │ Treasury │              │
│  │ ERC-20   │  │ ERC-721  │  │ Governor │  │ Timelock │              │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘              │
│                               │                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │              209 IGT GOVERNANCE TOKENS                          │   │
│  │  Ministry tokens · Financial tokens · Utility tokens            │   │
│  │  Each token represents a sovereign government function          │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                               │                                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐              │
│  │ Staking  │  │ Quantum  │  │ Oracle   │  │ Token    │              │
│  │ Pool     │  │ Bridge   │  │ Network  │  │ Factory  │              │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘              │
│                               │                                         │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │              EVM-COMPATIBLE EXECUTION LAYER                     │   │
│  │     Solidity 0.8+ · 30M Gas Limit · 574 Validators Max         │   │
│  └─────────────────────────────────────────────────────────────────┘   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

## Core Smart Contracts

| Contract | Type | Address (Testnet) | Description |
|----------|------|-------------------|-------------|
| **WAMPUM** | ERC-20 | `0x574a...0001` | Governance token for DAO voting |
| **ISB** | ERC-20 | `0x574a...0002` | Native sovereign currency |
| **BDET** | ERC-20 | `0x574a...0003` | Digital economy utility token |
| **FutureWampumId** | ERC-721 | `0x574a...0010` | Sovereign digital identity NFT |
| **GovernanceDAO** | Governor | `0x574a...0020` | On-chain governance with proposals & voting |
| **Treasury** | Timelock | `0x574a...0030` | Multi-sig treasury with 48hr timelock |
| **TokenFactory** | Factory | `0x574a...0040` | Deploys and manages all 209 IGT tokens |
| **StakingPool** | Staking | `0x574a...0050` | 15% APY staking for validators |
| **QuantumBridge** | Bridge | `0x574a...0060` | Cross-chain bridge (ETH, Polygon, BSC, etc.) |
| **OracleAggregator** | Oracle | `0x574a...0070` | Decentralized price feeds & data |

## 209 IGT Governance Tokens

The Ierahkwa Governance Token (IGT) system comprises **209 unique ERC-20 tokens**, each representing a sovereign government function, ministry, or service. Tokens are organized into the following categories:

### Token Categories

| # | Range | Category | Count | Examples |
|---|-------|----------|-------|----------|
| 1 | 01-30 | **Government Ministries** | 30 | IGT-PM (Prime Minister), IGT-MFA (Foreign Affairs), IGT-MFT (Finance & Treasury), IGT-MJ (Justice), IGT-MI (Interior), IGT-MD (Defense) |
| 2 | 31-50 | **Financial Infrastructure** | 20 | IGT-BDET (Blockchain Economy), IGT-NT (National Treasury), IGT-AG (Agriculture), IGT-SC (Supreme Court) |
| 3 | 51-80 | **Social Services** | 30 | IGT-MH (Health), IGT-ME (Education), IGT-HOUSING, IGT-WELFARE, IGT-LABOR |
| 4 | 81-100 | **Technology & Innovation** | 20 | IGT-SOVEREIGN, IGT-IISB, IGT-DOCFLOW, IGT-ESIGN, IGT-AI |
| 5 | 101-130 | **Commerce & Trade** | 30 | IGT-MARKET, IGT-EXCHANGE, IGT-CUSTOMS, IGT-EXPORT |
| 6 | 131-160 | **Infrastructure** | 30 | IGT-TELECOM, IGT-ENERGY, IGT-TRANSPORT, IGT-WATER |
| 7 | 161-190 | **Culture & Education** | 30 | IGT-CULTURE, IGT-LANGUAGE, IGT-MUSEUM, IGT-SPORTS |
| 8 | 191-209 | **Special Purpose** | 19 | IGT-NFT, IGT-DAO, IGT-ORACLE, IGT-LAUNCHPAD, IGT-METAVERSE |

### Token Specifications

- **Standard:** ERC-20 (OpenZeppelin v5.0)
- **Initial Supply:** Varies per token (governance-controlled minting)
- **Decimal Precision:** 18
- **Governance Weight:** Each token provides voting power in its respective domain
- **Staking Enabled:** All tokens can be staked in the Sovereign Staking Pool

### Token Directory Structure

```
tokens/
├── 01-IGT-PM/           # Prime Minister governance token
│   ├── 01-IGT-PM.json   # Token metadata & configuration
│   └── contracts/        # Solidity source
├── 02-IGT-MFA/          # Ministry of Foreign Affairs
├── ...
├── 99-IGT-ORACLE/       # Oracle infrastructure token
├── 100-IGT-SOVEREIGN/   # Sovereign governance master token
├── ...
└── 209 total token directories
```

## FutureWampum Identity System (FWID)

The FutureWampum Identity system is a sovereign, self-custodial digital identity platform built on ERC-721 NFTs with verifiable credentials.

### Architecture

```
future-wampum/
└── FutureWampumId/
    ├── Mamey.FWID.Identities/     # Core identity management
    │   ├── Identity CRUD operations
    │   ├── KYC verification levels (1-3)
    │   ├── Biometric binding
    │   └── Multi-chain attestation
    ├── Mamey.FWID.Notifications/   # Identity event notifications
    │   ├── Verification alerts
    │   ├── Document expiration
    │   └── Security warnings
    ├── Mamey.FWID.Operations/      # Identity operations
    │   ├── Recovery flows
    │   ├── Delegation management
    │   └── Cross-chain identity sync
    └── Mamey.FWID.Sagas/           # Long-running identity workflows
        ├── Onboarding sagas
        ├── KYC verification sagas
        └── Recovery sagas
```

### FWID Features

- **Self-Sovereign:** Citizens own their identity - no central authority can revoke it
- **Quantum-Secured:** Post-quantum signatures protect identity claims
- **Multi-Level KYC:** Three verification levels (Basic, Standard, Enhanced)
- **Biometric Binding:** Optional biometric attestation for enhanced security
- **Cross-Chain:** Identity portable across Ethereum, Polygon, BSC, and other chains
- **Verifiable Credentials:** W3C-compliant verifiable credentials for education, health, government
- **Recovery:** Social recovery with guardian system (5-of-9 multi-sig)
- **Privacy:** Zero-knowledge proofs for selective disclosure

### Identity Data Model

```solidity
struct FutureWampumIdentity {
    uint256 tokenId;           // Unique NFT ID
    address owner;             // Wallet address
    bytes32 citizenIdHash;     // Hashed sovereign citizen ID
    uint8 kycLevel;            // 1=Basic, 2=Standard, 3=Enhanced
    uint256 issuedAt;          // Timestamp of issuance
    uint256 expiresAt;         // Expiration timestamp
    bytes32 biometricHash;     // Optional biometric attestation
    string[] credentials;      // List of verifiable credential URIs
    bool isActive;             // Active status
}
```

## Quantum Computing Infrastructure

The sovereign quantum computing system provides post-quantum security and computational capabilities.

### Specifications

| Parameter | Value |
|-----------|-------|
| Qubits | 128 |
| Gate Fidelity | 99.9% |
| Coherence Time | 10 microseconds |
| Gate Speed | 1M gates/second |
| Error Rate | < 0.1% |
| Operating Temperature | 15 mK |

### Quantum Applications

1. **Post-Quantum Cryptography**
   - Lattice-based encryption (CRYSTALS-Kyber)
   - Hash-based signatures (SPHINCS+)
   - Code-based cryptography (Classic McEliece)
   - Quantum Key Distribution (QKD)

2. **Financial Optimization**
   - Portfolio optimization via QAOA
   - Risk analysis with quantum Monte Carlo
   - Derivative pricing
   - Market simulation

3. **Blockchain Security**
   - Quantum-safe transaction signing
   - Quantum random number generation
   - Quantum-proof key exchange for bridge
   - Post-quantum consensus validation

4. **Machine Learning**
   - Quantum neural networks for anomaly detection
   - Pattern recognition in governance data
   - Predictive analytics for resource allocation

### Quantum API

```
Base URL: https://quantum.mamey.ierahkwa.nation/api

POST /job/submit         - Submit quantum computation job
GET  /job/{id}           - Get job status
GET  /job/{id}/result    - Get job result
GET  /system/status      - System health status
GET  /system/queue       - Job queue information
POST /crypto/keygen      - Generate quantum-safe key pair
POST /crypto/sign        - Quantum-safe signature
POST /random             - Quantum random number generation
```

## Network Configuration

### Testnet (Mamey Testnet)

| Parameter | Value |
|-----------|-------|
| Chain ID | 574 |
| RPC URL | `https://testnet.mamey.ierahkwa.nation` |
| WebSocket | `wss://testnet-ws.mamey.ierahkwa.nation` |
| Explorer | `https://explorer.mamey.ierahkwa.nation` |
| Faucet | `https://faucet.mamey.ierahkwa.nation` |
| Block Time | 3 seconds |
| Gas Limit | 30,000,000 |
| Native Currency | ISB (18 decimals) |

### MetaMask Configuration

```json
{
  "chainId": "0x23E",
  "chainName": "Mamey Testnet",
  "rpcUrls": ["https://testnet.mamey.ierahkwa.nation"],
  "blockExplorerUrls": ["https://explorer.mamey.ierahkwa.nation"],
  "nativeCurrency": { "name": "ISB", "symbol": "ISB", "decimals": 18 }
}
```

### Validator Requirements

- Minimum Stake: 100,000 WMP
- Maximum Validators: 574 (one per tribal nation)
- Slashing Rate: 1% for downtime
- Reward Rate: 8% APY
- Unbonding Period: 7 days

## Deployment

### Quick Start

```bash
# Clone and install
cd 14-blockchain
npm install

# Configure environment
cp .env.example .env
# Edit .env with your deployer private key

# Deploy to testnet
./deploy-testnet.sh

# Deploy contracts only
./deploy-testnet.sh --contracts-only

# Deploy 209 IGT tokens only
./deploy-testnet.sh --tokens

# Verify contracts on explorer
./deploy-testnet.sh --verify
```

### Project Structure

```
14-blockchain/
├── README.md                 # This documentation
├── testnet-config.json       # Testnet network & contract configuration
├── deploy-testnet.sh         # Deployment script
├── package.json              # Node.js dependencies
├── future-wampum/            # FutureWampum Identity System
│   └── FutureWampumId/
│       ├── Mamey.FWID.Identities/
│       ├── Mamey.FWID.Notifications/
│       ├── Mamey.FWID.Operations/
│       └── Mamey.FWID.Sagas/
├── quantum/                  # Quantum computing infrastructure
│   ├── README.md
│   └── index.html
└── tokens/                   # 209 IGT governance tokens
    ├── 01-IGT-PM/
    ├── 02-IGT-MFA/
    ├── ...
    ├── 209 token directories
    ├── generate-all-tokens.js
    └── index.html
```

## Security Model

### Multi-Layer Defense

1. **Layer 1 - Quantum Cryptography:** Post-quantum signatures and QKD
2. **Layer 2 - Smart Contract Audits:** OpenZeppelin-based contracts with formal verification
3. **Layer 3 - Validator Security:** 574 geographically distributed validators
4. **Layer 4 - Bridge Security:** Quantum-secured cross-chain transfers with threshold signatures
5. **Layer 5 - Governance:** DAO-controlled upgrades with 48-hour timelock

### Audit Status

| Component | Auditor | Status |
|-----------|---------|--------|
| Core Contracts | Internal | Completed |
| Token Factory | Internal | Completed |
| FutureWampum ID | Internal | Completed |
| Quantum Bridge | Internal | In Progress |
| Oracle Network | Internal | Planned |

## Integration with NEXUS Ecosystem

The blockchain integrates with all 15 NEXUS mega-portals of the Ierahkwa sovereign platform:

- **NEXUS Orbital** - Telecom token payments and satellite data anchoring
- **NEXUS Escudo** - Defense AI agent verification on-chain
- **NEXUS Cerebro** - AI/Quantum compute job settlements
- **NEXUS Tesoro** - Full DeFi stack: DEX, lending, insurance
- **NEXUS Voces** - Decentralized content hashing and rewards
- **NEXUS Consejo** - On-chain governance proposals and voting
- **NEXUS Tierra** - Environmental data oracles and carbon credits
- **NEXUS Forja** - Developer bounties and grant disbursement
- **NEXUS Urbe** - Smart city IoT payments
- **NEXUS Raices** - Cultural NFTs and heritage preservation
- **NEXUS Salud** - Health credential verification
- **NEXUS Academia** - Education certificate issuance
- **NEXUS Escolar** - Student identity and progress tracking
- **NEXUS Entretenimiento** - Gaming tokens and event ticketing
- **NEXUS Amparo** - Social protection fund distribution

## Roadmap

| Phase | Timeline | Milestone |
|-------|----------|-----------|
| Phase 1 | Q1 2026 | Testnet launch with core contracts |
| Phase 2 | Q2 2026 | 209 IGT tokens deployed, FutureWampum ID live |
| Phase 3 | Q3 2026 | Quantum bridge operational, mainnet launch |
| Phase 4 | Q4 2026 | Full NEXUS integration, 574 validators |
| Phase 5 | 2027 | Cross-sovereign interoperability |

---

**Status:** Active Development
**License:** Sovereign Government License
**Contact:** blockchain@ierahkwa.nation

(c) 2026 Sovereign Government of Ierahkwa Ne Kanienke. All rights reserved.
