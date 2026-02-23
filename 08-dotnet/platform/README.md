# IERAHKWA Platform - .NET 10

## ✅ Plataforma Completa en .NET 10

Esta es la versión completa de la plataforma IERAHKWA migrada a .NET 10.

### 🚀 Características

- ✅ **ASP.NET Core Web API** - Framework .NET 10
- ✅ **Todos los Endpoints** - API completa migrada
- ✅ **68 Módulos Cargados** - 56 servicios + 12 departamentos
- ✅ **103 Tokens IGT** - Todos los tokens del gobierno
- ✅ **Health Checks** - Monitoreo de servicios
- ✅ **File System API** - Operaciones de archivos
- ✅ **AI Integration** - Chat y generación de código
- ✅ **Static Files** - Dashboard HTML servido

### 📁 Estructura del Proyecto

```
IERAHKWA.Platform/
├── Controllers/
│   ├── PlatformController.cs    # API de plataforma
│   ├── AIController.cs          # API de IA
│   ├── FilesController.cs       # API de archivos
│   ├── SettingsController.cs    # API de configuración
│   ├── MembersController.cs     # API de miembros
│   ├── UsageController.cs       # API de uso
│   └── DashboardController.cs  # API de dashboard
├── Services/
│   ├── IPlatformService.cs      # Interfaz servicio plataforma
│   ├── PlatformService.cs       # Implementación plataforma
│   ├── IAIService.cs           # Interfaz servicio IA
│   ├── AIService.cs            # Implementación IA
│   ├── IFileService.cs         # Interfaz servicio archivos
│   └── FileService.cs          # Implementación archivos
├── Models/
│   └── PlatformModels.cs       # Modelos de datos
└── Program.cs                   # Configuración principal
```

### 🔌 Endpoints Disponibles

#### Platform API
- `GET /api/platform/overview` - Resumen de la plataforma
- `GET /api/platform/services` - Estado de todos los servicios
- `GET /api/platform/health/{serviceId}` - Health check de servicio
- `GET /api/platform/modules` - Todos los módulos (servicios + departamentos)
- `GET /api/platform/departments` - Todos los departamentos
- `GET /api/platform/tokens` - Todos los tokens IGT
- `GET /api/platform/config` - Configuración completa

#### AI API
- `POST /api/ai/chat` - Chat con IA
- `POST /api/ai/code/generate` - Generar código
- `POST /api/ai/analyze` - Analizar código

#### Files API
- `GET /api/files/tree` - Árbol de archivos
- `GET /api/files/read` - Leer archivo
- `POST /api/files/save` - Guardar archivo
- `POST /api/files/create` - Crear archivo
- `POST /api/files/mkdir` - Crear directorio
- `DELETE /api/files/delete` - Eliminar archivo

#### Dashboard API
- `GET /api/dashboard/overview` - Datos del dashboard
- `GET /api/members` - Miembros del equipo
- `GET /api/usage/models` - Uso de modelos
- `POST /api/settings` - Guardar configuración

### 🏃 Ejecutar

```bash
cd IERAHKWA.Platform
dotnet run --urls "http://localhost:3000"
```

### 📊 Estadísticas

- **Total Módulos**: 68
- **Servicios**: 56
- **Departamentos**: 12
- **Tokens**: 103
- **Versión**: 2.0.0
- **Framework**: .NET 10.0

### ✅ Funcionalidades Implementadas

1. ✅ Carga de config.json
2. ✅ Health checks de servicios
3. ✅ API completa de plataforma
4. ✅ Integración con AI
5. ✅ Sistema de archivos
6. ✅ Dashboard HTML servido
7. ✅ CORS configurado
8. ✅ Swagger/OpenAPI

### 🌐 URLs

- **Dashboard**: http://localhost:3000/dashboard.html
- **API Health**: http://localhost:3000/api/health
- **Swagger**: http://localhost:3000/swagger (en desarrollo)

### 📝 Notas

- El proyecto carga automáticamente `config.json` desde `../../platform/config.json`
- Los archivos estáticos se sirven desde `../../platform/`
- El workspace de archivos está en `../../platform/workspace/`
