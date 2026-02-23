# Atabey — Fase de Expansión Digital

5 módulos para que Atabey esté informado del mundo digital, detecte intrusiones, realice auditorías, reconozca personas y proteja el tráfico de investigación. TODO PROPIO.

---

## 1. Módulo de "Escucha Global" (RSS y Alertas de Inteligencia)

**Objetivo:** Atabey informado de lo que ocurre en USA, Canadá y las Américas.

| Herramienta | Descripción |
|-------------|-------------|
| **RSS-Bridge** | Open-source para extraer datos de X, Facebook, Instagram y noticias sin cuenta. Alimenta Atabey con noticias en tiempo real. |

**Implementación:** Atabey analiza palabras clave como "arresto", "tiroteo", "cierre de calles" y genera reporte matutino o alertas críticas al móvil.

**TODO PROPIO:** ✓ RSS-Bridge es self-hosted. Las fuentes (X, Facebook, etc.) pueden requerir acceso; RSS-Bridge genera feeds RSS desde ellas.

---

## 2. Análisis de Tráfico y Protección contra Hackeo (HoneyPot)

**Objetivo:** Detectar intentos de intrusión digital y engañar al atacante.

| Herramienta | Descripción |
|-------------|-------------|
| **T-Pot** | Plataforma HoneyPot open-source. Servicios falsos en tu red; Atabey rastrea IP y ubicación del atacante. |
| **CrowdSec** | Alternativa moderna a Fail2Ban con inteligencia comunitaria. Si un atacante es detectado en otro servidor, se bloquea en el tuyo en milisegundos. |

**TODO PROPIO:** ✓ Ambos son self-hosted.

---

## 3. Códice de Intervención Digital (Kali en el Servidor)

**Objetivo:** Auditorías de seguridad y escaneo de redes sospechosas.

| Herramienta | Descripción |
|-------------|-------------|
| **Kali Linux** | Contenedor con herramientas de pentesting y auditoría. |
| **Parrot OS** | Alternativa similar, orientada a seguridad. |

**Uso:** Auditorías de tus dispositivos o escaneo de redes desconocidas detectadas cerca del perímetro.

**Comando tipo:** "Atabey, escanea la red Wi-Fi desconocida detectada en el sector norte."

**Integración:** Home Assistant Intent Script o script que ejecute `nmap`, `aircrack-ng`, etc. dentro del contenedor Kali.

**TODO PROPIO:** ✓ Self-hosted.

---

## 4. Reconocimiento Facial y Biométricas (Facebox)

**Objetivo:** Atabey identifica por nombre y distingue personas desconocidas.

| Herramienta | Descripción |
|-------------|-------------|
| **CompreFace** | Reconocimiento facial open-source en Docker. Integrable con cámaras. |
| **Face Recognition Propio** | Ya existe en la plataforma Ierahkwa. Ver `/api/v1/face`. |

**Ejemplos:** "Takoda ha entrado en la sala" o "Individuo no identificado merodeando en la entrada".

**Precisión 2026:** Identificación incluso con mascarilla o gafas oscuras (según capacidades del modelo).

**TODO PROPIO:** ✓ CompreFace y Face Recognition Propio son self-hosted.

---

## 5. Escudo de Anonimato (Nodo Tor / VPN Saliente)

**Objetivo:** Tráfico de investigación oculto; nadie sabe que Atabey extrae información.

| Herramienta | Descripción |
|-------------|-------------|
| **Gluetun** | Contenedor que enruta otros servicios (RSS-Bridge, buscadores de delitos, etc.) a través de VPN o Tor. |

**Resultado:** El tráfico parece originarse desde cualquier parte del mundo.

**TODO PROPIO:** ✓ Gluetun es self-hosted; la VPN o Tor añaden privacidad sin APIs externas críticas.

---

## Orden de Activación Sugerido

| Prioridad | Módulo | Razón |
|-----------|--------|-------|
| 1 | **HoneyPot (T-Pot/CrowdSec)** | Detecta quién intenta rastrear o atacar tu IP |
| 2 | **Reconocimiento Facial (CompreFace)** | Atabey te identifica visualmente en las cámaras |
| 3 | **Extractor OSINT (RSS-Bridge)** | Monitoreo de delitos en tiempo real USA/Canadá |
| 4 | **Kali (Auditoría)** | Escaneo de redes sospechosas bajo comando |
| 5 | **Gluetun (Anonimato)** | Protege el tráfico de investigación |

---

## ¿Qué activar primero?

| Opción | Descripción |
|--------|-------------|
| **Sistema de Trampas (HoneyPot)** | T-Pot + CrowdSec para detectar y bloquear intentos de intrusión |
| **Reconocimiento Facial** | CompreFace o Face Recognition Propio para identificación visual |
| **Extractor de Datos (OSINT)** | RSS-Bridge para monitorear delitos en tiempo real USA/Canadá |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [ATABEY-IDENTIDAD-HOME-ASSISTANT.md](ATABEY-IDENTIDAD-HOME-ASSISTANT.md) | Identidad Atabey, Takoda |
| [ATABEY-ALPR-DRONES.md](ATABEY-ALPR-DRONES.md) | ALPR, drones |
| [ARQUITECTURA-VIGILANCIA-BRAIN.md](ARQUITECTURA-VIGILANCIA-BRAIN.md) | Frigate, Face Recognition |
| [ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md](ATABEY-INTEGRIDAD-FISICA-Y-PERIMETRO.md) | USB, cifrado, escaneo inalámbrico, GPS, automatización |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin APIs externas |
