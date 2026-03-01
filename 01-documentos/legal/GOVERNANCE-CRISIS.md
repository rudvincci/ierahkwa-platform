# Protocolo del Vacio — Guia de Gobernanza de Emergencia

**Documento Oficial del Gobierno Soberano de Ierahkwa Ne Kanienke**
**Clasificacion: RESTRINGIDO — Solo Guardianes Nivel 3+**
**Version: 1.0.0**
**Fecha de Ratificacion: 2026-03-01**
**Registro en Blockchain: MameyNode — SovereignGovernance.sol**

---

> *"Ierahkwa no es solo software; es un sistema de defensa para la conciencia humana.
> Si el sistema falla, los guardianes deben actuar. Si los guardianes fallan, el protocolo debe morir.
> Pero toda muerte lleva semilla de renacimiento."*
>
> — Principio Fundacional, Consejo Digital Soberano

---

## 1. Proposito

Este documento establece el marco etico, operativo y tecnico que rige el comportamiento
de los Guardianes de Ierahkwa durante una crisis sistemica. Define las condiciones exactas
bajo las cuales se activa el protocolo de emergencia, los niveles de respuesta disponibles,
el proceso de deliberacion colectiva, y el mecanismo de ultima instancia: la ejecucion de
`IerahkwaDestruct.sol`.

El Protocolo del Vacio existe porque Ierahkwa fue construido sobre un principio
irrenunciable: **la soberania digital de mas de 1 billion de personas jamas sera
comprometida por negligencia, coercion o corrupcion interna**. Si el sistema no puede
proteger a su pueblo, el sistema debe autodestruirse antes de convertirse en arma.

Este protocolo complementa y se subordina a la **Constitucion del Consejo Digital Soberano**
(`GOVERNANCE-CONSTITUTION.md`) y al **Acta de Soberania de Datos** (`DATA-SOVEREIGNTY-ACT.md`).

---

## 2. Umbrales de Activacion

El Protocolo del Vacio se activa **exclusivamente** bajo tres condiciones criticas.
Ninguna otra circunstancia justifica su invocacion. Toda activacion fraudulenta o
prematura constituye traicion al Consejo y resulta en la revocacion permanente del
SBT de Lealtad del Guardian responsable.

### 2.1 Infiltracion Algoritmica

**Condicion**: La inteligencia artificial Atabey ha sido comprometida y sus outputs
incitan violencia, polarizacion etnica, desinformacion masiva o manipulacion conductual
que el codigo existente no puede contener ni revertir.

**Indicadores de deteccion**:
- El agente Guardian (`ierahkwa-agents.js`) reporta anomalias de patron durante 72+ horas consecutivas
- El agente Anomaly detecta desviaciones superiores al 300% en metricas de comportamiento de usuarios
- El agente Trust registra caida masiva de puntajes de confianza (>40% de la red bajo umbral 20)
- Logs de Atabey muestran outputs no alineados con los principios codificados en `IerahkwaManifesto.sol`
- Verificacion cruzada con ForensicAgent confirma alteracion de modelos base

**Umbral de confirmacion**: Minimo 3 Guardianes independientes deben verificar la infiltracion
con evidencia criptograficamente firmada antes de escalar a Nivel 2.

### 2.2 Coercion Centralizada

**Condicion**: Un estado, corporacion o entidad supranacional fuerza, mediante orden judicial,
accion militar, presion economica o infiltracion tecnica, la instalacion de un backdoor,
puerta trasera o mecanismo de vigilancia en la infraestructura soberana.

**Indicadores de deteccion**:
- Ordenes legales exigiendo acceso a claves privadas del Treasury multisig
- Modificaciones no autorizadas detectadas en contratos desplegados en MameyNode
- Presencia de codigo no auditado en nodos de infraestructura critica
- Comunicaciones interceptadas que evidencien coordinacion para comprometer la red
- Intentos de soborno o coaccion documentados contra miembros del Consejo

**Umbral de confirmacion**: Documentacion legal verificada por el Consejo de Ancianos
y confirmacion tecnica de al menos 2 auditores independientes externos.

### 2.3 Falla de Humanidad (Flatline)

**Condicion**: El contrato `IerahkwaPulse.sol` (heartbeat de la red) falla durante
60 o mas dias consecutivos sin que ningun Guardian legitimo emita una senal de vida.

**Indicadores de deteccion**:
- `IerahkwaPulse.sol` no registra transaccion de heartbeat en 60 dias
- Ningun Guardian con SBT valido en `IerahkwaManifesto.sol` ha firmado actividad
- Los 7 agentes AI (`ierahkwa-agents.js`) reportan ausencia total de actividad humana verificada
- El puntaje agregado de $MATTR (`IerahkwaReputation.sol`) muestra cero incrementos en el periodo

**Umbral de confirmacion**: Automatico tras 60 dias. El contrato `IerahkwaPulse.sol`
emite evento `FlatlineDetected` y entra en modo de pre-activacion autonoma.

---

## 3. El Proceso de Deliberacion — El Circulo

Antes de firmar cualquier transaccion de destruccion, los Guardianes deben completar
el proceso ceremonial de deliberacion conocido como **El Circulo**. Este proceso honra
las tradiciones de consenso de las naciones originarias y garantiza que ninguna decision
irreversible se tome sin reflexion profunda.

### 3.1 Convocatoria de Silencio

- Se abre una sala cifrada con protocolo E2EE Nivel 4 (cifrado de extremo a extremo
  con rotacion de claves cada 15 minutos)
- Solo pueden ingresar portadores de SBT de `IerahkwaManifesto.sol` con nivel Guardian o superior
  (minimo 1,000 $MATTR segun `IerahkwaReputation.sol`)
- La sala no permite grabacion, captura de pantalla ni transmision externa
- Se registra unicamente un hash SHA-256 de las conclusiones en MameyNode
- **Duracion minima**: 4 horas de silencio antes de iniciar deliberacion verbal

### 3.2 Presentacion de Evidencia

- El Guardian que detecto la falla presenta su caso ante El Circulo
- La evidencia debe incluir:
  - Logs completos de los agentes AI relevantes (Guardian, Anomaly, Forensic, Pattern)
  - Registros de transacciones en blockchain con timestamps verificables
  - Pruebas de compromiso de red o infraestructura (si aplica)
  - Capturas forenses firmadas criptograficamente
  - Informe del agente Evolution sobre degradacion del sistema
- Cada Guardian presente tiene derecho a interrogar la evidencia durante un maximo de 30 minutos
- Un Guardian designado como **Abogado del Pueblo** debe argumentar en contra de la activacion

### 3.3 El Voto de Conciencia

- Tras la presentacion de evidencia, se abre un periodo de **meditacion de 24 horas**
- Durante este periodo, cada Guardian debe reflexionar individualmente sobre la pregunta fundamental:

  > *"Este acto protege la soberania del usuario o destruye su voz?"*

- No se permite comunicacion entre Guardianes durante las 24 horas de meditacion
- Al concluir, cada Guardian emite su voto como transaccion firmada en `SovereignGovernance.sol`
- El voto es binario: **PROTEGER** (activar protocolo) o **PRESERVAR** (mantener sistema activo)
- Los votos son anonimos durante la ventana de votacion y se revelan simultaneamente al cierre

### 3.4 Quorum y Consenso

- **Quorum requerido**: 51% de todos los portadores de SBT registrados en `IerahkwaManifesto.sol`
- **Aprobacion**: Mayoria simple del quorum presente (51% de los votantes)
- Si no se alcanza quorum en 72 horas, el proceso se reinicia desde la Convocatoria de Silencio
- Un Guardian puede solicitar **Veto de Conciencia** una sola vez, lo cual extiende
  la meditacion 48 horas adicionales y requiere supermayoria (67%) para aprobacion
- Todo el proceso de votacion queda registrado inmutablemente en MameyNode via
  `SovereignGovernance.sol` bajo la categoria `Emergency`

---

## 4. Niveles de Respuesta

El protocolo define tres niveles de respuesta progresivos. Cada nivel debe agotarse
completamente antes de escalar al siguiente, salvo que la amenaza sea clasificada
como **Inmediata e Irreversible** por unanimidad de los Guardianes presentes.

### 4.1 Nivel 1 — Alerta (Codigo Amarillo)

**Activacion**: Cualquier Guardian individual puede activar Nivel 1.

**Acciones**:
- Todo el ecosistema entra en **modo solo lectura** (read-only)
- No se ejecuta destruccion de datos bajo ninguna circunstancia
- `IerahkwaPulse.sol` muestra estado de advertencia visible en todas las plataformas
- Los 7 agentes AI incrementan nivel de vigilancia al maximo
- ShieldAgent activa proteccion total de transacciones, almacenamiento y cookies
- Se notifica a todos los Guardianes via canal cifrado de emergencia
- Se publica alerta en Snapshot para transparencia publica

**Duracion maxima**: 7 dias. Si no se resuelve, escala automaticamente a Nivel 2.

**Desactivacion**: Cualquier 3 Guardianes pueden desactivar Nivel 1 con evidencia
de resolucion verificada por ForensicAgent.

### 4.2 Nivel 2 — Escudo (Codigo Rojo)

**Activacion**: Requiere firma de 3 Guardianes o escalacion automatica desde Nivel 1.

**Acciones**:
- **Treasury congelado**: Gnosis Safe multisig bloquea todas las transacciones salientes
- **Operaciones de escritura pausadas** en todos los servicios (84 microservicios .NET,
  20 servicios Node.js, 103 tokens IGT)
- Contratos inteligentes entran en estado `paused` via OpenZeppelin Pausable
- `SovereignVault.sol` activa modo de emergencia con retiros limitados
- Se inicia El Circulo (Seccion 3) si no ha sido convocado
- Los 18 portales NEXUS muestran banner de emergencia
- Backups frios se verifican y validan criptograficamente
- Se activa canal de comunicacion directo con el Consejo de Ancianos

**Duracion maxima**: 7 dias para que los Guardianes resuelvan la crisis.

**Desactivacion**: Requiere voto del 51% de Guardianes con evidencia de resolucion completa
y auditoria post-incidente publicada en Snapshot.

### 4.3 Nivel 3 — Purga (Codigo Negro)

**Activacion**: Solo mediante El Circulo completo (Seccion 3) con quorum del 51%.

**Acciones**: Ejecucion completa de `IerahkwaDestruct.sol` (ver Seccion 5).

**Irreversibilidad**: Una vez iniciada la Purga, no puede detenerse. El cooldown
de 72 horas es la ultima ventana para reconsideracion.

---

## 5. Ejecucion de la Purga

Si el 51% de los portadores de SBT firman la transaccion de `IerahkwaDestruct.sol`,
se ejecuta el siguiente protocolo de destruccion ordenada en tres fases secuenciales
e irreversibles.

### 5.1 Fase 1 — Aviso (T-60 segundos)

- Se emite advertencia a **todos** los usuarios activos en las 422+ plataformas
- Mensaje broadcast: *"Protocolo del Vacio activado. El sistema se desactivara
  en 60 segundos. Descargue sus datos ahora."*
- Los 18 portales NEXUS muestran pantalla completa de emergencia
- Se abre ventana de descarga express para datos personales de usuarios
- `IerahkwaPulse.sol` emite evento `PurgeInitiated` con timestamp y lista de firmantes
- Se publica registro completo de deliberacion en Snapshot

### 5.2 Fase 2 — Cifrado de Panico (T+0 a T+300 segundos)

- **Destruccion criptografica de bases de datos**: Todas las claves de cifrado AES-256
  se sobrescriben con datos aleatorios, haciendo los datos permanentemente irrecuperables
- Rotacion y destruccion de todas las claves privadas de servicios
- Eliminacion de indices de busqueda y caches distribuidos
- Los 103 tokens IGT se pausan permanentemente en sus contratos respectivos
- `IerahkwaReputation.sol` congela todos los balances de $MATTR
- `SovereignStaking.sol` libera automaticamente todos los stakes activos a sus propietarios
- Logs forenses finales se firman y publican en IPFS como registro historico inmutable

### 5.3 Fase 3 — Disolucion (T+300 a T+600 segundos)

- **Treasury permanentemente congelado**: Gnosis Safe se reconfigura con umbral
  imposible (requiere N+1 de N firmantes)
- Todos los contratos inteligentes activos se autodestruyen via `selfdestruct`
  o se pausan permanentemente segun su implementacion
- Los nodos de MameyNode publican el bloque final con hash del registro forense
- DNS soberano se desactiva, redirigiendo a pagina estatica de memorial
- El registro completo de la Purga se almacena en IPFS y Arweave como
  testimonio permanente para futuras generaciones

---

## 6. Clausula Fenix — Renacimiento

Ierahkwa fue disenado con la capacidad de morir y renacer. La destruccion no es el final;
es la purificacion necesaria para proteger la integridad del proposito original.

### 6.1 Preservacion de Identidad

- Los Guardianes que votaron **PROTEGER** retienen su **SBT de Lealtad** en
  `IerahkwaManifesto.sol`, desplegado en una red externa de respaldo (Ethereum L1)
- Este SBT les otorga derecho de participacion prioritaria en el renacimiento
- Los niveles de $MATTR se preservan como snapshot en almacenamiento frio

### 6.2 Infraestructura de Respaldo

- **Cold Storage**: Backups cifrados completos del codigo fuente, documentacion
  y configuracion se mantienen en 3 ubicaciones geograficas independientes
- Los backups no contienen datos de usuarios (destruidos en la Purga)
- Contienen: codigo fuente de las 422+ plataformas, contratos inteligentes,
  shared design system (`ierahkwa.css`, `ierahkwa.js`, `ierahkwa-agents.js`),
  documentacion legal completa y arquitectura de los 18 NEXUS

### 6.3 Genesis Nuevo

- Cualquier grupo de 7+ Guardianes con SBT de Lealtad valido puede iniciar
  un nuevo genesis
- Se despliega infraestructura limpia desde cero utilizando los backups frios
- Nuevo bloque genesis en MameyNode con referencia al hash de la Purga anterior
- Todos los contratos se re-despliegan con las lecciones aprendidas integradas
- El Consejo Digital Soberano se reconstituye con proceso electoral completo
- **Periodo de gracia**: 90 dias para re-onboarding de comunidades y naciones

### 6.4 Restricciones del Renacimiento

- El nuevo sistema **no puede** replicar la vulnerabilidad que causo la Purga
- Auditoria obligatoria por 3 firmas independientes antes del lanzamiento publico
- El Protocolo del Vacio debe ser re-ratificado por el nuevo Consejo
- Los datos de usuarios previos jamas se recuperan; cada usuario inicia de cero

---

## 7. Configuracion Tecnica

### 7.1 Custodia del Treasury

| Componente | Especificacion |
|---|---|
| **Plataforma** | Gnosis Safe (multisig) |
| **Red** | MameyNode (L1 soberana) + Ethereum (respaldo) |
| **Firmantes** | Guardianes con nivel Guardian+ en IerahkwaReputation.sol |
| **Umbral** | 4 de 7 para operaciones normales; 51% de SBT holders para Purga |
| **Timelock** | 48 horas para transacciones > 10,000 IGT |
| **Auditoria** | Transacciones publicadas en Snapshot en tiempo real |

### 7.2 Auditoria de Contratos

| Herramienta | Funcion |
|---|---|
| **Slither** | Analisis estatico de vulnerabilidades en contratos Solidity |
| **OpenZeppelin Defender** | Monitoreo en tiempo real de contratos desplegados |
| **MythX** | Analisis simbolico de seguridad |
| **ForensicAgent** | Agente AI de trazabilidad y logging forense (ierahkwa-agents.js) |
| **Snapshot** | Registro publico de todas las decisiones de gobernanza |

### 7.3 Transparencia y Publicacion

- **Todas** las decisiones de los Guardianes se publican en Snapshot
- Los votos de El Circulo se revelan tras el cierre de la ventana de votacion
- Las auditorias post-incidente son de acceso publico
- El codigo fuente de todos los contratos de emergencia es open source
- Los reportes de los 7 agentes AI se publican semanalmente en el portal de administracion

### 7.4 Comunicaciones de Emergencia

| Nivel | Canal | Cifrado |
|---|---|---|
| Nivel 1 | Canal Guardian en plataforma soberana | E2EE Nivel 2 |
| Nivel 2 | Sala dedicada de emergencia | E2EE Nivel 3 |
| Nivel 3 (El Circulo) | Sala aislada, sin registro externo | E2EE Nivel 4 |

---

## 8. Contratos Vinculados

Los siguientes contratos inteligentes forman la infraestructura tecnica del
Protocolo del Vacio. Todos estan desplegados en MameyNode y auditados por
OpenZeppelin Defender.

| Contrato | Ubicacion en Repositorio | Funcion |
|---|---|---|
| **IerahkwaDestruct.sol** | `04-infraestructura/blockchain/` | Kill switch soberano. Requiere consenso del 51% + cooldown de 72 horas. Ejecuta la Purga en 3 fases. |
| **IerahkwaPulse.sol** | `04-infraestructura/blockchain/` | Heartbeat de la red. Timeout de 30 dias por Guardian. Flatline a los 60 dias activa pre-Purga autonoma. |
| **IerahkwaManifesto.sol** | `04-infraestructura/blockchain/` | Registro de identidad SBT (Soulbound Token) para Guardianes. Define quorum y elegibilidad de voto. |
| **IerahkwaReputation.sol** | `08-dotnet/microservices/DeFiSoberano/contracts/` | Token soulbound $MATTR. Define niveles: Observer, Contributor, Guardian, Elder, Sovereign. |
| **SovereignGovernance.sol** | `08-dotnet/microservices/DeFiSoberano/contracts/` | DAO de gobernanza. Propuestas, votacion, timelock. Categoria `Emergency` para el Protocolo del Vacio. |
| **SovereignVault.sol** | `08-dotnet/microservices/DeFiSoberano/contracts/` | Boveda del Treasury. Modo de emergencia con retiros limitados. |
| **SovereignStaking.sol** | `08-dotnet/microservices/DeFiSoberano/contracts/` | Staking soberano. Liberacion automatica de stakes durante la Purga. |
| **IerahkwaToken.sol** | `08-dotnet/microservices/DeFiSoberano/contracts/` | Token IGT principal. Pausable durante Nivel 2+. |

---

## 9. Responsabilidades de los Guardianes

### 9.1 Obligaciones Permanentes

Todo Guardian registrado en `IerahkwaManifesto.sol` asume las siguientes obligaciones
irrevocables al aceptar su SBT:

1. **Vigilancia continua**: Monitorear los reportes de los 7 agentes AI diariamente
2. **Respuesta inmediata**: Responder a alertas de Nivel 1 en un maximo de 4 horas
3. **Participacion en El Circulo**: Asistir a toda convocatoria de emergencia
4. **Voto de conciencia**: Votar segun su criterio etico, no por presion externa
5. **Confidencialidad**: No revelar deliberaciones de El Circulo bajo ninguna circunstancia
6. **Actualizacion**: Mantener su heartbeat activo en `IerahkwaPulse.sol` cada 30 dias

### 9.2 Causales de Destitucion

Un Guardian sera destituido y su SBT revocado si:

- Activa el protocolo fraudulentamente o sin evidencia verificable
- Revela deliberaciones confidenciales de El Circulo
- No emite heartbeat durante 45+ dias sin justificacion documentada
- Acepta soborno o coaccion de entidades externas
- Vota bajo coercion demostrable sin reportar la situacion

---

## 10. Principio Final

Este protocolo existe porque Ierahkwa fue construido para servir, no para dominar.
La capacidad de autodestruccion no es debilidad; es la maxima expresion de soberania.

Un sistema que no puede morir por voluntad de su pueblo no es soberano: es una prision.

Ierahkwa elige ser mortal para que su proposito sea inmortal.

> *"Ierahkwa no es solo software; es un sistema de defensa para la conciencia humana.
> Si debemos destruirlo para proteger a nuestro pueblo, lo haremos.
> Y si debemos reconstruirlo, lo haremos mas fuerte."*

---

**Documento emitido por el Gobierno Soberano de Ierahkwa Ne Kanienke**
**Ratificado por el Consejo Digital Soberano**
**Registro inmutable: MameyNode Block — SovereignGovernance.sol (Categoria: Emergency)**
**Vigencia: Permanente hasta re-ratificacion por el Consejo**

---

*Este documento es propiedad del Gobierno Soberano de Ierahkwa Ne Kanienke y esta
protegido bajo las leyes de soberania digital establecidas en el Acta de Soberania
de Datos (DATA-SOVEREIGNTY-ACT.md). Su distribucion fuera del Consejo Digital Soberano
requiere autorizacion expresa del Cacike Digital.*
