# Preparar todo para estar READY

Un solo flujo para dejar la plataforma lista: pre-vuelo, datos, verificación live.

---

## Un comando

Desde la **raíz del repo**:

```bash
./scripts/prepare-ready.sh
```

- Comprueba que exista `.env` (y lo crea desde `.env.example` si falta).
- Comprueba que los secrets JWT tengan al menos 32 caracteres (aviso si no).
- Comprueba que **todos los archivos de datos** estén en `RuddieSolution/node/data/` (incl. `bank-registry.json`, casino, sovereignty).
- Si el **Node (8545)** está en marcha, ejecuta **verificar-production-live.js** (health, ready, live, production, bank-registry, bank-status, sovereignty, casino).
- Si todo pasa: **READY**. Si algo falla: **NOT READY** y pasos a seguir.

---

## Sin levantar el servidor (solo pre-vuelo)

Para comprobar solo `.env` y archivos de datos:

```bash
SKIP_SERVER=1 ./scripts/prepare-ready.sh
```

---

## Flujo completo recomendado

| Paso | Comando | Qué hace |
|------|---------|----------|
| 1 | `./scripts/prepare-ready.sh` | Pre-vuelo + datos; si Node está arriba, verificación live. Corregir hasta READY. |
| 2 | `./start.sh` | Arranca Node (8545), Banking Bridge (3001), y servicios configurados. |
| 3 | `./status.sh` | Comprueba que todo esté ONLINE (Node, Bridge, Banco BHBK, etc.). |
| 4 | `cd RuddieSolution/node && node scripts/verificar-production-live.js` | Verificación 100% Production Live (exit 0 = listo). |

Si en el paso 1 ya tenías Node arriba y la verificación pasó, el paso 4 es redundante pero sirve para CI o antes de desplegar.

---

## Checklist rápido

- [ ] Ejecutar `./scripts/prepare-ready.sh` hasta ver **READY**.
- [ ] Rellenar `.env`: `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` (≥32 caracteres). `openssl rand -hex 32`.
- [ ] Todos los archivos de datos presentes (incl. `bank-registry.json`).
- [ ] `./start.sh` — Node y Banking Bridge responden.
- [ ] `./status.sh` — Banco (BHBK) OK, certificados si aplica.
- [ ] Opcional: HTTPS con `scripts/setup-ssl-certbot-nginx.sh` o `scripts/setup-ssl.sh`.

---

## Referencias

- **Checklist 100% production:** `docs/PLATAFORMA-100-PRODUCTION.md`
- **Verificación script:** `RuddieSolution/node/scripts/verificar-production-live.js`
- **Certificados:** `docs/CERTIFICADOS-SSL-TLS.md`
- **Banco (BHBK):** `docs/ARQUITECTURA-BHBK-DEPARTAMENTOS.md`

---

*Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister.*
