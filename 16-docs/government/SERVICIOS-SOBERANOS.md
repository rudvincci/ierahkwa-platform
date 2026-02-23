# Servicios Soberanos - TODO PROPIO

Índice de módulos que reemplazan dependencias de 3ra compañía (y de proveedores regulados por otros gobiernos).

## Módulos de aplicación

| Servicio | Archivo | Reemplaza |
|----------|---------|-----------|
| Email | `services/email-soberano.js` | SendGrid |
| Storage | `services/storage-soberano.js` | AWS S3 |
| Pagos | `services/pagos-soberano.js` | Stripe |
| AI | `services/ai-soberano.js` | OpenAI, Anthropic |
| SMS | `services/sms-soberano.js` | Twilio |
| **Workforce** | `services/workforce-soberano.js` | Slack, Jira, Monday, Gusto, ADP |

## Red y seguridad (firewall, routing)

Para no depender de Fortinet ni Cisco (regulados por otros gobiernos):

| Equipo actual | Reemplazo soberano | Dónde |
|---------------|--------------------|-------|
| FortiGate 90D/100D | **OPNsense** o pfSense (BSD) | `sovereign-network/opnsense/` |
| Cisco IOS (routing) | **VyOS** o Linux + FRR | `sovereign-network/vyos/` |
| Firewall en servidor | **nftables** (Linux) | `sovereign-network/linux-nftables/` |

- Estrategia completa: **`docs/SOBERANO-SOFTWARE-RED-Y-SEGURIDAD.md`**
- Instalación stack en servidor Linux (Node, PM2, firewall opcional): **`./scripts/instalar-stack-soberano-servidor.sh`**
- Inventario físico (Cisco, HPE, FortiGate): **`docs/INVENTARIO-RACK-CISCO-HPE-FORTINET.md`**
- Cisco + SWIFT propio: **`docs/CISCO-SWIFT-SOBERANO.md`**

## Uso

```javascript
const { email, storage, pagos, ai, sms, workforce } = require('./services/index-soberano');

await email.send('user@mail.com', 'Asunto', '<p>HTML</p>');
const file = await storage.upload(buffer, { bucket: 'documents' });
const intent = await pagos.createPaymentIntent({ amount: 100, currency: 'IGT' });
const { content } = await ai.chat([{ role: 'user', content: 'Hola' }]);
await sms.send('+1234567890', 'Mensaje');
// Workforce: trabajadores, tareas, nómina BDET
await workforce.createWorker({ name: 'Juan', role: 'worker', department: 'tech' });
await workforce.createTask({ title: 'Reporte Q1', assignedTo: 'WRK-xxx' });
await workforce.runPayroll();
```

## Configuración

- **Email**: Cola en `data/email-queue/`. Procesar con cron + sendmail local.
- **Storage**: Archivos en `data/storage-soberano/`
- **AI**: Ollama en `http://localhost:11434` (`ollama run llama2`)
- **Workforce**: API `/api/v1/workforce` · UI `/platform/workforce-soberano.html` · Nómina vía BDET

## Activar modo soberano

```bash
USE_SOBERANO=true node server.js
```

## Cola email

Los emails se guardan en `data/email-queue/`. Procesar con:

```bash
node RuddieSolution/node/scripts/procesar-cola-email.js
```

Orden de envío: 1) sendmail, 2) SMTP directo (localhost:25), 3) guardar .eml en `data/email-pending-smtp/`.  
Variables: `SMTP_HOST`, `SMTP_PORT`.

Cron cada 5 min: `*/5 * * * * cd /ruta/proyecto && node RuddieSolution/node/scripts/procesar-cola-email.js`

## Cola SMS

```bash
node RuddieSolution/node/scripts/procesar-cola-sms.js
```

Envía vía Telecom API (`/api/v1/telecom/sms/send`). Cron: `*/5 * * * *`

## Ollama (AI local)

```bash
./scripts/instalar-ollama.sh
ollama run llama2
```

## Auditoría

```bash
node scripts/auditar-soberania.js
```

## Procesar colas (unificado)

```bash
./scripts/procesar-colas.sh   # email + SMS
```

## Estado soberano

```bash
./scripts/status-soberano.sh  # colas, Ollama, storage, backups
```

## Cron automático (colas cada 5 min)

```bash
./scripts/instalar-cron-colas.sh
```

## Dashboard estado soberano

`/platform/sovereign-status.html` — muestra servicios, Ollama, Telecom.

## Backup propio

```bash
./scripts/backup-soberano.sh
```
