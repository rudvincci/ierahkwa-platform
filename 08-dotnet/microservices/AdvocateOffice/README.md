# Advocate Office Management System — .NET 10

Sistema de gestión para bufetes/abogados (IGT-LEGAL), implementado en **.NET 10** e integrado en la plataforma IERAHKWA.

## Credenciales demo

- **Usuario:** `admin`
- **Contraseña:** `advocate`

## Cómo ejecutar

```bash
cd AdvocateOffice
dotnet run
```

- **App:** http://localhost:3010/
- **Login:** http://localhost:3010/login.html
- **API Swagger:** http://localhost:3010/swagger

## Módulos

- **Dashboard** — Resumen (clientes, casos, tareas, facturas)
- **Clientes** — CRUD
- **Tribunales** — CRUD
- **Categorías de caso** — CRUD
- **Etapas de caso** — CRUD
- **Casos** — CRUD (con cliente, tribunal, categoría, etapa)
- **Documentos** — Por caso
- **Facturas** — Con ítems, impuestos, fechas de emisión y vencimiento
- **Tareas** — Por caso, estados
- **Empleados** — HR
- **Banking** — Cuentas bancarias
- **Proveedores** — Vendors
- **Productos/Servicios** — Para facturación
- **Contactos** — Formulario de contacto
- **Testimonios** — Gestión
- **Sponsors** — Patrocinadores
- **Actualizaciones** — Novedades
- **Citas (Appointments)** — Agenda

## Acceso desde IERAHKWA Platform

En **IERAHKWA_PLATFORM_V1.html**, en la sección **GOVERNMENT & LEGAL**, la tarjeta **Advocate Office** abre http://localhost:3010/ cuando el servidor está en marcha.

## Tecnologías

- **.NET 10** (net10.0)
- ASP.NET Core, BCrypt.Net, Swashbuckle
- Persistencia en `Data/database.json`

---
Sovereign Government of Ierahkwa Ne Kanienke · IGT-LEGAL
