# Sovereign Government of Ierahkwa Ne Kanienke -- Digital Platform

> **35+ sovereign platforms for 72 million indigenous people across the Americas. Zero taxes. Free schools. Free hospitals.**

> **35+ plataformas soberanas para 72 millones de personas indigenas en las Americas. Cero impuestos. Escuelas gratis. Hospitales gratis.**

[![CI/CD](https://img.shields.io/badge/CI%2FCD-passing-brightgreen)]()
[![License: Sovereign-1.0](https://img.shields.io/badge/License-Sovereign--1.0-gold.svg)](LICENSE)
[![Platforms](https://img.shields.io/badge/Platforms-35+-2dd4a8.svg)]()
[![MameyNode](https://img.shields.io/badge/MameyNode-v4.2-purple.svg)]()
[![Languages](https://img.shields.io/badge/Languages-43-9b6dff.svg)]()
[![Tax Rate](https://img.shields.io/badge/Tax_Rate-0%25-ff6b4a.svg)]()

---

## Overview / Resumen

**English:** The Ierahkwa Ne Kanienke Digital Platform is a complete sovereign technology ecosystem designed to serve 72 million indigenous people across 19 countries in the Americas. It replaces dependency on Big Tech with self-governed alternatives -- from social media and email to banking, voting, and education. All services run on MameyNode, a post-quantum blockchain with 12,847 TPS and Proof-of-Sovereignty consensus. Citizens pay zero taxes; platform transaction fees (5-12%) automatically fund free schools, hospitals, and infrastructure.

**Espanol:** La Plataforma Digital Ierahkwa Ne Kanienke es un ecosistema tecnologico soberano completo diseado para servir a 72 millones de personas indigenas en 19 paises de las Americas. Reemplaza la dependencia en Big Tech con alternativas autogobernadas -- desde redes sociales y correo hasta banca, votaciones y educacion. Todos los servicios corren sobre MameyNode, una blockchain post-cuantica con 12,847 TPS y consenso Proof-of-Sovereignty. Los ciudadanos pagan cero impuestos; las comisiones de plataforma (5-12%) financian automaticamente escuelas, hospitales e infraestructura gratuitos.

---

## Architecture / Arquitectura

```
                         Internet
                            |
                    +-------+-------+
                    |  Nginx Proxy  |  :80 / :443
                    |   (SSL/TLS)   |
                    +-------+-------+
                            |
          +-----------------+-----------------+
          |                 |                 |
    +-----+-----+    +-----+-----+    +------+------+
    |  .NET Core |    |  Node.js  |    | MameyNode   |
    |  Services  |    | Platforms |    | Blockchain  |
    +-----+-----+    +-----+-----+    +------+------+
    | :5001 ID   |    | :3001 Bank|    | :8545 RPC   |
    | :5002 ZKP  |    | :3002 Voz |    | Chain ID    |
    | :5003 Treas|    | :3003 Red |    |   777777    |
    +------------+    | :3004 Mail|    +------+------+
                      | :3005 Rsrv|           |
                      | :3006 Vote|    +------+------+
                      | :3007 Trad|    | Smart       |
                      +-----------+    | Contracts   |
                            |          | (Solidity)  |
          +-----------------+          +-------------+
          |                 |
    +-----+-----+    +-----+-----+
    | PostgreSQL |    |   Redis   |
    |   :5432    |    |   :6379   |
    +------------+    +-----------+

    +-------------+   +-------------+
    | Prometheus  |   |   Grafana   |
    |   :9090     |   |   :3000     |
    +-------------+   +-------------+

    All services bind to 127.0.0.1 (sovereign-net)
    All traffic encrypted with post-quantum algorithms
```

---

## Quick Start / Inicio Rapido

```bash
# 1. Clone the repository / Clonar el repositorio
git clone https://github.com/ierahkwa/sovereign-platform.git
cd sovereign-platform

# 2. Launch everything / Levantar todo
chmod +x scripts/levantar-todo.sh
bash scripts/levantar-todo.sh

# 3. Check status / Verificar estado
bash scripts/estado.sh
```

The launcher script will automatically:
- Check prerequisites (Docker, Docker Compose)
- Generate `.env` from `.env.example` with secure random passwords
- Build all Docker images
- Start 16 services
- Wait for health checks
- Open the Grafana dashboard at http://127.0.0.1:3000

---

## Services / Servicios

### Blockchain

| Service | Port | Description |
|---------|------|-------------|
| MameyNode | 8545 | Sovereign blockchain (12,847 TPS, post-quantum) |

### .NET Microservices

| Service | Port | Description |
|---------|------|-------------|
| Identity Service | 5001 | Self-sovereign identity (Face + Sovereign ID) |
| ZKP Service | 5002 | Zero-knowledge proof verification |
| Treasury Service | 5003 | Wampum (WMP) token treasury management |

### Node.js Platforms

| Service | Port | Description |
|---------|------|-------------|
| BDET Bank | 3001 | Decentralized banking and payments |
| Voz Soberana | 3002 | Microblogging (replaces Twitter/X) |
| Red Social | 3003 | Social network (replaces Facebook) |
| Correo Soberano | 3004 | Post-quantum encrypted email (replaces Gmail) |
| Reservas | 3005 | Booking and reservations system |
| Voto Soberano | 3006 | Blockchain-based voting |
| Trading | 3007 | Wampum token exchange and trading |

### Infrastructure

| Service | Port | Description |
|---------|------|-------------|
| Nginx Proxy | 80/443 | Reverse proxy with SSL termination |
| PostgreSQL 16 | 5432 | Primary relational database |
| Redis 7 | 6379 | Caching and session management |
| Prometheus | 9090 | Metrics collection and alerting |
| Grafana | 3000 | Monitoring dashboards |

---

## All Platforms (35+) / Todas las Plataformas

| # | Platform | Replaces | Category |
|---|----------|----------|----------|
| 1 | Correo Soberano | Gmail | Communication |
| 2 | Red Soberana | Facebook | Social |
| 3 | Busqueda Soberana | Google Search | Search |
| 4 | Canal Soberano | YouTube | Video |
| 5 | Musica Soberana | Spotify | Music |
| 6 | Hospedaje Soberano | Airbnb | Lodging |
| 7 | Artesania Soberana | Etsy | Artisan marketplace |
| 8 | Cortos Indigenas | TikTok | Short video |
| 9 | Comercio Soberano | Shopify | E-commerce |
| 10 | Invertir Soberano | Robinhood | Investments |
| 11 | Docs Soberanos | Google Docs | Documents |
| 12 | Mapa Soberano | Google Maps | Maps (no tracking) |
| 13 | Voz Soberana | Twitter/X | Microblogging |
| 14 | Trabajo Soberano | LinkedIn | Professional network |
| 15 | Renta Soberano | TaskRabbit | Workforce marketplace |
| 16 | BDET Bank | PayPal/Banks | Banking and payments |
| 17 | Sabiduria Soberana | Wikipedia | Encyclopedia |
| 18 | Universidad Soberana | Coursera | Education (FREE) |
| 19 | Noticia Soberana | Google News | News |
| 20 | Cloud Soberana | AWS/GCP | Sovereign cloud |
| 21 | Code Soberano | GitHub | Code hosting |
| 22 | Soberano Doctor | Teladoc | Telemedicine (FREE) |
| 23 | Pupitre Soberano | Khan Academy | K-12 Education (FREE) |
| 24 | Soberano Uber | Uber | Rides (95% to drivers) |
| 25 | Soberano Eats | DoorDash | Food delivery (90%) |
| 26 | Voto Soberano | N/A | Blockchain voting |
| 27 | Justicia Soberano | N/A | Dispute resolution |
| 28 | Censo Soberano | N/A | Census system |
| 29 | Soberano ID | N/A | Self-sovereign identity |
| 30 | Soberano Servicios | TaskRabbit | 30 service categories |
| 31 | Soberano Farm | John Deere | Agriculture AI |
| 32 | Radio Soberana | iHeartRadio | Community radio |
| 33 | Cooperativa Soberana | N/A | Cooperative management |
| 34 | Turismo Soberano | TripAdvisor | Cultural tourism |
| 35 | Soberano Freelance | Fiverr | Gig marketplace |
| 36 | Soberano POS | Square | Point of sale |

---

## Creator Revenue Model

| Platform | Creator Gets | Big Tech Equivalent |
|----------|-------------|---------------------|
| Video (Canal Soberano) | **92%** | YouTube: 55% |
| Music (Musica Soberana) | **90%** | Spotify: 30% |
| Artisan (Artesania Soberana) | **88%** | Etsy: 80% |
| Rides (Soberano Uber) | **95%** | Uber: 70% |
| Food (Soberano Eats) | **90%** | DoorDash: 70% |
| Services | **92%** | TaskRabbit: 70% |
| Freelance | **92%** | Fiverr: 80% |

---

## Zero Tax Model / Modelo Cero Impuestos

Citizens pay zero taxes. Platform fees (5-12%) auto-allocate:

| Service | Allocation |
|---------|------------|
| Education | 25% -- Free preschool through university |
| Healthcare | 25% -- Free doctors, medicines, hospitals |
| Infrastructure | 20% -- Internet, water, electricity, roads |
| Technology | 15% -- MameyNode, AI, platform development |
| Security | 10% -- Emergency services |
| Reserve | 5% -- Emergency fund |

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| **Blockchain** | MameyNode v4.2 (12,847 TPS, post-quantum) |
| **Consensus** | Proof of Sovereignty (574 max validators) |
| **Token** | Wampum (WMP) -- 720M max supply, 0.1% burn |
| **Backend** | Node.js + Express, .NET 10, Rust, Go |
| **Database** | PostgreSQL 16, Redis 7 |
| **AI** | AI Fortress (42 engines) |
| **Translation** | Atabey Neural MT (14 indigenous + 6 global) |
| **Encryption** | ML-DSA-65 + ML-KEM-1024 (post-quantum) |
| **Infrastructure** | Docker, Kubernetes, Nginx, Terraform |
| **Monitoring** | Prometheus + Grafana |
| **CI/CD** | GitHub Actions |

---

## Languages / Idiomas

**Indigenous (37):** Quechua, Nahuatl, Guarani, Aymara, Mapudungun, Maya Yucateco, Zapotec, Garifuna, Taino, Navajo, Cherokee, Lakota, Ojibwe, Cree, K'iche', Kaqchikel, Q'eqchi', Miskito, Wayuunaiki, Shipibo, Tikuna, Embera, Kuna, Mixteco, Tzotzil, Totonaco, P'urhepecha, Raramuri, Bribri, Ngabere, Ashaninka, Wichi, Qom, Yanomami, Shuar, Inuktitut, Mohawk

**Global (6):** Spanish, English, Portuguese, French, Dutch, Haitian Creole

---

## Project Structure

```
Soberano-Organizado/
|-- 01-documentos/          # Founding documents and legal framework
|-- 02-plataformas-html/    # 35 platform front-end UIs
|-- 03-backend/             # Node.js microservices
|   |-- plataforma-principal/
|   |-- red-social/
|   |-- reservas/
|   |-- trading/
|   |-- voto-soberano/
|   |-- blockchain-api/
|   |-- api-gateway/
|   +-- social-media/
|-- 04-infraestructura/     # Docker, Nginx, K8s, Terraform, DB
|-- 05-api/                 # OpenAPI specs
|-- 06-dashboards/          # Monitoring dashboards
|-- 07-scripts/             # Utility and deployment scripts
|-- 08-dotnet/              # .NET solution (Identity, ZKP, Treasury)
|-- 09-assets/              # Logos, images, branding
|-- scripts/                # Master launcher and status scripts
|-- docker-compose.sovereign.yml
|-- .env.example
|-- .gitignore
+-- README.md
```

---

## Development

```bash
# Start all services
bash scripts/levantar-todo.sh

# Check service status
bash scripts/estado.sh

# View logs for a specific service
docker logs -f bdet-bank

# Stop all services
docker compose -f docker-compose.sovereign.yml down

# Rebuild a specific service
docker compose -f docker-compose.sovereign.yml build trading
docker compose -f docker-compose.sovereign.yml up -d trading
```

---

## Contributing / Contribuir

We welcome contributions from all indigenous communities and allies.

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/mi-mejora`
3. Make your changes with clear commit messages
4. Ensure all services start cleanly: `bash scripts/estado.sh`
5. Submit a pull request

### Guidelines

- All code comments should be in Spanish or the relevant indigenous language
- Follow existing code style and patterns
- Add health check endpoints (`/health`) to any new service
- Bind all new services to `127.0.0.1` only
- Never commit secrets or credentials (check `.gitignore`)
- Test with `docker compose build` before submitting

---

## Reach / Alcance

- **72M+** indigenous people across the Americas
- **19** countries
- **574** tribal nations (NCAI recognized)
- **37** indigenous languages supported
- **6** global languages supported
- **0%** tax rate (Constitutional Article VII)

---

## License / Licencia

**Sovereign License 1.0**

Free for all indigenous communities and sovereign nations. This software is provided as a tool for digital sovereignty and self-determination. Commercial use by non-indigenous entities requires written authorization from the Sovereign Government of Ierahkwa Ne Kanienke.

---

## Contact / Contacto

- **Government:** Sovereign Government of Ierahkwa Ne Kanienke
- **Platform:** Ierahkwa Sovereign Digital Platform
- **Blockchain:** MameyNode v4.2
- **Token:** Wampum (WMP)

---

*Built for sovereignty. Built for 72 million indigenous people across the Americas.*

*Construido para la soberania. Construido para 72 millones de personas indigenas en las Americas.*
