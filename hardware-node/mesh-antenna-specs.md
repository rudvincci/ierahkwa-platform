# Ierahkwa Mesh Network -- Antenna & Hardware Specifications

Technical reference for deploying LoRa mesh nodes across sovereign territories. This document covers recommended hardware, antenna specifications, frequency bands, power consumption, connection diagrams, and a bill of materials.

---

## 1. Recommended Hardware

### 1.1 LilyGo T-Beam v1.2 (Primary Recommendation)

| Specification      | Value                                    |
|--------------------|------------------------------------------|
| MCU                | ESP32-D0WDQ6 (dual-core, 240 MHz)       |
| LoRa Chip          | SX1262 (or SX1276 for older revisions)  |
| GPS                | NEO-6M (integrated)                      |
| Battery            | 18650 Li-Ion holder (built-in charger)   |
| WiFi               | 802.11 b/g/n                             |
| Bluetooth          | BLE 4.2                                  |
| Flash / PSRAM      | 4 MB / 8 MB                              |
| USB                | Type-C (CP2104 USB-UART bridge)          |
| Antenna Connector  | U.FL (IPEX) for LoRa; SMA for GPS       |
| Dimensions         | 100 x 36 x 16 mm                        |
| Weight             | 52 g (without battery)                   |
| Price Range        | $28 - $38 USD                            |

Best for: Mobile nodes, vehicle-mounted relay stations, field deployments with GPS tracking.

### 1.2 Heltec LoRa 32 V3

| Specification      | Value                                    |
|--------------------|------------------------------------------|
| MCU                | ESP32-S3FN8 (dual-core, 240 MHz)        |
| LoRa Chip          | SX1262                                   |
| Display            | 0.96" OLED (128x64)                      |
| Battery            | JST-PH 2.0 connector (3.7V Li-Po)       |
| WiFi               | 802.11 b/g/n                             |
| Bluetooth          | BLE 5.0                                  |
| Flash / PSRAM      | 8 MB / 8 MB                              |
| USB                | Type-C (CH9102 USB-UART bridge)          |
| Antenna Connector  | U.FL (IPEX) for LoRa                     |
| Dimensions         | 50 x 25 x 11 mm                         |
| Weight             | 18 g                                     |
| Price Range        | $18 - $25 USD                            |

Best for: Indoor nodes, community center installations, low-cost mesh expansion.

### 1.3 RAK WisBlock (Modular System)

| Specification      | Value                                    |
|--------------------|------------------------------------------|
| Base Board         | RAK19007 (WisBlock Base)                 |
| Core Module        | RAK4631 (nRF52840 + SX1262)             |
| Sensor Slots       | 4x I2C + 2x IO + 1x UART               |
| Battery            | JST-PH 2.0 (3.7V Li-Po)                |
| Solar Input        | 5V via connector (MPPT built-in)        |
| BLE                | 5.0 (nRF52840 native)                   |
| USB                | Type-C                                   |
| Antenna Connector  | U.FL (IPEX) for LoRa; U.FL for BLE      |
| Dimensions         | 60 x 30 x 13 mm (base board)            |
| Price Range        | $35 - $55 USD (base + core)              |

Best for: Environmental monitoring stations, solar-powered remote nodes, modular sensor deployments.

---

## 2. Antenna Specifications

### 2.1 Urban Deployment (1-5 km range)

| Parameter          | Specification                            |
|--------------------|------------------------------------------|
| Type               | Quarter-wave whip (spring or rigid)      |
| Gain               | 2.0 - 3.0 dBi                           |
| Polarization       | Vertical omnidirectional                 |
| Height             | 80 - 170 mm                              |
| Connector          | SMA Male (with U.FL to SMA pigtail)     |
| VSWR               | < 1.5:1                                  |
| Impedance          | 50 Ohm                                   |
| Mounting           | Magnetic base or direct-mount            |
| Estimated Range    | 1 - 5 km (line-of-sight in urban)        |
| Recommended Model  | Taoglas FXP73 or generic 915 MHz whip   |

### 2.2 Rural / Long-Range Deployment (5-25 km range)

| Parameter          | Specification                            |
|--------------------|------------------------------------------|
| Type               | Fiberglass collinear omnidirectional     |
| Gain               | 5.0 - 8.0 dBi                           |
| Polarization       | Vertical omnidirectional                 |
| Height             | 400 - 900 mm                             |
| Connector          | N-Type Female (with N-to-SMA adapter)   |
| VSWR               | < 1.3:1                                  |
| Impedance          | 50 Ohm                                   |
| Cable              | LMR-400 coaxial (low loss), max 10 m    |
| Mounting           | Pole mount (minimum 6 m above ground)   |
| Estimated Range    | 5 - 25 km (open terrain, elevated)       |
| Recommended Model  | RAK 5.8 dBi fiberglass 915 MHz          |

### 2.3 Point-to-Point Backbone (25-80 km range)

| Parameter          | Specification                            |
|--------------------|------------------------------------------|
| Type               | Yagi directional                         |
| Gain               | 10.0 - 14.0 dBi                         |
| Polarization       | Horizontal or vertical (match both ends) |
| Elements           | 5 - 10                                   |
| Beamwidth          | 30 - 60 degrees                          |
| Connector          | N-Type Female                            |
| VSWR               | < 1.4:1                                  |
| Impedance          | 50 Ohm                                   |
| Cable              | LMR-400 or LMR-600, max 15 m            |
| Mounting           | Tower or rooftop with azimuth adjustment |
| Estimated Range    | 25 - 80 km (line-of-sight, elevated)     |
| Recommended Model  | Arrow Antenna 915 MHz Yagi               |

---

## 3. Frequency Bands by Region (Americas)

| Region / Country           | ISM Band     | Frequency Range    | Max EIRP        | Duty Cycle  | Meshtastic Region Code |
|----------------------------|--------------|--------------------|------------------|-------------|------------------------|
| United States              | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | US                     |
| Canada                     | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | US                     |
| Mexico                     | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | US                     |
| Brazil                     | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | BR                     |
| Argentina                  | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | AR                     |
| Colombia                   | ISM 915      | 902 - 928 MHz      | 27 dBm (500 mW) | No limit    | US                     |
| Chile                      | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | US                     |
| Peru                       | ISM 915      | 902 - 928 MHz      | 30 dBm (1 W)    | No limit    | US                     |
| Central America (general)  | ISM 915      | 902 - 928 MHz      | 27 dBm (500 mW) | No limit    | US                     |
| Caribbean (general)        | ISM 915      | 902 - 928 MHz      | 27 dBm (500 mW) | Varies      | US                     |

Important notes:
- All Americas regions use the 915 MHz ISM band (no 868 MHz EU devices).
- Always verify local regulations before deployment in indigenous territories that may span national borders.
- The 902-928 MHz band is shared with other ISM devices; interference is possible in dense urban areas.

---

## 4. Power Consumption

### 4.1 Per-Device Power Profile

| State               | LilyGo T-Beam | Heltec LoRa 32 | RAK WisBlock |
|----------------------|---------------|-----------------|--------------|
| Deep Sleep           | 10 uA         | 20 uA           | 2.5 uA       |
| Idle (listening)     | 35 mA         | 30 mA           | 12 mA        |
| WiFi Active          | 180 mA        | 160 mA          | N/A          |
| BLE Active           | 80 mA         | 70 mA           | 15 mA        |
| LoRa TX (20 dBm)    | 120 mA        | 110 mA          | 85 mA        |
| LoRa TX (30 dBm)    | 350 mA        | N/A             | N/A          |
| GPS Acquisition      | 45 mA         | N/A             | 25 mA (ext)  |
| Peak (TX + GPS)      | 400 mA        | 180 mA          | 110 mA       |

### 4.2 Battery Life Estimates

| Configuration                  | Battery       | Estimated Runtime   |
|--------------------------------|---------------|---------------------|
| T-Beam, TX every 15 min       | 3400 mAh 18650| 5 - 7 days          |
| Heltec, TX every 15 min       | 1000 mAh LiPo | 2 - 3 days          |
| RAK WisBlock, TX every 30 min | 3000 mAh LiPo | 14 - 21 days        |
| RAK + 6W Solar Panel          | 3000 mAh LiPo | Indefinite (sunny)  |

### 4.3 Solar Panel Recommendations

| Panel Rating | Dimensions      | Use Case                            |
|-------------|-----------------|--------------------------------------|
| 1W (5V)     | 110 x 70 mm    | Extend battery, not self-sustaining  |
| 3W (5V)     | 150 x 130 mm   | Marginal self-sustaining in tropics  |
| 6W (5V)     | 260 x 170 mm   | Self-sustaining (recommended min.)   |
| 10W (5V)    | 340 x 230 mm   | Self-sustaining with margin          |

---

## 5. Connection Diagram

```
                    IERAHKWA MESH NODE -- CONNECTION DIAGRAM
                    ========================================

    [Solar Panel 6W]
         |
         | 5V USB-C / JST
         |
    +----+----+          U.FL Pigtail       +------------------+
    |         |---------- coax ------------>| LoRa Antenna      |
    |  LoRa   |                             | (915 MHz)         |
    |  Module  |                             | 5.8 dBi Omni     |
    | (T-Beam  |                             +------------------+
    |  Heltec  |
    |  or RAK) |          USB-C             +------------------+
    |         |---------- serial ---------->| Raspberry Pi 5    |
    |         |          /dev/ttyUSB0       | (Gateway Node)    |
    +----+----+                             |                   |
         |                                  | - Docker          |
         | 18650 /                          | - Node.js 22      |
         | LiPo Battery                     | - PM2             |
         |                                  | - Meshtastic CLI  |
    [3400 mAh]                              +----+---------+----+
                                                 |         |
                                            Ethernet    WiFi/4G
                                                 |         |
                                            +----+---------+----+
                                            |                   |
                                            |   Internet /      |
                                            |   MameyNode P2P   |
                                            |   (Port 30303)    |
                                            |                   |
                                            +-------------------+

    LoRa Mesh Topology:
    ====================

        [Node A]---<915 MHz>---[Node B]---<915 MHz>---[Node C]
            \                      |                      /
             \                     |                     /
              <915 MHz>       <915 MHz>          <915 MHz>
                \                  |                 /
                 \                 |                /
              [Node D]---<915 MHz>---[GATEWAY]---<internet>---[MameyNode]
                                       |
                                  [Raspberry Pi]
                                       |
                                  [IPFS / PostgreSQL]


    Antenna Mounting (Rural):
    =========================

                        === Antenna (6m+ AGL) ===
                              |
                              | LMR-400 Coax (max 10m)
                              |
                         N-to-SMA Adapter
                              |
                         U.FL Pigtail
                              |
                     +--------+--------+
                     |   LoRa Module   |
                     | (inside shelter)|
                     +--------+--------+
                              |
                         USB Serial
                              |
                     +--------+--------+
                     | Raspberry Pi 5  |
                     | + UPS / Battery |
                     +-----------------+
```

---

## 6. Bill of Materials

### 6.1 Basic Node (Urban, battery-powered)

| Item                              | Qty | Unit Price (USD) | Total (USD) |
|-----------------------------------|-----|------------------|-------------|
| Heltec LoRa 32 V3                | 1   | $22.00           | $22.00      |
| 915 MHz Whip Antenna (SMA)       | 1   | $5.00            | $5.00       |
| U.FL to SMA Pigtail Cable        | 1   | $3.00            | $3.00       |
| 1000 mAh LiPo Battery (3.7V)    | 1   | $8.00            | $8.00       |
| USB-C Cable (1m)                 | 1   | $3.00            | $3.00       |
| Waterproof Enclosure (IP65)      | 1   | $12.00           | $12.00      |
| **Subtotal**                      |     |                  | **$53.00**  |

### 6.2 Gateway Node (Rural, solar-powered, with Raspberry Pi)

| Item                              | Qty | Unit Price (USD) | Total (USD) |
|-----------------------------------|-----|------------------|-------------|
| Raspberry Pi 5 (8GB)             | 1   | $80.00           | $80.00      |
| Pi 5 Official Power Supply       | 1   | $12.00           | $12.00      |
| MicroSD Card 64GB (A2)           | 1   | $12.00           | $12.00      |
| LilyGo T-Beam v1.2 (SX1262)     | 1   | $34.00           | $34.00      |
| 18650 Battery (3400 mAh)         | 1   | $8.00            | $8.00       |
| RAK 5.8 dBi Fiberglass Antenna   | 1   | $25.00           | $25.00      |
| N-Type to SMA Adapter            | 1   | $4.00            | $4.00       |
| LMR-400 Coax Cable (5m)          | 1   | $18.00           | $18.00      |
| U.FL to SMA Pigtail Cable        | 1   | $3.00            | $3.00       |
| 6W Solar Panel (5V USB)          | 1   | $15.00           | $15.00      |
| UPS HAT for Raspberry Pi         | 1   | $25.00           | $25.00      |
| 18650 Batteries for UPS (x2)     | 2   | $8.00            | $16.00      |
| Antenna Mounting Pole (3m)       | 1   | $20.00           | $20.00      |
| Waterproof Enclosure (IP67)      | 1   | $18.00           | $18.00      |
| Ethernet Cable (Cat6, 10m)       | 1   | $8.00            | $8.00       |
| Misc (connectors, ties, tape)    | 1   | $10.00           | $10.00      |
| **Subtotal**                      |     |                  | **$308.00** |

### 6.3 Backbone Relay (Point-to-Point, long range)

| Item                              | Qty | Unit Price (USD) | Total (USD) |
|-----------------------------------|-----|------------------|-------------|
| LilyGo T-Beam v1.2 (SX1262)     | 2   | $34.00           | $68.00      |
| 915 MHz Yagi Antenna (10 dBi)    | 2   | $45.00           | $90.00      |
| LMR-400 Coax Cable (10m)         | 2   | $30.00           | $60.00      |
| N-Type to SMA Adapter            | 2   | $4.00            | $8.00       |
| U.FL to SMA Pigtail              | 2   | $3.00            | $6.00       |
| 10W Solar Panel (5V)             | 2   | $22.00           | $44.00      |
| Charge Controller + Battery Box  | 2   | $35.00           | $70.00      |
| 18650 Batteries (x4 per site)    | 8   | $8.00            | $64.00      |
| Antenna Tower/Mast (6m)          | 2   | $60.00           | $120.00     |
| Weatherproof Enclosure (IP67)    | 2   | $18.00           | $36.00      |
| Lightning Arrester (N-Type)      | 2   | $15.00           | $30.00      |
| Grounding Kit                    | 2   | $12.00           | $24.00      |
| Misc (hardware, cable, clamps)   | 1   | $30.00           | $30.00      |
| **Subtotal (pair)**               |     |                  | **$650.00** |

---

## 7. Deployment Checklist

1. Verify local frequency regulations for the deployment territory.
2. Select node type based on use case (urban basic, rural gateway, backbone relay).
3. Procure hardware from the appropriate BOM section above.
4. Flash Meshtastic firmware onto LoRa devices.
5. Run `raspberry-pi-setup.sh` on gateway Raspberry Pi units.
6. Run `configure-meshtastic.sh` after connecting LoRa device via USB.
7. Mount antennas at maximum practical height (minimum 6 meters for rural).
8. Use LMR-400 coax to minimize signal loss between antenna and device.
9. Install lightning arresters on all elevated outdoor antennas.
10. Verify mesh connectivity by checking node list in Meshtastic app.
11. Monitor node health via the `health-check.sh` cron job.

---

Ierahkwa Ne Kanienke -- Sovereign Digital Nation
