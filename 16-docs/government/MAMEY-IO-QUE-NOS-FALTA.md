# Mamey.io / Mamey-io — Estudio y lo que nos falta

**Referencia:** [GitHub Org Mamey-io](https://github.com/orgs/Mamey-io/dashboard) · Ecosistema soberano IERAHKWA (Chain ID 777777)

---

## 1. Qué es Mamey (en nuestro proyecto)

- **Mamey** = infraestructura blockchain y servicios soberanos (Rust + C# + SDKs).
- **MameyNode** = nodo blockchain en Rust (puerto 8545, API tipo Ethereum).
- **Servicios Mamey:** Identity (5001), ZKP (5002), Treasury (5003), Notifications (5004), Sagas (5005), TreatyValidators (5006), Whistleblower (5007), KeyCustodies (5008), GovernanceAI (5009), TemplateEngine (5010), Maestro (5011), FileReader (5083).
- **SDKs:** TypeScript (`@mamey-io/mamey-sdk`), Go (`github.com/mamey-io/mamey-sdk-go`), Python (`mamey_sdk`).
- En el repo está todo el código (Mamey/, MAMEY-FUTURES, Banking.MameyNode); la org **Mamey-io** en GitHub es donde se podría publicar y organizar repos/SDKs.

---

## 2. Lo que SÍ tenemos

| Área | Estado |
|------|--------|
| Código MameyNode (Rust), MameyFramework (C#), servicios SICB/Identity/ZKP/Treasury | ✅ En `Mamey/` |
| SDK TypeScript (blockchain, identity, treasury, ZKP) | ✅ `Mamey/sdks/typescript/` |
| SDK Go, SDK Python (estructura) | ✅ En `Mamey/sdks/` |
| MameyNodeClient C# para Node | ✅ `MameyFramework/Blockchain/MameyNodeClient.cs` |
| Gateway que unifica TradeX, NET10, Banking, etc. | ✅ `mamey-gateway-server.js` |
| Stubs SICB en Node (treasury, emissions, treaty) | ✅ `sicb-mamey-stubs.js` (modo standalone; “cuando MameyNode/SICB estén listos, se conectan”) |
| Docs internos (README Mamey, servicios creables, reporte IERAHKWA vs Mamey) | ✅ Varios .md en repo |
| Integración Node ↔ Mamey (proxy, rutas) | ✅ Parcial (gateway + stubs) |

---

## 3. Lo que NOS FALTA (resumen)

### 3.1 Tests y calidad (vs referencia “Mamey/Manolo”)

| Ítem | Referencia (reporte) | Nosotros |
|------|----------------------|----------|
| Tests unitarios | 2.530+ (Rust 85 crates, .NET, etc.) | ~50 (Rust SWIFT, .NET Banking, Node) |
| Tests E2E integración | 138 (settlement, loans, cross-border, BFT, PQC, compliance, etc.) | 0 (solo smoke/health) |
| Benchmarks TPS | 42K TPS, settlement 10–20K/s | No medidos |
| PQC (Post-Quantum) | ML-KEM-768, ML-DSA, híbrido | No en stack actual |
| Tolerancia a fallos / BFT | Byzantine, partition, failover documentados | No documentado |
| Conformidad protocolos | SWIFT MT103/202, ISO 20022, SEPA, ACH, FIX, ISO 8583 | SWIFT MT/MX parcial (Rust) |

**Acciones sugeridas:** Aumentar tests E2E (flujos pago, préstamos, remesas), añadir benchmarks TPS, evaluar PQC y documentar BFT/failover.

### 3.2 SDK y publicación (Mamey-io)

| Ítem | Estado |
|------|--------|
| Paquete npm `@mamey-io/mamey-sdk` | No publicado (solo nombre en `Mamey/sdks/typescript/package.json`) |
| Repo Go `github.com/mamey-io/mamey-sdk-go` | Código local; no verificado si está en GitHub org |
| Paquete Python en PyPI | No verificado |
| Repos en GitHub org Mamey-io | Dashboard existe; no hay lista pública de repos (login requerido) |

**Acciones sugeridas:** Publicar SDK TypeScript en npm (scope `@mamey-io`), subir o enlazar SDK Go/Python en la org, crear repos en Mamey-io para MameyNode, SDKs y docs.

### 3.3 Integración Node ↔ Mamey real

| Ítem | Estado |
|------|--------|
| SICB/Treasury/Identity reales | Node usa stubs locales; “cuando MameyNode/SICB estén listos, se conectan” |
| Variables `MAMEY_NODE_URL` / `SICB_URL` | Documentadas en stubs; falta uso consistente y despliegue con MameyNode + servicios |
| Servicios 5001–5011 + 5083 | Definidos en Mamey README; no está verificado que todos estén levantados e integrados al Node en producción |

**Acciones sugeridas:** Conectar Node a MameyNode + servicios (Identity, ZKP, Treasury) vía env; sustituir stubs por clientes reales donde corresponda; documentar en RUNBOOK qué servicios Mamey deben estar up.

**Implementado en repo:** Variables documentadas en `docs/MAMEY-ENV-VARS.md`. Script `RuddieSolution/node/scripts/check-mamey-integration.js` para comprobar health, stats y SICB. Test E2E `RuddieSolution/node/tests/e2e-mamey-integration.test.js` (ejecutar con Node arriba: `TEST_BASE_URL=http://localhost:8545 npm test -- e2e-mamey-integration`).

### 3.4 Documentación pública “Mamey.io”

| Ítem | Estado |
|------|--------|
| API reference pública (docs.mamey.io o similar) | No hay |
| Landing o sitio “Mamey.io” | No verificado |
| Documentación para desarrolladores externos | Solo README y docs dentro del repo |

**Acciones sugeridas:** Definir si Mamey.io será producto público; si sí, añadir API reference y guías (por ejemplo en GitHub Pages o docs en repo de la org).

### 3.5 Corrección ya hecha en el repo

- **SDK TypeScript:** Treasury usaba puerto 5002 y ZKP 5003; según Mamey README, Treasury = 5003 y ZKP = 5002. Corregido en `Mamey/sdks/typescript/src/index.ts`.

---

## 4. Conclusión del reporte IERAHKWA vs Mamey

- **Mamey (infra):** mejor en tests, TPS, PQC, protocolos y resiliencia.
- **IERAHKWA (producto):** mejor en plataformas, UX, AI y alcance funcional.
- **Objetivo:** usar Mamey como backbone e IERAHKWA como capa de aplicaciones.

Para alinearnos con eso nos falta sobre todo: **tests E2E y de protocolos, benchmarks, opcionalmente PQC/BFT, publicación de SDKs en Mamey-io, y conectar el Node a MameyNode y servicios SICB/Identity/ZKP de forma real (no solo stubs).**

---

## 5. Referencias en el repo

- `Mamey/README.md` — Servicios, puertos, APIs, SDK.
- `Mamey/SERVICIOS-SEGURIDAD.md` — Seguridad e integración.
- `REPORTE-DETALLADO-IERAHKWA-VS-MAMEY-MANOLO.md` — Comparativa tests/rendimiento.
- `RuddieSolution/platform/docs/SERVICIOS-CREABLES-DESDE-MAMEY.md` — Servicios que se pueden construir encima de Mamey.
- `RuddieSolution/node/routes/sicb-mamey-stubs.js` — Stubs SICB y hint de integración con MameyNode/SICB.

---

*Documento generado a partir del estudio del ecosistema Mamey y la org Mamey-io. Actualizar cuando se publiquen SDKs o se cierre integración Node ↔ Mamey.*
