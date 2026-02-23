# IERAHKWA Outlook Email Extractor - Setup Guide

## SSL Certificate Fix for NuGet

If you're getting SSL certificate errors when running `dotnet restore`, try these solutions:

### Solution 1: Clear NuGet Cache
```bash
dotnet nuget locals all --clear
export SSL_CERT_FILE=""
dotnet restore
```

### Solution 2: Use HTTP NuGet Source (Temporary)
```bash
# Add insecure HTTP source temporarily
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org --protocol-version 3

# Or disable SSL certificate validation (for development only)
export DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT=0
dotnet restore
```

### Solution 3: Install/Update Root Certificates
```bash
# On macOS
/Applications/Python\ 3.*/Install\ Certificates.command

# Or update certificates manually
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain /path/to/cert
```

### Solution 4: Use NuGet Config
Create or edit `~/.nuget/NuGet/NuGet.Config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

## Manual Package Installation

If restore continues to fail, you can manually download and install packages:

1. Visit https://www.nuget.org/packages/
2. Download these packages:
   - Microsoft.Graph (v5.56.0)
   - Azure.Identity (v1.12.0)
   - ClosedXML (v0.104.1)
   - Microsoft.Identity.Web (v3.2.0)
   - Microsoft.AspNetCore.Authentication.JwtBearer (v8.0.0)
   - Swashbuckle.AspNetCore (v7.2.0)

3. Place them in your local NuGet cache:
   ```bash
   ~/.nuget/packages/
   ```

## Quick Start (After Fixing SSL)

### 1. Restore Packages
```bash
cd OutlookExtractor
dotnet restore
```

### 2. Build Solution
```bash
dotnet build
```

### 3. Run the API
```bash
cd OutlookExtractor.API
dotnet run
```

The API will be available at:
- **HTTP**: http://localhost:5055
- **HTTPS**: https://localhost:7178
- **Swagger UI**: http://localhost:5055/swagger

## Project Structure

```
OutlookExtractor/
├── IerahkwaOutlookExtractor.sln          # Solution file
├── OutlookExtractor.Core/                # Domain layer
│   ├── Models/
│   │   └── ExtractedEmail.cs             # Email models
│   └── Interfaces/
│       └── IServices.cs                  # Service interfaces
├── OutlookExtractor.Infrastructure/      # Service implementations
│   └── Services/
│       ├── MicrosoftGraphService.cs      # Graph API authentication
│       ├── EmailExtractionService.cs     # Email extraction logic
│       ├── ExportService.cs              # Text/Excel export
│       └── StatisticsService.cs          # Analytics
└── OutlookExtractor.API/                 # Web API layer
    ├── Controllers/
    │   ├── AuthenticationController.cs   # Auth endpoints
    │   ├── ExtractionController.cs       # Extraction endpoints
    │   └── ExportController.cs           # Export endpoints
    ├── Program.cs                        # App configuration
    ├── appsettings.json                  # Configuration
    └── wwwroot/
        └── index.html                    # Landing page
```

## Configuration

### Azure AD App Registration

1. Go to https://portal.azure.com
2. Navigate to **Azure Active Directory** > **App Registrations**
3. Click **New Registration**
4. Name: "IERAHKWA Email Extractor"
5. Redirect URI: `http://localhost:5055/auth/callback`
6. Click **Register**

### API Permissions

Add these Microsoft Graph permissions:
- `Mail.Read`
- `Mail.ReadWrite`
- `Calendars.Read`
- `Contacts.Read`
- `User.Read`

Grant admin consent for your organization.

### Client Secret

1. Go to **Certificates & Secrets**
2. Click **New client secret**
3. Copy the secret value (you won't see it again!)

### Update Configuration

Edit `OutlookExtractor.API/appsettings.json`:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-tenant-id-here",
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here"
  }
}
```

## Testing the API

### 1. Authenticate
```bash
curl -X POST http://localhost:5055/api/authentication/authenticate \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "your-tenant-id",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret"
  }'
```

### 2. Extract Emails
```bash
curl -X POST http://localhost:5055/api/extraction/all \
  -H "Content-Type: application/json" \
  -d '{
    "includeEmails": true,
    "includeCalendar": true,
    "includeContacts": true,
    "maxEmailsToScan": 1000,
    "removeDuplicates": true
  }'
```

### 3. Export to Excel
```bash
curl -X POST http://localhost:5055/api/export/excel \
  -H "Content-Type: application/json" \
  -d '{}'
```

### 4. Get Statistics
```bash
curl http://localhost:5055/api/extraction/statistics
```

## Features Implemented

### Core Features
- ✅ Microsoft Graph API integration
- ✅ Office 365 / Outlook / Hotmail / Exchange support
- ✅ Service Principal authentication
- ✅ Interactive browser authentication
- ✅ Email extraction (Inbox, Sent, Archive, Deleted)
- ✅ Calendar event extraction
- ✅ Contact list extraction
- ✅ Duplicate removal
- ✅ Metadata extraction (company, job title, phone)

### Export Features
- ✅ Text file export (.txt)
- ✅ Excel file export (.xlsx)
- ✅ Professional formatting
- ✅ Custom IERAHKWA branding
- ✅ Color-coded sections
- ✅ Summary statistics

### Analytics Features
- ✅ Domain statistics
- ✅ Source statistics
- ✅ Frequency tracking
- ✅ Top contacts report
- ✅ Extraction summaries

### API Features
- ✅ RESTful API design
- ✅ Swagger/OpenAPI documentation
- ✅ CORS support
- ✅ Health checks
- ✅ Comprehensive error handling
- ✅ Logging

## IERAHKWA Architecture

This project follows the standard **IERAHKWA Platform Architecture**:

### Three-Layer Pattern
1. **Core** - Domain models and interfaces (no dependencies)
2. **Infrastructure** - Service implementations (depends on Core)
3. **API** - Web API layer (depends on Core + Infrastructure)

### Design Principles
- Dependency Injection
- Interface-based design
- Separation of concerns
- Clean Architecture
- SOLID principles

This is the same architecture used in:
- TradeX (Crypto Exchange)
- SpikeOffice (Office Management)
- HRM (Human Resources)
- SmartSchool (Education Platform)
- AdvocateOffice (Legal System)

## Troubleshooting

### "Not authenticated" errors
- Ensure you've called `/api/authentication/authenticate` first
- Verify your credentials are correct
- Check API permissions are granted

### "Failed to retrieve emails"
- Verify the account has emails
- Check API permission scopes
- Review Microsoft Graph rate limits

### Export path errors
- Ensure the output directory exists
- Check write permissions
- Use absolute paths

### SSL/Certificate errors
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Update root certificates
- Try HTTP source temporarily (development only)

## Security Best Practices

1. **Never commit secrets** - Use environment variables or Azure Key Vault
2. **Use HTTPS** in production
3. **Rotate client secrets** regularly
4. **Minimum permissions** - Only grant required API scopes
5. **Audit logs** - Monitor authentication attempts
6. **Rate limiting** - Implement API throttling

## Next Steps

After fixing the SSL issue and restoring packages:

1. Build the solution: `dotnet build`
2. Run the API: `cd OutlookExtractor.API && dotnet run`
3. Open Swagger UI: http://localhost:5055/swagger
4. Test authentication endpoint
5. Extract emails from your account
6. Export to Excel/Text files

## Support

For assistance:
- Review the main README.md
- Check Swagger documentation at /swagger
- Review logs in the console output
- Verify Azure AD configuration

## License

© 2026 Sovereign Government of Ierahkwa Ne Kanienke  
All Rights Reserved

---

**IERAHKWA Platform - Sovereign Government Technology**
