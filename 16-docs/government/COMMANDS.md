# ══════════════════════════════════════════════════════════════════════════════
# COMANDOS DE SERVICIOS - IERAHKWA SOVEREIGN PLATFORM
# ══════════════════════════════════════════════════════════════════════════════

```
╔══════════════════════════════════════════════════════════════════════════════╗
║                                                                              ║
║                    IERAHKWA SOVEREIGN PLATFORM                               ║
║                    COMANDOS DE INICIO DE SERVICIOS                           ║
║                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════╝
```

---

## INICIO RÁPIDO

### Todos los Servicios (Unix/Mac)
```bash
./start-all-services.sh
```

### Todos los Servicios (Windows PowerShell)
```powershell
.\start-services.ps1
```

---

## SERVICIOS .NET 10

### TradeX Exchange (Puerto 5054)
```bash
cd TradeX/TradeX.API
dotnet run --urls "http://localhost:5054"
```
**Swagger:** http://localhost:5054/swagger
**Health:** http://localhost:5054/health

---

### NET10 DeFi (Puerto 5071)
```bash
cd NET10/NET10.API
dotnet run --urls "http://localhost:5071"
```

---

### FarmFactory (Puerto 5061)
```bash
cd FarmFactory/FarmFactory.API
dotnet run --urls "http://localhost:5061"
```

---

### SpikeOffice HR (Puerto 5056)
```bash
cd SpikeOffice/SpikeOffice.API
dotnet run --urls "http://localhost:5056"
```

---

### RnBCal Calendar (Puerto 5055)
```bash
cd RnBCal/RnBCal.API
dotnet run --urls "http://localhost:5055"
```

---

### AppBuilder (Puerto 5060)
```bash
cd AppBuilder/AppBuilder.API
dotnet run --urls "http://localhost:5060"
```

---

### IDOFactory (Puerto 5097)
```bash
cd IDOFactory/IDOFactory.API
dotnet run --urls "http://localhost:5097"
```

---

### ProjectHub (Puerto 7070)
```bash
cd ProjectHub/ProjectHub.API
dotnet run --urls "http://localhost:7070"
```

---

### MeetingHub (Puerto 7071)
```bash
cd MeetingHub/MeetingHub.API
dotnet run --urls "http://localhost:7071"
```

---

### Advocate Office (Puerto 3010)
```bash
cd AdvocateOffice/AdvocateOffice.API
dotnet run --urls "http://localhost:3010"
```

---

## SERVICIOS NODE.JS

### SmartSchool (Puerto 3000)
```bash
cd smart-school-node
npm install  # Primera vez
npm start
```

---

### Ierahkwa Shop (Puerto 3001)
```bash
cd ierahkwa-shop
npm install  # Primera vez
PORT=3001 npm start
```

---

### POS System (Puerto 3002)
```bash
cd pos-system
npm install  # Primera vez
PORT=3002 npm start
```

---

### Inventory System (Puerto 3003)
```bash
cd inventory-system
npm install  # Primera vez
PORT=3003 npm start
```

---

### Blockchain Node (Puerto 3004)
```bash
cd node
npm install  # Primera vez
PORT=3004 npm start
```

---

## APLICACIONES DESKTOP

### InventoryManager (Windows Forms)
```bash
cd InventoryManager
dotnet build
dotnet run
```

---

## PORTAL WEB (Estático)

Abrir directamente en el navegador:
```
platform/index.html
```

O servir con cualquier servidor HTTP:
```bash
cd platform
npx serve .
# o
python -m http.server 8080
```

---

## HEALTH DASHBOARD

Abrir en navegador:
```
platform/health-dashboard.html
```

---

## DOCKER (Opcional)

### Build y Run TradeX
```bash
cd TradeX
docker build -t ierahkwa-tradex .
docker run -p 5054:5054 ierahkwa-tradex
```

### Docker Compose (todos los servicios)
```bash
docker-compose up -d
```

---

## URLS DE ACCESO RÁPIDO

| Servicio | URL | Swagger |
|----------|-----|---------|
| TradeX | http://localhost:5054 | http://localhost:5054/swagger |
| NET10 DeFi | http://localhost:5071 | http://localhost:5071/swagger |
| FarmFactory | http://localhost:5061 | http://localhost:5061/swagger |
| SpikeOffice | http://localhost:5056 | http://localhost:5056/swagger |
| RnBCal | http://localhost:5055 | http://localhost:5055/swagger |
| AppBuilder | http://localhost:5060 | http://localhost:5060/swagger |
| IDOFactory | http://localhost:5097 | http://localhost:5097/swagger |
| ProjectHub | http://localhost:7070 | http://localhost:7070/swagger |
| MeetingHub | http://localhost:7071 | http://localhost:7071/swagger |
| Advocate | http://localhost:3010 | http://localhost:3010/swagger |
| SmartSchool | http://localhost:3000 | - |
| Shop | http://localhost:3001 | - |
| POS | http://localhost:3002 | - |
| Inventory | http://localhost:3003 | - |
| Node | http://localhost:3004 | - |

---

## REQUISITOS

### .NET
- .NET SDK 8.0 o 10.0
- Verificar: `dotnet --version`

### Node.js
- Node.js 18+ LTS (recomendado 20 LTS)
- Verificar: `node --version` y `npm --version`

### Instalación Rápida

**macOS:**
```bash
brew install dotnet node
```

**Ubuntu/Debian:**
```bash
# .NET
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Node.js
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs
```

**Windows:**
- Descargar .NET SDK de https://dotnet.microsoft.com
- Descargar Node.js de https://nodejs.org

---

```
══════════════════════════════════════════════════════════════════════════════
                    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
                          OFFICE OF THE PRIME MINISTER
══════════════════════════════════════════════════════════════════════════════
```
