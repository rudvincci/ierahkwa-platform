# Monetización Activa · IERAHKWA

## Fuentes de ingresos implementadas

### 1. Pagos (pagos-soberano)
- **Fee:** 0.5% (config: `PAYMENT_FEE_RATE`)
- **Wallet plataforma:** `PLATFORM_REVENUE_WALLET`
- **API:** `createPaymentIntent` incluye `fee`, `netAmount`, `platformFee`

### 2. Marketplace
- **Fee:** 0% indígenas/pequeños (<$10k/mes), 5% medio, 8% grande
- **API:** `POST /api/v1/market/orders` con `sellerTier`, `sellerMonthlyVolume`, `isIndigenous`
- Respuesta: `platformFee`, `sellerPayout`

### 3. Mamey Futures / Trading
- **Spot:** Maker 0.02%, Taker 0.04%
- **Futures:** Maker 0.01%, Taker 0.03%
- **Options:** Maker 0.02%, Taker 0.05%
- **API:** `GET /api/v1/mamey/revenue`

### 4. Licencias (License Authority)
- **Fees:** $100 - $1M según tipo (80+ tipos)
- **API:** `GET /api/v1/licenses/statistics` → `totalFeesCollected`

### 5. Premium Pro (Freemium)
- **Tiers:** Free ($0), Pro Individual ($9.99), Pro Business ($49.99), Creator Pro ($19.99)
- **API:** `/api/v1/premium/tiers`, `/api/v1/premium/check/:userId`, `POST /api/v1/premium/subscribe`

## Resumen consolidado

```
GET /api/v1/revenue-aggregate/summary
```

Devuelve: `payments`, `marketplace`, `trading`, `licenses`, `premium` y total.
