# Próximos pasos — Soberanía

**Sovereign Government of Ierahkwa Ne Kanienke**  
Checklist y prioridades para seguir fortaleciendo el ecosistema soberano.

---

## 1. Migración de tokens externos

- [ ] Identificar tokens o sistemas emitidos en otra chain (Ethereum, BSC, etc.).
- [ ] Registrar cada uno en ISB (añadir a `platform-tokens.json` o `tokens/XX-IGT-SYMBOL/`).
- [ ] Documentar origen (chain, contrato, snapshot).
- [ ] Quemar o retirar en la chain antigua según política (`docs/ARQUITECTURA-SOBERANIA-ROL-BLOCKCHAIN.md` §7).
- [ ] Exponer estado de migración en dashboard o `GET /api/v1/sovereignty/migracion-protocolos`.

**Referencia:** `node/data/migracion-protocolos.json`, `GET /api/v1/sovereignty/migracion-protocolos`.

---

## 2. Quemas programadas (valor)

- [ ] Definir política de quema por token (p. ej. % mensual o por evento).
- [ ] Usar `POST /api/v1/tokens/:symbol/burn` (body: `{ amount }`) para quemas.
- [ ] Registrar cada quema en Sovereignty Bridge (evento inmutable).
- [ ] Comunicar en dashboard o reporte: total quemado por token y supply circulante.

**Referencia:** Tokens con `burnable: true`, `totalBurned` en `GET /api/v1/tokens`.

---

## 3. Sovereignty Bridge (libro de verdad)

- [x] Endpoint mínimo: `POST /api/v1/sovereignty/bridge/event` para registrar eventos (identidad, gobernanza, quemas).
- [ ] Persistencia en archivo o DB; hash/timestamp por evento.
- [ ] Consulta: `GET /api/v1/sovereignty/bridge/events` (últimos N eventos).
- [ ] Opcional: firma con KMS o escritura en ISB cuando el chain esté disponible.

**Referencia:** `docs/ARQUITECTURA-SOBERANIA-ROL-BLOCKCHAIN.md` §4.

---

## 4. Auditoría de licencias

- [ ] Listar todas las plataformas activas (Banco, Casino, TradeX, etc.).
- [ ] Cruzar con `GET /api/v1/licenses/licenses` y `GET /api/v1/licenses/check?entity=...&type=...`.
- [ ] Dashboard o informe: “Licencias vigentes por plataforma”.
- [ ] Alertas si una licencia está por vencer o revocada.

**Referencia:** `docs/REGLAS-Y-LICENCIAS-PARA-TODOS.md`, `GET /api/v1/sovereignty/reglas-y-licencias`.

---

## 5. Integración Real Estate / Travel (Futurehead)

- [ ] Conectar APIs de viajes (Amadeus/Expedia) según plan de implementación.
- [ ] Conectar motor de casino (NuxGame/SoftSwiss) si se escala.
- [ ] Orquestador de liquidación: bonos casino → créditos viaje/inmueble (firewall de valor).
- [ ] Dashboard único (Super-App) para Citizen: tier, propiedades, puntos casino, próximos viajes.

**Referencia:** `docs/ECOSISTEMA-MODULAR-FUTUREHEAD.md`, `docs/PLAN-IMPLEMENTACION-FUTUREHEAD-2026.md`.

---

## 6. Staking y gobernanza en chain

- [ ] Diseñar contratos o módulos de staking para IGT/IRHK (según whitepaper).
- [ ] Votaciones IEP: resultados registrados en Sovereignty Bridge o ISB.
- [ ] Recompensas y tier (Starter → Tycoon) ligados a participación en chain.

**Referencia:** `docs/WHITEPAPER-FUTUREHEAD-TRUST-ECOSYSTEM-2026.md`.

---

## Resumen

| Prioridad | Tarea | Estado |
|-----------|--------|--------|
| Alta | Migración tokens externos a ISB | Pendiente |
| Alta | Quemas programadas + registro en Bridge | Pendiente |
| Media | Sovereignty Bridge (eventos mínimos) | Implementado endpoint |
| Media | Auditoría licencias + dashboard | Pendiente |
| Media | Casino: canje códigos promocionales | Implementado en UI |
| Baja | Real Estate / Travel / Super-App | Según plan 2026 |
| Baja | Staking y gobernanza en chain | Según whitepaper |

---

*Actualizado a partir de reglas y licencias para todos, arquitectura de soberanía y hub único (8545).*
