# NEXUS Escolar — Sovereign K-12 Education Platform

**Ierahkwa Ne Kanienke — Sovereign Digital Nation**
**NEXUS Color:** `#1E88E5` | **Version:** v5.5.0 | **Platforms:** 10

---

## Overview

NEXUS Escolar is the K-12 education mega-portal within the Ierahkwa sovereign digital ecosystem. It provides a complete, culturally-aligned educational infrastructure for primary and secondary schools across 574 tribal nations and 19 indigenous countries, serving 72 million indigenous people. Every tool is designed with indigenous language preservation, cultural curriculum integration, and data sovereignty at its core.

## Sub-Platforms

| # | Platform | Description |
|---|----------|-------------|
| 1 | **Aula Virtual Soberana** | Virtual classroom with live video, whiteboard, and attendance tracking |
| 2 | **Matricula Escolar** | Student enrollment, registration, and grade-level management |
| 3 | **Calificaciones Soberana** | Gradebook system with rubrics, competency tracking, and parent portals |
| 4 | **Biblioteca Escolar** | Digital library with indigenous-language textbooks and open educational resources |
| 5 | **Tareas Soberana** | Homework assignment, submission, and grading workflows |
| 6 | **Evaluaciones Escolar** | Assessment builder with adaptive testing and psychometric analytics |
| 7 | **Horario Escolar** | Class scheduling, timetable generation, and room allocation |
| 8 | **Transporte Escolar** | School bus routing, GPS tracking, and parent notifications |
| 9 | **Comedor Escolar** | Cafeteria management, nutrition planning, and meal tracking |
| 10 | **Comunicacion Escolar** | Parent-teacher messaging, announcements, and newsletter system |

## Architecture Overview

```
NEXUS Escolar Portal (index.html)
├── Shared Design System (../shared/ierahkwa.css)
├── Shared Runtime (../shared/ierahkwa.js)
├── AI Agents (../shared/ierahkwa-agents.js)
│   ├── Guardian Agent — Student data protection
│   ├── Pattern Agent — Learning behavior analysis
│   └── Trust Agent — Access control scoring
├── Microservices Layer
│   ├── ClassroomService (:7100)
│   ├── EnrollmentService (:7101)
│   ├── GradebookService (:7102)
│   ├── LibraryService (:7103)
│   └── AssessmentService (:7104)
├── MameyNode Blockchain — Credential verification
└── Service Worker (PWA offline-first)
```

## Technology Stack

- **Frontend:** Self-contained HTML5 + CSS3 (zero external dependencies)
- **Design System:** `ierahkwa.css` dark-theme, GAAD accessible, responsive
- **AI:** 7 sovereign AI agents (Guardian, Pattern, Anomaly, Trust, Shield, Forensic, Evolution)
- **Blockchain:** MameyNode for academic credential verification and WAMPUM token economy
- **Offline:** Service Worker + IndexedDB for offline-first operation
- **Languages:** Support for 14+ indigenous languages

## Deployment

1. Serve the `02-plataformas-html/` directory from any static hosting or sovereign MameyNode infrastructure.
2. Each sub-platform is a self-contained `index.html` referencing `../shared/` assets.
3. Microservices run on ports 7100-7104 behind the sovereign API gateway.
4. Service Worker enables PWA installation for offline classroom use.

```bash
# Local development
cd 02-plataformas-html/nexus-escolar
python3 -m http.server 8013

# Production — behind sovereign reverse proxy
ierahkwa deploy --nexus escolar --target mameynode
```

## NEXUS Interconnections

- **ACADEMIA** — University pathway and credit transfer
- **CEREBRO** — AI tutoring and adaptive learning engine
- **VOCES** — Student media projects and school broadcasting
- **CONSEJO** — Education policy and tribal council directives
- **SALUD** — Student health records and wellness screening
- **AMPARO** — Scholarship programs and social protection for students
- **ESCRITORIO** — Docs, spreadsheets for teachers and administrators
- **RAICES** — Cultural curriculum and heritage language materials

## Contributing

1. Fork the repository: `https://github.com/rudvincci/ierahkwa-platform.git`
2. Create a feature branch: `git checkout -b feature/escolar-improvement`
3. Follow the design patterns in `shared/ierahkwa.css`
4. Ensure GAAD accessibility compliance
5. Test in offline mode (Service Worker)
6. Submit a pull request with description of changes

## License

Sovereign license — Ierahkwa Ne Kanienke Digital Nation. All rights reserved under indigenous digital sovereignty framework.
