# Ierahkwa Sovereign Platform — Architecture

> **Version:** 1.0.0 | **Updated:** 2026-02-22 | **Maintainer:** Sovereign Government of Ierahkwa Ne Kanienke

---

## System Overview

```mermaid
graph TB
    subgraph "Tier 1 — Akwesasne Content Layer"
        A1[35 HTML Platforms]
        A2[VozSoberana MVP]
        A3[Mobile App - React Native]
    end

    subgraph "Tier 2 — Ierahkwa Application Layer"
        B1[API Gateway :3000]
        B2[Identity Service :5001]
        B3[Governance Service :5002]
        B4[Treasury Service :5003]
        B5[16 Node.js Services :3000-3500]
        B6[37 .NET Microservices :5010-5050]
    end

    subgraph "Tier 3 — Mamey Backend & Blockchain"
        C1[PostgreSQL 16]
        C2[Redis 7]
        C3[MameyNode Chain 777777]
        C4[WAMPUM Token Engine]
    end

    A1 --> B1
    A2 --> B1
    A3 --> B1
    B1 --> B2 & B3 & B4 & B5 & B6
    B2 & B3 --> C1
    B3 & B4 --> C3
    B4 --> C4
    B5 & B6 --> C1 & C2
```

## Service Communication

```mermaid
flowchart LR
    Client([Browser]) -->|HTTPS TLS 1.3| GW[API Gateway :3000]
    GW -->|gRPC| IDS[Identity :5001]
    GW -->|gRPC| GOV[Governance :5002]
    GW -->|gRPC| TRS[Treasury :5003]
    GW -->|REST| NODE[Node.js Services]
    GW -->|REST| NET[.NET Microservices]
    IDS & GOV & TRS --> PG[(PostgreSQL)]
    NODE --> PG & RD[(Redis)]
    GOV & TRS -->|JSON-RPC :8545| MN[MameyNode]
```

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| **Frontend** | HTML5, React 19, Blazor WASM, React Native |
| **Backend** | .NET 10, Node.js 22, Go 1.22, Rust 1.80+ |
| **Blockchain** | MameyNode (Rust), Chain 777777, WAMPUM token |
| **Data** | PostgreSQL 16, Redis 7, IPFS |
| **Infra** | Docker 27, Kubernetes 1.30, Terraform |
| **Security** | ML-DSA-65, ML-KEM-1024, ZKP, TLS 1.3 |

## MameyNode Blockchain

| Parameter | Value |
|-----------|-------|
| Chain ID | 777777 |
| Consensus | Sovereign Proof-of-Authority |
| Block Time | 3 seconds |
| Throughput | 12,847 TPS |
| Native Token | WAMPUM (Ⓦ) |
| Post-Quantum | ML-DSA-65 (FIPS 204) |
| EVM | Shanghai-compatible |

## Port Map

| Port | Service | Stack |
|------|---------|-------|
| 80/443 | Nginx Proxy | Infra |
| 3000 | API Gateway | Node.js |
| 3002 | VozSoberana | Node.js |
| 3030 | POS System | Node.js |
| 3100 | Ierahkwa Shop | Fastify |
| 3200 | Inventory | Node.js |
| 3300 | Image Upload | Node.js |
| 3400 | Forex Trading | Node.js |
| 3500 | Smart School | Node.js |
| 5001 | Identity | .NET 10 |
| 5002 | Governance | .NET 10 |
| 5003 | Treasury | .NET 10 |
| 5010-5050 | 37 Microservices | .NET 10 |
| 5432 | PostgreSQL | DB |
| 6379 | Redis | Cache |
| 8545 | MameyNode RPC | Blockchain |
| 9090 | Prometheus | Monitoring |

---

*Niawenhko:wa — Built with sovereignty in mind.*
