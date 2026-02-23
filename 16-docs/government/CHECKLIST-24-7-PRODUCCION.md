# ✅ CHECKLIST: Operación 24/7 en Producción
## IERAHKWA Sovereign Platform - Lo que falta para 100% uptime

**Fecha:** 22 de enero, 2026  
**Estado Actual:** Desarrollo/Testing  
**Objetivo:** Producción 24/7 con 99.9%+ uptime

---

## 🔴 CRÍTICO - Requerido para producción

### 1. Process Management & Auto-Restart
- [x] PM2 configurado para algunos servicios (`platform/ecosystem.config.js`)
- [ ] **PM2 para TODOS los servicios Node.js** (server.js, banking-bridge.js, etc.)
- [ ] **systemd services para servicios .NET** (no solo scripts bash)
- [ ] **Auto-restart en crash** configurado en PM2/systemd
- [ ] **Graceful shutdown** handlers en todos los servicios
- [ ] **Health check endpoints** que PM2/systemd puedan monitorear
- [ ] **Max memory restart** configurado (evitar memory leaks)

**Archivos a crear:**
- `node/ecosystem.config.js` - PM2 para server.js y banking-bridge.js
- `platform-dotnet/*.service` - systemd units para servicios .NET
- `scripts/start-production.sh` - Script maestro que inicia todo con PM2/systemd

---

### 2. Database & Connection Management
- [ ] **Connection pooling** configurado (PostgreSQL, Redis)
- [ ] **Retry logic** para conexiones de DB
- [ ] **Connection timeout** y max connections configurados
- [ ] **Database migrations** automatizadas
- [ ] **Read replicas** para alta disponibilidad
- [ ] **Backup automático de DB** (no solo archivos)
- [ ] **Database health checks** en cada servicio

**Implementar:**
```csharp
// .NET - Connection pooling en appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=ierahkwa;Pooling=true;MinPoolSize=5;MaxPoolSize=100;Connection Lifetime=300"
}
```

```javascript
// Node.js - pg pool config
const pool = new Pool({
  max: 20,
  idleTimeoutMillis: 30000,
  connectionTimeoutMillis: 2000,
});
```

---

### 3. Logging Centralizado
- [ ] **Logging estructurado** (JSON format)
- [ ] **Log aggregation** (ELK Stack o similar)
- [ ] **Log rotation** automático
- [ ] **Error tracking** (Sentry, Rollbar, o similar)
- [ ] **Log levels** configurados (DEBUG, INFO, WARN, ERROR)
- [ ] **Request logging** middleware en todos los servicios
- [ ] **Performance logging** (slow queries, slow endpoints)

**Herramientas sugeridas:**
- Node.js: `winston` + `winston-daily-rotate-file` + ELK
- .NET: `Serilog` + `Serilog.Sinks.Elasticsearch`
- Error tracking: Sentry (gratis hasta cierto límite)

---

### 4. Monitoring & Alerting
- [ ] **Health check endpoints** en todos los servicios (`/health`, `/ready`, `/live`)
- [ ] **Metrics collection** (Prometheus o similar)
- [ ] **Dashboard de monitoreo** (Grafana)
- [ ] **Alertas automáticas** (PagerDuty, OpsGenie, o email/SMS)
- [ ] **Uptime monitoring** (UptimeRobot, Pingdom, o similar)
- [ ] **Resource monitoring** (CPU, RAM, Disk, Network)
- [ ] **Application Performance Monitoring (APM)** (New Relic, Datadog, o similar)

**Endpoints necesarios:**
```
GET /health - Health check básico
GET /ready - Readiness probe (Kubernetes)
GET /live - Liveness probe (Kubernetes)
GET /metrics - Prometheus metrics
```

---

### 5. SSL/TLS & Security
- [ ] **HTTPS habilitado** (no solo HTTP)
- [ ] **Certificados SSL** configurados (Let's Encrypt o comercial)
- [ ] **Auto-renewal de certificados** (certbot con cron)
- [ ] **Security headers** (HSTS, CSP, X-Frame-Options, etc.)
- [ ] **Rate limiting** global (por IP, por usuario)
- [ ] **DDoS protection** (Cloudflare o similar)
- [ ] **WAF (Web Application Firewall)** configurado
- [ ] **Secrets management** (HashiCorp Vault, AWS Secrets Manager, o .env seguro)

**Implementar:**
```nginx
# nginx.conf - SSL redirect
server {
    listen 80;
    server_name api.ierahkwa.gov;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    ssl_certificate /etc/letsencrypt/live/api.ierahkwa.gov/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/api.ierahkwa.gov/privkey.pem;
}
```

---

### 6. Backup & Disaster Recovery
- [x] Backup de archivos configurado (`backup-system/`)
- [ ] **Backup automático de bases de datos** (PostgreSQL, Redis)
- [ ] **Backup off-site** (S3, Google Cloud Storage, etc.)
- [ ] **Backup retention policy** (30 días, 90 días, 1 año)
- [ ] **Disaster recovery plan** documentado
- [ ] **Restore testing** periódico (mensual)
- [ ] **Point-in-time recovery** para bases de datos críticas

**Scripts necesarios:**
- `scripts/backup-database.sh` - Backup PostgreSQL
- `scripts/backup-redis.sh` - Backup Redis
- `scripts/restore-database.sh` - Restore desde backup
- `scripts/test-restore.sh` - Validar backups

---

### 7. Load Balancing & High Availability
- [ ] **Load balancer** configurado (nginx, HAProxy, o cloud LB)
- [ ] **Multiple instances** de cada servicio (no solo 1)
- [ ] **Session affinity** si es necesario
- [ ] **Health checks** en load balancer
- [ ] **Failover automático** si un servicio cae
- [ ] **Geographic distribution** (múltiples regiones)

**Configuración nginx:**
```nginx
upstream ierahkwa_api {
    least_conn;
    server localhost:3000 max_fails=3 fail_timeout=30s;
    server localhost:3001 max_fails=3 fail_timeout=30s;
    server localhost:3002 backup;
}
```

---

### 8. Environment Variables & Configuration
- [ ] **.env files** para cada ambiente (dev, staging, prod)
- [ ] **Secrets en variables de entorno** (no hardcoded)
- [ ] **Configuration validation** al iniciar servicios
- [ ] **Environment-specific configs** (appsettings.Production.json)
- [ ] **Secret rotation** plan

**Estructura:**
```
.env.development
.env.staging
.env.production
.env.production.local (gitignored, secrets)
```

---

### 9. Error Handling & Circuit Breakers
- [ ] **Global error handler** en todos los servicios
- [ ] **Circuit breakers** para llamadas externas (API, DB)
- [ ] **Retry logic** con exponential backoff
- [ ] **Timeout configurado** en todas las llamadas HTTP
- [ ] **Graceful degradation** cuando servicios externos fallan
- [ ] **Error notifications** automáticas

**Librerías:**
- Node.js: `opossum` (circuit breaker), `axios-retry`
- .NET: `Polly` (resilience patterns)

---

### 10. Rate Limiting & Throttling
- [ ] **Rate limiting global** (por IP)
- [ ] **Rate limiting por usuario** (autenticado)
- [ ] **Rate limiting por endpoint** (críticos más restrictivos)
- [ ] **DDoS protection** (Cloudflare, AWS Shield)
- [ ] **Request throttling** (max concurrent requests)

**Implementar:**
```javascript
// Node.js - express-rate-limit
const rateLimit = require('express-rate-limit');
const limiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutos
  max: 100 // 100 requests por IP
});
app.use('/api/', limiter);
```

---

## 🟡 IMPORTANTE - Mejora significativa de confiabilidad

### 11. CI/CD Pipeline
- [ ] **GitHub Actions / GitLab CI** configurado
- [ ] **Automated testing** (unit, integration, e2e)
- [ ] **Automated deployment** (staging, production)
- [ ] **Rollback automático** si deployment falla
- [ ] **Blue-green deployment** o canary releases
- [ ] **Database migrations** en pipeline

---

### 12. Containerization & Orchestration
- [x] Docker Compose existe (`docker-compose.yml`)
- [ ] **Dockerfiles optimizados** para producción
- [ ] **Multi-stage builds** (imágenes más pequeñas)
- [ ] **Kubernetes** o Docker Swarm para orquestación
- [ ] **Service mesh** (Istio) para comunicación entre servicios
- [ ] **Auto-scaling** basado en métricas

---

### 13. Database Optimization
- [ ] **Indexes** en tablas críticas
- [ ] **Query optimization** (slow query log analysis)
- [ ] **Connection pooling** optimizado
- [ ] **Read replicas** para queries de lectura
- [ ] **Database monitoring** (pg_stat_statements, etc.)
- [ ] **Vacuum/analyze** automático

---

### 14. Caching Strategy
- [ ] **Redis caching** para datos frecuentes
- [ ] **Cache invalidation** strategy
- [ ] **CDN** para assets estáticos
- [ ] **Browser caching** headers configurados
- [ ] **Cache warming** para datos críticos

---

### 15. Queue & Background Jobs
- [ ] **Message queue** para jobs asíncronos (BullMQ, RabbitMQ)
- [ ] **Job retry logic** con exponential backoff
- [ ] **Dead letter queue** para jobs fallidos
- [ ] **Job monitoring** dashboard
- [ ] **Priority queues** para jobs críticos

---

## 🟢 RECOMENDADO - Mejoras adicionales

### 16. Documentation
- [ ] **API documentation** (Swagger/OpenAPI)
- [ ] **Runbook** para operaciones comunes
- [ ] **Incident response playbook**
- [ ] **Architecture diagrams** actualizados
- [ ] **Deployment guide** paso a paso

---

### 17. Testing
- [ ] **Unit tests** (coverage > 80%)
- [ ] **Integration tests** para APIs críticas
- [ ] **E2E tests** para flujos principales
- [ ] **Load testing** (k6, Artillery, JMeter)
- [ ] **Chaos engineering** (Netflix Chaos Monkey)

---

### 18. Performance Optimization
- [ ] **Code profiling** (identificar bottlenecks)
- [ ] **Database query optimization**
- [ ] **API response compression** (gzip, brotli)
- [ ] **Image optimization** (WebP, lazy loading)
- [ ] **Bundle size optimization** (frontend)

---

## 📋 PRIORIDADES DE IMPLEMENTACIÓN

### Fase 1 (Semana 1-2) - CRÍTICO
1. ✅ Process management (PM2/systemd) para todos los servicios
2. ✅ Health checks en todos los servicios
3. ✅ Logging centralizado básico
4. ✅ Database connection pooling
5. ✅ Backup automático de bases de datos

### Fase 2 (Semana 3-4) - IMPORTANTE
6. ✅ SSL/TLS y HTTPS
7. ✅ Monitoring básico (Prometheus + Grafana)
8. ✅ Rate limiting
9. ✅ Error handling mejorado
10. ✅ Environment variables y secrets

### Fase 3 (Mes 2) - MEJORAS
11. ✅ Load balancing
12. ✅ CI/CD pipeline
13. ✅ Containerization completa
14. ✅ Caching strategy
15. ✅ Documentation completa

---

## 🚀 QUICK WINS (Implementar primero)

1. **PM2 para todos los servicios Node.js** (30 minutos)
2. **Health check endpoints** (1 hora)
3. **Logging básico con winston** (2 horas)
4. **Database connection pooling** (1 hora)
5. **Backup script de PostgreSQL** (1 hora)

**Total: ~6 horas de trabajo para mejoras críticas**

---

## 📝 NOTAS

- **Uptime objetivo:** 99.9% = máximo 8.76 horas de downtime por año
- **SLA típico:** 99.95% = máximo 4.38 horas de downtime por año
- **Monitoreo 24/7:** Necesario para detectar problemas antes de que afecten usuarios
- **Backup testing:** Crítico - un backup que no se puede restaurar es inútil

---

## 🔗 RECURSOS

- [PM2 Documentation](https://pm2.keymetrics.io/)
- [systemd Service Files](https://www.freedesktop.org/software/systemd/man/systemd.service.html)
- [Prometheus + Grafana](https://prometheus.io/docs/introduction/overview/)
- [ELK Stack](https://www.elastic.co/what-is/elk-stack)
- [Let's Encrypt](https://letsencrypt.org/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)

---

**Última actualización:** 22 de enero, 2026  
**Próxima revisión:** Después de implementar Fase 1
