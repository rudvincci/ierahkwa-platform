# IERAHKWA Appy – AI-Powered No-Code Mobile App Builder

**.NET 10** · Estructura IERAHKWA · Sovereign Government of Ierahkwa Ne Kanienke

SaaS tipo Appy: crear apps nativas desde una URL en minutos, planes Free/Pro/Enterprise, PayPal y transferencia bancaria, créditos de build, asistentes IA, push, GDPR, vista previa en navegador, QR, plugins Android (incluido) y WordPress (de pago).

---

## Estructura de la solución

```
AppBuilder/
├── IerahkwaAppBuilder.sln
├── AppBuilder.API/          # Web API, JWT, Swagger, wwwroot (login, app builder, admin)
├── AppBuilder.Core/         # Models, Interfaces (Auth, Subscription, Payment, Invoice, Push, AI, Plugin, AppBuilder)
└── AppBuilder.Infrastructure/  # Services: AuthService, SubscriptionService, PaymentService, InvoiceService,
                                # PushNotificationService, AiAssistantService, PluginService, AppBuilderService
```

---

## Características implementadas

| Funcionalidad | Detalle |
|---------------|---------|
| **No-code desde URL** | `POST /api/projects/from-url`, `POST /api/projects` |
| **Vista previa en navegador** | `GET /api/preview/{projectId}` |
| **Builds** | `POST /api/builds`, créditos por usuario, `GET /api/builds/{id}/qr` |
| **Planes** | Free (5 builds/mes), Pro (50), Enterprise (ilimitado) |
| **Pagos** | PayPal (mock redirect + callback), Bank Transfer (factura) |
| **Facturas** | `GET /api/invoices`, `GET /api/invoices/{id}/html` |
| **Push** | `POST /api/pushnotifications`, programables, con imagen |
| **IA** | `POST /api/ai/chat` (OpenAI/Anthropic o mock) |
| **Plugins** | `GET /api/plugins/platforms` (Android, WordPress) |
| **Auth** | JWT, `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/social` |
| **GDPR** | `GET /api/users/export-data`, `DELETE /api/users/delete-account` |
| **Admin** | `GET /api/admin/stats`, `GET /api/admin/builds`, `GET /api/admin/notifications` |

---

## Cómo ejecutar

```bash
cd AppBuilder
dotnet build
dotnet run --project AppBuilder.API
```

- **App:** http://localhost:5060  
- **Swagger:** http://localhost:5060/swagger  
- **Login:** http://localhost:5060/login.html  
- **Admin:** http://localhost:5060/admin/  

Puerto por defecto en `AppBuilder.API/Properties/launchSettings.json` (p. ej. 5060).

---

## Configuración (`appsettings.json`)

- **Jwt:** `SecretKey`, `Issuer`, `Audience`, `ExpirationMinutes`
- **Ai:** `Provider` (OpenAI|Anthropic), `OpenAI:ApiKey`, `Anthropic:ApiKey`
- **PayPal:** `ClientId`, `ClientSecret`, `Mode`
- **BankTransfer:** `AccountName`, `IBAN`, `Reference`

---

## Token IGT

Servicio registrado en `platform-services.json` como **IGT-APP** (appy.ierahkwa.gov, puerto local 5060).

---

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister · © 2026**
