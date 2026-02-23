# DocumentFlow

## Intelligent Document Management
### IGT-DOCFLOW | .NET 10

---

## 📄 OVERVIEW

DocumentFlow es el sistema de gestión documental del Gobierno Soberano con AI integrado para búsqueda semántica, OCR inteligente y auto-organización.

## 🏗️ ARQUITECTURA

```
┌─────────────────────────────────────────────────────────────┐
│                    DOCUMENTFLOW + AI                         │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│   ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐  │
│   │  UPLOAD   │ │   OCR     │ │ CLASSIFY  │ │  STORE    │  │
│   │  Engine   │ │  Engine   │ │  AI       │ │  Engine   │  │
│   └───────────┘ └───────────┘ └───────────┘ └───────────┘  │
│         │             │             │             │          │
│   ┌─────┴─────────────┴─────────────┴─────────────┴─────┐   │
│   │              SEMANTIC SEARCH ENGINE                  │   │
│   │                 (AI-Powered)                         │   │
│   └─────────────────────────────────────────────────────┘   │
│                           │                                  │
│   ┌─────────────────────────────────────────────────────┐   │
│   │               WORKFLOW ENGINE                        │   │
│   │        (Approval flows, notifications)               │   │
│   └─────────────────────────────────────────────────────┘   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## 🔧 FUNCIONALIDADES

### 1. Document Upload
- Multi-formato (PDF, DOCX, images)
- Drag & drop
- Batch upload
- Version control

### 2. AI-Powered OCR
- Extracción de texto
- Reconocimiento de tablas
- Entity extraction
- Multi-idioma

### 3. Auto-Classification
- Clasificación automática
- Etiquetado inteligente
- Categorización
- Metadata extraction

### 4. Semantic Search
- Búsqueda por significado
- Natural language queries
- Similar documents
- Full-text search

### 5. Workflow
- Approval flows
- Multi-level review
- Notifications
- Audit trail

## 📡 API ENDPOINTS

```
Base URL: /api/v1/documents

# Documents
POST /upload            - Upload document
GET  /{id}              - Get document
GET  /{id}/download     - Download file
DELETE /{id}            - Delete document

# Search
GET  /search?q=         - Semantic search
GET  /search/similar    - Similar docs

# Folders
GET  /folders           - List folders
POST /folders           - Create folder
PUT  /folders/{id}      - Update folder

# Workflow
POST /{id}/submit       - Submit for approval
POST /{id}/approve      - Approve
POST /{id}/reject       - Reject
```

## 📊 FORMATOS SOPORTADOS

| Tipo | Formatos |
|------|----------|
| Documents | PDF, DOCX, DOC, ODT, RTF |
| Spreadsheets | XLSX, XLS, CSV, ODS |
| Images | PNG, JPG, TIFF, BMP |
| Archives | ZIP, RAR, 7Z |

## 🔐 SEGURIDAD

- Encriptación at rest (AES-256)
- Access control granular
- Audit logging
- Digital signatures
- Watermarking

## 📁 ESTRUCTURA

```
DocumentFlow/
├── DocumentFlow.API/
│   ├── Controllers/
│   │   ├── DocumentsController.cs
│   │   ├── FoldersController.cs
│   │   └── TemplatesController.cs
│   ├── Services/
│   │   ├── DocumentService.cs
│   │   ├── OCRService.cs
│   │   └── SearchService.cs
│   └── Program.cs
├── DocumentFlow.Core/
│   ├── Models/
│   │   ├── Document.cs
│   │   ├── DocumentVersion.cs
│   │   └── Folder.cs
│   └── Interfaces/
├── DocumentFlow.Infrastructure/
├── index.html           # Dashboard
└── DocumentFlow.sln
```

## 🚀 DEPLOYMENT

```bash
cd DocumentFlow/DocumentFlow.API
dotnet run
```

## 🔗 INTEGRACIONES

- E-Signature (firma digital)
- IERAHKWA AI (OCR, clasificación)
- NotifyHub (notificaciones)
- AuditTrail (logging)

---

**Estado:** ✅ ACTIVO
**Token:** IGT-DOCFLOW

© 2026 Sovereign Government of Ierahkwa Ne Kanienke
