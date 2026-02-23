# Inventory Manager Pro - Node.js Web Application

Sistema de Gestión de Inventario profesional construido con Node.js, Express, SQLite y Bootstrap.

## Características

### Funcionalidades Principales
- **Autenticación de Usuarios**: Login seguro con contraseñas hasheadas (bcrypt)
- **Soporte Multi-Usuario**: SQLite WAL mode para acceso concurrente
- **Control de Acceso por Roles**: Admin, Manager, User
- **Estado de Stock en Tiempo Real**: Seguimiento instantáneo del inventario

### Gestión de Productos
- Catálogo de productos con categorías y proveedores
- Soporte para código de barras
- Códigos de producto auto-generados
- Imágenes de productos
- Alertas de stock mínimo/máximo

### Operaciones de Stock
- **Stock In**: Entradas por compras
- **Stock Out**: Salidas por ventas
- **Quick Entry**: Soporte para escáner de código de barras
- **Ajustes**: Correcciones de inventario
- **Historial**: Registro completo de movimientos

### Búsqueda y Navegación
- Búsqueda rápida instantánea
- Auto-completar en formularios
- Filtros avanzados
- Búsqueda en grids con DataTables

### Reportes y Exportaciones
- Reporte de Stock actual
- Alerta de Stock Bajo
- Reporte de Movimientos
- Exportar a Excel (.xlsx)
- Exportar a PDF
- Imprimir desde cualquier reporte

### Gestión de Base de Datos
- Backup de base de datos (descargar)
- Backup completo (ZIP con imágenes)
- Log de actividad (auditoría)
- Configuración de la aplicación

## Requisitos

- Node.js 18+ 
- npm o yarn

## Instalación

```bash
# Navegar al directorio
cd inventory-system

# Instalar dependencias
npm install

# Iniciar servidor
npm start

# O para desarrollo con auto-reload
npm run dev
```

## Acceso

Abrir navegador en: **http://localhost:3500**

### Credenciales por Defecto
- **Usuario**: admin
- **Contraseña**: admin123

> **Importante**: Cambiar la contraseña después del primer inicio de sesión.

## Estructura del Proyecto

```
inventory-system/
├── server.js                 # Punto de entrada
├── package.json              # Dependencias
├── src/
│   ├── db.js                 # Configuración SQLite
│   ├── middleware/
│   │   └── auth.js           # Autenticación
│   └── routes/
│       ├── auth.js           # Login/Logout
│       ├── dashboard.js      # Dashboard
│       ├── products.js       # Productos CRUD
│       ├── categories.js     # Categorías
│       ├── suppliers.js      # Proveedores
│       ├── movements.js      # Movimientos de stock
│       ├── reports.js        # Reportes
│       ├── users.js          # Gestión de usuarios
│       ├── settings.js       # Configuración
│       └── api.js            # API JSON
├── views/                    # Templates EJS
│   ├── partials/             # Header/Footer
│   ├── products/             # Vistas de productos
│   ├── movements/            # Stock In/Out
│   ├── reports/              # Reportes
│   └── ...
├── public/
│   ├── css/style.css         # Estilos
│   ├── js/app.js             # JavaScript cliente
│   └── uploads/              # Imágenes de productos
└── data/
    └── inventory.db          # Base de datos SQLite
```

## Tecnologías

- **Backend**: Node.js, Express.js
- **Base de Datos**: SQLite (better-sqlite3) con WAL mode
- **Templates**: EJS
- **Frontend**: Bootstrap 5, DataTables, Chart.js
- **Autenticación**: bcryptjs, express-session
- **Exportación**: ExcelJS, PDFKit

## API Endpoints

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | /api/products/search?q= | Buscar productos |
| GET | /api/products/barcode/:code | Obtener por código/barcode |
| GET | /api/products/:id | Detalles del producto |
| GET | /api/stats | Estadísticas del dashboard |
| GET | /api/categories | Lista de categorías |
| GET | /api/suppliers | Lista de proveedores |
| POST | /movements/process | Procesar movimiento de stock |

## Roles de Usuario

| Rol | Permisos |
|-----|----------|
| admin | Acceso completo, gestión de usuarios y configuración |
| manager | Operaciones de inventario, reportes |
| user | Operaciones básicas de stock |

## Base de Datos

La base de datos SQLite se almacena en `data/inventory.db` y se crea automáticamente en el primer inicio.

### Tablas Principales
- users
- products
- categories
- suppliers
- stock_movements
- activity_logs
- settings

## Soporte

Para soporte técnico, contactar: support@inventory.local

## Licencia

Copyright © 2026 Sovereign Akwesasne Government. Todos los derechos reservados.
