# Due Diligence Financiero — Jesco Construction Corporation

**Documento:** DD financiero para evaluación de contraparte  
**Empresa:** Jesco Construction Corporation (JESCO)  
**Fecha DD:** Febrero 2026  
**Fuente base:** KYC Know Your Client (Febrero 2026) — FINRA 2090/2111, FinCEN AML  

---

## 1. Resumen ejecutivo

| Criterio | Valor |
|----------|-------|
| Nombre | Jesco Construction Corporation |
| Tipo | C Corporation privada |
| Fundación | Enero 1993 (33 años operando) |
| Propietario | John E. Shavers — 100% |
| Empleados | ~2,700 domésticos + ~15,000 internacionales |
| Bonding | Hasta $1,200M |
| Regulación | Contratista clasificado del US DoD |

JESCO cumple KYC/AML y presenta indicadores cualitativos de solvencia. El DD financiero completo requiere estados financieros y datos operativos adicionales.

### Evaluación de capacidad financiera (sistema soberano)

**Ejecutar DD con el sistema:** `node scripts/ejecutar-dd-jesco.js`  
Usa Compliance Watch (sanciones/PEP) + Capacity (capacidad financiera). Registro en `RuddieSolution/node/data/dd-executions/jesco-YYYY-MM-DD.json` y en `compliance-watch/checks-log.json`.

| Resultado | Valor |
|-----------|-------|
| **Score** | 100/100 |
| **Nivel** | ALTA |
| **Etiqueta** | Capacidad financiera alta |
| **Exposición máxima sugerida** | USD 180,000,000 (15% del bonding) |
| **Compliance (listas propias)** | Sin hits sanciones ni PEP (empresa + John E. Shavers) |
| **Estado** | **PENDIENTE DE VERIFICACIÓN** — no usar para decisiones hasta verificar |
| **Fuente** | SOVEREIGN_DD_SYSTEM (capacity-api + compliance-watch-api) |

**Importante:** El resultado del sistema no debe usarse para aprobar operaciones hasta que un responsable (compliance officer) verifique: (1) cruce con listas oficiales de sanciones/PEP que correspondan, (2) validez de los datos KYC utilizados, (3) criterio de capacidad. El registro de verificación se guarda en `node/data/dd-executions/verifications.json`.

---

## 2. Identificación corporativa

### Datos legales

| Campo | Valor |
|-------|--------|
| Razón social | Jesco Construction Corporation |
| Domicilio fiscal | 46 Flint Creek Road, Wiggins, MS 39577 |
| Sede operativa | 1515 Engineers Road, Belle Chasse, LA 70037 |
| Incorporación | 26 de enero de 1993 |
| Tipo societario | C Corporation |
| Licencia Louisiana | 31677 (válida hasta 20-Jun-2027) |
| Web | www.JescoConstruction.com |
| Teléfono | (850) 255-5300 |

### Contactos

| Rol | Nombre | Email |
|-----|--------|--------|
| Presidente | John E. Shavers | — |
| Secretaria Corporativa | Ann B Shavers | Ann.Shavers@JescoConstruction.com |

---

## 3. Estructura de propiedad

| Elemento | Valor |
|----------|--------|
| Acciones autorizadas | 1,000 acciones comunes |
| Acciones emitidas | 100 comunes |
| Propietario único | John E. Shavers (100%) |
| Cargo | Chairman y President |

**Beneficiario final:** John E. Shavers — titular de pasaporte USA 565735035, vigente hasta enero 2028.

---

## 4. Perfil operativo (KYC)

### Capacidades

- Servicios profesionales al US Department of Defense
- Contratista clasificado aprobado por el Gobierno de EE.UU.
- Empresa de Veterano con discapacidad y titularidad nativa americana (Diversity Scorecard = 33)
- Una de las flotas marinas privadas más grandes del Golfo de México
- Certificaciones FEMA > 100
- Capacidades de ingeniería y gestión ambiental (monitoreo de aguas subterráneas, protección de tormentas, calidad del aire, diseño ambiental)

### Activos cualitativos (KYC)

- Barcazas y remolcadores, embarcaciones de trabajo, airboats, bulldozers, excavadoras, grúas, skimmers, transportadores, camiones, dump trucks
- Sin litigios relevantes o materiales indicados en el KYC
- Seguros para proyectos de distintos tamaños
- Calificación de seguridad descrita como impecable

---

## 5. Indicadores financieros (inferidos del KYC)

> **Nota:** El KYC no incluye estados financieros ni cifras de ingresos/resultados. Las siguientes estimaciones se basan en magnitudes descritas en el documento.

| Indicador | Valor estimado / inferido | Fuente |
|-----------|---------------------------|--------|
| Línea de bonding | $1,200M | KYC |
| Empleados totales | ~17,700 | 2,700 domésticos + 15,000 intl. |
| Tamaño (orden) | Grandes proyectos federales | Contratista clasificado DoD |
| Riesgo litigios | Bajo | No litigios materiales en KYC |
| Calificación seguridad | Impecable | KYC |
| Regulación / licencias | Vigente | Licencia LA hasta 2027 |

### Métricas financieras a solicitar

Para completar el DD:

1. **Balance general** — últimos 3 años
2. **Estado de resultados** — últimos 3 años
3. **Estado de flujos de efectivo** — últimos 3 años
4. **Informe de auditoría** — si existe
5. **Deuda consolidada** y vencimientos
6. **Contratos en curso** y montos
7. **Backlog** y proyecciones de ingresos

---

## 6. Cumplimiento normativo

### KYC/AML

- Cumple FINRA 2090 (Know Your Customer) y 2111 (Suitability)
- Cumple estándares FinCEN KYC para prevención de lavado de dinero
- Incluye formulario de beneficiarios reales 31 CFR § 1010.230
- Declaración bajo pena de perjurio del 9 de febrero de 2026

### Documentación de soporte

- Pasaporte USA del propietario
- Copia de incorporación
- Licencia Louisiana
- Certificación FinCEN sobre beneficiarios reales

---

## 7. Riesgos identificados

| Riesgo | Nivel | Comentario |
|--------|-------|------------|
| Concentración en un propietario | Medio | 100% propiedad y control en una sola persona |
| Dependencia de contratos federales | Medio | Enfoque significativo en DoD; variabilidad según ciclos presupuestarios |
| Exposición internacional | Bajo–Medio | ~15,000 empleados internacionales; evaluar jurisdicciones y riesgos AML |
| Litigios | Bajo | Sin material identificado en el KYC |

---

## 8. Conclusión y recomendaciones

### Conclusión preliminar

Jesco Construction Corporation presenta un perfil corporativo y de cumplimiento sólido según el KYC disponible. La capacidad de bonding ($1.2B), la escala de empleo y el estatus de contratista clasificado del DoD son señales de capacidad operativa y financiera. El DD financiero completo requiere estados financieros y datos operativos adicionales.

### Recomendaciones

1. **Documentación adicional:** Solicitar estados financieros auditados (o al menos revisados) de los últimos 3 años.
2. **Referencias:** Pedir referencias bancarias y de suretys (aseguradoras de bonos).
3. **Verificación:** Confirmar vigencia de licencias (Louisiana y demás estados donde opere).
4. **Beneficiarios reales:** Verificar que el formulario FinCEN coincida con la estructura actual de control.
5. **Scope of work:** Para transacciones concretas, evaluar garantías, condiciones de pago y plazos.

---

## 9. Verificación obligatoria

**No se puede confiar en el DD sin verificación.** El sistema solo produce un resultado preliminar. Antes de usar para decisiones (contratos, límites, aprobaciones):

1. **Compliance:** Verificar empresa y beneficiario contra las listas que apliquen (propias actualizadas, o cruce con fuentes oficiales según política).
2. **Datos KYC:** Confirmar que bonding, empleados, litigios y licencias reflejan documentación vigente.
3. **Registro:** Anotar la verificación en `RuddieSolution/node/data/dd-executions/verifications.json` (company, verifiedBy, verifiedAt, notes). Solo entonces el DD se considera verificado para uso. Comando: `node scripts/verificar-dd-jesco.js "Nombre del verificador" "Notas"`.

---

## 10. Uso del sistema de DD

**Ejecutar DD completo (recomendado):**
```bash
node scripts/ejecutar-dd-jesco.js
```
Usa el mismo código que las APIs: Compliance Watch (sanciones/PEP para empresa y beneficiario) y Capacity (capacidad financiera). Escribe en `node/data/dd-executions/jesco-YYYY-MM-DD.json` y actualiza `node/data/compliance-watch/checks-log.json`.

Solo capacidad (sin compliance):
```bash
node scripts/evaluar-jesco-capacidad.js
```

Vía API (con Node 8545 corriendo):
- `POST /api/v1/capacity/corporate` — capacidad
- `POST /api/v1/compliance-watch/check` — sanciones/PEP
- `POST /api/v1/ml/risk` — riesgo AML/KYC (Python)

---

## 11. Anexos

- KYC Jesco Construction Corporation — Febrero 2026 (16 páginas)
- Certificación FinCEN Beneficial Owners
- Datos de contacto según KYC

---

*Documento elaborado para evaluación de contraparte. Sovereign Government of Ierahkwa Ne Kanienke.*
