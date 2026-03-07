# Plataforma Principal — API Gateway

> Gateway central de la Red Soberana que enruta 35+ rutas hacia 19+ microservicios internos.

## Descripcion

La Plataforma Principal es el API Gateway central de la Red Soberana Digital. Actua como punto de entrada unico para todas las solicitudes de clientes, distribuyendo el trafico hacia los microservicios especializados del ecosistema Ierahkwa Ne Kanienke.

El gateway implementa proxy inverso mediante `http-proxy-middleware`, enrutando peticiones a servicios de banca (BDET Bank), redes sociales, salud, educacion, transporte, gobernanza, identidad digital, blockchain, machine learning, y mas. Cada ruta se mapea a un servicio Docker interno con su propio puerto, permitiendo escalado independiente y alta disponibilidad.

Ademas, incluye un SDK oficial (`index.js`) de MameyNode v4.2 que expone una interfaz JavaScript unificada para interactuar con la blockchain, el banco BDET, autenticacion, traduccion (Atabey) e inteligencia artificial (MameyAI).

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **Proxy**: http-proxy-middleware 3.x
- **Seguridad**: Helmet 7.x, CORS configurado
- **Puerto**: 3000 (configurable via `GATEWAY_PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con estado de todos los microservicios |
| GET | /v1/services-catalog | Catalogo completo de servicios y rutas registradas |
| * | /v1/wallet, /v1/payments, /v1/exchange, /v1/trading, /v1/remittance, /v1/escrow, /v1/loans, /v1/insurance, /v1/staking, /v1/treasury, /v1/fiscal | Proxy a BDET Bank (:4000) |
| * | /v1/feed, /v1/posts, /v1/stories, /v1/comments, /v1/likes, /v1/follow, /v1/profiles, /v1/groups, /v1/chat, /v1/notifications, /v1/live | Proxy a Social Media (:4001) |
| * | /v1/doctor | Proxy a Soberano Doctor (:4002) |
| * | /v1/education | Proxy a PupitreSoberano (:4003) |
| * | /v1/rides | Proxy a Soberano Uber (:4004) |
| * | /v1/food | Proxy a Soberano Eats (:4005) |
| * | /v1/vote | Proxy a Voto Soberano (:4006) |
| * | /v1/disputes | Proxy a Justicia Soberano (:4007) |
| * | /v1/census | Proxy a Censo Soberano (:4008) |
| * | /v1/identity | Proxy a Soberano ID (:4009) |
| * | /v1/services, /v1/bookings | Proxy a Soberano Servicios (:4010) |
| * | /v1/mail | Proxy a Correo Soberano (:4011) |
| * | /v1/search | Proxy a Busqueda Soberana (:4012) |
| * | /v1/maps | Proxy a Mapa Soberano (:4013) |
| * | /v1/cloud | Proxy a Nube Soberana (:4014) |
| * | /v1/farm | Proxy a Soberano Farm (:4015) |
| * | /v1/radio | Proxy a Radio Soberana (:4016) |
| * | /v1/cooperatives | Proxy a Cooperativa Soberana (:4017) |
| * | /v1/tourism | Proxy a Turismo Soberano (:4018) |
| * | /v1/freelance | Proxy a Soberano Freelance (:4019) |
| * | /v1/pos | Proxy a Soberano POS (:4020) |
| * | /v1/blocks, /v1/tokens, /v1/validators, /v1/governance | Proxy a Blockchain API (:3000) |
| * | /v1/ml, /v1/anomaly, /v1/trust | Proxy a Ierahkwa ML (:3092) |
| * | /v1/vigilancia | Proxy a Vigilancia Soberana (:3091) |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| GATEWAY_PORT | Puerto del API Gateway | 3000 |

## Instalacion

```bash
npm install
npm start
```

## Docker

```bash
docker build -t plataforma-principal .
docker run -p 3000:3001 plataforma-principal
```

## Arquitectura

El gateway utiliza un patron de Service Registry declarativo donde cada ruta se asocia a un servicio objetivo con nombre y URL. Las peticiones entrantes se proxean de forma transparente, con timeout de 30 segundos y manejo de errores 503 cuando un servicio no esta disponible. El endpoint `/health` realiza health checks en paralelo a todos los servicios unicos, reportando el estado general como `operational`, `degraded` o `down`.

```
Cliente --> [API Gateway :3000]
                |
                |--> BDET Bank (:4000)         -- 11 rutas financieras
                |--> Social Media (:4001)       -- 11 rutas sociales
                |--> Doctor (:4002)             -- Telemedicina
                |--> Educacion (:4003)          -- E-learning
                |--> Transporte (:4004-4005)    -- Rides + Food
                |--> Gobernanza (:4006-4008)    -- Voto, Justicia, Censo
                |--> Identidad (:4009)          -- DID soberano
                |--> Servicios (:4010)          -- Reservas
                |--> Comunicacion (:4011-4012)  -- Mail, Busqueda
                |--> Infraestructura (:4013-4020) -- Maps, Cloud, Farm, etc.
                |--> Blockchain (:3000)         -- MameyNode chain
                |--> ML/AI (:3092)              -- Machine Learning
                |--> SIEM (:3091)               -- Vigilancia
```

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
