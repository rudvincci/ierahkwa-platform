# Configuración de Acceso Remoto y Monitoreo

Sovereign Government of Ierahkwa Ne Kanienke. Guía de verificación para túnel cifrado, monitoreo e interfaz principal. TODO PROPIO.

---

## 1. Configuración de Acceso Remoto

Para configurar el acceso remoto seguro, se requiere establecer una **conexión cifrada**.

| Paso | Acción |
|------|--------|
| **Instrucción** | Acceder al archivo de configuración en el servidor y definir los parámetros necesarios para el túnel cifrado. |
| **Acción** | Una vez configurado, podrás acceder a la interfaz de administración de forma segura desde fuera de la red local. |

**Herramientas:** SSH, Tailscale, WireGuard — ver `ATABEY-CONEXION-EXTERIOR-SEGURA.md`, `HARDENING-NODO-8545-SENTINELA.md`.

---

## 2. Configuración de Monitoreo

La configuración del monitoreo permite **visualizar el estado del sistema**.

| Paso | Acción |
|------|--------|
| **Activación** | Configurar las herramientas de monitoreo para visualizar métricas clave y alertas. |
| **Visualización** | Acceder a esta información a través de la interfaz de administración. |

**Herramientas:** Wazuh, Grafana, Prometheus — ver `ESCUDOS-KILL-SWITCH-WAR-ROOM-WAZUH.md`, `ARQUITECTURA-VIGILANCIA-BRAIN.md`.

---

## 3. Finalización de la Configuración

| Elemento | Descripción |
|----------|-------------|
| **Interfaz principal** | `http://localhost:8545/platform/quantum-platform.html` |
| **Acceso restringido** | El acceso a esta interfaz debe estar **restringido** para garantizar la seguridad. Nunca exponer puerto 8545 a la IP pública. |
| **Verificación** | Asegurarse de que todas las configuraciones se hayan aplicado correctamente. |

---

## Checklist de Verificación

- [ ] Túnel cifrado (SSH/Tailscale/WireGuard) configurado
- [ ] Herramientas de monitoreo (Wazuh, Grafana) activas
- [ ] Puerto 8545 no expuesto a IP pública
- [ ] Acceso a `quantum-platform.html` solo vía túnel o red local

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) | VPN, túnel cifrado |
| [HARDENING-NODO-8545-SENTINELA.md](HARDENING-NODO-8545-SENTINELA.md) | Puerto 8545, Nodo Centinela |
| [ESCUDOS-KILL-SWITCH-WAR-ROOM-WAZUH.md](ESCUDOS-KILL-SWITCH-WAR-ROOM-WAZUH.md) | Wazuh, monitoreo, dashboard |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Grafana, Frigate |
