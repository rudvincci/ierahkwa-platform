# Guia de Comunicaciones Offline / Offline Communications Guide

Version: 1.0
Fecha / Date: 2026-03-01
Clasificacion: Uso interno de guardianes / Internal guardian use

---

## Resumen / Overview

Esta guia describe los tres niveles de comunicacion offline disponibles para guardianes de Ierahkwa cuando la infraestructura de internet no esta disponible. Los niveles se activan de forma progresiva segun la severidad de la situacion.

This guide describes the three levels of offline communication available to Ierahkwa guardians when internet infrastructure is unavailable. Levels are activated progressively based on situation severity.

---

## Paso 1: Bluetooth y Wi-Fi Direct con Briar

### Que es Briar

Briar es una aplicacion de mensajeria que funciona sin internet. Utiliza Bluetooth, Wi-Fi Direct y la red Tor para comunicarse. Para Ierahkwa, Briar es la primera linea de comunicacion cuando el internet falla.

### Requisitos de hardware

- Telefono Android 5.0 o superior
- Bluetooth 4.0 o superior habilitado
- Wi-Fi habilitado (no requiere conexion a router)

### Instalacion

1. Si tienes internet: Descarga Briar desde F-Droid (https://f-droid.org) o desde el kit offline (carpeta `apps/briar.apk`).
2. Si no tienes internet: Transfiere el APK desde otro dispositivo via Bluetooth o cable USB.
3. Abre Briar y crea una cuenta local. No se requiere correo electronico ni numero de telefono.

### Configuracion para Ierahkwa

1. Agrega contactos: Para agregar un contacto Briar ambos dispositivos deben estar fisicamente cerca. Abre Briar, selecciona "Agregar contacto" y escanea el codigo QR del otro guardian.
2. Crea el grupo "Guardianes Ierahkwa" con todos los guardianes de tu zona.
3. Activa la opcion "Conectar via Bluetooth" en Configuracion > Conexiones.
4. Activa la opcion "Conectar via Wi-Fi" para redes locales sin internet.

### Alcance y limitaciones

- Bluetooth: Alcance efectivo de 10-30 metros entre dispositivos.
- Wi-Fi Direct: Alcance efectivo de 50-100 metros en linea de vista.
- Los mensajes se propagan de dispositivo en dispositivo (mesh). Si el Guardian A esta conectado con B, y B con C, un mensaje de A llegara a C cuando B sincronice con C.
- La sincronizacion no es instantanea. Los mensajes pueden tardar minutos u horas en propagarse dependiendo de la proximidad de los guardianes.

### Protocolo de uso en emergencia

1. Al detectar caida de internet, abrir Briar inmediatamente.
2. Enviar mensaje al grupo: "BRIAR ACTIVO [tu codigo de guardian] [hora local]"
3. Mantener Bluetooth y Wi-Fi activos permanentemente.
4. Moverse fisicamente hacia el punto de reunion mas cercano para maximizar conectividad.

---

## Paso 2: Radio LoRa con Meshtastic

### Que es Meshtastic

Meshtastic convierte radios LoRa economicos en una red mesh de largo alcance. Los mensajes saltan de nodo en nodo, cubriendo distancias de varios kilometros sin ninguna infraestructura de internet.

### Requisitos de hardware

- Dispositivo LoRa compatible con Meshtastic. Modelos recomendados:
  - LILYGO T-Beam (GPS integrado, bateria 18650) -- Recomendado para guardianes moviles
  - Heltec WiFi LoRa 32 V3 -- Economico, bueno para nodos fijos
  - RAK WisBlock -- Modular, bajo consumo energetico
- Antena externa (recomendada para mayor alcance): 868MHz (Europa) o 915MHz (Americas)
- Bateria o fuente de alimentacion (panel solar de 6W recomendado para nodos permanentes)
- Cable USB para configuracion inicial
- Telefono con Bluetooth para la app Meshtastic

### Instalacion y configuracion

1. Flashear el firmware Meshtastic en el dispositivo LoRa:
   ```
   pip install meshtastic
   meshtastic --firmware-update
   ```
2. Configurar la region de frecuencia:
   ```
   meshtastic --set lora.region US
   ```
   Usar `US` para Americas (915 MHz). Verificar regulaciones locales.

3. Configurar el canal Ierahkwa:
   ```
   meshtastic --ch-set name "Ierahkwa" --ch-index 0
   meshtastic --ch-set psk random --ch-index 0
   ```
   Distribuir la clave PSK (pre-shared key) a los guardianes de forma presencial.

4. Configurar canales adicionales:
   - Canal 0: Ierahkwa General (todos los guardianes)
   - Canal 1: Emergencias (solo alertas RED)
   - Canal 2: Coordinacion local (por zona)
   ```
   meshtastic --ch-set name "Emergencia" --ch-index 1
   meshtastic --ch-set name "Zona-Local" --ch-index 2
   ```

5. Configurar el nombre del nodo:
   ```
   meshtastic --set-owner "GRD-[tu-codigo]"
   ```

### Alcance y limitaciones

- Alcance nodo a nodo: 2-15 km en terreno plano, 1-5 km en zona urbana.
- Con repetidores en puntos altos: 30-80 km entre repetidores.
- Tamano maximo de mensaje: aproximadamente 230 bytes.
- Velocidad: los mensajes pueden tardar segundos a minutos en propagarse por la malla.
- Sin cifrado por defecto: activar PSK en todos los canales.

### Integracion con el bridge LoRa-Matrix

El script `lora_mesh_bridge.py` conecta automaticamente la red LoRa con Matrix:
- Cuando hay internet: los mensajes criticos de Matrix se reenvian a LoRa, y los mensajes LoRa se publican en Matrix.
- Cuando no hay internet: la red LoRa opera de forma independiente y los mensajes se encolan para sincronizar cuando vuelva la conectividad.

### Protocolo de uso en emergencia

1. Encender el dispositivo LoRa y verificar que aparece en la malla.
2. Enviar beacon en Canal 0: "GRD-[codigo] ACTIVO [hora] [ubicacion]"
3. Monitorear Canal 1 para alertas de emergencia.
4. Si tienes acceso a un punto elevado, posicionar el dispositivo como repetidor.

---

## Paso 3: Dead Drops Fisicos

### Que son los Dead Drops

Un dead drop es un punto fisico preestablecido donde los guardianes pueden dejar y recoger mensajes escritos o en dispositivos USB cuando las comunicaciones electronicas no estan disponibles. Este es el metodo de ultimo recurso.

### Establecimiento de puntos

1. Cada zona de guardianes debe tener minimo 3 puntos de dead drop preestablecidos.
2. Los puntos deben ser:
   - Accesibles a pie desde las zonas de residencia de los guardianes.
   - Discretos pero localizables (un arbol marcado, una piedra especifica, un punto en un parque).
   - Protegidos de la lluvia y la humedad.
3. Las coordenadas de los puntos se comparten unicamente de forma presencial y se almacenan cifradas en el kit offline.

### Materiales necesarios

- Bolsas hermeticas tipo Ziploc para proteger documentos de la humedad.
- Memorias USB con cifrado por hardware o archivos cifrados con GPG.
- Lapiz y papel (resistente al agua si es posible).
- Cinta adhesiva resistente para fijar paquetes.

### Protocolo de dead drop

1. **Deposito**: El guardian deposita el mensaje en el punto acordado y marca el indicador de actividad (por ejemplo, una piedra colocada de cierta forma visible desde la distancia).
2. **Verificacion**: El guardian receptor verifica el indicador y recoge el mensaje.
3. **Confirmacion**: El receptor marca un segundo indicador para confirmar la recogida.
4. **Destruccion**: Los mensajes fisicos se destruyen despues de ser leidos y procesados.

### Formato de mensajes fisicos

Todos los mensajes fisicos deben incluir:
```
[FECHA] [HORA UTC]
[CODIGO DE GUARDIAN EMISOR]
[PRIORIDAD: INFO / ADVERTENCIA / CRITICO]
[MENSAJE]
[FIRMA: ultimos 6 caracteres del hash de tu DID]
```

### Cifrado manual basico

Para mensajes sensibles sin acceso a herramientas digitales, utilizar cifrado de transposicion previamente acordado entre guardianes:

1. Cada par de guardianes acuerda una palabra clave de forma presencial.
2. La palabra clave determina el patron de transposicion.
3. Este metodo no es criptograficamente seguro pero dificulta la lectura casual.

Para mensajes que requieren seguridad real, utilizar GPG en una memoria USB:
```
gpg --armor --encrypt --recipient GUARDIAN_ID mensaje.txt
```

---

## Procedimientos de Emergencia / Emergency Procedures

### Activacion del modo offline

El modo offline se activa automaticamente cuando se cumple alguna de estas condiciones:
1. El Peace Oracle detecta una alerta RED.
2. La conectividad a internet falla por mas de 15 minutos.
3. Un guardian senior emite una orden manual de activacion.

### Secuencia de activacion

1. **Minuto 0**: Se detecta la condicion de activacion.
2. **Minuto 1**: Se ejecuta `survival_sync.sh` para asegurar que el kit offline esta actualizado.
3. **Minuto 2**: Se activa Briar y se envia confirmacion de estado al grupo de guardianes.
4. **Minuto 5**: Se enciende el dispositivo LoRa y se envia beacon.
5. **Minuto 10**: Si no hay respuesta por canales electronicos, se activa protocolo de dead drop.
6. **Minuto 30**: Primer punto de situacion (sitrep) por el canal disponible mas confiable.
7. **Cada hora**: Sitrep de actualizacion hasta que se restablezca la comunicacion normal.

### Desactivacion

El modo offline se desactiva cuando:
1. La conectividad a internet se restablece de forma estable (30 minutos sin interrupcion).
2. El Peace Oracle confirma nivel GREEN.
3. Un guardian senior emite orden de desactivacion.

Al desactivarse, todos los mensajes encolados se sincronizan automaticamente con Matrix.

---

## Requisitos de Hardware Completos / Complete Hardware Requirements

### Kit minimo por guardian

| Componente | Costo aproximado (USD) | Prioridad |
|-----------|----------------------|-----------|
| Telefono Android (usado) | 50-100 | Critico |
| Dispositivo LoRa (T-Beam) | 25-40 | Critico |
| Antena externa 915MHz | 10-15 | Recomendado |
| Bateria externa 10000mAh | 15-20 | Critico |
| Panel solar portatil 6W | 15-25 | Recomendado |
| Memoria USB 16GB | 5-10 | Recomendado |
| Bolsas hermeticas (paquete) | 3-5 | Basico |
| Cable USB-C | 5 | Critico |

### Kit de zona (compartido entre 5-10 guardianes)

| Componente | Costo aproximado (USD) | Prioridad |
|-----------|----------------------|-----------|
| Repetidor LoRa en punto alto | 50-80 | Critico |
| Panel solar 20W + bateria | 40-60 | Critico |
| Caja estanca IP67 | 15-20 | Critico |
| Router mesh (LibreRouter) | 80-120 | Recomendado |

### Mantenimiento

- Verificar carga de baterias semanalmente.
- Probar comunicacion LoRa mensualmente.
- Actualizar firmware de dispositivos trimestralmente.
- Renovar contenido del kit offline cuando se ejecute `survival_sync.sh`.
- Verificar puntos de dead drop mensualmente.
