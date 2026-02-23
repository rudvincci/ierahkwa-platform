# Asegurar 100% y production en cada plataforma

**Gobierno Soberano de Ierahkwa Ne Kanienke**

Objetivo: que **todo trabaje al 100%** y cada plataforma esté **lista para producción**. Cada negocio tiene su propia página independiente; todos comparten el mismo backend y data; solo en el main dashboard se mezcla todo. Ver `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md`.

---

## 1. Verificaciones obligatorias (dos pasos)

### Paso 1 — Código y configuración (links, rutas, data de plataforma)

Desde la **raíz del repo**:

```bash
node scripts/verificar-plataforma-100-production.js
```

- **Qué comprueba:** Enlaces en `platform-links.json` apuntan a archivos que existen; rutas en `platform-routes.js` tienen su HTML en `platform/`; archivos de data en `platform/data/` (platform-links, government-departments, etc.); departamentos.
- **Resultado esperado:** Exit 0, **0 links rotos, 0 rutas rotas, 0 data faltante**.
- **Salida:** `docs/REPORTE-VERIFICACION-COMPLETA-100-PRODUCTION.md` y `RuddieSolution/node/data/verificacion-100-production.json`.

### Paso 2 — Datos y endpoints de producción (backend compartido)

Con el **Node (8545) en marcha** (ej. `./start.sh`):

```bash
cd RuddieSolution/node && node scripts/verificar-production-live.js
```

- **Qué comprueba:** Archivos de datos en `node/data/` (resumen-soberano, bank-registry, casino, whitepaper-futurehead, etc.); endpoints `/health`, `/ready`, `/live`, `/api/v1/production/*`, sovereignty, casino, BDET bank-registry.
- **Resultado esperado:** Exit 0, todos los archivos presentes y todos los endpoints responden OK.
- **Sin servidor:** Los checks de archivos pasan; los de endpoints fallan hasta que Node esté arriba.

**Ambos con exit 0** → plataforma 100% lista para producción desde el punto de vista de código, data y APIs del backend compartido.

---

## 2. Script único (ejecutar las dos verificaciones)

Desde la **raíz del repo**:

```bash
./scripts/asegurar-100-production.sh
```

- Ejecuta primero `verificar-plataforma-100-production.js` y luego `verificar-production-live.js` (desde `RuddieSolution/node`).
- **Exit 0** solo si ambas pasan; **exit 1** si alguna falla.
- Útil para CI/CD o antes de cada despliegue.

---

## 3. Por qué “cada plataforma” trabaje al 100%

| Ámbito | Qué asegurar |
|--------|---------------|
| **Cada negocio / token** | Tiene su **propia página** (pre-launch, whitepaper). La verificación de links y rutas comprueba que esas páginas existan y estén enlazadas. |
| **Main dashboard** | Es el único lugar donde todo se mezcla; los enlaces del dashboard a cada plataforma/negocio se verifican en el Paso 1. |
| **Backend compartido** | Un solo Node, Banking Bridge, BDET, data en `node/data/`. El Paso 2 comprueba que los datos y endpoints de ese backend estén completos y respondan. |
| **Production** | `.env` con JWT y NODE_ENV=production; archivos de datos requeridos (incl. `bank-registry.json`); endpoints `/api/v1/production/ready` y `status` operativos. |

---

## 4. Checklist rápido go/no-go

- [ ] `node scripts/verificar-plataforma-100-production.js` → **exit 0** (0 fallos).
- [ ] Node (8545) en marcha (`./start.sh` o PM2).
- [ ] `cd RuddieSolution/node && node scripts/verificar-production-live.js` → **exit 0**.
- [ ] (Opcional) `./scripts/asegurar-100-production.sh` → **exit 0**.

Si todo pasa → **todo al 100% y production por cada plataforma** (cada una con su página, todas usando el mismo backend y data).

---

## 5. Documentación relacionada

| Tema | Documento |
|------|------------|
| Checklist definitivo 100% production | `docs/PLATAFORMA-100-PRODUCTION.md` |
| Producción resumen | `docs/PRODUCTION-LISTO.md` |
| 100% Production Live (datos + endpoints) | `docs/100-PRODUCTION-LIVE.md` |
| Negocios separados, dashboard único, backend compartido | `docs/PRINCIPIO-NEGOCIOS-SEPARADOS-DASHBOARD-UNICO.md` |
| Reporte verificación (links/rutas/data) | `docs/REPORTE-VERIFICACION-COMPLETA-100-PRODUCTION.md` |

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
