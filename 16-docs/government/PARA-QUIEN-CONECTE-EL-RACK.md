# Para quien conecte el rack

**Sovereign Government of Ierahkwa Ne Kanienke**  
Guía breve para la persona que vaya a conectar el equipo en el rack y dejar la plataforma en vivo.

---

## Orden de arranque

1. **Conexión física**  
   Conectar red, alimentación y cualquier dispositivo de red (FortiGate, Cisco, etc.) según el diagrama del rack.

2. **Firewall / router**  
   Si hay FortiGate, Cisco o similar, encenderlo y verificar que las reglas permitan el tráfico hacia el servidor de la plataforma (puertos 8545, 3001, 3002 según lo que se exponga).

3. **Servidor de la plataforma**  
   En el servidor donde está el repositorio:
   ```bash
   cd /ruta/al/repo
   ./GO-LIVE-PRODUCTION.sh
   ```
   Ese script:
   - Ejecuta el pre-live check (si algo crítico falla, pregunta si continuar).
   - Verifica enlaces de la plataforma.
   - Arranca Node (8545), Banking Bridge (3001), Editor API (3002) y los servicios configurados.

4. **Comprobar que todo responde**  
   - Navegador: abrir la URL principal (ej. `https://app.ierahkwa.gov` o la que esté configurada).
   - Opcional: ejecutar `./scripts/todo-listo-produccion.sh https://tu-dominio` para una verificación rápida.

---

## URLs principales (ejemplo)

| Uso              | URL típica |
|------------------|------------|
| Portal / login   | `https://app.ierahkwa.gov` o `https://host:8545` |
| Dashboard        | `.../platform/dashboard.html` |
| Sovereign Ops    | `.../platform/sovereign-ops.html` (estado IDS, FIM, pre-live) |
| Health           | `.../health` |
| Pre-live check   | `.../api/v1/fortress/pre-live-check` |

Las rutas exactas dependen de cómo esté configurado el reverse proxy y el dominio.

---

## Dónde están los logs

- **Plataforma (Node):** `RuddieSolution/node/logs/` (p. ej. salida de `server.js` si se redirige ahí).
- **Servidores de plataforma (múltiples):** `RuddieSolution/logs/platform-servers.log`, `RuddieSolution/logs/banking-hierarchy.log` si se usan los arranques que escriben en esos archivos.
- **Security / Fortress:** `RuddieSolution/node/logs/fortress/` (p. ej. `fortress-alert.log`, `security-*.log`).

Revisar estos archivos si algo no responde o hay alertas.

---

## Si algo no responde

1. **Comprobar procesos:** que Node (puerto 8545), Banking Bridge (3001) y Editor API (3002) estén en ejecución.
2. **Comprobar red:** ping al servidor, que los puertos estén abiertos en el firewall.
3. **Logs:** mirar los logs anteriores en busca de errores o mensajes de alerta.
4. **Pre-live:** abrir `/api/v1/fortress/pre-live-check` (o el dashboard Sovereign Ops) y revisar qué check falla (JWT, CORS, NODE_ENV, etc.).
5. **Reinicio controlado:** usar `./stop-all.sh` y luego `./GO-LIVE-PRODUCTION.sh` (o `./start.sh` según cómo esté configurado el entorno).

---

## Servicios soberanos (ya en el repo)

IDS/IPS, FIM, DNS, PKI, correo SMTP y VPN Manager están integrados en el Node; no dependen de terceros. Para hardening opcional:

```bash
./scripts/sovereign-hardening.sh
```

Estado de todos ellos: en el dashboard **Sovereign Ops** (`/platform/sovereign-ops.html`) o en la API `GET /api/v1/sovereign-services/status`.

---

## Verificación rápida “todo listo”

```bash
./scripts/todo-listo-produccion.sh https://tu-dominio
```

Exit 0 = OK para producción; exit 1 = hay algo que corregir (el script indica qué).
