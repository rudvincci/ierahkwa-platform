# 🚀 Arranque automático – Ierahkwa Platform

**Sovereign Government of Ierahkwa Ne Kanienke**

---

## Inicio rápido

| Acción | Comando o gesto |
|--------|------------------|
| **Iniciar (terminal)** | `./start.sh` |
| **Iniciar (doble clic)** | Doble clic en `start.command` |
| **Detener** | `./stop.sh` |
| **Estado** | `./status.sh` |

---

## Arranque al iniciar sesión (macOS)

Para que el nodo se levante **solo al abrir sesión**:

```bash
./install-autostart.sh
```

- Se instala un agente en `~/Library/LaunchAgents/`.
- El nodo se inicia al hacer login y se reinicia si se cae.
- Logs: `logs/ierahkwa-node.log` y `logs/ierahkwa-node.err`.

Para **quitar** el arranque automático:

```bash
./uninstall-autostart.sh
```

---

## URLs (puerto 8545)

- Dashboard: http://localhost:8545  
- VIP: http://localhost:8545/vip  
- RPC: http://localhost:8545/rpc  
- API: http://localhost:8545/api/v1  

---

## Notas

- **`start.sh`** / **`start.command`**: solo el **nodo** (blockchain + API + plataforma).
- **`start-all-services.sh`**: nodo + servidor extra de plataforma (puerto 8080), si lo usas.
- El autostart (`install-autostart.sh`) usa `start.sh` (solo nodo en 8545).
