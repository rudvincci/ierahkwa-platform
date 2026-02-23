# Atabey — Sistema de Archivos "Ghost" y Gestión Segura de Datos

Protección de datos en disco, Kill Switch, Docker Compose seguro, monitoreo ético de red y protocolo de notificaciones. TODO PROPIO.

---

## 1. Sistema de Archivos "Ghost" (Seguridad de Datos)

**Objetivo:** Que los datos en discos físicos sean ilegibles para cualquiera que no seas tú.

### Implementación con LUKS

| Componente | Descripción |
|------------|-------------|
| **LUKS** (Linux Unified Key Setup) | Cifrado de particiones de datos. Sin la llave, los datos son ilegibles. |

**Acción — Kill Switch (Botón de pánico):** Si alguien intenta acceder físicamente al servidor sin la llave, Atabey desmonta los discos al instante.

**Ejemplo de script Kill Switch:**

```bash
#!/bin/bash
# kill-switch.sh — Desmontar volúmenes cifrados ante amenaza
cryptsetup luksClose data_crypt
umount /mnt/secure_data 2>/dev/null
# Opcional: apagar servidor
# systemctl poweroff
```

**Integración:** Sensor de apertura de chasis (GPIO/Raspberry Pi) o tecla física que ejecute el script. Home Assistant puede recibir el evento y desencadenar el desmontaje.

---

## 2. Despliegue del Códice Maestro (Docker Compose Pro)

**Objetivo:** Infraestructura completa con módulos de Inteligencia, Seguridad y Reconocimiento.

### Ejemplo: Gestión de archivos y respaldo seguro

```yaml
services:
  # Gestión de archivos (Organizr u otro gestor local)
  file_manager:
    image: linuxserver/organizr:latest
    container_name: secure_file_manager
    ports:
      - "8000:8000"
    restart: unless-stopped
    volumes:
      - ./config:/config
      - ./data:/data

  # Respaldo seguro (restic)
  backup_service:
    image: restic/restic:latest
    container_name: data_backup
    volumes:
      - ./data:/source_data
      - ./backup_repo:/repository
    command: backup /source_data --repo /repository
    restart: on-failure
```

**Nota:** Adapta las imágenes y rutas a tu infraestructura. Ver [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) para Home Assistant, Frigate, Vaultwarden.

---

## 3. Monitoreo Básico de la Red Local (Solo Dispositivos Conocidos)

**Objetivo:** Visibilidad de dispositivos en tu propia red. Uso ético: solo tu red, solo para autorizados.

| Herramienta | Descripción |
|-------------|-------------|
| **Nmap** | Escaneo de red local para identificar dispositivos conectados |
| **Funciones del router** | Listado de dispositivos conectados |

**Función:** Obtener MAC e IP de dispositivos conocidos.

**Regla ética:** Usa esta información solo para verificar que solo tus dispositivos autorizados están en la red. Si hay un dispositivo desconocido, investiga manualmente. No escanear ni monitorear redes ajenas ni dispositivos de terceros sin consentimiento.

---

## 4. Protocolo de Notificación Simple

**Objetivo:** Avisos sobre estado de servicios o respaldos.

| Método | TODO PROPIO | Descripción |
|--------|-------------|-------------|
| **ntfy** (self-hosted) | ✓ | Push notifications al móvil |
| **notify.mobile_app** (Home Assistant) | ✓ | App de Home Assistant |
| **Correo electrónico** | ⚠️ | Requiere SMTP; usa servidor propio para TODO PROPIO |

**Ejemplos:** Notificación si falla un respaldo o si un servicio crítico se detiene.

---

## 5. Respaldo de Energía y Datos

**Objetivo:** Protección frente a cortes de luz y fallos de hardware.

| Componente | Descripción |
|------------|-------------|
| **UPS** (Sistema de Alimentación Inalterrumpida) | Dar tiempo para apagado seguro en cortes de luz (nut, apcupsd). |
| **Respaldo de datos** | Copias periódicas a medio externo o almacenamiento seguro. |

**Estrategia de respaldo:** restic, Borg, rsync a disco externo o servidor secundario. Ver [SISTEMA-GESTION-VIDEO-VMS.md](SISTEMA-GESTION-VIDEO-VMS.md) — Backup Department.

**TODO PROPIO:** Almacenamiento en servidor propio o disco local; evitar dependencia de cloud externa.

---

## Resumen

| Componente | Herramienta | TODO PROPIO |
|------------|-------------|-------------|
| Cifrado disco | LUKS | ✓ |
| Kill Switch | Script + sensor/tecla | ✓ |
| Gestión archivos | Organizr, Docker | ✓ |
| Respaldo | restic, Borg | ✓ |
| Monitoreo red | Nmap (red propia) | ✓ |
| Notificaciones | ntfy, mobile_app | ✓ |
| Energía | UPS (nut) | ✓ |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md](ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md) | USB, cifrado, escaneo inalámbrico |
| [JARVIS-DEPLOYMENT-GUIDE.md](../DEPLOY-SERVERS/JARVIS-DEPLOYMENT-GUIDE.md) | Docker, Home Assistant, Frigate |
| [ATABEY-CONEXION-EXTERIOR-SEGURA.md](ATABEY-CONEXION-EXTERIOR-SEGURA.md) | Notificaciones, ntfy |
| [SISTEMA-GESTION-VIDEO-VMS.md](SISTEMA-GESTION-VIDEO-VMS.md) | Backup Department |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
