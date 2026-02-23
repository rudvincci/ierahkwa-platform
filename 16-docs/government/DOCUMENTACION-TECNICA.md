# Documentación Técnica — Ierahkwa Sovereign Platform v01

**Para desarrolladores y administradores: APIs, arquitectura, componentes**  
Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## 1. Visión general

### 1.1 Stack tecnológico

| Capa | Tecnología |
|------|------------|
| Runtime | Node.js ≥18 (recomendado 20 LTS) |
| Backend | Express, Fastify (según módulo) |
| Base de datos | SQLite (desarrollo), PostgreSQL (producción), JSON (algunos módulos) |
| Frontend | HTML, CSS, JavaScript (Bootstrap, jQuery en varios módulos) |
| Auth | bcryptjs, JWT, sesiones |
| Blockchain | Ierahkwa Sovereign Blockchain (ISB), Ierahkwa Futurehead Mamey Node |

### 1.2 Módulos .NET

- **SmartSchool**, **InventoryManager**, **TradeX**, **IerahkwaBankPlatform**: .NET 8+ (revisar cada .csproj).  
- Bases de datos: SQL Server o compatible según configuración.

---

## 2. Arquitectura de servicios

### 2.1 Esquema simplificado

```
                    ┌─────────────────────────────────────┐
                    │     PORTAL (platform/index.html)     │
                    │     platform-services.json          │
                    └─────────────────────────────────────┘
                                          │
    ┌─────────────┬─────────────┬─────────┼─────────┬─────────────┬─────────────┐
    │             │             │         │         │             │             │
    ▼             ▼             ▼         ▼         ▼             ▼             ▼
┌───────┐   ┌───────────┐   ┌──────┐  ┌──────┐  ┌──────┐   ┌──────────┐   ┌─────────┐
│ Shop  │   │ POS/Chat  │   │Invent│  │ Node │  │Image │   │SmartSchool│   │Forex/   │
│ :3100 │   │ (Shop)    │   │(Shop)│  │ :8545│  │:3500 │   │ (Node)   │   │TradeX   │
└───────┘   └───────────┘   └──────┘  └──────┘  └──────┘   └──────────┘   └─────────┘
    │             │             │         │         │             │             │
    └─────────────┴─────────────┴─────────┴─────────┴─────────────┴─────────────┘
                                          │
                              ┌───────────┴───────────┐
                              │  BD: SQLite / JSON /  │
                              │  PostgreSQL / SQL Srv │
                              └───────────────────────┘
```

### 2.2 Responsabilidades por componente

| Componente | Ruta/Carpeta | Puerto típico | Función |
|------------|--------------|---------------|---------|
| Portal | `platform/` | - | Punto de entrada, lista de servicios |
| Shop | `ierahkwa-shop/` | 3100 | E-Commerce, Admin, POS, Chat, Inventario |
| Node | `node/` | 8545 | Blockchain: RPC, REST, tokens, stats |
| Image Upload | `image-upload/` | 3500 | Subida y gestión de imágenes |
| POS System | `pos-system/` | 3000 | POS alternativo con CRM/inventario |
| SmartSchool Node | `smart-school-node/` | - | API educación |
| Forex | `forex-trading-server/` | - | Servidor Forex |

---

## 3. APIs

### 3.1 Ierahkwa Shop (E-Commerce) — puerto 3100

Base: `http://localhost:3100` (o el host/puerto configurado).

#### Productos

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/products` | Listar productos |
| GET | `/api/products/:id` | Obtener producto |
| POST | `/api/products` | Crear (admin) |
| PUT | `/api/products/:id` | Actualizar (admin) |
| DELETE | `/api/products/:id` | Eliminar (admin) |

#### Categorías

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/categories` | Listar categorías |

#### Pedidos

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/orders` | Listar pedidos |
| POST | `/api/orders` | Crear pedido |
| GET | `/api/orders/:id` | Obtener pedido |
| PATCH | `/api/orders/:id` | Actualizar estado (admin) |

#### Admin / Dashboard

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/admin/dashboard` | Estadísticas (requiere auth admin) |

- Autenticación: sesión o JWT según implementación en `src/routes/admin.js`, `auth.js` o middleware equivalente.  
- Rutas adicionales: revisar `ierahkwa-shop/src/routes/` (shop, inventory, pos, banking, monetary, node, services, backup).

### 3.2 Node (Blockchain) — puerto 8545

| Recurso | URL | Descripción |
|---------|-----|-------------|
| Dashboard | `http://localhost:8545` | Interfaz web |
| RPC | `http://localhost:8545/rpc` | JSON-RPC (métodos tipo eth_*) |
| REST — Tokens | `http://localhost:8545/api/v1/tokens` | Lista de tokens IGT |
| REST — Stats | `http://localhost:8545/api/v1/stats` | Estadísticas de red |
| REST — API genérica | `http://localhost:8545/api/v1` | Otros endpoints según `node/` |

### 3.3 POS System (pos-system) — puerto 3000

Revisar `pos-system/src/routes/`:

- `auth.js`: login, sesión  
- `items.js`: ítems de menú  
- `orders.js`: pedidos  
- `tables.js`: mesas  
- `inventory.js`, `reports.js`, `users.js`, `crm.js`  

Base: `http://localhost:3000/api/...` (o el prefijo definido en `server.js`).

### 3.4 Image Upload — puerto 3500

- **POST** a la ruta de upload (p. ej. `/upload`) con `multipart/form-data`.  
- Archivos en `public/uploads/`.  
- Consultar `image-upload/server.js` para la ruta exacta y opciones.

### 3.5 SmartSchool (smart-school-node)

Rutas en `smart-school-node/src/routes/`:

- `auth.routes.js`, `student.routes.js`, `teacher.routes.js`, `classroom.routes.js`, `grade.routes.js`, `homework.routes.js`, `invoice.routes.js`, `library.routes.js`, `liveclass.routes.js`, `material.routes.js`, `parent.routes.js`, `schedule.routes.js`, `tenant.routes.js`, `user.routes.js`, `web.routes.js`, etc.

Prefijo típico: `/api/...` (revisar `server.js` o `app.js`).

### 3.6 Producción (URLs de referencia)

- REST API: `https://api.ierahkwa.gov`  
- GraphQL: `https://graphql.ierahkwa.gov`  
- Trading: `https://api.trading.ierahkwa.gov`  
- Bank: `https://bdet.ierahkwa.gov/api/v1`  
- RPC: `https://rpc.ierahkwa.gov`  

Detalle completo en `REPORTE-COMPLETO-PLATAFORMA.md` y `REPORTE-EJECUTIVO-COMPLETO-2026.md`.

---

## 4. Base de datos

### 4.1 Shop (ierahkwa-shop)

- **Motor**: SQLite o PostgreSQL según `src/db.js` y variables de entorno.  
- **Schema**: `scripts/db/schema.sql`.  
- **Tablas típicas**: users, products, categories, orders, order_items, customers, inventory, etc. (según schema real).

### 4.2 POS (pos-system)

- **Almacenamiento**: `data/database.json` (JSON).  
- Estructura según `src/db.js` o equivalentes.

### 4.3 Node

- Estado de la blockchain y configuración según implementación en `node/` (archivos, LevelDB o similar según el cliente usado).

### 4.4 SmartSchool / TradeX / InventoryManager

- .NET: connection string en `appsettings.json`.  
- Entidades en `*.Domain/Entities/` o `Models/`.  
- Migraciones o scripts SQL según cada solución.

---

## 5. Autenticación y autorización

- **Shop/Admin**: sesión (express-session), cookies; posible JWT en APIs.  
- **Login**: bcrypt para hash de contraseñas.  
- **Roles**: admin, manager, usuario, etc., según `users.role` o tabla de permisos.  
- **APIs**: middleware de auth en `src/routes/` o `middleware/`; validar token JWT o sesión antes de rutas protegidas.

---

## 6. Blockchain y tokens IGT

- ** Estándar**: Ierahkwa Government Token (IGT).  
- **Decimales**: 9.  
- **Supply por token**: 10.000.000.000.000 (10T).  
- **Total tokens**: 100 (40 gobierno + 60 utility).  
- **Token maestro**: IGT-SOVEREIGN.  
- Especificaciones por token: directorio `tokens/` (p. ej. `01-IGT-PM`, `08-IGT-NT`, …).  
- **Generador**: `generate-tokens.js` para landing pages y metadatos.

---

## 7. Configuración (archivos clave)

| Archivo | Uso |
|---------|-----|
| `platform-services.json` | Servicios del Portal: lista, puertos, rutas, categorías |
| `PLATAFORMA-UNIFICADA.json` | Módulos, tokens, infraestructura unificada |
| `ierahkwa-shop/.env` | PORT, DB, JWT, etc. |
| `node/config.toml` | Configuración del nodo blockchain |
| `pos-system/data/database.json` | Datos POS |
| `*/*/appsettings.json` | .NET: BD, logs, URLs |

---

## 8. Dependencias principales (Node)

Revisar `package.json` de cada módulo. Ejemplos:

- **express** — servidor HTTP  
- **express-session**, **cookie-parser** — sesiones  
- **bcryptjs** — contraseñas  
- **jsonwebtoken** — JWT (si se usa)  
- **sql.js** o **pg** — SQLite / PostgreSQL  
- **multer** — subida de archivos  
- **socket.io** — Chat en tiempo real  
- **dotenv** — variables de entorno  

Detalle de librerías propias vs. abiertas: **`docs/LIBRERIAS-COMPONENTES.md`**.

---

## 9. Despliegue y operación

- **Proceso**: PM2 o systemd para Node.  
- **Proxy**: Nginx/Apache con HTTPS (TLS 1.2+).  
- **Logs**: stdout/stderr; PM2 o journald; opcional agregación (ELK, Loki, etc.).  
- **Monitoreo**:健康 checks HTTP a `/` o `/api/health` si existen; Prometheus/Grafana si se configuran.  
- **Backups**: BD (SQLite/PostgreSQL) y `data/`, `uploads/`, `config`; automatizar con cron o tareas programadas.

---

## 10. Seguridad

- **HTTPS** en producción.  
- **Secrets**: no commitear `.env`; usar variables de entorno o vault.  
- **CORS**: restringir orígenes en APIs.  
- **Rate limiting**: considerar en rutas públicas (express-rate-limit o similar).  
- **Validación**: validar y sanitizar entradas; evitar inyección SQL (consultas parametrizadas/ORM).  
- **Dependencias**: `npm audit`; actualizar paquetes con vulnerabilidades.

---

## 11. Referencias cruzadas

- **EULA y derechos**: `docs/EULA-CONTRATO-LICENCIA.md`  
- **Uso**: `docs/MANUAL-USUARIO.md`  
- **Instalación**: `docs/MANUAL-INSTALACION-CONFIGURACION.md`  
- **Librerías y componentes**: `docs/LIBRERIAS-COMPONENTES.md`  
- **Acceso, claves y tokens de licencia**: `docs/DERECHOS-ACCESO-CLAVES.md`  
- **Planos y visión v01**: `docs/PLANO-PLATAFORMA-01.md`  
- **Reportes**: `REPORTE-COMPLETO-PLATAFORMA.md`, `REPORTE-EJECUTIVO-COMPLETO-2026.md`

---

```
Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister  
Ierahkwa Sovereign Platform v01 — Documentación Técnica
```
