# 🚀 DEPLOYMENT GUIDE - IERAHKWA NET10 PLATFORM

## Guía de Despliegue para Producción

**Versión:** 1.0.0  
**Estado:** ✅ 100% Operativo para Producción  
**Fecha:** 22 de Enero, 2026

---

## 📋 Requisitos Previos

### Software Requerido
- ✅ .NET 10 SDK instalado
- ✅ Sistema operativo: Linux, macOS, o Windows
- ✅ Puerto 5071 disponible (o configurar otro)
- ✅ Permisos de escritura en directorio de logs

### Verificar Instalación
```bash
dotnet --version
# Debe mostrar: 10.x.x o superior
```

---

## 🚀 Inicio Rápido - Producción

### Opción 1: Script de Inicio (Recomendado)
```bash
cd NET10
chmod +x start-production.sh
./start-production.sh Production 5071
```

### Opción 2: Comando Directo
```bash
cd NET10/NET10.API
export ASPNETCORE_ENVIRONMENT=Production
dotnet run --configuration Release --urls "http://0.0.0.0:5071"
```

### Opción 3: Publicar y Ejecutar
```bash
cd NET10/NET10.API
dotnet publish -c Release -o ./publish
cd publish
ASPNETCORE_ENVIRONMENT=Production dotnet NET10.API.dll --urls "http://0.0.0.0:5071"
```

---

## 🔧 Configuración de Producción

### Variables de Entorno

```bash
# Entorno
export ASPNETCORE_ENVIRONMENT=Production

# URL y Puerto
export ASPNETCORE_URLS="http://0.0.0.0:5071"

# Logging (opcional)
export ASPNETCORE_LOGGING__LOGLEVEL__DEFAULT=Information
```

### Archivo appsettings.Production.json

El archivo `appsettings.Production.json` ya está configurado con:
- ✅ Logging apropiado para producción
- ✅ Configuración de Kestrel
- ✅ Configuración de NET10 Platform

---

## 📊 Verificación de Salud

### Verificar que el Sistema Está Funcionando

```bash
# Ejecutar health check
cd NET10
chmod +x health-check.sh
./health-check.sh http://localhost:5071
```

### Verificar Manualmente

```bash
# Health endpoint
curl http://localhost:5071/health

# API Health
curl http://localhost:5071/api/health

# Services Status
curl http://localhost:5071/api/health/services
```

---

## 🌐 Acceso al Sistema

### URLs Principales

| Servicio | URL |
|----------|-----|
| **Swagger API Docs** | http://localhost:5071/swagger |
| **DeFi Exchange** | http://localhost:5071/index.html |
| **Dashboard** | http://localhost:5071/dashboard.html |
| **NAGADAN ERP** | http://localhost:5071/erp.html |
| **GoMoney** | http://localhost:5071/gomoney.html |
| **Geocoder Pro** | http://localhost:5071/geocoder.html |
| **Contributions** | http://localhost:5071/contributions.html |
| **Health Check** | http://localhost:5071/health |

### APIs Disponibles

| Producto | Endpoint Base |
|----------|--------------|
| College | `/api/college` |
| Cyber Cafe | `/api/cybercafe` |
| Hospital | `/api/hospital` |
| Inventory | `/api/inventory` |
| Finance | `/api/finance` |
| ERP | `/api/erp/*` |
| DeFi Swap | `/api/swap` |
| Pools | `/api/pool` |
| Farms | `/api/farm` |
| Tokens | `/api/token` |
| Hotel | `/api/hotel` |
| Geocoding | `/api/geocoding` |
| Web ERP | `/api/web-erp` |
| Banking | `/api/bank` |
| Contribution | `/api/contribution` |

---

## 🔄 Servicio como Daemon (Linux)

### Crear servicio systemd

Crear archivo `/etc/systemd/system/ierahkwa-net10.service`:

```ini
[Unit]
Description=Ierahkwa NET10 DeFi Platform
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=/path/to/NET10/NET10.API
ExecStart=/usr/bin/dotnet /path/to/NET10/NET10.API/bin/Release/net10.0/NET10.API.dll --urls "http://0.0.0.0:5071"
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

### Comandos del Servicio

```bash
# Recargar configuración
sudo systemctl daemon-reload

# Iniciar servicio
sudo systemctl start ierahkwa-net10

# Habilitar inicio automático
sudo systemctl enable ierahkwa-net10

# Ver estado
sudo systemctl status ierahkwa-net10

# Ver logs
sudo journalctl -u ierahkwa-net10 -f
```

---

## 🐳 Docker (Opcional)

### Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5071

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["NET10.API/NET10.API.csproj", "NET10.API/"]
RUN dotnet restore "NET10.API/NET10.API.csproj"
COPY . .
WORKDIR "/src/NET10.API"
RUN dotnet build "NET10.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NET10.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NET10.API.dll", "--urls", "http://0.0.0.0:5071"]
```

### Comandos Docker

```bash
# Construir imagen
docker build -t ierahkwa-net10:latest .

# Ejecutar contenedor
docker run -d -p 5071:5071 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name ierahkwa-net10 \
  ierahkwa-net10:latest

# Ver logs
docker logs -f ierahkwa-net10

# Detener
docker stop ierahkwa-net10
```

---

## 📝 Logs

### Ubicación de Logs

Los logs se generan en:
- **Consola:** Salida estándar
- **Archivo:** `logs/` (si está configurado)

### Niveles de Log

- **Information:** Operaciones normales
- **Warning:** Advertencias
- **Error:** Errores
- **Critical:** Errores críticos

---

## 🔐 Seguridad en Producción

### Recomendaciones

1. **Firewall:** Configurar reglas de firewall
2. **HTTPS:** Configurar certificado SSL (recomendado)
3. **CORS:** Configurar CORS apropiadamente
4. **Rate Limiting:** Implementar límites de tasa
5. **Autenticación:** Habilitar autenticación para APIs sensibles

### Configurar HTTPS (Opcional)

```csharp
// En Program.cs, descomentar y configurar:
builder.WebHost.UseUrls("https://0.0.0.0:5072");
// Configurar certificado SSL
```

---

## 📊 Monitoreo

### Health Checks

El sistema incluye endpoints de health check:
- `/health` - Health básico
- `/api/health` - Health de API
- `/api/health/services` - Estado de servicios

### Métricas

- Uptime del servidor
- Número de requests
- Estado de servicios
- Uso de memoria
- Threads activos

---

## 🔄 Actualización

### Proceso de Actualización

1. **Backup:** Hacer backup de datos
2. **Detener:** Detener el servicio actual
3. **Actualizar:** Copiar nuevos archivos
4. **Compilar:** `dotnet build --configuration Release`
5. **Iniciar:** Iniciar el servicio
6. **Verificar:** Ejecutar health check

---

## 🆘 Troubleshooting

### Problemas Comunes

#### Puerto en Uso
```bash
# Verificar qué proceso usa el puerto
lsof -i :5071

# Matar proceso si es necesario
kill -9 <PID>
```

#### Error de Compilación
```bash
# Limpiar y recompilar
dotnet clean
dotnet build --configuration Release
```

#### Servicio No Inicia
```bash
# Verificar logs
journalctl -u ierahkwa-net10 -n 50

# Verificar permisos
ls -la NET10/NET10.API
```

---

## ✅ Checklist de Producción

- [x] .NET 10 SDK instalado
- [x] Proyecto compila sin errores
- [x] Configuración de producción creada
- [x] Scripts de inicio creados
- [x] Health checks funcionando
- [x] Todos los servicios registrados
- [x] Frontends accesibles
- [x] APIs respondiendo
- [x] Logs configurados
- [x] Documentación completa

---

## 📞 Soporte

**Propiedad:** Sovereign Government of Ierahkwa Ne Kanienke  
**Sistema:** Ierahkwa NET10 DeFi Platform  
**Versión:** 1.0.0  
**Estado:** ✅ 100% Operativo

---

**© 2026 All Rights Reserved**
