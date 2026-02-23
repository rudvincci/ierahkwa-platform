# NET10 DeFi Platform

## Decentralized Finance Exchange
### Swap • Pools • Farming | .NET 10

---

## 🌐 OVERVIEW

NET10 es la plataforma DeFi oficial del Gobierno Soberano. Ofrece swap de tokens, pools de liquidez y yield farming en múltiples chains.

## 🏗️ ARQUITECTURA

```
┌─────────────────────────────────────────────────────────────┐
│                       NET10 DEFI                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│   ┌────────────┐  ┌────────────┐  ┌────────────┐           │
│   │    SWAP    │  │  LIQUIDITY │  │   YIELD    │           │
│   │   ENGINE   │  │   POOLS    │  │  FARMING   │           │
│   └────────────┘  └────────────┘  └────────────┘           │
│          │               │               │                   │
│   ┌──────┴───────────────┴───────────────┴──────┐          │
│   │              AMM (Automated Market Maker)    │          │
│   └─────────────────────────────────────────────┘          │
│                          │                                   │
│   ┌─────────────────────────────────────────────┐          │
│   │    MULTI-CHAIN BRIDGE (ETH, BSC, POLYGON)   │          │
│   └─────────────────────────────────────────────┘          │
│                          │                                   │
│   ┌─────────────────────────────────────────────┐          │
│   │        IERAHKWA SOVEREIGN BLOCKCHAIN         │          │
│   └─────────────────────────────────────────────┘          │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 FUNCIONALIDADES

### 1. Token Swap
- Swap instantáneo entre tokens
- Routing optimizado
- Slippage protection
- Price impact warning

### 2. Liquidity Pools
- Crear/agregar liquidez
- LP tokens
- Fee sharing (0.3%)
- Impermanent loss calculator

### 3. Yield Farming
- Stake LP tokens
- Earn IGT rewards
- Variable APY
- Auto-compound option

## 📊 POOLS DISPONIBLES

| Pool | APY | TVL |
|------|-----|-----|
| IGT-MAIN/USDT | 25% | $1M+ |
| IGT-STABLE/USDC | 15% | $500K+ |
| IGT-DEFI/ETH | 40% | $200K+ |

### 4. Contribution Graph (GitHub-Style)
- Visualización de actividad estilo GitHub
- Tracking de commits, transacciones, votos, stakes
- Estadísticas de usuario (streaks, proyectos)
- Leaderboard de contribuidores
- Distribución por tipo de actividad

## 📡 API ENDPOINTS

```
Base URL: http://localhost:5071/api/v1

# Swap
GET  /swap/quote        - Get swap quote
POST /swap/execute      - Execute swap
GET  /swap/routes       - Available routes

# Pools
GET  /pools             - All pools
GET  /pools/{id}        - Pool details
POST /pools/add         - Add liquidity
POST /pools/remove      - Remove liquidity

# Farming
GET  /farms             - All farms
POST /farms/stake       - Stake LP tokens
POST /farms/unstake     - Unstake
POST /farms/harvest     - Claim rewards

# Contribution Graph (NEW)
GET  /contribution/graph/{userId}        - Get contribution graph (current year)
GET  /contribution/graph/{userId}/{year} - Get contribution graph (specific year)
GET  /contribution/stats/{userId}        - Get contribution statistics
GET  /contribution/user/{userId}         - Get recent contributions
GET  /contribution/projects/{userId}     - Get user projects
GET  /contribution/leaderboard           - Get top contributors
GET  /contribution/leaderboard/month     - Top contributors this month
GET  /contribution/leaderboard/week      - Top contributors this week
POST /contribution                       - Add new contribution
POST /contribution/batch                 - Add contributions in batch
```

## 🔐 SEGURIDAD

- Smart contracts auditados
- Multi-sig treasury
- Time-lock para cambios
- Bug bounty program

## 📁 ESTRUCTURA

```
NET10/
├── NET10.API/
│   ├── Controllers/
│   │   ├── SwapController.cs
│   │   ├── PoolController.cs
│   │   ├── FarmController.cs
│   │   ├── TokenController.cs
│   │   ├── AdminController.cs
│   │   └── ContributionController.cs  ← NEW
│   ├── wwwroot/
│   │   ├── index.html
│   │   └── contributions.html         ← NEW
│   └── Program.cs
├── NET10.Core/
│   ├── Models/
│   │   ├── Token.cs
│   │   ├── Farm.cs
│   │   ├── LiquidityPool.cs
│   │   ├── Swap.cs
│   │   └── Contribution.cs            ← NEW
│   └── Interfaces/
│       └── IServices.cs
├── NET10.Infrastructure/
│   └── Services/
│       ├── TokenService.cs
│       ├── PoolService.cs
│       ├── SwapService.cs
│       ├── FarmService.cs
│       └── ContributionService.cs     ← NEW
└── IerahkwaNET10.sln
```

## 🚀 DEPLOYMENT

```bash
cd NET10/NET10.API
dotnet run --urls "http://localhost:5071"
```

---

**Puerto:** 5071
**Estado:** ✅ ACTIVO
**Token:** IGT-DEFI

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
