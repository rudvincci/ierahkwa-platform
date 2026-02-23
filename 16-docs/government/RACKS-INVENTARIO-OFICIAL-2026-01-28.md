# Inventario oficial – 5 racks IERAHKWA
## Registro físico para que todo el trabajo sea localizable y facturable

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

**Fotos de referencia:**
- 5 racks: `assets/PHOTO-2026-01-28-15-16-26-f5248b6b-b9d9-4d6e-801e-b11fc103d798.png`
- **Rack de inicio (con planos):** `assets/Screenshot_2026-01-28_at_4.57.04_PM-ad568a9a-2ff2-4bae-9bd3-dc5aa5ef0b8e.png`

---

## Planos y rack de inicio

Se habían hecho **planos** (diseño y disposición) para uno de los racks. Con **ese rack** íbamos a empezar para live production. Ese rack está documentado en:

- **`docs/PLANO-RACK-INICIO-LIVE-PRODUCTION.md`** — Plano del rack de inicio: Fortinet, Cisco, monitor consola (CLI), switch, enclosures de almacenamiento (12 bahías), 10–12 servidores slim, UPS FUZE. Foto: `Screenshot_2026-01-28_at_4.57.04_PM...`.

---

## Objetivo: producción en vivo (live production)

Estos servidores y racks se estaban **preparando para ir a producción en vivo** (air live production): Bank, Exchange, 60 plataformas, blockchain, confirmaciones de gas, Cisco a internet. Todo lo montado aquí forma parte del plan para llevar el sistema IERAHKWA a **producción real**.

---

## Resumen

Estos son los **5 racks** (raquet) que soportan la infraestructura del sistema: Bank, Exchange, 60 plataformas, confirmaciones de gas blockchain, Cisco a internet. Todo lo montado aquí queda **registrado** en este documento para que el trabajo y la inversión sean **encontrables** y no se pierdan.

---

## RACK 1 (izquierda)

| Elemento | Descripción |
|----------|-------------|
| **Monitor** | Pantalla plana negra, estante inferior |
| **Teclado** | Teclado negro, consola de gestión |
| **UPS** | AKZOM AK-700VA-M (base del rack) |
| **Resto** | Rails visibles, espacio superior libre; cableado suelto |

**Uso:** Consola de gestión / KVM; alimentación protegida.

---

## RACK 2

| Elemento | Descripción |
|----------|-------------|
| **Switches** | Varios switches de red (negro/gris/azul), muchos puertos RJ45 |
| **Cableado** | Cables verdes y azules en patch; cables negros hacia abajo |
| **Rol** | Red/Core – posible equipamiento tipo **Cisco** para conectar todo a internet |

**Uso:** Conmutación de red; conexión de servidores y salida a internet.

---

## RACK 3 (centro)

| Elemento | Descripción |
|----------|-------------|
| **Monitor** | Pantalla plana negra, parte superior |
| **Laptop** | Portátil abierto con terminal/código (pantalla encendida) |
| **Switch** | Switch de red horizontal negro |
| **Servidores** | Múltiples unidades rackeables compactas (16+ visibles) – tipo **ProLiant EC200a / HP G4** |
| **Blades/cajas** | 2 cajas negras con ranuras verticales (módulos tipo blade) |
| **UPS** | AKZOM AK-700VA-M (base) |

**Uso:** Nodos de confirmación de gas blockchain, servicios core, consola de operaciones.

---

## RACK 4

| Elemento | Descripción |
|----------|-------------|
| **Monitor** | Pantalla plana negra, superior |
| **Switch** | Switch horizontal negro |
| **Servidores** | Misma disposición que Rack 3: 16+ unidades compactas (ProLiant/HP G4) + 2 cajas con slots verticales |
| **Cable** | Cable de red azul visible (diagonal) |
| **UPS** | AKZOM AK-700VA-M (base) |

**Uso:** Confirmadores de gas, redundancia, servicios distribuidos.

---

## RACK 5 (derecha)

| Elemento | Descripción |
|----------|-------------|
| **Monitor** | Pantalla plana negra, superior |
| **Servidores** | Rack muy poblado: 16–24+ unidades rackeables compactas (ProLiant/HP G4) en varias alturas |
| **UPS** | AKZOM AK-700VA-M (base) |

**Uso:** Capacidad de cómputo para las 60 plataformas (Bank, Exchange, etc.); nodos y servicios.

---

## Equipos identificados (conteo orientativo)

| Tipo | Cantidad aprox. | Notas |
|------|------------------|--------|
| **Racks 19"** | 5 | Fila completa |
| **UPS AKZOM AK-700VA-M** | 5 | Uno por rack |
| **Monitores** | 5 | Uno por rack |
| **Switches de red** | Múltiples | Racks 2, 3, 4 – tipo Cisco/enterprise |
| **Servidores compactos (ProLiant/HP G4)** | 40+ | Racks 3, 4, 5 |
| **Cajas tipo blade** | 4 | 2 en Rack 3, 2 en Rack 4 |
| **Laptop/consola** | 1 | Rack 3 (operaciones) |

---

## Relación con el proyecto

- **Confirmaciones de gas blockchain:** ProLiant EC200a y HP G4 en Racks 3, 4, 5 (ver `docs/HARDWARE-BLOCKCHAIN-GAS-CONFIRMACIONES.md`).
- **Cisco / internet:** Switches en Rack 2 (y según diseño en otros racks) para conectar todo a internet.
- **60 plataformas (Bank, Exchange, etc.):** Capacidad de cómputo repartida en estos racks; ver `REPORTE-FINAL-SISTEMA-COMPLETO-2026-01-23.md`.

Este inventario es la **referencia oficial** del trabajo físico realizado: todo lo que está en estos racks queda documentado aquí para que sea **encontrable** y no se pierda a la hora de cobrar o auditar.

---

## Estado: preparación para live production

| Aspecto | Estado |
|---------|--------|
| **Objetivo** | Llevar IERAHKWA a **producción en vivo** (live production) |
| **Racks** | 5 racks montados y cableados |
| **Servidores** | ProLiant EC200a, HP G4 – preparados para confirmaciones de gas y 60 plataformas |
| **Red** | Cisco/switches – preparados para conectar todo a internet |
| **UPS** | AKZOM por rack – alimentación protegida |
| **Próximo paso** | Completar configuración de red, IPs y puesta en marcha de servicios para go-live |
| **Planos** | Se habían hecho planos; rack de inicio documentado en `docs/PLANO-RACK-INICIO-LIVE-PRODUCTION.md` |

---

*Documento vivo. Actualizar si se añade o cambia equipo en los racks. Foto: 28-Ene-2026. Estos servidores estaban siendo preparados para air live production. Se habían hecho planos; íbamos a empezar con el rack documentado en PLANO-RACK-INICIO-LIVE-PRODUCTION.md.*
