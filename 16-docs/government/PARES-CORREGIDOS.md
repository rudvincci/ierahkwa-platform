# Correcciones aplicadas – Páginas y servicios que no se veían

**Fecha:** 19 Enero 2026  
**Sovereign Government of Ierahkwa Ne Kanienke**

---

## 1. Enlaces "Volver" rotos (`../../../`)

En las UIs servidas desde **http://localhost:PUERTO/** (wwwroot), el enlace  
`href="../../../IERAHKWA_PLATFORM_V1.html"` no resuelve bien y lleva a 404 o a una ruta incorrecta.

### Cambio aplicado

Se reemplazó por un enlace que:

- Si hay historial: `history.back()` (vuelve a la plataforma o página previa).
- Si se abrió directo (p. ej. `http://localhost:5061/`): redirige a  
  `http://localhost:8545/platform` (nodo Ierahkwa con la plataforma).

### Archivos modificados

| Servicio     | Archivo                         |
|-------------|----------------------------------|
| FarmFactory | `FarmFactory.API/wwwroot/index.html` |
| RnBCal      | `RnBCal/RnBCal.API/wwwroot/index.html` |
| HRM         | `HRM/HRM.API/wwwroot/index.html`     |

---

## 2. `UseHttpsRedirection` en desarrollo

Con `app.UseHttpsRedirection()` activo y solo **HTTP** (p. ej. `http://localhost:5061`), el navegador redirige a HTTPS. Como en desarrollo a menudo no hay certificado o el HTTPS está en otro puerto, la página no carga o da error.

### Cambio aplicado

Se dejó la redirección a HTTPS **solo fuera de Development**:

```csharp
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
```

Así, en **Development** se puede usar sin problemas `http://localhost:PUERTO`.

### Program.cs modificados (fuera de `backup*` y `backup/`)

- **Con UI (wwwroot + / o /index.html):**  
  FarmFactory, NET10, RnBCal, HRM, IDOFactory, TradeX, DocumentFlow, ESignature, SpikeOffice, OutlookExtractor, AppBuilder, SmartSchool.

- **Solo API (sin wwwroot):**  
  DataHub, AuditTrail, NotifyHub, DigitalVault, FormBuilder, ReportEngine, BudgetControl, ProcurementHub, AssetTracker, ContractManager, ServiceDesk, CitizenCRM, ProjectHub, MeetingHub.

---

## 3. Cómo probar

1. **Nodo Ierahkwa (plataforma):**  
   ```bash
   ./start.sh
   ```
   Luego: `http://localhost:8545/platform`

2. **Servicios .NET con UI:**  
   Ejemplos:
   - FarmFactory: `cd FarmFactory/FarmFactory.API && dotnet run` → `http://localhost:5061`
   - NET10: `cd NET10/NET10.API && dotnet run` → `http://localhost:5071`
   - RnBCal: `http://localhost:5055`
   - HRM: `http://localhost:3030` (revisar `launchSettings.json`)

3. **Enlace "Volver":**  
   - Abriendo desde la plataforma (8545) → debe volver atrás.
   - Abriendo directo `http://localhost:5061` → debe ir a `http://localhost:8545/platform` (con el nodo en marcha).

---

## 4. Notas

- Las carpetas `backup`, `backup-*` y `IERAHKWA-INDEPENDENT` **no** se modificaron.
- Para producción con HTTPS válido, `UseHttpsRedirection` seguirá aplicando cuando  
  `ASPNETCORE_ENVIRONMENT` no sea `Development`.
- Si el nodo no corre en 8545, el enlace "Volver" en acceso directo llevará a  
  `http://localhost:8545/platform`; si ese puerto no responde, el navegador mostrará error de conexión. Ajustar la URL en los `wwwroot/index.html` si se usa otro host/puerto.

---

*Documento de correcciones – 19 Enero 2026*
