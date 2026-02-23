# Si Cursor dice "The window terminated unexpectedly"

## Qué hacer de inmediato

1. **Reopen** — Pulsa "Reopen" para recuperar la ventana y los editores.
2. Si quieres empezar sin restaurar pestañas: marca **"Don't restore editors"** y luego "Reopen".

## Si sigue crasheando

- Cierra pestañas que no uses (menos memoria).
- Cierra otras ventanas de Cursor.
- Sal de Cursor por completo y vuelve a abrirlo.
- Actualiza Cursor a la última versión.

## Qué hicimos en el proyecto para reducir crashes

- Se creó **`.cursorignore`** para que Cursor no indexe `node_modules`, `bin/`, `obj/`, logs, bases de datos, etc. Así el editor trabaja con menos carga y suele ser más estable.

Si el crash vuelve siempre al abrir el mismo archivo o carpeta, puede ser por ese archivo; evita abrirlo o desactiva extensiones recientes para probar.
