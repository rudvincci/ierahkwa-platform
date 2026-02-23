# Monetización de la plataforma

Qué hay hoy y qué falta para métricas, cobros y dashboards de ingresos.

---

## 1. Lo que ya existe

| Componente | Dónde | Qué hace |
|------------|--------|----------|
| **Servicios de renta** | `platform/servicios-renta.html`, `servicios-plataformas.html` | Oferta de servicios y plataformas en renta (comercial). |
| **Licencias** | `platform/licenses-department.html` | Departamento de licencias (tipos, tarifas). |
| **Datos de precios** | `platform/data/commercial-services-rental.json`, `commercial-services-monthly.json`, etc. | Precios y categorías para renta. |
| **Renta comercial (widget)** | `platform/assets/commercial-rent-widget.js` | Widget de renta comercial en páginas. |
| **Revenue / monetización en Node** | Módulos `platform-revenue`, `revenue-aggregator`, `premium-pro` (APIs bajo `/api/v1/revenue`, etc.) | Agregación de ingresos y servicios premium. |

---

## 2. Lo que falta (roadmap)

| Objetivo | Acción |
|----------|--------|
| **Métricas unificadas** | Dashboard que agregue ingresos por servicio, licencia y plataforma (consumir APIs de revenue + datos de renta). |
| **Cobros automatizados** | Flujo de cobro recurrente (mensual/anual) ligado a servicios-renta y licencias; pagos soberanos (ver `pagos-soberano`, `provider-soberano`). |
| **Reportes para el líder** | ✅ Vista en ATABEY (Vista Global): card Ingresos · Total, Pagos, Marketplace, Trading, Licencias, Premium — `/api/v1/revenue-aggregate/summary`. |

---

## 3. Cómo seguir

- Reutilizar **platform-links.json** y las páginas de **servicios-renta** y **licenses-department** como fuente de “productos”.
- Conectar **revenue-aggregator** y **platform-revenue** con datos reales de transacciones cuando existan.
- ~~Card Ingresos en ATABEY~~ **Hecho:** Vista Global muestra Ingresos vía `/api/v1/revenue-aggregate/summary`. Pendiente: cobros automáticos.

---

**Referencias:** `RuddieSolution/platform/data/commercial-services-rental.json`, `RuddieSolution/node/modules/platform-revenue.js`, `revenue-aggregator.js`, `docs/ROADMAP-ALCANCE-Y-NECESIDADES.md` (Negocio · Monetización).
