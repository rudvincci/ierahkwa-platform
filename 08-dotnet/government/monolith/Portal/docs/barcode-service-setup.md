# Barcode Service Setup & Integration

## Overview

The portal integrates with the Mamey Barcode service for generating AAMVA-compliant PDF417 barcodes for ID cards and driver licenses. The service runs as a separate Docker container and is accessed via HTTP API.

## Service Configuration

### Docker Compose

The barcode service is configured in `/Utilities/MameyBarcode/docker-compose.yml`:

```yaml
services:
  barcode-app:
    build: .
    ports:
      - "18648:5000"
    environment:
      FLASK_ENV: production
    networks:
      - portal_net
networks:
  portal_net:
    name: mamey-portal-net
```

### Application Configuration

The portal is configured to connect to the barcode service via `appsettings.json`:

```json
{
  "Barcode": {
    "ApiUrl": "http://localhost:18648"
  }
}
```

For development, override in `appsettings.Development.json` if needed.

## Starting the Service

### Option 1: Docker Compose (Recommended)

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Utilities/MameyBarcode
docker-compose up -d
```

### Option 2: Local Python

```bash
cd /Volumes/Barracuda/mamey-io/code-final/Utilities/MameyBarcode
python3 -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
pip install -r requirements.txt
python3 mamey-barcode.py
```

## Integration Points

### Services

1. **`IAamvaBarcodeService`** - Generates AAMVA-compliant PDF417 barcodes
   - `GenerateDriverLicenseBarcodeAsync()` - For driver license cards
   - `GenerateIdentificationCardBarcodeAsync()` - For identification cards
   - `GenerateBarcodeFromEncodedDataAsync()` - From pre-encoded AAMVA data

2. **`IBarcodeService`** (Mamey.Barcode) - Low-level barcode generation
   - `GeneratePDF417Async()` - PDF417 barcode generation with configurable parameters

3. **`IStandardsComplianceValidator`** - Field validation and sanitization
   - Validates AAMVA fields before encoding
   - Applies truncation and sanitization rules

### Usage in Document Issuance

The barcode service is automatically called during ID card and driver license issuance:

```csharp
// In CitizenshipBackofficeService.IssueIdCardAsync()
var aamvaCard = new IdentificationCard { /* ... */ };
var barcodeBytes = await _aamvaBarcode.GenerateIdentificationCardBarcodeAsync(aamvaCard, ct);
var barcodeBase64 = Convert.ToBase64String(barcodeBytes);
// Embed in HTML template
```

## AAMVA Compliance

The barcode generation uses AAMVA-compliant settings:

- **Security Level**: 5 (minimum for AAMVA compliance)
- **Columns**: 20 (standard column count)
- **Scale**: 2 (standard scale)
- **Ratio**: 3 (height-to-width ratio)
- **Padding**: 5 pixels

## Testing

### Verify Service is Running

```bash
curl http://localhost:18648/health
```

### Test PDF417 Generation

```bash
curl -X POST http://localhost:18648/generate-barcode \
  -H "Content-Type: application/json" \
  -d '{
    "data": "TEST_DATA",
    "type": "pdf417",
    "columns": 20,
    "security_level": 5,
    "scale": 2,
    "ratio": 3
  }'
```

### Test from Portal

1. Start the portal: `dotnet run --project src/Mamey.Portal.Web`
2. Issue an ID card via `/dev/issue/idcard` (dev-only endpoint)
3. Verify the barcode is embedded in the generated HTML

## Troubleshooting

### Service Not Accessible

- Verify the service is running: `docker ps | grep barcode`
- Check network connectivity: `docker network ls | grep portal_net`
- Verify port mapping: `docker port <container-id>`

### Barcode Generation Fails

- Check service logs: `docker logs <container-id>`
- Verify AAMVA field validation passes (check portal logs)
- Ensure encoded data is not empty (check `FieldEncodingHelper.Encode*` output)

### Network Issues

- Ensure both services are on the same Docker network (`portal_net`)
- For local development, use `http://localhost:18648` (not container name)
- For Docker-to-Docker, use `http://barcode-app:5000` (container name)

## References

- [Mamey Barcode API Documentation](../../Utilities/MameyBarcode/readme.md)
- [AAMVA Standards](../../Mamey/src/Mamey.AmvvaStandards/README.md)
- [Barcode Service Implementation](../src/Mamey.Portal.Citizenship.Application/Services/AamvaBarcodeService.cs)


