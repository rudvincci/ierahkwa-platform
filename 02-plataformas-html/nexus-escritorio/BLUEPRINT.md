# NEXUS Escritorio — Technical Blueprint

**Sovereign Office & Productivity Architecture**
**Ierahkwa Ne Kanienke | v5.5.0**

---

## 1. System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                  NEXUS ESCRITORIO PORTAL                        │
│               (nexus-escritorio/index.html)                     │
│                     Theme: #26C6DA                              │
├─────────────────────────────────────────────────────────────────┤
│  DOCUMENTS & PRESENTATIONS                                     │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │   Docs   │ │Ofimatica │ │Plantillas│ │  Video   │          │
│  │Soberanos │ │ Soberana │ │ Soberana │ │  Editor  │          │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘          │
│  PRODUCTIVITY & ORGANIZATION                                   │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │Calendario│ │  Notas   │ │Formulario│ │  Hojas   │          │
│  │ Soberano │ │ Soberana │ │   s      │ │ Calculo  │          │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘          │
│  COLLABORATION & MANAGEMENT                                    │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │ Collab   │ │ Proyecto │ │   CRM    │ │  Diseno  │          │
│  │ Soberana │ │ Soberano │ │ Soberano │ │ Soberano │          │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘          │
├─────────────────────────────────────────────────────────────────┤
│                    SHARED LAYER                                 │
│  ierahkwa.css │ ierahkwa.js │ ierahkwa-agents.js │ sw.js       │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                     ┌─────▼─────┐
                     │  API GW   │
                     │  :6199    │
                     └─────┬─────┘
          ┌────────────────┼────────────────┐
    ┌─────▼─────┐   ┌─────▼─────┐   ┌─────▼─────┐
    │   Docs    │   │  Sheets   │   │ Calendar  │
    │  :6200    │   │  :6201    │   │  :6202    │
    └───────────┘   └───────────┘   └───────────┘
    ┌─────▼─────┐   ┌─────▼─────┐
    │   Forms   │   │  Collab   │
    │  :6203    │   │  :6204    │
    └───────────┘   └───────────┘
                           │
          ┌────────────────┼────────────────┐
    ┌─────▼─────┐   ┌─────▼─────┐   ┌─────▼─────┐
    │  Document │   │ MameyNode │   │  WAMPUM   │
    │   Store   │   │ Blockchain│   │ Invoicing │
    └───────────┘   └───────────┘   └───────────┘
```

## 2. CRDT Collaboration Engine

```
┌──────────┐     ┌──────────┐     ┌──────────┐
│  User A  │     │  User B  │     │  User C  │
│(Browser) │     │(Browser) │     │(Browser) │
└────┬─────┘     └────┬─────┘     └────┬─────┘
     │                │                │
     ▼                ▼                ▼
┌──────────┐    ┌──────────┐    ┌──────────┐
│  Local   │    │  Local   │    │  Local   │
│  CRDT    │    │  CRDT    │    │  CRDT    │
│  State   │    │  State   │    │  State   │
└────┬─────┘    └────┬─────┘    └────┬─────┘
     │               │               │
     └───────────────┼───────────────┘
                     │
              ┌──────▼──────┐
              │ CollabService│
              │   (:6204)   │
              │  WebSocket  │
              └──────┬──────┘
                     │
              ┌──────▼──────┐
              │  Canonical  │
              │   CRDT      │
              │   State     │
              └──────┬──────┘
                     │
              ┌──────▼──────┐
              │  Document   │
              │   Store     │
              │ (Versioned) │
              └─────────────┘
```

## 3. Component Interaction

```
┌──────────┐     ┌──────────┐     ┌──────────┐
│Individual│────>│  Portal  │<────│  Team    │
│   User   │     │  (PWA)   │     │  Admin   │
└────┬─────┘     └────┬─────┘     └────┬─────┘
     │                │                │
     │         ┌──────▼──────┐        │
     └────────>│  AI Agents  │<───────┘
               │  (Browser)  │
               └──────┬──────┘
                      │
        ┌─────────────┼─────────────┐
   ┌────▼────┐   ┌────▼────┐  ┌────▼────┐
   │ IndexDB │   │ Service │  │  CRDT   │
   │(Offline)│   │ Worker  │  │ Engine  │
   └─────────┘   └─────────┘  └─────────┘
```

## 4. Data Flow

### 4.1 Document Editing Flow

```
User Input ──> CRDT Local State
                    │
           ┌────────▼────────┐
           │   Local Apply   │
           │  (Instant UI)   │
           └────────┬────────┘
                    │
           ┌────────▼────────┐
           │  WebSocket Tx   │
           │  to CollabSvc   │
           └────────┬────────┘
                    │
           ┌────────▼────────┐
           │   Merge CRDT    │
           │  (Server-side)  │
           └────────┬────────┘
                    │
        ┌───────────┼───────────┐
   ┌────▼────┐ ┌────▼────┐ ┌───▼────┐
   │Broadcast│ │ Version │ │Forensic│
   │to Peers │ │ History │ │  Log   │
   └─────────┘ └─────────┘ └────────┘
```

### 4.2 CRM Pipeline Flow

```
Lead Source ──> CRM Soberano
                    │
           ┌────────▼────────┐
           │  AI Lead Score  │
           │  (Trust Agent)  │
           └────────┬────────┘
                    │
           ┌────────▼────────┐
           │  Pipeline Stage │
           │  (Kanban Board) │
           └────────┬────────┘
                    │
        ┌───────────┼───────────┐
   ┌────▼────┐ ┌────▼────┐ ┌───▼────┐
   │ Contact │ │ Invoice │ │Activity│
   │  Record │ │(WAMPUM) │ │  Log   │
   └─────────┘ └────┬────┘ └────────┘
                     │
              ┌──────▼──────┐
              │  MameyNode  │
              │ (Blockchain)│
              └─────────────┘
```

## 5. API Endpoints

### 5.1 DocsService (:6200)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/docs/create` | Create new document |
| GET | `/api/docs/{id}` | Get document content |
| PUT | `/api/docs/{id}` | Update document |
| POST | `/api/docs/{id}/export` | Export to PDF/DOCX/ODT |
| GET | `/api/docs/{id}/history` | Get version history |
| POST | `/api/docs/{id}/ai/summarize` | AI summarization |

### 5.2 SheetsService (:6201)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/sheets/create` | Create new spreadsheet |
| GET | `/api/sheets/{id}` | Get spreadsheet data |
| POST | `/api/sheets/{id}/formula` | Execute formula |
| POST | `/api/sheets/{id}/ai/formula` | AI formula from natural language |
| GET | `/api/sheets/{id}/pivot` | Generate pivot table |
| POST | `/api/sheets/{id}/chart` | Create chart visualization |

### 5.3 CalendarService (:6202)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/calendar/events` | List events |
| POST | `/api/calendar/event/create` | Create event |
| PUT | `/api/calendar/event/{id}` | Update event |
| POST | `/api/calendar/ai/schedule` | AI smart scheduling |
| GET | `/api/calendar/availability` | Check availability |

### 5.4 FormsService (:6203)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/forms/create` | Create form |
| GET | `/api/forms/{id}` | Get form definition |
| POST | `/api/forms/{id}/submit` | Submit form response |
| GET | `/api/forms/{id}/responses` | Get all responses |
| GET | `/api/forms/{id}/analytics` | Get response analytics |

### 5.5 CollabService (:6204)

| Method | Endpoint | Description |
|--------|----------|-------------|
| WS | `/ws/collab/{docId}` | WebSocket for real-time collaboration |
| GET | `/api/collab/{docId}/presence` | Get active collaborators |
| POST | `/api/collab/{docId}/comment` | Add comment |
| GET | `/api/collab/{docId}/activity` | Get activity feed |

## 6. Deployment Topology

```
┌─────────────────────────────────────────────┐
│        SOVEREIGN CLOUD (MameyNode)          │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐    │
│  │  Docs   │  │ Sheets  │  │Calendar │    │
│  │ Service │  │ Service │  │ Service │    │
│  │ Cluster │  │ Cluster │  │ Cluster │    │
│  └────┬────┘  └────┬────┘  └────┬────┘    │
│       └─────────────┼───────────┘          │
│              ┌──────▼──────┐               │
│              │ WebSocket   │               │
│              │  Cluster    │               │
│              │ (CollabSvc) │               │
│              └──────┬──────┘               │
│              ┌──────▼──────┐               │
│              │  Document   │               │
│              │  Store      │               │
│              │  (E2E Enc)  │               │
│              └──────┬──────┘               │
└─────────────────────┼──────────────────────┘
                      │
        ┌─────────────┼─────────────┐
   ┌────▼────┐   ┌────▼────┐  ┌────▼────┐
   │Desktop  │   │ Mobile  │  │ Tablet  │
   │  (PWA)  │   │  (PWA)  │  │  (PWA)  │
   └─────────┘   └─────────┘  └─────────┘
```

## 7. Database Schema (Core)

```
documents
├── id (UUID)
├── owner_id (FK)
├── type (doc|sheet|form|slide)
├── title
├── crdt_state (BLOB)
├── encryption_key_id
├── language
├── version
├── blockchain_hash
└── updated_at

collaborators
├── document_id (FK)
├── user_id (FK)
├── role (admin|editor|viewer|commenter)
├── trust_score
├── joined_at
└── last_active

crm_contacts
├── id (UUID)
├── organization_id (FK)
├── name / email / phone
├── lead_score (0-100)
├── pipeline_stage
├── total_value_wampum
└── created_at
```

## 8. Security Boundaries

```
┌─────────────────────────────────────────┐
│  PUBLIC ZONE                            │
│  - Portal landing page                  │
│  - Pricing information                  │
│  - Public form submissions              │
└──────────────┬──────────────────────────┘
               │ Auth (FIDO2/TOTP)
┌──────────────▼──────────────────────────┐
│  MEMBER ZONE (Trust > 40)               │
│  - Personal docs, notes, calendar       │
│  - Basic collaboration (3 users)        │
│  - 5GB storage                          │
└──────────────┬──────────────────────────┘
               │ Subscription upgrade
┌──────────────▼──────────────────────────┐
│  RESIDENT ZONE (Trust > 60)             │
│  - Full 12-app suite                    │
│  - Unlimited collaboration              │
│  - AI assistant, CRM basic              │
│  - 100GB storage                        │
└──────────────┬──────────────────────────┘
               │ Enterprise auth
┌──────────────▼──────────────────────────┐
│  CITIZEN ZONE (Trust > 80)              │
│  - Unlimited storage                    │
│  - Full CRM + pipeline                  │
│  - Admin console + SSO                  │
│  - API access + audit trail             │
└─────────────────────────────────────────┘
```

---

*NEXUS Escritorio Blueprint -- Ierahkwa Ne Kanienke Sovereign Digital Nation*
