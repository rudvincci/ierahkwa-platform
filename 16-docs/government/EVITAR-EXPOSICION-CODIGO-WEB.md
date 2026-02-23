# Evitar que se vea código interno en la web

**Sovereign Government of Ierahkwa Ne Kanienke**  
En producción no debe exponerse al navegador código, rutas internas ni mensajes de error que ayuden a un atacante.

---

## Qué hace el servidor (Node) en producción

- **X-Powered-By:** se elimina (`res.removeHeader('X-Powered-By')`) para no revelar Express/tecnología.
- **Respuestas 5xx:** todo `res.json(...)` con `res.statusCode >= 500` se sanitiza:
  - `error` → siempre `"Internal Server Error"`.
  - Se eliminan `stack`, `details`, `message`, `detail` del cuerpo que va al cliente.
- **Manejador global de errores:** en producción solo se envía `error: 'Internal Server Error'` y `requestId`; no se envía `err.message` ni `stack` al cliente.

Así se evita que en la web se vean trazas de código, rutas de archivos o mensajes internos.

---

## Recomendaciones para el frontend (HTML/JS en platform/)

- **No dejar credenciales ni tokens** en el HTML ni en variables globales en scripts embebidos.
- **console.log:** en producción conviene no loguear datos sensibles (tokens, IDs internos). Si quieres, puedes envolver logs en `if (typeof window !== 'undefined' && window.__DEV__ !== true)` o no usar `console.log` con datos sensibles.
- **Source maps:** si generas bundles minificados, no sirvas `.map` en producción (o sírvelos solo en un entorno interno) para que no se pueda reconstruir el código fuente desde las herramientas de desarrollo.
- **Comentarios:** los comentarios en HTML/JS se envían al navegador; evita comentarios con rutas de servidor, nombres de BD o detalles de implementación.

---

## Resumen

| Dónde              | Qué se hace / recomienda                                      |
|--------------------|---------------------------------------------------------------|
| API (server.js)    | 5xx sanitizados: solo "Internal Server Error", sin stack/detail |
| API (server.js)   | X-Powered-By eliminado                                       |
| Frontend           | Sin credenciales en HTML/JS; cuidado con console.log y .map  |

Con esto se reduce lo que “se mira en código” en la web en producción.
