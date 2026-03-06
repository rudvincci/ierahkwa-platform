# NEXUS Escolar — Technical Blueprint

**Sovereign K-12 Education Architecture**
**Ierahkwa Ne Kanienke | v5.5.0**

---

## 1. System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    NEXUS ESCOLAR PORTAL                         │
│                   (nexus-escolar/index.html)                    │
│                     Theme: #1E88E5                              │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐          │
│  │  Aula    │ │Matricula │ │Califica- │ │Biblioteca│          │
│  │ Virtual  │ │ Escolar  │ │ ciones   │ │ Escolar  │          │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘          │
│  ┌────┴─────┐ ┌────┴─────┐ ┌────┴─────┐ ┌────┴─────┐          │
│  │  Tareas  │ │Evaluacio-│ │ Horario  │ │Transporte│          │
│  │ Soberana │ │   nes    │ │ Escolar  │ │ Escolar  │          │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘          │
│  ┌────┴─────┐ ┌────┴─────┐                                    │
│  │ Comedor  │ │Comunica- │                                     │
│  │ Escolar  │ │  cion    │                                     │
│  └──────────┘ └──────────┘                                     │
├─────────────────────────────────────────────────────────────────┤
│                    SHARED LAYER                                 │
│  ierahkwa.css │ ierahkwa.js │ ierahkwa-agents.js │ sw.js       │
└──────────────────────────┬──────────────────────────────────────┘
                           │
                     ┌─────▼─────┐
                     │  API GW   │
                     │  :7099    │
                     └─────┬─────┘
          ┌────────────────┼────────────────┐
    ┌─────▼─────┐   ┌─────▼─────┐   ┌─────▼─────┐
    │ Classroom │   │ Enrollment│   │ Gradebook │
    │  :7100    │   │  :7101    │   │  :7102    │
    └───────────┘   └───────────┘   └───────────┘
    ┌─────▼─────┐   ┌─────▼─────┐
    │  Library  │   │ Assessment│
    │  :7103    │   │  :7104    │
    └───────────┘   └───────────┘
                           │
                     ┌─────▼─────┐
                     │ MameyNode │
                     │ Blockchain│
                     └───────────┘
```

## 2. Component Interaction

```
┌──────────┐     ┌──────────┐     ┌──────────┐
│  Parent  │────>│  Portal  │────>│ Teacher  │
│  App     │     │  (PWA)   │     │  App     │
└────┬─────┘     └────┬─────┘     └────┬─────┘
     │                │                │
     │         ┌──────▼──────┐        │
     └────────>│  AI Agents  │<───────┘
               │  (Browser)  │
               └──────┬──────┘
                      │
               ┌──────▼──────┐
               │   IndexedDB  │  ← Offline-first store
               │  (Per User)  │
               └──────┬──────┘
                      │ sync
               ┌──────▼──────┐
               │  Sovereign  │
               │  API Layer  │
               └──────┬──────┘
          ┌───────────┼───────────┐
     ┌────▼────┐ ┌────▼────┐ ┌───▼────┐
     │ Student │ │ Course  │ │ Grade  │
     │  Store  │ │  Store  │ │ Store  │
     └─────────┘ └─────────┘ └────────┘
```

## 3. Data Flow

### 3.1 Student Enrollment Flow

```
Student/Parent                    School Admin
     │                                │
     ├──> Fill Enrollment Form        │
     │    (offline-capable)           │
     │         │                      │
     │    ┌────▼────────┐            │
     │    │  Validate   │            │
     │    │  (AI Agent) │            │
     │    └────┬────────┘            │
     │         │                      │
     │    ┌────▼────────┐            │
     │    │ EnrollSvc   │            │
     │    │  :7101      │────────────>├──> Review & Approve
     │    └────┬────────┘            │
     │         │                      │
     │    ┌────▼────────┐            │
     │    │ MameyNode   │            │
     │    │ (Record)    │            │
     │    └────┬────────┘            │
     │         │                      │
     │<── Enrollment Confirmed ──────┘
```

### 3.2 Grading Data Flow

```
Teacher Input ──> GradebookService (:7102)
                       │
              ┌────────┼────────┐
              │        │        │
         ┌────▼──┐ ┌───▼───┐ ┌─▼──────┐
         │Rubric │ │ Grade │ │Report  │
         │Engine │ │ Calc  │ │  Card  │
         └───────┘ └───┬───┘ └────────┘
                       │
              ┌────────▼────────┐
              │  Parent Portal  │
              │  (Real-time)    │
              └────────┬────────┘
                       │
              ┌────────▼────────┐
              │  MameyNode      │
              │  (Immutable)    │
              └─────────────────┘
```

## 4. API Endpoints

### 4.1 ClassroomService (:7100)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/classroom/sessions` | List active classroom sessions |
| POST | `/api/classroom/create` | Create new virtual classroom |
| PUT | `/api/classroom/{id}/attendance` | Record student attendance |
| GET | `/api/classroom/{id}/whiteboard` | Get whiteboard state |
| WS | `/ws/classroom/{id}/live` | WebSocket for live classroom stream |

### 4.2 EnrollmentService (:7101)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/enrollment/register` | Register new student |
| GET | `/api/enrollment/student/{id}` | Get student record |
| PUT | `/api/enrollment/student/{id}/advance` | Advance student grade level |
| POST | `/api/enrollment/transfer` | Transfer student between schools |
| GET | `/api/enrollment/school/{id}/roster` | Get school roster |

### 4.3 GradebookService (:7102)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/grades/record` | Record grade entry |
| GET | `/api/grades/student/{id}/transcript` | Get student transcript |
| GET | `/api/grades/class/{id}/report` | Get class grade report |
| POST | `/api/grades/rubric/create` | Create assessment rubric |
| GET | `/api/grades/analytics/{schoolId}` | Get school-level analytics |

### 4.4 LibraryService (:7103)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/library/catalog` | Browse digital library catalog |
| GET | `/api/library/book/{id}` | Get book content |
| POST | `/api/library/checkout` | Check out digital resource |
| GET | `/api/library/indigenous/{lang}` | Get resources by indigenous language |

### 4.5 AssessmentService (:7104)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/assessment/create` | Create new assessment |
| POST | `/api/assessment/{id}/submit` | Submit student responses |
| GET | `/api/assessment/{id}/results` | Get assessment results |
| GET | `/api/assessment/adaptive/{studentId}` | Get adaptive test for student |

## 5. Deployment Topology

```
┌─────────────────────────────────────────────┐
│           SOVEREIGN CLOUD (MameyNode)       │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐    │
│  │ Region  │  │ Region  │  │ Region  │    │
│  │  North  │  │ Central │  │  South  │    │
│  │ (US/CA) │  │(MX/GT)  │  │(CO/PE)  │    │
│  └────┬────┘  └────┬────┘  └────┬────┘    │
│       └─────────────┼───────────┘          │
│              ┌──────▼──────┐               │
│              │  CDN Layer  │               │
│              │  (Static)   │               │
│              └──────┬──────┘               │
└─────────────────────┼──────────────────────┘
                      │
        ┌─────────────┼─────────────┐
   ┌────▼────┐   ┌────▼────┐  ┌────▼────┐
   │  School │   │  School │  │  School │
   │  Edge   │   │  Edge   │  │  Edge   │
   │ Server  │   │ Server  │  │ (USB)   │
   └────┬────┘   └────┬────┘  └────┬────┘
        │              │            │
   ┌────▼────┐   ┌────▼────┐  ┌────▼────┐
   │ Tablets │   │Chromebks│  │ Offline │
   │  (PWA)  │   │  (PWA)  │  │  (PWA)  │
   └─────────┘   └─────────┘  └─────────┘
```

## 6. Database Schema (Core)

```
students
├── id (UUID)
├── tribal_nation_id (FK)
├── first_name / last_name
├── indigenous_name
├── date_of_birth
├── grade_level
├── primary_language
├── enrollment_date
└── blockchain_credential_hash

courses
├── id (UUID)
├── school_id (FK)
├── teacher_id (FK)
├── name / description
├── language (indigenous + secondary)
├── grade_level
└── semester

grades
├── id (UUID)
├── student_id (FK)
├── course_id (FK)
├── assessment_type
├── score / max_score
├── rubric_data (JSONB)
├── blockchain_hash
└── recorded_at
```

## 7. Security Boundaries

```
┌─────────────────────────────────────────┐
│  PUBLIC ZONE (No Auth Required)         │
│  - Portal landing page                  │
│  - Public school information            │
└──────────────┬──────────────────────────┘
               │ Auth Gate (FIDO2/TOTP)
┌──────────────▼──────────────────────────┐
│  AUTHENTICATED ZONE (Trust Score > 50)  │
│  - Student dashboard                    │
│  - Parent portal                        │
│  - Teacher gradebook                    │
└──────────────┬──────────────────────────┘
               │ Role + Trust > 80
┌──────────────▼──────────────────────────┐
│  ADMIN ZONE (Principal/Admin Only)      │
│  - Enrollment management                │
│  - System configuration                 │
│  - Audit logs                           │
└─────────────────────────────────────────┘
```

---

*NEXUS Escolar Blueprint — Ierahkwa Ne Kanienke Sovereign Digital Nation*
