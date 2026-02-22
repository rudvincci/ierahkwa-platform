# 🌐 Ierahkwa — Red Soberana

> **296 sovereign platforms for 72 million indigenous people. Zero taxes. Free schools. Free hospitals.**

[![CI/CD](https://github.com/soberano/red-soberana/actions/workflows/ci.yml/badge.svg)](https://github.com/soberano/red-soberana/actions)
[![License: Soberana-1.0](https://img.shields.io/badge/License-Soberana--1.0-gold.svg)](LICENSE)
[![Platforms](https://img.shields.io/badge/Platforms-296-2dd4a8.svg)]()
[![Languages](https://img.shields.io/badge/Languages-43-9b6dff.svg)]()
[![Tax Rate](https://img.shields.io/badge/Tax_Rate-0%25-ff6b4a.svg)]()

## Quick Start

```bash
git clone https://github.com/soberano/red-soberana.git
cd red-soberana
docker-compose -f infra/docker-compose.full.yml up -d
# 19 microservices running on ports 3000-4020
```

## Architecture

```
Client → Traefik (SSL/TLS) → API Gateway (:3000)
                              ├── BDET Bank (:4000) ──────── 11 financial engines
                              ├── Social Media (:4001) ───── 14 social routes
                              ├── SoberanoDoctor (:4002) ─── Telemedicine (FREE)
                              ├── PupitreSoberano (:4003) ── Education (FREE)
                              ├── SoberanoUber (:4004) ───── Rides (95% to drivers)
                              ├── SoberanoEats (:4005) ───── Food delivery (90%)
                              ├── VotoSoberano (:4006) ───── Blockchain voting
                              ├── JusticiaSoberano (:4007) ─ Dispute resolution
                              ├── CensoSoberano (:4008) ─── Census
                              ├── SoberanoID (:4009) ─────── Self-sovereign identity
                              ├── SoberanoServicios (:4010)─ 30 service categories
                              ├── CorreoSoberano (:4011) ── Email (post-quantum E2E)
                              ├── BusquedaSoberana (:4012) ─ Search (Meilisearch)
                              ├── MapaSoberano (:4013) ──── Maps (no tracking)
                              ├── NubeSoberana (:4014) ──── Cloud storage
                              ├── SoberanoFarm (:4015) ──── Agriculture AI
                              ├── RadioSoberana (:4016) ─── Community radio
                              ├── CooperativaSoberana(:4017) Cooperative mgmt
                              ├── TurismoSoberano (:4018)── Cultural tourism
                              ├── SoberanoFreelance (:4019)─ Gig marketplace
                              └── SoberanoPOS (:4020) ───── Point of sale
```

## The Numbers

| Metric | Value |
|--------|-------|
| Platforms | 296 (98 with full UI) |
| Microservices | 19 |
| Smart Contracts | 8 (Solidity) |
| Languages | 43 (37 indigenous + 6 global) |
| API Routes | 22 gateway + 150+ microservice endpoints |
| Backend Code | 5,400+ lines |
| Platform UIs | 11,000+ lines HTML |
| SQL Tables | 18 |
| Tests | 6 suites |
| Tax Rate | **0% — Constitutional Article VII** |

## Zero Tax Model

Citizens never pay taxes. Platform fees (5-12%) auto-allocate to public services:

| Service | Allocation |
|---------|-----------|
| 🎓 Education | 25% — Free preschool through university |
| 🏥 Healthcare | 25% — Free doctors, medicines, hospitals |
| 🏗️ Infrastructure | 20% — Internet, water, electricity, roads |
| 💻 Technology | 15% — MameyNode, AI, platform development |
| 🛡️ Security | 10% — Emergency services |
| 🏦 Reserve | 5% — Emergency fund |

## Creator Revenue

| Platform | Creator Gets | Big Tech Equivalent |
|----------|-------------|-------------------|
| Video (CanalSoberano) | **92%** | YouTube: 55% |
| Music (MusicaSoberana) | **90%** | Spotify: 30% |
| Artisan (ArtesaniaSoberana) | **88%** | Etsy: 80% |
| Rides (SoberanoUber) | **95%** | Uber: 70% |
| Food (SoberanoEats) | **90%** | DoorDash: 70% |
| Services (SoberanoServicios) | **92%** | TaskRabbit: 70% |
| Freelance | **92%** | Fiverr: 80% |
| POS | **95%** | Square: 97% |

## 43 Languages

**Indigenous (37):** Quechua, Nahuatl, Guarani, Aymara, Mapudungun, Maya Yucateco, Zapotec, Garifuna, Taino, Navajo, Cherokee, Lakota, Ojibwe, Cree, K'iche', Kaqchikel, Q'eqchi', Miskito, Wayuunaiki, Shipibo, Tikuna, Emberá, Kuna, Mixteco, Tzotzil, Totonaco, P'urhepecha, Rarámuri, Bribri, Ngäbere, Asháninka, Wichí, Qom, Yanomami, Shuar, Inuktitut, Hopi, Mohawk, Muskogee

**Global (6):** Spanish, English, Portuguese, French, Dutch, Haitian Creole

## Technology Stack

- **Blockchain:** MameyNode v4.2 (12,847 TPS, post-quantum)
- **Consensus:** Proof of Sovereignty (574 max validators)
- **Token:** Wampum (WMP) — 720M max supply, 0.1% deflationary burn
- **Backend:** Node.js + Express + PostgreSQL + Redis
- **Search:** Meilisearch
- **AI:** AI Fortress (42 engines)
- **Translation:** Atabey Neural MT
- **Encryption:** ML-DSA-65 + ML-KEM-1024 (post-quantum)
- **Infrastructure:** Docker + Kubernetes + Traefik + Terraform
- **CI/CD:** GitHub Actions → Staging → Production
- **Monitoring:** Prometheus + Grafana + Fluentd → ELK

## Development

```bash
make dev          # Start all services locally
make test         # Run all tests
make lint         # Lint all code
make build        # Build Docker images
make deploy-stg   # Deploy to staging
make deploy-prod  # Deploy to production
```

## Contributing

See [CONTRIBUTION-WORKFLOW.md](docs/CONTRIBUTION-WORKFLOW.md) and [DEVELOPER-ONBOARDING.md](docs/DEVELOPER-ONBOARDING.md).

## License

Soberana-1.0 — Free for all indigenous communities and sovereign nations.

---

**🌿 Soberanía siempre. 0% impuestos. Escuelas y hospitales gratis.**

*Built with love for 72 million indigenous people across the Americas.*
