# Monitoreo de cámaras, video y dispositivos

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO  
Referencia: NVR/VMS, análisis de tráfico y dispositivos móviles, escáneres de red. Herramientas de código abierto para vigilancia y seguridad soberana.

---

## 1. Monitoreo de cámaras y video (NVR/VMS)

Para ver y gestionar cámaras de seguridad desde un servidor propio:

| Herramienta | Descripción | Uso en Ierahkwa |
|-------------|-------------|------------------|
| **iSpy / Agent DVR** | Plataforma avanzada de videovigilancia. Audio bidireccional, detección de movimiento, grabación local o en la nube. | Referencia para vigilancia soberana; despliegue propio en infraestructura controlada. |
| **ZoneMinder** | Sistema robusto para Linux; controla cámaras IP de múltiples fabricantes. Ideal para vigilancia compleja y privada. | Listado en `node/data/security-tools-recommended.json`. |
| **go2rtc** | Servidor de streaming de alto rendimiento; audio bidireccional para cámaras Ring, Xiaomi, Tapo y otras; integrable con domótica. | Listado en `security-tools-recommended.json`. |

Referencia de vigilancia en plataforma: `docs/PLATAFORMA-VIGILANCIA-MEJOR-QUE-AGENCIAS.md`, `platform/security-fortress.html`.

---

## 2. Monitoreo y análisis de dispositivos (móviles y red)

Para auditar actividad o seguridad de teléfonos y red:

| Herramienta | Descripción | Uso en Ierahkwa |
|-------------|-------------|------------------|
| **Mobile Security Framework (MobSF)** | Análisis de seguridad en apps Android e iOS; revisa permisos (micrófono, cámara, etc.) que solicitan. | Listado en `security-tools-recommended.json`. |
| **Wireshark** | Estándar para capturar y analizar tráfico de red; permite ver qué datos envían dispositivos a servidores externos en tiempo real. | Listado en `security-tools-recommended.json`. |
| **Rayhunter** (EFF) | Proyecto de la Electronic Frontier Foundation para detectar simuladores de celdas (Stingrays) que interceptan comunicaciones móviles. | Listado en `security-tools-recommended.json`; alineado con protección de privacidad soberana. |

---

## 3. Escáner de red (IPv4 / IPv6)

Para descubrir dispositivos activos en redes locales o públicas:

| Herramienta | Descripción | Nota |
|-------------|-------------|------|
| **LanScan** | Escáner IPv4 e IPv6: detecta interfaces (Wi‑Fi, Ethernet, virtuales), escanea rangos IP (ARP local, Ping/SMB/mDNS público), muestra IP, MAC, hostnames, vendor. Exporta CSV. Versión de prueba con algunas opciones limitadas. | Referencia para Mac; útil para mapear cámaras, móviles y servidores en la red. Alternativa open source en Linux: **Nmap** (ya en `security-tools-recommended.json`). |
| **Nmap** | Exploración y cartografía de redes (open source). | Ya incluido en `security-tools-recommended.json`; alternativa totalmente abierta a LanScan. |

---

## 4. Códigos de diagnóstico Android

Códigos integrados en el código fuente de Android para verificar hardware y posibles rastreos:

| Código | Función |
|--------|---------|
| `*#*#4636#*#*` | Menú de información: estadísticas de uso de apps y estado de la red. |
| `*#*#197328640#*#*` | Modo de servicio: revisiones de configuración de red y posibles rastreos. |

Solo para diagnóstico propio del dispositivo. No usar para acceder a dispositivos de terceros sin autorización.

---

## 5. Nota legal

**El acceso a cámaras o micrófonos de terceros sin su consentimiento explícito es ilegal en la mayoría de jurisdicciones** y puede acarrear consecuencias penales graves. Todas estas herramientas deben usarse únicamente en sistemas propios, con autorización o en el marco de operaciones legales de vigilancia soberana.

---

## Resumen

- **NVR/VMS:** ZoneMinder, go2rtc, iSpy/Agent DVR.
- **Red y móviles:** MobSF, Wireshark, Rayhunter; Nmap y LanScan para descubrimiento de dispositivos.
- **Android:** Códigos de diagnóstico para uso propio.
- **Legal:** Consentimiento y autorización obligatorios.

Principio: `PRINCIPIO-TODO-PROPIO.md`. Índice: `docs/INDEX-DOCUMENTACION.md`. Herramientas: `GET /api/soberania/security-tools`.
