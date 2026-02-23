# Plano – Rack de inicio para live production
## El rack con el que íbamos a empezar (planos y diseño)

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

**Foto de referencia:** `assets/Screenshot_2026-01-28_at_4.57.04_PM-ad568a9a-2ff2-4bae-9bd3-dc5aa5ef0b8e.png`

---

## Resumen

Se habían hecho **planos** (diseño y disposición) para este rack. Este es el rack con el que **íbamos a empezar** para llevar IERAHKWA a producción en vivo (live production). Todo lo que está aquí queda documentado para que el trabajo y los planos sean **encontrables**.

---

## Orden en el rack (de arriba a abajo)

| Posición | Equipo | Descripción |
|----------|---------|-------------|
| **Parte superior** | **FORTINET** | Aparato de seguridad (firewall/gateway). Marca visible: Fortinet. |
| | **Cisco** | Dispositivo de red (router o switch). Conectando todo a internet. |
| | **Monitor consola** | Pantalla con CLI – texto tipo "Menu V.9.3.4 (R 160) tty1", "login:". Consola local para gestión del rack. |
| **Sección media** | **Switch de red** | Muchos puertos Ethernet; cables azules y amarillos conectados. |
| | **Enclosures de discos** | 2 unidades negras apiladas, cada una con **6 bahías** hot-swap (12 bahías en total). Almacenamiento / “bank ext” para datos. |
| | **Servidores** | Aprox. **10–12 unidades** negras delgadas e idénticas. Los servidores que se estaban preparando para live production. |
| **Parte inferior** | **Estante perforado** | Estante negro vacío (flujo de aire / gestión de cables / expansión). |
| | **UPS FUZE** | Marca FUZE; display muestra "FUZE 81". Alimentación protegida para todo el rack. |
| | **Cableado** | Cables de alimentación y red en la base. |

---

## Equipos identificados en este rack

| Tipo | Cantidad | Notas |
|------|----------|--------|
| **Fortinet** | 1 | Firewall/gateway – parte alta del rack |
| **Cisco** | 1 | Router/switch – debajo de Fortinet |
| **Monitor consola** | 1 | CLI (Menu V.9.3.4, login) |
| **Switch** | 1 | Patch con cables azul/amarillo |
| **Enclosures de almacenamiento** | 2 | 6 bahías c/u = 12 slots |
| **Servidores (slim)** | 10–12 | Unidades negras estándar para live production |
| **UPS FUZE** | 1 | Display "FUZE 81" |

---

## Relación con los planos

- Se habían hecho **planos** (diseño y disposición) para este rack.
- Este es el rack con el que **íbamos a empezar** para live production.
- Inventario general de los 5 racks: `docs/RACKS-INVENTARIO-OFICIAL-2026-01-28.md`.
- Hardware blockchain (ProLiant, HP G4, Cisco): `docs/HARDWARE-BLOCKCHAIN-GAS-CONFIRMACIONES.md`.

---

## Próximo paso (según plan)

1. Retomar o recrear los **planos** (diagrama de red, IPs, servicios por unidad) si se perdieron.
2. Usar **este rack como punto de inicio** para go-live: Fortinet → Cisco → switch → servidores y storage.
3. Documentar IPs, VLANs y roles (cuál es Mamey Node, cuál Banking Bridge, cuál storage) cuando se definan.

---

*Documento vivo. Este es el rack para el que se habían hecho planos y con el que íbamos a empezar para live production. Actualizar cuando se retome el despliegue.*
