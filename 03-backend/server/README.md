# Ierahkwa Server — Servidor Soberano con Ghost Mode

> Servidor financiero soberano con modo fantasma, servidores phantom, encriptacion AES-256-GCM y fortaleza de seguridad.

## Descripcion

Ierahkwa Server es el servidor base/legacy de la plataforma financiera soberana. Implementa una arquitectura de seguridad avanzada denominada "Ghost Mode" (Modo Fantasma) que utiliza 7 servidores phantom secretos con rotacion automatica impredecible, combinados con 35 servidores decoy (senuelos) que confunden a atacantes potenciales.

El servidor esta construido sobre HTTP nativo de Node.js (sin Express) para maximo control y minima superficie de ataque. Incluye un sistema completo de encriptacion AES-256-GCM con derivacion de claves PBKDF2, un almacen de datos encriptado con backups automaticos, un sistema de auditoria forense que registra cada evento de seguridad, y rate limiting por IP con bloqueo automatico.

Los 35 servidores decoy estan configurados para aparentar ser infraestructura real distribuida globalmente, incluyendo honeypots que imitan paneles de administracion, accesos SSH, bases de datos abiertas y billeteras cripto -- todos disenados como trampas para atacantes.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: HTTP nativo (sin dependencias externas)
- **Encriptacion**: AES-256-GCM, PBKDF2 (100,000 iteraciones, SHA-512)
- **Almacenamiento**: Sistema de archivos encriptado (.enc)
- **Puerto**: 3000 (configurable via `PORT`)

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /api/status | Estado del sistema con Ghost Mode y uptime |
| GET | /api/ghost/public | Informacion publica de servidores (solo decoys) |
| POST | /api/ghost/secret | Informacion secreta de phantoms (requiere adminKey) |
| POST | /api/ghost/rotate | Forzar rotacion de servidor phantom |
| POST | /api/data/save | Guardar datos encriptados con AES-256-GCM |
| POST | /api/data/load | Cargar datos encriptados |
| GET | /api/audit | Obtener logs de auditoria (param: count) |
| GET | /api/backups | Listar backups disponibles |
| * | /* | Servidor de archivos estaticos desde /platform |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servidor | 3000 |

## Instalacion

```bash
npm install
npm start
```

Para iniciar con Ghost Mode explicito:

```bash
npm run ghost
```

## Docker

```bash
docker build -t ierahkwa-server .
docker run -p 3000:3000 ierahkwa-server
```

## Modulos de Seguridad

### Ghost Mode (Modo Fantasma)
- **7 servidores phantom** con IDs secretos que rotan automaticamente
- **Rotacion impredecible**: intervalos aleatorios entre 15 segundos y 20 minutos con variacion adicional de +-25%
- **Clave de rotacion** regenerada en cada cambio

### Servidores Decoy (35 Senuelos)
- **3 servidores principales** (New York, Los Angeles, London)
- **3 bases de datos** (Frankfurt, Paris, Amsterdam)
- **3 APIs** (Singapore, Tokyo, Sydney)
- **3 cache** (Mumbai, Dubai, Sao Paulo)
- **3 seguridad** (Toronto, Mexico City, Buenos Aires)
- **8 honeypots** (paneles admin, SSH, FTP, MySQL, Redis, credenciales, claves privadas, acceso root)
- **3 dev/staging** (Portland, Austin, Phoenix)
- **3 backups** (Zurich, Geneva, Stockholm)
- **4 financieros** (Hong Kong, Cayman Islands, Luxembourg, Malta)

### Rate Limiter
- 100 peticiones por minuto por IP
- Bloqueo automatico de 5 minutos al exceder limite

### Encriptacion
- AES-256-GCM con sal de 64 bytes
- Derivacion de clave PBKDF2 con 100,000 iteraciones SHA-512
- Tags de autenticacion de 16 bytes

## Arquitectura

```
Internet --> [Rate Limiter] --> [Security Headers]
                                      |
                            +----+----+----+
                            |         |         |
                        API Routes  Static   Ghost Mode
                            |       Files     |     |
                     +------+------+    7 Phantoms  35 Decoys
                     |      |      |
                  Ghost  Data   Audit
                  Mode   Store  Log
                     |      |
                  Rotate  Encrypt --> [.enc Files]
                                      [Backups]
```

## Parte de

**Ierahkwa Ne Kanienke** -- Plataforma Soberana Digital
