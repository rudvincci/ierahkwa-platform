# Estado de Servicios — Todas las Plataformas Operativas

**Sistema:** Sovereign Government of Ierahkwa Ne Kanienke — Plataforma Digital  
**Servidor:** Mamey Node — `http://localhost:8545`  
**Última verificación:** 2026-01-23  

---

## 1. Resumen Ejecutivo

Las **86 plataformas HTML** del ecosistema están correctamente enlazadas, registradas en el servidor y servidas sin errores. Las rutas cortas, el `config.json` y los enlaces del hub (`index.html`) apuntan a destinos válidos; tanto el acceso por ruta corta (ej. `/bdet-bank`, `/forex`, `/admin`) como por ruta completa (ej. `/platform/blockchain-platform.html`) están operativos.

---

## 2. Verificación Técnica

### 2.1 Plataformas HTML (platform/)

| Concepto | Valor |
|----------|--------|
| Archivos en `platform/*.html` | **86** |
| Acceso vía `express.static` | `/platform/<archivo>.html` |
| Archivos comprobados | Todos existen y coinciden con las rutas del servidor |

Cada `platform/*.html` es accesible como `http://localhost:8545/platform/<nombre>.html`.

### 2.2 Rutas Cortas (app.get + sendFile)

El servidor define rutas cortas que entregan el HTML correspondiente. Todas las rutas revisadas apuntan a archivos existentes en `platform/`. Ejemplos (lista no exhaustiva):

| Ruta | Archivo servido |
|------|------------------|
| `/`, `/platform` | index / platform (redirect o static) |
| `/admin` | `admin.html` |
| `/login` | `login.html` |
| `/bdet-bank`, `/bdet` | `bdet-bank.html` |
| `/forex` | `forex.html` |
| `/wallet` | `wallet.html` |
| `/gaming` | `gaming-platform.html` |
| `/casino`, `/lotto`, `/raffle`, `/sports-betting` | `casino.html`, `lotto.html`, `raffle.html`, `sports-betting.html` |
| `/vip-transactions`, `/vip`, `/transactions` | `vip-transactions.html` |
| `/central-banks`, `/4-banks` | `central-banks.html` |
| `/siis`, `/settlement` | `siis-settlement.html` |
| `/debt-collection`, `/deudas` | `debt-collection.html` |
| `/sovereignty`, `/soberania` | `sovereignty-education.html` |
| `/futurehead`, `/futurehead-group` | `futurehead-group.html` |
| `/mamey-futures`, `/mamey`, `/trading`, `/futures`, `/commodities`, `/options` | `mamey-futures.html` |
| `/bitcoin-hemp`, `/crypto` | `bitcoin-hemp.html` |
| `/atm`, `/atm-manufacturing` | `atm-manufacturing.html` |
| `/bank-worker`, `/global-banking`, `/banking` | `bank-worker.html` |
| `/security`, `/leader-control`, `/monitor` | `security-fortress.html`, `leader-control.html`, `monitor.html` |
| `/documents`, `/esignature`, `/citizen-crm`, `/health-dashboard`, `/support-ai` | Respectivos `.html` |
| `/spike-office`, `/rnbcal`, `/appbuilder`, `/smartschool`, `/social-media` | Respectivos `.html` |
| `/analytics`, `/voting`, `/governance`, `/rewards`, `/gamification` | `analytics-dashboard.html`, `voting.html`, `rewards.html` |
| `/membership`, `/citizen-membership`, `/members`, `/invest` | `citizen-membership.html` |
| `/departments`, `/103-departments`, `/depts` | `departments.html` |
| `/launchpad`, `/citizen-launchpad`, `/tokenize`, `/register-project` | `citizen-launchpad.html` |
| `/backup`, `/backup-department` | `backup-department.html` |
| `/token-factory`, `/create-token`, `/bridge` | `token-factory.html`, `bridge.html` |

Lista completa de rutas en **PLATAFORMAS-8545.md**.

### 2.3 Configuración (config.json)

- **headerNav (15 ítems):** Enlaces a `/platform/*.html` o rutas como `/bdet-bank`, `/leader-control`. Destinos comprobados.
- **quickActions (11 ítems):** `platformKey` resueltos por `openPlatform()` hacia `platforms` o `config.services`. Claves y URLs verificadas.
- **services (51 claves):**  
  - Rutas internas (`/platform/...`, `/bdet-bank`, `/forex`, `/vip-transactions`, `/chat`, etc.): servidas por el Node o por `express.static` en `/platform`, `/ierahkwa-shop`, `/DocumentFlow`, `/ESignature`, `/image-upload`, `/pos-system`, etc.  
  - URLs a microservicios (`http://localhost:5055`, `5060`, `5056`, `3010`, `5097`, `7070`, `7071`): requieren que el servicio correspondiente esté en ejecución; el enlace en sí está bien definido.

### 2.4 Hub Principal (index.html)

Los version-badges, botones “Open X Dashboard”, `headerNav`, `quickActions` y platform-cards utilizan:

- `href` a rutas del servidor o a `/platform/*.html`
- `openPlatform(key)` con claves del objeto `platforms` y de `config.services`

Los destinos referenciados existen o están cubiertos por rutas del servidor; no se han detectado enlaces rotos a plataformas propias del ecosistema.

---

## 3. Servicios Externos (Opcionales)

Algunas entradas de `config.services` apuntan a puertos locales de microservicios:

| key | URL | Requiere |
|-----|-----|----------|
| rnbcal | http://localhost:5055 | RnBCal en ejecución |
| appbuilder | http://localhost:5060 | AppBuilder en ejecución |
| spikeoffice | http://localhost:5056 | SpikeOffice en ejecución |
| advocate | http://localhost:3010 | Advocate en ejecución |
| idofactory | http://localhost:5097 | IDO Factory en ejecución |
| projecthub | http://localhost:7070 | ProjectHub en ejecución |
| meetinghub | http://localhost:7071 | MeetingHub en ejecución |

Si el microservicio no está levantado, el enlace devolverá error de conexión; la configuración de la plataforma es correcta. `school-node` (`http://localhost:8545`) utiliza el propio Mamey Node.

---

## 4. Rutas con barra final (/ruta/)

Si se accede a una ruta con barra final (ej. `/smartschool/`, `/admin/`, `/gaming/`) el servidor **redirige automáticamente** a la versión sin barra (`/smartschool`, `/admin`, `/gaming`) con 301. Así se evita el error "Cannot GET /ruta/". Excepciones: `/docs/` y `/tradex/` se mantienen con barra.

---

## 5. Reinicio del servidor (cuando /admin o /gaming dan 404)

Las rutas `/admin` y `/gaming` están definidas en **RuddieSolution/node/server.js**. Si al acceder a `http://localhost:8545/admin` o `http://localhost:8545/gaming` se obtiene **404**, el proceso que escucha en el puerto 8545 no es el de RuddieSolution (p. ej. se arrancó desde otro `server.js` o una instancia antigua).

**Solución:** en la raíz del repo ejecutar:

```bash
./stop.sh
./start.sh
```

`./start.sh` usa **RuddieSolution/scripts/start.sh**, que arranca `RuddieSolution/node/server.js`. Tras el reinicio, `/admin`, `/gaming` y el resto de rutas cortas responderán correctamente.

---

## 6. Conclusión

- **Plataformas HTML:** Las 86 páginas de `platform/` se sirven correctamente vía `/platform/<archivo>.html`.  
- **Rutas cortas:** Las rutas cortas definidas en el servidor entregan el HTML correspondiente; no se han encontrado `sendFile` a archivos inexistentes.  
- **Config y hub:** `headerNav`, `quickActions`, `services` y el `index.html` enlazan a destinos válidos dentro del ecosistema. Los únicos enlaces que pueden fallar por entorno son los que dependen de microservicios externos en `localhost`.

Para el detalle de URLs y agrupación por categoría, véase **PLATAFORMAS-8545.md**. Para el inventario de links y botones editables desde Admin, **REPORTE-LINKS-Y-BOTONES.md**. Para la visión de conjunto del sistema, **REPORTE-COMPLETO.md**.

---

*Documento de estado. Actualizar la fecha de verificación al revalidar rutas o al añadir nuevas plataformas.*
