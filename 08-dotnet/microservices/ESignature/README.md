# Ierahkwa ESignature

Sistema de Firma Electrónica Soberana para el Gobierno de Ierahkwa.

## Características

- **Firma Electrónica**: Firmas simples con imagen
- **Firma Digital**: Firmas con certificados X.509
- **Firma Blockchain**: Registro inmutable en Ierahkwa Sovereign Blockchain
- **Multi-firmante**: Soporte para múltiples firmantes secuenciales o paralelos
- **Verificación de Identidad**: Email, SMS, 2FA, Biométrica, ID Gubernamental
- **Campos Personalizables**: Firma, iniciales, fecha, texto, checkbox, etc.
- **Plantillas**: Reutilización de configuraciones de firma
- **Auditoría Completa**: Registro de todas las acciones con timestamps
- **Certificados**: Gestión completa de certificados digitales
- **API RESTful**: Integración fácil con otros sistemas

## Estructura del Proyecto

```
ESignature/
├── ESignature.API/          # Web API
├── ESignature.Core/         # Modelos e Interfaces
└── ESignature.Infrastructure/  # Servicios
```

## Endpoints Principales

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | /api/signaturerequests | Crear solicitud de firma |
| GET | /api/signaturerequests/{id} | Obtener solicitud |
| POST | /api/signaturerequests/{id}/send | Enviar para firmar |
| POST | /api/signing/{signerId}/sign | Firmar documento |
| POST | /api/signing/{signerId}/decline | Rechazar firma |
| GET | /api/signaturerequests/{id}/verify | Verificar documento |
| POST | /api/certificates | Crear certificado |

## Configuración

```bash
cd ESignature
dotnet restore
dotnet build
dotnet run --project ESignature.API
```

API disponible en: `https://localhost:7061`

## Token: IGT-ESIGN

---

**Sovereign Government of Ierahkwa Ne Kanienke**
