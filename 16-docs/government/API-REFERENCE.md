# ══════════════════════════════════════════════════════════════════════════════
# API REFERENCE - IERAHKWA SOVEREIGN PLATFORM
# Sovereign Government of Ierahkwa Ne Kanienke
# ══════════════════════════════════════════════════════════════════════════════

```
╔══════════════════════════════════════════════════════════════════════════════╗
║                                                                              ║
║                    IERAHKWA SOVEREIGN PLATFORM                               ║
║                    API REFERENCE v1.0                                        ║
║                                                                              ║
║                    Documentación Técnica Completa                            ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
```

---

# TABLA DE CONTENIDOS

1. [Información General](#información-general)
2. [Platform API — Servicios comerciales (todas las plataformas)](#platform-api--servicios-comerciales-todas-las-plataformas)
3. [Autenticación](#autenticación)
4. [TradeX Exchange API](#tradex-exchange-api)
5. [Wallet API](#wallet-api)
6. [Staking API](#staking-api)
7. [Node API](#node-api)
8. [Códigos de Error](#códigos-de-error)

---

# INFORMACIÓN GENERAL

## Base URLs

| Servicio | URL Base | Puerto |
|----------|----------|--------|
| **Node / Platform** | `http://localhost:8545` | 8545 |
| TradeX Exchange | `http://localhost:5054/api` | 5054 |
| NET10 DeFi | `http://localhost:5071/api` | 5071 |
| FarmFactory | `http://localhost:5061/api` | 5061 |
| SpikeOffice HR | `http://localhost:5056/api` | 5056 |
| RnBCal | `http://localhost:5055/api` | 5055 |
| AppBuilder | `http://localhost:5060/api` | 5060 |
| IDOFactory | `http://localhost:5097/api` | 5097 |
| SmartSchool | `http://localhost:3000/api` | 3000 |
| Ierahkwa Shop | `http://localhost:3001/api` | 3001 |

## Headers Comunes

```http
Content-Type: application/json
Accept: application/json
Authorization: Bearer {token}
X-API-Key: {api_key}
```

## Rate Limits

| Endpoint Type | Límite | Ventana |
|---------------|--------|---------|
| Public | 100 req | 1 min |
| Authenticated | 1000 req | 1 min |
| Trading | 50 req | 1 sec |
| WebSocket | Unlimited | - |

---

# AUTENTICACIÓN

## Obtener Token JWT

```http
POST /api/auth/login
```

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "your_password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g...",
  "expiresIn": 3600,
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "vipLevel": "Gold"
  }
}
```

## Refresh Token

```http
POST /api/auth/refresh
```

**Request Body:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2g..."
}
```

---

# TRADEX EXCHANGE API

**Base URL:** `http://localhost:5054/api`

## Trading Pairs

### Get All Trading Pairs

```http
GET /api/trading/pairs
```

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "symbol": "IGT-PM/USDT",
      "baseAsset": "IGT-PM",
      "quoteAsset": "USDT",
      "lastPrice": 1.0,
      "change24h": 2.5,
      "high24h": 1.05,
      "low24h": 0.95,
      "volume24h": 1500000,
      "takerFee": 0.001,
      "makerFee": 0.0008,
      "minOrderAmount": 10,
      "maxOrderAmount": 1000000,
      "isActive": true
    }
  ]
}
```

### Get Single Trading Pair

```http
GET /api/trading/pairs/{id}
```

## Orders

### Place Order

```http
POST /api/trading/orders
```

**Request Body:**
```json
{
  "userId": "uuid",
  "tradingPairId": "uuid",
  "side": "Buy",
  "type": "Limit",
  "amount": 100,
  "price": 1.05,
  "stopPrice": null
}
```

**Order Types:**
- `Market` - Ejecuta al mejor precio disponible
- `Limit` - Ejecuta al precio especificado o mejor
- `StopLimit` - Limit order que se activa al alcanzar stopPrice
- `StopMarket` - Market order que se activa al alcanzar stopPrice

**Order Sides:**
- `Buy` - Compra
- `Sell` - Venta

**Response:**
```json
{
  "id": "uuid",
  "userId": "uuid",
  "tradingPairId": "uuid",
  "side": "Buy",
  "type": "Limit",
  "price": 1.05,
  "amount": 100,
  "filledAmount": 0,
  "remainingAmount": 100,
  "fee": 0.105,
  "status": "Open",
  "createdAt": "2026-01-17T12:00:00Z"
}
```

### Cancel Order

```http
DELETE /api/trading/orders/{orderId}?userId={userId}
```

### Get Open Orders

```http
GET /api/trading/orders/open/{userId}
```

### Get Order History

```http
GET /api/trading/orders/history/{userId}?page=1&pageSize=50
```

### Get Trade History

```http
GET /api/trading/trades/{userId}?page=1&pageSize=50
```

**Response:**
```json
{
  "data": [
    {
      "id": "uuid",
      "buyOrderId": "uuid",
      "sellOrderId": "uuid",
      "price": 1.05,
      "amount": 50,
      "buyerFee": 0.0525,
      "sellerFee": 0.0525,
      "executedAt": "2026-01-17T12:00:00Z"
    }
  ],
  "page": 1,
  "pageSize": 50,
  "total": 150
}
```

---

# WALLET API

**Base URL:** `http://localhost:5054/api/wallet`

## Get Wallets

```http
GET /api/wallet/user/{userId}
```

**Response:**
```json
{
  "wallets": [
    {
      "id": "uuid",
      "assetId": "uuid",
      "assetSymbol": "IGT-PM",
      "availableBalance": 10000,
      "lockedBalance": 500,
      "totalBalance": 10500,
      "depositAddress": "0x1234...abcd",
      "isActive": true
    }
  ]
}
```

## Get Balance

```http
GET /api/wallet/balance/{userId}/{assetId}
```

## Deposit

```http
POST /api/wallet/deposit
```

**Request Body:**
```json
{
  "userId": "uuid",
  "assetId": "uuid",
  "amount": 1000,
  "txHash": "0xabc123..."
}
```

## Withdraw

```http
POST /api/wallet/withdraw
```

**Request Body:**
```json
{
  "userId": "uuid",
  "assetId": "uuid",
  "amount": 500,
  "toAddress": "0xdef456..."
}
```

**Response:**
```json
{
  "id": "uuid",
  "type": "Withdrawal",
  "amount": 500,
  "fee": 0.5,
  "netAmount": 499.5,
  "status": "Processing",
  "isVipPriority": true,
  "vipFeeDiscountPercent": 25,
  "createdAt": "2026-01-17T12:00:00Z"
}
```

## Transfer (Internal)

```http
POST /api/wallet/transfer
```

**Request Body:**
```json
{
  "fromUserId": "uuid",
  "toUserId": "uuid",
  "assetId": "uuid",
  "amount": 100
}
```

## Transaction History

```http
GET /api/wallet/transactions/{userId}?page=1&pageSize=50&vipOnly=false
```

---

# STAKING API

**Base URL:** `http://localhost:5054/api/staking`

## Get Staking Pools

```http
GET /api/staking/pools
```

**Response:**
```json
{
  "pools": [
    {
      "id": "uuid",
      "name": "IGT Flexible Staking",
      "description": "Stake IGT tokens with flexible withdrawal",
      "apy": 8.0,
      "lockPeriodDays": 30,
      "minStakeAmount": 100,
      "maxStakeAmount": 1000000,
      "totalPoolSize": 100000000,
      "currentStaked": 25000000,
      "availableCapacity": 75000000,
      "earlyWithdrawalAllowed": true,
      "earlyWithdrawalPenalty": 0.05,
      "isActive": true
    },
    {
      "id": "uuid",
      "name": "IGT 90-Day Lock",
      "apy": 15.0,
      "lockPeriodDays": 90
    },
    {
      "id": "uuid",
      "name": "IGT Annual Premium",
      "apy": 25.0,
      "lockPeriodDays": 365
    },
    {
      "id": "uuid",
      "name": "BDET Bank Savings",
      "apy": 12.0,
      "lockPeriodDays": 180
    }
  ]
}
```

## Stake Tokens

```http
POST /api/staking/stake
```

**Request Body:**
```json
{
  "userId": "uuid",
  "poolId": "uuid",
  "amount": 5000
}
```

**Response:**
```json
{
  "id": "uuid",
  "userId": "uuid",
  "stakingPoolId": "uuid",
  "amount": 5000,
  "estimatedReward": 102.74,
  "earnedReward": 0,
  "claimedReward": 0,
  "stakedAt": "2026-01-17T12:00:00Z",
  "unlockDate": "2026-02-16T12:00:00Z",
  "status": "Active"
}
```

## Unstake

```http
POST /api/staking/unstake
```

**Request Body:**
```json
{
  "stakeId": "uuid",
  "userId": "uuid",
  "early": false
}
```

## Claim Rewards

```http
POST /api/staking/claim
```

**Request Body:**
```json
{
  "stakeId": "uuid",
  "userId": "uuid"
}
```

## Get User Stakes

```http
GET /api/staking/user/{userId}
```

---

# NODE API

**Base URL:** `http://localhost:5054/api/node`

## Get Node Info

```http
GET /api/node/info
```

**Response:**
```json
{
  "name": "Ierahkwa Futurehead Mamey Node",
  "version": "1.0.0",
  "networkId": "ierahkwa-mainnet",
  "chainId": 777777,
  "currency": "IGT",
  "blockHeight": 1234567,
  "peers": 12,
  "uptime": "99.999%",
  "status": "Healthy"
}
```

## Get Block

```http
GET /api/node/block/{blockNumber}
```

## Get Transaction

```http
GET /api/node/tx/{txHash}
```

## Get Token Info

```http
GET /api/node/token/{symbol}
```

**Response:**
```json
{
  "symbol": "IGT-PM",
  "name": "Office of the Prime Minister",
  "decimals": 9,
  "totalSupply": "10000000000000",
  "contractAddress": "0x...",
  "type": "Government Token"
}
```

---

# HEALTH CHECK

Todos los servicios exponen un endpoint de health check:

```http
GET /health
```

**Response:**
```json
{
  "status": "healthy",
  "service": "Ierahkwa TradeX Exchange",
  "version": "1.0.0",
  "node": "Ierahkwa Futurehead Mamey Node",
  "timestamp": "2026-01-17T12:00:00Z"
}
```

---

# CÓDIGOS DE ERROR

| Código | Nombre | Descripción |
|--------|--------|-------------|
| 200 | OK | Solicitud exitosa |
| 201 | Created | Recurso creado |
| 400 | Bad Request | Parámetros inválidos |
| 401 | Unauthorized | Token inválido o expirado |
| 403 | Forbidden | Sin permisos |
| 404 | Not Found | Recurso no encontrado |
| 409 | Conflict | Conflicto de estado |
| 422 | Unprocessable | Validación fallida |
| 429 | Too Many Requests | Rate limit excedido |
| 500 | Internal Error | Error del servidor |

## Formato de Error

```json
{
  "error": {
    "code": "INSUFFICIENT_BALANCE",
    "message": "Saldo insuficiente. Disponible: 100, necesario: 500",
    "details": {
      "available": 100,
      "required": 500
    }
  },
  "timestamp": "2026-01-17T12:00:00Z",
  "requestId": "uuid"
}
```

---

# VIP LEVELS

El sistema TradeX tiene niveles VIP con descuentos en comisiones:

| Level | Descuento Fee | Requisito |
|-------|---------------|-----------|
| None | 0% | - |
| Bronze | 5% | Hold 1,000 IGT |
| Silver | 10% | Hold 10,000 IGT |
| Gold | 15% | Hold 50,000 IGT |
| Platinum | 20% | Hold 100,000 IGT |
| Diamond | 25% | Hold 500,000 IGT |
| Royal | 30% | Hold 1,000,000 IGT |
| Sovereign | 50% | Government Official |

---

# WEBSOCKET API

## Conexión

```javascript
const ws = new WebSocket('wss://localhost:5054/ws');

ws.onopen = () => {
  // Subscribe to order book
  ws.send(JSON.stringify({
    type: 'subscribe',
    channel: 'orderbook',
    symbol: 'IGT-PM/USDT'
  }));
};

ws.onmessage = (event) => {
  const data = JSON.parse(event.data);
  console.log(data);
};
```

## Channels

| Channel | Descripción |
|---------|-------------|
| `orderbook` | Actualizaciones del libro de órdenes |
| `trades` | Trades ejecutados en tiempo real |
| `ticker` | Precio y stats por par |
| `kline` | Candlesticks (1m, 5m, 15m, 1h, 4h, 1d) |
| `user.orders` | Órdenes del usuario (autenticado) |
| `user.trades` | Trades del usuario (autenticado) |

---

# EJEMPLOS DE CÓDIGO

## JavaScript/Node.js

```javascript
const axios = require('axios');

const api = axios.create({
  baseURL: 'http://localhost:5054/api',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer YOUR_TOKEN'
  }
});

// Get trading pairs
const pairs = await api.get('/trading/pairs');

// Place order
const order = await api.post('/trading/orders', {
  userId: 'your-user-id',
  tradingPairId: 'pair-id',
  side: 'Buy',
  type: 'Limit',
  amount: 100,
  price: 1.05
});

// Stake tokens
const stake = await api.post('/staking/stake', {
  userId: 'your-user-id',
  poolId: 'pool-id',
  amount: 5000
});
```

## C# / .NET

```csharp
using System.Net.Http.Json;

var client = new HttpClient { 
    BaseAddress = new Uri("http://localhost:5054/api/") 
};
client.DefaultRequestHeaders.Add("Authorization", "Bearer YOUR_TOKEN");

// Get trading pairs
var pairs = await client.GetFromJsonAsync<List<TradingPair>>("trading/pairs");

// Place order
var order = await client.PostAsJsonAsync("trading/orders", new {
    UserId = Guid.Parse("your-user-id"),
    TradingPairId = Guid.Parse("pair-id"),
    Side = "Buy",
    Type = "Limit",
    Amount = 100m,
    Price = 1.05m
});
```

## Python

```python
import requests

api_url = "http://localhost:5054/api"
headers = {
    "Content-Type": "application/json",
    "Authorization": "Bearer YOUR_TOKEN"
}

# Get trading pairs
pairs = requests.get(f"{api_url}/trading/pairs", headers=headers).json()

# Place order
order = requests.post(f"{api_url}/trading/orders", headers=headers, json={
    "userId": "your-user-id",
    "tradingPairId": "pair-id",
    "side": "Buy",
    "type": "Limit",
    "amount": 100,
    "price": 1.05
}).json()
```

---

```
══════════════════════════════════════════════════════════════════════════════
                    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
                          OFFICE OF THE PRIME MINISTER
                              
                    API REFERENCE v1.0 | 17 de Enero de 2026
══════════════════════════════════════════════════════════════════════════════
```
