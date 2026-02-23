# Mamey.Authentik.Generated

This project contains the auto-generated API client code from Authentik's OpenAPI schema.

## Code Generation

To generate the client code:

### Using PowerShell

```powershell
.\scripts\generate-client.ps1 -AuthentikUrl "https://your-authentik-instance.com"
```

### Using Bash

```bash
./scripts/generate-client.sh https://your-authentik-instance.com
```

## Manual Generation Steps

1. Fetch the OpenAPI schema:
   ```bash
   ./scripts/update-schema.sh https://your-authentik-instance.com
   ```

2. Generate the client:
   ```bash
   ./scripts/generate-client.sh https://your-authentik-instance.com
   ```

## Generated Structure

After generation, this project will contain:

- `Api/` - Generated API client classes
- `Models/` - Generated DTOs and models
- `Client/` - Base HTTP client implementation

## Notes

- **Do not manually edit generated files** - They will be overwritten on regeneration
- **Regenerate after Authentik updates** - When Authentik releases new API versions
- **Review breaking changes** - Check for API changes that may affect your code
