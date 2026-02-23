# Manual de Instalación y Configuración — Ierahkwa Sovereign Platform v01

**Pasos para instalar y configurar el software**  
Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## 1. Requisitos previos

### 1.1 Software necesario

| Componente | Versión mínima |
|------------|----------------|
| **Node.js** | 18.x LTS (recomendado 20.x) |
| **npm** | 9.x o superior (incluido con Node) |
| **Git** | Cualquier versión reciente (opcional, para clonar) |
| **Base de datos** | SQLite (incluido) o PostgreSQL (producción) |

### 1.2 Para módulos .NET (SmartSchool, InventoryManager, TradeX)

| Componente | Versión |
|------------|---------|
| **.NET SDK** | 8.0 o superior (según .csproj) |

### 1.3 Sistema operativo

- Windows 10/11, macOS 10.15+, Linux (Ubuntu 20.04+, o equivalentes).

---

## 2. Obtención del software

### 2.1 Estructura de entrega

Recibirá (o dispondrá en el repositorio):

```
soberanos-natives/
├── platform/              # Portal principal
├── ierahkwa-shop/         # E-Commerce, POS, Chat, Inventario
├── pos-system/            # POS independiente (alternativo)
├── node/                  # Ierahkwa Futurehead Mamey Node
├── image-upload/          # Servicio de subida de imágenes
├── SmartSchool/           # Sistema educativo (.NET)
├── smart-school-node/     # API Node para SmartSchool
├── InventoryManager/      # Inventario desktop (.NET)
├── TradeX/                # Trading (.NET)
├── forex-trading-server/  # Servidor Forex
├── inventory-system/      # Sistema inventario (Node)
├── platform-services.json
├── PLATAFORMA-UNIFICADA.json
├── docs/                  # Documentación (EULA, manuales, etc.)
└── tokens/                # Especificaciones de tokens IGT
```

### 2.2 Verificación

- Compruebe que existe la carpeta `docs/` con: EULA, Manual de Usuario, Manual de Instalación, Documentación Técnica, Certificado de Licencia, etc.  
- Verifique que dispone de **Clave/Serial/Token** si su licencia lo requiere (ver `docs/DERECHOS-ACCESO-CLAVES.md`).

---

## 3. Instalación paso a paso

### 3.1 Portal y configuración base

1. Descomprimir o clonar en una ruta, por ejemplo:  
   `C:\ierahkwa\` o `/opt/ierahkwa/`  
2. El **Portal** (`platform/index.html`) es estático: abrirlo en el navegador. Para que las tarjetas de servicios carguen datos, `platform-services.json` debe estar en la ruta relativa correcta (p. ej. `../platform-services.json` respecto a `platform/index.html`).  
3. Si usa un servidor web (nginx, Apache), configurar el document root sobre la raíz del proyecto o servir `platform/` y el JSON según la estructura.

### 3.2 Ierahkwa Futurehead Shop (E-Commerce + POS + Chat + Inventario)

```bash
cd ierahkwa-shop
npm install
```

- **Variables de entorno** (opcional): copiar `env.example` a `.env` y ajustar:
  - `PORT=3100`
  - `NODE_ENV=development` o `production`
  - `DB_PATH` o `DATABASE_URL` si usa PostgreSQL

- **Base de datos**:
  - SQLite: suele crearse automáticamente. Si hay script:
    ```bash
    node scripts/db/init.js
    node scripts/db/seed.js
    ```
  - PostgreSQL: configurar `DATABASE_URL` y ejecutar `scripts/db/schema.sql` si se proporciona.

- **Iniciar**:
  ```bash
  npm start
  ```
  Por defecto: http://localhost:3100  
  - Shop: http://localhost:3100  
  - Admin: http://localhost:3100/admin  
  - POS: http://localhost:3100/pos  
  - Chat: http://localhost:3100/chat  
  - Inventario: http://localhost:3100/inventory  

### 3.3 Ierahkwa Futurehead Mamey Node (Blockchain)

```bash
cd node
npm install
npm start
```

- Por defecto: http://localhost:8545  
- RPC: http://localhost:8545/rpc  
- API REST: http://localhost:8545/api/v1  
- Ajustes en `config.toml` o `package.json` según la distribución.

### 3.4 Image Upload

```bash
cd image-upload
npm install
npm start
```

- Por defecto: http://localhost:3500 (verificar en `package.json` o `server.js`).  
- La carpeta `public/uploads/` debe existir y ser escribible.

### 3.5 POS System (independiente, si se usa)

```bash
cd pos-system
npm install
npm start
```

- Base de datos en `data/database.json` (JSON).  
- Puerto por defecto: 3000 (revisar `package.json`/`server.js`).

### 3.6 SmartSchool (Node API)

```bash
cd smart-school-node
npm install
npm start
```

- Revisar `package.json` para puerto y variables de entorno.  
- Si usa base de datos, configurar según `src/config/` y seeders.

### 3.7 SmartSchool / InventoryManager / TradeX (.NET)

- Abrir la solución (`.sln`) en Visual Studio o Rider.  
- Restaurar paquetes NuGet.  
- Configurar connection strings en `appsettings.json` o variables de entorno.  
- Compilar y ejecutar (F5 o `dotnet run`).

Ejemplo InventoryManager:

```bash
cd InventoryManager
dotnet restore
dotnet build
dotnet run
```

### 3.8 Forex Trading Server

```bash
cd forex-trading-server
npm install
npm start
```

- Revisar puerto y configuración en `server.js` y `package.json`.

---

## 4. Configuración

### 4.1 platform-services.json

Define los servicios que muestra el Portal:

- `services`: id, name, description, domain, localPort, localPath, url, token, category, features, status.  
- `categories`: nombre, icono, color.  
- `tokens`: total, governmentTokens, utilityTokens, masterToken.

Ajuste `localPort` y `localPath` si sus instancias usan otros puertos o rutas.

### 4.2 Variables de entorno habituales

| Variable | Uso | Ejemplo |
|----------|-----|---------|
| `PORT` | Puerto del servidor | `3100` |
| `NODE_ENV` | Entorno | `development` o `production` |
| `DATABASE_URL` | PostgreSQL | `postgresql://user:pass@host:5432/db` |
| `DB_PATH` | Ruta SQLite | `./data/shop.sqlite` |
| `JWT_SECRET` | Clave para JWT | Cadena segura aleatoria |
| `SESSION_SECRET` | Sesiones | Cadena segura aleatoria |
| `RPC_URL` | RPC del Node | `http://localhost:8545` |

### 4.3 Base de datos

- **Desarrollo**: SQLite suele bastar. Asegurar permisos de escritura en la carpeta de la base de datos.  
- **Producción**: se recomienda PostgreSQL. Ejecutar migraciones o `schema.sql` y, si existe, `seed.js` o equivalentes para datos iniciales (admin, etc.).

### 4.4 Primer usuario administrador

- Si hay seed: el usuario por defecto suele ser `admin@ierahkwa.gov` / `admin123`.  
- **Cambiar la contraseña** inmediatamente en producción.  
- Si no hay seed, revisar la documentación del módulo o `scripts/db/` para crear el primer admin.

---

## 5. Despliegue en producción (resumen)

1. **Servidor**: Linux (Ubuntu 22.04 LTS o similar).  
2. **Node.js**: instalar v20 LTS (por ejemplo con `nvm` o paquetes oficiales).  
3. **Proceso persistente**: usar **PM2** o systemd:
   - Ejemplo PM2:
     ```bash
     pm2 start ierahkwa-shop/server.js --name shop
     pm2 start node/... --name node
     pm2 save && pm2 startup
     ```
4. **Reverse proxy**: Nginx o Apache delante de los Node (puertos 3100, 8545, 3500, etc.), con HTTPS (TLS) y, si aplica, dominio (p. ej. `shop.ierahkwa.gov`).  
5. **Base de datos**: PostgreSQL en servidor dedicado o gestionado; backups automáticos.  
6. **Firewall**: abrir solo 80/443 (y 8545 si se expone el Node) según necesidad.  
7. **Logs y monitoreo**: redirigir logs de PM2/systemd; considerar Herramientas de monitoreo (ver Documentación Técnica).

---

## 6. Verificación post-instalación

| Comprobación | Cómo |
|--------------|------|
| Portal | Abrir `platform/index.html` y que se listen los servicios desde `platform-services.json`. |
| Shop | http://localhost:3100 y login en /admin. |
| POS | http://localhost:3100/pos (o 3000 si usa pos-system). |
| Node | http://localhost:8545 y /api/v1/stats o /rpc. |
| Chat | http://localhost:3100/chat, enviar mensaje de prueba. |
| Inventario | http://localhost:3100/inventory, listar productos/almacenes. |

---

## 7. Actualizaciones

1. Hacer **backup** de bases de datos y archivos de configuración.  
2. Revisar **changelog** o notas de la versión.  
3. Sustituir archivos del código (excepto `.env`, `data/`, `uploads/`, y configuraciones localmente modificadas).  
4. Ejecutar `npm install` (o `dotnet restore`) si cambian dependencias.  
5. Ejecutar migraciones de BD si se indican.  
6. Reiniciar servicios (PM2, systemd, etc.).  
7. Comprobar de nuevo la verificación post-instalación.

---

## 8. Solución de problemas de instalación

| Error | Posibles causas | Acciones |
|-------|-----------------|----------|
| `EADDRINUSE` | Puerto en uso | Cambiar `PORT` o detener el proceso que usa el puerto. |
| `MODULE_NOT_FOUND` | Dependencias | `npm install` o `rm -rf node_modules && npm install`. |
| Error de SQLite | Permisos o ruta | Comprobar permisos de la carpeta y que la ruta en `DB_PATH` exista. |
| Error de PostgreSQL | Connection string o servicio | Verificar `DATABASE_URL`, que Postgres esté en ejecución y que la BD exista. |
| Node no responde en 8545 | Node no iniciado o config | Revisar `node/config.toml`, `package.json` y logs. |
| 404 en rutas del Shop | Proxy o base path | Revisar configuración de nginx/Apache y `localPath` en `platform-services.json`. |

Para detalles de APIs, arquitectura y operación, ver **`docs/DOCUMENTACION-TECNICA.md`**.

---

```
Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister  
Ierahkwa Sovereign Platform v01 — Manual de Instalación y Configuración
```
