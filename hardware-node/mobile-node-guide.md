# Nodos Moviles Soberanos / Sovereign Mobile Nodes

## Gobierno Soberano de Ierahkwa Ne Kanienke

---

## Por que nodos moviles? / Why Mobile Nodes?

> Una red que se mueve es imposible de mapear y atacar.
> A network that moves is impossible to map and attack.

Los nodos fijos son vulnerables. Pueden ser localizados, confiscados, desconectados.
Un nodo movil montado en un vehiculo, mochila o embarcacion puede cruzar fronteras,
evadir censura, y extender la red soberana a comunidades remotas en cualquier punto
de las Americas.

Fixed nodes are vulnerable. They can be located, confiscated, disconnected.
A mobile node mounted in a vehicle, backpack, or boat can cross borders,
evade censorship, and extend the sovereign network to remote communities
anywhere across the Americas.

**Casos de uso / Use cases:**

- Van-life Guardians que viajan entre comunidades indigenas
- Vehiculos de emergencia con comunicaciones soberanas
- Embarcaciones en rios y costas del Caribe y el Pacifico
- Mochileros que llevan la red a comunidades sin infraestructura
- Mercados itinerantes conectados a ComercioSoberano

---

## 1. Energia / Power System

### Panel Solar / Solar Panel

| Especificacion    | Minimo         | Recomendado     |
|-------------------|----------------|-----------------|
| Potencia          | 100W           | 200W            |
| Tipo              | Monocristalino | Monocristalino  |
| Voltaje           | 18V            | 18-36V          |
| Tamano            | 120x54 cm      | 160x70 cm       |
| Peso              | 7 kg           | 12 kg           |
| Plegable          | Opcional       | Recomendado     |

### Bateria / Battery

| Especificacion    | Minimo         | Recomendado     |
|-------------------|----------------|-----------------|
| Quimica           | LiFePO4        | LiFePO4         |
| Capacidad         | 100Ah (12V)    | 200Ah (12V)     |
| Energia           | 1,280 Wh       | 2,560 Wh        |
| Ciclos            | 3,000+         | 5,000+          |
| BMS integrado     | Si             | Si              |
| Peso              | 12 kg          | 22 kg           |

> LiFePO4 es la unica quimica aceptable para nodos soberanos: no se incendia,
> soporta 3,000+ ciclos, y funciona en temperaturas extremas.

> LiFePO4 is the only acceptable chemistry for sovereign nodes: it does not
> catch fire, supports 3,000+ cycles, and works in extreme temperatures.

### Controlador de Carga / Charge Controller

- Tipo: MPPT (maxima eficiencia)
- Corriente: 30A minimo
- Voltaje de entrada: Hasta 100V
- Recomendado: Victron SmartSolar 100/30 o Renogy Rover 40A
- Monitoreo via Bluetooth para verificar carga desde el telefono

### Autonomia estimada / Estimated Runtime

| Configuracion     | Consumo    | Autonomia sin sol |
|-------------------|------------|-------------------|
| Pi 5 + LoRa       | ~15W       | 85 horas (100Ah)  |
| Pi 5 + LoRa + HF  | ~25W       | 51 horas (100Ah)  |
| NUC + Todo        | ~45W       | 28 horas (100Ah)  |

---

## 2. Conectividad / Connectivity

### 2.1 Starlink (Primario / Primary)

- Cuando esta disponible, ofrece internet de alta velocidad
- Requiere vista clara del cielo (no funciona bajo techo denso)
- Consumo: ~50-75W (alimentar desde bateria LiFePO4)
- **IMPORTANTE**: Todo trafico debe pasar por Tor o VPN
- Plan: Starlink Roam (portabilidad entre paises)

### 2.2 LoRa Mesh (Siempre activo / Always-on secondary)

- Red mesh independiente del internet
- Alcance: 5-15 km (linea de vista), 1-3 km (urbano)
- Hardware: LilyGo T-Beam v1.2 con Meshtastic
- Frecuencia: 915 MHz (Americas) / 868 MHz (Europa)
- Enlace a `lora_mesh_bridge.py` para puente Matrix
- Sin costo de operacion / Zero operating cost

### 2.3 JS8Call HF Radio (Respaldo global / Global fallback)

- Comunicacion global via rebote ionosferico
- Funciona sin internet, sin infraestructura, sin satelites
- Alcance: Global (senal debil, -24 dB SNR)
- Requiere licencia de radioaficionado
- Enlace a `js8call_bridge.py` para puente Matrix
- Bandas: 40m (7.078 MHz) y 20m (14.078 MHz)
- Velocidad: ~50 caracteres por transmision

### 2.4 LTE/5G (Oportunista / Opportunistic)

- Usar SOLO cuando otras opciones no estan disponibles
- **OBLIGATORIO**: VPN/Tor en todo momento
- SIM prepago anonima preferida
- No confiar en LTE como canal primario — es vigilado
- Util para sincronizar grandes volumenes de datos rapidamente

### Prioridad de conexion / Connection Priority

```
1. Starlink (si hay cielo abierto) + Tor
2. LoRa mesh (siempre activo para mensajes criticos)
3. JS8Call HF (cuando todo falla — alcance global)
4. LTE/5G (ultimo recurso, siempre via Tor)
```

---

## 3. Hardware

### 3.1 Computadora / Computer

**Opcion A: Raspberry Pi 5 (Recomendado para la mayoria)**

| Especificacion    | Valor                    |
|-------------------|--------------------------|
| RAM               | 4 GB minimo, 8 GB ideal  |
| Almacenamiento    | microSD 64GB A2 + SSD USB|
| Consumo           | 5-12W                    |
| Precio            | $60-80 USD               |
| Ventaja           | Bajo consumo, compacto   |

**Opcion B: Intel NUC (Para nodos de alto rendimiento)**

| Especificacion    | Valor                    |
|-------------------|--------------------------|
| CPU               | Intel N100 o superior    |
| RAM               | 16 GB                    |
| Almacenamiento    | 256 GB NVMe              |
| Consumo           | 15-45W                   |
| Precio            | $200-400 USD             |
| Ventaja           | Ollama AI local rapido   |

### 3.2 Radio LoRa

- **LilyGo T-Beam v1.2** (ESP32 + SX1262 + GPS + 18650)
- Firmware: Meshtastic (ultima version estable)
- Antena: Reemplazar stock con antena externa para vehiculo
- Conexion al Pi via USB-C

### 3.3 Radio HF (para JS8Call)

| Radio              | Precio    | Potencia | Peso   | Notas                    |
|--------------------|-----------|----------|--------|--------------------------|
| Yaesu FT-891       | $650      | 100W     | 2.2 kg | Compacto, ideal vehiculo |
| Yaesu FT-818ND     | $550      | 6W       | 0.9 kg | QRP, ideal mochila       |
| Xiegu G90          | $450      | 20W      | 1.5 kg | Buen balance             |
| IC-705 (Icom)      | $1,300    | 10W      | 1.1 kg | Premium, pantalla tactil |

> Requiere licencia de radioaficionado (Technician o General).
> Amateur radio license required (Technician or General class).

### 3.4 Antenas

**Antena VHF/UHF (para LoRa y comunicacion local):**
- Diamond X50 (dual-band 144/430 MHz)
- Montaje magnetico para vehiculo
- Ganancia: 4.5 dBi / 7.2 dBi

**Antena HF (para JS8Call):**
- Dipolo de 20m (corte para 14.078 MHz): 2 x 5.05m
- Alambre #14 AWG, aisladores ceramicos
- Montaje: Entre arboles, postes, o soporte telescopico
- Alternativa vehicular: Hamstick o MFJ-1640T (antena movil HF)

### 3.5 Proteccion

- **Pelican Case 1520** o similar (IP67, resistente a impactos)
- Pasamuros para cables USB, coaxial, y alimentacion
- Ventilacion con filtro de polvo
- Cierre con candado para seguridad fisica

---

## 4. Stack de Software / Software Stack

Todos los servicios corren en Docker sobre el Pi o NUC:

```
docker-compose.yml (sovereign-mobile)
|
+-- sovereign-core        (API Gateway, servicios basicos)
+-- synapse               (Matrix server, comunicacion cifrada)
+-- ollama                (AI local — llama3 cuantizado)
+-- meshtastic-bridge     (lora_mesh_bridge.py)
+-- js8call-bridge        (js8call_bridge.py, si hay HF)
+-- tor                   (Hidden service + SOCKS proxy)
+-- ipfs                  (Almacenamiento distribuido)
+-- ntfy                  (Notificaciones push)
+-- postgres              (Base de datos local)
+-- redis                 (Cache)
```

---

## 5. Instalacion / Installation Steps

### Paso 1: Preparar el hardware

1. Flashear Raspberry Pi OS (64-bit, Bookworm) en microSD
2. Conectar SSD USB externo (datos persistentes)
3. Conectar LilyGo T-Beam via USB
4. (Opcional) Conectar radio HF via USB CAT

### Paso 2: Ejecutar el setup

```bash
# Clonar el repositorio
git clone https://github.com/soberano/red-soberana.git
cd red-soberana

# Ejecutar setup automatizado
sudo bash hardware-node/raspberry-pi-setup.sh

# Generar secretos
bash scripts/generate-secrets.sh
```

### Paso 3: Configurar conectividad

```bash
# Configurar Meshtastic
~/configure-meshtastic.sh

# Configurar Tor
sudo cp 04-infraestructura/tor/torrc /etc/tor/torrc
sudo systemctl restart tor

# (Opcional) Configurar JS8Call
# Instalar JS8Call, habilitar TCP API en puerto 2442
```

### Paso 4: Levantar servicios

```bash
# Levantar todos los contenedores
docker compose -f docker-compose.mobile.yml up -d

# Verificar estado
bash scripts/protocols/ierahkwa_check.sh
```

### Paso 5: Verificar conectividad

```bash
# Test LoRa
python3 scripts/protocols/lora_mesh_bridge.py --test

# Test Matrix
curl -s http://127.0.0.1:8008/_matrix/client/versions | python3 -m json.tool

# Test IPFS
curl -s -X POST http://127.0.0.1:5101/api/v0/id | python3 -m json.tool
```

### Paso 6: Configurar auto-inicio

```bash
sudo systemctl enable ierahkwa-node
sudo systemctl start ierahkwa-node
```

---

## 6. Seguridad / Security

### Reglas obligatorias / Mandatory Rules

1. **Tor siempre activo** — Todo trafico pasa por Tor o VPN. Sin excepciones.
2. **VPN killswitch** — Si la VPN cae, todo trafico de internet se bloquea.
3. **tactical-wipe.sh** — Boton de panico que borra claves y datos sensibles.
4. **Cifrado de disco** — LUKS en el SSD externo. Sin clave = sin datos.
5. **Sin logs de ubicacion** — GPS deshabilitado para Matrix y servicios publicos.

### Boton de panico / Panic Button

```bash
# Asignar a un boton fisico GPIO o tecla rapida:
bash scripts/security/tactical-wipe.sh

# Esto ejecuta:
# 1. Borra claves privadas de Matrix, Tor, blockchain
# 2. Borra wallet de $MATTR
# 3. Sobrescribe con datos aleatorios
# 4. Desmonta SSD cifrado
# 5. Apaga el nodo
```

### Privacidad de GPS / GPS Privacy

- El GPS del T-Beam esta activo SOLO para Meshtastic mesh (posicion entre nodos)
- **NUNCA** compartir ubicacion GPS con Matrix, IPFS, o servicios de internet
- Compartir posicion solo con Guardianes de confianza via canal cifrado LoRa
- Deshabilitar `position.gps_enabled` en Meshtastic si no se necesita

---

## 7. Niveles de Presupuesto / Budget Tiers

### Basico / Basic — $200 USD

| Componente          | Precio    |
|---------------------|-----------|
| Raspberry Pi 5 4GB  | $60       |
| microSD 64GB        | $10       |
| LilyGo T-Beam      | $35       |
| Panel solar 100W    | $50       |
| Bateria LiFePO4 20Ah| $40       |
| Caja protectora     | $5        |
| **Total**           | **$200**  |

> Nodo basico LoRa con energia solar. Sin HF, sin SSD.
> Basic LoRa node with solar power. No HF, no SSD.

### Estandar / Standard — $800 USD

| Componente          | Precio    |
|---------------------|-----------|
| Raspberry Pi 5 8GB  | $80       |
| SSD 256GB + case    | $40       |
| LilyGo T-Beam      | $35       |
| Panel solar 200W    | $100      |
| Bateria LiFePO4 100Ah| $250     |
| MPPT controller     | $60       |
| Diamond X50 antena  | $80       |
| Pelican case 1520   | $100      |
| Cables y accesorios | $55       |
| **Total**           | **$800**  |

> Nodo completo LoRa + solar robusto. Ideal para van-life.
> Full LoRa node + robust solar. Ideal for van-life.

### Soberania Completa / Full Sovereignty — $2,500 USD

| Componente          | Precio    |
|---------------------|-----------|
| Intel NUC N100      | $300      |
| SSD 512GB NVMe      | $50       |
| LilyGo T-Beam      | $35       |
| Yaesu FT-891 HF     | $650      |
| Dipolo 20m + coaxial| $40       |
| Panel solar 200W x2 | $200      |
| Bateria LiFePO4 200Ah| $450     |
| MPPT controller 50A | $100      |
| Diamond X50 antena  | $80       |
| Starlink dish       | $300      |
| Pelican case 1550   | $150      |
| Cables, montaje     | $145      |
| **Total**           | **$2,500**|

> Nodo soberano completo: LoRa + HF + Starlink + AI local.
> Communication reaches anywhere on Earth.
> Full sovereign node: LoRa + HF + Starlink + local AI.

---

## 8. Diagrama de Montaje Vehicular / Vehicle Mounting Diagram

```
    VEHICULO / VEHICLE (vista lateral / side view)
    _______________________________________________
   /                                               \
  /   [Panel Solar 200W]                            \
 /     montado en rack de techo                      \
|______________________________________________________|
|                                                      |
|   Techo / Roof:                                      |
|     [Diamond X50]  antena LoRa/VHF (magnetica)       |
|     [Hamstick HF]  antena HF 20m (base movil)        |
|     [Starlink]     dish (cuando estacionado)          |
|                                                      |
|   Interior / Inside:                                 |
|   +-------------------------------------------+      |
|   |  PELICAN CASE (bajo asiento o en carga)   |      |
|   |  +--------+  +--------+  +----------+    |      |
|   |  | Pi 5   |  | T-Beam |  | FT-891   |    |      |
|   |  | + SSD  |  | LoRa   |  | HF Radio |    |      |
|   |  +--------+  +--------+  +----------+    |      |
|   |  +--------+  +-------------------+       |      |
|   |  | MPPT   |  | LiFePO4 200Ah    |       |      |
|   |  | Ctrlr  |  | Battery          |       |      |
|   |  +--------+  +-------------------+       |      |
|   +-------------------------------------------+      |
|                                                      |
|______________________________________________________|

    Cables:
    Panel Solar --> MPPT --> Bateria --> Pi 5
                                   --> FT-891
    T-Beam (USB) --> Pi 5
    FT-891 (USB CAT) --> Pi 5
    Diamond X50 (coaxial) --> T-Beam
    Hamstick (coaxial) --> FT-891
    Starlink (ethernet) --> Pi 5
```

```
    MOCHILA / BACKPACK (nodo portatil / portable node)
    ___________________________
   |                           |
   |  +-----+    +---------+  |
   |  | Pi 5 |   | T-Beam  |  |
   |  | 4GB  |   | + GPS   |  |
   |  +-----+    +---------+  |
   |  +---------------------+  |
   |  | LiFePO4 20Ah        |  |
   |  | (peso: ~3 kg)       |  |
   |  +---------------------+  |
   |  [Panel solar plegable    |
   |   100W en exterior]       |
   |___________________________|
   Peso total: ~5 kg
   Autonomia: 85h sin sol
```

---

## 9. Mantenimiento / Maintenance

### Diario / Daily
- Verificar carga de bateria
- Ejecutar `ierahkwa_check.sh` (automatico via cron)

### Semanal / Weekly
- Actualizar firmware Meshtastic si hay version nueva
- Revisar logs: `journalctl -u ierahkwa-node --since "7 days ago"`
- Verificar peers IPFS

### Mensual / Monthly
- Actualizar Docker images: `docker compose pull && docker compose up -d`
- Backup de claves (cifrado, almacenado en IPFS + LoRa transfer)
- Limpiar polvo de Pelican case y antenas
- Verificar conexiones de cable coaxial

### Anual / Yearly
- Reemplazar pasta termica del Pi/NUC
- Inspeccionar bateria LiFePO4 (voltaje por celda)
- Verificar integridad del panel solar
- Actualizar licencia de radioaficionado si aplica

---

## 10. Preguntas Frecuentes / FAQ

**Q: Necesito licencia de radioaficionado?**
A: Solo para el radio HF (JS8Call). LoRa opera en bandas ISM (sin licencia).
El nodo basico y estandar NO requieren licencia.

**Q: Funciona fuera de las Americas?**
A: LoRa en 915 MHz es legal en las Americas. Para otros continentes, usar
868 MHz (Europa/Africa) o 433 MHz (Asia). JS8Call funciona globalmente.

**Q: Puedo usar una laptop en vez del Pi?**
A: Si, pero consume mas energia. Una laptop antigua con Linux puede funcionar,
pero la autonomia sera menor. Recomendamos Pi 5 o NUC para eficiencia.

**Q: Que pasa si me confiscan el nodo?**
A: Si ejecutaste `tactical-wipe.sh`, no hay datos recuperables. El disco esta
cifrado con LUKS y las claves fueron borradas. La red sigue funcionando
porque es distribuida — tu nodo era solo uno de muchos.

**Q: Como conecto dos nodos moviles entre si?**
A: Automaticamente via LoRa mesh (Meshtastic). Si estan fuera de rango LoRa,
via JS8Call HF. Si hay internet, via Matrix. La red es auto-sanante.

---

*Ierahkwa Ne Kanienke — Soberania digital en movimiento*
*Digital sovereignty in motion*
