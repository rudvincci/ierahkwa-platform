# SoberanoServicios — Sistema de Reservas

> Plataforma de reservas de servicios con 31 categorias, seguimiento en tiempo real y 92% de ingresos para proveedores.

## Descripcion

SoberanoServicios es el backend del sistema de reservas y servicios a domicilio de la Red Soberana. Permite a los usuarios reservar una amplia gama de servicios profesionales organizados en 31 categorias que abarcan belleza y cuidado personal, hogar y reparaciones, automotriz, educacion, comida, salud, eventos, delivery, tecnologia y mascotas.

El servicio opera con un modelo economico justo donde el 92% del pago va directamente al proveedor, con una comision de plataforma de solo 8% y 0% de impuestos. Implementa un flujo completo de reservas con estados (pending, confirmed, in-progress, completed, reviewed) y un sistema de escrow que retiene fondos hasta que el servicio se completa satisfactoriamente.

La comunicacion en tiempo real se realiza via WebSocket, permitiendo actualizaciones de estado de reservas y seguimiento de ubicacion de proveedores. El servicio soporta 43 idiomas y ofrece tanto servicios a domicilio como en establecimiento.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **WebSocket**: ws 8.x
- **Almacenamiento**: In-memory Store (Map) + modulos de rutas
- **Puerto**: 4010 (configurable via `SERVICIOS_PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con estadisticas del store |
| GET | /v1/stats | Estadisticas generales del servicio |
| GET | /v1/categories | Listar las 31 categorias de servicios |
| GET | /v1/categories/:id | Detalle de una categoria |
| GET | /v1/providers | Listar proveedores registrados |
| POST | /v1/providers | Registrar nuevo proveedor |
| GET | /v1/services | Listar servicios disponibles |
| POST | /v1/bookings | Crear nueva reserva (escrow automatico) |
| GET | /v1/bookings/mine | Mis reservas (como cliente o proveedor) |
| GET | /v1/bookings/:id | Detalle de una reserva |
| POST | /v1/bookings/:id/confirm | Proveedor confirma la reserva |
| POST | /v1/bookings/:id/start | Iniciar servicio |
| POST | /v1/bookings/:id/complete | Completar servicio (libera pago al proveedor) |
| POST | /v1/bookings/:id/cancel | Cancelar reserva (reembolso automatico) |
| PUT | /v1/bookings/:id/reschedule | Reprogramar reserva |
| GET | /v1/reviews | Listar resenas |
| POST | /v1/reviews | Crear resena |
| GET | /v1/availability | Consultar disponibilidad de proveedores |
| GET | /v1/locations | Consultar ubicaciones de proveedores |
| WS | /ws | WebSocket para actualizaciones de reservas en tiempo real |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| SERVICIOS_PORT | Puerto del servicio | 4010 |

## Instalacion

```bash
npm install
npm start
```

## Docker

```bash
docker build -t reservas .
docker run -p 3005:4010 reservas
```

## Categorias de Servicios (31)

**Belleza y Cuidado Personal**: Barberia, Estilista, Tatuajes, Piercing, Unas, Maquillaje, Masajes, Spa, Depilacion

**Hogar y Reparaciones**: Plomero, Electricista, Carpintero, Pintor, Cerrajero, Limpieza, Aire Acondicionado, Electrodomesticos

**Automotriz**: Mecanico, Lavado de Autos, Grua

**Educacion y Profesional**: Tutor, Profesor de Idiomas, Contador, Abogado

**Comida**: Chef Privado, Catering

**Salud y Bienestar**: Entrenador Personal, Curandero (medicina ancestral), Partera

**Eventos y Creativos**: Fotografo, Musico, DJ, Organizador de Eventos

**Delivery y Transporte**: Delivery/Mandados, Mudanzas

**Tecnologia**: Soporte Tecnico, Desarrollador Web

**Mascotas**: Veterinario, Peluqueria Canina, Cuidador de Mascotas

## Arquitectura

El servicio se organiza en capas modulares:

- **server.js** -- Servidor Express + WebSocket, inyecta el Store en cada router
- **lib/store.js** -- Store central en memoria con Maps para proveedores, servicios, reservas
- **routes/** -- 7 routers separados: providers, services, bookings, reviews, categories, availability, locations
- **services/realtime.js** -- Modulo WebSocket para actualizaciones en tiempo real

```
Clientes HTTP/WS --> [SoberanoServicios :4010]
                          |
               +----------+----------+----------+
               |          |          |          |
          Providers   Bookings   Reviews   Categories
               |          |
            [Store]    [Escrow]
               |          |
          Availability  Realtime WS
```

**Flujo de Reserva:**
1. Cliente crea booking --> estado `pending`, fondos en escrow
2. Proveedor confirma --> estado `confirmed`
3. Proveedor inicia servicio --> estado `in-progress`
4. Proveedor completa --> estado `completed`, pago liberado (92% al proveedor)
5. Cliente puede cancelar antes de inicio --> reembolso automatico

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
