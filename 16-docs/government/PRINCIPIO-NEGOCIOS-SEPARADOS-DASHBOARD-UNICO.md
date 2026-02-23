# Principio: Negocios separados — Dashboard único — Backend compartido

**Gobierno Soberano de Ierahkwa Ne Kanienke**  
**Enfoque profesional:** cada negocio con su propia página; vista unificada solo en el main dashboard; todos comparten el mismo backend y datos para comunicaciones.

---

## Regla

1. **Solo en el main dashboard se mezcla todo.** Es el **único lugar** donde todos los negocios/servicios/tokens aparecen juntos (vista unificada, acceso, estado).
2. **Cada negocio tiene su propia página de negocio independiente.** Identidad propia, URL propia, pre-launch y whitepaper propios. No se fusionan en una sola “página genérica”.
3. **Todos comparten el mismo backend y la misma data** para tener comunicaciones, consistencia y una sola fuente de verdad (APIs, BDET, TradeX, base de datos, mensajería).

---

## Por qué es mejor así

- **Profesional:** Cada negocio se presenta de forma independiente; el usuario ve una marca y una oferta claras por línea.
- **Escalable:** Se añaden o modifican negocios sin mezclar lógica ni diseños; cada uno tiene su página.
- **Comunicación unificada:** Un solo backend y una sola data permiten que todos los sistemas hablen entre sí (settlement, clearing, notificaciones, KYC, wallet).
- **Dashboard como centro de mando:** El main dashboard es el único punto donde “se mezcla todo” para navegación y visión de conjunto; el resto permanece separado.

---

## Resumen

| Ámbito | Cómo funciona |
|--------|----------------|
| **Main dashboard** | Único lugar donde todo se mezcla: vista unificada, enlaces a cada negocio/servicio/token, estado global. |
| **Cada negocio** | Su **propia página de negocio independiente** (pre-launch, whitepaper, oferta, contacto). No se comparte página entre negocios. |
| **Exchanges / trading** | Cada token/negocio por su cuenta: listado propio, pares propios, order book propio. |
| **Backend y data** | **Compartidos por todos**: mismo backend (Node, Banking Bridge, APIs), misma data (registros, settlement, usuarios, KYC) para que haya comunicaciones y consistencia. |

---

## Implicaciones

- **Main dashboard (platform/index.html, dashboard.html):** Muestra enlaces y estado de todos los negocios en una sola vista. Es el **único** lugar donde “se mezcla todo”.
- **Páginas de negocio:** Cada token/negocio tiene su página independiente (ej. `tokens/06-IGT-MD/index.html`, plataformas por servicio). Misma identidad visual del ecosistema, pero contenido y mensaje propios.
- **Backend compartido:** Node (:8545), Banking Bridge, BDET, TradeX, SIIS, base de datos, APIs de plataforma — todos los negocios consumen el mismo backend y la misma data para operar y comunicarse.
- **Exchanges y trading:** Cada IGT/listado con su símbolo, sus pares y su libro de órdenes; no se agrupan en un solo producto.

---

*Referencia: `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md` · `docs/PLAN-NEGOCIOS-TOKENS-PRE-LAUNCH-WHITEPAPER.md` · `docs/INDEX-DOCUMENTACION.md`*
