# PM2 Installation & Setup Guide

## Instalación de PM2

PM2 no se pudo instalar automáticamente debido a problemas de red. Sigue estos pasos:

### Opción 1: Instalación Manual

```bash
# Instalar PM2 globalmente
npm install -g pm2

# Verificar instalación
pm2 --version
```

### Opción 2: Si hay problemas de red/proxy

```bash
# Configurar proxy si es necesario
npm config set proxy http://proxy-server:port
npm config set https-proxy http://proxy-server:port

# O usar registry alternativo
npm install -g pm2 --registry https://registry.npmmirror.com
```

### Opción 3: Instalación Local (sin -g)

```bash
cd node
npm install pm2 --save-dev
npx pm2 start ecosystem.config.js
```

---

## Uso de PM2

### Iniciar Servicios

```bash
# Desde el directorio node/
cd node
pm2 start ecosystem.config.js

# O usar el script de producción
./scripts/start-production.sh
```

### Comandos Útiles

```bash
# Ver estado de todos los procesos
pm2 list

# Ver logs en tiempo real
pm2 logs

# Ver logs de un proceso específico
pm2 logs ierahkwa-node-server

# Monitorear recursos (CPU, memoria)
pm2 monit

# Reiniciar todos los procesos
pm2 restart all

# Reiniciar un proceso específico
pm2 restart ierahkwa-node-server

# Detener todos los procesos
pm2 stop all

# Eliminar todos los procesos
pm2 delete all

# Guardar configuración actual
pm2 save

# Configurar PM2 para iniciar al boot (systemd)
pm2 startup
pm2 save
```

### Auto-restart al Reiniciar el Sistema

```bash
# Generar script de startup
pm2 startup

# Seguir las instrucciones que muestra (ejecutar el comando sudo que genera)
# Luego guardar la configuración
pm2 save
```

---

## Configuración en ecosystem.config.js

El archivo `node/ecosystem.config.js` está configurado con:

- **Auto-restart**: `autorestart: true`
- **Max memory restart**: `max_memory_restart: '2G'` (reinicia si usa más de 2GB)
- **Max restarts**: `max_restarts: 10` (máximo 10 reinicios en 1 minuto)
- **Cluster mode**: Para `server.js` (2 instancias)
- **Logs rotativos**: En `./logs/`

---

## Health Checks con PM2

PM2 puede monitorear health checks automáticamente:

```javascript
// En ecosystem.config.js
{
  wait_ready: true,
  listen_timeout: 10000,
  // PM2 esperará a que el servicio responda en /health
}
```

Los endpoints `/health`, `/ready`, y `/live` ya están implementados.

---

## Troubleshooting

### PM2 no inicia procesos

```bash
# Ver logs de PM2
pm2 logs --lines 100

# Verificar que los puertos no estén en uso
lsof -i :8545
lsof -i :3001
```

### Proceso se reinicia constantemente

```bash
# Ver logs de errores
pm2 logs ierahkwa-node-server --err

# Verificar configuración
pm2 describe ierahkwa-node-server
```

### Limpiar procesos huérfanos

```bash
pm2 kill
pm2 resurrect
```

---

## Próximos Pasos

Una vez PM2 esté instalado:

1. Iniciar servicios: `./scripts/start-production.sh`
2. Verificar estado: `pm2 list`
3. Configurar auto-start: `pm2 startup && pm2 save`
4. Monitorear: `pm2 monit`
