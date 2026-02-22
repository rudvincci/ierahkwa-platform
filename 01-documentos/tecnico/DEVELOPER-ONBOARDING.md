# Developer Onboarding — Red Soberana

## Quick Start (60 seconds)
```bash
git clone https://github.com/soberano/red-soberana.git
cd red-soberana
bash init.sh
```

## Architecture
```
Client → Traefik (SSL) → API Gateway (:3000) → PostgreSQL/Redis
                        → BDET Bank (:4000) → MameyNode Blockchain
                        → Social Media (:4001) → Meilisearch
                        → SoberanoDoctor (:4002)
                        → PupitreSoberano (:4003)
                        → SoberanoUber (:4004)
                        → SoberanoEats (:4005)
                        → VotoSoberano (:4006)
                        → SoberanoID (:4009)
```

## Key Principles
1. **Zero taxes** for citizens — all fees fund public services
2. **92% to creators** — minimum platform take
3. **Zero ads, zero tracking** — ever
4. **Post-quantum encryption** — ML-DSA + ML-KEM
5. **14 indigenous languages** — via Atabey
6. **Blockchain verified** — MameyNode

## Services & Ports
| Service | Port | Description |
|---------|------|-------------|
| API Gateway | 3000 | 22 platform routes |
| BDET Bank | 4000 | 11 financial engines |
| Social Media | 4001 | 14 social routes |
| SoberanoDoctor | 4002 | Telemedicine |
| PupitreSoberano | 4003 | Education LMS |
| SoberanoUber | 4004 | Transportation |
| SoberanoEats | 4005 | Food delivery |
| VotoSoberano | 4006 | Blockchain voting |
| JusticiaSoberano | 4007 | Dispute resolution |
| CensoSoberano | 4008 | Census |
| SoberanoID | 4009 | Self-sovereign identity |
| PostgreSQL | 5432 | Primary database |
| Redis | 6379 | Cache + sessions |
| MameyNode | 8545 | Blockchain RPC |

## Making Changes
1. Create feature branch: `git checkout -b feature/my-feature`
2. Write code + tests
3. Run: `make test`
4. PR to `develop` → CI runs → merge → auto-deploy staging
5. PR `develop` → `main` → deploy production

## Fiscal Rule
Every platform fee auto-allocates:
- 25% Education · 25% Healthcare · 20% Infrastructure
- 15% Technology · 10% Security · 5% Reserve
- Citizens pay: **0%** (Constitutional Article VII)
