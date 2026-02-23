# REPORTE COMPLETO – COSTOS Y INVERSIÓN EN HARDWARE

**Sovereign Government of Ierahkwa Ne Kanienke**  
**Fecha:** 19 Enero 2026  
**Alcance:** Todo el sistema (software, servicios, hardware de todas las plataformas) y cuándo invertir

---

## 1. RESUMEN EJECUTIVO – COSTO TOTAL

| Categoría | Una vez (USD) | Anual (USD) |
|-----------|----------------|-------------|
| **Desarrollo (software)** | 355 350 | — |
| **Certificaciones (opc., prorrateo)** | 22 500 – 65 000 | — |
| **Licencias** | — | 1 500 – 5 000 |
| **Infraestructura (cloud/VPS)** | — | 2 450 – 6 500 |
| **Servicios externos** | — | 7 000 – 74 900 |
| **Mantenimiento y soporte** | — | 59 300 |
| **Hardware (ver Sección 3–4)** | 28 500 – 118 000 | 2 000 – 8 000 |
| **TOTAL** | **406 350 – 538 350** | **71 250 – 153 700** |

**Inversión primer año (software ya hecho + año 1 recurrente + hardware Fase 1–2):**  
**~90 000 – 220 000 USD**

---

## 2. COSTO DETALLADO (SIN HARDWARE)

### 2.1 Una sola vez

| Partida | USD |
|---------|-----|
| Desarrollo (blockchain, Node, .NET, web, móvil, DevOps) | 355 350 |
| ISO 27001 / SOC 2 / Auditoría blockchain (opc., prorrateo 1 año) | 22 500 – 65 000 |
| **Subtotal una vez** | **377 850 – 420 350** |

### 2.2 Recurrente anual

| Partida | USD/año |
|---------|---------|
| Licencias (SQL Server, IDE si aplica) | 1 500 – 5 000 |
| Infra cloud (VPS, BD, DNS, SSL, WAF) | 2 450 – 6 500 |
| Servicios externos (oracles, email, SWIFT, exchanges) | 7 000 – 74 900 |
| Mantenimiento 15% + soporte L2 | 59 300 |
| **Subtotal recurrente** | **70 250 – 145 700** |

---

## 3. HARDWARE POR PLATAFORMA

### 3.1 Blockchain y CryptoHost

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Nodo principal (Mamey)** | Servidor | 4–8 vCPU, 16–32 GB RAM, 500 GB SSD, 1 Gbps | 1 | 1 500 – 4 000 | Fase 1 (arranque) |
| **Nodo Alpha (NA)** | VPS/servidor | 2–4 vCPU, 8 GB RAM, 200 GB SSD | 1 | 600 – 1 500 | Fase 2 (6–12 m) |
| **Nodo Beta (EU)** | VPS/servidor | 2–4 vCPU, 8 GB RAM, 200 GB SSD | 1 | 600 – 1 500 | Fase 2 |
| **Nodo Gamma (APAC)** | VPS/servidor | 2–4 vCPU, 8 GB RAM, 200 GB SSD | 1 | 600 – 1 500 | Fase 2 |
| **Nodo Delta (LATAM)** | VPS/servidor | 2–4 vCPU, 8 GB RAM, 200 GB SSD | 1 | 600 – 1 500 | Fase 2 |
| **CryptoHost / HSM** | HSM o hardware seguro | Módulo seguridad claves (Thales, Utimaco, o similar) | 1 | 3 000 – 15 000 | Fase 2–3 |
| **Cold storage** | Dispositivo offline | Hardware wallet institucional o PC aislada | 1–2 | 500 – 2 000 | Fase 1–2 |
| **Subtotal Blockchain/CryptoHost** | | | | **7 400 – 27 500** | |

### 3.2 Servidores de aplicaciones (Node + .NET)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Apps Node (Shop, POS, Inventory, SmartSchool, etc.)** | Servidor / VPS | 4–8 vCPU, 16 GB RAM, 500 GB, Linux | 1 | 1 200 – 3 500 | Fase 1 |
| **APIs .NET (TradeX, DocumentFlow, HRM, etc.)** | Servidor / VPS | 4–8 vCPU, 16 GB RAM, 300 GB, Windows Server o Linux + .NET | 1 | 1 500 – 4 000 | Fase 1 |
| **Base de datos (MySQL/Postgres, SmartSchool, etc.)** | Servidor / VPS o BD gestionada | 2–4 vCPU, 8–16 GB RAM, 200–500 GB SSD | 1 | 800 – 2 500 | Fase 1 |
| **Balanceador / reverse proxy** | Incl. en VPS o LB gestionado | Nginx / cloud LB | — | 0 – 400/año | Fase 2 |
| **Subtotal aplicaciones** | | | | **3 500 – 10 000** | |

*Nota: en Fase 1 se puede unificar Node + .NET + BD en 1–2 servidores; al crecer se separan.*

### 3.3 Plataforma web (front-end)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **HTML/JS (platform/, deploy)** | Servido por Nodo Mamey o CDN | — | 0 | 0 | — |
| **CDN / estáticos** | Servicio cloud (Cloudflare, etc.) | — | — | 0 – 300/año | Fase 2 |
| **Subtotal web** | | | | **0 – 300/año** | |

### 3.4 Móvil (React Native)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Desarrollo iOS** | Mac | Mac mini o MacBook, M1+, 16 GB RAM | 1 | 800 – 2 000 | Fase 1 |
| **Pruebas Android** | Dispositivos / emulador | 1–2 Android o emulador en PC | — | 0 – 400 | Fase 1 |
| **Backend móvil** | Mismo que apps Node/API | — | 0 | 0 | — |
| **Subtotal móvil** | | | | **800 – 2 400** | |

### 3.5 Puntos de venta (POS, retail, ierahkwa-shop)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Caja POS por sede** | PC o tablet + teclado | 4–8 GB RAM, 64 GB, pantalla táctil | por sede | 300 – 600 c/u | Según apertura |
| **Impresora tickets** | Térmica 80 mm | USB/red | 1 por caja | 80 – 200 c/u | Según apertura |
| **Cajón de dinero** | Con llave, compatible impresora | — | 1 por caja | 80 – 150 c/u | Según apertura |
| **Lector códigos / RFID** | 1D/2D o RFID | USB | 0–1 por caja | 0 – 120 c/u | Según apertura |
| **Por 1 punto de venta (estimado)** | | | 1 | **460 – 1 070** | |
| **Por 5 puntos (ejemplo)** | | | 5 | **2 300 – 5 350** | Fase 2–3 |
| **Por 10 puntos** | | | 10 | **4 600 – 10 700** | Fase 3+ |

### 3.6 Inventario y almacén (InventoryManager, inventory-system)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **PC administración** | Desktop / laptop | 8 GB RAM, 256 GB, Windows | 1–2 | 500 – 1 200 | Fase 1 |
| **Lector de códigos / handheld** | Pistola o handheld 1D/2D | USB o WiFi | 1–2 | 100 – 400 | Fase 2 |
| **Impresora etiquetas** | Zebra o similar | — | 0–1 | 0 – 500 | Fase 2–3 |
| **Subtotal inventario** | | | | **600 – 2 100** | |

### 3.7 Educación (SmartSchool)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Servidor (ya en 3.2)** | — | — | 0 | 0 | — |
| **PC laboratorio / admin** | PC o thin client | 4–8 GB RAM | por sede | 400 – 800 c/u | Según sede |
| **Proyector / pantalla** | Por aula | — | por aula | 300 – 800 c/u | Según sede |
| **Por 1 sede (5 aulas, 2 admin)** | | | 1 | **3 100 – 5 600** | Fase 2–3 |

### 3.8 Workstations – Gobierno y operación

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **PM / Leader control** | Laptop alta gama | 16 GB RAM, 512 GB SSD, Windows | 1 | 1 000 – 2 000 | Fase 1 |
| **Admin / soporte** | Laptop o desktop | 8–16 GB RAM, 256 GB | 1–2 | 600 – 1 500 c/u | Fase 1 |
| **Desarrollo** | PC o Mac | 16 GB RAM, 512 GB | 1–2 | 1 000 – 2 500 c/u | Fase 1 |
| **AdvocateOffice, contratos, legal** | PC | 8 GB RAM, 256 GB | 0–1 | 0 – 900 | Fase 2 |
| **Subtotal workstations** | | | | **2 600 – 6 900** | |

### 3.9 Red, seguridad y respaldo (on‑premise o híbrido)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Firewall** | appliance o virtual | UTM / next‑gen | 1 | 300 – 2 000 | Fase 1–2 |
| **Switch** | 16–24 puertos gigabit | Managed | 1 | 150 – 400 | Fase 1 |
| **Router** | Empresarial, WiFi opc. | — | 1 | 100 – 300 | Fase 1 |
| **UPS** | Para servidores + red | 1–3 kVA, 10–15 min | 1 | 200 – 600 | Fase 1 |
| **Backup local (NAS o discos)** | 4–8 TB | NAS 2 bahías o discos externos | 1 | 300 – 800 | Fase 1 |
| **Subtotal red/seguridad** | | | | **1 050 – 4 100** | |

### 3.10 Centro de datos (si es on‑premise)

| Plataforma / Uso | Hardware | Especificación | Cant. | Costo est. (USD) | Cuándo |
|------------------|----------|----------------|-------|------------------|--------|
| **Rack** | 42U estándar | — | 1 | 500 – 1 500 | Solo on‑prem |
| **Cooling** | A/A precisión o in‑row | — | 1 | 2 000 – 8 000 | Solo on‑prem |
| **Energía / PDUs** | PDU, cables | — | — | 200 – 600 | Solo on‑prem |
| **Subtotal DC (opcional)** | | | | **2 700 – 10 100** | Fase 3+ |

*Si se usa solo cloud, esta partida = 0.*

---

## 4. CUÁNDO INVERTIR EN HARDWARE – CRONOGRAMA

### Fase 1: Arranque (Mes 0 – 3)

**Objetivo:** Nodo vivo, apps principales, administración básica, desarrollo.

| ítem | Inversión est. (USD) |
|------|------------------------|
| Nodo principal (Mamey) – VPS o servidor | 1 500 – 4 000 |
| Apps (Node + .NET + BD) – 1–2 servidores o VPS | 2 000 – 5 000 |
| Cold storage básico (hardware wallet o PC aislada) | 500 – 1 000 |
| Mac para desarrollo iOS (móvil) | 800 – 2 000 |
| Workstations: PM + 1 admin + 1 dev | 2 600 – 5 000 |
| Red básica: firewall, switch, router, UPS | 600 – 2 000 |
| Backup local (NAS o discos) | 300 – 800 |
| **Total Fase 1** | **8 300 – 19 800** |

**Cuándo:** Inicio del despliegue o ya en producción local; antes o en los primeros 3 meses de operación formal.

---

### Fase 2: Crecimiento y redundancia (Mes 4 – 12)

**Objetivo:** Nodos respaldo, más resilencia, CryptoHost/HSM, primeros POS/educación.

| ítem | Inversión est. (USD) |
|------|------------------------|
| 4 nodos respaldo (Alpha, Beta, Gamma, Delta) – VPS/servidores | 2 400 – 6 000 |
| HSM o hardware seguro (CryptoHost) | 3 000 – 15 000 |
| Separar BD o escalar servidores de apps | 500 – 2 000 |
| Balanceador / LB (si no está en cloud) | 0 – 400 |
| 1–3 puntos de venta (POS completos) | 1 400 – 3 200 |
| PC inventario + lector de códigos | 600 – 1 600 |
| Workstation adicional (legal, soporte) | 0 – 900 |
| Mejorar firewall, UPS, backup | 400 – 1 500 |
| **Total Fase 2** | **8 300 – 30 600** |

**Cuándo:** Cuando haya volumen de transacciones que justifique redundancia y/o se abran sedes físicas (retail, educación).

---

### Fase 3: Escalamiento y sedes (Año 2)

**Objetivo:** Más POS, más sedes SmartSchool, posible DC on‑premise o más servidores.

| ítem | Inversión est. (USD) |
|------|------------------------|
| 5–10 puntos de venta adicionales | 2 300 – 10 700 |
| 1–2 sedes SmartSchool (PC, proyector, admin) | 3 100 – 11 200 |
| Impresora de etiquetas (inventario) | 0 – 500 |
| Servidores adicionales o migración a DC on‑premise (opc.) | 2 700 – 10 100 |
| **Total Fase 3** | **8 100 – 32 500** |

**Cuándo:** Año 2, según apertura de locales y colegios.

---

### Fase 4: Consolidación y alta disponibilidad (Año 2–3)

**Objetivo:** Tier 4/Tier 3 (ref. cryptohost), más capacidad, más sedes.

| ítem | Inversión est. (USD) |
|------|------------------------|
| DC on‑premise (rack, cooling, PDUs) si se opta | 2 700 – 10 100 |
| Más nodos o más capacidad por nodo | 2 000 – 8 000 |
| Más POS, más sedes educación, más workstations | 5 000 – 20 000 |
| **Total Fase 4** | **9 700 – 38 100** |

**Cuándo:** Año 2–3, según planes de expansión y cumplimiento (ISO, SOC 2, Tier).

---

## 5. RESUMEN DE INVERSIÓN EN HARDWARE

| Fase | Período | Inversión (USD) |
|------|---------|----------------------|
| **Fase 1** | Mes 0–3 | 8 300 – 19 800 |
| **Fase 2** | Mes 4–12 | 8 300 – 30 600 |
| **Fase 3** | Año 2 | 8 100 – 32 500 |
| **Fase 4** | Año 2–3 | 9 700 – 38 100 |
| **Total hardware (todas las fases)** | | **34 400 – 121 000** |

**Recurrente hardware (reposición, ampliación, mantenimiento):** **2 000 – 8 000 USD/año** (a partir de Fase 2).

---

## 6. COSTO TOTAL – SOFTWARE + HARDWARE

### 6.1 Una vez

| Concepto | USD |
|----------|-----|
| Desarrollo software | 355 350 |
| Certificaciones (prorrateo, opc.) | 22 500 – 65 000 |
| Hardware Fase 1 | 8 300 – 19 800 |
| Hardware Fase 2 (prorrateo año 1) | 4 150 – 15 300 |
| **Total una vez (año 1)** | **390 300 – 455 450** |

*Si el software ya está construido, la “inversión nueva” año 1 es: certificaciones (opc.) + Fase 1 + mitad de Fase 2 ≈ 35 000 – 100 000 USD (solo hardware + certs).*

### 6.2 Recurrente anual

| Concepto | USD/año |
|----------|---------|
| Licencias | 1 500 – 5 000 |
| Infra cloud | 2 450 – 6 500 |
| Servicios externos | 7 000 – 74 900 |
| Mantenimiento y soporte | 59 300 |
| Hardware (reposición, ampliación, mantenimiento) | 2 000 – 8 000 |
| **Total recurrente** | **72 250 – 153 700** |

### 6.3 Inversión en hardware por momento (cuándo invertir)

| Cuándo | En qué invertir | Monto est. (USD) |
|--------|------------------|-------------------|
| **Ahora / Mes 0–3** | Nodo, apps, BD, cold storage, Mac dev, workstations (PM, admin, dev), red, UPS, backup | 8 300 – 19 800 |
| **Mes 4–6** | 2 nodos respaldo (p. ej. Alpha, Beta) | 1 200 – 3 000 |
| **Mes 7–12** | 2 nodos más (Gamma, Delta), HSM/CryptoHost, 1–3 POS, inventario, balanceador | 7 100 – 27 600 |
| **Año 2** | POS adicionales, sedes SmartSchool, más workstations, impresora etiquetas; opc. DC on‑premise | 8 100 – 32 500 |
| **Año 2–3** | DC on‑premise (si aplica), más nodos/capacidad, más sedes | 9 700 – 38 100 |

---

## 7. TABLA GLOBAL – PLATAFORMA ↔ HARDWARE

| Plataforma | Hardware principal | Fase | Costo aprox. (USD) |
|------------|--------------------|------|---------------------|
| **Mamey Node / blockchain** | Servidor 4–8 vCPU, 16–32 GB | 1 | 1 500 – 4 000 |
| **Nodos Alpha–Delta** | 4× VPS 2–4 vCPU, 8 GB | 2 | 2 400 – 6 000 |
| **CryptoHost / HSM** | HSM + cold storage | 2 | 3 500 – 17 000 |
| **ierahkwa-shop, inventory, POS, SmartSchool (Node)** | 1 servidor 4–8 vCPU, 16 GB | 1 | 1 200 – 3 500 |
| **APIs .NET (TradeX, HRM, etc.)** | 1 servidor 4–8 vCPU, 16 GB | 1 | 1 500 – 4 000 |
| **MySQL/Postgres (SmartSchool, etc.)** | 1 VPS/servidor 2–4 vCPU, 8–16 GB | 1 | 800 – 2 500 |
| **Platform (HTML/JS)** | Servido por Mamey o CDN | 1 | 0 |
| **ierahkwa-mobile** | Mac (dev) + Android (pruebas) | 1 | 800 – 2 400 |
| **POS (por punto)** | PC/tablet + impresora + cajón + opc. lector | 2–3 | 460 – 1 070 c/u |
| **InventoryManager / inventario** | PC + lector + opc. impresora etiquetas | 1–2 | 600 – 2 100 |
| **SmartSchool (por sede)** | PC admin + PC aulas + proyector | 2–3 | 3 100 – 5 600 |
| **Leader control, admin, legal** | Laptops/desktops | 1 | 2 600 – 6 900 |
| **Red, firewall, UPS, backup** | Firewall, switch, router, UPS, NAS | 1 | 1 050 – 4 100 |
| **DC on‑premise (opc.)** | Rack, cooling, PDUs | 3–4 | 2 700 – 10 100 |

---

## 8. CHECKLIST – QUÉ COMPRAR Y CUÁNDO

### Inmediato (Fase 1)

- [ ] Servidor o VPS para **Nodo Mamey** (4–8 vCPU, 16–32 GB, 500 GB SSD).
- [ ] Servidor o VPS para **Node + .NET + BD** (o 2: uno Node, uno .NET+BD).
- [ ] **Cold storage** (hardware wallet o PC sin red).
- [ ] **Mac** para compilar y probar app iOS.
- [ ] **Workstations:** 1 PM, 1 admin, 1 desarrollo.
- [ ] **Red:** firewall, switch, router, UPS.
- [ ] **Backup:** NAS o discos 4–8 TB.

### En 6–12 meses (Fase 2)

- [ ] **4 nodos respaldo** (Alpha, Beta, Gamma, Delta) como VPS o servidores.
- [ ] **HSM** o hardware seguro para CryptoHost.
- [ ] **1–3 puntos POS** (PC/tablet, impresora, cajón, opc. lector).
- [ ] **PC inventario** + lector de códigos.
- [ ] **Balanceador** si el tráfico lo pide.
- [ ] Más **workstations** si crece el equipo.

### En año 2 (Fase 3)

- [ ] **5–10 POS** más según sedes.
- [ ] **1–2 sedes SmartSchool** (PC, proyector, admin).
- [ ] **Impresora de etiquetas** para almacén.
- [ ] Valorar **DC on‑premise** o más servidores en cloud.

### En año 2–3 (Fase 4)

- [ ] **DC on‑premise** (rack, cooling) si se decide salir de 100% cloud.
- [ ] Más **nodos o capacidad** de nodos.
- [ ] Más **POS, sedes y workstations** según expansión.

---

## 9. NOTAS

1. **Cloud vs on‑premise:** Las cifras de servidores pueden ser **cloud (VPS/dedicado)** o **on‑premise**. DC on‑premise solo aplica si hay sitio y presupuesto (Fase 3–4).
2. **POS y SmartSchool:** Costos por punto/sede; el total depende del número de locales y escuelas.
3. **HSM:** Rango amplio según fabricante y modelo; 3 000–15 000 USD es orientativo.
4. **Moneda:** Todo en USD.
5. **Actualización:** Revisar este reporte al menos una vez al año y al abrir nuevas sedes.

---

**Sovereign Government of Ierahkwa Ne Kanienke**  
*Reporte completo de costos e inversión en hardware – 19 Enero 2026*
