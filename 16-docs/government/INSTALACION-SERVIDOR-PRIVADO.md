# Instalación en servidor privado (VPS / on‑premise)

Pasos para levantar IERAHKWA en un VPS o servidor propio (Ubuntu/Debian recomendado).

---

## 1. Requisitos

- **Node.js 20** LTS
- **.NET 8** (opcional; solo si usas Banking API .NET)
- **PM2** (recomendado para producción)

---

## 2. Instalación rápida

```bash
git clone <tu-repo> ierahkwa && cd ierahkwa
chmod +x install-private-server.sh start-all.sh scripts/test-live.sh   # si hace falta
./install-private-server.sh
```

Esto instala Node, dependencias npm (node + platform), crea `.env` si no existe, y PM2.

---

## 3. Arrancar todos los servicios

```bash
./start-all.sh
```

Inicia:

| Servicio        | Puerto | Endpoints típicos       |
|-----------------|--------|--------------------------|
| Node Mamey      | 8545   | `/health`, `/ready`      |
| Banking Bridge  | 3001   | `/api/health`, `/api/ready`, `/api/status` |
| Banking .NET    | 5000   | `/health`, `/swagger`    |
| Platform        | 8080   | `/health`, `/index.html` |

Si existe `node/ecosystem.config.js` y PM2, Node y Banking Bridge se arrancan con PM2. Banking .NET y Platform se lanzan en background.

---

## 4. Verificar (smoke test)

```bash
./scripts/test-live.sh
```

Comprueba `/health`, `/api/health`, etc. en localhost. Para probar en otro host:

```bash
NODE_URL=http://tu-servidor:8545 \
BRIDGE_URL=http://tu-servidor:3001 \
BANKING_URL=http://tu-servidor:5000 \
PLATFORM_URL=http://tu-servidor:8080 \
./scripts/test-live.sh
```

---

## 5. systemd (opcional, 24/7)

```bash
sudo bash systemd/install-services.sh
sudo systemctl start ierahkwa-node
sudo systemctl start ierahkwa-banking
sudo systemctl start ierahkwa-tradex
```

Ajusta los `.service` en `systemd/` para que apunten a `banking-bridge` y `platform` si quieres gestionarlos también por systemd.

---

## 6. Variables de entorno

Crea o edita `.env` en la raíz (o `node/.env`). Algunas útiles:

- `JWT_SECRET` – Clave para JWT (mín. 32 caracteres).
- `BANKING_API_URL` – URL del Banking .NET (p. ej. `http://localhost:5000`).
- `POSTGRES_PASSWORD`, `REDIS_PASSWORD` – Si usas Postgres/Redis (p. ej. con Docker).

---

## 7. Servicios extra (.NET)

Para TradeX, NET10, FarmFactory, etc.:

```bash
INCLUIR_EXTRA=1 ./start-all.sh
```

---

## 8. Docker en el servidor privado

```bash
docker compose -f deploy/docker-compose.cloud.yml --project-directory . up -d
./scripts/test-live.sh
```

El `--project-directory .` se usa cuando el compose está en `deploy/` y se ejecuta desde la raíz. Ver `DEPLOY-CLOUD.md` para más opciones en la nube.
