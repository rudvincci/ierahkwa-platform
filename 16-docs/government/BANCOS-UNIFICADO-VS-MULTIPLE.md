# Bancos: ¿Varios o uno? Main admin y códigos en el Node

## Por qué hay “varios bancos” hoy

En el código aparecen:

1. **4 bancos centrales** (Águila, Quetzal, Cóndor, Caribe) con puertos propios en `config/services-ports.json` (4100–4400).
2. **BDET** como hub principal (nodo 8545 + Banking Bridge 3001).
3. **Bancos regionales e internacionales** como entradas en un registro (SWIFT, nombre, tipo) en el frontend (`bdet-bank.html` → `BANK_REGISTRY`).

Es decir: hay **varios “bancos” como concepto** (centrales, regionales, internacionales) pero **una sola aplicación** que los muestra y opera (BDET Bank UI + Node + Banking Bridge).

---

## ¿Mejor unificar o dejar varios?

**Recomendación: unificar bajo un solo Node + main admin.**

| Enfoque | Ventajas | Desventajas |
|--------|----------|-------------|
| **Varios procesos** (4 servidores Águila/Quetzal/Cóndor/Caribe) | Aislamiento por región/entidad. | Más despliegues, más puertos, más mantenimiento. Solo compensa si cada uno es un datacenter/entidad legal distinta. |
| **Un solo Node + registro de bancos** | Un despliegue, una configuración, un lugar para administrar todos los códigos. Los “bancos” son **registros** (código SWIFT, nombre, tipo), no procesos separados. | Si en el futuro quieres 4 nodos físicos (4 países), habría que sacar algunos del registro a servicios propios. |

Para 2026, tener **un main admin y cada código de banco dentro del Node** es lo más práctico: un solo sitio donde editar/alta/baja de bancos y un solo punto de operación.

---

## Cómo queda la arquitectura unificada

1. **Un Node (8545)**  
   Sirve la plataforma (HTML/JS), las APIs propias del banco y el **registro de bancos**.

2. **Registro de bancos en el Node (una sola fuente de verdad)**  
   - Archivo: `RuddieSolution/node/data/bank-registry.json`  
   - API: `GET /api/v1/bdet/bank-registry`  
   - Contenido: objeto con clave = código SWIFT (ej. `IERBDETXXX`, `IERAGUILAX`) y valor = datos del banco (nombre, tipo, región, api, status, etc.).

3. **Main admin (Ierahkwa Futurehead BDET Bank back)**  
   - **Hoy:** editar `node/data/bank-registry.json` a mano o con un script.  
   - **Después:** pantalla de admin en BDET Bank que lea/edite ese registro vía API (GET + PUT/POST si se implementan). Todo el admin del ecosistema se hace desde BDET Bank.

4. **BDET Bank (frontend)**  
   - Al cargar, llama a `GET /api/v1/bdet/bank-registry`.  
   - Si responde OK, usa ese registro como `BANK_REGISTRY`.  
   - Si falla (404, red), usa el `getDefaultBankRegistry()` local como respaldo.

5. **Banking Bridge (3001)**  
   Sigue siendo la API que ejecuta transferencias, cuentas, etc. El **registro** (quién es cada banco) vive en el Node; la **lógica** de operaciones, en el Bridge.

---

## Los 4 “bancos centrales” (Águila, Quetzal, Cóndor, Caribe)

- **Opción A (recomendada para empezar):** no levantar 4 procesos (4100–4400). Son **4 códigos** en `bank-registry.json` con `type: 'central'`. Toda la lógica sigue en el mismo Node + Banking Bridge.  
- **Opción B:** si más adelante una región tiene que correr en su propio servidor, ese banco puede pasar a tener su propia API (su propio “node” o bridge) y en el registro se pone `api: 'https://...'`; el resto sigue siendo “solo registro” en el Node principal.

---

## Redundancia: para que no se caiga uno y todo al piso

**Un solo nodo** = más simple, pero si ese servidor cae o se corta la red, **todo el sistema se cae**.  
**Varios nodos físicos (o réplicas)** = si uno falla, los demás siguen; el servicio no se va “todo al piso”.

| Escenario | Qué hacer | Resultado |
|-----------|-----------|-----------|
| **Desarrollo / MVP** | 1 Node + 1 Bridge. | Rápido de operar; si cae, se reinicia. |
| **Producción / no querer que todo caiga** | **2 o más nodos** (o 2+ instancias del mismo Node detrás de un balanceador). Misma app, mismo `bank-registry.json` (replicado o en almacenamiento compartido). | Si un nodo se cae, el otro(s) atienden. Alta disponibilidad. |
| **1 International Settlement + 4 Bancos Centrales** | Un nodo = SIIS (liquidación internacional, puerto 8500); debajo, 4 nodos = bancos centrales independientes (Águila, Quetzal, Cóndor, Caribe en 8545–8548). Ver **`docs/CUATRO-NODOS-REGIONES.md`** y `node/ecosystem.4regions.config.js`. | Máxima resiliencia: el settlement coordina; si cae un banco central, los otros tres siguen. |

**Recomendación práctica:**  
- Para **no depender de un solo punto de fallo**, lo mejor es **al menos 2 nodos** (activo/activo o activo/pasivo) o 4 si quieres un nodo por banco central. Así, que se caiga uno no deja “todo al piso”.  
- El **registro de bancos** (main admin) puede seguir siendo un solo `bank-registry.json` replicado a todos los nodos, o un servicio compartido (DB/API) que todos consultan.

---

## Resumen

- **Unificar** bajo un solo Node + main admin es mejor para operar y mantener.
- **Cada banco** = una entrada en el registro (código SWIFT + datos), guardada en el Node en `node/data/bank-registry.json`.
- **Main admin** = ese archivo (por ahora) y/o una futura pantalla que use `GET /api/v1/bdet/bank-registry` (y, si se añade, PUT/POST para editar).
- Los “varios bancos” son **códigos y metadatos** en un solo sitio; la operación real sigue en un solo ecosistema (Node 8545 + Bridge 3001).
- **Para que no se caiga uno y todo al piso:** en producción conviene **2 o más nodos** (o 4 si cada banco central tiene su propio servidor); así la caída de un nodo no tira todo el sistema.

**Referencias:**  
- Registro en el Node: `node/data/bank-registry.json`  
- API del registro: `GET /api/v1/bdet/bank-registry`  
- UI: `platform/bdet-bank.html` (puede consumir esa API y usar fallback local)
