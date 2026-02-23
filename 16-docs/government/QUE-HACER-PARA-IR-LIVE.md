# Qué tienes que hacer TÚ para ir live

**Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister**

---

## Resumen en 3 pasos

1. **Configurar `.env`** (una vez).
2. **Ejecutar verificación** (opcional pero recomendado).
3. **Ejecutar go-live** y listo.

---

## Paso 1 — Configurar entorno (una vez)

### 1.1 Crear o revisar `.env`

Desde la **raíz del proyecto**:

```bash
# Si no existe .env, se crea desde el ejemplo:
cp RuddieSolution/node/.env.example RuddieSolution/node/.env
```

Luego **edita** `RuddieSolution/node/.env` y asegúrate de:

| Variable | Qué hacer |
|----------|-----------|
| **JWT_ACCESS_SECRET** | Poner un valor de al menos 32 caracteres aleatorios. Nunca dejar el de ejemplo. |
| **JWT_REFRESH_SECRET** | Otro valor de al menos 32 caracteres, distinto al anterior. |
| **SOVEREIGN_MASTER_KEY** | Si no está, el script GO-LIVE puede generarla; o créala tú (64 caracteres hex). |
| **CORS_ORIGIN** | En producción real: tu dominio, ej. `https://app.ierahkwa.gov` |

Para generar secrets:

```bash
openssl rand -hex 32   # para JWT_ACCESS_SECRET, JWT_REFRESH_SECRET
openssl rand -hex 32   # para SOVEREIGN_MASTER_KEY si la quieres fija
```

### 1.2 Dependencias Node

```bash
cd RuddieSolution/node && npm install && cd ../..
```

---

## Paso 2 — Verificación (recomendado antes de live)

Desde la **raíz del proyecto**:

```bash
./scripts/verificar-100.sh
```

Comprueba que no haya fallos graves (links, .env, puertos, node_modules). Si algo falla, el script te indica qué revisar.

Opcional:

```bash
./scripts/pre-live-check.sh    # seguridad pre-live
./scripts/check-production-env.sh   # variables .env
```

---

## Paso 3 — Ir live

Desde la **raíz del proyecto**:

```bash
./GO-LIVE-PRODUCTION.sh
```

Ese script:

- Pone `NODE_ENV=production`
- Ejecuta verificaciones (verificar-100, pre-live, etc.)
- Añade a `.env` claves que falten (SOVEREIGN_MASTER_KEY, STORAGE_ENCRYPT_KEY, INTERNAL_SERVICE_TOKEN) si no están
- Mata procesos que usen los puertos 8545, 3001, 3002, 5000, 8080
- Arranca con **PM2** (si está instalado) o en background:
  - Node (8545)
  - Banking Bridge (3001)
  - Editor API (3002)
  - Banking .NET (5000) si hay `dotnet`
- Comprueba que los servicios respondan
- Muestra las URLs y **abre el navegador** en la plataforma

Cuando termine, el sistema está **live** en:

- **Plataforma:** http://localhost:8545/platform  
- **BDET / Central Banks / TradeX / Security Fortress / Health / Admin:** las URLs que imprime el script.

---

## Después de ir live (opcional)

### Backups y health cada 5 min

```bash
./scripts/install-cron-production.sh
```

Con dominio real:

```bash
BASE_URL=https://app.ierahkwa.gov ./scripts/install-cron-production.sh
```

### Comandos útiles

| Comando | Uso |
|---------|-----|
| `pm2 status` | Estado de los servicios |
| `pm2 logs` | Logs en tiempo real |
| `pm2 restart all` | Reiniciar todo |
| `./stop-all.sh` | Parar todo |

### Si vas a producción con dominio e HTTPS

1. **DNS:** que tu dominio (ej. `app.ierahkwa.gov`) apunte al servidor (IP o CNAME).
2. **SSL:** en el servidor, con nginx y dominio listo:
   ```bash
   sudo DOMAIN=app.ierahkwa.gov EMAIL=admin@ierahkwa.gov ./scripts/setup-ssl-certbot-nginx.sh
   ```
3. **CORS:** en `.env` poner `CORS_ORIGIN=https://app.ierahkwa.gov` (o el dominio que uses).

---

## Checklist rápido (todo en uno)

```bash
./scripts/verificar-100.sh   # links, env, config
./GO-LIVE-PRODUCTION.sh      # inicia todo
./scripts/install-cron-production.sh   # backups + health (opcional)
```

---

## Si algo falla

| Problema | Qué hacer |
|----------|-----------|
| "Permission denied" al ejecutar scripts | `chmod +x GO-LIVE-PRODUCTION.sh scripts/verificar-100.sh scripts/*.sh` |
| Puerto 8545 en uso | El script intenta liberarlo; si no, `lsof -i :8545` y matar el proceso, o reiniciar y volver a ejecutar GO-LIVE. |
| ".env no encontrado" | Ejecutar `cp RuddieSolution/node/.env.example RuddieSolution/node/.env` y configurar JWT y CORS (Paso 1). |
| Node no arranca | Revisar `RuddieSolution/node/logs/server.log` y que `npm install` se haya hecho en `RuddieSolution/node`. |
| Pre-Live Check falla | Ejecutar `./scripts/pre-live-check.sh` y corregir los puntos que indique antes de seguir. |

---

**Resumen:** Configuras `.env` (Paso 1), opcionalmente verificas (Paso 2), ejecutas `./GO-LIVE-PRODUCTION.sh` (Paso 3) y el sistema queda live. El resto es opcional (cron, SSL, dominio).

*Sovereign Government of Ierahkwa Ne Kanienke · Office of the Prime Minister*
