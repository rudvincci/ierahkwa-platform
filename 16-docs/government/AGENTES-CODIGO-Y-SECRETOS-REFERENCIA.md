# Agentes de código y gestión de secretos — Referencia

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO  
Herramientas de código abierto para agentes de codificación, ingeniería de software autónoma, automatización por GUI y gestión segura de secretos.

---

## Agentes de codificación e ingeniería de software

| Herramienta | Descripción | Uso en Ierahkwa |
|-------------|-------------|------------------|
| **OpenCode** | Agente de codificación open source que funciona desde la terminal. Ayuda a escribir y depurar código de forma **privada** (local). | Encaja con “todo propio”: desarrollo y depuración sin enviar código a servicios externos. |
| **Open SWE** | Agente de ingeniería de software **asíncrono**. Se conecta a GitHub, planifica tareas y realiza **pull requests** de forma autónoma. | Útil para flujos CI/CD y contribuciones automatizadas; usar con repos propios (self-hosted Git si aplica). |
| **Agent S** | Framework para que agentes interactúen con la computadora vía **interfaces gráficas (GUI)**; automatiza tareas complejas en el escritorio. | Automatización de flujos que requieren interacción con ventanas/apps; evaluar si se ejecuta en entorno controlado. |

---

## Gestión de secretos y seguridad

| Herramienta | Descripción | Uso en Ierahkwa |
|-------------|-------------|------------------|
| **Secret-agent (ForgeRock)** | Herramienta open source para **generar y gestionar secretos aleatorios** automáticamente dentro de clústeres **Kubernetes**. | Gestión de secretos en K8s sin depender de soluciones propietarias; alineado con infra propia. |
| **Vault (HashiCorp)** | Solución robusta para **secretos**, cifrado de datos y **acceso privilegiado** de forma segura. Tiene versión open source (Vault OSS). | Referencia estándar para secretos y PKI; se puede desplegar **self-hosted** (Vault OSS) sin usar HashiCorp Cloud. |
| **Agent-security** | Escáner **local** para prevenir la **filtración accidental de datos sensibles** al usar agentes de IA (p. ej. Claude Code, Cursor). | Protege que APIs clave, tokens o datos críticos no se envíen a servicios de IA de terceros; refuerza “nada de 3ra compañía” en flujos con agentes. |

---

## Resumen

- **Codificación privada / local:** OpenCode (terminal, privacidad).
- **Automatización en repos y PRs:** Open SWE (GitHub, tareas, PRs autónomos).
- **Automatización por GUI:** Agent S (escritorio, ventanas).
- **Secretos en Kubernetes:** Secret-agent (ForgeRock).
- **Secretos y acceso privilegiado (self-hosted):** Vault OSS (HashiCorp).
- **Protección frente a fugas con agentes de IA:** Agent-security (escáner local).

Principio: `PRINCIPIO-TODO-PROPIO.md` — preferir versiones open source y despliegue propio; evitar enviar código o secretos a servicios externos. Para gestión de claves en el backend actual ver también `RuddieSolution/node/services/kms.js` y variables de entorno (`.env`, nunca en código).
