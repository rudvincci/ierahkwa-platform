# Planos visuales – IERAHKWA Live Production
## Todo en dibujos para no perderse

**Para quien prefiere planos y dibujos.**

---

## 0. TU SETUP: MAC + SERVIDORES + INTERNET EN CISCO

```
  INTERNET  ──────────────────────────────────────────►  CISCO  (aquí está el internet)
                                                              │
                                                              │ cable
                                                              ▼
  ┌─────────────────────────────────────────────────────────────────────────┐
  │  TU MAC  ◄────────── cables ──────────►  SERVIDORES (racks)              │
  │                                                                          │
  │  • Los servidores están CONECTADOS a tu Mac (red, USB, etc.)             │
  │  • El INTERNET entra por los CISCO (modem → Cisco → resto)                │
  │  • ATABEY + todo el sistema corren en Node (en tu Mac o en un servidor)   │
  └─────────────────────────────────────────────────────────────────────────┘
```

**Resumen:** Servidores conectados al Mac. Internet en los Cisco.

---

## 1. FLUJO INTERNET → TU PLATAFORMA

```
    INTERNET
        │
        ▼
   ┌─────────┐
   │  MODEM  │  192.168.0.1  (entrada de internet)
   │   ISP   │
   └────┬────┘
        │ cable
        ▼
   ┌─────────┐
   │  CISCO  │  10.0.0.1  (router principal)
   │ Router  │
   └────┬────┘
        │
        ▼
   ┌─────────┐
   │FORTINET │  (firewall – opcional)
   │Firewall │
   └────┬────┘
        │
        ▼
   ┌─────────┐
   │ SWITCH  │  (reparte cables a los servidores)
   │  Rack   │
   └────┬────┘
        │
        ├──────────┬──────────┐
        ▼          ▼          ▼
   ┌─────────┐ ┌─────────┐ ┌─────────┐
   │ SRV01   │ │ SRV02   │ │  ...    │
   │ Node    │ │ DB      │ │         │
   │ :8545   │ │         │ │         │
   │ :3001   │ │         │ │         │
   └─────────┘ └─────────┘ └─────────┘
      ▲
      │  Aquí corre tu plataforma
      │  (server.js + banking-bridge.js)
```

---

## 2. QUÉ CABLES VAN A QUÉ (vista simple)

```
  [MODEM]  ─── cable WAN ───►  [CISCO] puerto WAN

  [CISCO]  ─── cable LAN ───►  [SWITCH del rack]

  [SWITCH] ─── cables ───►  [Servidor 1] [Servidor 2] [Monitor] [UPS]
```

---

## 3. UN RACK (el que tenías en el plano – de arriba a abajo)

```
  ┌─────────────────────────────────────┐
  │  FORTINET (firewall)                │  ← Arriba
  ├─────────────────────────────────────┤
  │  CISCO (router/switch)               │
  ├─────────────────────────────────────┤
  │  Monitor consola (pantalla + teclado)│
  ├─────────────────────────────────────┤
  │  Switch (muchos puertos RJ45)        │
  ├─────────────────────────────────────┤
  │  Enclosures discos (almacenamiento)  │
  ├─────────────────────────────────────┤
  │  Servidores (ProLiant / HP G4)       │  ← Aquí corre Node/Bridge
  │  [■] [■] [■] [■] [■] [■] ...         │
  ├─────────────────────────────────────┤
  │  (estante vacío – cableado)          │
  ├─────────────────────────────────────┤
  │  UPS FUZE (alimentación)             │  ← Abajo
  └─────────────────────────────────────┘
```

---

## 4. LOS 5 RACKS (vista desde frente)

```
  RACK 1      RACK 2      RACK 3      RACK 4      RACK 5
  (consola)   (red)       (centro)    (servidores)(servidores)

  ┌─────┐    ┌─────┐    ┌─────┐    ┌─────┐    ┌─────┐
  │ M   │    │ SW  │    │ M   │    │ M   │    │ M   │
  │ K   │    │ SW  │    │ L   │    │ SW  │    │ SW  │
  │     │    │ SW  │    │ SW  │    │ S   │    │ S   │
  │ UPS │    │     │    │ S   │    │ S   │    │ S   │
  └─────┘    └─────┘    │ S   │    │ S   │    │ S   │
                        │ UPS │    │ UPS │    │ UPS │
                        └─────┘    └─────┘    └─────┘

  M = Monitor   K = Teclado   SW = Switches   S = Servidores
```

---

## 5. QUÉ PUERTO VA A QUÉ (para port forwarding)

```
  INTERNET (gente fuera)          TU SERVIDOR (dentro)

       :80  ──────────────────►  10.0.10.1:80   (web HTTP)
      :443  ──────────────────►  10.0.10.1:443  (web HTTPS)
     :8545  ──────────────────►  10.0.10.1:8545 (Node – API, dashboard)
     :3001  ──────────────────►  10.0.10.1:3001 (Banking Bridge)

  En el modem o Cisco: "cuando llegue tráfico al puerto 8545, mandarlo a 10.0.10.1:8545"
```

---

## 6. DÓNDE ESTÁ CADA PROGRAMA (en el servidor)

```
  SERVIDOR (ej. 10.0.10.1 o tu Mac)

  ┌─────────────────────────────────────────────────────────┐
  │  server.js          →  puerto 8545  (Node principal)    │
  │  banking-bridge.js  →  puerto 3001  (Banco, chat, API)  │
  └─────────────────────────────────────────────────────────┘

  Carpeta donde está todo:
  RuddieSolution/node/
       ├── server.js
       ├── banking-bridge.js
       ├── ai/           (agentes AI)
       └── data/         (datos)
```

---

## 7. PASOS EN ORDEN (con números en el dibujo)

```
  PASO 1     Encender MODEM  ──►  ¿Hay internet en un PC? ✓

  PASO 2     Cable MODEM → CISCO (puerto WAN del Cisco)

  PASO 3     Cable CISCO → SWITCH del rack (puerto LAN)

  PASO 4     Cables SWITCH → Servidores (donde corre Node)

  PASO 5     En modem/Cisco: Port forwarding 80, 443, 8545, 3001 → IP del servidor

  PASO 6     En el servidor: Node 18+ y después  ./start.sh  (o node server.js + node banking-bridge.js)

  PASO 7     Probar: http://localhost:8545/health  y  http://localhost:3001/api/bankers
```

---

## 8. RESUMEN EN UNA IMAGEN (todo junto)

```
         INTERNET
            │
            ▼
        [ MODEM ]
            │
            ▼
        [ CISCO ]  ◄── Port forwarding: 80, 443, 8545, 3001
            │
            ▼
     [ FORTINET ]  (opcional)
            │
            ▼
        [ SWITCH ]
            │
            ▼
    [ SERVIDOR ]  ←  Node :8545  +  Bridge :3001
         │
         └── Carpeta: RuddieSolution/node/
                     server.js  +  banking-bridge.js
```

---

---

## 9. ATABEY Y TODO EL SISTEMA (dónde está cada cosa)

```
  TU MAC
     │
     ├── Cursor (donde trabajas) + proyecto (código, docs)
     │
     └── Si corres Node en tu Mac:
             │
             ▼
  ┌─────────────────────────────────────────────────────────────────┐
  │  server.js (puerto 8545)                                         │
  │  ─────────────────────────────────────────────────────────────  │
  │  • AI Hub + ATABEY     →  /api/ai-hub   /ai-hub   /atabey        │
  │  • AI Orchestrator     →  /api/v1/ai/orchestrator                │
  │  • AI Banker           →  /api/v1/ai/banker                      │
  │  • AI Code Generator   →  /api/ai/code, /api/ai/chat             │
  │  • Todo el sistema     →  health, dashboards, APIs               │
  └───────────────────────────────┬─────────────────────────────────┘
                                  │  proxy
                                  ▼
  ┌─────────────────────────────────────────────────────────────────┐
  │  banking-bridge.js (puerto 3001)  →  banco, chat, bankers       │
  └─────────────────────────────────────────────────────────────────┘
```

**ATABEY:** Es la controladora maestra de todos los AI (AI Hub). Se abre en: **http://localhost:8545/atabey** (y el AI Hub en **http://localhost:8545/ai-hub**). Todo el “cert”/sistema (ATABEY, AI Hub, AI Banker, orquestador, etc.) está dentro de server.js (8545) cuando lo corres en tu Mac o en un servidor.

---

Si quieres, el siguiente paso puede ser un solo plano grande (una sola página) con todo esto junto, o planos separados por tema (solo internet, solo racks, solo servidor).
