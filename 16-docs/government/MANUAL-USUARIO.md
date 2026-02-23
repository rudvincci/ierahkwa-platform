# Manual de Usuario — Ierahkwa Sovereign Platform v01

**Guía para usar el software**  
Sovereign Government of Ierahkwa Ne Kanienke • Office of the Prime Minister

---

## 1. Introducción

### 1.1 Qué es la Plataforma

**Ierahkwa Sovereign Platform v01** es un ecosistema unificado que integra:

- **Comercio**: E-Commerce (Shop), POS, Inventario, Marketplace  
- **Finanzas**: Exchange, Trading, Pay, Wallet  
- **Banca**: BDET Bank, IISB (Settlement)  
- **Negocio**: CRM  
- **Comunicación**: Chat  
- **Educación**: SmartSchool  
- **Blockchain**: Ierahkwa Futurehead Mamey Node, Ierahkwa Sovereign Blockchain (ISB)

### 1.2 Acceso rápido

| Servicio | URL local | URL producción |
|----------|-----------|----------------|
| Portal principal | `platform/index.html` | https://platform.ierahkwa.gov |
| Shop / E-Commerce | http://localhost:3100 | https://shop.ierahkwa.gov |
| Admin Shop | http://localhost:3100/admin | https://shop.ierahkwa.gov/admin |
| POS | http://localhost:3100/pos | https://pos.ierahkwa.gov |
| Inventario | http://localhost:3100/inventory | https://inventory.ierahkwa.gov |
| Chat | http://localhost:3100/chat | https://chat.ierahkwa.gov |
| Node / Blockchain | http://localhost:8545 | https://rpc.ierahkwa.gov |

### 1.3 Credenciales por defecto (solo desarrollo)

- **Email**: `admin@ierahkwa.gov`  
- **Password**: `admin123`

> **Seguridad**: Cambie la contraseña tras el primer acceso en entornos reales.

---

## 2. Portal de la Plataforma

### 2.1 Descripción

El **Portal** (`platform/index.html`) muestra:

- Estadísticas: servicios activos, tokens IGT, blockchain (ISB), banco (BDET)  
- Filtro por categoría: Commerce, Finance, Banking, Business, Communication, Education, Media  
- Tarjetas de cada servicio con: nombre, dominio, descripción, features, token IGT  
- Botones: **Local** (localhost) y **Open** (producción)

### 2.2 Cómo usarlo

1. Abra `platform/index.html` en el navegador.  
2. Use los botones de categoría para filtrar.  
3. En cada tarjeta, use **Local** para el entorno local o **Open** para producción.

---

## 3. Ierahkwa Futurehead Shop (E-Commerce)

### 3.1 Funciones principales

- **Productos**: crear, editar, variantes (color, talla, almacenamiento), categorías, marcas, impuestos, SKU/código de barras, imágenes, descuentos.  
- **Inventario**: stock en tiempo real, alertas de stock bajo, movimientos, múltiples almacenes.  
- **Pedidos**: ciclo de vida (Pendiente → Confirmado → Procesando → Enviado → Entregado), estado de pago, historial.  
- **Clientes**: base de datos, grupos (Regular, VIP, Mayorista), historial de pedidos.  
- **Pagos**: efectivo, tarjeta, transferencia, **IGT**, cupones.  
- **Idiomas**: EN, ES, FR, Kanien'kéha (MOH).

### 3.2 Panel de administración (`/admin`)

- Dashboard con estadísticas.  
- Informes de ventas (diario/mensual), inventario, logs de actividad.  
- Roles y permisos de usuario.  
- Gestión de productos, categorías, pedidos, clientes, configuración.

### 3.3 Uso básico (cliente)

1. Navegar por categorías y productos.  
2. Añadir al carrito.  
3. Ir a checkout, elegir envío y pago.  
4. Ver estado del pedido en "Mis pedidos" (si hay sesión).

---

## 4. POS (Point of Sale)

### 4.1 Funciones

- Gestión de mesas.  
- Artículos de menú.  
- Seguimiento de pedidos, cobro rápido.  
- Múltiples formas de pago e informes.

### 4.2 Uso típico

1. Seleccionar mesa.  
2. Añadir ítems al pedido.  
3. Aplicar descuentos o cupones si aplica.  
4. Cobrar (efectivo, tarjeta, IGT, etc.).  
5. Cerrar mesa o imprimir ticket.

---

## 5. Inventario

### 5.1 Funciones

- Productos, almacenes, proveedores.  
- Movimientos de stock, órdenes de compra, ajustes, transferencias.  
- Informes, alertas de stock bajo, valoración.

### 5.2 Uso básico

1. **Productos**: alta y edición con códigos, categorías, precios.  
2. **Almacenes**: definición de ubicaciones.  
3. **Movimientos**: entradas, salidas, transferencias, ajustes.  
4. **Informes**: valoración, rotación, stock bajo.

---

## 6. Chat

### 6.1 Funciones

- Mensajería en tiempo real (Socket.IO).  
- Salas/canales, indicador de escritura, estado en línea.  
- Historial de mensajes.  
- Soporte PWA (instalable).

### 6.2 Uso

1. Entrar con usuario (o anónimo según configuración).  
2. Elegir o crear sala.  
3. Enviar y recibir mensajes en tiempo real.

---

## 7. Node / Blockchain

### 7.1 Ierahkwa Futurehead Mamey Node

- **Dashboard**: http://localhost:8545  
- **RPC**: http://localhost:8545/rpc  
- **REST API**: http://localhost:8545/api/v1  
- **Tokens**: http://localhost:8545/api/v1/tokens  
- **Stats**: http://localhost:8545/api/v1/stats  

### 7.2 Uso para desarrolladores/usuarios avanzados

- Consultar bloques, transacciones, balances.  
- Enviar transacciones vía RPC o API REST.  
- Consultar tokens IGT (100 tokens: 40 gubernamentales + 60 utility).

---

## 8. SmartSchool (Educación)

### 8.1 Funciones

- Estudiantes, profesores, cursos, notas, asistencia.  
- Módulos: recepción, biblioteca, contabilidad, clases en línea, etc.

### 8.2 Uso

- Acceso según rol (admin, profesor, alumno, padre).  
- Gestión de clases, tareas, materiales, calificaciones desde el panel correspondiente.

---

## 9. Otros servicios (Finance, Banking)

- **Exchange**: trading de tokens/cripto, order book, datos de mercado.  
- **Trading**: escritorio de trading, gráficos, portfolio.  
- **Pay**: pasarela de pagos, facturas, suscripciones, API.  
- **Wallet**: multi-moneda, envío/recepción, historial.  
- **IISB**: settlement, transferencias, SWIFT, clearing.

La documentación específica de cada uno se detalla en **Documentación Técnica** y en los README de cada módulo.

---

## 10. Idiomas

La plataforma soporta:

- **en** — English  
- **es** — Español  
- **fr** — Français  
- **moh** — Kanien'kéha (Mohawk)

El selector de idioma suele estar en la cabecera o en ajustes de la aplicación.

---

## 11. Consejos de uso y mejores prácticas

1. **Contraseña**: cambiar la de admin tras la primera instalación.  
2. **Backups**: configurar respaldos de bases de datos y archivos según `MANUAL-INSTALACION-CONFIGURACION.md`.  
3. **Actualizaciones**: aplicar parches de seguridad y versiones recomendadas.  
4. **Soporte**: usar el canal definido en su tipo de licencia (ver `DERECHOS-ACCESO-CLAVES.md`).

---

## 12. Resolución de problemas frecuentes

| Problema | Posible causa | Acción |
|----------|---------------|--------|
| No carga el Shop | Servidor no iniciado | `cd ierahkwa-shop && npm start` |
| Error 404 en rutas | Ruta o puerto equivocado | Revisar `platform-services.json` y puertos |
| No conecta al Node | Node no arrancado | `cd node && npm start` |
| Error de base de datos | SQLite/Postgres no configurado | Revisar `src/db.js`, `.env`, `scripts/db/` |
| Login fallido | Credenciales o sesión | Verificar usuario/contraseña; borrar cookies/caché |

Para más detalle técnico, ver **`docs/DOCUMENTACION-TECNICA.md`** y **`docs/MANUAL-INSTALACION-CONFIGURACION.md`**.

---

```
Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister  
Ierahkwa Sovereign Platform v01 — Manual de Usuario
```
