# Referencia: Inversiones tipo AI / HYIP (planes, ROI, suscripción)

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO  
Referencia de funcionalidades de addons comerciales (ej. AI Investments for Bicrypto) y cómo se cubren con el **módulo soberano de inversiones** sin depender de terceros.

---

## 1. Funciones de referencia (addon comercial)

| Función | Descripción | En Ierahkwa |
|---------|-------------|-------------|
| **Planes de inversión** | Porcentaje de beneficio, min/max, resultado WIN/LOSS/DRAW | `sovereign-investments`: planes con profitMin, profitMax, profitDefault, minAmount, maxAmount |
| **Duraciones** | Horaria, diaria, semanal, mensual; relación N:M con planes | `durations` con type (hourly/daily/weekly/monthly), value, asociados a planIds |
| **Wallet** | Balance, deducción al crear inversión, crédito al completar ROI | Integración con cuentas BDET / tokens (SPOT/ECO); módulo usa saldo y movimientos propios |
| **ROI automático** | Cron que procesa inversiones vencidas y reparte ganancia | Job en `sovereign-investments` que corre periódicamente y actualiza estado + balance |
| **Estados** | ACTIVE → COMPLETED / CANCELLED / REJECTED | `status` en cada inversión; auditoría en log de transacciones |
| **Resultado** | WIN / LOSS / DRAW por inversión | `result: 'WIN'|'LOSS'|'DRAW'`; lógica propia (ej. aleatorio controlado o reglas soberanas) |
| **Admin** | CRUD planes, duraciones, listado/filtro de inversiones | APIs: `/api/v1/investments/plans`, `/durations`, `/investments`; panel en `platform/investments.html` |
| **Analíticas** | KPIs, gráficos por estado, series temporales | Endpoints `/api/v1/investments/analytics`, `/api/v1/investments/stats` |
| **Notificaciones** | Email e in-app al crear y al completar | Servicios propios: `email-soberano`, notificaciones in-app (sin SendGrid ni terceros) |
| **Multi-moneda** | BTC, ETH, USDT, BUSD, USD, etc. | Soporte por plan/currency; formato por decimales (BTC 8, ETH 6, USDT 2) |
| **Permisos** | Ver/crear/editar/borrar planes, duraciones, inversiones | Middleware JWT + roles; permisos granulares en admin |

---

## 2. Módulo soberano implementado

- **Código:** `RuddieSolution/node/modules/sovereign-investments.js`  
- **Datos:** `RuddieSolution/node/data/sovereign-investments/` (plans.json, durations.json, investments.json)  
- **API:** `GET/POST /api/v1/investments/plans`, `GET/POST /api/v1/investments/durations`, `GET/POST /api/v1/investments`, `POST /api/v1/investments/process-completed` (cron)  
- **UI:** `RuddieSolution/platform/investments.html` (panel usuario + enlace a admin)  
- **Cron:** Procesamiento de inversiones vencidas (ROI, resultado WIN/LOSS/DRAW, actualización de wallet/balance).

Sin Bicrypto, sin CodeCanyon, sin APIs externas. Principio: `PRINCIPIO-TODO-PROPIO.md`.

---

## 3. Enlaces

- Índice: `docs/INDEX-DOCUMENTACION.md`  
- BDET / banca: `RuddieSolution/node/banking-bridge.js`, `platform/bdet-bank.html`  
- Recompensas / staking: `node/modules/sovereign-reward-engine.js`
