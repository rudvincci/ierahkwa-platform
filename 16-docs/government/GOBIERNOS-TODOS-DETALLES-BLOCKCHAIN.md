# Todos los gobiernos — Cada uno con detalles plataforma blockchain

**Sovereign Government of Ierahkwa Ne Kanienke · Plataforma de la unidad — Las Américas**

Este documento lista **todos los gobiernos** de la estructura (Global, Central, Confederación, Naciones) y para cada uno incluye los **detalles de la plataforma blockchain** (nodo, Chain ID, tokens, bancos, APIs).

**Documento detallado del nodo principal (IERAHKWA):** [DETALLES-PLATAFORMA-BLOCKCHAIN.md](./DETALLES-PLATAFORMA-BLOCKCHAIN.md)

---

## Índice de gobiernos

| # | Gobierno / Nivel | Código | Blockchain / Nodo |
|---|------------------|--------|-------------------|
| 1 | [Global — Las Américas Indígenas](#1-global--las-américas-indígenas) | GIFN / Continental | ISB 8545, Chain 77777 |
| 2 | [Central Águila (Norte)](#2-central-águila-norte) | Águila | ISB 8545, Banco 4100 |
| 3 | [Central Quetzal (Centro)](#3-central-quetzal-centro) | Quetzal | ISB 8545, Banco 4200 |
| 4 | [Central Cóndor (Sur)](#4-central-cóndor-sur) | Cóndor | ISB 8545, Banco 4300 |
| 5 | [Central Caribe (Taínos)](#5-central-caribe-taínos) | Caribe | ISB 8545, Banco 4400 |
| 6 | [Sovereign Government of Ierahkwa Ne Kanienke](#6-sovereign-government-of-ierahkwa-ne-kanienke) | SGINK / Ierahkwa | ISB 8545, BDET, IISB |
| 7 | [Haudenosaunee Confederacy](#7-haudenosaunee-confederacy) | HC | ISB 8545 |
| 8 | [Mohawk Nation Council](#8-mohawk-nation-council) | MNC | ISB 8545 |
| 9 | [Oneida Nation](#9-oneida-nation) | ON | ISB 8545 |
| 10 | [Onondaga Nation](#10-onondaga-nation) | ONN | ISB 8545 |
| 11 | [Cayuga Nation](#11-cayuga-nation) | CN | ISB 8545 |
| 12 | [Seneca Nation](#12-seneca-nation) | SN | ISB 8545 |
| 13 | [Tuscarora Nation](#13-tuscarora-nation) | TN | ISB 8545 |
| 14 | [Jurisdicciones financieras (PRIMARY, CARIBBEAN, GLOBAL)](#14-jurisdicciones-financieras) | INK, CIEZ, GIFN | ISB 8545 |
| 15 | [Naciones indígenas de las Américas (~120)](#15-naciones-indígenas-de-las-américas) | AKW, KAH, MAY, … | ISB 8545, SWIFT por nación |

---

## 1. Global — Las Américas Indígenas

| Dato | Valor |
|------|--------|
| **Nombre** | Las Américas Indígenas — coordinador continental |
| **Nivel** | Global (1) |
| **Código** | GIFN (Global Indigenous Financial Network) |
| **Blockchain** | Ierahkwa Sovereign Blockchain (ISB) |
| **Nodo RPC** | Puerto 8545, Chain ID 77777 |
| **Consenso** | SPoA, gas 0, finality instantánea |
| **Tokens** | 101 IGT (todos); uso transversal por región |
| **Bancos** | BDET central, IISB; coordinación con 4 centrales (Águila, Quetzal, Cóndor, Caribe) |
| **APIs** | Mismo nodo: `/rpc`, `/api/v1/stats`, `/api/v1/tokens`, `/api/v1/blocks` |
| **Referencia** | PLAN-GOBIERNOS-AMERICAS-COMPLETO.md, sovereign-financial-center.js (GLOBAL) |

---

## 2. Central Águila (Norte)

| Dato | Valor |
|------|--------|
| **Nombre** | Banco Central Águila (Norte América) |
| **Nivel** | Central (4 centrales) |
| **Código** | Águila |
| **Puerto banco** | 4100 (central-bank-server.js, BANK_NAME=aguila) |
| **Blockchain** | ISB, Chain ID 77777, nodo 8545 |
| **Región** | Norteamérica |
| **Bancos regionales (datos)** | Banco Regional Norte Reserve, Banco Regional Sur Reserve (aguila.json) |
| **Tokens** | IGT 01–40 (Gobierno), 41–52 (Finanzas); asignación regional Norte |
| **APIs** | Health: `http://localhost:4100/health`; RPC/IGT vía 8545 |

---

## 3. Central Quetzal (Centro)

| Dato | Valor |
|------|--------|
| **Nombre** | Banco Central Quetzal (Centro América) |
| **Nivel** | Central |
| **Código** | Quetzal |
| **Puerto banco** | 4200 (BANK_NAME=quetzal) |
| **Blockchain** | ISB, Chain ID 77777, nodo 8545 |
| **Región** | Centroamérica |
| **Bancos regionales (datos)** | Banco Regional Maya Reserve, Banco Regional Istmo Reserve (quetzal.json) |
| **Tokens** | IGT; asignación regional Centro |
| **APIs** | Health: `http://localhost:4200/health`; RPC/IGT vía 8545 |

---

## 4. Central Cóndor (Sur)

| Dato | Valor |
|------|--------|
| **Nombre** | Banco Central Cóndor (Sur América) |
| **Nivel** | Central |
| **Código** | Cóndor |
| **Puerto banco** | 4300 (BANK_NAME=condor) |
| **Blockchain** | ISB, Chain ID 77777, nodo 8545 |
| **Región** | Sudamérica |
| **Bancos regionales (datos)** | Banco Regional Andino Reserve, Banco Regional Sur Reserve (condor.json) |
| **Tokens** | IGT; asignación regional Sur |
| **APIs** | Health: `http://localhost:4300/health`; RPC/IGT vía 8545 |

---

## 5. Central Caribe (Taínos)

| Dato | Valor |
|------|--------|
| **Nombre** | Banco Central Caribe (Taínos / Caribe) |
| **Nivel** | Central |
| **Código** | Caribe |
| **Puerto banco** | 4400 (BANK_NAME=caribe) |
| **Blockchain** | ISB, Chain ID 77777, nodo 8545 |
| **Región** | Caribe |
| **Bancos regionales (datos)** | Banco Regional Taíno Reserve, Banco Regional Antillas Reserve (caribe.json) |
| **Tokens** | IGT; asignación regional Caribe |
| **APIs** | Health: `http://localhost:4400/health`; RPC/IGT vía 8545 |

---

## 6. Sovereign Government of Ierahkwa Ne Kanienke

| Dato | Valor |
|------|--------|
| **Nombre** | Sovereign Government of Ierahkwa Ne Kanienke |
| **Código** | SGINK / Ierahkwa |
| **Tipo** | Primary Sovereign |
| **Blockchain** | Ierahkwa Sovereign Blockchain (ISB) — nodo principal Mamey |
| **Nodo** | Ierahkwa Futurehead Mamey Node, puerto 8545, Chain ID 77777 |
| **Tokens** | 101 IGT (IGT-PM, IGT-MFA, IGT-BDET, IGT-MAIN, IGT-IISB, etc.) |
| **Bancos** | BDET (IERBDETXXX), IISB (IISBGLOB) — CONNECTED en config |
| **Departamentos** | 41 (government-departments.json): PM, MFA, MFT, MJ, BDET, NT, Salud, Educación, etc. |
| **Documento completo** | [DETALLES-PLATAFORMA-BLOCKCHAIN.md](./DETALLES-PLATAFORMA-BLOCKCHAIN.md) |

---

## 7. Haudenosaunee Confederacy

| Dato | Valor |
|------|--------|
| **Nombre** | Haudenosaunee Confederacy |
| **Código** | HC |
| **Tipo** | Confederacy |
| **Blockchain** | ISB, nodo 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 50,000,000,000,000 |
| **Tokens** | IGT; uso compartido con nodo soberano |
| **APIs** | Mismo nodo 8545; login y segregación por gobierno (plan) |

---

## 8. Mohawk Nation Council

| Dato | Valor |
|------|--------|
| **Nombre** | Mohawk Nation Council |
| **Código** | MNC |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 25,000,000,000,000 |
| **Nación indígena** | Akwesasne, Kahnawake, Kanehsatà:ke (SWIFT IERAKWXXX, IERKAHXXX, IERKANXXX) |

---

## 9. Oneida Nation

| Dato | Valor |
|------|--------|
| **Nombre** | Oneida Nation |
| **Código** | ON |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 20,000,000,000,000 |

---

## 10. Onondaga Nation

| Dato | Valor |
|------|--------|
| **Nombre** | Onondaga Nation |
| **Código** | ONN |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 20,000,000,000,000 |

---

## 11. Cayuga Nation

| Dato | Valor |
|------|--------|
| **Nombre** | Cayuga Nation |
| **Código** | CN |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 18,000,000,000,000 |

---

## 12. Seneca Nation

| Dato | Valor |
|------|--------|
| **Nombre** | Seneca Nation |
| **Código** | SN |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 22,000,000,000,000 |

---

## 13. Tuscarora Nation

| Dato | Valor |
|------|--------|
| **Nombre** | Tuscarora Nation |
| **Código** | TN |
| **Tipo** | Nation |
| **Blockchain** | ISB, 8545, Chain ID 77777 |
| **Capital (referencia shop)** | 15,000,000,000,000 |

---

## 14. Jurisdicciones financieras

| Jurisdicción | Código | Tipo | Autoridad | Blockchain |
|--------------|--------|------|-----------|------------|
| **Ierahkwa Ne Kanienke Sovereign Territory** | INK | PRIMARY_SOVEREIGN | Constitutional Land Custodian - North America | ISB 8545, 77777 |
| **Caribbean Indigenous Economic Zone** | CIEZ | TREATY_ZONE | Taíno-Kalinago Confederation Partnership | ISB 8545, 77777 |
| **Global Indigenous Financial Network** | GIFN | INTERNATIONAL | Inter-Indigenous Sovereign Treaty | ISB 8545, 77777 |

*Fuente: node/modules/sovereign-financial-center.js*

---

## 15. Naciones indígenas de las Américas

**Todas** usan la **misma plataforma blockchain**: ISB, nodo 8545, Chain ID 77777. Cada nación tiene **código** y **SWIFT** propio (Bancos Locales IERAHKWA). Regiones: Norteamérica, Centroamérica, Sudamérica, Caribe.

| ID | Nombre | Región | SWIFT |
|----|--------|--------|-------|
| AKW | Akwesasne | Norteamérica | IERAKWXXX |
| KAH | Kahnawake | Norteamérica | IERKAHXXX |
| KAN | Kanehsatà:ke | Norteamérica | IERKANXXX |
| SIX | Six Nations | Norteamérica | IERSIXXXX |
| NAV | Navajo | Norteamérica | IERNAVXXX |
| CHR | Cherokee | Norteamérica | IERCHRXXX |
| CHO | Choctaw | Norteamérica | IERCHOXXX |
| LAK | Lakota/Sioux | Norteamérica | IERLAKXXX |
| MAY | Maya | Centroamérica | IERMAYXXX |
| NAH | Nahua | Centroamérica | IERNAHXXX |
| QUECH | Quechua | Sudamérica | IERQUXXX |
| AYM | Aymara | Sudamérica | IERAYMXXX |
| MAP | Mapuche | Sudamérica | IERMAPXXX |
| GUA | Guaraní | Sudamérica | IERGUAXXX |
| TAI | Taíno | Caribe | IERTAIXXX |
| KARIB | Kalinago | Caribe | IERKALXXX |
| … | *(~120 naciones en total)* | … | IER*XXX |

**Lista completa:** `RuddieSolution/data/indigenous-nations.json` (meta.total: 800; array con ~120 ejemplos con SWIFT).

**Detalles blockchain (común a todas):**
- **Nodo:** Ierahkwa Futurehead Mamey Node (:8545)
- **Chain ID:** 77777
- **Tokens:** IGT (IGT-20); asignación por nación/región según admin
- **APIs:** `/rpc`, `/api/v1/stats`, `/api/v1/tokens`, `/api/v1/blocks`; health del nodo: `/health`
- **Bancos:** BDET central; cada nación con SWIFT propio en lista

---

## Resumen técnico común (todos los gobiernos)

| Elemento | Valor |
|----------|--------|
| **Blockchain** | Ierahkwa Sovereign Blockchain (ISB) |
| **Nodo principal** | Ierahkwa Futurehead Mamey Node |
| **Puerto RPC** | 8545 |
| **Chain ID** | 77777 |
| **Consenso** | SPoA |
| **Gas price** | 0 |
| **Estándar token** | IGT-20 (101 tokens registrados) |
| **Bancos centrales (puertos)** | Águila 4100, Quetzal 4200, Cóndor 4300, Caribe 4400 |
| **BDET / IISB** | IERBDETXXX, IISBGLOB (conectados al nodo) |
| **Documento detallado nodo** | [DETALLES-PLATAFORMA-BLOCKCHAIN.md](./DETALLES-PLATAFORMA-BLOCKCHAIN.md) |

---

*Sovereign Government of Ierahkwa Ne Kanienke · Plataforma de la unidad — Las Américas · One Love, One Life*
