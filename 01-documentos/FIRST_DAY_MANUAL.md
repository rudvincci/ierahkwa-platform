# Manual del Primer Dia / First Day Manual

Guia de inicio rapido para nuevos Guardianes de Ierahkwa
Quick start guide for new Ierahkwa Guardians

Version: 1.0
Fecha / Date: 2026-03-01

---

## Bienvenido / Welcome

Bienvenido a la red soberana digital Ierahkwa Ne Kanienke. Este manual te guiara paso a paso en la configuracion inicial de todas las herramientas que necesitas como guardian. Sigue cada paso en orden.

Welcome to the Ierahkwa Ne Kanienke sovereign digital network. This manual will guide you step by step through the initial setup of all the tools you need as a guardian. Follow each step in order.

---

## Paso 1: Configuracion de la Antena LoRa / Setting Up the LoRa Antenna

### Que necesitas

- Dispositivo LoRa compatible con Meshtastic (recomendado: LILYGO T-Beam v1.1 o superior)
- Cable USB-C
- Antena 915 MHz (Americas) o 868 MHz (Europa) -- si tu dispositivo no trae antena integrada
- Computadora o telefono Android con Bluetooth
- Bateria 18650 (para T-Beam) o bateria externa USB

### Procedimiento

**1.1 -- Ensamblar el hardware**

Conecta la antena al conector SMA del dispositivo LoRa. Si usas un T-Beam, inserta la bateria 18650 respetando la polaridad (el polo positivo queda hacia la marca +). No enciendas el dispositivo sin antena conectada, ya que el transmisor puede danarse.

**1.2 -- Instalar el firmware Meshtastic**

Conecta el dispositivo a tu computadora via USB. Abre una terminal y ejecuta:

```bash
pip install meshtastic
pip install esptool

# Verificar que el dispositivo es detectado
meshtastic --info
```

Si el dispositivo no es detectado, instala los drivers CP2102 o CH340 segun tu modelo. En Linux puede ser necesario agregar tu usuario al grupo `dialout`:

```bash
sudo usermod -a -G dialout $USER
```

Actualiza el firmware:

```bash
meshtastic --firmware-update
```

**1.3 -- Configurar la region de frecuencia**

```bash
meshtastic --set lora.region US
```

Regiones validas: `US` (Americas 915MHz), `EU_868` (Europa), `CN` (China), `JP` (Japon), `ANZ` (Australia/NZ), `KR` (Corea), `TW` (Taiwan), `IN` (India).

**1.4 -- Configurar tu identidad de nodo**

```bash
meshtastic --set-owner "GRD-[TU-CODIGO]"
meshtastic --set-owner-short "[4 letras]"
```

Reemplaza `[TU-CODIGO]` con el codigo de guardian que recibiste al registrarte (ejemplo: GRD-MX042).

**1.5 -- Unirse al canal Ierahkwa**

La clave del canal te sera proporcionada por tu guardian mentor de forma presencial. Ejecuta:

```bash
# Configurar canal principal
meshtastic --ch-set name "Ierahkwa" --ch-index 0
meshtastic --ch-set psk base64:[CLAVE_PROPORCIONADA] --ch-index 0

# Canal de emergencias
meshtastic --ch-set name "Emergencia" --ch-index 1
meshtastic --ch-set psk base64:[CLAVE_EMERGENCIA] --ch-index 1
```

**1.6 -- Verificar conectividad**

```bash
meshtastic --nodes
```

Deberias ver al menos un nodo vecino. Si no ves ninguno, verifica que la antena esta bien conectada y que hay otros guardianes en tu zona con dispositivos encendidos.

Envia un mensaje de prueba:

```bash
meshtastic --sendtext "GRD-[TU-CODIGO] primer contacto" --ch-index 0
```

**1.7 -- Ubicacion del dispositivo**

Para mejor alcance, ubica el dispositivo en el punto mas alto disponible (ventana alta, techo, poste). Cada metro de altura adicional puede agregar cientos de metros de alcance.

---

## Paso 2: Configuracion de Handshake DNS / Configuring Handshake DNS

### Que es Handshake DNS

Handshake es un protocolo de nombres de dominio descentralizado. Ierahkwa utiliza dominios .ierahkwa que no dependen de registradores convencionales (ICANN). Esto garantiza que nuestros dominios no pueden ser censurados, confiscados ni desactivados por terceros.

### Procedimiento

**2.1 -- Instalar el resolver Handshake**

Opcion A -- HNSD (resolutor ligero):

```bash
git clone https://github.com/handshake-org/hnsd.git
cd hnsd
./autogen.sh && ./configure && make
sudo make install
```

Opcion B -- Usar un servidor DNS recursivo que soporte Handshake:

Edita tu configuracion DNS para apuntar a un resolver compatible. Agrega a `/etc/resolv.conf` o a la configuracion de tu red:

```
nameserver 103.196.38.38
nameserver 103.196.38.39
```

Estos son servidores DNS publicos que resuelven dominios Handshake. Para mayor privacidad, ejecuta tu propio resolver.

**2.2 -- Verificar resolucion**

```bash
# Probar que los dominios .ierahkwa resuelven correctamente
dig @103.196.38.38 portal.ierahkwa
nslookup portal.ierahkwa 103.196.38.38
```

Si obtienes una respuesta con una direccion IP, la configuracion es correcta.

**2.3 -- Configuracion en el navegador**

Para Firefox:
1. Abre `about:config`
2. Busca `network.trr.mode` y establece el valor en `3` (solo DNS-over-HTTPS)
3. Busca `network.trr.uri` y establece la URL de tu resolver Handshake DoH

Para navegadores basados en Chromium:
1. Abre Configuracion > Privacidad y seguridad > Seguridad
2. En "Usar DNS seguro", selecciona "Con" y establece la URL del resolver

**2.4 -- Verificar en el navegador**

Navega a `http://portal.ierahkwa/` en tu navegador. Deberias ver la pagina principal del portal Ierahkwa.

---

## Paso 3: Carga del Mediador AI / Loading the AI Mediator

### Que es el Mediador AI

El mediador AI es un servicio que analiza las conversaciones en los canales Matrix de Ierahkwa para detectar hostilidad y sugerir reformulaciones constructivas. Utiliza un modelo de lenguaje local (Ollama) para preservar la privacidad.

### Procedimiento

**3.1 -- Instalar Ollama**

```bash
# Linux / macOS
curl -fsSL https://ollama.com/install.sh | sh

# Verificar instalacion
ollama --version
```

**3.2 -- Descargar el modelo**

```bash
ollama pull mistral
```

El modelo Mistral ocupa aproximadamente 4 GB. En conexiones lentas, este paso puede tardar. El modelo se almacena localmente y funciona sin internet una vez descargado.

Verifica que el modelo funciona:

```bash
ollama run mistral "Responde solo con OK si puedes leer esto."
```

**3.3 -- Configurar variables de entorno**

Crea un archivo `.env` en el directorio `scripts/protocols/`:

```bash
# Matrix
MATRIX_HOMESERVER=https://matrix.ierahkwa.org
MATRIX_USER=@mediador-[tu-zona]:ierahkwa.org
MATRIX_PASSWORD=[contrasena proporcionada por tu mentor]

# Ollama
OLLAMA_URL=http://localhost:11434
OLLAMA_MODEL=mistral

# Mediador
TOXICITY_THRESHOLD=0.55
LOG_DIR=logs/
```

Nota: La contrasena de Matrix te sera proporcionada por tu guardian mentor. Nunca la compartas ni la almacenes en repositorios publicos.

**3.4 -- Ejecutar el test del mediador**

Antes de activar el mediador, verifica que funciona correctamente:

```bash
cd scripts/protocols/
python test_mediator.py
```

El test ejecuta 10 mensajes (5 polarizados y 5 positivos) y genera un reporte. Todos los tests deben pasar antes de activar el mediador en produccion.

**3.5 -- Iniciar el mediador**

```bash
# Cargar variables de entorno
export $(cat .env | xargs)

# Ejecutar
python mediator.py
```

Para ejecucion persistente con systemd:

```bash
sudo tee /etc/systemd/system/ierahkwa-mediator.service << SERVICEEOF
[Unit]
Description=Ierahkwa AI Mediator
After=network.target

[Service]
Type=simple
User=$USER
WorkingDirectory=$(pwd)
EnvironmentFile=$(pwd)/.env
ExecStart=$(which python3) $(pwd)/mediator.py
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
SERVICEEOF

sudo systemctl daemon-reload
sudo systemctl enable ierahkwa-mediator
sudo systemctl start ierahkwa-mediator
```

---

## Paso 4: Configuracion de Alertas del Peace Oracle / Configuring Peace Oracle Alerts

### Que es el Peace Oracle

El Peace Oracle monitorea la API de ACLED (Armed Conflict Location & Event Data) para detectar eventos de conflicto en la region de las Americas. Clasifica cada evento como GREEN, YELLOW o RED y envia notificaciones push cuando se detectan amenazas.

### Procedimiento

**4.1 -- Obtener credenciales ACLED**

1. Registrate en https://acleddata.com/register/ (es gratuito para uso academico y comunitario).
2. Una vez registrado, obtendras un API key y tendras asociado tu correo electronico.

**4.2 -- Instalar ntfy en tu telefono**

1. Descarga la app ntfy desde F-Droid o Google Play.
2. Abre ntfy y suscribete al topic: `ierahkwa-[tu-region]` (ejemplo: `ierahkwa-MX-OAX`).
3. Si la red usa un servidor ntfy auto-alojado, configura la URL del servidor en los ajustes de la app.

**4.3 -- Configurar variables de entorno**

Agrega a tu archivo `.env`:

```bash
# Peace Oracle
ACLED_API_KEY=[tu-api-key]
ACLED_EMAIL=[tu-email-registrado]
POLL_INTERVAL=3600
DB_PATH=data/peace_oracle.db
NTFY_URL=https://ntfy.sh
NTFY_TOPIC=ierahkwa-[tu-region]
```

**4.4 -- Ejecutar el Peace Oracle**

```bash
export $(cat .env | xargs)
python peace_oracle.py
```

La primera ejecucion descargara los eventos mas recientes y los almacenara en la base de datos SQLite local. Las ejecuciones posteriores solo descargaran eventos nuevos.

**4.5 -- Configurar ejecucion automatica con cron**

```bash
# Editar crontab
crontab -e

# Agregar linea para ejecutar cada hora
0 * * * * cd /ruta/a/scripts/protocols && /usr/bin/python3 peace_oracle.py >> logs/peace_oracle_cron.log 2>&1
```

**4.6 -- Verificar notificaciones**

Para probar que las notificaciones funcionan, utiliza el script de notificacion directa:

```bash
python notify_guardians.py 3 "Test de notificacion - Peace Oracle configurado correctamente" --region [tu-region]
```

Deberias recibir una notificacion en tu telefono a traves de la app ntfy.

---

## Paso 5: Protocolo de Latido (Heartbeat) / Heartbeat Protocol Explanation

### Que es el Heartbeat

El protocolo de latido (heartbeat) es el mecanismo que permite a la red Ierahkwa saber que un guardian y su nodo estan activos. Cada nodo envia una senal periodica que confirma su presencia. Si un nodo deja de enviar latidos, se activan mecanismos de alerta.

### Como funciona

**Frecuencia**: Cada nodo envia un latido cada 5 minutos.

**Contenido del latido**: El mensaje de heartbeat contiene:
- Codigo de guardian (GRD-XXXXX)
- Timestamp UTC
- Estado del nodo (online, degradado, solo-lora)
- Nivel de bateria del dispositivo LoRa (si aplica)
- Numero de nodos vecinos visibles
- Hash de la ultima configuracion aplicada

**Canales de heartbeat**:
1. **Primario -- Matrix**: El nodo envia un heartbeat via Matrix al canal #heartbeat:ierahkwa.org.
2. **Secundario -- LoRa**: Simultaneamente, el dispositivo LoRa envia un beacon al canal 0 de la malla.
3. **Terciario -- HTTP**: Si hay internet, el nodo envia un POST al endpoint `/api/heartbeat` del servidor central.

**Deteccion de ausencia**:
- Si un nodo no envia heartbeat en 15 minutos: se marca como "posiblemente offline" (estado amarillo en el dashboard).
- Si un nodo no envia heartbeat en 1 hora: se marca como "offline" (estado rojo).
- Si un nodo no envia heartbeat en 24 horas: se envia una notificacion al guardian senior de la zona.

### Configuracion del heartbeat

El heartbeat se configura automaticamente al iniciar el mediador AI y el bridge LoRa. Para verificar que tu heartbeat esta funcionando:

```bash
# Verificar heartbeat HTTP
curl -s http://localhost:8080/health | python3 -m json.tool

# Verificar heartbeat LoRa
meshtastic --nodes | grep "GRD-[TU-CODIGO]"

# Verificar logs de heartbeat
tail -20 logs/heartbeat.log
```

### Heartbeat manual

Si los servicios automaticos no estan activos, puedes enviar un heartbeat manual:

```bash
# Via LoRa
meshtastic --sendtext "HB|GRD-[TU-CODIGO]|$(date -u +%H:%M)|OK" --ch-index 0

# Via ntfy (como respaldo)
curl -d "HB|GRD-[TU-CODIGO]|$(date -u +%H:%M)|OK" https://ntfy.sh/ierahkwa-heartbeat
```

### Por que importa

El heartbeat es la forma en que la red mide su salud. Los datos de heartbeat alimentan:
- El indicador de salud de la red en el dashboard.
- El calculo de $MATTR (mantener el nodo activo 24h genera +1 $MATTR diario).
- Las metricas del Chaos Scheduler (medir tiempo de respuesta en simulacros).
- La deteccion de zonas desconectadas que pueden necesitar apoyo.

---

## Lista de Verificacion del Primer Dia / First Day Checklist

Marca cada paso a medida que lo completes:

```
[ ] 1.1 Hardware LoRa ensamblado y antena conectada
[ ] 1.2 Firmware Meshtastic instalado
[ ] 1.3 Region de frecuencia configurada
[ ] 1.4 Identidad de nodo configurada
[ ] 1.5 Canal Ierahkwa configurado con clave PSK
[ ] 1.6 Conectividad verificada (al menos 1 nodo vecino visible)
[ ] 1.7 Dispositivo ubicado en punto alto
[ ] 2.1 Resolver Handshake DNS instalado
[ ] 2.2 Resolucion DNS verificada por terminal
[ ] 2.3 Navegador configurado
[ ] 2.4 Portal Ierahkwa accesible desde el navegador
[ ] 3.1 Ollama instalado
[ ] 3.2 Modelo descargado y funcionando
[ ] 3.3 Variables de entorno configuradas
[ ] 3.4 Test del mediador pasado exitosamente
[ ] 3.5 Mediador ejecutandose
[ ] 4.1 Credenciales ACLED obtenidas
[ ] 4.2 App ntfy instalada y suscrita al topic
[ ] 4.3 Variables del Peace Oracle configuradas
[ ] 4.4 Peace Oracle ejecutado exitosamente
[ ] 4.5 Cron configurado
[ ] 4.6 Notificacion de prueba recibida
[ ] 5.0 Heartbeat funcionando (verificado en dashboard o logs)
```

### Siguiente paso

Una vez completada esta lista, contacta a tu guardian mentor para confirmar tu activacion. Tu mentor verificara tu heartbeat en el dashboard y te asignara tu primera mision de monitoreo.

Bienvenido a la red. Tu soberania digital comienza hoy.
