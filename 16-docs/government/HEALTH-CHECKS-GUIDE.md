# Health Checks Implementation Guide

## 📊 Sistema de Monitoreo Centralizado

El **Mega Dashboard** monitorea todos los servicios de la plataforma IERAHKWA. Para que un servicio aparezca en el dashboard, debe implementar un endpoint `/health`.

## ✅ Endpoints Requeridos

### Node.js Services
Ya implementado en `server.js`:
```javascript
app.get('/health', (req, res) => {
  res.json({ 
    status: 'healthy', 
    node: NODE_CONFIG.node.name,
    version: NODE_CONFIG.node.version,
    uptime: Date.now() - state.startTime
  });
});
```

### .NET Services (ASP.NET Core)

Agregar en `Program.cs` o `Startup.cs`:

```csharp
// Health Check endpoint
app.MapGet("/health", () => 
{
    return Results.Ok(new 
    {
        status = "healthy",
        service = "ServiceName",
        version = "1.0.0",
        timestamp = DateTime.UtcNow
    });
});
```

O usando el paquete `Microsoft.AspNetCore.Diagnostics.HealthChecks`:

```csharp
// En Program.cs
builder.Services.AddHealthChecks();

app.MapHealthChecks("/health");
```

### Python Services (FastAPI)

```python
from fastapi import FastAPI
from datetime import datetime

app = FastAPI()

@app.get("/health")
async def health():
    return {
        "status": "healthy",
        "service": "ServiceName",
        "version": "1.0.0",
        "timestamp": datetime.utcnow().isoformat()
    }
```

### Rust Services (Actix-web)

```rust
use actix_web::{web, HttpResponse, Responder};
use serde_json::json;

async fn health() -> impl Responder {
    HttpResponse::Ok().json(json!({
        "status": "healthy",
        "service": "ServiceName",
        "version": "1.0.0",
        "timestamp": chrono::Utc::now().to_rfc3339()
    }))
}

// En main()
.route("/health", web::get().to(health))
```

### Go Services

```go
func healthHandler(w http.ResponseWriter, r *http.Request) {
    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(map[string]interface{}{
        "status":    "healthy",
        "service":   "ServiceName",
        "version":   "1.0.0",
        "timestamp": time.Now().UTC().Format(time.RFC3339),
    })
}

// En main()
http.HandleFunc("/health", healthHandler)
```

## 🔧 Registro en Health Monitor

Los servicios se registran automáticamente en `node/services/health-monitor.js`. Para agregar un nuevo servicio:

```javascript
const SERVICES = {
    // ... servicios existentes
    
    nuevoServicio: {
        name: 'Nuevo Servicio',
        url: 'http://localhost:PUERTO/health',
        type: 'api', // o 'frontend'
        category: 'categoria' // platform, trading, defi, etc.
    }
};
```

## 📊 Categorías Disponibles

- `platform` - Servicios de plataforma
- `trading` - Trading y exchanges
- `defi` - DeFi protocols
- `financial` - Servicios financieros
- `government` - Servicios gubernamentales
- `documents` - Gestión documental
- `office` - Oficina y calendario
- `project` - Gestión de proyectos
- `infrastructure` - Infraestructura
- `data` - Data y reportes
- `security` - Seguridad
- `utilities` - Utilidades
- `blockchain` - Blockchain y crypto
- `ai` - Inteligencia artificial
- `hr` - Recursos humanos
- `frontend` - Páginas frontend

## 🎯 Core Systems

Los sistemas core (blockchain, bancos, etc.) se definen en `CORE_SYSTEMS` y no requieren health checks HTTP, solo status estático.

## 📈 Dashboard

Acceso al dashboard:
- URL: `http://localhost:8545/mega-dashboard.html`
- API: `http://localhost:8545/api/health/all`

El dashboard se actualiza automáticamente cada 5 segundos.

## ✅ Checklist de Implementación

- [ ] Agregar endpoint `/health` al servicio
- [ ] Retornar JSON con `status: "healthy"`
- [ ] Registrar servicio en `health-monitor.js`
- [ ] Verificar que el servicio aparezca en el dashboard
- [ ] Verificar que la latencia se muestre correctamente

## 🔍 Troubleshooting

Si un servicio no aparece en el dashboard:

1. Verificar que el endpoint `/health` responda correctamente
2. Verificar que la URL en `health-monitor.js` sea correcta
3. Verificar que el servicio esté corriendo
4. Revisar la consola del navegador para errores
5. Verificar CORS si el servicio está en otro dominio
