# RnBCal - Guía de Uso Completa

## 🚀 Inicio Rápido

### 1. Ejecutar el Servidor

```bash
cd RnBCal/RnBCal.API
dotnet run
```

El servidor estará disponible en:
- **Frontend**: http://localhost:5055
- **API Swagger**: http://localhost:5055/swagger
- **Health Check**: http://localhost:5055/health

### 2. Acceso desde IERAHKWA Platform

Abre `IERAHKWA_PLATFORM_V1.html` y haz clic en la tarjeta **RnBCal Sync** en la sección "COMMERCE & BUSINESS".

## 📋 Casos de Uso Prácticos

### Caso 1: Alquiler de Vehículos

#### Crear una Reserva de Auto

```bash
curl -X POST http://localhost:5055/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "María González",
    "customerEmail": "maria@example.com",
    "customerPhone": "+1-555-0123",
    "itemName": "Tesla Model S",
    "itemType": "Car",
    "type": "CarRental",
    "startDate": "2026-03-15T09:00:00Z",
    "endDate": "2026-03-20T17:00:00Z",
    "location": "IERAHKWA Rental Center, 123 Main St",
    "totalAmount": 1500.00,
    "currency": "USD",
    "description": "Alquiler de vehículo eléctrico premium",
    "status": "Confirmed"
  }'
```

#### Sincronizar con Calendarios

```bash
# Obtener el ID de la reserva creada (ejemplo: abc-123)
BOOKING_ID="abc-123"

# Sincronizar automáticamente
curl -X POST http://localhost:5055/api/calendar/sync \
  -H "Content-Type: application/json" \
  -d @- <<EOF
{
  "id": "$BOOKING_ID",
  "customerName": "María González",
  "customerEmail": "maria@example.com",
  "itemName": "Tesla Model S",
  "itemType": "Car",
  "type": "CarRental",
  "startDate": "2026-03-15T09:00:00Z",
  "endDate": "2026-03-20T17:00:00Z",
  "location": "IERAHKWA Rental Center",
  "totalAmount": 1500.00,
  "currency": "USD"
}
EOF
```

**Respuesta:**
```json
{
  "success": true,
  "message": "Calendar sync completed successfully",
  "icsFileContent": "BEGIN:VCALENDAR\nVERSION:2.0...",
  "icsFileName": "booking-abc-123.ics",
  "calendarLinks": {
    "Google": {
      "provider": "Google Calendar",
      "directLink": "https://www.google.com/calendar/render?action=TEMPLATE&text=...",
      "displayName": "Add to Google Calendar"
    },
    "Outlook": { ... },
    "Yahoo": { ... }
  }
}
```

### Caso 2: Reserva de Hotel

```javascript
// Ejemplo usando JavaScript/Fetch API
const booking = {
  customerName: "John Smith",
  customerEmail: "john@example.com",
  customerPhone: "+1-555-0456",
  itemName: "Presidential Suite - Ocean View",
  itemType: "Hotel",
  type: "HotelRoom",
  startDate: "2026-06-01T15:00:00Z",
  endDate: "2026-06-07T11:00:00Z",
  location: "IERAHKWA Grand Hotel, Akwesasne",
  totalAmount: 3500.00,
  currency: "USD",
  description: "Habitación presidencial con vista al océano, incluye desayuno",
  status: "Confirmed",
  notes: "Cliente VIP, solicita late checkout"
};

// Crear reserva
const response = await fetch('http://localhost:5055/api/bookings', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(booking)
});

const createdBooking = await response.json();

// Sincronizar con calendarios
const syncResponse = await fetch('http://localhost:5055/api/calendar/sync', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(createdBooking)
});

const syncResult = await syncResponse.json();
console.log('Calendar Links:', syncResult.calendarLinks);
```

### Caso 3: Alquiler de Yate

```python
import requests
import json
from datetime import datetime, timedelta

# Configuración
API_URL = "http://localhost:5055/api"

# Datos de la reserva
booking_data = {
    "customerName": "Roberto Martínez",
    "customerEmail": "roberto@example.com",
    "customerPhone": "+1-555-0789",
    "itemName": "Luxury Yacht 'Sea Dream' - 60ft",
    "itemType": "Yacht",
    "type": "YachtRental",
    "startDate": (datetime.now() + timedelta(days=30)).isoformat() + "Z",
    "endDate": (datetime.now() + timedelta(days=37)).isoformat() + "Z",
    "location": "IERAHKWA Marina, Dock 5",
    "totalAmount": 25000.00,
    "currency": "USD",
    "description": "Yate de lujo para 10 personas, incluye tripulación y catering",
    "status": "Confirmed",
    "customFields": {
        "passengers": "8",
        "captain": "Yes",
        "catering": "Premium"
    }
}

# Crear reserva
response = requests.post(f"{API_URL}/bookings", json=booking_data)
booking = response.json()
print(f"✅ Reserva creada: {booking['id']}")

# Sincronizar con calendarios
sync_response = requests.post(f"{API_URL}/calendar/sync", json=booking)
sync_result = sync_response.json()

# Descargar archivo ICS
if sync_result['success']:
    ics_content = sync_result['icsFileContent']
    with open(f"yacht_booking_{booking['id']}.ics", 'w') as f:
        f.write(ics_content)
    print(f"✅ Archivo ICS guardado: yacht_booking_{booking['id']}.ics")
    
    # Mostrar enlaces directos
    print("\n📅 Enlaces directos a calendarios:")
    for provider, link_data in sync_result['calendarLinks'].items():
        print(f"  • {link_data['displayName']}")
        print(f"    {link_data['directLink']}\n")
```

### Caso 4: Propiedad Airbnb

```bash
# Crear reserva de Airbnb
cat > airbnb_booking.json <<EOF
{
  "customerName": "Sophie Laurent",
  "customerEmail": "sophie@example.com",
  "customerPhone": "+33-1-2345-6789",
  "itemName": "Modern Loft in Downtown - 2BR/2BA",
  "itemType": "Airbnb",
  "type": "AirbnbProperty",
  "startDate": "2026-07-10T16:00:00Z",
  "endDate": "2026-07-17T10:00:00Z",
  "location": "Downtown Akwesasne, 456 Riverside Dr, Unit 12A",
  "totalAmount": 2100.00,
  "currency": "USD",
  "description": "Loft moderno con vista al río, WiFi de alta velocidad, cocina completa",
  "status": "Confirmed",
  "customFields": {
    "guests": "4",
    "checkInInstructions": "Self check-in with lockbox, code will be sent 24h before",
    "parking": "1 spot included"
  },
  "notes": "Guest requested early check-in if available"
}
EOF

# Crear y sincronizar
BOOKING_ID=$(curl -s -X POST http://localhost:5055/api/bookings \
  -H "Content-Type: application/json" \
  -d @airbnb_booking.json | jq -r '.id')

echo "Reserva creada: $BOOKING_ID"

# Generar archivo ICS y descargarlo
curl -X POST http://localhost:5055/api/calendar/generate-ics \
  -H "Content-Type: application/json" \
  -d @airbnb_booking.json \
  -o "airbnb_${BOOKING_ID}.ics"

echo "✅ Archivo ICS descargado: airbnb_${BOOKING_ID}.ics"
```

## 📧 Envío de Emails con Calendario

### Configurar Email (appsettings.json)

```json
{
  "Email": {
    "Enabled": true,
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "FromEmail": "bookings@ierahkwa.gov",
    "FromName": "IERAHKWA Booking System"
  }
}
```

### Enviar Email de Confirmación

```bash
curl -X POST http://localhost:5055/api/calendar/send-confirmation \
  -H "Content-Type: application/json" \
  -d '{
    "id": "abc-123",
    "customerName": "Ana García",
    "customerEmail": "ana@example.com",
    "itemName": "BMW 320i",
    "itemType": "Car",
    "type": "CarRental",
    "startDate": "2026-04-01T08:00:00Z",
    "endDate": "2026-04-05T18:00:00Z",
    "location": "IERAHKWA Rental Center",
    "totalAmount": 800.00,
    "currency": "USD"
  }'
```

El email incluirá:
- ✅ Detalles completos de la reserva
- ✅ Archivo .ics adjunto
- ✅ Botones para añadir a Google, Outlook, Yahoo, etc.
- ✅ Diseño profesional con branding IERAHKWA

## 🔐 Google Calendar Auto-Sync

### 1. Configurar OAuth 2.0

#### Paso 1: Google Cloud Console
1. Ve a https://console.cloud.google.com
2. Crea un nuevo proyecto: "IERAHKWA RnBCal"
3. Habilita "Google Calendar API"
4. Crea credenciales OAuth 2.0:
   - Tipo: Web application
   - Redirect URI: `http://localhost:5055/api/calendar/google/callback`

#### Paso 2: Configurar appsettings.json
```json
{
  "GoogleCalendar": {
    "Enabled": true,
    "ClientId": "tu-client-id.apps.googleusercontent.com",
    "ClientSecret": "tu-client-secret",
    "RedirectUri": "http://localhost:5055/api/calendar/google/callback",
    "CalendarId": "primary"
  }
}
```

#### Paso 3: Obtener Tokens de Acceso

```bash
# 1. Obtener URL de autorización
curl http://localhost:5055/api/calendar/google/oauth-url

# Respuesta:
# {
#   "oauthUrl": "https://accounts.google.com/o/oauth2/v2/auth?client_id=..."
# }

# 2. Abre la URL en el navegador, autoriza la aplicación
# 3. Serás redirigido a: http://localhost:5055/api/calendar/google/callback?code=...
# 4. Usa ese código para obtener tokens (esto requiere implementación adicional)
```

### 2. Usar Auto-Sync

Una vez configurado, cada vez que sincronices una reserva, automáticamente se añadirá a Google Calendar:

```bash
curl -X POST http://localhost:5055/api/calendar/sync \
  -H "Content-Type: application/json" \
  -d @booking.json
```

## 📊 Gestión de Reservas

### Listar Todas las Reservas

```bash
curl http://localhost:5055/api/bookings
```

### Obtener Reserva Específica

```bash
curl http://localhost:5055/api/bookings/abc-123
```

### Actualizar Reserva

```bash
curl -X PUT http://localhost:5055/api/bookings/abc-123 \
  -H "Content-Type: application/json" \
  -d '{
    "id": "abc-123",
    "customerName": "María González",
    "status": "InProgress",
    ...
  }'
```

### Eliminar Reserva

```bash
curl -X DELETE http://localhost:5055/api/bookings/abc-123
```

### Obtener Estadísticas

```bash
curl http://localhost:5055/api/bookings/stats
```

**Respuesta:**
```json
{
  "totalBookings": 45,
  "byStatus": [
    { "status": "Confirmed", "count": 28 },
    { "status": "InProgress", "count": 12 },
    { "status": "Completed", "count": 5 }
  ],
  "byType": [
    { "type": "CarRental", "count": 20 },
    { "type": "HotelRoom", "count": 15 },
    { "type": "YachtRental", "count": 5 },
    { "type": "AirbnbProperty", "count": 5 }
  ],
  "totalRevenue": 125000.00,
  "recentBookings": [...]
}
```

## 🌐 Integración con Otros Sistemas

### Desde WooCommerce (PHP)

```php
<?php
// Webhook de WooCommerce cuando se crea una orden de alquiler
add_action('woocommerce_order_status_completed', 'sync_to_rnbcal', 10, 1);

function sync_to_rnbcal($order_id) {
    $order = wc_get_order($order_id);
    
    $booking_data = [
        'customerName' => $order->get_billing_first_name() . ' ' . $order->get_billing_last_name(),
        'customerEmail' => $order->get_billing_email(),
        'customerPhone' => $order->get_billing_phone(),
        'itemName' => $order->get_items()[0]->get_name(),
        'itemType' => 'Car', // o dinámico según categoría
        'type' => 'CarRental',
        'startDate' => get_post_meta($order_id, '_rental_start_date', true),
        'endDate' => get_post_meta($order_id, '_rental_end_date', true),
        'location' => get_option('rental_pickup_location'),
        'totalAmount' => floatval($order->get_total()),
        'currency' => $order->get_currency(),
        'status' => 'Confirmed'
    ];
    
    // Enviar a RnBCal
    $response = wp_remote_post('http://localhost:5055/api/bookings', [
        'headers' => ['Content-Type' => 'application/json'],
        'body' => json_encode($booking_data)
    ]);
    
    if (!is_wp_error($response)) {
        $booking = json_decode(wp_remote_retrieve_body($response), true);
        
        // Sincronizar con calendarios
        wp_remote_post('http://localhost:5055/api/calendar/sync', [
            'headers' => ['Content-Type' => 'application/json'],
            'body' => json_encode($booking)
        ]);
        
        update_post_meta($order_id, '_rnbcal_booking_id', $booking['id']);
    }
}
?>
```

### Desde Node.js

```javascript
const axios = require('axios');

class RnBCalClient {
  constructor(baseUrl = 'http://localhost:5055/api') {
    this.baseUrl = baseUrl;
  }
  
  async createBooking(bookingData) {
    const response = await axios.post(`${this.baseUrl}/bookings`, bookingData);
    return response.data;
  }
  
  async syncToCalendar(booking) {
    const response = await axios.post(`${this.baseUrl}/calendar/sync`, booking);
    return response.data;
  }
  
  async sendConfirmationEmail(booking) {
    const response = await axios.post(`${this.baseUrl}/calendar/send-confirmation`, booking);
    return response.data;
  }
  
  async getBookings() {
    const response = await axios.get(`${this.baseUrl}/bookings`);
    return response.data;
  }
}

// Uso
const client = new RnBCalClient();

async function createAndSyncBooking() {
  const booking = await client.createBooking({
    customerName: "Test User",
    customerEmail: "test@example.com",
    itemName: "Test Item",
    itemType: "Car",
    type: "CarRental",
    startDate: new Date(Date.now() + 86400000 * 7).toISOString(),
    endDate: new Date(Date.now() + 86400000 * 14).toISOString(),
    location: "Test Location",
    totalAmount: 500,
    currency: "USD"
  });
  
  console.log('Booking created:', booking.id);
  
  const syncResult = await client.syncToCalendar(booking);
  console.log('Calendar sync:', syncResult.success ? 'SUCCESS' : 'FAILED');
  
  if (syncResult.success) {
    await client.sendConfirmationEmail(booking);
    console.log('Confirmation email sent');
  }
}

createAndSyncBooking();
```

## 🎯 Mejores Prácticas

### 1. Gestión de Errores

```javascript
try {
  const response = await fetch('http://localhost:5055/api/calendar/sync', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(booking)
  });
  
  if (!response.ok) {
    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
  }
  
  const result = await response.json();
  
  if (!result.success) {
    console.error('Sync failed:', result.errors);
    // Reintentar o notificar al usuario
  }
} catch (error) {
  console.error('Error syncing calendar:', error);
  // Manejo de errores: guardar en cola, notificar administrador, etc.
}
```

### 2. Validación de Datos

```javascript
function validateBooking(booking) {
  const required = ['customerName', 'customerEmail', 'itemName', 'startDate', 'endDate'];
  const missing = required.filter(field => !booking[field]);
  
  if (missing.length > 0) {
    throw new Error(`Missing required fields: ${missing.join(', ')}`);
  }
  
  if (new Date(booking.endDate) <= new Date(booking.startDate)) {
    throw new Error('End date must be after start date');
  }
  
  if (!booking.customerEmail.includes('@')) {
    throw new Error('Invalid email address');
  }
  
  return true;
}
```

### 3. Manejo de Zonas Horarias

```javascript
// Siempre usa UTC para las fechas
const booking = {
  startDate: new Date('2026-06-15T10:00:00').toISOString(), // Automáticamente a UTC
  endDate: new Date('2026-06-20T18:00:00').toISOString()
};

// Para mostrar al usuario, convierte a su zona horaria local
const localStartDate = new Date(booking.startDate).toLocaleString('es-ES', {
  timeZone: 'America/New_York',
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  hour: '2-digit',
  minute: '2-digit'
});
```

## 🔧 Troubleshooting

### Problema: Email no se envía

**Solución:**
1. Verifica que `Email.Enabled = true` en `appsettings.json`
2. Para Gmail, usa "App Passwords" en vez de tu contraseña normal
3. Revisa los logs del servidor para errores SMTP

### Problema: Google Calendar Auto-Sync no funciona

**Solución:**
1. Verifica que `GoogleCalendar.Enabled = true`
2. Asegúrate de tener tokens de acceso válidos
3. Revisa que la Google Calendar API esté habilitada en tu proyecto
4. Los tokens expiran - implementa refresh token flow

### Problema: Archivos ICS no se abren correctamente

**Solución:**
1. Verifica que el contenido ICS cumpla con RFC 5545
2. Algunos clientes de calendario requieren formato específico
3. Prueba descargando el archivo y abriéndolo manualmente

## 📚 Recursos Adicionales

- **RFC 5545** (iCalendar): https://tools.ietf.org/html/rfc5545
- **Google Calendar API**: https://developers.google.com/calendar
- **Swagger UI**: http://localhost:5055/swagger
- **IERAHKWA Platform**: ../IERAHKWA_PLATFORM_V1.html

---

**🏛️ Sovereign Government of Ierahkwa Ne Kanienke**  
IERAHKWA RnBCal v1.0.4 • © 2026 All Rights Reserved
