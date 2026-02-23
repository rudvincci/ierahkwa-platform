# Rotación de secrets JWT — Sin downtime

**Sovereign Government of Ierahkwa Ne Kanienke**  
Cómo rotar `JWT_ACCESS_SECRET` y `JWT_REFRESH_SECRET` en producción sin cortar sesiones activas.

---

## 1. Variables a rotar

| Variable | Uso |
|----------|-----|
| `JWT_ACCESS_SECRET` | Firma de access tokens (corta vida, ej. 15 min) |
| `JWT_REFRESH_SECRET` | Firma de refresh tokens (larga vida, ej. 7 días) |

---

## 2. Estrategia recomendada (doble clave)

1. **Añadir nuevas claves en `.env`** (sin borrar las viejas todavía):
   - `JWT_ACCESS_SECRET_NEW=...`
   - `JWT_REFRESH_SECRET_NEW=...`

2. **Actualizar el código** (si no está ya) para que al **verificar** un token acepte:
   - Primero la clave actual (`JWT_ACCESS_SECRET`).
   - Si falla, probar la clave nueva (`JWT_ACCESS_SECRET_NEW`).
   Así los tokens emitidos con la clave vieja siguen siendo válidos hasta su expiración.

3. **Al emitir tokens nuevos**, usar solo las claves nuevas (`_NEW`).

4. **Ventana de rotación:** dejar 1–2 veces el tiempo máximo de vida del refresh token (ej. 14 días si el refresh vive 7 días).

5. **Pasado ese tiempo:** eliminar del `.env` las claves viejas y renombrar `_NEW` a las definitivas; reiniciar la app una vez.

---

## 3. Rotación rápida (con corte de sesión)

Si no necesitas mantener sesiones:

1. Generar nuevos secrets: `openssl rand -hex 32` (dos veces).
2. Sustituir en `.env` `JWT_ACCESS_SECRET` y `JWT_REFRESH_SECRET`.
3. Reiniciar el Node. Todos los usuarios tendrán que volver a iniciar sesión.

---

## 4. Referencias

- Checklist producción: `docs/GO-LIVE-CHECKLIST.md`
- Variables de entorno: `RuddieSolution/node/.env.example`
