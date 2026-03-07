# Ierahkwa ML Engine

> Motor soberano de Machine Learning con deteccion de anomalias, scoring de confianza, analisis de patrones y monitoreo de red swarm.

## Descripcion

El Ierahkwa ML Engine es el servicio central de inteligencia artificial y machine learning de la plataforma soberana. Integra cuatro motores especializados que trabajan en conjunto para proteger la red: TrustEngine (puntuacion de confianza dinamica por usuario), AnomalyDetector (deteccion de anomalias por ensemble con Z-score, EMA e IQR), PatternAnalyzer (analisis de patrones de comportamiento con fingerprinting), y SwarmMonitor (monitoreo de salud de red distribuida con consenso).

El servicio actua como backend para los 7 agentes IA del frontend (ierahkwa-agents.js), recibiendo eventos, senales de confianza y acciones via un endpoint unificado de sincronizacion (`/api/agent/sync`). Los motores se retroalimentan: comportamientos anomalos y desviaciones de patron penalizan automaticamente la puntuacion de confianza del usuario, creando un sistema de defensa en capas.

Un dashboard consolidado expone metricas en tiempo real de los cuatro motores, incluyendo detecciones recientes, estadisticas de confianza, alertas de red y estado de consenso. El swarm monitor ejecuta barridos periodicos cada 30 segundos para detectar nodos inactivos y fragmentacion de red.

## Stack Tecnico

- **Runtime**: Node.js 18+
- **Framework**: Express 4.x
- **ML Engine**: Motores propios (zero-dependency ML)
  - TrustEngine: Scoring con decay/boost rates
  - AnomalyDetector: Z-score + EMA + IQR ensemble
  - PatternAnalyzer: Fingerprinting conductual
  - SwarmMonitor: Heartbeat + consenso distribuido
- **Seguridad**: Helmet, CORS, compresion
- **Puerto**: 3092

## API Endpoints

### Health y Dashboard

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con estado de los 4 modulos |
| GET | /api/dashboard | Dashboard completo de ML (todos los motores) |

### Sincronizacion de Agentes Frontend

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/agent/sync | Sync unificado: eventos, confianza, patrones, heartbeat |

### Trust Engine (Confianza)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/trust/calculate | Calcular score de confianza para usuario |
| POST | /api/trust/event | Reportar evento positivo/negativo |
| GET | /api/trust/profile/:userId | Perfil de confianza del usuario |
| GET | /api/trust/leaderboard | Top usuarios por confianza |
| GET | /api/trust/stats | Estadisticas del motor de confianza |

### Anomaly Detector (Deteccion de Anomalias)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/anomaly/detect | Detectar anomalia en metrica (Z-score+EMA+IQR) |
| POST | /api/anomaly/event | Analizar evento de seguridad |
| GET | /api/anomaly/log | Log de detecciones recientes |
| GET | /api/anomaly/stats | Estadisticas del detector |

### Pattern Analyzer (Analisis de Patrones)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/pattern/analyze | Analizar accion de usuario vs patron habitual |
| GET | /api/pattern/fingerprint/:userId | Fingerprint conductual (min 20 acciones) |
| GET | /api/pattern/stats | Estadisticas del analizador |

### Swarm Monitor (Monitoreo de Red)

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| POST | /api/swarm/heartbeat | Reportar heartbeat de nodo |
| GET | /api/swarm/status | Estado de salud de la red |
| GET | /api/swarm/consensus | Verificar consenso de red |
| GET | /api/swarm/nodes | Nodos agrupados por tipo |
| GET | /api/swarm/alerts | Alertas recientes de la red |

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3092 |
| HOST | Host de escucha | 0.0.0.0 |
| CORS_ORIGIN | Origen CORS permitido | * |
| TRUST_DECAY_RATE | Tasa de decaimiento de confianza | 0.02 |
| TRUST_BOOST_RATE | Tasa de incremento de confianza | 0.5 |
| ANOMALY_WINDOW | Tamano de ventana de deteccion | 100 |
| ANOMALY_Z_THRESHOLD | Umbral Z-score para anomalia | 2.5 |
| ANOMALY_EMA_ALPHA | Alpha para EMA (0-1) | 0.1 |
| ANOMALY_IQR_MULT | Multiplicador IQR | 1.5 |

## Instalacion

```bash
npm install
npm start
```

### Desarrollo

```bash
npm run dev   # --watch mode
npm test      # Jest
```

## Docker

```bash
docker build -t ierahkwa-ml .
docker run -p 3092:3092 \
  -e TRUST_DECAY_RATE=0.02 \
  -e ANOMALY_Z_THRESHOLD=2.5 \
  ierahkwa-ml
```

## Arquitectura

```
Frontend (ierahkwa-agents.js)
  7 Agentes IA ──→ POST /api/agent/sync ──→ ML Engine
                                              │
                    ┌─────────────────────────┼─────────────────────────┐
                    │                         │                         │
              TrustEngine           AnomalyDetector           PatternAnalyzer
              (Score 0-100)         (Z+EMA+IQR ensemble)     (Fingerprinting)
                    │                         │                         │
                    └─────────┬───────────────┘                         │
                              │ Cross-engine feedback                   │
                              │ (anomalias/desviaciones → penalizacion) │
                              └─────────────────────────────────────────┘
                                              │
                                       SwarmMonitor
                                    (Heartbeat cada 30s)
                                    (Consenso distribuido)
                                    (Deteccion de fragmentacion)

Dashboard: GET /api/dashboard → Vista consolidada de los 4 motores
```

Los cuatro motores operan en memoria sin dependencias externas de base de datos, lo que permite respuestas de baja latencia para deteccion en tiempo real.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
