# 🚀 IERAHKWA Sovereign Platform - Deployment Guide

## Guía de Despliegue para Operación 24/7

**Versión:** 2.0  
**Fecha:** 22 de enero, 2026

---

## 📋 Requisitos Previos

### Hardware Mínimo (Producción)
- CPU: 8 cores
- RAM: 32 GB
- Almacenamiento: 500 GB SSD
- Red: 1 Gbps

### Software Requerido
- Docker 24.0+
- Docker Compose 2.20+
- Node.js 20 LTS
- .NET 10 SDK
- nginx (incluido en Docker)
- PostgreSQL 16 (incluido en Docker)
- Redis 7 (incluido en Docker)

---

## 🔧 Configuración Inicial

### 1. Clonar y Configurar

```bash
# Clonar repositorio
git clone https://github.com/ierahkwa/sovereign-platform.git
cd sovereign-platform

# Copiar archivo de configuración
cp .env.example .env

# Editar configuración (usar editor de preferencia)
nano .env
```

### 2. Configurar Variables de Entorno

Editar `.env` con valores reales:

```bash
# Generar secrets seguros
openssl rand -base64 32  # Para JWT_SECRET
openssl rand -base64 32  # Para JWT_REFRESH_SECRET
openssl rand -base64 32  # Para ENCRYPTION_KEY

# Generar passwords de base de datos
openssl rand -base64 16  # Para PG_PASSWORD
openssl rand -base64 16  # Para MONGO_PASSWORD
openssl rand -base64 16  # Para REDIS_PASSWORD
```

### 3. Configurar SSL/TLS

```bash
# Ejecutar script de configuración
./scripts/setup-ssl.sh

# Elegir opción:
# 1) Let's Encrypt con Docker (producción)
# 2) Let's Encrypt standalone
# 3) Self-signed (solo desarrollo)
```

---

## 🐳 Despliegue con Docker

### Desarrollo

```bash
# Iniciar solo servicios esenciales
docker-compose up -d postgres redis

# Iniciar aplicación Node.js localmente
cd node
npm install
npm run dev
```

### Producción

```bash
# Construir imágenes
docker-compose -f docker-compose.production.yml build

# Iniciar todos los servicios
docker-compose -f docker-compose.production.yml up -d

# Ver logs
docker-compose -f docker-compose.production.yml logs -f

# Ver estado
docker-compose -f docker-compose.production.yml ps
```

### Servicios Individuales

```bash
# Reiniciar servicio específico
docker-compose -f docker-compose.production.yml restart node-app

# Escalar servicio
docker-compose -f docker-compose.production.yml up -d --scale node-app=3

# Ver logs de servicio
docker-compose -f docker-compose.production.yml logs -f node-app
```

---

## 📦 Despliegue sin Docker

### 1. Instalar Dependencias

```bash
# Node.js
cd node
npm ci --production

# Plataforma
cd ../platform
npm ci --production
```

### 2. Configurar PM2

```bash
# Instalar PM2
npm install -g pm2

# Iniciar servicios
cd node
pm2 start ecosystem.config.js

# Configurar auto-start
pm2 startup
pm2 save
```

### 3. Configurar nginx

```bash
# Copiar configuración
sudo cp nginx/nginx.conf /etc/nginx/nginx.conf

# Verificar configuración
sudo nginx -t

# Reiniciar nginx
sudo systemctl restart nginx
```

### 4. Iniciar Servicios .NET

```bash
# Usar script de inicio
./start-all-services.sh

# O usar systemd (recomendado)
sudo cp services/*.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable ierahkwa-platform
sudo systemctl start ierahkwa-platform
```

---

## 🔐 Configuración de Seguridad

### Firewall

```bash
# UFW (Ubuntu)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 22/tcp  # SSH
sudo ufw enable

# Bloquear puertos internos
sudo ufw deny 5432  # PostgreSQL
sudo ufw deny 27017 # MongoDB
sudo ufw deny 6379  # Redis
```

### Hardening nginx

La configuración de nginx ya incluye:
- ✅ Security headers (HSTS, CSP, X-Frame-Options)
- ✅ Rate limiting
- ✅ SSL/TLS 1.2/1.3
- ✅ Gzip compression
- ✅ Connection limits

### Secrets Management

```bash
# Nunca commitear .env
echo ".env" >> .gitignore

# Usar HashiCorp Vault en producción (opcional)
vault kv put secret/ierahkwa PG_PASSWORD=xxx JWT_SECRET=xxx
```

---

## 📊 Monitoreo

### Acceder a Dashboards

| Servicio | URL | Credenciales |
|----------|-----|--------------|
| Grafana | http://localhost:3001 | admin / $GRAFANA_PASSWORD |
| Prometheus | http://localhost:9090 | - |
| Kibana | http://localhost:5601 | - |
| RabbitMQ | http://localhost:15672 | ierahkwa / $RABBITMQ_PASSWORD |

### Health Checks

```bash
# Verificar servicios
curl http://localhost:8545/health
curl http://localhost:8545/ready
curl http://localhost:8545/live
curl http://localhost:8545/metrics

# Verificar circuit breakers
curl http://localhost:8545/api/v1/circuit-breakers
```

### Alertas

Las alertas de Prometheus están configuradas en `monitoring/alerts/critical.yml`:
- ServiceDown (servicio caído)
- HighErrorRate (>5% errores)
- HighLatency (>2s p95)
- HighMemoryUsage (>90%)
- DatabaseDown
- SSLCertificateExpiring

---

## 💾 Backups

### Backup Manual

```bash
# Ejecutar backup
./scripts/backup-database.sh

# Ver backups
ls -la backups/database/
```

### Backup Automático

```bash
# Configurar cron
./scripts/setup-cron-backups.sh

# Verificar cron
crontab -l
```

### Restore

```bash
# PostgreSQL
gunzip -c backups/database/postgresql_ierahkwa_db_YYYYMMDD.sql.gz | psql -U ierahkwa_admin -d ierahkwa_db

# Redis
redis-cli SHUTDOWN SAVE
cp backups/database/redis_YYYYMMDD.rdb /var/lib/redis/dump.rdb
redis-server
```

---

## 🔄 Actualizaciones

### Zero-Downtime Update (Docker)

```bash
# Pull nuevas imágenes
docker-compose -f docker-compose.production.yml pull

# Actualizar con rolling update
docker-compose -f docker-compose.production.yml up -d --no-deps --scale node-app=3 node-app

# Esperar a que los nuevos contenedores estén healthy
sleep 30

# Reducir a número normal de réplicas
docker-compose -f docker-compose.production.yml up -d --no-deps --scale node-app=2 node-app
```

### Update con PM2

```bash
# Pull cambios
git pull origin main

# Instalar dependencias
cd node && npm ci

# Reload sin downtime
pm2 reload all

# Verificar
pm2 list
```

---

## 🆘 Troubleshooting

### Servicio no inicia

```bash
# Ver logs
docker-compose logs node-app
pm2 logs ierahkwa-node-server

# Verificar puertos
lsof -i :8545
netstat -tlnp | grep 8545
```

### Error de conexión a DB

```bash
# Verificar PostgreSQL
docker-compose exec postgres pg_isready

# Verificar Redis
docker-compose exec redis redis-cli ping

# Verificar conexión
psql -h localhost -U ierahkwa_admin -d ierahkwa_db
```

### Alto uso de memoria

```bash
# Ver uso de memoria
docker stats

# Reiniciar servicio problemático
docker-compose restart node-app

# PM2
pm2 restart ierahkwa-node-server
```

### Circuit Breaker abierto

```bash
# Ver estado
curl http://localhost:8545/api/v1/circuit-breakers

# Reset manual (si es necesario)
curl -X POST http://localhost:8545/api/v1/circuit-breakers/reset
```

---

## 📝 Checklist de Despliegue

### Pre-Deploy
- [ ] `.env` configurado con valores de producción
- [ ] Secrets generados de forma segura
- [ ] SSL/TLS configurado
- [ ] Backups verificados
- [ ] Firewall configurado

### Durante Deploy
- [ ] Base de datos migrada
- [ ] Servicios iniciados
- [ ] Health checks pasando
- [ ] SSL funcionando (curl -I https://domain)
- [ ] Logs sin errores críticos

### Post-Deploy
- [ ] Monitoreo activo
- [ ] Alertas configuradas
- [ ] Backup automático funcionando
- [ ] Documentación actualizada
- [ ] Team notificado

---

## 📞 Contacto

Para soporte técnico:
- **Email:** devops@ierahkwa.gov
- **Slack:** #platform-ops
- **On-call:** Ver PagerDuty

---

**Última actualización:** 22 de enero, 2026
