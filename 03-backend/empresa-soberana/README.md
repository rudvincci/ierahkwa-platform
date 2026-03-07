# Empresa Soberana

> Sistema ERP soberano con modulos de CRM, inventario y facturacion para empresas de la nacion Ierahkwa.

## Descripcion

Empresa Soberana es el sistema de planificacion de recursos empresariales (ERP) de la plataforma Ierahkwa. Proporciona tres modulos activos completamente funcionales: CRM (gestion de contactos y clientes), Inventario (catalogo de productos y control de stock) y Contabilidad (facturacion con soporte multi-moneda). Ademas tiene dos modulos planificados: Recursos Humanos y Manufactura.

El sistema opera con una tasa impositiva del 0% conforme al Articulo VII de la Constitucion Soberana. Cada modulo implementa CRUD completo con busqueda, filtrado, ordenamiento y paginacion. Las facturas soportan multiples items con calculo automatico de subtotales y totales. El control de inventario incluye alertas de stock bajo y seguimiento de stock minimo por producto.

Un dashboard centralizado ofrece metricas en tiempo real: total de contactos, productos, facturas, ingresos cobrados, ingresos pendientes, valor total de inventario, y las 5 facturas mas recientes. Toda la persistencia se realiza en PostgreSQL con inicializacion automatica de esquema al arrancar.

## Stack Tecnico

- **Runtime**: Node.js 18+
- **Framework**: Express 4.x
- **Base de Datos**: PostgreSQL (pg 8.x)
- **Seguridad**: Helmet, CORS, middleware compartido Ierahkwa
- **Identificadores**: UUID v4
- **Puerto**: 3092

## API Endpoints

### Health y Dashboard

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con conteos y revenue |
| GET | /api/dashboard | Dashboard completo con metricas de todos los modulos |
| GET | /api/modules | Lista de modulos ERP (activos y planificados) |

### CRM — Contactos

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/contacts | Crear contacto (name, email requeridos) |
| GET | /api/contacts | Listar contactos (filtro: status, company, search) |
| GET | /api/contacts/:id | Obtener contacto por ID |
| PUT | /api/contacts/:id | Actualizar contacto |
| DELETE | /api/contacts/:id | Eliminar contacto |

### Inventario — Productos

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/products | Crear producto (name, sku, price requeridos) |
| GET | /api/products | Listar productos (filtro: category, search, inStock) |
| GET | /api/products/:id | Obtener producto por ID |
| PUT | /api/products/:id | Actualizar producto |
| DELETE | /api/products/:id | Eliminar producto |

### Contabilidad — Facturas

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/invoices | Crear factura (customer, items requeridos) |
| GET | /api/invoices | Listar facturas (filtro: status, customer, search) |
| GET | /api/invoices/:id | Obtener factura por ID |
| PUT | /api/invoices/:id | Actualizar factura (items solo en draft) |
| DELETE | /api/invoices/:id | Eliminar factura (no permite eliminar pagadas) |

### Estados de Factura

`draft` → `sent` → `paid` / `overdue` / `cancelled`

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3092 |
| DATABASE_URL | URL de conexion PostgreSQL | postgresql://user:pass@localhost/empresa |
| CORS_ORIGINS | Origenes permitidos | http://localhost:3000 |

## Instalacion

```bash
npm install
npm start
```

### Requisitos previos

- PostgreSQL 16 ejecutandose
- Las tablas (contacts, products, invoices) se crean automaticamente al arrancar

## Docker

```bash
docker build -t empresa-soberana .
docker run -p 3092:3092 \
  -e DATABASE_URL=postgresql://user:pass@db:5432/empresa \
  empresa-soberana
```

## Arquitectura

```
Cliente ──→ Express API ──→ PostgreSQL
              │
              ├── /api/contacts   → Tabla: contacts
              │     (CRM: leads, clientes, oportunidades)
              │
              ├── /api/products   → Tabla: products
              │     (Inventario: SKU, stock, stock minimo)
              │
              ├── /api/invoices   → Tabla: invoices
              │     (Contabilidad: items JSON, estados, moneda)
              │
              └── /api/dashboard  → Queries agregadas
                    (Revenue, stock value, conteos, top 5 facturas)

Modulos:
  [ACTIVO]     CRM, Accounting, Inventory
  [PLANIFICADO] Human Resources, Manufacturing

Tasa Impositiva: 0% (Articulo VII Constitucional)
```

Todas las operaciones de datos son registradas por el security logger compartido para trazabilidad completa.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
