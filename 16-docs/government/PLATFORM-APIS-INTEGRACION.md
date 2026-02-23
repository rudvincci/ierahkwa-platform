# APIs en Todas las Plataformas

**IERAHKWA FUTUREHEAD — Integración API unificada**

---

## Resumen

Todas las plataformas tienen acceso a las APIs del sistema mediante:

1. **ApiClient** — Cliente HTTP unificado con reintentos y timeout
2. **PlatformAPIs** — Verificación de conectividad y consumo de APIs

---

## APIs disponibles

| ID | Nombre | Endpoint | Categoría |
|----|--------|----------|-----------|
| platform-health | Platform Health | `/api/platform/health` | core |
| telecom | Telecom | `/api/v1/telecom/status` | telecom |
| telecom-network | Red Comunicación | `/api/v1/telecom/network` | telecom |
| telecom-voip | VoIP | `/api/v1/telecom/voip/status` | telecom |
| citizen-numbers | Números Ciudadanos | `/api/v1/telecom/citizen-numbers/info` | telecom |
| operativity | Operatividad 24/7 | `/api/operativity` | core |
| soberania | Soberanía | `/api/soberania/status` | core |
| node-health | Node Health | `/health` | core |

---

## Uso en plataformas

### Cargar scripts

```html
<script src="assets/api-client.js"></script>
<script src="assets/platform-apis.js"></script>
```

O usar `unified-load.js` (incluye ambos) o `unified-core.js` (carga automática si no están).

### Widget de estado

Agregar en cualquier plataforma:

```html
<div id="platformApis" data-platform-apis></div>
```

Se mostrará automáticamente el estado de todas las APIs (online/offline, latencia).

### Uso programático

```javascript
// Verificar todas las APIs
const results = await PlatformAPIs.checkAll();

// Obtener red de comunicación
const network = await PlatformAPIs.getNetwork();

// Obtener número de ciudadano
const citizen = await PlatformAPIs.getCitizenNumber('CIT-123');

// Usar ApiClient directamente
const { data } = await ApiClient.get('/api/v1/telecom/network/departments');
```

---

## Plataformas integradas

- index.html
- template-unified.html
- telecom-platform.html
- finance-platform.html
- government-portal.html
- Cualquier plataforma que cargue unified-core.js

---

*Febrero 2026 — IERAHKWA FUTUREHEAD*
