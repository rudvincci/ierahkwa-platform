# Liberar espacio — Mac y disco externo

**Sovereign Government of Ierahkwa Ne Kanienke**  
Uso del **disco externo (Extreme Pro)** y de la **Mac** para trabajar, y cómo liberar espacio en ambos sin borrar nada del proyecto.

---

## Usar disco externo y Mac

| Dónde está el proyecto | Cómo trabajar | Liberar espacio |
|------------------------|----------------|-----------------|
| **Solo en la Mac** | Abres la carpeta del proyecto en la Mac (Cursor, terminal). | Mac: `./scripts/liberar-espacio.sh --limpiar`. Disco: no aplica. |
| **Solo en el disco externo** | Conectas el disco → abres `/Volumes/Extreme Pro/soberanos-natives` en Cursor → trabajas ahí. La Mac no guarda otra copia del proyecto; **no duplicar** en la Mac. | Mac: liberar cachés (npm, Papelera, Xcode, etc.) con el script. Disco: no borrar carpetas del proyecto; si hace falta espacio en el disco, vaciar Papelera del disco o borrar solo cosas que no sean el repo. |
| **Mac + disco (misma versión)** | **Una** es la carpeta de trabajo (Mac o disco). La otra se mantiene con **rsync/backup** desde esa una. Ver [MUDAR-PROYECTO-A-EXTREME-PRO.md](MUDAR-PROYECTO-A-EXTREME-PRO.md): instalar en disco desde Mac, o `backup-a-extreme-pro.sh` para llevar cambios al disco. No tener “dos proyectos” editando a la vez para no desincronizar. | Mac: `./scripts/liberar-espacio.sh --limpiar`. Disco: no tocar contenido del proyecto. |

**Resumen:** Trabajas **o** desde la Mac **o** desde el disco (una sola carpeta “viva”). La otra puede ser copia de respaldo o destino de backup. Así no duplicas y no borras nada del proyecto. Para instalar/copiar al disco: `./scripts/instalar-en-extreme-pro-para-trabajar.sh` o `./scripts/backup-a-extreme-pro.sh`.

---

## Regla

- **No se borra nada** del código ni de la data del proyecto.
- **Solo duplicados:** si quieres borrar archivos duplicados, primero se listan (script); tú revisas y borras a mano si lo ves bien.
- No duplicar el proyecto: una carpeta de trabajo (Mac o disco); el otro es backup/copia si lo usas.

---

## 1. Script rápido (recomendado)

Desde la raíz del proyecto:

```bash
# Ver qué ocupa (disco, proyecto, cachés)
./scripts/liberar-espacio.sh

# Limpiar cachés seguros (npm, pip, logs viejos del proyecto, etc.)
./scripts/liberar-espacio.sh --limpiar
```

Eso **no toca** código ni data; solo cachés de sistema y logs del proyecto de más de 3 días (opcional con `--limpiar`).

---

## 2. Listar duplicados (solo informe, no borra)

Para ver **posibles archivos duplicados** en el proyecto y decidir tú si borrar alguno:

```bash
./scripts/listar-duplicados-proyecto.sh
```

Escribe un informe en `docs/listado-duplicados-proyecto.txt`. **No borra nada**; solo lista grupos de archivos con el mismo contenido (hash). Revisas y, si quieres, borras a mano.

---

## 3. Limpiezas seguras en la Mac (fuera del proyecto)

| Qué | Comando / acción | Espacio aproximado |
|-----|------------------|--------------------|
| **npm cache** | `npm cache clean --force` | Variable |
| **Papelera** | Finder → Papelera → Vaciar | Variable |
| **Homebrew** | `brew cleanup -s` | Cachés de fórmulas |
| **Xcode DerivedData** | `rm -rf ~/Library/Developer/Xcode/DerivedData/*` | 1–10 GB |
| **Simuladores iOS** | Xcode → Window → Devices and Simulators → borrar no usados | ~5 GB |
| **pip** | `rm -rf ~/Library/Caches/pip/*` | Variable |
| **Logs del proyecto** | `./scripts/liberar-espacio.sh --logs` (borra todo `logs/`) | ~200 MB si hay muchos |

---

## 4. Qué no tocar

- Carpetas del proyecto (RuddieSolution, Mamey, docs, scripts, tokens, etc.): **no borrar**.
- `node_modules`: se pueden regenerar con `npm install`; si necesitas espacio urgente puedes borrarlos y volver a instalar (en el proyecto o en el disco). El script **no** los borra por defecto.
- `.git`: **no borrar** (historial del repo).

---

## 5. Liberar espacio en el disco externo

- **No borrar** carpetas del proyecto en el disco (soberanos-natives, soberanos-natives-copia2, soberanos-backup si existen).
- **Sí se puede:** vaciar la Papelera del disco (en Finder, con el disco seleccionado, Papelera → Vaciar). Borrar solo archivos o carpetas que **no** sean el proyecto (p. ej. descargas viejas que hayas copiado al disco).
- Si el disco está muy lleno y solo tienes el proyecto: la opción es mover otros datos fuera del disco o usar un disco más grande; no eliminar partes del repo.

---

## 6. Resumen

| Objetivo | Acción |
|----------|--------|
| Ver estado del disco y tamaños (Mac) | `./scripts/liberar-espacio.sh` |
| Liberar en Mac sin tocar proyecto | `./scripts/liberar-espacio.sh --limpiar` |
| Ver duplicados (solo listado) | `./scripts/listar-duplicados-proyecto.sh` |
| Vaciar Papelera / Homebrew / Xcode | Manual o comandos de la tabla anterior |
| Instalar proyecto en disco (Mac → disco) | `./scripts/instalar-en-extreme-pro-para-trabajar.sh` |
| Backup Mac → disco | `./scripts/backup-a-extreme-pro.sh` |
| Trabajar desde disco | Abrir `/Volumes/Extreme Pro/soberanos-natives` en Cursor |

*Última actualización: febrero 2026.*
