# Plan de Implementación — 58 Microservicios .NET

**Objetivo:** Completar y unificar los 23 servicios reales (>500 líneas) + 35 parciales para dejarlos production-ready.

---

## Estado actual (resumen)

| Categoría | Cantidad | Qué tienen | Qué falta |
|-----------|:--------:|-------------|-----------|
| **COMPLETOS** | 23 | CRUD real, EF Core, Swagger, Controllers | Auth, CORS seguro, tests, health |
| **PARCIALES** | 35 | Controllers + DbContext básico | Validación, tests, logging |
| **STUB** | 6 | Program.cs mínimo | Todo (o eliminar) |

---

## Fase 1: Checklist por servicio (2-4h cada uno)

Para **cada** microservicio aplicar:

### 1.1 Seguridad
- [ ] **JWT/Auth** — `[Authorize]` en controllers o middleware
- [ ] **CORS** — Reemplazar `AllowAnyOrigin()` por whitelist (CORS_ORIGINS)
- [ ] **HTTPS** — Redirection en producción
- [ ] **Rate limiting** — Opcional por servicio

### 1.2 Observabilidad
- [ ] **Health check** — `/health` endpoint (ASP.NET `AddHealthChecks()`)
- [ ] **Logging** — Serilog o ILogger estructurado
- [ ] **OpenTelemetry** — Trazas (opcional)

### 1.3 Calidad
- [ ] **Validación** — FluentValidation o DataAnnotations en DTOs
- [ ] **Tests unitarios** — xUnit, 1 test por controller mínimo
- [ ] **OpenAPI** — Swagger en dev, documentación en prod (opcional)

### 1.4 Infraestructura
- [ ] **Dockerfile** — Multi-stage build
- [ ] **appsettings** — Connection strings desde env
- [ ] **docker-compose** — Entrada en sovereign.yml

---

## Fase 2: Shared library para todos

Crear `08-dotnet/shared/Mamey.Microservice.Base/` con:

```
Mamey.Microservice.Base/
├── Extensions/
│   ├── ServiceCollectionExtensions.cs   # AddJwtAuth, AddCorsSoberano, AddHealthChecks
│   └── ApplicationBuilderExtensions.cs # UseJwtAuth, UseCorsSoberano
├── Middleware/
│   └── RequestLoggingMiddleware.cs
├── Attributes/
│   └── AuthorizeSoberanoAttribute.cs
└── Mamey.Microservice.Base.csproj
```

Uso en cada microservicio:
```csharp
builder.Services.AddSoberanoAuth(builder.Configuration);
builder.Services.AddSoberanoCors(builder.Configuration);
builder.Services.AddSoberanoHealthChecks(builder.Configuration);
```

---

## Fase 3: Priorizar por impacto

### Grupo A — Core Gobierno (implementar primero)
| Servicio | Líneas | Prioridad | Tiempo est. |
|----------|:------:|:---------:|:-----------:|
| CitizenCRM | 1,950 | 1 | 4h |
| identity | 488 | 2 | 4h |
| TaxAuthority | 294 | 3 | 3h |
| JusticeService | 261 | 4 | 3h |
| VotingSystem | 348 | 5 | 3h |
| GovernanceService | 262 | 6 | 3h |
| AuditTrail | 319 | 7 | 3h |
| ContractManager | 315 | 8 | 3h |

### Grupo B — Finanzas y Compliance
| Servicio | Líneas | Prioridad | Tiempo est. |
|----------|:------:|:---------:|:-----------:|
| treasury | 975 | 9 | 4h |
| compliance | 674 | 10 | 4h |
| BudgetControl | 248 | 11 | 3h |
| DeFiSoberano | 184 | 12 | 6h (smart contracts) |

### Grupo C — Operaciones
| Servicio | Líneas | Prioridad | Tiempo est. |
|----------|:------:|:---------:|:-----------:|
| SearchService | 235 | 13 | 4h |
| MessagingService | 243 | 14 | 4h |
| EmergencyService | 243 | 15 | 3h |
| HealthService | 277 | 16 | 4h |
| EmploymentService | 238 | 17 | 3h |
| TransportService | 266 | 18 | 3h |
| TourismService | 430 | 19 | 3h (ya tiene CRUD) |

### Grupo D — Resto (35 servicios)
Aplicar checklist Fase 1 en batch. Estimación: 2h promedio = **70h total** (o 2 semanas con 2 devs).

---

## Fase 4: Servicios VACÍOS/STUB — Decidir

| Servicio | Líneas | Opción A | Opción B |
|----------|:------:|----------|----------|
| TransactionCodes | 0 | Crear desde plantilla Mamey.TemplateEngine | Eliminar del repo |
| AIModelServer | 163 | Completar con modelo simple (ML.NET o ONNX) | Dejar stub, documentar "fase 2" |
| NFTCertificates | 47 | Conectar a DeFiSoberano contracts | Stub |
| GovernanceDAO | 43 | Implementar DAO básico | Stub |
| AIFraudDetection | 43 | Reglas simples + log | Stub |

**Recomendación:** TransactionCodes crearlo; resto dejar como stub con `[ApiExplorerSettings(IgnoreApi = true)]` hasta fase 2.

---

## Fase 5: Integración con API Gateway

1. **Registrar todos en plataforma-principal** — Añadir proxy routes para cada microservicio.
2. **Service discovery** — Si usas K8s, servicios ya se descubren por nombre.
3. **Health checks agregados** — Gateway llama `/health` de cada servicio para status.

Ejemplo en `plataforma-principal`:
```javascript
{ path: '/api/tourism', target: 'http://tourism-service:5000' },
{ path: '/api/citizen-crm', target: 'http://citizen-crm:5000' },
// ... 58 rutas
```

---

## Fase 6: Docker Compose — Añadir los 58

Template por servicio:
```yaml
tourism-service:
  build: ./08-dotnet/microservices/TourismService
  environment:
    - ConnectionStrings__DefaultConnection=${POSTGRES_CONNECTION}
    - ASPNETCORE_ENVIRONMENT=Production
    - JWT_SECRET=${JWT_SECRET}
  depends_on: [postgres]
  networks: [sovereign-net]
```

Script para generar: `07-scripts/generate-docker-compose-microservices.sh`

---

## Resumen de esfuerzo

| Fase | Tareas | Tiempo total |
|------|--------|:------------:|
| 1 | Checklist x 58 servicios | 116-232h |
| 2 | Shared library | 8h |
| 3 | Grupo A+B (16 servicios prioritarios) | 52h |
| 4 | Stub/empty (5 servicios) | 8h |
| 5 | API Gateway integration | 16h |
| 6 | Docker Compose generation | 4h |
| **TOTAL** | | **~200-300h** (~8-12 semanas 1 dev) |

---

## Quick wins (esta semana)

1. **Crear Mamey.Microservice.Base** con Auth + CORS + Health (8h)
2. **Aplicar a CitizenCRM, TourismService, TaxAuthority** (12h)
3. **Fix CORS en CitizenCRM** — Reemplazar AllowAll por whitelist (30 min)
4. **Añadir /health a los 58** — Script o template (4h)
5. **Crear TransactionCodes** con Mamey.TemplateEngine (2h)

---

## Próximo paso concreto

¿Empezamos por la shared library `Mamey.Microservice.Base` o por aplicar el checklist a los 3 servicios prioritarios (CitizenCRM, TourismService, TaxAuthority)?
