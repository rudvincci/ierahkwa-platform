# Mamey Technologies Ecosystem - Glossary of Terms

**Version**: 1.0  
**Date**: 2024-12-21  
**Organization**: Mamey Technologies (mamey.io)  
**Audience**: All Stakeholders  
**Purpose**: Terminology reference

---

## A

### AML/CFT
**Anti-Money Laundering / Counter-Terrorism Financing**. Automated compliance screening for financial transactions to detect and prevent money laundering and terrorism financing.

### API (Application Programming Interface)
A set of protocols and tools for building software applications. Mamey provides REST, gRPC, and WebSocket APIs.

### ARR (Annual Recurring Revenue)
The predictable annual revenue from subscriptions or recurring contracts.

---

## B

### Blockchain
A distributed ledger technology that maintains a continuously growing list of records (blocks) linked and secured using cryptography. MameyNode is a production-ready blockchain.

### Block Lattice
A blockchain architecture where each account has its own blockchain, enabling parallel processing and eliminating global bottlenecks. MameyNode uses Block Lattice architecture for improved scalability and performance.

### Banking Libraries
Proprietary .NET libraries (110+) providing complete banking infrastructure for microservices development.

---

## C

### CBDC (Central Bank Digital Currency)
A digital form of a country's fiat currency issued and regulated by the central bank. MameyNode provides complete CBDC infrastructure.

### CQRS (Command Query Responsibility Segregation)
An architectural pattern that separates read and write operations. Mamey Banking Libraries use CQRS.

### Compliance
Adherence to laws, regulations, and industry standards. Mamey provides automated compliance for AML/CFT, KYC, and regulatory reporting.

### Consensus
A mechanism for achieving agreement on a single data value among distributed systems. MameyNode uses DPoS (Delegated Proof-of-Stake) consensus.

---

## D

### DDD (Domain-Driven Design)
A software development approach that focuses on modeling software to match a domain according to input from domain experts. Mamey uses DDD patterns.

### DEX (Decentralized Exchange)
A cryptocurrency exchange that operates without a central authority. MameyNode includes a complete DEX with AMM and liquidity pools.

### DID (Decentralized Identifier)
A new type of identifier that enables verifiable, decentralized digital identity. Mamey Government Services uses DID for identity management.

### DPoS (Delegated Proof-of-Stake)
A consensus mechanism where token holders vote for delegates (representatives) to validate transactions. MameyNode uses DPoS with trusted representatives for fast finality (5.9ms average) and regulatory compliance.

### Master Trust Account
A hierarchical account structure in MameyNode where a master account contains multiple sub-accounts (savings, checking, currency wallets, loan accounts). Enables comprehensive account management for businesses and institutions.

---

## E

### Event Sourcing
An architectural pattern that stores all changes to application state as a sequence of events. Mamey Banking Libraries use Event Sourcing.

---

## F

### Finality
The point at which a transaction is considered irreversible. MameyNode achieves finality in ~5.9ms average.

---

## G

### gRPC
A high-performance, open-source RPC (Remote Procedure Call) framework. MameyNode provides gRPC APIs with 9+ services.

---

## H

### HIPAA (Health Insurance Portability and Accountability Act)
US healthcare data privacy and security regulations. Mamey Holistic Medicine is HIPAA compliant.

---

## I

### Identity Management
The process of managing digital identities. Mamey Government Services provides DID-based identity management.

---

## J

### JWT (JSON Web Token)
A compact, URL-safe token format for securely transmitting information. Mamey supports JWT authentication.

---

## K

### KYC (Know Your Customer)
The process of verifying the identity of customers. Mamey provides automated KYC verification.

---

## L

### Latency
The time delay between a request and response. MameyNode achieves < 50ms latency (p99).

### LMDB (Lightning Memory-Mapped Database)
A high-performance embedded database. MameyNode uses LMDB for blockchain storage.

---

## M

### MameyNode
Mamey's production-ready blockchain infrastructure designed specifically for regulated financial institutions and governments. Uses Block Lattice architecture with DPoS consensus, supporting 35+ modules (19 core + 16 specialized), 500+ functions, and 200+ use cases. Achieves 24,356+ TPS with 5.9ms finality. Features include Master Trust Accounts, multi-currency support, built-in compliance (AML/CFT, KYC, sanctions), and Universal Protocol Gateway (UPG) for multi-protocol connectivity.

### Microservices
An architectural approach where applications are built as a collection of small, independent services. Mamey provides 409+ production-ready microservices.

---

## N

### Node
A computer that participates in a blockchain network. MameyNode supports banking, government, and general nodes.

---

## O

### OAuth 2.0
An authorization framework for third-party applications. Mamey supports OAuth 2.0 authentication.

---

## P

### P2P (Peer-to-Peer)
A distributed network architecture where participants share resources directly. MameyNode uses P2P networking.

### Portable Nodes
Mobile and edge computing solutions for offline-capable banking operations with satellite connectivity.

---

## Q

### Query
A read operation in CQRS architecture. Mamey Banking Libraries separate commands (writes) from queries (reads).

---

## R

### RedWebNetwork
Mamey's social media platform providing complete Facebook clone functionality including posts, comments, reactions, messaging, groups, pages, events, stories, marketplace, watch, and gaming features.

### Pupitre
Mamey's educational platform for sovereign education systems. Features AI-first teaching, inclusive design for special needs, gamification, and verifiable credentials. Currently ~65% complete with microservices created. Future MameyNode integration planned for credential verification.

### Casino/MameyCasino
Mamey's AI-first, blockchain-native gaming platform. Features AI dealers, provably fair gaming, comprehensive game library (50+ games), and responsible gaming tools. Currently ~65% complete with microservices created. Future MameyNode integration planned for provably fair gaming and audit trails.

### REST (Representational State Transfer)
An architectural style for web services. Mamey provides REST APIs.

### RTGS (Real-Time Gross Settlement)
A funds transfer system where transactions are settled in real-time. MameyNode provides RTGS capabilities.

### ROI (Return on Investment)
A measure of the profitability of an investment. Mamey delivers 200-400% ROI over 3 years.

---

## S

### SDK (Software Development Kit)
A collection of software tools for developing applications. Mamey provides SDKs for .NET, JavaScript, Python, Rust, and Go.

### Smart Contracts
Self-executing contracts with terms directly written into code. MameyNode supports WASM-based smart contracts.

---

## T

### TAM (Total Addressable Market)
The total market demand for a product or service. Mamey's TAM is $1T+.

### TPS (Transactions Per Second)
A measure of transaction processing capacity. MameyNode achieves 24,356+ TPS.

---

## U

### UPG (Universal Protocol Gateway)
MameyNode's gateway supporting 15+ payment protocols (SWIFT, ISO 20022, RTGS, FedNow, RTP, PIX, UPI, crypto, etc.).

### Use Case
A specific scenario or application of a technology. Mamey supports 200+ use cases.

---

## V

### Verifiable Credentials
Digital credentials that can be cryptographically verified. Mamey Government Services uses verifiable credentials for identity.

---

## W

### WASM (WebAssembly)
A binary instruction format for stack-based virtual machines. MameyNode uses WASM for smart contract execution.

### WebSocket
A communication protocol for real-time, bidirectional communication. Mamey provides WebSocket APIs for real-time updates.

---

## Acronyms and Abbreviations

| Acronym | Full Form |
|---------|-----------|
| **AML** | Anti-Money Laundering |
| **API** | Application Programming Interface |
| **ARR** | Annual Recurring Revenue |
| **CBDC** | Central Bank Digital Currency |
| **CFT** | Counter-Terrorism Financing |
| **CQRS** | Command Query Responsibility Segregation |
| **DDD** | Domain-Driven Design |
| **DEX** | Decentralized Exchange |
| **DID** | Decentralized Identifier |
| **DPoS** | Delegated Proof-of-Stake |
| **EHR** | Electronic Health Record |
| **GDPR** | General Data Protection Regulation |
| **gRPC** | gRPC Remote Procedure Calls |
| **HIPAA** | Health Insurance Portability and Accountability Act |
| **JWT** | JSON Web Token |
| **KYC** | Know Your Customer |
| **LMDB** | Lightning Memory-Mapped Database |
| **P2P** | Peer-to-Peer |
| **REST** | Representational State Transfer |
| **ROI** | Return on Investment |
| **RTGS** | Real-Time Gross Settlement |
| **SAM** | Serviceable Addressable Market |
| **SDK** | Software Development Kit |
| **SOM** | Serviceable Obtainable Market |
| **TAM** | Total Addressable Market |
| **TLS** | Transport Layer Security |
| **TPS** | Transactions Per Second |
| **UPG** | Universal Protocol Gateway |
| **WASM** | WebAssembly |

---

## Platform-Specific Terms

### Banking Libraries Terms

- **Command**: A write operation in CQRS architecture
- **Event**: A record of something that happened in the system
- **Aggregate**: A cluster of domain objects treated as a single unit
- **Repository**: An abstraction for data access

### MameyNode Terms

- **Block**: A collection of transactions in the blockchain
- **Account**: A blockchain account with balance and state
- **Transaction**: An operation that changes blockchain state
- **Confirmation**: The process of validating and finalizing a transaction
- **Mempool**: A pool of unconfirmed transactions

### Government Services Terms

- **Digital Identity**: A digital representation of an individual
- **Verifiable Credential**: A cryptographically verifiable digital credential
- **Document Verification**: The process of verifying document authenticity
- **Voting System**: A secure electronic voting platform

### Healthcare Terms

- **Patient Records**: Electronic health records for patients
- **Telemedicine**: Remote healthcare delivery
- **Wellness Tracking**: Health and wellness monitoring

---

## Technical Terms

### Architecture Terms

- **Microservices**: Small, independent services
- **Monolith**: A single, unified application
- **Service Mesh**: Infrastructure layer for service-to-service communication
- **API Gateway**: A single entry point for API requests

### Database Terms

- **PostgreSQL**: Open-source relational database
- **MongoDB**: NoSQL document database
- **Redis**: In-memory data structure store
- **LMDB**: Lightning Memory-Mapped Database

### Message Broker Terms

- **RabbitMQ**: Open-source message broker
- **Kafka**: Distributed event streaming platform
- **Event Stream**: A sequence of events

### Security Terms

- **Encryption**: The process of encoding information
- **Zero-Trust**: Security model with no implicit trust
- **Audit Trail**: A record of system activities
- **Key Management**: The process of managing cryptographic keys

---

## Business Terms

### Financial Terms

- **Revenue**: Income from business operations
- **ARR**: Annual Recurring Revenue
- **TCO**: Total Cost of Ownership
- **ROI**: Return on Investment
- **Payback Period**: Time to recover investment

### Market Terms

- **TAM**: Total Addressable Market
- **SAM**: Serviceable Addressable Market
- **SOM**: Serviceable Obtainable Market
- **Market Share**: Percentage of market controlled

### Partnership Terms

- **Technology Partner**: Integration partnership
- **System Integrator**: Implementation partnership
- **Reseller**: Distribution partnership
- **Strategic Partner**: Joint go-to-market partnership

---

## Contact

**Questions about Terms**:  
Email: info@mamey.io  
Documentation: docs.mamey.io

---

**Mamey Technologies** - Building better financial infrastructure for the sovereign era

*This glossary provides definitions of key terms. For additional terms, see the documentation or contact support.*







