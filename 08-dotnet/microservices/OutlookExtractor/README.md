# IERAHKWA Outlook Email Extractor

**Sovereign Government of Ierahkwa Ne Kanienke**  
Office 365 / Outlook / Hotmail / Exchange Address Extractor

## Overview

The IERAHKWA Outlook Email Extractor is a powerful .NET 10 application that connects to Microsoft 365, Office 365, Outlook, Hotmail, Live, and Exchange accounts to automatically extract email addresses from:

- **Emails** (Inbox, Sent Items, Archive, Deleted Items)
- **Calendar Events** (Organizers and Attendees)
- **Contacts** (Complete contact information)

All extracted data can be easily exported to:
- **Text Files** (.txt) - Clean email lists
- **Excel Files** (.xlsx) - Professional formatted spreadsheets

## Architecture

The application follows the **IERAHKWA Platform Architecture** with three layers:

### 1. OutlookExtractor.Core
- **Models**: Data structures for extracted emails, metadata, and configurations
- **Interfaces**: Service contracts and abstractions

### 2. OutlookExtractor.Infrastructure
- **Services**: 
  - `MicrosoftGraphService` - Handles Microsoft 365 authentication
  - `EmailExtractionService` - Extracts emails from various sources
  - `ExportService` - Exports data to text and Excel
  - `StatisticsService` - Provides analytics and insights

### 3. OutlookExtractor.API
- **Controllers**:
  - `AuthenticationController` - Handles Microsoft 365 authentication
  - `ExtractionController` - Manages email extraction operations
  - `ExportController` - Handles file exports
- **Swagger UI**: Complete API documentation

## Features

✅ **Office 365 / Microsoft 365 Integration**
- Native Microsoft Graph API integration
- Support for both service principal and interactive authentication
- Secure credential handling

✅ **Comprehensive Email Extraction**
- Extract from Inbox, Sent, Archive, and Deleted folders
- Extract from calendar events and meetings
- Extract from contact lists
- Automatic duplicate removal
- Frequency tracking

✅ **Advanced Filtering**
- Filter by date range
- Include/exclude specific domains
- Choose specific folders
- Limit number of items scanned

✅ **Professional Export**
- Text files with clean formatting
- Excel files with custom styling and colors
- Includes metadata (company, job title, phone, etc.)
- Automatic file naming with timestamps

✅ **Analytics & Statistics**
- Email count by domain
- Email count by source
- Top frequent contacts
- Detailed extraction summaries

## Prerequisites

- .NET 10 SDK
- Microsoft 365 / Office 365 account
- Azure AD App Registration (for authentication)

## Setup

### 1. Azure AD App Registration

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App Registrations**
3. Click **New Registration**
4. Set a name (e.g., "IERAHKWA Email Extractor")
5. Under **Redirect URI**, add: `http://localhost:5055/auth/callback`
6. Click **Register**

### 2. Configure API Permissions

1. In your app registration, go to **API Permissions**
2. Click **Add a permission** > **Microsoft Graph** > **Delegated permissions**
3. Add these permissions:
   - `Mail.Read`
   - `Mail.ReadWrite`
   - `Calendars.Read`
   - `Contacts.Read`
   - `User.Read`
4. Click **Grant admin consent**

### 3. Create Client Secret

1. Go to **Certificates & Secrets**
2. Click **New client secret**
3. Add description and set expiration
4. **Copy the secret value** (you won't see it again)

### 4. Configure Application

Edit `OutlookExtractor.API/appsettings.json`:

```json
{
  "MicrosoftGraph": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

## Running the Application

### Build the Solution

```bash
cd OutlookExtractor
dotnet build
```

### Run the API

```bash
cd OutlookExtractor.API
dotnet run
```

The API will start at:
- **HTTP**: http://localhost:5055
- **HTTPS**: https://localhost:7178

### Access Swagger UI

Open your browser and navigate to:
- http://localhost:5055/swagger

## Usage Examples

### 1. Authenticate

**POST** `/api/authentication/authenticate`

```json
{
  "tenantId": "your-tenant-id",
  "clientId": "your-client-id",
  "clientSecret": "your-client-secret"
}
```

### 2. Extract Emails from All Sources

**POST** `/api/extraction/all`

```json
{
  "includeEmails": true,
  "includeCalendar": true,
  "includeContacts": true,
  "includeSent": true,
  "includeInbox": true,
  "includeArchive": false,
  "maxEmailsToScan": 10000,
  "maxCalendarEvents": 5000,
  "removeDuplicates": true,
  "includeMetadata": true
}
```

### 3. Export to Excel

**POST** `/api/export/excel`

```json
{
  "outputPath": "/path/to/exports/emails.xlsx"
}
```

### 4. Export to Both Formats

**POST** `/api/export/both`

```json
{
  "outputDirectory": "/path/to/exports"
}
```

### 5. Get Statistics

**GET** `/api/extraction/statistics`

Returns:
```json
{
  "success": true,
  "totalEmails": 1542,
  "uniqueEmails": 847,
  "domainStatistics": {
    "gmail.com": 324,
    "outlook.com": 198,
    "company.com": 156
  },
  "sourceStatistics": {
    "Email": 1120,
    "Calendar": 312,
    "Contact": 110
  }
}
```

## Export File Formats

### Text File Format
```
======================================================
   IERAHKWA EMAIL EXTRACTOR - TEXT EXPORT
   Sovereign Government of Ierahkwa Ne Kanienke
   Generated: 2026-01-18 15:30:00 UTC
======================================================

Total Emails: 1542
Unique Emails: 847

EMAIL ADDRESSES:
------------------------------------------------------

john.doe@company.com
  Name: John Doe
  Source: Email
  Frequency: 5
  Company: Acme Corp
  Job Title: Manager

...
```

### Excel File Format
- Professional header with branding
- Color-coded sections
- Columns: Email Address, Display Name, Source, Frequency, Company, Job Title, Phone, Subject/Event, Date, Folder
- Auto-fit column widths
- Summary statistics at bottom

## API Endpoints Reference

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/authentication/authenticate` | Authenticate with service principal |
| POST | `/api/authentication/authenticate-interactive` | Interactive browser login |
| GET | `/api/authentication/status` | Check auth status |
| POST | `/api/extraction/all` | Extract from all sources |
| POST | `/api/extraction/emails` | Extract from emails only |
| POST | `/api/extraction/calendar` | Extract from calendar |
| POST | `/api/extraction/contacts` | Extract from contacts |
| GET | `/api/extraction/results` | Get all extracted emails |
| GET | `/api/extraction/statistics` | Get statistics |
| DELETE | `/api/extraction/clear` | Clear extracted data |
| POST | `/api/export/text` | Export to text file |
| POST | `/api/export/excel` | Export to Excel file |
| POST | `/api/export/both` | Export to both formats |
| GET | `/api/export/download/{fileName}` | Download exported file |

## Security Notes

- Never commit credentials to source control
- Use environment variables for sensitive data
- Grant minimum required API permissions
- Regularly rotate client secrets
- Use HTTPS in production

## Troubleshooting

### Authentication Issues
- Verify tenant ID, client ID, and client secret
- Ensure API permissions are granted
- Check admin consent is provided

### Extraction Errors
- Verify account has email/calendar data
- Check API permission scopes
- Review rate limiting from Microsoft Graph

### Export Issues
- Ensure output directory exists and is writable
- Check disk space availability
- Verify file paths are valid

## License

© 2026 Sovereign Government of Ierahkwa Ne Kanienke  
All Rights Reserved

## Support

For support and inquiries:
- Email: tech@ierahkwa.gov
- Documentation: http://localhost:5055/swagger

## Technology Stack

- **.NET 10** - Latest .NET framework
- **Microsoft Graph API** - Office 365 integration
- **Azure Identity** - Authentication
- **ClosedXML** - Excel file generation
- **Swagger/OpenAPI** - API documentation

---

**Built with the IERAHKWA Platform Architecture**  
*Sovereign Government Technology*
