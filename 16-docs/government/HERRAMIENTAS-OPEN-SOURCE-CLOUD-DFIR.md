# Herramientas open source: cloud, cumplimiento y DFIR

Referencia de herramientas de código abierto para administración de nube (AWS, Azure, GCP), cumplimiento, respuesta a incidentes (DFIR) y seguridad en repositorios. Complementa la lista unificada en `security-tools-recommended.json` y la documentación de monitoreo/cámaras.

---

## Cloud Custodian

Motor de reglas **sin estado** para administrar entornos AWS, Microsoft Azure y Google Cloud Platform (GCP). Consolida scripts de cumplimiento en una sola herramienta con informes y métricas unificadas. Permite definir reglas que verifiquen el entorno frente a estándares de seguridad, cumplimiento y optimización de costos.

- **Políticas**: YAML; tipo y conjunto de recursos a verificar y acciones sobre ellos (p. ej. cifrado en buckets S3).
- **Integración**: Servicios nativos en la nube y runtimes sin servidor para resolver políticas automáticamente.
- **Origen**: Kapil Thangavelu (Capital One). Proyecto CNCF en incubación.
- **Repositorio**: <https://github.com/cloud-custodian/cloud-custodian>

---

## Cartography

Herramienta de **mapeo de infraestructura** que genera grafos de cómo están conectados los activos en la nube. Mejora la visibilidad de seguridad, informes de activos, identificación de rutas de ataque y áreas de mejora.

- **Stack**: Python, base de datos Neo4j.
- **Cobertura**: AWS, Google Cloud Platform, G Suite.
- **Origen**: Lyft; actualmente bajo CNCF (cartography-cncf/cartography).
- **Repositorios**: <https://github.com/lyft/cartography> · <https://github.com/cartography-cncf/cartography>

---

## Diffy

Herramienta de **clasificación para análisis forense digital y respuesta a incidentes (DFIR)**. Tras un compromiso, ayuda a barrer recursos y destacar valores atípicos en instancias, máquinas virtuales y comportamientos (puertos abiertos, procesos, crontab, módulos de kernel). Utiliza baseline funcional o clustering para identificar outliers.

- **Estado**: En etapa temprana; **deprecated** en el repositorio oficial.
- **Foco**: Principalmente instancias Linux en AWS; arquitectura de plugins para extender a otras nubes.
- **Origen**: Netflix SIRT (Security Intelligence and Response Team). Python, Apache 2.0.
- **Repositorio**: <https://github.com/Netflix-Skunkworks/diffy>

---

## Gitleaks

Herramienta de **prueba de seguridad estática** que escanea repositorios Git en busca de secretos, claves API y tokens. Adecuada para DevSecOps y “shift left”: escaneo en repos privados y a nivel organización; informes JSON y CSV.

- **Uso**: CLI, GitHub Action, pre-commit, integrable en CI/CD.
- **Stack**: Go. Mantenido por Zachary Rice (GitLab).
- **Repositorio**: <https://github.com/gitleaks/gitleaks> · <https://github.com/zricethezav/gitleaks>

---

## Git-secrets

Herramienta de **seguridad en desarrollo** que evita incluir secretos e información confidencial en el repositorio Git. Analiza commits y mensajes de commit y rechaza los que coincidan con patrones de expresiones prohibidas (p. ej. credenciales AWS).

- **Diseño**: Orientado a uso en AWS; creado y mantenido por AWS Labs.
- **Repositorio**: <https://github.com/awslabs/git-secrets>

---

## OSSEC

Plataforma de **seguridad** que combina detección de intrusiones basada en host (HIDS), monitoreo de registros y gestión de eventos e información de seguridad. Útil en entornos locales y en máquinas virtuales en la nube.

- **Cobertura**: AWS, Azure, GCP; múltiples sistemas operativos (Linux, Windows, macOS, Solaris).
- **Características**: Comprobación de integridad de archivos, monitoreo de logs, detección de rootkits, respuesta activa. Servidor de administración centralizado y monitoreo con o sin agentes.
- **Mantenimiento**: Fundación OSSEC.
- **Repositorio principal**: <https://github.com/ossec/ossec-hids>

---

## PacBot (Policy as Code Bot)

Plataforma de **monitoreo de cumplimiento** que implementa políticas como código y compara recursos y activos con esas políticas. Permite informes de cumplimiento automáticos y resolución de infracciones con correcciones predefinidas.

- **Grupos de activos**: Organización de recursos en el panel (p. ej. EC2 por estado) y alcance de acciones por grupo.
- **Cobertura**: AWS y Azure.
- **Origen**: T-Mobile.
- **Repositorio**: <https://github.com/tmobile/pacbot>

---

## Pacu

**Kit de herramientas de prueba de penetración** para entornos AWS. Proporciona módulos de ataque para equipos rojos: compromiso de EC2, pruebas de configuración en buckets S3, alteración de capacidades de monitoreo, etc. Incluye auditoría de ataque para documentación y cronograma de pruebas.

- **Stack**: Python. Mantenido por Rhino Security Labs.
- **Repositorio**: <https://github.com/RhinoSecurityLabs/pacu>

---

## Prowler

Herramienta de **línea de comandos para AWS** que evalúa la infraestructura frente a los benchmarks del AWS Center for Internet Security (CIS) y comprobaciones GDPR e HIPAA. Permite revisar toda la infraestructura o perfiles/regiones concretos; salida en CSV, JSON y HTML; integración con AWS Security Hub.

- **Origen**: Toni de la Fuente (consultor de seguridad AWS).
- **Repositorio**: <https://github.com/prowler-cloud/prowler>

---

## Security Monkey

Herramienta de **monitoreo** de cambios en políticas y configuraciones vulnerables en entornos AWS, GCP y OpenStack. En AWS, alerta sobre cambios en grupos S3, grupos de seguridad y seguimiento de claves IAM, entre otras tareas.

- **Estado**: **Archivado** (Netflix; soporte limitado a correcciones menores). Alternativas de proveedor: AWS Config, Google Cloud Asset Inventory.
- **Repositorio**: <https://github.com/Netflix/security_monkey>

---

## Relación con este proyecto

- La lista unificada de herramientas recomendadas (incluidas las anteriores) está en **`RuddieSolution/node/data/security-tools-recommended.json`**, con `docRef` a monitoreo/cámaras y `docRefCloud` a este documento.
- Para monitoreo de cámaras y dispositivos: **`docs/MONITOREO-CAMARAS-Y-DISPOSITIVOS-REFERENCIA.md`**.
- Para ciberseguridad y alineación con SentinelOne/Aikido: **`docs/CIBERSEGURIDAD-101.md`** y **`RuddieSolution/node/data/ciberseguridad-101.json`**.
