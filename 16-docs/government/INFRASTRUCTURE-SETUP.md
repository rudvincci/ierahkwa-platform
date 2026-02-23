# 🏛️ IERAHKWA SOVEREIGN DATA CENTER
## Infrastructure Setup & Configuration Guide
### Fecha: 28 de Enero, 2026

---

## 📊 INVENTARIO COMPLETO DE HARDWARE

### 🌐 NETWORKING

| # | Equipo | Modelo | IP Sugerida | Función |
|---|--------|--------|-------------|---------|
| 1 | ISP Modem | Sagemcom F@ST 3890 V3 | 192.168.0.1 | WAN Entry |
| 2 | Main Router | Cisco ISR4331 | 10.0.0.1 | Core Router |
| 3 | VPN Router | Cisco C881-V | 10.0.0.2 | VPN Gateway |
| 4 | Branch Router 1 | Cisco 861 | 10.0.1.1 | VLAN Servers |
| 5 | Branch Router 2 | Cisco 881 | 10.0.2.1 | VLAN Mining |
| 6 | Firewall | FortiGate 90D | 10.0.0.254 | Security |
| 7 | WiFi AP | TP-Link AX53 | 10.0.0.10 | Wireless |

### 🔌 SWITCHES (TP-Link 48-Port Managed)

| # | Ubicación | IP Sugerida | VLANs |
|---|-----------|-------------|-------|
| SW1 | Rack 1 - Top | 10.0.0.11 | 10,20,30 |
| SW2 | Rack 1 - Mid | 10.0.0.12 | 10,20,30 |
| SW3 | Rack 2 - Top | 10.0.0.13 | 40,50 |
| SW4 | Rack 2 - Mid | 10.0.0.14 | 40,50 |
| SW5 | Rack 3 - Top | 10.0.0.15 | 60,70 |
| SW6 | Rack 3 - Mid | 10.0.0.16 | 60,70 |
| SW7 | Rack 4 | 10.0.0.17 | 80 |
| SW8 | Rack 5 | 10.0.0.18 | 90 |

### 💻 SERVIDORES (HP Mini PCs)

| # | Función | IP | Specs Recomendados |
|---|---------|----|--------------------|
| SRV01 | Main Platform (Node.js) | 10.0.10.1 | 16GB RAM, SSD |
| SRV02 | Database (MongoDB) | 10.0.10.2 | 32GB RAM, SSD |
| SRV03 | Redis Cache | 10.0.10.3 | 16GB RAM |
| SRV04 | Rust Service (SWIFT) | 10.0.10.4 | 8GB RAM |
| SRV05 | Go Service (Queue) | 10.0.10.5 | 8GB RAM |
| SRV06 | Python ML Service | 10.0.10.6 | 16GB RAM, GPU |
| SRV07 | Backup/Redundancy | 10.0.10.7 | 16GB RAM |
| SRV08-85 | Mining Nodes | 10.0.20.1-77 | 8GB RAM each |
| SRV86-1000 | Future Mining | 10.0.20.78+ | Expansion |

### ⚡ POWER (UPS Forza)

| # | Capacidad | Protege |
|---|-----------|---------|
| UPS1 | 1500VA | Rack 1 (Routers) |
| UPS2 | 1500VA | Rack 2 (Switches) |
| UPS3 | 1500VA | Rack 3 (Servers) |
| UPS4 | 3000VA | Rack 4-5 (Mining) |

---

## 🔧 CONFIGURACIÓN DE RED

### VLAN Structure

```
VLAN 10: Management (10.0.0.0/24)
VLAN 20: Servers (10.0.10.0/24)
VLAN 30: Services (10.0.11.0/24)
VLAN 40: Database (10.0.12.0/24)
VLAN 50: Mining Pool 1 (10.0.20.0/24)
VLAN 60: Mining Pool 2 (10.0.21.0/24)
VLAN 70: Mining Pool 3 (10.0.22.0/24)
VLAN 80: DMZ/Public (10.0.100.0/24)
VLAN 90: Guest/IoT (10.0.200.0/24)
```

### ISP Modem (Sagemcom F@ST 3890)

```
URL: http://192.168.0.1
Usuario: admin
Password: 4Z3pT4nMCX

WiFi SSID: LIB-4833603
WiFi Pass: hXqsiWeos2gm

Configuración:
1. Modo Bridge (si es posible) → Cisco ISR4331
2. O Port Forwarding:
   - 8545 → 10.0.10.1 (Main Platform)
   - 8546 → 10.0.10.1 (Blockchain RPC)
   - 443  → 10.0.10.1 (HTTPS)
   - 80   → 10.0.10.1 (HTTP)
```

### Cisco ISR4331 Basic Config

```cisco
enable
configure terminal

! Hostname
hostname IERAHKWA-CORE

! WAN Interface (to ISP)
interface GigabitEthernet0/0/0
 ip address dhcp
 ip nat outside
 no shutdown

! LAN Interface (to switches)
interface GigabitEthernet0/0/1
 ip address 10.0.0.1 255.255.255.0
 ip nat inside
 no shutdown

! VLAN Interfaces
interface GigabitEthernet0/0/1.10
 encapsulation dot1Q 10
 ip address 10.0.0.1 255.255.255.0
 
interface GigabitEthernet0/0/1.20
 encapsulation dot1Q 20
 ip address 10.0.10.1 255.255.255.0

interface GigabitEthernet0/0/1.50
 encapsulation dot1Q 50
 ip address 10.0.20.1 255.255.255.0

! NAT
ip nat inside source list 1 interface GigabitEthernet0/0/0 overload
access-list 1 permit 10.0.0.0 0.255.255.255

! DHCP Pools
ip dhcp pool SERVERS
 network 10.0.10.0 255.255.255.0
 default-router 10.0.10.1
 dns-server 8.8.8.8 8.8.4.4

ip dhcp pool MINING
 network 10.0.20.0 255.255.255.0
 default-router 10.0.20.1
 dns-server 8.8.8.8 8.8.4.4

! Save
end
write memory
```

### FortiGate 90D Basic Config

```
config system interface
    edit "wan1"
        set ip 192.168.0.2 255.255.255.0
        set allowaccess ping https ssh
    next
    edit "internal"
        set ip 10.0.0.254 255.255.255.0
        set allowaccess ping https ssh http
    next
end

config firewall policy
    edit 1
        set name "LAN-to-WAN"
        set srcintf "internal"
        set dstintf "wan1"
        set srcaddr "all"
        set dstaddr "all"
        set action accept
        set schedule "always"
        set service "ALL"
        set nat enable
    next
end
```

---

## 🚀 DEPLOYMENT STEPS

### Step 1: Network Setup
```bash
# En cada servidor (Ubuntu/Debian):
sudo apt update && sudo apt upgrade -y
sudo apt install -y net-tools curl wget git nodejs npm docker.io
```

### Step 2: Deploy Platform to Main Server (SRV01)
```bash
# SSH a SRV01
ssh user@10.0.10.1

# Crear directorio
sudo mkdir -p /opt/ierahkwa
cd /opt/ierahkwa

# Copiar archivos desde tu Mac
# (En tu Mac):
scp -r ./node ./platform ./services user@10.0.10.1:/opt/ierahkwa/

# Instalar dependencias
cd /opt/ierahkwa/node
npm install --production

# Iniciar con PM2
npm install -g pm2
pm2 start server.js --name ierahkwa -i max
pm2 save
pm2 startup
```

### Step 3: Deploy MongoDB (SRV02)
```bash
# SSH a SRV02
ssh user@10.0.10.2

# Instalar MongoDB
wget -qO - https://www.mongodb.org/static/pgp/server-7.0.asc | sudo apt-key add -
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu jammy/mongodb-org/7.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list
sudo apt update
sudo apt install -y mongodb-org

# Configurar para red
sudo nano /etc/mongod.conf
# Cambiar bindIp: 0.0.0.0

sudo systemctl enable mongod
sudo systemctl start mongod
```

### Step 4: Deploy Redis (SRV03)
```bash
# SSH a SRV03
ssh user@10.0.10.3

sudo apt install -y redis-server
sudo sed -i 's/bind 127.0.0.1/bind 0.0.0.0/' /etc/redis/redis.conf
sudo systemctl enable redis-server
sudo systemctl restart redis-server
```

### Step 5: Mining Nodes Setup (SRV08-85)
```bash
# Script para configurar nodo de minería
# Ejecutar en cada servidor de mining

#!/bin/bash
NODE_ID=$1  # Pasar como argumento

sudo apt update && sudo apt upgrade -y
sudo apt install -y nodejs npm

# Clonar configuración de minería
mkdir -p /opt/mining
cd /opt/mining

# Crear archivo de configuración
cat > config.json << EOF
{
  "nodeId": "MINING-NODE-${NODE_ID}",
  "poolUrl": "http://10.0.10.1:8546",
  "walletAddress": "0x...",
  "threads": $(nproc),
  "gasPrice": "1000000000"
}
EOF

# Iniciar minero (ejemplo genérico)
pm2 start miner.js --name "miner-${NODE_ID}"
```

---

## 📊 MONITORING

### URLs de Acceso (después del deploy)

| Servicio | URL Interna | URL Externa |
|----------|-------------|-------------|
| Platform | http://10.0.10.1:8545 | https://ierahkwa.gov |
| Admin | http://10.0.10.1:8545/platform/admin.html | - |
| Bank | http://10.0.10.1:8545/platform/bdet-bank.html | - |
| Mining Status | http://10.0.10.1:8545/api/v1/mining/status | - |
| PM2 Monitor | pm2 monit | - |

### Health Check Script

```bash
#!/bin/bash
# health-check.sh

echo "=== IERAHKWA HEALTH CHECK ==="
echo ""

# Main Platform
curl -s http://10.0.10.1:8545/health && echo " ✓ Main Platform" || echo " ✗ Main Platform DOWN"

# MongoDB
mongo --eval "db.stats()" 10.0.10.2:27017 && echo " ✓ MongoDB" || echo " ✗ MongoDB DOWN"

# Redis
redis-cli -h 10.0.10.3 ping && echo " ✓ Redis" || echo " ✗ Redis DOWN"

# Mining Nodes
for i in {1..85}; do
  curl -s http://10.0.20.$i:8547/status > /dev/null && echo " ✓ Mining Node $i" || echo " ✗ Mining Node $i"
done

echo ""
echo "=== CHECK COMPLETE ==="
```

---

## ⚡ QUICK START

```bash
# 1. En tu Mac, crear backup
cd "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives"
./SERVER-BACKUP-SYNC.sh backup

# 2. Copiar a servidor principal
scp backups/ierahkwa_production_*.tar.gz user@10.0.10.1:/tmp/

# 3. En el servidor, extraer y ejecutar
ssh user@10.0.10.1
cd /tmp
tar -xzf ierahkwa_production_*.tar.gz
sudo mv node platform services /opt/ierahkwa/
cd /opt/ierahkwa/node
npm install --production
pm2 start server.js --name ierahkwa -i max

# 4. Verificar
curl http://localhost:8545/health
```

---

## 📈 SCALING PLAN

| Fase | Nodos Mining | Capacidad | Timeline |
|------|--------------|-----------|----------|
| **Actual** | 85 | Base | Ahora |
| **Fase 2** | 200 | +115 nodos | +2 weeks |
| **Fase 3** | 500 | +300 nodos | +1 month |
| **Fase 4** | 1000 | +500 nodos | +2 months |

---

**Status: READY FOR PRODUCTION DEPLOYMENT** 🚀
