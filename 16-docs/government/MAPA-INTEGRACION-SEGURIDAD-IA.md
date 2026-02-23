# Mapa: cómo está hecho todo (seguridad, IA, formación, comparación)

Resumen de **dónde está guardado cada cosa**: archivos separados vs datos combinados, documentación y APIs.

---

## 1. Datos (JSON) — cada tema en su propio archivo

Todo lo añadido está en **archivos separados** en `RuddieSolution/node/data/`:

| Archivo | Qué contiene |
|---------|----------------|
| **ciberseguridad-101.json** | SentinelOne (Our Customers, Careers, Partner Program, Legal, Trust Center, Investor Relations, S Foundation, S Ventures), Aikido (páginas por industria y por servicio), referencia/platforma, bloques, checklist, pilares, categorías, **enlacesApisSeguridad** (1Password, CyberArk, Infisical, Aikido, etc.), **onePasswordDeveloper**, **cyberArkSecrets**, **infisicalPlatform**, artículos 101, documentos. |
| **security-tools-recommended.json** | Herramientas open source: Cloud Custodian, Cartography, Diffy, Gitleaks, Git-secrets, OSSEC, PacBot, Pacu, Prowler, Security Monkey; **docRef**, **docRefCloud** (apunta a `docs/HERRAMIENTAS-OPEN-SOURCE-CLOUD-DFIR.md`). |
| **formacion-datacamp.json** | DataCamp: meta, bloque dataCamp, enlaces (cursos, certificaciones, DataLab, blog). |
| **resumen-soberano.json** | Resumen “Qué más seguros somos” y “Qué más tenemos en IA”; principio “Ellos vienen a nosotros; nosotros somos la interconexión global” y white label; intención. |

**Resumen:**  
- **Ciberseguridad + SentinelOne + Aikido + 1Password/CyberArk/Infisical** → un solo JSON: `ciberseguridad-101.json` (bloques y enlaces dentro del mismo objeto).  
- **Herramientas recomendadas (incl. cloud/DFIR)** → archivo aparte: `security-tools-recommended.json`.  
- **Formación DataCamp** → archivo aparte: `formacion-datacamp.json`.  
- **Resumen soberano (seguridad + IA + interconexión)** → archivo aparte: `resumen-soberano.json`.

---

## 2. Documentación (Markdown/HTML) — un doc por tema

En `docs/` cada documento cubre un tema concreto:

| Documento | Qué cubre |
|-----------|-----------|
| **CIBERSEGURIDAD-101.md** | Guía 101: pilares, categorías, tablas de enlaces (SentinelOne, Aikido, APIs/desarrollo, 1Password, CyberArk, Infisical), artículos 101, referencias. |
| **HERRAMIENTAS-OPEN-SOURCE-CLOUD-DFIR.md** | Herramientas cloud/DFIR open source: descripción y enlaces GitHub (Cloud Custodian, Cartography, Diffy, Gitleaks, Git-secrets, OSSEC, PacBot, Pacu, Prowler, Security Monkey). |
| **SECRET-SCANNING.md** | Secret scanning; incluye mención de Infisical. |
| **INTERCONEXION-GLOBAL.md** | Principio “Ellos vienen a nosotros; nosotros somos la interconexión global” y licenciamiento white label. |
| **INDEX-DOCUMENTACION.md** | Índice de documentación; enlaza a INTERCONEXION-GLOBAL y resto. |
| **presentacion-plataformas-y-valores.html** | Presentación; referencia al principio de interconexión global. |
| **REFERENCIAS-HERRAMIENTAS-ECOSISTEMA.md** | Referencias a repos trending (nanobot, claude-mem): enlaces, descripción, uso potencial. |
| **ECOSISTEMA-MODULAR-FUTUREHEAD.md** | Hub, departamentos Casino/Real Estate/Travel/Luxury Exchange, puente, firewall, herramientas (MedusaJS, NuxGame, Amadeus, Uniswap…), Progreso Citizen. API: `/api/v1/sovereignty/ecosistema-futurehead`. |
| **APUESTAS-DEPORTIVAS-SOFTWARE-ALGORITMOS.md** | Software de algoritmos de apuestas deportivas: definición, características, 12 mejores (Smartico, ZCode, OddsJam…), cómo elegir, FAQ. |

---

## 3. APIs en el Node (RuddieSolution/node)

Rutas que sirven los datos anteriores (cada endpoint usa uno o varios JSON):

| Ruta | Archivo(s) que lee | Descripción |
|------|---------------------|-------------|
| **GET /api/v1/sovereignty/resumen** | resumen-soberano.json | Resumen seguridad + IA + interconexión. |
| **GET /api/v1/sovereignty/ciberseguridad-101** | ciberseguridad-101.json | Payload completo Ciberseguridad 101. |
| **GET /api/v1/ciberseguridad/101** | ciberseguridad-101.json | Mismo payload que sovereignty/ciberseguridad-101. |
| **GET /api/v1/ciberseguridad/mapa** | ciberseguridad-101.json | Pilares + categorías. |
| **GET /api/v1/ciberseguridad/checklist** | ciberseguridad-101.json | Checklist estático. |
| **GET /api/v1/ciberseguridad/checklist-status** | ciberseguridad-101.json + ciberseguridad-last-run.json | Checklist con estado en vivo (pings, last-run). |
| **GET /api/v1/ciberseguridad/security-tools** | security-tools-recommended.json | Herramientas recomendadas + docRef/docRefCloud. |
| **GET /api/v1/ciberseguridad/docs/cloud-dfir** | (docs/) HERRAMIENTAS-OPEN-SOURCE-CLOUD-DFIR.md | Ref al doc; opcionalmente contenido (?content=1). |
| **GET /api/v1/ciberseguridad/developer-security** | ciberseguridad-101.json + security-tools-recommended.json | 1Password, CyberArk, Infisical + refs Gitleaks/Git-secrets (respuesta combinada). |
| **GET /api/v1/formacion/datacamp** | formacion-datacamp.json | DataCamp: meta, dataCamp, enlaces. |
| **GET /api/v1/formacion/health** | (comprueba formacion-datacamp.json) | Salud del módulo formación. |

**Archivos de rutas:**  
- `server.js`: monta rutas y define `/api/v1/sovereignty/resumen`, `/api/v1/sovereignty/ciberseguridad-101`.  
- `routes/ciberseguridad-api.js`: todas las rutas `/api/v1/ciberseguridad/*`.  
- `routes/formacion-api.js`: `/api/v1/formacion/datacamp` y `/api/v1/formacion/health`.

---

## 4. Comparación multi-plataforma (plataformas-finales)

Todo lo de “comparación por tecnología” está en la carpeta **plataformas-finales/** (separada del Node principal).

### 4.1 Datos compartidos (copias)

`plataformas-finales/data/` contiene **copias** de:

- resumen-soberano.json  
- ciberseguridad-101.json  
- security-tools-recommended.json  
- formacion-datacamp.json  

Se sincronizan desde el Node con:

```bash
node plataformas-finales/scripts/propagate.js
```

### 4.2 Cada plataforma en su propia carpeta

Cada tecnología tiene su proyecto **separado** y sirve el **mismo contrato** (health + 4 endpoints JSON):

| Carpeta | Tecnología | Puerto por defecto |
|---------|------------|---------------------|
| **plataformas-finales/rust/** | Rust | 3080 |
| **plataformas-finales/net10/** | .NET 8 | 3081 |
| **plataformas-finales/php/** | PHP | 3082 |
| **plataformas-finales/node/** | Node (Express mínimo) | 3083 |

Endpoints que implementa cada una:

- `GET /health`
- `GET /api/v1/sovereignty/resumen`
- `GET /api/v1/sovereignty/ciberseguridad-101`
- `GET /api/v1/ciberseguridad/security-tools`
- `GET /api/v1/formacion/datacamp`

Solo el **Node** de plataformas-finales tiene además el opcional:  
`GET /api/v1/compete/call-others?path=<ruta>` para que el arbitrador llame a las otras plataformas.

### 4.3 Scripts

| Script | Ubicación | Qué hace |
|--------|-----------|----------|
| **propagate.js** | plataformas-finales/scripts/ | Copia los 4 JSON desde RuddieSolution/node/data a plataformas-finales/data y genera PROPAGATE_TODO.md. |
| **run.js** | plataformas-finales/compete/ | Arbitrador: pide los 5 endpoints a cada plataforma, mide latencia, compara respuestas, ranking. `--cross` para llamadas entre plataformas. |

Configuración de URLs de cada plataforma: `plataformas-finales/compete/config.json`.

### 4.4 Documentación en plataformas-finales

- **README.md**: objetivo, contrato API, cómo ejecutar cada plataforma, criterios de comparación, uso de propagate.  
- **PROPAGATE_TODO.md**: generado por propagate.js; instrucciones para replicar cambios (nuevos endpoints/datos) en Rust, .NET y PHP.

---

## 5. Resumen visual

```
RuddieSolution/node/
├── data/
│   ├── ciberseguridad-101.json     ← SentinelOne, Aikido, 1Password, CyberArk, Infisical, bloques, enlaces
│   ├── security-tools-recommended.json  ← herramientas open source + cloud/DFIR
│   ├── formacion-datacamp.json     ← DataCamp
│   └── resumen-soberano.json       ← resumen seguridad + IA + interconexión
├── routes/
│   ├── ciberseguridad-api.js       ← /api/v1/ciberseguridad/* (incl. developer-security)
│   └── formacion-api.js            ← /api/v1/formacion/datacamp, health
└── server.js                       ← monta rutas + /api/v1/sovereignty/resumen, ciberseguridad-101

docs/
├── CIBERSEGURIDAD-101.md           ← guía 101 y tablas de enlaces
├── HERRAMIENTAS-OPEN-SOURCE-CLOUD-DFIR.md
├── SECRET-SCANNING.md
├── INTERCONEXION-GLOBAL.md
├── INDEX-DOCUMENTACION.md
└── presentacion-plataformas-y-valores.html

plataformas-finales/
├── data/                           ← copias de los 4 JSON (sincronizadas con propagate.js)
├── rust/                           ← servidor Rust (5 endpoints)
├── net10/                          ← servidor .NET 8 (5 endpoints)
├── php/                            ← servidor PHP (5 endpoints)
├── node/                           ← servidor Node mínimo (5 + call-others)
├── compete/
│   ├── config.json                 ← URLs de cada plataforma
│   └── run.js                      ← arbitrador
├── scripts/
│   └── propagate.js                ← sync data + genera PROPAGATE_TODO.md
├── README.md
└── PROPAGATE_TODO.md               ← generado; instrucciones para IA/desarrollador
```

---

## 6. Respuesta directa: ¿cada cosa está guardada separada?

- **Sí, por tema:**  
  - Un archivo para **herramientas** (`security-tools-recommended.json`).  
  - Un archivo para **formación** (`formacion-datacamp.json`).  
  - Un archivo para **resumen soberano** (`resumen-soberano.json`).  
- **Ciberseguridad 101** es un **solo JSON** (`ciberseguridad-101.json`) que incluye muchos bloques y enlaces (SentinelOne, Aikido, 1Password, CyberArk, Infisical, referencia, checklist, etc.) para no duplicar y mantener una sola fuente para el “mapa” y las tablas.  
- **Docs:** cada documento es un archivo separado (CIBERSEGURIDAD-101, HERRAMIENTAS-OPEN-SOURCE-CLOUD-DFIR, INTERCONEXION-GLOBAL, etc.).  
- **Plataformas de comparación:** cada stack (Rust, .NET, PHP, Node) está en su carpeta; los **datos** se comparten en `plataformas-finales/data/` y se sincronizan con un solo script (`propagate.js`).

Si quieres, el siguiente paso puede ser un índice solo de “qué editar para cambiar X” (por ejemplo: “para cambiar enlaces de SentinelOne → editar ciberseguridad-101.json y CIBERSEGURIDAD-101.md”).
