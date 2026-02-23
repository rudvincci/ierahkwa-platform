# IERAHKWA Outlook Email Extractor - API Usage Examples

## Table of Contents
1. [Authentication](#authentication)
2. [Email Extraction](#email-extraction)
3. [Export Operations](#export-operations)
4. [Statistics & Analytics](#statistics--analytics)
5. [Complete Workflows](#complete-workflows)

---

## Authentication

### 1. Service Principal Authentication

**Request:**
```bash
curl -X POST http://localhost:5055/api/authentication/authenticate \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "12345678-1234-1234-1234-123456789012",
    "clientId": "87654321-4321-4321-4321-210987654321",
    "clientSecret": "your-client-secret-here~1234567890"
  }'
```

**Response:**
```json
{
  "success": true,
  "message": "Authentication successful",
  "userEmail": "admin@yourcompany.com",
  "displayName": "System Administrator"
}
```

### 2. Interactive Browser Authentication

**Request:**
```bash
curl -X POST http://localhost:5055/api/authentication/authenticate-interactive \
  -H "Content-Type: application/json" \
  -d '{
    "clientId": "87654321-4321-4321-4321-210987654321"
  }'
```

**Response:**
```json
{
  "success": true,
  "message": "Authentication successful",
  "userEmail": "user@company.com",
  "displayName": "John Doe"
}
```

### 3. Check Authentication Status

**Request:**
```bash
curl http://localhost:5055/api/authentication/status
```

**Response:**
```json
{
  "authenticated": true,
  "userEmail": "admin@yourcompany.com",
  "displayName": "System Administrator"
}
```

---

## Email Extraction

### 1. Extract from All Sources

**Request:**
```bash
curl -X POST http://localhost:5055/api/extraction/all \
  -H "Content-Type: application/json" \
  -d '{
    "includeEmails": true,
    "includeCalendar": true,
    "includeContacts": true,
    "includeSent": true,
    "includeInbox": true,
    "includeArchive": false,
    "includeDeleted": false,
    "maxEmailsToScan": 5000,
    "maxCalendarEvents": 2000,
    "removeDuplicates": true,
    "includeMetadata": true
  }'
```

**Response:**
```json
{
  "success": true,
  "summary": {
    "totalEmailsFound": 1847,
    "uniqueEmailsFound": 923,
    "emailsScanned": 1245,
    "calendarEventsScanned": 432,
    "contactsScanned": 170,
    "startTime": "2026-01-18T14:30:00Z",
    "endTime": "2026-01-18T14:32:15Z",
    "duration": "00:02:15",
    "errors": [],
    "emailsBySource": {
      "Email": 1245,
      "Calendar": 432,
      "Contact": 170
    }
  },
  "emails": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "emailAddress": "john.doe@company.com",
      "displayName": "John Doe",
      "source": "Email",
      "sourceId": "AAMkAGI...",
      "extractedAt": "2026-01-18T14:30:05Z",
      "frequency": 15,
      "metadata": {
        "subject": "Project Update",
        "messageDate": "2026-01-15T10:30:00Z",
        "folderPath": "inbox",
        "company": "Acme Corp",
        "jobTitle": "Project Manager"
      }
    }
  ]
}
```

### 2. Extract from Emails Only

**Request:**
```bash
curl -X POST http://localhost:5055/api/extraction/emails \
  -H "Content-Type: application/json" \
  -d '{
    "includeInbox": true,
    "includeSent": true,
    "includeArchive": true,
    "includeDeleted": false,
    "maxEmailsToScan": 10000,
    "removeDuplicates": true,
    "startDate": "2025-01-01T00:00:00Z",
    "endDate": "2026-01-18T23:59:59Z"
  }'
```

**Response:**
```json
{
  "success": true,
  "count": 1245,
  "uniqueCount": 687,
  "emails": [...]
}
```

### 3. Extract from Calendar Events

**Request:**
```bash
curl -X POST http://localhost:5055/api/extraction/calendar \
  -H "Content-Type: application/json" \
  -d '{
    "maxCalendarEvents": 1000,
    "removeDuplicates": true,
    "includeMetadata": true,
    "startDate": "2025-01-01T00:00:00Z"
  }'
```

**Response:**
```json
{
  "success": true,
  "count": 432,
  "uniqueCount": 289,
  "emails": [
    {
      "emailAddress": "jane.smith@partner.com",
      "displayName": "Jane Smith",
      "source": "Calendar",
      "frequency": 8,
      "metadata": {
        "eventTitle": "Quarterly Business Review",
        "eventDate": "2025-12-15T14:00:00Z",
        "isOrganizer": false
      }
    }
  ]
}
```

### 4. Extract from Contacts

**Request:**
```bash
curl -X POST http://localhost:5055/api/extraction/contacts \
  -H "Content-Type: application/json" \
  -d '{
    "removeDuplicates": true,
    "includeMetadata": true
  }'
```

**Response:**
```json
{
  "success": true,
  "count": 170,
  "uniqueCount": 170,
  "emails": [
    {
      "emailAddress": "sales@vendor.com",
      "displayName": "Vendor Sales Team",
      "source": "Contact",
      "frequency": 1,
      "metadata": {
        "company": "Vendor Corp",
        "jobTitle": "Sales Representative",
        "phoneNumber": "+1-555-0100"
      }
    }
  ]
}
```

### 5. Get Extraction Results

**Request:**
```bash
curl http://localhost:5055/api/extraction/results
```

**Response:**
```json
{
  "success": true,
  "count": 1847,
  "uniqueCount": 923,
  "emails": [...]
}
```

### 6. Clear Extracted Data

**Request:**
```bash
curl -X DELETE http://localhost:5055/api/extraction/clear
```

**Response:**
```json
{
  "success": true,
  "message": "Extracted data cleared"
}
```

---

## Export Operations

### 1. Export to Text File

**Request:**
```bash
curl -X POST http://localhost:5055/api/export/text \
  -H "Content-Type: application/json" \
  -d '{
    "outputPath": "/Users/ruddie/exports/emails_20260118.txt"
  }'
```

**Response:**
```json
{
  "success": true,
  "textFilePath": "/Users/ruddie/exports/emails_20260118.txt",
  "recordsExported": 923,
  "message": "Successfully exported to text file"
}
```

### 2. Export to Excel File

**Request:**
```bash
curl -X POST http://localhost:5055/api/export/excel \
  -H "Content-Type: application/json" \
  -d '{
    "outputPath": "/Users/ruddie/exports/emails_20260118.xlsx"
  }'
```

**Response:**
```json
{
  "success": true,
  "excelFilePath": "/Users/ruddie/exports/emails_20260118.xlsx",
  "recordsExported": 923,
  "message": "Successfully exported to Excel file"
}
```

### 3. Export to Both Formats

**Request:**
```bash
curl -X POST http://localhost:5055/api/export/both \
  -H "Content-Type: application/json" \
  -d '{
    "outputDirectory": "/Users/ruddie/exports"
  }'
```

**Response:**
```json
{
  "success": true,
  "textFilePath": "/Users/ruddie/exports/ierahkwa_emails_20260118_143015.txt",
  "excelFilePath": "/Users/ruddie/exports/ierahkwa_emails_20260118_143015.xlsx",
  "recordsExported": 923,
  "message": "Export completed successfully"
}
```

### 4. Download Exported File

**Request:**
```bash
curl http://localhost:5055/api/export/download/ierahkwa_emails_20260118_143015.xlsx \
  --output emails.xlsx
```

---

## Statistics & Analytics

### 1. Get Comprehensive Statistics

**Request:**
```bash
curl http://localhost:5055/api/extraction/statistics
```

**Response:**
```json
{
  "success": true,
  "totalEmails": 1847,
  "uniqueEmails": 923,
  "domainStatistics": {
    "gmail.com": 324,
    "outlook.com": 198,
    "company.com": 156,
    "vendor.com": 89,
    "partner.com": 67,
    "hotmail.com": 45,
    "yahoo.com": 32,
    "other": 12
  },
  "sourceStatistics": {
    "Email": 1245,
    "Calendar": 432,
    "Contact": 170
  },
  "topFrequentEmails": [
    {
      "emailAddress": "john.doe@company.com",
      "displayName": "John Doe",
      "frequency": 45,
      "source": "Email"
    },
    {
      "emailAddress": "jane.smith@partner.com",
      "displayName": "Jane Smith",
      "frequency": 38,
      "source": "Email"
    },
    {
      "emailAddress": "support@vendor.com",
      "displayName": "Vendor Support",
      "frequency": 32,
      "source": "Email"
    }
  ]
}
```

---

## Complete Workflows

### Workflow 1: Complete Extraction & Export

```bash
#!/bin/bash

# Step 1: Authenticate
echo "Authenticating..."
AUTH_RESPONSE=$(curl -s -X POST http://localhost:5055/api/authentication/authenticate \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": "your-tenant-id",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret"
  }')

echo "Auth Response: $AUTH_RESPONSE"

# Step 2: Extract emails from all sources
echo "Extracting emails..."
EXTRACT_RESPONSE=$(curl -s -X POST http://localhost:5055/api/extraction/all \
  -H "Content-Type: application/json" \
  -d '{
    "includeEmails": true,
    "includeCalendar": true,
    "includeContacts": true,
    "maxEmailsToScan": 10000,
    "removeDuplicates": true
  }')

echo "Extraction Response: $EXTRACT_RESPONSE"

# Step 3: Get statistics
echo "Getting statistics..."
STATS_RESPONSE=$(curl -s http://localhost:5055/api/extraction/statistics)

echo "Statistics: $STATS_RESPONSE"

# Step 4: Export to both formats
echo "Exporting..."
EXPORT_RESPONSE=$(curl -s -X POST http://localhost:5055/api/export/both \
  -H "Content-Type: application/json" \
  -d '{
    "outputDirectory": "/Users/ruddie/exports"
  }')

echo "Export Response: $EXPORT_RESPONSE"
echo "Complete!"
```

### Workflow 2: Targeted Email Extraction

```bash
#!/bin/bash

# Extract only from sent emails for the last 30 days
THIRTY_DAYS_AGO=$(date -u -v-30d +"%Y-%m-%dT%H:%M:%SZ")
TODAY=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

curl -X POST http://localhost:5055/api/extraction/emails \
  -H "Content-Type: application/json" \
  -d "{
    \"includeInbox\": false,
    \"includeSent\": true,
    \"includeArchive\": false,
    \"includeDeleted\": false,
    \"startDate\": \"$THIRTY_DAYS_AGO\",
    \"endDate\": \"$TODAY\",
    \"removeDuplicates\": true,
    \"includeMetadata\": true
  }"
```

### Workflow 3: Domain-Specific Export

```bash
#!/bin/bash

# Extract all emails
curl -X POST http://localhost:5055/api/extraction/all \
  -H "Content-Type: application/json" \
  -d '{
    "includeEmails": true,
    "includeCalendar": true,
    "includeContacts": true,
    "removeDuplicates": true,
    "includeOnlyDomains": ["company.com", "partner.com"]
  }'

# Export to Excel
curl -X POST http://localhost:5055/api/export/excel \
  -H "Content-Type: application/json" \
  -d '{}'
```

### Workflow 4: Calendar-Only Analysis

```bash
#!/bin/bash

# Extract only from calendar for the last 6 months
SIX_MONTHS_AGO=$(date -u -v-6m +"%Y-%m-%dT%H:%M:%SZ")
TODAY=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

curl -X POST http://localhost:5055/api/extraction/calendar \
  -H "Content-Type: application/json" \
  -d "{
    \"maxCalendarEvents\": 5000,
    \"startDate\": \"$SIX_MONTHS_AGO\",
    \"endDate\": \"$TODAY\",
    \"removeDuplicates\": true,
    \"includeMetadata\": true
  }"

# Get statistics
curl http://localhost:5055/api/extraction/statistics

# Export to text
curl -X POST http://localhost:5055/api/export/text \
  -H "Content-Type: application/json" \
  -d '{
    "outputPath": "/Users/ruddie/exports/calendar_contacts.txt"
  }'
```

---

## PowerShell Examples

### Complete Extraction (Windows/PowerShell)

```powershell
# Authenticate
$authBody = @{
    tenantId = "your-tenant-id"
    clientId = "your-client-id"
    clientSecret = "your-client-secret"
} | ConvertTo-Json

$authResponse = Invoke-RestMethod -Uri "http://localhost:5055/api/authentication/authenticate" `
    -Method Post -Body $authBody -ContentType "application/json"

Write-Host "Authenticated as: $($authResponse.userEmail)"

# Extract all
$extractBody = @{
    includeEmails = $true
    includeCalendar = $true
    includeContacts = $true
    maxEmailsToScan = 10000
    removeDuplicates = $true
    includeMetadata = $true
} | ConvertTo-Json

$extractResponse = Invoke-RestMethod -Uri "http://localhost:5055/api/extraction/all" `
    -Method Post -Body $extractBody -ContentType "application/json"

Write-Host "Extracted $($extractResponse.summary.totalEmailsFound) emails"

# Export to Excel
$exportBody = @{
    outputDirectory = "C:\exports"
} | ConvertTo-Json

$exportResponse = Invoke-RestMethod -Uri "http://localhost:5055/api/export/excel" `
    -Method Post -Body $exportBody -ContentType "application/json"

Write-Host "Exported to: $($exportResponse.excelFilePath)"
```

---

## Python Examples

### Complete Workflow (Python)

```python
import requests
import json
from datetime import datetime, timedelta

BASE_URL = "http://localhost:5055"

# Authenticate
auth_data = {
    "tenantId": "your-tenant-id",
    "clientId": "your-client-id",
    "clientSecret": "your-client-secret"
}

auth_response = requests.post(
    f"{BASE_URL}/api/authentication/authenticate",
    json=auth_data
)

print(f"Authenticated: {auth_response.json()}")

# Extract emails from last 90 days
start_date = (datetime.now() - timedelta(days=90)).isoformat() + "Z"
end_date = datetime.now().isoformat() + "Z"

extract_config = {
    "includeEmails": True,
    "includeCalendar": True,
    "includeContacts": True,
    "startDate": start_date,
    "endDate": end_date,
    "maxEmailsToScan": 10000,
    "removeDuplicates": True,
    "includeMetadata": True
}

extract_response = requests.post(
    f"{BASE_URL}/api/extraction/all",
    json=extract_config
)

summary = extract_response.json()["summary"]
print(f"Extracted {summary['totalEmailsFound']} emails")
print(f"Unique emails: {summary['uniqueEmailsFound']}")

# Get statistics
stats_response = requests.get(f"{BASE_URL}/api/extraction/statistics")
stats = stats_response.json()

print(f"\nTop Domains:")
for domain, count in list(stats["domainStatistics"].items())[:5]:
    print(f"  {domain}: {count}")

# Export to both formats
export_data = {
    "outputDirectory": "/Users/ruddie/exports"
}

export_response = requests.post(
    f"{BASE_URL}/api/export/both",
    json=export_data
)

export_result = export_response.json()
print(f"\nExported to:")
print(f"  Text: {export_result['textFilePath']}")
print(f"  Excel: {export_result['excelFilePath']}")
```

---

## Error Handling

### Common Error Responses

**Not Authenticated:**
```json
{
  "success": false,
  "message": "Not authenticated. Please authenticate first."
}
```

**Invalid Credentials:**
```json
{
  "success": false,
  "message": "Authentication failed. Please check your credentials."
}
```

**No Data to Export:**
```json
{
  "success": false,
  "message": "No emails to export. Please extract emails first."
}
```

**Rate Limit Exceeded:**
```json
{
  "success": false,
  "message": "Microsoft Graph rate limit exceeded. Please try again later."
}
```

---

## Health Check

```bash
curl http://localhost:5055/health
```

**Response:**
```json
{
  "status": "healthy",
  "service": "Ierahkwa Outlook Email Extractor",
  "version": "1.0.0",
  "features": [
    "Office 365 Integration",
    "Outlook / Hotmail / Live Support",
    "Exchange Server Support",
    "Email Address Extraction",
    "Calendar Event Extraction",
    "Contact List Extraction",
    "Text File Export",
    "Excel File Export",
    "Statistics & Analytics"
  ],
  "timestamp": "2026-01-18T14:30:00Z"
}
```

---

**IERAHKWA Platform - Sovereign Government Technology**  
© 2026 All Rights Reserved
