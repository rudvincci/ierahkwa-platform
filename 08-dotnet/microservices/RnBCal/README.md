# IERAHKWA RnBCal — Rental & Booking Calendar Sync System

Sistema de sincronización de calendarios para reservas y alquileres en **.NET 10**, integrado en la plataforma IERAHKWA.

## 🌟 Características

### v1.0.4 (Octubre 2025)

#### Nuevas Características
- ✅ Enlaces directos de calendario para:
  - Google Calendar
  - Yahoo Calendar
  - Outlook Calendar
  - Office 365 Calendar
  - Apple Calendar
  - AOL Calendar
- ✅ Enlaces de calendario integrados en el contenido del correo electrónico
- ✅ Sincronización automática con Google Calendar (OAuth 2.0)
- ✅ Generación de archivos ICS compatibles con RFC 5545
- ✅ API REST completa para gestión de reservas
- ✅ Integración con tokens IGT de IERAHKWA

#### Mejoras
- 📝 Estructura de archivos ICS refinada para mejor compatibilidad
- 📧 Opciones mejoradas de envío de correo electrónico
- ⚙️ Panel de configuración actualizado
- 🎨 Estilo CSS mejorado para consistencia de UI
- 🌍 Archivo POT reescrito para mejor soporte de localización

## 🚀 Tipos de Reserva Soportados

- 🚗 **Alquiler de Autos** (Car Rental)
- 🚴 **Alquiler de Bicicletas** (Bike Rental)
- 🛥️ **Alquiler de Yates** (Yacht Rental)
- 🏨 **Habitaciones de Hotel** (Hotel Room)
- 🏠 **Propiedades Airbnb** (Airbnb Property)
- 🔧 **Alquiler de Equipos** (Equipment Rental)
- 👗 **Alquiler de Vestidos** (Dress Rental)

## 🏗️ Arquitectura

```
RnBCal/
├── RnBCal.API/          # ASP.NET Core Web API
│   ├── Controllers/      # Calendar & Bookings controllers
│   ├── wwwroot/         # Frontend UI
│   └── Program.cs       # Application entry point
├── RnBCal.Core/         # Domain models & interfaces
│   ├── Models/          # Booking, CalendarEvent, etc.
│   └── Interfaces/      # ICalendarService, IEmailService
└── RnBCal.Infrastructure/ # Service implementations
    └── Services/        # CalendarService, EmailService, GoogleCalendarService
```

## 🔧 Configuración

### appsettings.json

```json
{
  "Email": {
    "Enabled": false,
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "GoogleCalendar": {
    "Enabled": false,
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  }
}
```

## 🚀 Inicio Rápido

### Requisitos
- .NET 10 SDK
- Visual Studio 2024 o VS Code

### Ejecutar el Proyecto

```bash
cd RnBCal/RnBCal.API
dotnet run
```

El servicio estará disponible en:
- HTTP: http://localhost:5055
- HTTPS: https://localhost:7055
- Swagger UI: http://localhost:5055/swagger

## 📡 API Endpoints

### Calendar Sync

```
POST /api/calendar/generate-ics          # Generar archivo ICS
POST /api/calendar/calendar-links        # Obtener enlaces directos
POST /api/calendar/sync                  # Sincronización completa
POST /api/calendar/send-confirmation     # Enviar email de confirmación
GET  /api/calendar/google/oauth-url      # URL de autorización OAuth
```

### Bookings Management

```
GET    /api/bookings            # Listar todas las reservas
POST   /api/bookings            # Crear nueva reserva
GET    /api/bookings/{id}       # Obtener reserva por ID
PUT    /api/bookings/{id}       # Actualizar reserva
DELETE /api/bookings/{id}       # Eliminar reserva
GET    /api/bookings/stats      # Estadísticas de reservas
```

## 📧 Integración de Email

El sistema puede enviar correos de confirmación con archivos ICS adjuntos y enlaces directos a calendarios.

**Ejemplo de email:**
- Detalles de la reserva
- Botones para añadir a calendarios (Google, Outlook, Yahoo, etc.)
- Archivo .ics adjunto
- Diseño responsive con branding IERAHKWA

## 🔐 Google Calendar Auto-Sync

Para habilitar la sincronización automática con Google Calendar:

1. Crear proyecto en Google Cloud Console
2. Habilitar Google Calendar API
3. Configurar OAuth 2.0 credentials
4. Actualizar `appsettings.json` con ClientId y ClientSecret
5. Obtener tokens de acceso mediante el flujo OAuth

## 🌐 Integración en IERAHKWA Platform

En **IERAHKWA_PLATFORM_V1.html**, en la sección **BUSINESS & COMMERCE**, la tarjeta **RnBCal** abre http://localhost:5055 cuando el servidor está en marcha.

```html
<div class="service-card" onclick="openService('rnbcal')">
    <i class="bi bi-calendar-check"></i>
    <div class="card-title">RnBCal Sync</div>
    <div class="card-desc">Calendar Booking Sync</div>
</div>
```

## 🎯 Casos de Uso

1. **Alquiler de Vehículos**: Sincronizar reservas de autos/motos con calendarios
2. **Hoteles & Airbnb**: Gestionar disponibilidad de habitaciones
3. **Eventos**: Coordinar reservas de espacios y equipos
4. **Servicios**: Programar citas y reservas de servicios

## 🔗 Enlaces Útiles

- **API Documentation**: http://localhost:5055/swagger
- **Health Check**: http://localhost:5055/health
- **IERAHKWA Platform**: http://localhost/IERAHKWA_PLATFORM_V1.html

## 📝 Ejemplo de Uso (cURL)

### Crear una reserva

```bash
curl -X POST http://localhost:5055/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Alquiler de Auto Deportivo",
    "customerName": "Juan Pérez",
    "customerEmail": "juan@example.com",
    "itemName": "Ferrari F8 Tributo",
    "itemType": "Car",
    "type": "CarRental",
    "startDate": "2026-02-01T10:00:00Z",
    "endDate": "2026-02-05T10:00:00Z",
    "location": "IERAHKWA Rental Center",
    "totalAmount": 5000,
    "currency": "USD"
  }'
```

### Sincronizar con calendarios

```bash
curl -X POST http://localhost:5055/api/calendar/sync \
  -H "Content-Type: application/json" \
  -d @booking.json
```

## 🏛️ Tecnologías

- **.NET 10** - Framework principal
- **ASP.NET Core** - Web API
- **Swagger/OpenAPI** - Documentación de API
- **System.Net.Mail** - Envío de emails
- **Google Calendar API** - Auto-sync
- **ICS (RFC 5545)** - Formato de calendario

## 📄 Licencia

© 2026 **Sovereign Government of Ierahkwa Ne Kanienke**  
Todos los derechos reservados.

---

**IERAHKWA RnBCal** • .NET 10 • Integrado en IERAHKWA Platform
