# Análisis: DAO Widget (CodeCanyon) y su aporte al nodo Ierahkwa

**Fecha:** 2026  
**Referencia:** Dao widget - Governance and proposals for your crypto token (CodeCanyon / OnOut)  
**Objetivo:** Evaluar si suma valor al **Ierahkwa Futurehead Mamey Node** y al ecosistema IGT.

---

## 1. Resumen del widget (CodeCanyon)

| Aspecto | Detalle |
|---------|---------|
| **Funciones** | Crear propuestas, votar, listar votos |
| **Wallet** | Metamask |
| **Gas** | Votación **sin gas** (usa Snapshot.org) |
| **Backend** | API de **Snapshot.org** (off-chain) |
| **Redes** | ERC20, BEP20, EVM en general |
| **Voting Period** | Configurable por creador o por propuesta de token holders |
| **Temas** | Light / Dark |

---

## 2. Estado actual en nuestro proyecto

### 2.1 Tokens y plataformas definidos (sin implementación)

| Token | Descripción | URL definida | Estado real |
|-------|-------------|--------------|-------------|
| **IGT-DAO** (98) | DAO Platform | dao.ierahkwa.gov | **No existe** |
| **IGT-VOTE** (92) | Voting Platform | vote.ierahkwa.gov | **No existe** |
| **IGT-GOV** (43) | Governance Token | - | **Solo metadata** |

En `platform-services.json` **no hay** servicio DAO ni Vote; en `ierahkwa-shop` figuran como `status: 'planned'`, `url: null`, `active: false`.

### 2.2 Nodo (Ierahkwa Futurehead Mamey Node)

- **RPC:** `eth_*`, `igt_*` (chainId 77777, ISB).
- **APIs REST:** `/api/v1/tokens`, `/api/v1/accounts/:address`, `/api/v1/blocks`, `/api/v1/transactions`, etc.
- **No hay:** endpoints de propuestas, votos ni gobernanza.
- **Governance en config:** `node/config.toml` y `ierahkwa-futurehead-mamey-node.json` tienen `governance: true` como *flag*, pero no lógica.

### 2.3 Contrato “Governance” (conceptual)

En `ierahkwa-shop/src/routes/node.js`:

```js
GOVERNANCE: {
  name: 'Sovereign Governance Contract',
  type: 'DAO',
  address: '0x0000000000000000000000000000000000002000',
  features: ['propose', 'vote', 'execute', 'delegate']
}
```

Es **solo metadata** para la UI; no hay smart contract ni backend que implemente propose/vote/execute/delegate.

---

## 3. ¿Qué aporta el DAO widget a nuestro nodo?

### 3.1 Lo que SÍ encaja y suma valor

1. **Cubre un hueco real:** IGT-DAO e IGT-VOTE están definidos y no tienen UI ni lógica. El widget ofrece exactamente:
   - Formulario “Create proposal”
   - “Vote for proposal”
   - “List of votes”
   - Peso del voto según holding del token (concepto Snapshot/ERC20).

2. **Arquitectura reutilizable:** Create / Vote / List es el flujo estándar de gobernanza. Nos sirve como modelo aunque no usemos Snapshot.

3. **Votación sin gas:** Muy alineado con “Zero fees for government transactions” de ISB. Si implementamos un backend propio tipo Snapshot (votos off-chain, verificación con balance en nuestro RPC), encaja con la filosofía del proyecto.

4. **Metamask + RPC personalizado:** Nuestro nodo expone RPC compatible con `eth_*`. Se puede añadir ISB en Metamask (Custom RPC, chainId 77777, URL del nodo). Cualquier frontend que use `ethers.js`/`web3` + Metamask podría:
   - Conectarse a ISB.
   - Leer `eth_getBalance` o balances de “tokens” (según cómo modelemos IGT en el nodo).

5. **EVM/ERC20:** Si en el futuro IGT se expone como ERC-20 en ISB (o en una red EVM puenteada), el patrón del widget (balance = poder de voto) se puede reusar.

6. **Tema dark:** Coincide con el dashboard del nodo y la plataforma (dark theme).

7. **Casos de uso que ya contemplan:** Colectivos, venture, fondos, política, fan ownership, etc. Son cercanos a uso gobierno soberano y comunidad.

### 3.2 Limitaciones importantes (Snapshot + ISB)

1. **Snapshot.org no conoce Ierahkwa ISB:**  
   Snapshot está pensado para Ethereum, BSC, Polygon, etc. **No** tiene “Ierahkwa Sovereign Blockchain” como red. Por tanto:
   - El widget **tal cual** (dependiente 100 % de Snapshot) **no puede** usar IGT ni el nodo ISB directamente.

2. **Uso directo del widget solo si IGT está en una red soportada:**  
   Si más adelante:
   - Se emite IGT (o un wrapped) en Ethereum/BSC/Polygon, o  
   - Se usa un snapshot/space en otra red y se “mapea” a IGT,  
   entonces el widget de CodeCanyon podría funcionar **con esa red**, no con ISB.

3. **OnOut/CodeCanyon:** Habría que revisar en la documentación del producto si permite:
   - Custom strategy / backend alternativo a Snapshot, o  
   - Solo Snapshot.

---

## 4. Opciones de integración

### A) Comprar el widget y usarlo en redes EVM “standard”

- **Cuándo tiene sentido:** Si IGT (o un wrapped) se emite en Ethereum, BSC, Polygon, etc.
- **Ventaja:** UI y flujo listos; votación sin gas vía Snapshot.
- **Desventaja:** No conecta con ISB ni con el nodo Mamey de forma nativa.

### B) Inspirarse en el widget y construir un “DAO Ierahkwa” propio

- **Backend en el nodo (o microservicio):**
  - `GET /api/v1/dao/proposals`
  - `POST /api/v1/dao/proposals`
  - `GET /api/v1/dao/proposals/:id`
  - `POST /api/v1/dao/votes` (voto + address + proposal id + opción)
  - `GET /api/v1/dao/votes?proposal=:id`
- **Reglas de peso:** Usar `eth_getBalance` o `/api/v1/accounts/:address` (o una futura `balanceOf` por token) en el bloque de snapshot elegido (ej. al crear la propuesta o al votar).
- **Frontend:** Crear propuesta, votar, listar votos (similar al widget), con Metamask apuntando al RPC del nodo (Custom RPC ISB).

Aquí el “nodo” gana: **nuevos endpoints** y una **UI de gobernanza** coherente con IGT-DAO/IGT-VOTE.

### C) Híbrido

- Misma **UI** (propuesta, voto, lista) que en el widget.
- **Backend propio** para ISB (como en B).
- Opcional: si más adelante IGT también está en una red soportada por Snapshot, se puede tener un “Space” en Snapshot para esa red y, si se desea, mostrar ambos (Snapshot + DAO nativo ISB) en la misma plataforma.

---

## 5. Recomendaciones concretas para el nodo

### 5.1 Corto plazo (sin comprar necesariamente el widget)

1. **Añadir en `platform-services.json`** un servicio **Ierahkwa Futurehead DAO**:
   - `id`: `dao`
   - `name`: Ierahkwa Futurehead DAO
   - `domain`: dao.ierahkwa.gov
   - `token`: IGT-DAO
   - `category`: `governance` (y añadir la categoría si no existe)
   - `status`: `PLANNED` o `IN_DEVELOPMENT`
   - `features`: Create proposal, Vote, List of votes, Token-weighted, Gas-free, Metamask.

2. **En el nodo (`server.js`):** Diseñar (o implementar en una segunda fase) la API DAO:
   - Estructura de datos: `proposals` (id, title, description, creator, createdAt, votingEnd, options, status), `votes` (proposalId, voter, option, weight, timestamp).
   - El “weight” se puede obtener desde `state.accounts` o desde un `balanceOf` simulado por token.

3. **Documentar** en `DOCUMENTACION-TECNICA.md` o en un `PLANO-DAO-01.md`:
   - Flujo: Create → Voting Period → Vote (off-chain) → Count (por balance en nodo).
   - Cómo conectar Metamask al nodo (chainId 77777, RPC URL).

### 5.2 Si compran el widget de CodeCanyon

- Revisar la doc de OnOut para:
  - Custom network / custom strategy.
  - Si solo usa Snapshot: planear uso en una red EVM donde IGT exista, y/o reutilizar solo el HTML/JS como referencia para la UI del DAO nativo ISB.
- En cualquier caso, **registrar el servicio DAO** en `platform-services.json` y en el `index` de la plataforma para que esté visible y alineado con IGT-DAO.

### 5.3 Coherencia con tokens y config

- **IGT-DAO** e **IGT-VOTE:** Pueden convivir: IGT-DAO para “espacio de gobernanza” (propuestas, config), IGT-VOTE para “peso del voto” (o al revés, según diseño). O un solo token (IGT-DAO) para peso. Definir en el plano DAO.
- **`ierahkwa-shop`:** Cuando el DAO esté en desarrollo o activo, actualizar `services.js` y el portal: `url`, `status: 'active'` o `'in_development'`.

---

## 6. Conclusión

| Pregunta | Respuesta |
|----------|-----------|
| **¿El DAO widget suma al nodo?** | **Sí:** como referencia de producto (Create/Vote/List, gas-free, token-weighted) y como posible UI en redes EVM. Como software “plug-and-play” con Snapshot, **no** se conecta a ISB tal cual. |
| **¿Vale la pena comprarlo?** | Depende: si van a emitir IGT en Ethereum/BSC/Polygon y quieren Snapshot, puede ahorrar trabajo. Para **ISB y el nodo Mamey**, lo más útil es **inspirarse** en el diseño y flujo, y construir un backend DAO propio en el nodo + una UI similar. |
| **Qué hacer ya en el nodo** | (1) Añadir servicio DAO en `platform-services.json`; (2) definir `PLANO-DAO-01` o equivalente con endpoints y reglas de peso; (3) opcional: esbozar endpoints `/api/v1/dao/*` en `server.js` para una Fase 2. |

---

**Sovereign Government of Ierahkwa Ne Kanienke**  
**Office of the Prime Minister**
