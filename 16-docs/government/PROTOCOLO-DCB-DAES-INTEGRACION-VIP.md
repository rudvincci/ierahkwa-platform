# Protocolo de ejecución — Integración DCB / DAES en VIP

**Sovereign Government of Ierahkwa Ne Kanienke**  
Digital Commercial Bank Ltd (DCB) / DAES Partner API — Fund Transfer Webhook (CashTransfer.v1)

---

## 1. Objetivo

Recibir en la plataforma soberana las notificaciones de **Fund Transfer** (CashTransfer.v1) enviadas por DCB/DAES vía webhook, registrarlas como transacciones VIP y dejar lista la integración para producción según el checklist DCB_DAES.

---

## 2. Referencias

- **DCB - API INTEGRATION INSTRUCTION – FUND TRANSFER WEBHOOK1.pdf** — Formato del payload y endpoint de recepción.
- **DCB - API PROPOSAL LETTER.pdf** — Contexto comercial/legal.
- **DAES_Partner_API_Integration_https___zen.site__...** — Documentación partner DAES (zen.site).
- **DCB_DAES_Checklist_Banking_API_Integration_EN_2025-12-21-1.pdf** — Checklist oficial de integración (producción, sandbox, auth, transfers, webhooks).

---

## 3. Requisitos previos

| Requisito | Descripción |
|-----------|-------------|
| **HTTPS** | El endpoint de webhook debe ser accesible por HTTPS (obligatorio en producción). |
| **URL pública** | URL base del nodo (ej. `https://tu-dominio.com`) para registrar en DAES. |
| **Cuentas** | Credenciales DAES/DCB (OAuth 2.0 / API Key+Secret / mTLS según checklist). |
| **Módulo VIP** | Módulo **DCB/DAES - Fund Transfer Webhook** ya añadido en `RuddieSolution/platform/vip-modules.json` (id: `dcb-daes`). |

---

## 4. Endpoint de recepción del webhook

- **Método:** `POST`
- **URL:**  
  `https://<TU_BASE_URL>/api/v1/webhooks/dcb/cash-transfer`
- **Content-Type:** `application/json`
- **Payload:** Objeto **CashTransfer.v1** (campos en camelCase o PascalCase admitidos).

### Campos mínimos requeridos

- `TransferRequestID` (o `transferRequestID`) — Identificador único de la solicitud de transferencia.
- `Amount` (o `amount`) — Monto numérico.

### Campos opcionales (recomendados)

- `SendingName`, `SendingAccount`, `SendingInstitution`, `SendingCurrency`
- `ReceivingName`, `ReceivingAccount`, `ReceivingInstitution`, `ReceivingCurrency`
- `Datetime` (ISO 8601)
- `Description`, `method`, `purpose`, `source`

### Respuesta esperada

- **200 OK:** `{ "success": true, "received": true, "transferRequestID": "...", "WEBHOOK_ACK": true, "transactionId": "VIP-DCB-..." }`
- **400:** Falta `TransferRequestID` o `Amount` válido — `WEBHOOK_ACK: false`
- **500:** Error de lectura/escritura VIP — `WEBHOOK_ACK: false`

---

## 5. Registro del webhook en DAES/DCB

Según el checklist DCB_DAES:

1. Acceder al **Partner API** de DAES (producción: `https://luxliqdaes.cloud/partner-api/v1`; sandbox: `https://luxliqdaes.cloud/partner-api/sandbox/v1`).
2. Autenticarse según el método acordado (OAuth 2.0, API Key+Secret o mTLS).
3. Registrar la URL del webhook con **POST /webhooks/register** (o el endpoint que indique DCB/DAES), usando:
   - **URL:** `https://<TU_BASE_URL>/api/v1/webhooks/dcb/cash-transfer`
   - Evento/tipo: Fund Transfer / CashTransfer.v1 (según documentación del partner).

Conservar el identificador del webhook y el secret (si aplica) para HMAC/firma en futuras mejoras.

---

## 6. Dónde queda registrada la transacción en VIP

- **Persistencia:** Cada webhook recibido se escribe en  
  `RuddieSolution/node/data/vip-transactions.json`  
  como una transacción VIP con:
  - `id`: `VIP-DCB-<TransferRequestID>`
  - `type`: `international`
  - `data.moduleId`: `dcb-daes`
  - `data.SERVER_CONFIRMATION`: `WEBHOOK_RECEIVED`
  - `data.WEBHOOK_ACK`: `true`
- **Interfaz:** Las transacciones se listan en la página **VIP Transactions** (`/vip-transactions` o `/vip`). El listado recarga desde archivo en cada petición, por lo que las entradas añadidas por el webhook DCB se ven de inmediato al refrescar la página.

---

## 7. Cómo probar (sandbox)

### 7.1 Con curl (ejemplo)

```bash
curl -X POST "https://<TU_BASE_URL>/api/v1/webhooks/dcb/cash-transfer" \
  -H "Content-Type: application/json" \
  -d '{
    "TransferRequestID": "TEST-REQ-001",
    "Amount": 10000.50,
    "SendingInstitution": "DCB",
    "ReceivingInstitution": "Sovereign Platform",
    "SendingName": "Sender Ltd",
    "ReceivingName": "Ierahkwa Sovereign",
    "Datetime": "2026-02-02T12:00:00Z",
    "SendingCurrency": "USD",
    "ReceivingCurrency": "USD",
    "Description": "Sandbox test CashTransfer.v1"
  }'
```

Respuesta esperada: `200` y cuerpo con `WEBHOOK_ACK: true` y `transactionId: "VIP-DCB-TEST-REQ-001"`.

### 7.2 Sandbox DAES

- Base: `https://luxliqdaes.cloud/partner-api/sandbox/v1`
- Realizar **POST /auth/token** (o el flujo indicado) y luego **POST /transfers** si se simula el envío desde DAES; configurar en DAES la URL de webhook apuntando a nuestro endpoint para recibir notificaciones.

---

## 8. Checklist de ejecución (resumen)

- [ ] Servidor con HTTPS y URL base definida.
- [ ] Módulo VIP `dcb-daes` presente en `vip-modules.json`.
- [ ] Endpoint `POST /api/v1/webhooks/dcb/cash-transfer` desplegado y accesible.
- [ ] Prueba con curl (o Postman) con payload CashTransfer.v1 mínima (TransferRequestID + Amount).
- [ ] Registro del webhook en DAES/DCB con la URL anterior.
- [ ] Verificación de que la transacción aparece en `node/data/vip-transactions.json` y, tras reinicio si aplica, en la UI VIP.
- [ ] Documentación interna actualizada con la URL de producción y contactos DCB/DAES.

---

## 9. Archivos modificados / creados en la plataforma

| Archivo | Cambio |
|---------|--------|
| `RuddieSolution/platform/vip-modules.json` | Nuevo módulo `dcb-daes` (DCB/DAES - Fund Transfer Webhook). |
| `RuddieSolution/node/routes/webhooks-api.js` | Ruta `POST /dcb/cash-transfer` que valida CashTransfer.v1 y persiste en VIP. |
| `RuddieSolution/node/data/vip-transactions.json` | Actualizado automáticamente al recibir cada webhook. |
| `docs/PROTOCOLO-DCB-DAES-INTEGRACION-VIP.md` | Este protocolo. |

---

*Documento generado para la integración DCB/DAES en VIP. Sovereign Government of Ierahkwa Ne Kanienke.*
