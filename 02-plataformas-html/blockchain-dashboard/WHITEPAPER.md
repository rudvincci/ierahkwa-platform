# WHITEPAPER: Blockchain Dashboard — MameyNode Sovereign Monitoring

> Technical Document v1.0 — Ierahkwa Ne Kanienke Sovereign Digital Nation
> March 2026

---

## 1. Abstract

The Blockchain Dashboard serves as the primary observability layer for the Ierahkwa
sovereign blockchain infrastructure. It provides real-time monitoring of MameyNode v4.2,
comprehensive visualization of all 109 IGT (Ierahkwa Government Token) assets, and
supervision of the FutureWampum decentralized identity system. This document describes
the technical architecture, token economics, identity framework, and post-quantum
security mechanisms that underpin the dashboard and the sovereign blockchain ecosystem.

## 2. MameyNode Sovereign Blockchain

### 2.1 Network Parameters
| Parameter          | Value                          |
|--------------------|--------------------------------|
| Chain ID           | 777777                         |
| Node Version       | MameyNode v4.2                 |
| Consensus          | Proof-of-Sovereignty (PoSov)   |
| Block Time         | 2.4 seconds                    |
| TPS Capacity       | 12,000-15,000                  |
| Active Validators  | 574 (one per tribal nation)    |
| Gas Token          | WMP (Wampum)                   |
| Total WMP Supply   | 10,000,000,000,000             |

### 2.2 Proof-of-Sovereignty Consensus
Unlike Proof-of-Work or Proof-of-Stake, Proof-of-Sovereignty (PoSov) is a consensus
mechanism designed for nations. Each of the 574 tribal nations operates exactly one
validator node. Block production rights rotate through nations using a deterministic
schedule weighted by population and activity. This ensures equitable representation
while maintaining high throughput.

Validator selection per epoch:
1. Deterministic rotation based on nation identifier hash
2. Population weight factor (0.3) + activity factor (0.7)
3. Finality after 2/3 supermajority (383+ validators)
4. Slashing only for provable malicious behavior (not downtime)

### 2.3 Network Health Monitoring
The dashboard tracks eight primary health metrics:
- Block height (animated counter, real-time increment)
- Gas price (target: 0.00042 WMP per gas unit)
- Active validator count and geographic distribution
- Block time moving average (target: 2.4s)
- Peer connectivity (target: 1,000+ peers)
- Current epoch and epoch progression
- Network uptime percentage (target: 99.95%+)
- Transaction throughput (TPS) with 3-second sampling

## 3. IGT Token Economics

### 3.1 Token Classification
The 109 IGT tokens are organized into nine functional categories:

| Category           | Range     | Count | Purpose                            |
|--------------------|-----------|-------|------------------------------------|
| Mamey Framework    | 01-30     | 30    | Government ministries & offices    |
| Financial          | 31-50     | 20    | Security, registry, financial core |
| DeFi               | 51-65     | 15    | Exchanges, payments, lending       |
| Social & Services  | 66-80     | 15    | Commerce, health, education, tech  |
| Infrastructure     | 81-100    | 20    | Energy, telecom, metaverse, DAO    |
| Sovereign          | 101-103   | 3     | International settlement, treaties |
| Social Services    | 104-106   | 3     | Housing, welfare, labor            |
| Commerce & Trade   | 107-108   | 2     | Customs, export trade              |
| Transport Infra.   | 109       | 1     | Transportation infrastructure      |

### 3.2 Token Standard
All IGT tokens conform to the Ierahkwa Government Token standard:
- **Decimals**: 9
- **Supply model**: Fixed supply per token (set at genesis)
- **Governance**: Multi-sig controlled by respective ministry/department
- **Interoperability**: Bridge-compatible via IGT-BRIDGE (token #48)

### 3.3 Economic Model
The WMP (Wampum) serves as the native gas token with a fixed supply of 10T.
Token distribution follows a sovereign model:
- 40% — National Treasury (IGT-NT, token #08)
- 20% — Staking rewards pool (IGT-STAKE, token #44)
- 15% — Liquidity provision (IGT-LIQ, token #45)
- 10% — International reserve (IGT-RESERVE, token #49)
- 10% — Ecosystem development (IGT-REWARD, token #46)
- 5%  — Bridge liquidity (IGT-BRIDGE, token #48)

## 4. FutureWampum Identity System

### 4.1 FWID Architecture
FutureWampum ID (FWID) is a W3C DID-compliant decentralized identity system
designed for indigenous sovereignty. Each FWID is a self-sovereign identifier
controlled entirely by the holder.

Key components:
- **DID Document**: W3C DID Core v1.0 compliant
- **Verifiable Credentials**: W3C VC Data Model v2.0
- **ZK Proofs**: Selective disclosure via Groth16 circuits
- **Recovery**: Social recovery through tribal council multi-sig

### 4.2 Trust Score System
Every FWID holder receives a dynamic trust score (0-100) computed by:
- Transaction history integrity (weight: 0.25)
- Community participation (weight: 0.20)
- Identity verification level (weight: 0.20)
- Peer endorsements (weight: 0.15)
- Governance participation (weight: 0.10)
- Account age and consistency (weight: 0.10)

Distribution targets:
- 90-100 (Exemplary): 68% of holders
- 70-89 (Trusted): 22% of holders
- 50-69 (Developing): 7% of holders
- 0-49 (New/Flagged): 3% of holders

### 4.3 ZK Proof Verification
The dashboard tracks cumulative ZK proof verifications across the network.
Each verification proves a claim (age, nationality, qualification) without
revealing the underlying data. The system uses Groth16 for efficiency and
Plonk for flexibility in complex predicates.

## 5. Post-Quantum Security

### 5.1 Cryptographic Stack
| Layer              | Algorithm               | NIST Level | Purpose               |
|--------------------|-------------------------|------------|-----------------------|
| Encryption         | CRYSTALS-Kyber-768      | Level 3    | Key encapsulation     |
| Hash               | SHA3-512 + BLAKE3       | Level 5    | Data integrity        |
| Signature          | CRYSTALS-Dilithium-5    | Level 5    | Digital signatures    |
| Key Exchange       | X25519 + Kyber-768      | Hybrid     | Session key agreement |
| ZK System          | Groth16 + Plonk         | N/A        | Zero-knowledge proofs |

### 5.2 Quantum Resistance Strategy
The Ierahkwa blockchain implements a hybrid approach:
1. Classical algorithms (X25519, Ed25519) for backward compatibility
2. Post-quantum algorithms (Kyber, Dilithium) for future-proofing
3. Hash-based fallbacks (SPHINCS+) for emergency migration
4. Crypto-agility layer enabling algorithm rotation without hard fork

### 5.3 Security Monitoring
The dashboard displays real-time security status including:
- Active encryption scheme and its operational status
- Hash algorithm chain integrity verification
- Digital signature scheme health
- Key exchange protocol verification
- ZK proof system operational metrics
- Overall NIST security level compliance

## 6. Dashboard Technical Implementation

### 6.1 Frontend Architecture
- Pure HTML5 with semantic markup and ARIA accessibility
- Inline CSS with CSS custom properties for theming
- Vanilla JavaScript for animations and data simulation
- Zero external dependencies (only shared Ierahkwa design system)
- Responsive grid layout supporting mobile through desktop

### 6.2 Data Simulation
In the current implementation, data is simulated client-side:
- Block height increments every 2.4 seconds (matching block time)
- TPS fluctuates around baseline (12,847) every 3 seconds
- Transaction feed generates new entries every 4 seconds
- Distribution charts animate on initial load

### 6.3 AI Agent Integration
Seven sovereign AI agents (ierahkwa-agents.js) provide:
- Guardian: Anti-fraud surveillance on transaction patterns
- Pattern: Behavioral learning per dashboard user
- Anomaly: Detection of suspicious blockchain activity
- Trust: Dynamic trust scoring integration
- Shield: Transaction and storage protection
- Forensic: Audit logging of all dashboard interactions
- Evolution: Self-improving detection rules

## 7. Future Roadmap

1. **WebSocket Integration**: Real-time data from MameyNode RPC endpoints
2. **Token Detail Views**: Click-through to individual token analytics
3. **Validator Map**: Geographic visualization of 574 validators
4. **Alert System**: Configurable alerts for network anomalies
5. **Multi-chain Bridge Monitor**: Cross-chain bridge status tracking
6. **FWID Credential Verifier**: Inline credential verification tool
7. **Governance Dashboard**: DAO proposal tracking and voting stats

## 8. Conclusion

The Blockchain Dashboard consolidates all critical metrics of the Ierahkwa sovereign
blockchain ecosystem into a single, accessible interface. By monitoring MameyNode
health, visualizing the complete 109-token economy, tracking FutureWampum identity
adoption, and verifying post-quantum security posture, it serves as the operational
control center for the world's first sovereign indigenous blockchain infrastructure.

---

*Ierahkwa Ne Kanienke — Sovereign Digital Nation*
*MameyNode v4.2 | Chain ID 777777 | 109 IGT Tokens | NIST Level 5 Security*
*March 2026*
