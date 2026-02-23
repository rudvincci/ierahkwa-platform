# Unificar colores en todas las plataformas

**Sovereign Government of Ierahkwa Ne Kanienke**  
Para que no haya un desastre de colores: una sola paleta en todo el proyecto.

---

## Regla

Todas las páginas bajo **`RuddieSolution/platform/`** deben cargar el sistema de diseño unificado:

```html
<link rel="stylesheet" href="assets/unified-styles.css">
```

(O la ruta correcta si la página está en un subdirectorio, ej. `../assets/unified-styles.css`.)

---

## Variables de color (usar siempre estas)

Definidas en **`platform/assets/unified-styles.css`** en `:root`:

| Variable | Uso |
|----------|-----|
| `--gold` | Títulos destacados, admin, premium |
| `--neon-green` | OK, éxito, enlaces, seguridad |
| `--neon-cyan` | Enlaces secundarios, tech, info |
| `--neon-purple` | AI, ATABEY, features especiales |
| `--neon-red` | Alertas, emergencias, error |
| `--text-primary` | Texto principal |
| `--text-secondary` | Texto secundario |
| `--text-muted` | Texto apagado |
| `--bg-dark`, `--bg-card` | Fondos |
| `--border-color` | Bordes |

## Clases de botones (evitar colores inline)

- **`.btn`** + **`.btn-gold`** / **`.btn-green`** / **`.btn-cyan`** / **`.btn-purple`** / **`.btn-red`**
- **`.btn-outline`** (borde verde)
- **`.btn-ghost`** (discreto)
- **`.admin-dash-link`** (enlaces Dashboard Admin — cyan)
- **`.admin-dash-dept`** (departamentos en Admin — verde)

## Qué no hacer

- No usar `style="color: #cualquiercosa"` ni colores hex sueltos en cada página.
- No inventar nuevos “neones” por página; usar las variables o las clases anteriores.

## Si una página se ve distinta

1. Comprobar que incluya **`unified-styles.css`** (y que cargue después de fonts).
2. Sustituir estilos inline por clases (`.btn`, `.card`, `.title-gold`, etc.).
3. Para bloques concretos, usar variables: `color: var(--neon-cyan);` en vez de `#00FFFF`.

---

Así **todas las plataformas comparten la misma paleta** y no hay mezcla de colores.
