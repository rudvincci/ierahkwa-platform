# Proyecto completo en memoria externa — Sovereign Government of Ierahkwa Ne Kanienke

**Si algo no está en el disco, el proyecto se daña.** Este doc define qué debe ir completo a la memoria externa y qué puede quedarse solo en la comp (o regenerarse en el disco).

---

## Regla

- **Memoria externa (disco Extreme Pro)** = copia **completa** del sistema soberano. Nada indispensable puede faltar.
- **Lista de todo lo que es el proyecto** = [INDICE-COMPLETO-PROYECTO-SOBERANOS.md](INDICE-COMPLETO-PROYECTO-SOBERANOS.md). Eso es lo que debe estar en el disco.
- Después de copiar, verificar con `./scripts/verificar-extreme-pro.sh` (reporte en `docs/verificacion-extreme-pro.txt`).

---

## Qué DEBE estar en el disco (completo)

Todo lo que está en el índice, carpeta por carpeta:

- **Núcleo:** RuddieSolution (node, platform, scripts, servers, services, config, **data**, IerahkwaBanking.NET10, etc.), Mamey, MAMEY-FUTURES, tokens, docs, scripts (todos, incl. los de Extreme Pro).
- **APIs .NET:** AIFraudDetection, AdvocateOffice, AppBuilder, … hasta WorkflowEngine (cada carpeta completa).
- **Infra y despliegue:** DEPLOY-SERVERS, IERAHKWA-INDEPENDENT, IERAHKWA-PLATFORM-DEPLOY, IerahkwaBankPlatform, kubernetes, systemd, sovereign-network, quantum.
- **Comercio y otros:** ierahkwa-shop, forex-trading-server, image-upload, mobile-app, platform-dotnet, Akwesasne, backups, auto-backup, backup, .cursor (rules), y el resto de carpetas listadas en el índice.
- **Git:** `.git` completo (incl. `.git/objects`) para que el repo en el disco sea válido y no se pierda historial.
- **Data y config:** Todo `RuddieSolution/node/data`, `RuddieSolution/platform/data`, archivos `.json` de estado, bancos, VIP, bonos, blockchain-state, etc. Nada de data se excluye.

**Script que deja el proyecto completo en el disco:** `./scripts/instalar-en-extreme-pro-para-trabajar.sh` (copia todo; ver sección siguiente para las únicas exclusiones).

---

## Lo que NO se copia (y por qué no daña el proyecto)

Solo esto se excluye del rsync; **todo lo demás va**:

| Excluido | Motivo | Dónde se recupera en el disco |
|----------|--------|-------------------------------|
| **node_modules** | Ocupan mucho y se generan con npm. | `./scripts/setup-en-extreme-pro.sh` hace `npm install` en el disco. |
| **logs** | Archivos de registro; no son código ni data del sistema. | Se regeneran al correr `./start.sh` o los servicios en el disco. |
| **.go-live.lock** | Archivo de bloqueo temporal. | Se crea de nuevo si hace falta. |
| **\*.log** | Logs sueltos. | Regenerables. |

**Importante:** `.git` (incl. `.git/objects`) **sí se copia** para que el proyecto en el disco sea completo y el historial no se pierda.

---

## Qué puede quedarse solo en la comp (Mac)

Cuando **trabajas desde el disco**, la Mac no necesita una copia del proyecto. Si tienes el proyecto también en la Mac:

- **En la Mac pueden quedarse (y no van al disco):**  
  Cachés (npm, pip, Xcode), Papelera, archivos temporales, `node_modules` de la Mac (si abres el proyecto en la Mac), `logs` de la Mac. Nada de eso es parte “única” del proyecto; el proyecto completo está en el disco.

- **No debe quedarse solo en la Mac** (si algo es único, debe estar también en el disco):  
  Código, data (`node/data`, `platform/data`), config (`.env` si tiene valores importantes), docs, scripts, `.git`, Mamey, RuddieSolution, tokens, todo lo listado en el índice. Si editas en la Mac, luego sincroniza al disco con `./scripts/backup-a-extreme-pro.sh` o rsync Mac → disco.

**Resumen:** Lo que “movemos y se queda en la comp” = solo cachés y temporales (no parte del repo). El **proyecto completo** vive en la memoria externa; la Mac es donde ejecutas los scripts (con el disco conectado) o donde tienes una copia de trabajo que luego subes al disco.

---

## Pasos para tener el proyecto completo en el disco

1. Conecta el disco Extreme Pro a la Mac.
2. Desde la carpeta del proyecto (en la Mac):

   ```bash
   ./scripts/instalar-en-extreme-pro-para-trabajar.sh
   ```

3. (Opcional) Verificar que no falte nada:

   ```bash
   ./scripts/verificar-extreme-pro.sh
   ```

   Revisa `docs/verificacion-extreme-pro.txt`.

4. Trabajar siempre desde el disco: abrir en Cursor `/Volumes/Extreme Pro/soberanos-natives` y desde ahí `./start.sh`.

---

## Referencias

- Lista completa de carpetas y contenido: [INDICE-COMPLETO-PROYECTO-SOBERANOS.md](INDICE-COMPLETO-PROYECTO-SOBERANOS.md).
- Instalar y trabajar desde disco: [MUDAR-PROYECTO-A-EXTREME-PRO.md](MUDAR-PROYECTO-A-EXTREME-PRO.md).
- Liberar espacio (Mac y disco): [LIBERAR-ESPACIO-DISCO-MAC.md](LIBERAR-ESPACIO-DISCO-MAC.md).

*Sovereign Government of Ierahkwa Ne Kanienke. Última actualización: febrero 2026.*
