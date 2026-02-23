# IERAHKWA FarmFactory

**.NET 10** · Estructura IERAHKWA · Staking & yield farming en Ethereum, BSC, Polygon, Aurora, xDai e IERAHKWA.

Sistema de **staking de activos** y **yield farming** inspirado en FarmFactory (CodeCanyon): depósito/retiro de tokens ERC20/BEP20 y reparto de recompensas por participación `(cantidad × tiempo) / total`.

## Características

- **Redes**: ETH, BSC, Polygon, Aurora, xDai, IERAHKWA
- **Tokens**: ERC20 / BEP20 (staking y reward pueden ser iguales o distintos)
- **Deposit / Withdraw / Claim** en cualquier momento
- **Cálculo de rewards**: `share = (amount × time_staked) / total_weight × rewards_distributed`
- **Período de farming** configurable: TotalReward y ventana Start–End

## Estructura (IERAHKWA)

```
FarmFactory/
├── FarmFactory.API        # Controllers, Program, wwwroot
├── FarmFactory.Core       # Models (FarmPool, FarmDeposit), IFarmFactoryService
└── FarmFactory.Infrastructure  # FarmFactoryService (in-memory, lógica de rewards)
```

## Ejecutar

```bash
cd FarmFactory
dotnet run --project FarmFactory.API
```

- **API**: http://localhost:5061
- **UI**: http://localhost:5061/index.html
- **Swagger**: http://localhost:5061/swagger
- **Health**: http://localhost:5061/health

## API principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/pools | Listar pools (opcional: `?network=ETH`, `?activeOnly=true`) |
| GET | /api/pools/{id} | Detalle de pool |
| POST | /api/pools | Crear pool (admin) |
| POST | /api/farms/deposit | `{ poolId, userWallet, amount }` |
| POST | /api/farms/withdraw | `{ depositId, userWallet }` |
| POST | /api/farms/claim | `{ userWallet, depositId?, poolId? }` |
| GET | /api/farms/deposits | `?wallet=0x...` `?poolId=...` |
| GET | /api/farms/deposits/{id}/pending-reward | Pending por depósito |
| GET | /api/farms/pending-reward | `?wallet=0x...&poolId=...` |

## Integración en IERAHKWA Platform

- **platform-config.json**: card en BANKING & FINANCE y `urls.farmfactory`
- **IERAHKWA_PLATFORM_V1.html**: tarjeta FarmFactory y `platforms.farmfactory`

---

**Sovereign Government of Ierahkwa Ne Kanienke** · © 2026
