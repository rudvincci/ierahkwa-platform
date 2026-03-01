# Manual del Guardian de Ierahkwa / Ierahkwa Guardian Manual

Version: 1.0
Fecha / Date: 2026-03-01

---

## Tabla de Contenidos / Table of Contents

1. [Entendiendo el Dashboard / Understanding the Dashboard](#1-entendiendo-el-dashboard)
2. [Como ganar reputacion $MATTR / How to Earn $MATTR Reputation](#2-como-ganar-reputacion-mattr)
3. [Gobernanza: de la idea a la accion / Governance: From Idea to Action](#3-gobernanza-de-la-idea-a-la-accion)
4. [Privacidad y soberania / Privacy and Sovereignty](#4-privacidad-y-soberania)
5. [Referencia rapida / Quick Reference](#5-referencia-rapida)

---

## 1. Entendiendo el Dashboard

El dashboard principal de Ierahkwa es tu centro de control como guardian de la red soberana. Al iniciar sesion, encontraras las siguientes secciones:

### Panel de Estado

- **Indicador de salud de la red**: Muestra el estado general de los nodos conectados. Verde indica operacion normal, amarillo indica degradacion parcial, rojo indica alerta critica.
- **Nodos activos**: Numero total de nodos MameyNode que reportan latido (heartbeat) en las ultimas 24 horas.
- **Mensajes pendientes**: Cantidad de mensajes en cola de sincronizacion, relevante cuando operas en modo offline.

### Panel de Alertas

Las alertas del Peace Oracle se clasifican en tres niveles:

- **GREEN (Verde)**: Informativo. Actividad normal en la region monitoreada. No requiere accion.
- **YELLOW (Amarillo)**: Tension detectada. Incrementar monitoreo y verificar canales de comunicacion.
- **RED (Rojo)**: Amenaza militar o violencia activa. Activar protocolo de emergencia, verificar kit offline y confirmar contacto con guardianes vecinos.

### Mapa de NEXUS

El mapa interactivo muestra los 18 mega-portales NEXUS con sus plataformas subordinadas. Cada NEXUS tiene un color distintivo y muestra el conteo de plataformas activas. Puedes hacer clic en cualquier NEXUS para ver el detalle de sus plataformas.

### Panel de Tokens IGT

Muestra tu balance actual de tokens IGT (Ierahkwa Governance Token), historial de transacciones recientes y el estado del blockchain MameyNode.

---

## Understanding the Dashboard

The main Ierahkwa dashboard is your control center as a sovereign network guardian. Upon login you will find the following sections:

### Status Panel

- **Network health indicator**: Shows overall status of connected nodes. Green is normal operation, yellow is partial degradation, red is critical alert.
- **Active nodes**: Total MameyNode nodes reporting heartbeat in the last 24 hours.
- **Pending messages**: Count of messages in the sync queue, relevant when operating in offline mode.

### Alerts Panel

Peace Oracle alerts are classified in three levels:

- **GREEN**: Informational. Normal activity in the monitored region. No action required.
- **YELLOW**: Tension detected. Increase monitoring and verify communication channels.
- **RED**: Military threat or active violence. Activate emergency protocol, verify offline kit, confirm contact with neighboring guardians.

### NEXUS Map

The interactive map shows all 18 NEXUS mega-portals with their subordinate platforms. Each NEXUS has a distinctive color and shows the count of active platforms.

### IGT Token Panel

Shows your current IGT (Ierahkwa Governance Token) balance, recent transaction history, and MameyNode blockchain status.

---

## 2. Como Ganar Reputacion $MATTR

$MATTR (Material de Confianza) es el token de reputacion no transferible que mide tu contribucion a la red. A diferencia de IGT, $MATTR no se puede comprar ni vender -- solo se gana mediante acciones verificables.

### Acciones que generan $MATTR

| Accion | $MATTR | Frecuencia maxima |
|--------|--------|-------------------|
| Mantener nodo activo 24h | +1 | Diario |
| Responder a simulacro (drill) en < 5 min | +5 | Por evento |
| Responder a simulacro en < 15 min | +3 | Por evento |
| Votar en propuesta de gobernanza | +2 | Por propuesta |
| Proponer mejora aceptada | +10 | Sin limite |
| Reportar anomalia confirmada | +8 | Por evento |
| Mediar conflicto exitosamente | +15 | Por evento |
| Completar capacitacion trimestral | +5 | Trimestral |
| Verificar kit offline | +2 | Mensual |
| Participar en red LoRa mesh | +3 | Semanal |

### Niveles de Guardian

- **Observador** (0-49 $MATTR): Acceso basico de lectura. Puede votar en propuestas.
- **Guardian** (50-199 $MATTR): Puede proponer cambios y participar en mediaciones.
- **Guardian Senior** (200-499 $MATTR): Puede validar transacciones y moderar discusiones.
- **Consejero** (500+ $MATTR): Acceso al Consejo de Gobernanza. Puede proponer cambios al protocolo.

### Decaimiento

$MATTR tiene un decaimiento del 5% mensual si no se realizan acciones. Esto incentiva la participacion continua. El decaimiento se calcula el primer dia de cada mes.

---

## How to Earn $MATTR Reputation

$MATTR (Trust Material) is the non-transferable reputation token that measures your contribution to the network. Unlike IGT, $MATTR cannot be bought or sold -- it can only be earned through verifiable actions.

### Actions That Generate $MATTR

| Action | $MATTR | Maximum frequency |
|--------|--------|-------------------|
| Keep node active 24h | +1 | Daily |
| Respond to drill < 5 min | +5 | Per event |
| Respond to drill < 15 min | +3 | Per event |
| Vote on governance proposal | +2 | Per proposal |
| Propose accepted improvement | +10 | Unlimited |
| Report confirmed anomaly | +8 | Per event |
| Successfully mediate conflict | +15 | Per event |
| Complete quarterly training | +5 | Quarterly |
| Verify offline kit | +2 | Monthly |
| Participate in LoRa mesh | +3 | Weekly |

### Guardian Levels

- **Observer** (0-49 $MATTR): Basic read access. Can vote on proposals.
- **Guardian** (50-199 $MATTR): Can propose changes and participate in mediations.
- **Senior Guardian** (200-499 $MATTR): Can validate transactions and moderate discussions.
- **Councilor** (500+ $MATTR): Access to the Governance Council. Can propose protocol changes.

### Decay

$MATTR decays 5% monthly without activity. This incentivizes continuous participation. Decay is calculated on the first day of each month.

---

## 3. Gobernanza: De la Idea a la Accion

El sistema de gobernanza de Ierahkwa sigue un proceso de siete pasos para convertir ideas en acciones concretas. Todo guardian con nivel Guardian o superior puede iniciar una propuesta.

### Proceso de Propuesta

**Paso 1 -- Borrador**
El guardian redacta la propuesta usando la plantilla estandar en la plataforma de Consejo (NEXUS Consejo). La propuesta debe incluir: titulo, descripcion del problema, solucion propuesta, recursos necesarios, e impacto esperado.

**Paso 2 -- Revision por pares**
La propuesta entra en un periodo de revision de 7 dias donde cualquier guardian puede comentar, sugerir mejoras o plantear objeciones. El mediador AI facilita la discusion manteniendo el tono constructivo.

**Paso 3 -- Refinamiento**
El autor incorpora la retroalimentacion y publica una version revisada. Si hay cambios sustanciales, se abre un periodo adicional de 3 dias para comentarios.

**Paso 4 -- Votacion**
La propuesta pasa a votacion durante 5 dias. Cada guardian tiene un voto, ponderado por su nivel de $MATTR:
- Observador: peso 1
- Guardian: peso 2
- Guardian Senior: peso 3
- Consejero: peso 5

**Paso 5 -- Umbral de aprobacion**
Se requiere participacion del 30% de guardianes activos y aprobacion del 60% de los votos ponderados.

**Paso 6 -- Implementacion**
Si se aprueba, se asigna un equipo de implementacion y se establece un cronograma. El progreso se reporta semanalmente en el dashboard.

**Paso 7 -- Evaluacion**
30 dias despues de la implementacion, se realiza una evaluacion de impacto y se publica un reporte para la comunidad.

---

## Governance: From Idea to Action

The Ierahkwa governance system follows a seven-step process to convert ideas into concrete actions. Any guardian at Guardian level or above can initiate a proposal.

### Proposal Process

**Step 1 -- Draft**
The guardian writes the proposal using the standard template on the Council platform (NEXUS Consejo). The proposal must include: title, problem description, proposed solution, required resources, and expected impact.

**Step 2 -- Peer review**
The proposal enters a 7-day review period where any guardian can comment, suggest improvements, or raise objections. The AI mediator facilitates constructive discussion.

**Step 3 -- Refinement**
The author incorporates feedback and publishes a revised version. If there are substantial changes, an additional 3-day comment period opens.

**Step 4 -- Voting**
The proposal goes to a 5-day vote. Each guardian has one vote, weighted by $MATTR level:
- Observer: weight 1
- Guardian: weight 2
- Senior Guardian: weight 3
- Councilor: weight 5

**Step 5 -- Approval threshold**
Requires 30% participation of active guardians and 60% approval of weighted votes.

**Step 6 -- Implementation**
If approved, an implementation team is assigned and a timeline established. Progress is reported weekly on the dashboard.

**Step 7 -- Evaluation**
30 days after implementation, an impact evaluation is conducted and a report published for the community.

---

## 4. Privacidad y Soberania

### Principios fundamentales

**Soberania de datos**: Tus datos personales nunca salen de la infraestructura controlada por la comunidad. No existen servidores de terceros que almacenen informacion identificable.

**Identidad auto-soberana (SSI)**: Tu identidad digital esta bajo tu control exclusivo. Utilizamos DIDs (Decentralized Identifiers) que no dependen de ninguna autoridad central. Tu puedes decidir que informacion compartir y con quien.

**Cifrado de extremo a extremo**: Todas las comunicaciones dentro de Matrix estan cifradas con el protocolo Olm/Megolm. Ni siquiera los administradores del servidor pueden leer los mensajes.

**Handshake DNS**: La red utiliza Handshake para resolucion de nombres de dominio, eliminando la dependencia de ICANN y los registradores tradicionales. Esto significa que los dominios .ierahkwa no pueden ser censurados ni confiscados.

**Almacenamiento local primero**: El navegador utiliza IndexedDB para almacenar datos localmente. La sincronizacion con la red ocurre unicamente cuando hay conectividad y el guardian lo autoriza.

**Anonimato por defecto**: Las interacciones en la red no requieren nombres reales. Los seudonomos basados en DID son la norma. La verificacion de identidad real solo ocurre en contextos de gobernanza donde es necesaria.

**Derecho al olvido**: Cualquier guardian puede solicitar la eliminacion completa de sus datos en cualquier momento. El proceso se ejecuta en 48 horas y es verificable en el blockchain.

---

## Privacy and Sovereignty

### Fundamental Principles

**Data sovereignty**: Your personal data never leaves community-controlled infrastructure. No third-party servers store identifiable information.

**Self-sovereign identity (SSI)**: Your digital identity is under your exclusive control. We use DIDs (Decentralized Identifiers) that do not depend on any central authority. You decide what information to share and with whom.

**End-to-end encryption**: All communications within Matrix are encrypted with the Olm/Megolm protocol. Not even server administrators can read messages.

**Handshake DNS**: The network uses Handshake for domain name resolution, eliminating dependence on ICANN and traditional registrars. This means .ierahkwa domains cannot be censored or seized.

**Local-first storage**: The browser uses IndexedDB to store data locally. Synchronization with the network only occurs when there is connectivity and the guardian authorizes it.

**Anonymous by default**: Interactions on the network do not require real names. DID-based pseudonyms are the norm. Real identity verification only occurs in governance contexts where necessary.

**Right to be forgotten**: Any guardian can request complete deletion of their data at any time. The process executes in 48 hours and is verifiable on the blockchain.

---

## 5. Referencia Rapida / Quick Reference

### Comandos esenciales / Essential Commands

```
# Verificar estado del nodo / Check node status
curl http://localhost:8080/health

# Ver alertas del Peace Oracle / View Peace Oracle alerts
python scripts/protocols/peace_oracle.py

# Ejecutar test del mediador / Run mediator test
python scripts/protocols/test_mediator.py

# Enviar alerta a guardianes / Send guardian alert
python scripts/protocols/notify_guardians.py 3 "Mensaje de prueba" --region MX-OAX

# Sincronizar kit offline / Sync offline kit
bash scripts/protocols/survival_sync.sh
```

### Contactos de emergencia / Emergency Contacts

Los contactos de emergencia se distribuyen de forma cifrada a traves del protocolo de guardianes. Consulta tu kit offline o el canal Matrix #emergencia para obtener la lista actualizada.

Emergency contacts are distributed encrypted through the guardian protocol. Check your offline kit or the Matrix #emergencia channel for the current list.

### Soporte / Support

- Canal Matrix: #soporte:ierahkwa.org
- LoRa Canal 3: Emergencias
- Wiki: https://wiki.ierahkwa.org/guardian-manual
