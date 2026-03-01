# Ierahkwa Hardware Node

Physical infrastructure layer for the Ierahkwa sovereign mesh network. This directory contains everything needed to deploy LoRa mesh nodes and Raspberry Pi gateway servers across indigenous territories.

## Contents

| File                       | Description                                              |
|----------------------------|----------------------------------------------------------|
| `raspberry-pi-setup.sh`   | Automated setup script for Raspberry Pi 5 gateway nodes  |
| `mesh-antenna-specs.md`   | Hardware specs, antenna guide, frequency bands, and BOM  |

## Quick Start

### 1. Prepare a Raspberry Pi 5

Flash Raspberry Pi OS (64-bit, Bookworm) onto a microSD card, boot the Pi, and connect it to the network.

### 2. Run the Setup Script

```bash
chmod +x raspberry-pi-setup.sh
sudo ./raspberry-pi-setup.sh
```

This installs Docker, Node.js 22, PostgreSQL client, clones the Ierahkwa platform repository, generates environment secrets, configures the firewall, sets up a systemd service for auto-start, and prepares the LoRa/Meshtastic serial interface.

### 3. Connect a LoRa Device

Plug a supported LoRa board (LilyGo T-Beam, Heltec LoRa 32, or RAK WisBlock) into the Pi via USB, then run:

```bash
~/configure-meshtastic.sh
```

### 4. Start the Node

```bash
sudo systemctl start ierahkwa-node
```

### 5. Verify Health

```bash
~/health-check.sh
```

A cron job also runs this check every 5 minutes automatically.

## Hardware Requirements

- Raspberry Pi 5 (8 GB recommended)
- 64 GB microSD card (A2 speed class)
- LoRa module: LilyGo T-Beam v1.2, Heltec LoRa 32 V3, or RAK WisBlock
- 915 MHz antenna (see `mesh-antenna-specs.md` for options)
- Power supply or solar panel for off-grid deployments

See `mesh-antenna-specs.md` for complete hardware specifications, antenna selection guides, frequency band regulations for the Americas, power consumption data, connection diagrams, and a bill of materials with cost estimates.

## Network Ports

| Port  | Protocol | Purpose                  |
|-------|----------|--------------------------|
| 22    | TCP      | SSH                      |
| 1883  | TCP      | MQTT (Meshtastic)        |
| 3000  | TCP      | Ierahkwa Node API        |
| 4001  | TCP      | IPFS Swarm               |
| 8080  | TCP      | IPFS Gateway             |
| 30303 | TCP/UDP  | MameyNode Blockchain P2P |

---

Ierahkwa Ne Kanienke -- Sovereign Digital Nation
