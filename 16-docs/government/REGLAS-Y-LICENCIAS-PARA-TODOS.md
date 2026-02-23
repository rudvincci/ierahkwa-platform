# Reglas y licencias para todos

**Sovereign Government of Ierahkwa Ne Kanienke**  
Un solo marco: reglas y licencias que aplican a ciudadanos, plataformas, departamentos y entidades.

---

## 1. Reglas generales (para todos)

| Regla | Descripción |
|-------|-------------|
| **Blockchain** | Todos los tokens son de nuestro blockchain (ISB). Lo emitido en otro protocolo debe migrarse a ISB. |
| **Hub único** | Cada plataforma opera en su propio lugar; el hub (8545) solo enlaza. No se mezclan operaciones entre sí. |
| **Licencia** | Cualquier actividad comercial, financiera, de juego o de plataforma requiere **licencia vigente** emitida por la Autoridad Soberana. |
| **EULA y documentación** | Cumplimiento del EULA y de la documentación oficial (docs/). No sublicenciar ni transferir sin autorización escrita. |
| **Transparencia** | Identidad, gobernanza y certificaciones pueden registrarse en ISB para trazabilidad. |

---

## 2. Reglas por tipo

### Ciudadanos
- Respetar EULA y tipo de licencia del software que usen.
- Identidad y tier (Starter → Tycoon) pueden registrarse en blockchain para beneficios y gobernanza.
- Uso de plataformas (Casino, Exchange, Banco) según términos de cada servicio y edad legal (21+ donde aplique).

### Plataformas
- Cada plataforma (Casino, TradeX, Banco, SWIFT, etc.) debe tener **licencia operativa vigente**.
- Servicios completos en su propio puerto/dominio; el hub solo enlaza.
- Tokens de plataforma: 10 trillones por token en ISB; quemables para valor.

### Departamentos
- Cada departamento gubernamental opera con token IGT propio (10T, ISB).
- Reglas de gasto, auditoría y gobernanza según normativa interna y Comptroller.

### Banco
- BDET, Treasury, Financial Center, SWIFT (8590): operan bajo **licencia bancaria/financiera**.
- Cuentas y movimientos en sistemas dedicados; el chain certifica identidad y permisos.

### Casino
- **Licencia de Casino/Gaming** obligatoria (Sovereign License Authority).
- RNG y sesiones server-side; bonos separados del capital real (Futurehead firewall).

### Exchange
- TradeX (5071) u otro exchange: **licencia de Cryptocurrency Exchange** o equivalente.
- Órdenes y liquidez en sistema dedicado; liquidación o certificación en chain si se define.

---

## 3. Licencias

Todas las entidades que operen en jurisdicción soberana deben contar con la **licencia correspondiente** a su actividad.  
**Emisión:** Sovereign License Authority (SLA-INK).  
**Referencia técnica:** módulo `license-authority.js`; categorías COMMERCIAL, FINANCIAL, PROFESSIONAL, TECHNOLOGY, INDUSTRIAL, TRANSPORTATION, RETAIL, DEFENSE, MEDIA, HEALTHCARE, EDUCATION.

### Tipos relevantes para las plataformas

| Tipo | Nombre | Aplica a |
|------|--------|----------|
| BANKING | Banking License | BDET, bancos |
| CASINO | Casino/Gaming License | Casino |
| CRYPTO_EXCHANGE | Cryptocurrency Exchange License | TradeX, exchanges |
| MONEY_TRANSMITTER | Money Transmitter License | Pagos, wallet |
| PAYMENT_PROCESSOR | Payment Processor License | Procesamiento de pagos |
| TELECOM | Telecommunications License | Telecom |
| STREAMING | Streaming Service License | Streaming |
| LOTTERY | Lottery Operator License | Lotto, raffle |

---

## 4. Documentos oficiales

- **Certificado de licencia (plantilla):** `docs/CERTIFICADO-LICENCIA.md`
- **EULA / Contrato de licencia:** `docs/EULA-CONTRATO-LICENCIA.md`
- **Derechos, acceso y claves:** `docs/DERECHOS-ACCESO-CLAVES.md`
- **Arquitectura soberanía y blockchain:** `docs/ARQUITECTURA-SOBERANIA-ROL-BLOCKCHAIN.md`
- **Migración desde otros protocolos:** API `GET /api/v1/sovereignty/migracion-protocolos`

---

## 5. API

- **Reglas y licencias (este documento en JSON):** `GET /api/v1/sovereignty/reglas-y-licencias`
- **Licencias emitidas / Authority:** consultar módulo license-authority o rutas `/api/v1/licenses` según despliegue.

---

*Datos completos en `RuddieSolution/node/data/reglas-y-licencias.json`.*
