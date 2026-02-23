# 🚀 RnBCal - Quick Start Guide

## 5-Minute Setup

### Step 1: Navigate to the Project
```bash
cd "RnBCal/RnBCal.API"
```

### Step 2: Run the Server
```bash
dotnet run
```

### Step 3: Open in Browser
- **Frontend**: http://localhost:5055
- **Swagger API**: http://localhost:5055/swagger
- **Health Check**: http://localhost:5055/health

---

## 🎯 Your First Booking in 30 Seconds

### Using cURL (Terminal)

```bash
curl -X POST http://localhost:5055/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "customerPhone": "+1-555-0100",
    "itemName": "Tesla Model 3",
    "itemType": "Car",
    "type": "CarRental",
    "startDate": "2026-06-15T09:00:00Z",
    "endDate": "2026-06-20T17:00:00Z",
    "location": "IERAHKWA Rental Center",
    "totalAmount": 1000.00,
    "currency": "USD",
    "status": "Confirmed"
  }'
```

### Using JavaScript (Browser Console)

```javascript
fetch('http://localhost:5055/api/bookings', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    customerName: "Jane Smith",
    customerEmail: "jane@example.com",
    itemName: "Luxury Yacht",
    itemType: "Yacht",
    type: "YachtRental",
    startDate: new Date(Date.now() + 86400000 * 30).toISOString(),
    endDate: new Date(Date.now() + 86400000 * 37).toISOString(),
    location: "IERAHKWA Marina",
    totalAmount: 5000,
    currency: "USD"
  })
})
.then(r => r.json())
.then(booking => {
  console.log('Booking created:', booking);
  
  // Sync to calendars
  return fetch('http://localhost:5055/api/calendar/sync', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(booking)
  });
})
.then(r => r.json())
.then(result => console.log('Calendar sync:', result));
```

---

## 📅 Get Calendar Links

```bash
curl -X POST http://localhost:5055/api/calendar/calendar-links \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Test User",
    "itemName": "Test Booking",
    "itemType": "Car",
    "startDate": "2026-07-01T10:00:00Z",
    "endDate": "2026-07-05T18:00:00Z",
    "location": "Test Location"
  }'
```

**Response:**
```json
{
  "Google": {
    "directLink": "https://www.google.com/calendar/render?action=TEMPLATE&text=...",
    "displayName": "Add to Google Calendar"
  },
  "Outlook": { ... },
  "Yahoo": { ... },
  "Office365": { ... },
  "Apple": { ... },
  "AOL": { ... }
}
```

---

## 📥 Download ICS File

```bash
curl -X POST http://localhost:5055/api/calendar/generate-ics \
  -H "Content-Type: application/json" \
  -d @booking.json \
  -o booking.ics
```

Open `booking.ics` with any calendar application!

---

## 📊 View All Bookings

```bash
curl http://localhost:5055/api/bookings
```

---

## 📈 Get Statistics

```bash
curl http://localhost:5055/api/bookings/stats
```

---

## 🌐 Access from IERAHKWA Platform

1. Open `IERAHKWA_PLATFORM_V1.html` in browser
2. Go to **COMMERCE & BUSINESS** section
3. Click on **RnBCal Sync** card
4. Or use Quick Action button: **RnBCal SYNC**

---

## ⚙️ Optional: Configure Email (for notifications)

Edit `appsettings.json`:

```json
{
  "Email": {
    "Enabled": true,
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "bookings@ierahkwa.gov"
  }
}
```

Then send confirmation:

```bash
curl -X POST http://localhost:5055/api/calendar/send-confirmation \
  -H "Content-Type: application/json" \
  -d @booking.json
```

---

## 🔐 Optional: Setup Google Calendar Auto-Sync

1. Go to https://console.cloud.google.com
2. Create project "IERAHKWA RnBCal"
3. Enable "Google Calendar API"
4. Create OAuth 2.0 credentials
5. Update `appsettings.json`:

```json
{
  "GoogleCalendar": {
    "Enabled": true,
    "ClientId": "your-client-id.apps.googleusercontent.com",
    "ClientSecret": "your-client-secret"
  }
}
```

---

## 🎉 That's It!

You're now ready to:
- ✅ Create bookings
- ✅ Sync to multiple calendars
- ✅ Generate ICS files
- ✅ Send email confirmations
- ✅ Auto-sync with Google Calendar

---

## 📚 More Information

- **Full Documentation**: See `README.md`
- **Usage Guide**: See `USAGE_GUIDE.md`
- **Implementation Details**: See `IMPLEMENTATION_SUMMARY.md`
- **API Documentation**: http://localhost:5055/swagger

---

## 🆘 Need Help?

### Common Issues

**Port already in use?**
```bash
# Change port in launchSettings.json or use:
dotnet run --urls="http://localhost:5056"
```

**Can't send emails?**
- Make sure `Email.Enabled = true`
- Use App Password for Gmail (not regular password)
- Check SMTP settings

**Google Calendar not syncing?**
- Verify `GoogleCalendar.Enabled = true`
- Check OAuth credentials
- Ensure Google Calendar API is enabled

---

**🏛️ IERAHKWA RnBCal v1.0.4**  
Sovereign Government of Ierahkwa Ne Kanienke  
© 2026 All Rights Reserved
