# Trabajar desde Extreme Pro — Disco externo

**Sovereign Government of Ierahkwa Ne Kanienke**  
**Extreme Pro** = disco externo. El proyecto vive ahí y trabajas siempre desde el disco.

---

## Qué va al disco (lista única: no duplicar)

**La lista completa de carpetas y contenido del proyecto** está en **[INDICE-COMPLETO-PROYECTO-SOBERANOS.md](INDICE-COMPLETO-PROYECTO-SOBERANOS.md)**. Ese es el inventario de todo lo que debe quedar en el disco.

- **El script `instalar-en-extreme-pro-para-trabajar.sh`** copia **todo** el árbol del repo con `rsync`, **excepto:** `node_modules`, `logs`, `.go-live.lock`, `*.log`. **`.git` se copia completo** (incl. historial) para que el proyecto en el disco no se dañe. Las dependencias (`node_modules`) se regeneran en el disco con `setup-en-extreme-pro.sh`.
- **Incluye:** RuddieSolution (node, platform, scripts, servers, services, config, data, IerahkwaBanking.NET10, etc.), todas las carpetas .NET (AIFraudDetection, AdvocateOffice, AppBuilder, …), Mamey, MAMEY-FUTURES, tokens, docs, scripts (incl. los de Extreme Pro), Akwesasne, ierahkwa-shop, forex-trading-server, backups, .cursor, y el resto del índice. Nada se omite salvo las exclusiones de rsync.
- **Scripts de Extreme Pro** (van en la copia, en `scripts/`): `instalar-en-extreme-pro-para-trabajar.sh`, `setup-en-extreme-pro.sh`, `backup-a-extreme-pro.sh`, `mudar-a-extreme-pro.sh`, `verificar-extreme-pro.sh`.
- **Después de instalar**, opcional: desde la Mac (con el disco conectado) ejecutar `./scripts/verificar-extreme-pro.sh` para comprobar que no falta nada; el reporte se escribe en `docs/verificacion-extreme-pro.txt`.

---

## Primera vez: instalar en el disco

### Una sola copia (para trabajar desde el disco)

1. **Conecta** el disco Extreme Pro a la Mac.
2. Desde la carpeta actual del proyecto (donde está ahora):

```bash
./scripts/instalar-en-extreme-pro-para-trabajar.sh
```

Ese script copia todo al disco (en `soberanos-natives`), hace `npm install` y crea `.env` si falta. Al terminar, el proyecto está listo para trabajar en el disco.

### Dos copias en el disco (más rápido que una a una)

Si necesitas **dos** carpetas en el mismo disco (principal + segunda copia / respaldo):

```bash
./scripts/instalar-dos-copias-en-disco.sh
```

Crea en el disco:
- **soberanos-natives** — copia principal (con setup: npm install, .env). Trabaja desde aquí.
- **soberanos-natives-copia2** — segunda copia (solo archivos). Si algún día abres esta carpeta, ejecuta ahí: `./scripts/setup-en-extreme-pro.sh`.

Disco con otro nombre:

```bash
DEST_BASE="/Volumes/NOMBRE_DE_TU_DISCO" ./scripts/instalar-dos-copias-en-disco.sh
```

Si el disco tiene otro nombre (solo una copia):

```bash
DEST_BASE="/Volumes/NOMBRE_DE_TU_DISCO" ./scripts/instalar-en-extreme-pro-para-trabajar.sh
```

---

## Trabajar siempre desde el disco

1. **Conecta** el disco Extreme Pro.
2. **Abre en Cursor** la carpeta: `/Volumes/Extreme Pro/soberanos-natives`  
   (File → Open Folder → elegir esa ruta)
3. En la terminal (desde esa carpeta), si es la primera vez en esta máquina o faltan dependencias:
   ```bash
   ./scripts/setup-en-extreme-pro.sh
   ```
   Luego arranca:
   ```bash
   ./start.sh
   ```
4. Plataforma: http://localhost:8545/platform

Todo lo que edites, guardes y ejecutes será en el disco. Si desconectas el disco, no podrás trabajar hasta que lo conectes de nuevo.

---

## Backup extra (opcional)

Si quieres una copia de respaldo aparte en el mismo disco:

```bash
./scripts/backup-a-extreme-pro.sh
```

Eso crea `soberanos-backup` en el disco. Tu trabajo diario sigue siendo en `soberanos-natives`.

---

## Resumen

| Paso | Acción |
|------|--------|
| **1 (solo una vez)** | Conecta disco → `./scripts/instalar-en-extreme-pro-para-trabajar.sh` |
| **2 (siempre)** | Conecta disco → Abre `/Volumes/Extreme Pro/soberanos-natives` en Cursor → `./scripts/setup-en-extreme-pro.sh` (si hace falta) → `./start.sh` |
| **Verificar** | Con disco conectado: `./scripts/verificar-extreme-pro.sh` (comprueba archivos clave; reporte en `docs/verificacion-extreme-pro.txt`) |
