# AUDITORÍA DE ORGANIZACIÓN — Plataforma Soberana Ierahkwa
**Fecha:** 22 de Febrero 2026
**Estado:** CRÍTICO — Archivos regados en 23 carpetas con duplicados masivos

---

## DIAGNÓSTICO: El Problema

Actualmente tienes **23 carpetas** en `/Users/ruddie/Desktop/files/`:

| Carpeta | Tamaño | Archivos | Problema |
|---------|--------|----------|----------|
| `files/` (PRINCIPAL) | 2.2 MB | 60+ archivos | Consolidado principal, PERO tiene copias internas también |
| `files-2/` | 40 KB | 5 archivos | 1 duplicado, 4 únicos |
| `files-3/` | 44 KB | 5 archivos | 1 duplicado, 4 únicos |
| `files-4/` | 44 KB | 7 archivos | 1 duplicado, 6 únicos |
| `files-5/` | 32 KB | 3 archivos | 1 duplicado, 2 únicos |
| `files-6/` | 12 KB | 3 archivos | 0 duplicados, 3 únicos |
| `files-7/` | 36 KB | 6 archivos | 1 duplicado, 5 únicos |
| `files-8/` | 20 KB | 5 archivos | 0 duplicados, 5 únicos |
| `files-9/` | 20 KB | 3 archivos | 0 duplicados, 3 únicos |
| `files-10/` | 36 KB | 6 archivos | 0 duplicados, 6 únicos |
| `files-11/` | 20 KB | 4+mnt archivos | 0 duplicados, 4 únicos + backend |
| `files-12/` | 8 KB | 1+mnt archivos | 0 duplicados, 1 único + backend |
| `files-13/` | 12 KB | 3 archivos | 1 duplicado, 2 únicos |
| `files-14/` | 16 KB | 3+mnt archivos | 0 duplicados, 3 únicos + backend |
| `files-15/` | 16 KB | 3 archivos | 0 duplicados, 3 únicos |
| `files-16/` | 20 KB | 3 archivos | 1 duplicado, 2 únicos |
| `files-17/` | 20 KB | 3 archivos | 0 duplicados, 3 únicos |
| `files17/` | 424 KB | 7 archivos | 7 duplicados, 0 únicos — 100% DUPLICADA |
| `files-18/` | 84 KB | 3 archivos | 3 duplicados, 0 únicos — 100% DUPLICADA |
| `files-19/` | 28 KB | 2 archivos | 2 duplicados, 0 únicos — 100% DUPLICADA |
| `files-20/` | 32 KB | 2 archivos | 2 duplicados, 0 únicos — 100% DUPLICADA |
| `files-21/` | 44 KB | 2 archivos | 2 duplicados, 0 únicos — 100% DUPLICADA |
| `files-22/` | 464 KB | 11 archivos | 11 duplicados, 0 únicos — 100% DUPLICADA |
| Root (sueltos) | ~400 KB | 7 archivos | 7 duplicados del root = files17 = files-22 |

### Resumen del Caos
- **6 carpetas 100% duplicadas** que se pueden eliminar: files17, files-18, files-19, files-20, files-21, files-22
- **7 archivos sueltos en root** que son duplicados
- **~55 archivos únicos** regados en files-2 a files-17 que NECESITAN consolidarse
- La carpeta `files/files/` ya tiene copias internas con sufijo " copy"

---

## INVENTARIO DE ARCHIVOS ÚNICOS POR CATEGORÍA

### 1. DOCUMENTOS LEGALES Y DE GOBIERNO
| Archivo | Ubicación Actual | Categoría |
|---------|-----------------|-----------|
| GOVERNANCE-CONSTITUTION.md | files-8 | Constitución digital |
| BILL-OF-DIGITAL-RIGHTS.md | files-10 | Carta de derechos digitales |
| DATA-SOVEREIGNTY-ACT.md | files-10 | Ley de soberanía de datos |
| FISCAL-POLICY.md | files-9 | Política fiscal |
| TOKENOMICS-WAMPUM.md | files-8 | Tokenomics del WAMPUM |
| LICENSE.md | files-4 | Licencia del proyecto |

### 2. DOCUMENTOS PARA INVERSIONISTAS
| Archivo | Ubicación Actual | Categoría |
|---------|-----------------|-----------|
| INVESTOR-ONE-PAGER.md | files-4 | Resumen para inversores |
| INVESTOR-DATA-ROOM.md | files-13 | Data room de inversores |
| BUSINESS-PLAN.md | files-8 | Plan de negocios |
| WHITEPAPER-IERAHKWA.md | files-10 | Whitepaper técnico |

### 3. DOCUMENTOS TÉCNICOS Y OPERATIVOS
| Archivo | Ubicación Actual | Categoría |
|---------|-----------------|-----------|
| DEVELOPER-ONBOARDING.md | files-14 | Guía de desarrolladores |
| ONBOARDING.md | files-4 | Guía de onboarding general |
| DISASTER-RECOVERY.md | files-8 | Plan de recuperación |
| CHANGELOG.md | files-7 | Registro de cambios |
| EMAILS-OUTREACH.md | files-2, files-3 | Emails de alcance (hay 2 versiones) |

### 4. PLATAFORMAS HTML (Frontends Únicos)
| Archivo | Ubicación Actual | Qué Reemplaza |
|---------|-----------------|---------------|
| portal-soberano.html | files-3, files-5 | Portal principal |
| landing.html | files-4, files-5 | Landing page |
| admin-dashboard.html | files-4 | Panel de administración |
| bdet-wallet.html | files-7 | Wallet BDET |
| blockchain-explorer.html | files-7 | Explorador blockchain |
| fiscal-dashboard.html | files-9 | Dashboard fiscal |
| fiscal-transparency.html | files-17 | Transparencia fiscal |
| trading-dashboard.html | files-17 | Trading dashboard |
| education-dashboard.html | files-10 | Dashboard educativo |
| healthcare-dashboard.html | files-10 | Dashboard de salud |
| INFOGRAPHIC.html | files-10 | Infografía general |

### 5. CÓDIGO BACKEND (JavaScript/Node.js)
| Archivo | Ubicación Actual | Función |
|---------|-----------------|---------|
| server.js | files-6, 7, 11, 12, 14, 15, 16 | **7 versiones diferentes!** |
| chat.js | files-11 | Backend de chat |
| posts.js | files-11 | Backend de posts |
| trading.js | files-11 | Backend de trading |
| bookings.js | files-15 | Backend de reservas |
| categories.js | files-15 | Backend de categorías |
| index.js | files-4, files-17 | Entry point (2 versiones) |

### 6. INFRAESTRUCTURA Y DEVOPS
| Archivo | Ubicación Actual | Función |
|---------|-----------------|---------|
| docker-compose.yml | files-2, files-3 | Docker básico (2 versiones) |
| docker-compose.full.yml | files-16 | Docker completo |
| sovereign-cluster.yaml | files-2 | Kubernetes cluster |
| github-actions.yml | files-2 | CI/CD pipeline |
| ci.yml | files-13 | CI alternativo |
| nginx.conf | files-6 | Config de Nginx |
| hardhat.config.js | files-6 | Config de Hardhat (blockchain) |
| main.tf | files-7 | Terraform (infraestructura) |
| openapi.yaml | files-14 | Especificación API |

### 7. ASSETS
| Archivo | Ubicación Actual | Función |
|---------|-----------------|---------|
| logo.svg | files-8 | Logo del proyecto |
| FiscalAllocation.sol | files-9 | Smart contract Solidity |

### 8. BACKEND EN SUBCARPETAS mnt/
| Archivo | Ubicación Actual | Función |
|---------|-----------------|---------|
| CloudSoberana README | files-3/mnt | Docs de cloud soberana |
| social-media server.js | files-11/mnt, files-12/mnt | Backend social media |
| voto-soberano server.js | files-14/mnt | Backend de votación |

---

## PROBLEMA DE LOS 7 server.js DIFERENTES

Esto es CRÍTICO. Hay 7 archivos `server.js` en diferentes carpetas, cada uno probablemente para un servicio diferente pero todos con el mismo nombre:

- `files-6/server.js` — Posiblemente: Blockchain/API gateway
- `files-7/server.js` — Posiblemente: Explorer/Wallet backend
- `files-11/server.js` — Posiblemente: Red Social backend
- `files-12/server.js` — Posiblemente: Social media (mnt version)
- `files-14/server.js` — Posiblemente: Voto Soberano API
- `files-15/server.js` — Posiblemente: Reservas/Bookings
- `files-16/server.js` — Posiblemente: Servicio principal

Necesitan ser renombrados o puestos en carpetas con nombre descriptivo.

---

## PLAN DE REORGANIZACIÓN PROPUESTO

### Estructura Nueva:

```
/Users/ruddie/Desktop/files/
├── AUDITORIA-ORGANIZACION-2026-02-22.md (este archivo)
│
├── Soberano-Organizado/                    ← NUEVA CARPETA MAESTRA
│   │
│   ├── 01-documentos/
│   │   ├── legal/
│   │   │   ├── GOVERNANCE-CONSTITUTION.md
│   │   │   ├── BILL-OF-DIGITAL-RIGHTS.md
│   │   │   ├── DATA-SOVEREIGNTY-ACT.md
│   │   │   ├── FISCAL-POLICY.md
│   │   │   ├── TOKENOMICS-WAMPUM.md
│   │   │   └── LICENSE.md
│   │   ├── inversores/
│   │   │   ├── INVESTOR-ONE-PAGER.md
│   │   │   ├── INVESTOR-DATA-ROOM.md
│   │   │   ├── BUSINESS-PLAN.md
│   │   │   └── WHITEPAPER-IERAHKWA.md
│   │   ├── tecnico/
│   │   │   ├── DEVELOPER-ONBOARDING.md
│   │   │   ├── ONBOARDING.md
│   │   │   ├── DISASTER-RECOVERY.md
│   │   │   ├── CHANGELOG.md
│   │   │   └── EMAILS-OUTREACH.md
│   │   └── auditoria/
│   │       ├── AUDITORIA-PLATAFORMA-SOBERANA.md
│   │       ├── MAPA-COMPLETO-PLATAFORMA-SOBERANA.md
│   │       ├── INDEX.md
│   │       └── INDIGENOUS-OUTREACH-DATABASE.md
│   │
│   ├── 02-plataformas-html/
│   │   ├── correo-soberano/
│   │   │   └── index.html (CorreoSoberano - el email client)
│   │   ├── red-soberana/
│   │   │   └── index.html
│   │   ├── busqueda-soberana/
│   │   │   └── index.html
│   │   ├── canal-soberano/
│   │   │   └── index.html
│   │   ├── musica-soberana/
│   │   │   └── index.html
│   │   ├── hospedaje-soberano/
│   │   │   └── index.html
│   │   ├── artesania-soberana/
│   │   │   └── index.html
│   │   ├── cortos-indigenas/
│   │   │   └── index.html
│   │   ├── comercio-soberano/
│   │   │   └── index.html
│   │   ├── invertir-soberano/
│   │   │   └── index.html
│   │   ├── docs-soberanos/
│   │   │   └── index.html
│   │   ├── mapa-soberano/
│   │   │   └── index.html
│   │   ├── voz-soberana/
│   │   │   └── index.html
│   │   ├── trabajo-soberano/
│   │   │   └── index.html
│   │   ├── bdet-bank/
│   │   │   └── index.html
│   │   ├── soberano-ecosystem/
│   │   │   └── index.html
│   │   ├── portal-soberano/
│   │   │   └── index.html
│   │   ├── bdet-wallet/
│   │   │   └── index.html
│   │   ├── blockchain-explorer/
│   │   │   └── index.html
│   │   ├── fiscal-dashboard/
│   │   │   └── index.html
│   │   ├── fiscal-transparency/
│   │   │   └── index.html
│   │   ├── trading-dashboard/
│   │   │   └── index.html
│   │   ├── education-dashboard/
│   │   │   └── index.html
│   │   ├── healthcare-dashboard/
│   │   │   └── index.html
│   │   ├── admin-dashboard/
│   │   │   └── index.html
│   │   ├── landing/
│   │   │   └── index.html
│   │   └── infographic/
│   │       └── index.html
│   │
│   ├── 03-backend/
│   │   ├── api-gateway/
│   │   │   └── server.js (files-6)
│   │   ├── blockchain-api/
│   │   │   └── server.js (files-7)
│   │   ├── red-social/
│   │   │   ├── server.js (files-11)
│   │   │   ├── chat.js
│   │   │   ├── posts.js
│   │   │   └── trading.js
│   │   ├── social-media/
│   │   │   └── server.js (files-12/mnt)
│   │   ├── voto-soberano/
│   │   │   └── server.js (files-14)
│   │   ├── reservas/
│   │   │   ├── server.js (files-15)
│   │   │   ├── bookings.js
│   │   │   └── categories.js
│   │   ├── plataforma-principal/
│   │   │   ├── server.js (files-16)
│   │   │   └── index.js (files-4)
│   │   └── trading/
│   │       └── index.js (files-17)
│   │
│   ├── 04-infraestructura/
│   │   ├── docker/
│   │   │   ├── docker-compose.yml
│   │   │   └── docker-compose.full.yml
│   │   ├── kubernetes/
│   │   │   └── sovereign-cluster.yaml
│   │   ├── ci-cd/
│   │   │   ├── github-actions.yml
│   │   │   └── ci.yml
│   │   ├── nginx/
│   │   │   └── nginx.conf
│   │   ├── terraform/
│   │   │   └── main.tf
│   │   └── blockchain/
│   │       ├── hardhat.config.js
│   │       └── FiscalAllocation.sol
│   │
│   ├── 05-api/
│   │   └── openapi.yaml
│   │
│   ├── 06-dashboards/
│   │   ├── mamey-dashboard.html (381KB - dashboard principal)
│   │   └── dashboard.html (command center 108 plataformas)
│   │
│   ├── 07-scripts/
│   │   ├── INSTALAR-TODO.sh
│   │   ├── UNIFICAR-TODO.sh
│   │   ├── UPGRADE-MAMEY.sh
│   │   ├── install-security.sh
│   │   ├── start-mamey-secure.sh
│   │   ├── stop-mamey.sh
│   │   ├── audit-platforms.sh
│   │   ├── rotate-keys.sh
│   │   ├── descubrir-plataformas.sh
│   │   └── generate-sln.sh
│   │
│   ├── 08-dotnet/
│   │   └── IerahkwaMamey.sln
│   │
│   ├── 09-assets/
│   │   └── logo.svg
│   │
│   └── README.md (README principal del proyecto)
│
├── _archivo-duplicados/                    ← RESPALDO antes de borrar
│   ├── files17/    (100% duplicada)
│   ├── files-18/   (100% duplicada)
│   ├── files-19/   (100% duplicada)
│   ├── files-20/   (100% duplicada)
│   ├── files-21/   (100% duplicada)
│   └── files-22/   (100% duplicada)
│
└── _archivos-originales-respaldo/          ← RESPALDO de files/files/ original
    └── files/  (copia completa antes de reorganizar)
```

---

## PASOS PARA EJECUTAR (en orden)

### Paso 1: RESPALDAR TODO PRIMERO
- Copiar `files/` completa a `_archivos-originales-respaldo/`
- No borrar NADA hasta confirmar que todo está copiado

### Paso 2: Crear estructura nueva `Soberano-Organizado/`
- Crear todas las carpetas de la estructura propuesta
- Copiar (no mover) cada archivo único a su nueva ubicación

### Paso 3: Verificar que no falta nada
- Comparar conteo de archivos: origen vs destino
- Verificar que cada archivo único tiene su copia en la estructura nueva

### Paso 4: Mover duplicados a `_archivo-duplicados/`
- files17, files-18, files-19, files-20, files-21, files-22
- Los 7 archivos sueltos del root

### Paso 5: Limpiar `files/files/` de archivos " copy"
- Eliminar los archivos con sufijo " copy" que son redundantes

### Paso 6: Actualizar README.md con la nueva estructura

---

## ARCHIVOS QUE NECESITAN REVISIÓN MANUAL

1. **EMAILS-OUTREACH.md** — Existe en files-2 Y files-3. Revisar cuál es la versión más completa
2. **portal-soberano.html** — Existe en files-3 Y files-5. Comparar versiones
3. **landing.html** — Existe en files-4 Y files-5. Comparar versiones
4. **index.js** — Existe en files-4 Y files-17. Son para servicios diferentes
5. **Los 7 server.js** — Cada uno es un servicio diferente, necesitan identificarse correctamente
6. **README.md** — Existen múltiples versiones en files-2, 3, 5, 7, 16. El de files/files/ es el principal

---

## MÉTRICAS

- **Total de archivos únicos a consolidar:** ~55
- **Total de archivos duplicados:** ~35+
- **Carpetas 100% duplicadas que sobran:** 6
- **Carpetas con contenido mixto:** 15
- **Tiempo estimado de reorganización:** Script automático ~2 minutos
- **Riesgo:** BAJO si se respalda todo primero
