# Variables de entorno — integración Mamey

Para que el **Node (8545)** use MameyNode (Rust) y servicios SICB reales en lugar de stubs locales.

## Variables

| Variable | Uso | Ejemplo |
|----------|-----|---------|
| **MAMEY_NODE_URL** | URL del MameyNode (Rust). El Node hace proxy de `/api/v1/stats` y otras rutas blockchain a esta URL. | `http://localhost:8545` (mismo proceso) o `http://127.0.0.1:8546` (Rust en otro puerto) |
| **SICB_URL** | (Futuro) URL del servicio SICB real (Treasury, etc.). Hoy los stubs en Node usan almacenamiento local; cuando SICB esté desplegado, apuntar aquí. | `http://127.0.0.1:5003` |
| **PORT** | Puerto del Node Express. Por defecto 8545 (Mamey). | `8545` |
| **TEST_BASE_URL** | Base URL para tests E2E (ej. `e2e-mamey-integration.test.js`). | `http://localhost:8545` |

## Cómo usarlas

1. **Solo Node (sin Rust aparte):** no hace falta definir `MAMEY_NODE_URL`; el Node usa `http://localhost:PORT` y sirve sus propias rutas. `/api/v1/stats` devolverá 503 si no hay nada que responda tipo MameyNode en ese puerto (a menos que el mismo Node implemente stats).
2. **Node + MameyNode Rust en otro puerto:** levantar MameyNode (ej. puerto 8546) y en el .env del Node: `MAMEY_NODE_URL=http://127.0.0.1:8546`. Así `/api/v1/stats` hará proxy al Rust.
3. **Tests E2E:** con el Node arriba, `TEST_BASE_URL=http://localhost:8545 npm test -- e2e-mamey-integration`.

## Comprobar integración

```bash
# Desde la raíz del proyecto
node RuddieSolution/node/scripts/check-mamey-integration.js
```

Ver también: `docs/MAMEY-IO-QUE-NOS-FALTA.md`, `Mamey/README.md`.
