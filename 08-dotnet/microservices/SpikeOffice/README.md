# SpikeOffice SaaS

## Human Resources & Office Management
### Ierahkwa Government | .NET 10

---

## 📊 OVERVIEW

SpikeOffice es el sistema integral de gestión de recursos humanos y oficina del Gobierno Soberano. Multi-tenant SaaS con todas las funciones de HR.

## 🏗️ ARQUITECTURA

```
┌─────────────────────────────────────────────────────────────┐
│                     SPIKEOFFICE SAAS                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌──────────┐ │
│  │  NÓMINA    │ │ ASISTENCIA │ │ CONTABIL.  │ │  TAREAS  │ │
│  │  PAYROLL   │ │ ATTENDANCE │ │ ACCOUNTING │ │  KANBAN  │ │
│  └────────────┘ └────────────┘ └────────────┘ └──────────┘ │
│                                                              │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌──────────┐ │
│  │ PRÉSTAMOS  │ │  PERMISOS  │ │ VACACIONES │ │ PREMIOS  │ │
│  │   LOANS    │ │   LEAVES   │ │  HOLIDAYS  │ │  AWARDS  │ │
│  └────────────┘ └────────────┘ └────────────┘ └──────────┘ │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │               PORTAL DE EMPLEADOS                     │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 MÓDULOS

### 1. Nómina (Payroll)
- Cálculo automático de salarios
- Deducciones fiscales
- Bonificaciones
- Horas extras
- Exportación a banco

### 2. Asistencia (Attendance)
- Clock in/out con IP
- Geolocalización
- Face recognition
- Reportes de puntualidad
- Alertas de ausencia

### 3. Contabilidad (Accounting)
- Doble partida
- Plan de cuentas configurable
- Estados financieros
- Conciliación bancaria
- Multi-moneda

### 4. Tareas (Task Management)
- Kanban boards
- Asignación de tareas
- Deadlines
- Colaboración en equipo
- Notificaciones

### 5. Préstamos (Loans)
- Solicitud de préstamos
- Aprobación multi-nivel
- Cálculo de intereses
- Descuento automático de nómina

### 6. Permisos (Leave Management)
- Solicitud de permisos
- Workflow de aprobación
- Balance de días
- Calendario de ausencias

### 7. Vacaciones (Holidays)
- Calendario de vacaciones
- Acumulación automática
- Planificación anual

### 8. Premios (Awards)
- Reconocimientos
- Puntos por desempeño
- Leaderboards

## 📡 API ENDPOINTS

```
Base URL: http://localhost:5056/api/v1

# Employees
GET    /employees
POST   /employees
GET    /employees/{id}
PUT    /employees/{id}

# Attendance
POST   /attendance/clock-in
POST   /attendance/clock-out
GET    /attendance/report

# Payroll
POST   /payroll/generate
GET    /payroll/slips
GET    /payroll/history

# Tasks
GET    /tasks
POST   /tasks
PUT    /tasks/{id}/status

# Leaves
POST   /leaves/request
GET    /leaves/balance
PUT    /leaves/{id}/approve
```

## 🔐 RBAC (Role-Based Access Control)

| Rol | Permisos |
|-----|----------|
| Admin | Todo |
| HR Manager | Empleados, Nómina, Permisos |
| Department Head | Su departamento, Tareas |
| Employee | Portal personal |

## 📁 ESTRUCTURA

```
SpikeOffice/
├── SpikeOffice.API/
│   ├── Controllers/
│   │   ├── EmployeesController.cs
│   │   ├── AttendanceController.cs
│   │   ├── PayrollController.cs
│   │   ├── TasksController.cs
│   │   └── LeavesController.cs
│   ├── Services/
│   ├── Models/
│   └── Program.cs
├── SpikeOffice.Core/
├── SpikeOffice.Infrastructure/
└── SpikeOffice.sln
```

## 🚀 DEPLOYMENT

```bash
cd SpikeOffice/SpikeOffice.API
dotnet run --urls "http://localhost:5056"
```

## 🔗 INTEGRACIONES

- BDET Bank (wire de nómina)
- DocumentFlow (documentos HR)
- E-Signature (contratos)
- NotifyHub (alertas)

---

**Puerto:** 5056
**Estado:** ✅ ACTIVO
**Token:** IGT-MLE (Ministry of Labor & Employment)

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
