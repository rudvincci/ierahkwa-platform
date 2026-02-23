# IERAHKWA HRM — Human Resource Management System

Sistema de Gestión de Recursos Humanos en **.NET 10**, integrado en la plataforma IERAHKWA.

## Características

- **Dashboard** con gráficos (asistencia, departamentos, estadísticas)
- **Asistencia** — Check-in/out, estados (Present, Absent, Late, HalfDay, Leave)
- **Permisos y vacaciones** — Tipos (Annual, Sick, Casual, Maternity), aprobación
- **Nómina** — Períodos, impuestos, deducciones, NetSalary
- **Préstamos** — Tipos, cuotas, intereses, estados
- **Reclutamiento** — Vacantes, plazos, candidatos
- **Premios al empleado** — Categorías, puntos de recompensa
- **Gestión de proyectos** — Avance, puntos al completar
- **Compras (Procurement)** — Items, proveedores, órdenes
- **Notificaciones** — Por empleado o globales
- **Puntos de recompensa** — Por premios y proyectos
- **Impuestos** — Configuración de tramos
- **Roles y permisos** — Control de acceso

## Ejecución

```bash
cd HRM
dotnet run --project HRM.API
```

La API y el dashboard se sirven en **http://localhost:5060**.

- **Dashboard**: http://localhost:5060 o http://localhost:5060/index.html  
- **Swagger**: http://localhost:5060/swagger  
- **Health**: http://localhost:5060/health  

## Integración en IERAHKWA

En **IERAHKWA_PLATFORM_V1.html**:

- Sección **HUMAN RESOURCES** con la tarjeta IERAHKWA HRM.
- Botón **HRM — RECURSOS HUMANOS** en **Acceso por Departamento**.
- Ruta de la app: `HRM/HRM.API/wwwroot/index.html`.

Si se abre el dashboard como archivo local (`file://`), las llamadas a la API usan por defecto `http://localhost:5060`; asegúrate de tener la API en ejecución.

## Estructura

```
HRM/
├── IerahkwaHRM.sln
├── HRM.Core/           # Modelos, DTOs, interfaces
├── HRM.Infrastructure/ # Servicios (en memoria)
└── HRM.API/            # Controladores, Program, wwwroot (dashboard)
```

## API (resumen)

| Recurso        | Ruta                         |
|----------------|------------------------------|
| Dashboard      | GET /api/dashboard/stats     |
| Empleados      | /api/employees               |
| Asistencia     | /api/attendance              |
| Permisos       | /api/leave                   |
| Nómina         | /api/payroll                 |
| Préstamos      | /api/loans                   |
| Reclutamiento  | /api/recruitment             |
| Premios        | /api/awards                  |
| Proyectos      | /api/projects                |
| Compras        | /api/procurement             |
| Notificaciones | /api/notifications           |
| Puntos         | /api/rewardpoints/employee/{id} |
| Impuestos      | /api/tax                     |
| Roles          | /api/roles                   |

---

**Sovereign Government of Ierahkwa Ne Kanienke** • .NET 10 • IERAHKWA Platform
