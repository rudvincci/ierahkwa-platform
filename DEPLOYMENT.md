# Deployment Guide - Ierahkwa Sovereign Platform

This guide covers deploying the Ierahkwa Sovereign Platform across development, staging, and production environments.

## Prerequisites

Ensure the following tools are installed and available:

| Tool | Version | Purpose |
|------|---------|---------|
| Docker | 27+ | Container runtime |
| Docker Compose | 2.20+ | Multi-container orchestration |
| Kubernetes (kubectl) | 1.29+ | Production orchestration |
| Terraform | 1.7+ | Infrastructure as code |
| Node.js | 22 LTS | Node.js services runtime |
| .NET SDK | 10 | .NET microservices runtime |
| Rust | 1.80+ | MameyNode / MameyForge |
| Go | 1.22+ | Bridge services and SDK |
| Python | 3.11+ | AI/ML services and scripts |
| PostgreSQL | 16 | Primary database |
| Redis | 7 | Caching and message broker |
| Nginx | 1.25+ | Reverse proxy |
| Certbot | latest | SSL certificate management |

## Development Setup

Development uses `docker-compose.dev.yml` for a lightweight local environment.

```bash
# Clone the repository
git clone https://github.com/rudvincci/ierahkwa-platform.git
cd ierahkwa-platform

# Copy environment configuration
cp .env.example .env

# Edit .env with your local settings
# Ensure DATABASE_URL, REDIS_URL, and service ports are configured

# Start all services in development mode
docker compose -f docker-compose.dev.yml up -d

# Verify services are running
docker compose -f docker-compose.dev.yml ps

# View logs
docker compose -f docker-compose.dev.yml logs -f

# Run database migrations
docker compose -f docker-compose.dev.yml exec gateway npm run migrate

# Stop services
docker compose -f docker-compose.dev.yml down
```

### Development Ports

| Service | Port |
|---------|------|
| Gateway API | 3000 |
| Identity Service | 5001 |
| ZKP Service | 5002 |
| Treasury Service | 5003 |
| MameyNode RPC | 8545 |
| Voz Soberana | 3001 |
| BDET Bank | 3002 |
| Atabey Translator | 3003 |
| PostgreSQL | 5432 |
| Redis | 6379 |

## Staging Setup

Staging uses `docker-compose.sovereign.yml` for a production-like environment.

```bash
# Use staging environment variables
cp .env.staging .env

# Start staging environment
docker compose -f docker-compose.sovereign.yml up -d

# Run health checks
curl http://localhost:3000/health
curl http://localhost:5001/health
curl http://localhost:8545/health

# Run integration tests against staging
npm run test:integration
```

## Production Setup

Production deployments use Kubernetes orchestrated by Terraform for infrastructure provisioning.

### 1. Infrastructure Provisioning with Terraform

```bash
# Navigate to infrastructure directory
cd infra/terraform

# Initialize Terraform
terraform init

# Review the execution plan
terraform plan -var-file=production.tfvars

# Apply infrastructure changes
terraform apply -var-file=production.tfvars

# Output will include cluster endpoint, database URLs, etc.
terraform output
```

### 2. Deploy to Kubernetes

```bash
# Configure kubectl to use the production cluster
export KUBECONFIG=~/.kube/config-ierahkwa-prod

# Create the namespace
kubectl create namespace ierahkwa-prod

# Apply secrets and config maps
kubectl apply -f k8s/secrets/ -n ierahkwa-prod
kubectl apply -f k8s/configmaps/ -n ierahkwa-prod

# Deploy all services
kubectl apply -f k8s/deployments/ -n ierahkwa-prod

# Apply services and ingress
kubectl apply -f k8s/services/ -n ierahkwa-prod
kubectl apply -f k8s/ingress/ -n ierahkwa-prod

# Verify deployment status
kubectl get pods -n ierahkwa-prod
kubectl get services -n ierahkwa-prod

# Check rollout status for each deployment
kubectl rollout status deployment/gateway -n ierahkwa-prod
kubectl rollout status deployment/identity -n ierahkwa-prod
kubectl rollout status deployment/mameynode -n ierahkwa-prod
```

### 3. Configure Nginx

```bash
# Copy Nginx configuration
sudo cp infra/nginx/ierahkwa.conf /etc/nginx/sites-available/ierahkwa
sudo ln -s /etc/nginx/sites-available/ierahkwa /etc/nginx/sites-enabled/

# Test configuration
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

### 4. Setup SSL with Certbot

```bash
# Obtain SSL certificate
sudo certbot --nginx -d ierahkwa.sovereign.io -d api.ierahkwa.sovereign.io

# Verify auto-renewal
sudo certbot renew --dry-run

# Certificate renewal is handled automatically via cron/systemd timer
```

### 5. Configure DNS

Configure the following DNS records with your DNS provider:

| Type | Name | Value | TTL |
|------|------|-------|-----|
| A | ierahkwa.sovereign.io | `<load-balancer-ip>` | 300 |
| A | api.ierahkwa.sovereign.io | `<load-balancer-ip>` | 300 |
| CNAME | www.ierahkwa.sovereign.io | ierahkwa.sovereign.io | 300 |
| TXT | _acme-challenge.ierahkwa.sovereign.io | `<certbot-value>` | 300 |

## Monitoring

The platform uses Prometheus for metrics collection and Grafana for visualization.

### Prometheus Setup

```bash
# Deploy Prometheus
kubectl apply -f k8s/monitoring/prometheus/ -n ierahkwa-monitoring

# Verify Prometheus is scraping targets
kubectl port-forward svc/prometheus 9090:9090 -n ierahkwa-monitoring
# Visit http://localhost:9090/targets
```

### Grafana Setup

```bash
# Deploy Grafana
kubectl apply -f k8s/monitoring/grafana/ -n ierahkwa-monitoring

# Access Grafana dashboard
kubectl port-forward svc/grafana 3100:3000 -n ierahkwa-monitoring
# Visit http://localhost:3100 (default: admin/admin)
```

### Key Metrics to Monitor

- **Gateway**: Request rate, error rate, response latency (p50, p95, p99)
- **MameyNode**: Block height, transaction throughput, peer count
- **Identity**: Authentication success/failure rate, ZKP verification time
- **Treasury**: Token transfer volume, balance reconciliation
- **Database**: Connection pool usage, query latency, replication lag
- **Infrastructure**: CPU, memory, disk usage, network I/O

### Alerting Rules

Alerts are configured in `k8s/monitoring/prometheus/alerts.yml` for:

- Service health check failures
- High error rates (> 1% of requests)
- High latency (p99 > 5s)
- Low disk space (< 20% free)
- Database replication lag (> 30s)
- MameyNode block production stalls

## Backup Procedures

### Database Backups

```bash
# Manual backup
pg_dump -h <db-host> -U ierahkwa -d ierahkwa_prod > backup_$(date +%Y%m%d_%H%M%S).sql

# Automated backups run via CronJob in Kubernetes
kubectl get cronjobs -n ierahkwa-prod

# Verify latest backup
kubectl logs job/db-backup-<latest> -n ierahkwa-prod
```

### Blockchain State Backups

```bash
# Snapshot MameyNode data
kubectl exec -it deployment/mameynode -n ierahkwa-prod -- mameynode snapshot create

# Copy snapshot to backup storage
kubectl cp ierahkwa-prod/mameynode-0:/data/snapshots/latest.snap ./backups/
```

### Backup Schedule

| Component | Frequency | Retention |
|-----------|-----------|-----------|
| PostgreSQL full backup | Daily at 02:00 UTC | 30 days |
| PostgreSQL WAL archiving | Continuous | 7 days |
| MameyNode snapshots | Every 6 hours | 14 days |
| Redis RDB snapshots | Every 4 hours | 7 days |
| Configuration backups | On every deploy | 90 days |

## Rollback Procedures

### Application Rollback (Kubernetes)

```bash
# View deployment history
kubectl rollout history deployment/gateway -n ierahkwa-prod

# Rollback to previous version
kubectl rollout undo deployment/gateway -n ierahkwa-prod

# Rollback to a specific revision
kubectl rollout undo deployment/gateway --to-revision=3 -n ierahkwa-prod

# Verify rollback
kubectl rollout status deployment/gateway -n ierahkwa-prod
```

### Infrastructure Rollback (Terraform)

```bash
# View Terraform state history
cd infra/terraform

# Rollback to a previous state
terraform plan -target=<resource> -var-file=production.tfvars
terraform apply -target=<resource> -var-file=production.tfvars
```

### Database Rollback

```bash
# Restore from backup
psql -h <db-host> -U ierahkwa -d ierahkwa_prod < backup_YYYYMMDD_HHMMSS.sql

# Or use point-in-time recovery if WAL archiving is enabled
# Adjust recovery_target_time in postgresql.conf and restart
```

### Emergency Procedures

1. **Service outage**: Check pod status, restart affected deployments
2. **Database corruption**: Restore from latest backup, replay WAL logs
3. **Blockchain fork**: Restore from last known good snapshot
4. **Security breach**: Rotate all secrets, revoke compromised credentials, audit access logs

---

*Skennen -- Peace*
