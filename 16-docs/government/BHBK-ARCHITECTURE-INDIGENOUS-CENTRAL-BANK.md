# BHBK Architecture — Indigenous Central Bank

## 2026 Goal

- Establish the ecosystem as a **sovereign power**.
- **The Americas and the Caribbean** are its territory.
- BHBK’s architecture operates in an **integrated manner** within this territory.
- The bank is an **Infrastructure Owner** that protects indigenous operations independently, not just a “service provider”.

---

## Single Point of Administration

**Everything is managed from Ierahkwa Futurehead BDET Bank (back).**

Bank registration, settlement, four central banks, treasury, compliance, RWA (Real World Assets), and citizen services are managed via the BDET Bank interface and APIs:

- **`platform/bdet-bank.html`**
- **Node** (port 8545)
- **Banking Bridge** (port 3001)

A single back-end system operates the entire ecosystem.

---

## Node Hierarchy (International Settlement + 4 Central Banks)

### One node: International Settlement (SIIS)

- International settlement layer.
- Operates on port **8500** with **`NODE_ROLE=settlement`**.

### Below it: 4 independent central bank nodes

| Node    | Port | Role           | Region   |
|---------|------|----------------|----------|
| **Eagle**    | 8545 | Central bank   | North    |
| **Quetzal**  | 8546 | Central bank   | Center   |
| **Condor**   | 8547 | Central bank   | South    |
| **Caribbean**| 8548 | Central bank   | Caribbean|

**If one falls, the other three continue.**

### At each node

- **Regional**, **National**, and **Commercial** levels are **separated** into different layers.
- Each layer has its own registry and logic.

### Between nodes

- **All nodes are interconnected.**
- The Settlement node connects to the four central banks.
- The four central banks connect to the Settlement node and to each other (mesh network).
- Clearing, SIIS, and registry are coordinated through this network.

**Details:** `docs/CUATRO-NODOS-REGIONES.md` and `RuddieSolution/node/ecosystem.4regions.config.js`.

---

## Internal Departments of the Bank (Power Structure)

These departments are the **heart of the bank** and dictate rules for all sectors.

| Department                    | Function |
|------------------------------|----------|
| **Treasury and Reserves**    | Safeguards **Futurehead Trust Coins (FHTC)** and gold/assets backing the currency. Manages liquidity so there is sufficient capital for seeding. |
| **Risk Management and Compliance** | Ensures medicinal plant compliance with sovereign laws. Provides security for “sensitive” industries (e.g. marijuana/hemp), preventing external blockages. |
| **Real Assets (RWA)**        | Digitizes indigenous land into NFTs. Manages land as collateral when a farmer uses it. |
| **Technology and Node 8545** | Maintains the bank’s server and blockchain security. The brain that connects money with delivery. |

---

## Specialized Services (Citizen Services)

These are **direct services** the bank offers to **other bots and citizens**.

| Service                         | Description |
|---------------------------------|-------------|
| **Futures Banking**             | Foreign governments buy early harvests, injecting fresh money into the bank. |
| **Dynamic Agricultural Credit** | “Plant-by-plant” loans with preferential rates (1.5%) that adjust according to productivity. |
| **Indigenous Trusts**          | Protection of community assets and management of franchise royalties. |
| **Multi-Currency Payment Gateway** | Instantly exchanges product tokens (e.g. IGT-HEMP) for dollars or euros for investors. |

---

## Operation Matrix (Bot Interaction)

Each **external bot** communicates with a **specific department** of the bank.

| External Bot     | Bank Department        | Service / Action                    |
|------------------|------------------------|-------------------------------------|
| **Agriculture Bot** | Real Assets (RWA)    | Land collateral validation.         |
| **Logistics Bot**  | Treasury              | Release of payments after confirmed delivery. |
| **Citizen Bot**    | Technology (Node 8545)| Unified login (SSO) and reward payments. |
| **Casino Bot**     | Risk Management       | Prize audit and custody of funds.   |

---

## Summary

- BHBK is administered from a **single point**: BDET Bank (back) and its APIs.
- **One Settlement node (SIIS)** plus **four central bank nodes** (Eagle, Quetzal, Condor, Caribbean) form a mesh; clearing and registry are coordinated across the network.
- **Internal departments** (Treasury, Risk/Compliance, RWA, Technology) define rules and operations.
- **Citizen-facing services** (Futures, Agricultural Credit, Trusts, Payment Gateway) are exposed to bots and citizens.
- **Bot interaction** is department-specific (e.g. Agriculture Bot → RWA for land collateral).

**References:**  
- Spanish: `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md`  
- Nodes: `docs/CUATRO-NODOS-REGIONES.md`, `RuddieSolution/node/ecosystem.4regions.config.js`  
- Code: `RuddieSolution/node/server.js`, `RuddieSolution/node/banking-bridge.js`, `RuddieSolution/platform/bdet-bank.html`
