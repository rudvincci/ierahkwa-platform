# Ruta única del proyecto — Sovereign Government of Ierahkwa Ne Kanienke

**Todo el código y todo lo del proyecto está en una sola ruta: la raíz del repo** (la carpeta que se llama como el proyecto, p. ej. `soberanos natives`).

---

## No se unificaron ni movieron carpetas de código

- No se ha borrado ni movido código. No se ha “unificado” nada en una sola carpeta nueva.
- El proyecto ya vive en **una sola ruta**: la raíz. Dentro de ella hay muchas carpetas (RuddieSolution, Mamey, docs, scripts, AdvocateOffice, etc.). Eso es la estructura normal.
- La carpeta `00-PROYECTO-SOBERANOS` es solo **referencia** (LEEME, enlaces). El código **no** está dentro de ella; está en la raíz y en sus subcarpetas.

---

## Dónde está el código (desde la raíz)

| Qué | Ruta (todo dentro de la raíz del proyecto) |
|-----|-------------------------------------------|
| Servidor Node | `RuddieSolution/node/server.js` |
| Rutas plataformas | `RuddieSolution/node/platform-routes.js` |
| HTML de plataformas | `RuddieSolution/platform/*.html` |
| Data (banco, VIP, config) | `RuddieSolution/node/data/`, `RuddieSolution/platform/data/` |
| Mamey | `Mamey/` |
| Documentación | `docs/` |
| Scripts | `scripts/` |
| Servicios .NET | `AdvocateOffice/`, `AIFraudDetection/`, … (cada uno en su carpeta) |

---

## Si “no hay código” o no ves nada

1. Abre la **carpeta raíz del proyecto** (la que contiene `00-PROYECTO-SOBERANOS`, `RuddieSolution`, `Mamey`, `docs`, etc.), no solo `00-PROYECTO-SOBERANOS`.
2. En Cursor: File → Open Folder → elige esa carpeta raíz.
3. El código está ahí; no se ha movido a otro sitio.

*Última actualización: febrero 2026.*
