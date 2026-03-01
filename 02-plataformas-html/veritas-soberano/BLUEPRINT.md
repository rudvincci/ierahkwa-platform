# Veritas Protocol — Blueprint Tecnico

**Planos Tecnicos de Implementacion**
**Version 1.0.0**
Gobierno Soberano de Ierahkwa Ne Kanienke

---

## 1. Esquema de Base de Datos

### 1.1 PostgreSQL — Tablas Principales

```sql
-- Contenido sometido a verificacion
CREATE TABLE veritas_content (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content_hash    CHAR(64) NOT NULL UNIQUE,
    source_url      TEXT,
    title           TEXT NOT NULL,
    body_excerpt    TEXT,
    media_type      VARCHAR(20) CHECK (media_type IN ('text','image','video','audio','mixed')),
    submitted_by    UUID REFERENCES sovereign_citizens(id),
    submitted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    status          VARCHAR(20) DEFAULT 'pending'
                    CHECK (status IN ('pending','in_review','verified','false','partial','disputed')),
    veracity_score  SMALLINT CHECK (veracity_score BETWEEN 0 AND 100),
    archived        BOOLEAN DEFAULT FALSE,
    blockchain_tx   CHAR(66),
    metadata        JSONB NOT NULL DEFAULT '{}'
);

-- Cadena de custodia
CREATE TABLE veritas_custody_chain (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content_id      UUID NOT NULL REFERENCES veritas_content(id),
    actor_id        UUID NOT NULL,
    action          VARCHAR(30) NOT NULL
                    CHECK (action IN ('created','edited','funded','shared','republished','cited','retracted')),
    details         JSONB DEFAULT '{}',
    previous_entry  UUID REFERENCES veritas_custody_chain(id),
    blockchain_tx   CHAR(66),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Reputacion de fuentes
CREATE TABLE veritas_source_reputation (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    source_id       UUID NOT NULL UNIQUE,
    source_name     TEXT NOT NULL,
    source_type     VARCHAR(20) CHECK (source_type IN ('media','journalist','institution','citizen','anonymous')),
    reputation_score SMALLINT DEFAULT 50 CHECK (reputation_score BETWEEN 0 AND 100),
    total_verified  INTEGER DEFAULT 0,
    total_false     INTEGER DEFAULT 0,
    total_partial   INTEGER DEFAULT 0,
    last_activity   TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Verificaciones comunitarias
CREATE TABLE veritas_community_votes (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    content_id      UUID NOT NULL REFERENCES veritas_content(id),
    validator_id    UUID NOT NULL,
    validator_level SMALLINT CHECK (validator_level IN (1,2,3)),
    vote            VARCHAR(10) CHECK (vote IN ('true','false','partial')),
    justification   TEXT NOT NULL,
    stake_amount    DECIMAL(18,8) NOT NULL,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(content_id, validator_id)
);

-- Pruebas ZK de informantes
CREATE TABLE veritas_whistleblower_proofs (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    proof_hash      CHAR(64) NOT NULL UNIQUE,
    zk_proof        BYTEA NOT NULL,
    encrypted_payload BYTEA NOT NULL,
    commitment      CHAR(64) NOT NULL,
    verified        BOOLEAN DEFAULT FALSE,
    content_id      UUID REFERENCES veritas_content(id),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Analisis de deepfake
CREATE TABLE veritas_deepfake_analysis (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    media_hash      CHAR(64) NOT NULL,
    media_type      VARCHAR(10) CHECK (media_type IN ('image','video','audio')),
    manipulation_score SMALLINT CHECK (manipulation_score BETWEEN 0 AND 100),
    detection_details JSONB DEFAULT '{}',
    model_version   VARCHAR(20) NOT NULL,
    processed_at    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Alertas de campañas de desinformacion
CREATE TABLE veritas_alerts (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    campaign_id     CHAR(64) NOT NULL,
    threat_level    SMALLINT CHECK (threat_level BETWEEN 1 AND 5),
    title           TEXT NOT NULL,
    description     TEXT NOT NULL,
    affected_regions TEXT[] DEFAULT '{}',
    content_ids     UUID[] DEFAULT '{}',
    active          BOOLEAN DEFAULT TRUE,
    detected_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    resolved_at     TIMESTAMPTZ
);

-- Archivo inmutable de hechos
CREATE TABLE veritas_fact_archive (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    fact_hash       CHAR(64) NOT NULL UNIQUE,
    content_id      UUID NOT NULL REFERENCES veritas_content(id),
    summary         TEXT NOT NULL,
    evidence_refs   JSONB NOT NULL DEFAULT '[]',
    cross_references UUID[] DEFAULT '{}',
    blockchain_tx   CHAR(66) NOT NULL,
    archived_at     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Indices
CREATE INDEX idx_content_hash ON veritas_content(content_hash);
CREATE INDEX idx_content_status ON veritas_content(status);
CREATE INDEX idx_custody_content ON veritas_custody_chain(content_id);
CREATE INDEX idx_votes_content ON veritas_community_votes(content_id);
CREATE INDEX idx_alerts_active ON veritas_alerts(active) WHERE active = TRUE;
CREATE INDEX idx_archive_fact_hash ON veritas_fact_archive(fact_hash);
```

## 2. Diseño de API

### 2.1 Endpoints REST

```yaml
openapi: 3.0.3
info:
  title: Veritas Protocol API
  version: 1.0.0
paths:
  /api/v1/veritas/verify/{content_hash}:
    get:
      summary: Consultar estado de verificacion
      parameters:
        - name: content_hash
          in: path
          required: true
          schema: { type: string, pattern: "^[a-f0-9]{64}$" }
      responses:
        200: { description: "Verificacion encontrada" }
        404: { description: "Contenido no registrado" }

  /api/v1/veritas/submit:
    post:
      summary: Enviar contenido para verificacion
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [content, source_url]
              properties:
                content: { type: string, maxLength: 50000 }
                source_url: { type: string, format: uri }
                media_hash: { type: string }
                metadata: { type: object }

  /api/v1/veritas/whistleblower/submit:
    post:
      summary: Envio anonimo con ZK-Proof
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              required: [zk_proof, encrypted_payload, commitment]
              properties:
                zk_proof: { type: string, format: base64 }
                encrypted_payload: { type: string, format: base64 }
                commitment: { type: string, pattern: "^[a-f0-9]{64}$" }

  /api/v1/veritas/source/{source_id}/reputation:
    get:
      summary: Reputacion de fuente
      responses:
        200:
          content:
            application/json:
              schema:
                type: object
                properties:
                  score: { type: integer, minimum: 0, maximum: 100 }
                  total_verified: { type: integer }
                  trend: { type: string, enum: [rising, stable, declining] }

  /api/v1/veritas/deepfake/analyze/{media_hash}:
    get:
      summary: Resultado de analisis deepfake
      responses:
        200:
          content:
            application/json:
              schema:
                type: object
                properties:
                  manipulation_score: { type: integer }
                  details: { type: object }

  /api/v1/veritas/alerts/active:
    get:
      summary: Alertas activas de desinformacion
      parameters:
        - name: region
          in: query
          schema: { type: string }
        - name: min_threat
          in: query
          schema: { type: integer, minimum: 1, maximum: 5 }

  /api/v1/veritas/archive/{fact_id}:
    get:
      summary: Consultar hecho en archivo inmutable
```

### 2.2 WebSocket Events

```
ws://veritas.ierahkwa.nation/ws/v1/stream

Events emitidos:
  verification.submitted   — Nuevo contenido enviado
  verification.completed   — Verificacion completada con score
  alert.new                — Nueva alerta de desinformacion
  alert.resolved           — Alerta resuelta
  deepfake.detected        — Deepfake detectado
  whistleblower.verified   — Prueba ZK verificada
```

## 3. Integracion con Oracle Soberano

### 3.1 Feed de Datos Verificados

Veritas Protocol expone un feed compatible con el protocolo Oracle Soberano:

```
GET /api/v1/oracle-feed/veritas/latest
```

Retorna los ultimos hechos verificados en formato compatible con smart contracts consumidores. Oracle Soberano agrega este feed junto con sus otros feeds de datos.

### 3.2 Contrato de Interoperabilidad

```solidity
interface IVeritasOracle {
    function getVeracityScore(bytes32 contentHash) external view returns (uint8);
    function isVerified(bytes32 contentHash) external view returns (bool);
    function getSourceReputation(address source) external view returns (uint256);
}
```

## 4. Infraestructura de Despliegue

### 4.1 Servicios Docker

```yaml
# Añadir a docker-compose.sovereign.yml
veritas-engine:
  build: ./03-backend/veritas-engine
  container_name: veritas-engine
  ports: ["127.0.0.1:3600:3600"]
  environment:
    - PORT=3600
    - DATABASE_URL=postgres://${POSTGRES_USER}:${POSTGRES_PASSWORD}@postgres:5432/ierahkwa_veritas
    - MAMEYNODE_RPC=http://mameynode:8545
    - REDIS_URL=redis://:${REDIS_PASSWORD}@redis:6379
  depends_on: [postgres, mameynode, redis]
  networks: [sovereign-net]
```

### 4.2 Requisitos de Hardware por Nodo

| Componente | Minimo | Recomendado |
|-----------|--------|-------------|
| CPU | 4 cores | 8 cores |
| RAM | 8 GB | 16 GB |
| Disco | 100 GB SSD | 500 GB NVMe |
| Red | 100 Mbps | 1 Gbps |
| GPU (deepfake) | N/A | NVIDIA T4 o superior |

### 4.3 Distribucion por Nacion

Cada nacion opera minimo 1 nodo verificador con la siguiente configuracion:

- PostgreSQL replica local (streaming replication desde nodo primario)
- Redis cache local para verificaciones recientes
- Modelo anti-deepfake descargado localmente (inferencia sin internet)
- Copia del Archivo Inmutable via IPFS pinning

## 5. Metricas y Monitoreo

```
Prometheus metrics expuestas en :3600/metrics

veritas_verifications_total{status}        — Total verificaciones por estado
veritas_deepfake_detections_total          — Total deepfakes detectados
veritas_alerts_active                       — Alertas activas actuales
veritas_zk_proofs_verified_total           — Pruebas ZK verificadas
veritas_community_votes_total{vote}        — Votos comunitarios por tipo
veritas_source_reputation_avg              — Reputacion promedio de fuentes
veritas_verification_latency_seconds       — Latencia de verificacion (histogram)
```

Dashboards Grafana pre-configurados en `04-infraestructura/grafana/veritas-dashboard.json`.

---

*Gobierno Soberano de Ierahkwa Ne Kanienke*
*Soberania Digital para 72M de Personas*
