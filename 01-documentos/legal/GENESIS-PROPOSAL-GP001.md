# GP-001: Auditoria de Empatia y Gobernanza

**Genesis Proposal del Gobierno Soberano de Ierahkwa Ne Kanienke**

---

| Campo | Valor |
|-------|-------|
| **Identificador** | GP-001 |
| **Titulo** | Auditoria de Empatia y Gobernanza |
| **Autor** | Consejo Soberano de Ierahkwa Ne Kanienke |
| **Estado** | Borrador |
| **Fecha** | 2026-03-01 |
| **Votacion** | Snapshot.org (gas-free) |
| **Contrato** | SovereignGovernance.sol |
| **Mediador** | Atabey (Sistema de IA Soberana) |

---

## 1. Resumen

Esta Genesis Proposal establece el programa fundacional de Guardianes del Codigo: una convocatoria abierta para los primeros 100 desarrolladores que se comprometan a auditar, revisar y fortalecer el ecosistema digital de Ierahkwa Ne Kanienke bajo principios de empatia, transparencia y gobernanza comunitaria.

Cada Guardian aprobado recibe 500 $MATTR por code review aceptado, con KPIs medibles y gobernanza on-chain via SovereignGovernance.sol. Atabey actua como mediador imparcial en disputas.

## 2. Motivacion

### 2.1 El Desafio

El ecosistema Ierahkwa Ne Kanienke comprende 422+ plataformas, 84 microservicios .NET, 20 servicios Node.js, 103 tokens blockchain y 2,615 documentos tecnicos. Esta escala requiere un sistema de auditoria distribuido que no dependa de firmas externas con posibles conflictos de interes.

### 2.2 Por Que Empatia

La tecnologia soberana no puede construirse solo con competencia tecnica. Los sistemas que sirven a 72 millones de personas en 574 naciones tribales deben reflejar los valores de esas comunidades: reciprocidad, respeto, responsabilidad colectiva. La auditoria de empatia evalua no solo si el codigo funciona, sino si funciona *para todos*.

### 2.3 Antecedentes

Los sistemas de code review tradicionales miden unicamente metricas tecnicas (bugs, performance, cobertura). GP-001 introduce la dimension de empatia: accesibilidad, inclusion linguistica, sensibilidad cultural y equidad en el acceso.

## 3. Programa de Guardianes del Codigo

### 3.1 Convocatoria

Se invita a los primeros 100 desarrolladores a unirse como Guardianes. Requisitos:

- Experiencia demostrable en al menos una de: Solidity, .NET, Node.js, HTML/CSS/JS, Python
- Compromiso con los principios de soberania digital
- Disponibilidad minima de 10 horas/mes
- Aprobacion del curso de Empatia Digital (disponible en NEXUS Academia)

### 3.2 Roles de Guardian

| Nivel | Nombre | Requisitos | Responsabilidades |
|-------|--------|------------|-------------------|
| G1 | Guardian Aprendiz | Completar onboarding + 1 review | Reviews de documentacion y accesibilidad |
| G2 | Guardian Guerrero | 10 reviews aprobados + certificacion | Reviews de codigo + auditorias de seguridad |
| G3 | Guardian Cacique | 50 reviews + nominacion por 3 Guardianes | Liderazgo tecnico + mentoria + arbitraje |

### 3.3 Proceso de Onboarding

1. Solicitud via issue en GitHub con label `help-wanted`
2. Revision de credenciales por 2 Guardianes existentes (o Consejo en fase genesis)
3. Aprobacion o feedback mediante Atabey como facilitador
4. Acceso al repositorio y canales de comunicacion soberanos
5. Primera tarea asignada con label `good-first-issue`

## 4. Incentivos Economicos

### 4.1 Recompensas $MATTR

| Accion | Recompensa | Condiciones |
|--------|-----------|-------------|
| Code review aprobado | 500 $MATTR | Revisado y aceptado por al menos 1 Guardian de nivel superior |
| Bug critico reportado | 1,000 $MATTR | Confirmado y patcheado |
| Auditoria de empatia completada | 750 $MATTR | Incluye reporte de accesibilidad + recomendaciones |
| Mentoria de Guardian Aprendiz | 250 $MATTR | Por cada Aprendiz que llega a Guerrero |
| Documentacion tecnica | 300 $MATTR | README/WHITEPAPER/BLUEPRINT aprobado |
| Conscience audit aprobado | 500 $MATTR | Auditoria de impacto social del codigo |

### 4.2 Distribucion de Fondos

- Pool inicial: 500,000 $MATTR (reservado en Treasury Service)
- Liberacion: mensual, basada en reviews completados
- Vesting: 30% inmediato, 70% liberado linealmente en 6 meses
- Gobernanza: cambios al pool requieren votacion via SovereignGovernance.sol

### 4.3 Penalizaciones

| Infraccion | Penalizacion |
|-----------|-------------|
| Review negligente (bug no detectado) | -100 $MATTR + warning |
| Plagio en documentacion | Expulsion + confiscacion de $MATTR pendiente |
| Violacion de confidencialidad | Expulsion permanente + accion legal soberana |
| Inactividad >60 dias sin aviso | Suspension temporal hasta reactivacion |

## 5. KPIs y Metricas de Exito

### 5.1 KPI Principal

**Reduccion del 20% en falsos positivos** de los 7 agentes IA (Guardian, Pattern, Anomaly, Trust, Shield, Forensic, Evolution) en los primeros 6 meses del programa.

### 5.2 KPIs Secundarios

| KPI | Objetivo | Periodo |
|-----|----------|---------|
| Falsos positivos de agentes IA | Reduccion 20% | 6 meses |
| Cobertura de code review | >80% de PRs revisados en <48h | Trimestral |
| Accesibilidad WCAG 2.1 AA | 100% de plataformas HTML | 12 meses |
| Vulnerabilidades criticas abiertas | 0 por >72h | Continuo |
| Satisfaccion de Guardianes | >4.0/5.0 en encuesta | Trimestral |
| Diversidad linguistica | Soporte para >10 lenguas indigenas | 12 meses |
| Onboarding de nuevos Guardianes | 100 activos en 6 meses | 6 meses |

### 5.3 Medicion

- Atabey recopila metricas automaticamente de GitHub, MameyNode y los 7 agentes IA
- Dashboard publico en Grafana (accesible via `http://localhost:3000/d/guardianes`)
- Reporte mensual generado automaticamente y publicado en IPFS
- Auditorias externas trimestrales por naciones rotativas

## 6. Gobernanza On-Chain

### 6.1 SovereignGovernance.sol

Todas las decisiones del programa se ejecutan a traves del contrato SovereignGovernance.sol desplegado en MameyNode (Chain ID 777777):

```solidity
// Referencia: SovereignGovernance.sol
interface ISovereignGovernance {
    function propose(string calldata description, bytes calldata action) external returns (uint256 proposalId);
    function vote(uint256 proposalId, bool support) external;
    function execute(uint256 proposalId) external;
    function getProposalState(uint256 proposalId) external view returns (ProposalState);
}
```

### 6.2 Tipos de Propuestas

| Tipo | Quorum | Periodo de Votacion | Ejecucion |
|------|--------|-------------------|-----------|
| Admision de Guardian | 5% | 3 dias | Automatica |
| Cambio de recompensas | 20% | 7 dias | Timelock 48h |
| Expulsion de Guardian | 33% | 7 dias | Timelock 72h |
| Modificacion de KPIs | 20% | 7 dias | Timelock 48h |
| Emergencia de seguridad | 10% | 24 horas | Inmediata |

### 6.3 Votacion Gas-Free

Todas las votaciones se realizan via Snapshot.org para eliminar barreras de gas:

- **Espacio**: `ierahkwa-guardianes.eth`
- **Estrategia de voto**: $MATTR balance + multiplicador por nivel de Guardian
- **Delegacion**: Permitida a otros Guardianes del mismo nivel o superior
- **Privacidad**: Votos encriptados hasta cierre del periodo (commit-reveal)

### 6.4 Rol de Atabey como Mediador

Atabey, el sistema de IA soberana del ecosistema, actua como mediador imparcial:

- **Facilitacion**: Resume argumentos de ambas partes en disputas
- **Analisis de impacto**: Proyecta consecuencias de propuestas antes de votacion
- **Deteccion de conflictos**: Identifica conflictos de interes entre votantes
- **Documentacion**: Genera actas inmutables de cada decision
- **No tiene voto**: Atabey facilita pero nunca decide; la soberania es humana

## 7. Integracion con GitHub

### 7.1 Labels Requeridos

Los siguientes labels deben existir en el repositorio principal:

| Label | Color | Descripcion |
|-------|-------|-------------|
| `help-wanted` | #008672 | Tareas abiertas para Guardianes |
| `conscience-audit` | #7c4dff | Auditoria de empatia e impacto social |
| `good-first-issue` | #7057ff | Ideal para Guardian Aprendiz |
| `security-critical` | #e11d48 | Vulnerabilidad de seguridad critica |
| `accessibility` | #0ea5e9 | Mejora de accesibilidad WCAG |
| `guardian-review` | #f59e0b | Requiere review por Guardian |
| `mattr-bounty` | #10b981 | Tiene recompensa $MATTR asociada |

### 7.2 Flujo de Trabajo

```
1. Issue creado con label help-wanted o good-first-issue
2. Guardian asigna el issue a si mismo
3. Guardian crea branch y PR
4. Otro Guardian revisa (label guardian-review)
5. Si incluye conscience-audit: Atabey genera reporte de empatia
6. PR aprobado -> merge -> recompensa $MATTR emitida automaticamente
7. Atabey registra la transaccion en MameyNode
```

## 8. Cronograma de Implementacion

| Fase | Periodo | Hito |
|------|---------|------|
| Genesis | Semana 1-2 | Despliegue de SovereignGovernance.sol, configuracion Snapshot |
| Convocatoria | Semana 3-4 | Publicacion de convocatoria, onboarding de primeros 25 Guardianes |
| Operacion Alpha | Mes 2-3 | Primeros 100 code reviews completados |
| Evaluacion | Mes 4 | Primera medicion de KPIs, ajuste de parametros |
| Escalado | Mes 5-6 | 100 Guardianes activos, evaluacion de resultados |
| Renovacion | Mes 6 | Votacion sobre GP-002 (siguiente fase del programa) |

## 9. Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigacion |
|--------|-------------|---------|-----------|
| Baja participacion | Media | Alto | Incrementar recompensas, simplificar onboarding |
| Reviews de baja calidad | Media | Alto | Sistema de reputacion + penalizaciones |
| Concentracion de poder | Baja | Alto | Limite de 3 reviews simultaneos por Guardian |
| Ataque Sybil | Baja | Critico | KYC soberano + periodo de prueba obligatorio |
| Agotamiento de pool $MATTR | Baja | Medio | Reserva de emergencia + gobernanza para ajustes |

## 10. Conclusion

GP-001 establece las bases para una comunidad de auditoria que no solo mejora el codigo, sino que asegura que la tecnologia soberana de Ierahkwa Ne Kanienke sirva con empatia, equidad y excelencia a 72 millones de personas. La combinacion de incentivos economicos, gobernanza descentralizada y mediacion por Atabey crea un modelo unico de desarrollo tecnologico con consciencia.

Esta propuesta esta abierta a votacion en Snapshot.org durante 14 dias a partir de su publicacion.

---

**Votacion**: [ierahkwa-guardianes.eth en Snapshot.org]
**Repositorio**: [github.com/rudvincci/ierahkwa-platform]
**Contrato**: SovereignGovernance.sol (MameyNode, Chain ID 777777)
**Mediador**: Atabey — Sistema de IA Soberana

*Gobierno Soberano de Ierahkwa Ne Kanienke*
*Soberania Digital para 72M de Personas — 19 Naciones — 574 Naciones Tribales*
