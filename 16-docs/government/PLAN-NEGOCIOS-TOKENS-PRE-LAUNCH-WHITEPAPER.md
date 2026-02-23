# Plan de Negocios: Tokens IGT — Pre-Lanzamiento y Whitepaper Propio

**Gobierno Soberano de Ierahkwa Ne Kanienke**  
**Objetivo:** Todo token emitido debe tener **página de pre-lanzamiento** y **whitepaper propio** antes de considerarse listo para lanzamiento comercial y regulatorio.

---

## 1. Objetivo y alcance

- **Objetivo:** Asegurar que cada token IGT (Ierahkwa Government Token) emitido en el ecosistema soberano tenga:
  1. **Página de pre-lanzamiento (pre-launch)** pública y enlazada desde el registro de tokens.
  2. **Whitepaper propio** por token (no genérico), en al menos un idioma oficial; recomendado: EN, ES, FR, MOH.

- **Alcance:** Todos los tokens bajo `tokens/` (01-IGT-PM hasta 103-IGT-ESIGN y los utility/deFi incluidos en el mismo árbol). Incluye tokens gubernamentales, comerciales y de utilidad del ecosistema ISB/BDET.

- **Principio:** *Todo propio* — documentación e identidad de cada token es responsabilidad del emisor soberano; no se depende de terceros para whitepapers ni landings.
- **Negocios separados:** Cada token/negocio tiene su propia página de negocio independiente; solo en el main dashboard se mezcla todo; en exchanges/trading cada uno va por su cuenta. Todos comparten el mismo backend y data para comunicaciones. Ver `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md`.

---

## 2. Requisitos por token

### 2.1 Página de pre-lanzamiento (pre-launch)

Cada token debe disponer de una **página de pre-lanzamiento** que cumpla:

| Requisito | Descripción |
|-----------|-------------|
| **URL estable** | Acceso vía `tokens/<NN-IGT-XXX>/index.html` (o ruta equivalente servida por la plataforma). |
| **Contenido mínimo** | Nombre del token, símbolo, categoría, descripción breve, uso principal. |
| **Token economics** | Total supply, decimals, blockchain (ISB), chainId, contract/dir (si aplica). |
| **Comercio / utilidad** | Qué se puede comprar o hacer con el token (comercio, staking, exchange, etc.). |
| **Ecosistema** | Integración con TradeX, BDET, liquidez, casino u otros servicios según corresponda. |
| **Enlace al whitepaper** | Botón o enlace visible a la versión del whitepaper (por idioma o versión por defecto). |
| **Atractivo inversión** | Bloque "Por qué invertir" (propuesta de valor, qué gana el holder, CTAs: Trade, Stake, Whitepaper). Ver `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md`. |
| **Diseño** | Coherente con la identidad visual del registro (`tokens/index.html`) y la plataforma. |

La página actúa como **landing pre-lanzamiento** y como **punto de entrada** para inversores, partners y cumplimiento.

### 2.2 Whitepaper propio por token

Cada token debe tener **su propio whitepaper** (no compartido con otros tokens). Contenido mínimo:

| Sección | Contenido |
|---------|-----------|
| **Título y versión** | Ej: "IGT-XX: [Nombre del token] — Whitepaper v1.0". |
| **Emisor** | Gobierno Soberano de Ierahkwa Ne Kanienke (o entidad autorizada que se indique). |
| **Resumen ejecutivo** | Qué es el token, para qué se usa, en qué ecosistema opera. |
| **Especificaciones** | Nombre, símbolo, supply, decimales, estándar (IGT), blockchain (ISB), ID departamento/categoría. |
| **Infraestructura** | Blockchain, nodo (Mamey/8545), banco central (BDET), consenso (SPoA). |
| **Uso y casos** | Lista concreta de usos (pagos, staking, gobernanza, etc.). |
| **Caso de inversión** | Sección que explique por qué un inversionista querría invertir: valor, demanda, cómo gana valor el token, respaldo, roadmap breve, riesgos. Ver `docs/PLAN-INVERSION-ATRACTIVO-POR-TOKEN.md`. |
| **Riesgos y disclaimer** | Breve mención de que el documento es informativo y no constituye asesoría legal/inversión. |
| **Fecha y copyright** | Año y "© Gobierno Soberano de Ierahkwa Ne Kanienke". |

**Idiomas:**  
- Mínimo: un idioma (recomendado español o inglés).  
- Estándar actual en tokens completos: **whitepaper-en.md**, **whitepaper-es.md**, **whitepaper-fr.md**, **whitepaper-moh.md**.

**Ubicación:**  
- En la carpeta del token: `tokens/<NN-IGT-XXX>/whitepaper-es.md` (y equivalentes por idioma).  
- La pre-launch debe enlazar a `whitepaper-es.md` o a una versión por defecto (ej. `whitepaper-en.md`).

---

## 3. Estándar de estructura por token

Cada carpeta de token debe contener al menos:

```
tokens/NN-IGT-XXX/
├── index.html          # Página pre-lanzamiento (obligatorio)
├── token.json          # Metadatos del token (id, symbol, name, category, description, uses, totalSupply, decimals, blockchain, chainId, contract)
├── whitepaper-es.md    # Whitepaper en español (obligatorio como mínimo)
├── whitepaper-en.md    # Opcional
├── whitepaper-fr.md    # Opcional
└── whitepaper-moh.md   # Opcional (Mohawk)
```

- **index.html:** Pre-launch page con enlace al whitepaper.  
- **token.json:** Ya existe en todos los tokens; debe mantenerse actualizado.  
- **whitepaper-*.md:** Al menos uno por token; recomendado cuatro idiomas para coherencia con los tokens ya completos.

---

## 4. Estado actual (registro)

### 4.1 Tokens con pre-launch + whitepaper completo (41 tokens)

Estos tokens tienen **index.html** (pre-launch) y **whitepaper** en al menos español (whitepaper-es.md):

| Rangos | Tokens |
|--------|--------|
| Gobierno / Core | 01-IGT-PM, 02-IGT-MFA, 03-IGT-MFT, 04-IGT-MJ, 05-IGT-MI, 06-IGT-MD, 07-IGT-BDET, 08-IGT-NT, 09-IGT-AG, 10-IGT-SC, 11-IGT-MH, 12-IGT-ME, 13-IGT-MLE, 14-IGT-MSD, 15-IGT-MHO, 16-IGT-MCH, 17-IGT-MSR, 18-IGT-MFC, 19-IGT-SSA, 20-IGT-PHS |
| Ministerios / Servicios | 21-IGT-MA, 22-IGT-MEN, 23-IGT-MEG, 24-IGT-MMR, 25-IGT-MCT, 26-IGT-MIN, 27-IGT-MT, 28-IGT-MTR, 29-IGT-MST, 30-IGT-MC, 31-IGT-NPS, 32-IGT-AFI, 33-IGT-NIS, 34-IGT-CBP, 35-IGT-CRO, 36-IGT-EC, 37-IGT-OCG, 38-IGT-OO, 39-IGT-NA, 40-IGT-PSI |
| Launchpad | 97-IGT-LAUNCHPAD |

### 4.2 Tokens que tenían pre-launch sin whitepaper (62) — **COMPLETADO**

Se generaron **whitepaper-es.md** y **whitepaper.md** (copia para enlace por defecto) para los 62 tokens con script `scripts/generar-whitepapers-faltantes.js`. Cada uno tiene ya pre-launch + whitepaper propio; el botón "Whitepaper" en `index.html` enlaza a `whitepaper.md` (equivalente a español).

---

## 5. Fases de implementación

| Fase | Acción | Prioridad |
|------|--------|-----------|
| **Fase 1** | Completar whitepaper (mínimo ES) para los 62 tokens pendientes. Usar plantilla estándar (ver sección 2.2 y `tokens/06-IGT-MD/whitepaper-es.md`). **Hecho:** `node scripts/generar-whitepapers-faltantes.js` generó 62 whitepaper-es.md; se copió a whitepaper.md donde el index enlaza a whitepaper.md. | Alta ✅ |
| **Fase 2** | En cada `index.html` de esos 62 tokens: añadir botón/enlace visible "Whitepaper" → whitepaper. **Hecho:** esos tokens ya tenían botón a `whitepaper.md`; whitepaper.md creado como copia de whitepaper-es.md. | Alta ✅ |
| **Fase 3** | Revisar que todos los tokens con whitepaper tengan en pre-launch el enlace al whitepaper correcto. | Media |
| **Fase 4** | Donde se desee máxima coherencia: añadir whitepaper-en, whitepaper-fr, whitepaper-moh a los 62 tokens (o por lotes por categoría). | Baja |
| **Fase 5** | Exponer vía API (opcional) lista de tokens con pre-launch + whitepaper para dashboards y cumplimiento. | Baja |

---

## 6. Referencias en código y documentación

- **Registro de tokens:** `tokens/index.html` — debe enlazar a cada token como pre-launch.
- **Ejemplo pre-launch + whitepaper:** `tokens/06-IGT-MD/` (index.html + whitepaper-en/es/fr/moh).
- **Generar whitepapers faltantes:** `node scripts/generar-whitepapers-faltantes.js` (desde la raíz) — crea whitepaper-es.md en cada token que tenga token.json pero no whitepaper-es.md.
- **Metadatos:** `tokens/<NN-IGT-XXX>/token.json`.
- **Arquitectura banco/departamentos:** `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md`.
- **API whitepaper por símbolo (shop):** `ierahkwa-shop/src/routes/services.js` — `GET /api/services/whitepaper/:symbol`.
- **Principio todo propio:** `.cursor/rules/todo-propio-soberania.mdc`, `PRINCIPIO-TODO-PROPIO.md`.

---

## 7. Resumen ejecutivo

- **Plan:** Todo token IGT emitido debe tener **página de pre-lanzamiento** y **whitepaper propio**.
- **Estado:** Todos los tokens con token.json tienen ya whitepaper-es.md (generado con `scripts/generar-whitepapers-faltantes.js`). Los 62 que faltaban tienen además whitepaper.md para el enlace por defecto.
- **Próximo paso (opcional):** Fase 3–5: revisar enlaces, añadir idiomas en/fr/moh donde se desee, API de listado.
- **Criterio de cierre:** Ningún token listado en `tokens/` sin al menos un whitepaper y pre-launch enlazado. **Cumplido** para los tokens con token.json.

*Documento de referencia: `docs/INDEX-DOCUMENTACION.md` · Tokens: `tokens/`*
