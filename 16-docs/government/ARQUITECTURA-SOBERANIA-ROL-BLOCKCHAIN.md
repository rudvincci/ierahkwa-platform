# Arquitectura de soberanía — Rol del blockchain

**Sovereign Government of Ierahkwa Ne Kanienke**  
Documento de referencia: qué va al blockchain soberano (ISB) y qué permanece en sistemas dedicados. Objetivo: **soberanía clara** sin mezclar plataformas ni duplicar lo que ya funciona.

---

## 1. Principio: hub único, servicios completos en su lugar

- **Main dashboard (8545)** = punto de entrada único; enlaza a cada servicio.
- **Cada plataforma** (Banco, SWIFT 8590, TradeX 5071, Casino, Platform) está **completa en su propio lugar**.
- **No hay confusión**: cada una crece en valor, trabajo, seguridad y transparencia por separado.
- El blockchain **no reemplaza** esos sistemas; actúa como **núcleo de identidad, gobernanza y certificación**.

---

## 2. Qué SÍ va al blockchain soberano (ISB)

| Área | Uso en chain | Motivo |
|------|----------------|--------|
| **Identidad y reputación** | Ciudadano, tier, permisos, niveles (Starter → Tycoon) | Una sola fuente de verdad soberana; las apps consultan quién es quién. |
| **Registro inmutable** | Licencias emitidas, contratos, auditoría, sellos de tiempo | Prueba de existencia y trazabilidad que no se puede alterar. |
| **Tokens soberanos** | IRHK, FHTC, IGT-* (todos los tokens) | **Todos los tokens son de nuestro blockchain (ISB)** — departamentos, plataformas e industrias; emisión y gobernanza bajo control soberano. |
| **Gobernanza** | Votaciones, propuestas IEP, resultados | Transparencia y consenso verificable. |
| **Prueba de existencia** | Hashes de documentos, certificados, eventos clave | No guardar el documento en chain; solo el compromiso (hash). |

El blockchain brilla en **consenso, inmutabilidad y confianza distribuida**. Ahí es donde aporta soberanía real.

---

## 3. Qué NO migrar al blockchain (sigue en su lugar)

| Sistema | Dónde vive | Motivo |
|---------|------------|--------|
| **BDET / Treasury / Financial Center** | Banco (8545, 8590) | Cuentas, movimientos, liquidez: mejor en bases de datos y APIs; el chain certifica identidad y permisos. |
| **Rust SWIFT (8590)** | Servicio bancario dedicado | Mensajería SWIFT MT/MX: volumen y estándares; el chain no reemplaza el mensajero. |
| **TradeX (5071)** | Exchange dedicado | Órdenes, matching, liquidez: alta frecuencia; el chain para liquidación o registro de resultados si se decide. |
| **Casino** | Motor en Node (8545) | RNG, sesiones, wallet de bonos: operación en tiempo real; el chain para eventos de auditoría o rollover si se requiere. |
| **SIIS, settlement** | Sistemas de clearing | Flujos operativos; el chain para acuerdos o certificación, no para cada transacción. |

Migrar “todo” al chain aumentaría costo y complejidad sin ganar soberanía; cada plataforma ya aporta valor en su dominio.

---

## 4. Puente de soberanía (Sovereignty Bridge)

- **Ubicación sugerida**: módulo en el Node (8545) o servicio dedicado que hable con ISB.
- **Función**: registrar en blockchain **solo** lo que importa para soberanía:
  - Eventos de identidad (alta/baja de ciudadano, cambio de tier).
  - Resultados de gobernanza (votaciones, propuestas aprobadas).
  - Hashes de contratos, licencias, documentos relevantes.
  - Emisión y quema de tokens soberanos (según reglas de gobernanza).
- **No duplica** la lógica de Banco, Casino o Exchange; solo **certifica** ante el libro soberano.

```text
[ Banco / Casino / Exchange ]  --->  [ Sovereignty Bridge ]  --->  [ ISB ]
         (operación)                        (firma/registro)         (libro de verdad)
```

---

## 5. Transparencia y control

- **Documentar** qué se escribe en chain y qué no (este documento + políticas internas).
- **Dashboards** que muestren: eventos recientes en chain, estado de identidad, gobernanza.
- **Reglas claras**: “esto va al blockchain, esto no”, para que soberanía = **claridad + control**, no “todo en una sola base”.

---

## 6. Resumen

| Pregunta | Respuesta |
|----------|-----------|
| ¿Migrar todo al blockchain? | **No.** Solo identidad, gobernanza, tokens soberanos y certificación. |
| ¿Dónde vive la operación? | Banco, SWIFT, TradeX, Casino, Platform: **cada uno en su lugar y puerto**. |
| ¿Qué une todo? | **Hub (8545)** + **blockchain como libro de verdad soberano** + **Sovereignty Bridge** opcional. |
| ¿Objetivo? | Soberanía = **valor, trabajo, seguridad y transparencia** por plataforma, sin confusión. |

---

## 7. Migración desde otros protocolos

**Regla:** Si un token o sistema se hizo con otro protocolo (Ethereum, BSC, Polygon, etc.), **hay que migrarlo a nosotros (ISB)**.

- **Objetivo:** Un solo libro de verdad soberano. Todo lo que sea “nuestro” (gobierno, departamentos, plataformas, industrias) debe vivir en **Ierahkwa Sovereign Blockchain (ISB)**, no en cadenas ajenas.
- **Qué migrar:** Tokens (IGT-*, IRHK, FHTC, etc.), identidad, gobernanza, certificaciones. No la operación diaria de bancos/exchange/casino (esos siguen en sus sistemas; el chain los certifica).
- **Cómo:**  
  1. Registrar el token/activo en ISB con el mismo símbolo/nombre y supply acordado (ej. 10T).  
  2. Documentar el origen (chain anterior, contrato, snapshot).  
  3. Quemar o retirar en la chain antigua según política (bridge unidireccional o cierre).  
  4. Los titulares reciben saldo en ISB; todo queda bajo nuestro estándar IGT-20 y Chain ID 77777.
- **Referencia técnica:** Node (8545) — `GET /api/v1/tokens`, `POST /api/v1/tokens/:symbol/burn`; datos en `node/data/platform-tokens.json` y carpetas `tokens/`. Política en `node/data/migracion-protocolos.json`. Nuevos tokens migrados se añaden ahí y se cargan en el Node.

*Referencias: ECOSISTEMA-MODULAR-FUTUREHEAD.md, hub único en node/public/index.html, PRODUCTION-LISTO.md.*
