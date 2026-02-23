# Qué más se puede implementar — Agriculture e industria medicinal

**Referencia:** Departamento de Agricultura (IGT-MA), Bitcoin Hemp, BHBK, plantas medicinales, ventas indígenas.  
**Fecha:** 2026-02-05  

---

## Ya implementado

- Catálogo plantas medicinales (registro + API `/api/v1/medicinal-industry/plants`, `registry`)
- Productos Américas y Caribe (JSON + API `products`, filtro por región)
- Seed-to-sale: lotes y estados (API: crear lote, actualizar estado)
- Franquicias industria medicinal (API: listar, registrar)
- BHBK: info banco, **cuentas** (abrir cuenta), **préstamos** (solicitar por terreno) — API + formularios en Agriculture
- Plataforma Agriculture: página única con accesos, plantas, productos, BHBK (cuentas y préstamos), seed-to-sale, ventas indígenas
- Card Agriculture en el main dashboard (index.html)
- Rutas y favoritos para `agriculture` / `igt-ma`

---

## Próximas implementaciones sugeridas

### Alta prioridad

| # | Qué implementar | Dónde | Notas |
|---|------------------|--------|--------|
| 1 | **Depósito en cuenta BHBK** | Node: `POST /bank/accounts/:id/deposit` | Simular pago a agricultor (monto, descripción); actualizar balance en `agriculture-bhbk-accounts.json` |
| 2 | **Consulta “Mi cuenta” / “Mis préstamos”** | agriculture-department.html | Formulario o panel: ingresar ID cuenta o nombre y listar balance; listar préstamos por accountId o applicantName |
| 3 | **UI Seed-to-sale** | agriculture-department.html o bitcoin-hemp.html | Formulario crear lote (planta, variedad, ubicación); lista de lotes con botón “Cambiar estado” (seed → grow → … → sold) |
| 4 | **Aprobar/rechazar préstamo (admin)** | Leader-control o página BHBK | Llamar `PUT /bank/loans/:id/status` con `approved` o `rejected`; lista de solicitudes pendientes |

### Media prioridad

| # | Qué implementar | Dónde | Notas |
|---|------------------|--------|--------|
| 5 | **Manufacturing Americas** | Bitcoin Hemp o Agriculture | Mapa o lista de sitios/estados donde hay manufactura (por región); integrado a 103 Departamentos |
| 6 | **Logística / delivery** | Nuevo módulo o Agriculture | Rutas, tracking de envíos; integrar con seed-to-sale (estado “distribute”) y órdenes |
| 7 | **Integración hoteles (wellness)** | IGT-HOTEL, NET10 | Paquetes “por planta” (wellness, retiros); productos del catálogo vinculados a reservas |
| 8 | **Formulario registrar franquicia** | agriculture-department.html | Form: nombre, tipo (dispensary/wellness/hotel), ubicación, plantas; POST a `/api/v1/medicinal-industry/franchises` |
| 9 | **Migración token Polygon → nodo** | Node + MultichainBridge | Especificación y flujo: token existente en Polygon → emisión o bridge al nodo soberano (8545); documentar en reporte |

### Baja prioridad / mejoras

| # | Qué implementar | Dónde | Notas |
|---|------------------|--------|--------|
| 10 | **Widget Agriculture en Leader-control** | leader-control.html | Resumen: nº cuentas BHBK, préstamos pendientes, lotes activos (llamadas a las APIs) |
| 11 | **Vínculo tienda indígena** | agriculture-department.html | Botón “Comprar” o “Ver en tienda” por producto → ierahkwa-shop o commerce-business-dashboard con filtro |
| 12 | **Más plantas (Iroquois Medical Botany)** | medicinal-plants-registry.json | Ampliar registro con más especies del libro (yarrow, milkweed, goldenrod, etc.) si aún no están |
| 13 | **Exportar trazabilidad** | API seed-to-sale | `GET /seed-to-sale/lots/:id/trace` o export CSV/PDF para auditoría y regulación |

---

## Resumen

- **Conectado:** Banco BHBK a la plataforma (cuentas + préstamos con formularios en Agriculture).
- **Siguiente paso recomendado:** Depósito en cuenta y consulta “Mi cuenta / Mis préstamos” para que los agricultores vean pagos y estado de préstamos.
- **Después:** UI seed-to-sale (crear lote, cambiar estado) y panel admin para aprobar/rechazar préstamos.

Principio: todo propio, sin dependencias de terceros (APIs y datos en el Node).
