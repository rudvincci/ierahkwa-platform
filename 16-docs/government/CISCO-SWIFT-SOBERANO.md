# Cisco + SWIFT — Stack Soberano

**Sovereign Government of Ierahkwa Ne Kanienke**  
Red y mensajería financiera propias: Cisco en la capa de red, SWIFT MT/MX propio en código (Rust).

---

## 1. Visión soberana

- **No duplicar:** no se usa SWIFT de terceros como caja negra; se implementa lo propio.
- **Cisco:** equipos bajo nuestro control para red, firewall y segmentación que protegen el servicio SWIFT propio.
- **Código propio:** servicio Rust en `RuddieSolution/services/rust/` — MT/MX (parse, generación) en puerto **8590**.

---

## 2. Stack SWIFT propio (ya en el repo)

| Componente        | Ubicación                          | Función                          |
|------------------|------------------------------------|----------------------------------|
| Parser MT        | `services/rust/src/swift/mt.rs`    | Bloques {1:..} a {5:..} MT       |
| Parser MX        | `services/rust/src/swift/mx.rs`    | ISO 20022 XML                    |
| Servicio Rust    | `services/rust/` (puerto 8590)     | HTTP API, crypto, SWIFT         |
| BIC soberanos    | BDET Bank UI / datos               | IERBDETXXX, IERAGUILAX, etc.     |

El Node (8545) y el Banking Bridge (3001) pueden consumir el servicio Rust vía `RUST_SERVICE_URL` (por defecto `http://127.0.0.1:8590`).

---

## 3. Rol de Cisco (capa de red)

Cisco no ejecuta SWIFT; **protege y conecta** el servidor donde corre nuestro servicio:

- **Segmentación:** VLAN o zonas para:
  - **Zona SWIFT:** solo el host del servicio Rust (8590) y los que hablan con él (ej. Node, Banking Bridge).
  - **Zona banca:** BDET, SIIS, Clearing, etc.
  - **Zona usuario:** plataforma web, admin.
- **Firewall (ACL / ZBFW):**
  - Permitir solo los puertos necesarios (8590 desde Node/Bridge, 8545, 3001 según diseño).
  - Denegar acceso directo a 8590 desde internet.
- **NAT (si aplica):** salida controlada para el tráfico que deba salir; sin exponer 8590 al público.
- **QoS (opcional):** priorizar tráfico hacia/desde el host del servicio Rust para baja latencia.

---

## 4. Modelos Cisco útiles

| Uso                    | Opción                    | Notas                                      |
|------------------------|---------------------------|--------------------------------------------|
| Routers con VM         | ISR 4000, ASR 1000 (IOS XE)| KVM: se puede correr Linux/containers      |
| Firewall + segmentación| ASA, FTD                  | Zonas, ACL, inspección                    |
| Solo routing/L3        | ISR 1000/900, Catalyst    | VLANs, ACL, QoS                            |
| Código abierto en equipo | OpenWrt/DD-WRT (RV, etc.) | Si el hardware lo permite, control total   |

No es necesario el modelo más alto; basta con segmentar y filtrar tráfico hacia el host del servicio Rust.

---

## 5. Arquitectura de referencia

```
                    Internet (controlado)
                           |
                    [Cisco FW / Router]
                           |
         +-----------------+------------------+
         |                 |                  |
    [Zona usuario]   [Zona banca]      [Zona SWIFT]
    8545, 8080      3001, 4001..      8590 (Rust SWIFT)
    Platform        BDET, SIIS         MT/MX, crypto
```

- El **servicio Rust (8590)** solo es alcanzado por los servicios internos que lo necesiten (ej. Node, Banking Bridge).
- Cisco aplica reglas de firewall y, si se usa, QoS hacia ese host.

---

## 6. Configuración mínima (ejemplo conceptual)

En el equipo Cisco (ACL o ZBFW):

- Permitir **8590** solo desde IPs de los servidores Node/Banking Bridge (o desde la VLAN banca).
- Permitir **8545**, **3001** según política de acceso a la plataforma.
- No exponer **8590** a internet.
- Opcional: política QoS para tráfico destinado al host 8590.

(Comandos exactos dependen del modelo y versión de IOS/IOS XE; aquí se deja el criterio.)

---

## 7. Resumen

| Qué                | Quién lo hace        | Dónde                    |
|--------------------|----------------------|---------------------------|
| Mensajería SWIFT   | Nosotros (Rust)      | `services/rust`, puerto 8590 |
| BIC / lógica banco | Nosotros             | BDET Bank, datos          |
| Red, seguridad     | Cisco (nuestro)      | Routers/FW bajo nuestro control |

**Cisco SWIFT “soberano”** = red y firewall Cisco protegiendo y conectando **nuestro** servicio SWIFT (Rust MT/MX), sin depender de implementaciones SWIFT de terceros en la capa de aplicación.

---

*Documento de referencia técnica. Sovereign Government of Ierahkwa Ne Kanienke.*
