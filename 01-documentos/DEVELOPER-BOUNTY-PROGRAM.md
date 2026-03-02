# Programa de Recompensas para Desarrolladores / Developer Bounty Program

## Gobierno Soberano de Ierahkwa Ne Kanienke

---

## Filosofia / Philosophy

> Los Guardianes no trabajan por sueldo, sino por reputacion inmutable ($MATTR).
> Guardians do not work for a salary, but for immutable reputation ($MATTR).

El token $MATTR (Materia Soberana) no es una criptomoneda especulativa.
Es un registro inmutable de contribucion a la infraestructura soberana.
Cada $MATTR minted representa trabajo verificado por la comunidad de
Guardianes y almacenado permanentemente en la blockchain MameyNode.

$MATTR cannot be bought. It can only be earned.

$MATTR no se puede comprar. Solo se puede ganar.

Cada contribucion aceptada queda registrada en el contrato inteligente
`IerahkwaManifesto.sol` con el hash del commit, la direccion del
contribuidor, y el voto de aprobacion del Consejo de Guardianes.

---

## Niveles de Recompensa / Bounty Tiers

### Bronze — 100 a 500 $MATTR

**Tipo de trabajo:** Correcciones menores, documentacion, traducciones

Ejemplos / Examples:
- Corregir un bug no critico en la interfaz
- Mejorar documentacion existente (claridad, ejemplos)
- Traducir documentos a espanol, portugues, quechua, nahuatl, o guarani
- Agregar comentarios de codigo o docstrings
- Actualizar dependencias menores
- Escribir tests unitarios para funciones existentes
- Corregir errores tipograficos en contratos inteligentes

**Requisitos / Requirements:**
- Pull Request aprobado por 1+ Guardian
- Tests pasando (si aplica)
- Sin regresiones

---

### Silver — 500 a 2,000 $MATTR

**Tipo de trabajo:** Implementacion de features, integracion de protocolos

Ejemplos / Examples:
- Implementar un nuevo endpoint en la API Gateway
- Integrar un nuevo protocolo de comunicacion (puente a Signal, IRC, etc.)
- Crear un nuevo componente de interfaz para el dashboard Maestro
- Optimizar rendimiento de consultas PostgreSQL
- Implementar caching inteligente con Redis
- Crear scripts de automatizacion para despliegue
- Integrar un nuevo servicio en docker-compose
- Mejorar compresion de mensajes LoRa

**Requisitos / Requirements:**
- Pull Request aprobado por 2+ Guardians
- Tests unitarios + tests de integracion
- Documentacion actualizada
- Sin vulnerabilidades de seguridad

---

### Gold — 2,000 a 10,000 $MATTR

**Tipo de trabajo:** Infraestructura critica, auditorias de seguridad, smart contracts

Ejemplos / Examples:
- Desarrollar un nuevo smart contract para la gobernanza
- Realizar auditoria completa de seguridad de un servicio
- Implementar nuevo servicio core (SoberanoDoctor, PupitreSoberano, etc.)
- Disenar e implementar nueva arquitectura de base de datos
- Crear sistema de replicacion IPFS con Filecoin
- Implementar cifrado post-cuantico en un servicio
- Desarrollar firmware para nodo hardware ESP32
- Construir sistema de votacion cuadratica

**Requisitos / Requirements:**
- Pull Request aprobado por 3+ Guardians
- Tests unitarios + integracion + end-to-end
- Documentacion tecnica completa
- Revisiones de seguridad documentadas
- Performance benchmarks

---

### Diamond — 10,000+ $MATTR

**Tipo de trabajo:** Vulnerabilidades criticas, contribuciones arquitectonicas mayores

Ejemplos / Examples:
- Descubrir y corregir una vulnerabilidad critica de seguridad
- Redisenar arquitectura de un sistema core
- Implementar migracion de cifrado a post-cuantico (ML-DSA + ML-KEM)
- Construir puente cross-chain con otra blockchain soberana
- Crear sistema de identidad descentralizada completo
- Desarrollar protocolo de consenso para MameyNode
- Implementar almacenamiento en ADN (DNA storage) funcional

**Requisitos / Requirements:**
- Pull Request aprobado por 5+ Guardians (o Consejo completo)
- Auditoria de seguridad independiente
- Tests exhaustivos en todos los niveles
- Documentacion tecnica + guia de operacion
- Plan de rollback documentado
- Periodo de prueba en staging (minimo 30 dias)

---

## Como Participar / How to Participate

### Paso 1: Fork del repositorio

```bash
# Hacer fork en GitHub, luego clonar
git clone https://github.com/TU-USUARIO/red-soberana.git
cd red-soberana
git remote add upstream https://github.com/soberano/red-soberana.git
```

### Paso 2: Firmar el Manifiesto / Sign the Manifesto

Firmar el contrato `IerahkwaManifesto.sol` con tu wallet:

```bash
# Conectar wallet a MameyNode
# Navegar a la dApp de gobernanza
# Firmar la transaccion "SignManifesto"
```

Al firmar el manifiesto aceptas:
- Contribuir a la soberania digital de los pueblos de las Americas
- No introducir backdoors, malware, o vulnerabilidades intencionales
- Respetar la Constitucion de Gobernanza (GOVERNANCE-CONSTITUTION.md)
- No ceder derechos de tu contribucion a entidades corporativas o estatales

### Paso 3: Reclamar un bounty / Claim a Bounty

1. Ir a GitHub Issues con la etiqueta `bounty`
2. Comentar en el issue: "Reclamo este bounty / I claim this bounty"
3. Un Guardian asignara el issue a tu cuenta
4. Tienes 14 dias para enviar un PR (extension posible con justificacion)

### Paso 4: Enviar Pull Request

```bash
# Crear rama con nombre descriptivo
git checkout -b bounty/001-lora-compression

# Hacer tus cambios
# ...

# Commit con referencia al bounty
git commit -m "feat: optimize LoRa compression algorithm

Bounty #001
Reduces payload by 35% using Huffman coding for common Guardian messages.
Tested on LilyGo T-Beam with Meshtastic firmware."

# Push y crear PR
git push origin bounty/001-lora-compression
```

### Paso 5: Revision / Review

- Minimo de revisores segun el tier del bounty
- Los revisores verifican: funcionalidad, seguridad, tests, documentacion
- Comentarios y correcciones pueden ser requeridos
- El PR debe pasar el CI pipeline completo

### Paso 6: Merge y recompensa / Merge and Reward

Una vez aprobado y mergeado:
1. El Consejo de Guardianes vota la emision de $MATTR
2. Los tokens son minted al wallet del contribuidor
3. La transaccion queda registrada en MameyNode
4. Tu contribucion aparece en el registro publico de Guardianes

---

## Bounties Activos / Active Bounties

### Bounty #001 — Optimizacion de compresion LoRa

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 1,000 $MATTR (Silver)                          |
| Descripcion    | Optimizar algoritmo de compresion en `lora_mesh_bridge.py` para reducir payload al menos 30% |
| Habilidades    | Python, compresion de datos, protocolo LoRa     |
| Estado         | Abierto                                         |

### Bounty #002 — Testing del puente JS8Call

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 500 $MATTR (Bronze)                            |
| Descripcion    | Escribir test suite para `js8call_bridge.py`, incluyendo tests de compresion, codec, y TCP mock |
| Habilidades    | Python, pytest, mocking                         |
| Estado         | Abierto                                         |

### Bounty #003 — Firmware de bio-sensores ESP32

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 2,000 $MATTR (Gold)                            |
| Descripcion    | Firmware para ESP32 que lea sensores de calidad de agua (pH, turbidez, temperatura) y envie datos via LoRa |
| Habilidades    | C/C++, ESP-IDF, LoRa, sensores analogicos       |
| Estado         | Abierto                                         |

### Bounty #004 — Optimizacion Docker para nodos moviles

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 1,500 $MATTR (Silver)                          |
| Descripcion    | Reducir el tamano total de imagenes Docker para que el stack completo quepa en 8GB. Multi-stage builds, Alpine base. |
| Habilidades    | Docker, Linux, optimizacion                     |
| Estado         | Abierto                                         |

### Bounty #005 — Replicacion IPFS a Filecoin

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 1,000 $MATTR (Silver)                          |
| Descripcion    | Implementar script que replique CIDs criticos de IPFS a Filecoin storage deals automaticamente |
| Habilidades    | IPFS, Filecoin, Go o Python                    |
| Estado         | Abierto                                         |

### Bounty #006 — UI de votacion cuadratica

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 2,000 $MATTR (Gold)                            |
| Descripcion    | Interfaz web para VotoSoberano con votacion cuadratica. React/Svelte, conectada al smart contract. |
| Habilidades    | Frontend, Web3, smart contracts                 |
| Estado         | Abierto                                         |

### Bounty #007 — Gestion de zona Handshake DNS

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 1,500 $MATTR (Silver)                          |
| Descripcion    | Panel de administracion para gestionar registros DNS en el TLD soberano via Handshake blockchain |
| Habilidades    | DNS, Handshake, Node.js                         |
| Estado         | Abierto                                         |

### Bounty #008 — Auditoria del puente Matrix-a-Tor

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 5,000 $MATTR (Gold)                            |
| Descripcion    | Auditoria completa de seguridad del puente entre Matrix Synapse y el hidden service Tor. Reporte escrito + correcciones. |
| Habilidades    | Seguridad, Tor, Matrix, penetration testing     |
| Estado         | Abierto                                         |

### Bounty #009 — Actualizacion a intercambio de claves post-cuantico

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 10,000 $MATTR (Diamond)                        |
| Descripcion    | Migrar todos los protocolos de intercambio de claves a ML-KEM (Kyber) y ML-DSA (Dilithium). Incluir migracion gradual. |
| Habilidades    | Criptografia, post-cuantica, Python/Rust        |
| Estado         | Abierto                                         |

### Bounty #010 — Traduccion espanol/portugues/quechua

| Campo          | Valor                                          |
|----------------|------------------------------------------------|
| Recompensa     | 500 $MATTR (Bronze)                            |
| Descripcion    | Traducir toda la documentacion tecnica y de usuario a espanol, portugues brasileno, y quechua |
| Habilidades    | Idiomas, documentacion tecnica                  |
| Estado         | Abierto                                         |

---

## Gobernanza de Bounties / Bounty Governance

### Aprobacion

- Todos los bounties son propuestos y aprobados por el Consejo de Guardianes
- Se requiere mayoria simple (>50%) para bounties Bronze y Silver
- Se requiere supermayoria (>66%) para bounties Gold
- Se requiere unanimidad para bounties Diamond

### Disputas

- Si un contribuidor disputa la evaluacion de su PR, puede solicitar
  arbitraje ante el JusticiaSoberano (servicio de resolucion de disputas)
- El arbitraje es gratuito y la decision es vinculante
- Los arbitros son seleccionados aleatoriamente entre Guardianes sin
  conflicto de interes

### Proponer nuevos bounties

Cualquier Guardian puede proponer un nuevo bounty:

1. Crear un issue en GitHub con la etiqueta `bounty-proposal`
2. Incluir: descripcion, tier propuesto, habilidades requeridas
3. El Consejo vota en la siguiente sesion semanal
4. Si se aprueba, se cambia la etiqueta a `bounty`

---

## Donde encontrarnos / Where to Find Us

- **GitHub Issues**: Etiqueta `bounty` en el repositorio principal
- **Matrix**: Room `#dev-bounties:ierahkwa.org`
- **LoRa Mesh**: Canal `BOUNTY` en la red Meshtastic
- **ntfy**: Topic `ierahkwa-bounties` para notificaciones de nuevos bounties

---

## Registro de Contribuciones / Contribution Ledger

Todas las contribuciones y pagos de $MATTR son publicos y verificables:

```
MameyNode Explorer: http://explorer.ierahkwa/
Contrato: IerahkwaManifesto.sol
Metodo: getContributions(address)
```

Tu trabajo vive para siempre en la blockchain. Tu reputacion es inmutable.
Your work lives forever on the blockchain. Your reputation is immutable.

---

*Ierahkwa Ne Kanienke*
*Soberania construida por la comunidad, para la comunidad*
*Sovereignty built by the community, for the community*
