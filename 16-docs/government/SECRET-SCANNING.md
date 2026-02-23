# Escaneo de secretos (Secret Scanning) — Atabey Fortress

Implementación de **secret scanning** en el repositorio soberano, siguiendo las mejores prácticas descritas en el marco [Ciberseguridad 101](https://www.sentinelone.com/es/cybersecurity-101/) de SentinelOne:

**[Las mejores herramientas de escaneo de secretos para 2025](https://www.sentinelone.com/es/cybersecurity-101/cloud-security/secret-scanning-tools/)** (SentinelOne, Cybersecurity 101)

El escaneo de secretos detecta de forma automática información confidencial (tokens, claves API, credenciales) en código, historial de commits y configuraciones, para evitar exposición involuntaria y acceso no autorizado.

---

## Qué está implementado

| Componente        | Descripción |
|-------------------|-------------|
| **Script local**  | `scripts/secret-scan.sh` — ejecuta Gitleaks (y opcionalmente TruffleHog) sobre el repo. |
| **CI (GitHub)**   | Workflow `.github/workflows/secret-scan.yml` — escaneo en cada push/PR. |
| **Documentación** | Este documento y referencia al artículo SentinelOne. |

### Herramientas usadas

- **Gitleaks** — Código abierto, reglas personalizables, escaneo offline. Detecta secretos en repos y archivos.
- **TruffleHog** (opcional) — Código abierto, 600+ detectores, validación con APIs. Se puede activar en CI o local con variable de entorno.

Otras herramientas mencionadas en el artículo (GitGuardian, Spectral, GitHub Secret Scanning, AWS Secrets Manager, etc.) son alternativas comerciales o específicas de plataforma; para soberanía en tu propio hardware usamos herramientas open source ejecutables en tu entorno.

---

## Uso local

### Requisitos

- **Opción A:** [Gitleaks](https://github.com/gitleaks/gitleaks) instalado (`brew install gitleaks` o binario desde releases).
- **Opción B:** Docker — el script usa la imagen `gitleaks/gitleaks` si no hay binario local.

### Ejecutar

```bash
# Desde la raíz del repo
./scripts/secret-scan.sh

# Sobre un directorio concreto
./scripts/secret-scan.sh ./RuddieSolution/node
```

### TruffleHog opcional

```bash
SECRET_SCAN_TRUFFLEHOG=1 ./scripts/secret-scan.sh
```

(Requiere `trufflehog` en PATH o Docker con imagen `trufflesecurity/trufflehog`.)

---

## Integración en CI (GitHub Actions)

El workflow **Secret Scan** (`.github/workflows/secret-scan.yml`) se ejecuta en:

- `push` a `main` y `develop`
- `pull_request` a `main` y `develop`

Si se detectan secretos, el job puede fallar para bloquear merge (configurable). Los resultados se muestran en la pestaña **Actions**.

---

## Buenas prácticas (según el artículo)

1. **Protección de datos confidenciales** — No subir claves API, tokens ni contraseñas al repo; usar variables de entorno o gestores de secretos.
2. **Mitigación de riesgos** — El escaneo continuo en CI ayuda a identificar fugas antes de que sean explotadas.
3. **Cumplimiento** — Alinear con requisitos de protección de datos (PCI-DSS, HIPAA, SOC 2, etc.) según tu contexto.
4. **Integración CI/CD** — El escaneo en cada push/PR evita que secretos lleguen a producción.

---

## Cómo elegir y ampliar

Según [la guía del artículo](https://www.sentinelone.com/es/cybersecurity-101/cloud-security/secret-scanning-tools/), al evaluar herramientas hay que tener en cuenta:

- **Precisión y fiabilidad** — Menos falsos positivos.
- **Integración** — CI/CD y repos existentes.
- **Supervisión continua** — Escaneo en tiempo real o en cada cambio.
- **Personalización** — Reglas y patrones propios (p. ej. `.gitleaks.toml`).
- **Alertas e informes** — Notificaciones claras para corregir rápido.

Para permitir falsos positivos conocidos (p. ej. ejemplos o claves de prueba), se puede añadir un archivo **`.gitleaks.toml`** en la raíz con reglas de exclusión; ver [Gitleaks Configuration](https://github.com/gitleaks/gitleaks#configuration).

---

## Referencia rápida de herramientas (artículo SentinelOne)

| Herramienta           | Tipo        | Notas |
|-----------------------|------------|--------|
| SentinelOne           | Comercial  | 750+ tipos, CI/CD, CNAPP, Purple AI. |
| Spectral Secrets      | Comercial  | Políticas personalizables, IaC y código. |
| AWS Secrets Manager   | Cloud AWS  | Rotación, cifrado, VPC. |
| GitHub Secret Scanning| GitHub     | Tiempo real en repos GitHub. |
| GitGuardian           | Comercial  | Detección en Git, políticas. |
| **Gitleaks**          | **Open source** | **Usado en este proyecto.** |
| Git-Secrets           | Open source | Cifrado/descifrado de archivos en Git. |
| Whispers              | Open source | Análisis estático, múltiples formatos. |
| HawkScan              | Contenedores | Imágenes y secretos en contenedores. |
| **TruffleHog**        | **Open source** | **Opcional en este proyecto.** |
| Infisical Secret Scanning | Plataforma (cloud/self-hosted) | Detección en código y CI; integración GitHub, GitLab, Bitbucket; ver [Infisical Docs](https://infisical.com/docs/documentation/platform/secret-scanning/overview). |

**Referencia adicional:** [Infisical](https://infisical.com/docs/documentation/platform/project) incluye Secrets Management, Secret Scanning, PKI, KMS, PAM y [Point-in-Time Recovery](https://infisical.com/docs/documentation/platform/pit-recovery) para rollback de secretos a cualquier snapshot (Pro/Enterprise).

---

*Implementación soberana basada en [SentinelOne — Las mejores herramientas de escaneo de secretos para 2025](https://www.sentinelone.com/es/cybersecurity-101/cloud-security/secret-scanning-tools/).*
