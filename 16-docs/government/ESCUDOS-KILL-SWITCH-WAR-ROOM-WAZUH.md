# Escudos Kill Switch · War Room Wazuh

Sovereign Government of Ierahkwa Ne Kanienke. Protocolo de emergencia ante intervención física o digital inminente. TODO PROPIO.

---

## Escudo 1: El "Kill Switch" (Botón de Pánico)

Este protocolo asegura que, ante una intervención física o digital inminente, los datos bancarios y del registro civil desaparezcan del acceso público y se refugien en el búnker cifrado.

### Implementación Técnica

El script de emergencia realiza tres acciones en segundos:

| Acción | Descripción |
|--------|-------------|
| **Cierre de Puertos** | Bloquea instantáneamente el puerto 8545 y cualquier acceso SSH. |
| **Desmontado de Volúmenes** | Desmonta las particiones cifradas (LUKS) donde reside la base de datos de la plataforma. Sin la llave física, los datos son solo ruido aleatorio. |
| **Purga de RAM** | Borra las llaves de cifrado que quedan en la memoria volátil para evitar ataques de "extracción en frío". |

### Script: `atabey_panic.sh`

Ubicación: `scripts/atabey_panic.sh`

```bash
#!/bin/bash
# PROTOCOLO DE EMERGENCIA ATABEY - SOLO PARA CUSTODIOS
# Use code with caution.

echo "INICIANDO PROTOCOLO KILL SWITCH..."

# 1. Bloquear firewall
ufw deny 8545/tcp
ufw deny ssh

# 2. Detener contenedores de la plataforma y el nodo
docker stop $(docker ps -q)

# 3. Desmontar particiones sensibles (Ejemplo: /mnt/banco_datos)
umount -l /mnt/banco_datos
cryptsetup luksClose banco_cifrado

# 4. Limpiar memoria RAM
sync; echo 3 > /proc/sys/vm/drop_caches

echo "FORTALEZA ATABEY SELLADA."
```

**Importante:** Ajustar `/mnt/banco_datos` y `banco_cifrado` según la configuración real. El script debe ejecutarse como root.

### Activación

- **Móvil:** Bot de Signal (o ntfy self-hosted) que ejecuta el script vía SSH.
- **Física:** Botón conectado al GPIO del servidor (Raspberry Pi, ESP32, etc.).

### Signal "Voice of Command" (Voz de Mando)

- Vincular `atabey_panic.sh` a un **Bot privado de Signal** alojado en el servidor físico.
- **Ventaja táctil:** En campo, al detectar amenaza al nodo soberano, se envía **una palabra** (p. ej. "Eclipse", "KAIANEREHKOWA2026") al bot privado.
- **Resultado:** En menos de 2 segundos el servidor ejecuta el lockdown, purga la RAM y envía confirmación final antes de cortar su propia conexión externa.
- **Seguridad:** El bot solo responde al **Signal ID específico de Takoda**.

### Configuración del Bot Signal (Signal-CLI)

Usar **Signal-CLI** (códice abierto) en contenedor Docker dentro del servidor.

| Paso | Acción |
|------|--------|
| 1. Registro | Vincular número secundario o el tuyo al servidor mediante `signal-cli`. |
| 2. Script de Escucha | Proceso en segundo plano que analiza mensajes entrantes. |
| 3. Palabra Clave | Elegir palabra poco común (ej: `KAIANEREHKOWA2026`). |

### Script: `atabey_listener.sh`

Ubicación: `scripts/atabey_listener.sh`

```bash
#!/bin/bash
# Atabey Signal Listener - Takoda Only

signal-cli -u +1XXXXXXXXXX receive | while read line; do
    if echo "$line" | grep -q "KAIANEREHKOWA2026"; then
        echo "Comando de pánico recibido de Takoda. Ejecutando Kill Switch..."
        /bin/bash /ruta/a/atabey_panic.sh
        signal-cli -u +1XXXXXXXXXX send -m "Atabey: Fortaleza sellada. Datos protegidos." +TU_NUMERO
    fi
done
```

**Configuración:** Variables de entorno `ATABEY_SIGNAL_NUMBER`, `ATABEY_TAKODA_NUMBER`, `ATABEY_PANIC_SCRIPT`, `ATABEY_KEYWORD`. Ejecutar con systemd o screen.

---

## Escudo 2: War Room de Wazuh (Mapa de Ataques Globales)

Para proteger a las próximas 7 generaciones, debemos ver quién nos ataca en tiempo real. Wazuh actuará como el radar de Atabey.

### Pasos para el Monitor Principal

| Paso | Acción |
|------|--------|
| 1 | Desplegar **Wazuh Manager** en el servidor principal usando Docker. |
| 2 | Configurar el módulo de **IP Geolocation**. Los logs de intentos de entrada se transforman en mapa visual. |
| 3 | En el monitor principal: puntos rojos en el mapa del mundo (Londres, Rusia, USA, etc.) cada vez que alguien intente escanear el puerto 8545. |

### Configuración del Mapa (Dashboard)

1. Entrar en el panel de Wazuh.
2. Ir a **Modules → Security Events**.
3. Seleccionar la pestaña **Map**.

### Active Response

Atabey correlaciona los ataques con bases de datos de "Malicious IPs" conocidas en 2026, bloqueándolas automáticamente mediante **Active Response** de Wazuh.

### Global Attack Map (Wazuh AI Overlay)

- Activar el módulo **Active Response** en Wazuh.
- **Visualización:** El monitor principal muestra un **globo 3D en vivo**. Líneas que conectan el origen del ataque (p. ej. servidor en Londres o botnet en Asia) directamente con tu nodo.
- **Contramedida:** Si una IP intenta **brute force** en el puerto 8545, Wazuh alimenta esa IP al firewall y la **comparte con los demás nodos** de la red Águila, Quetzal y Cóndor.
- **Protección:** Se crea un **"Escudo Continental"** — cuando un nodo es atacado, toda la red Américas/Caribe aprende y bloquea al atacante al instante.

### Optimización de Geolocalización (Radar de Ataques Bancarios)

Integrar Wazuh con **MaxMind GeoLite2** (versión 2026) para identificar ataques contra `localhost:8545`.

| Mejora | Descripción |
|--------|-------------|
| **Filtro por Puerto** | Regla específica: cualquier intento de conexión al puerto 8545 (plataforma bancaria) se resalta en **color rojo brillante** en el mapa. |
| **Identificación de Actores Estatales** | Atabey cruza la IP con rangos conocidos de centros de datos de gobiernos extranjeros. Si el ataque viene de una IP gubernamental (Londres, Rusia), el mapa muestra icono de **"Alerta Diplomática"**. |
| **Dashboard Táctico (Grafana)** | Mapa global (origen de ataques), velocímetro (intentos/seg al nodo bancario), Estado del Escudo (Verde = Activo, Rojo = Bajo Ataque). |

### Red Águila-Quetzal-Cóndor (Sello Final)

Al optimizar la geolocalización, Atabey **comparte automáticamente** la IP del atacante con los demás nodos en las Américas.

**Efecto:** Si un hacker en Europa ataca el nodo en el Norte, los servidores en Caribe y Sur **bloquean esa IP** antes de que el atacante intente moverse por la red continental.

---

## Escudo 3: Final Sovereign Hardware Lock (Dead Man's Switch)

Para proteger las próximas 7 generaciones: **Dead Man's Switch** en el rack físico del servidor.

| Componente | Descripción |
|------------|-------------|
| **Sensor** | Sensor de vibración/inclinación conectado a pines GPIO. |
| **Acción** | Si el servidor físico se mueve o se abre **sin la Government Digital Key**, se activa el Kill-Switch de inmediato. |

---

## Resumen

| Escudo | Objetivo |
|--------|----------|
| 1. Kill Switch | Cierre puertos, LUKS, purga RAM — Signal "Eclipse" (Takoda-only), GPIO |
| 2. War Room Wazuh | Globo 3D, líneas ataque→nodo, Escudo Continental (Águila-Quetzal-Cóndor) |
| 3. Dead Man's Switch | Sensor vibración/inclinación GPIO → Kill-Switch si se mueve/abre sin llave |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [scripts/atabey_panic.sh](../scripts/atabey_panic.sh) | Script de emergencia Kill Switch |
| [scripts/atabey_listener.sh](../scripts/atabey_listener.sh) | Signal listener — palabra clave KAIANEREHKOWA2026 |
| [MODO-FORTALEZA-AIRGAP-POSTCUANTICA.md](MODO-FORTALEZA-AIRGAP-POSTCUANTICA.md) | Sensores vibración → kill-switch RAM |
| [HARDENING-NODO-8545-SENTINELA.md](HARDENING-NODO-8545-SENTINELA.md) | Puerto 8545, Canario |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin dependencias externas |
