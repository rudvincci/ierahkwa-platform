# Estándares de cifrado de código abierto y banca abierta

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO  
Referencia: algoritmos abiertos (auditables, sin puertas traseras), APIs cifradas y mensajería segura. Alineado con banca soberana.

---

## 1. Estándares de cifrado de código abierto

El uso de algoritmos abiertos es la norma en banca porque permite que expertos auditen el código en busca de puertas traseras.

| Estándar | Descripción | Uso en Ierahkwa |
|----------|-------------|------------------|
| **AES (Advanced Encryption Standard)** | Estándar global de código abierto. En 2026, **AES-256** sigue siendo el pilar para datos en reposo (bases de datos) y en tránsito. | `RuddieSolution/node/ssl/ssl-config.js` (suites AES-256-GCM), `satellite-system.js`, `quantum-encryption.js`, `secure-chat.html` (AES-256-GCM E2E). |
| **TLS (Transport Layer Security)** | Protocolo abierto que cifra la comunicación navegador–servidor; evita que códigos de transacciones sean interceptados. | `RuddieSolution/node/ssl/ssl-config.js`: TLSv1.2 mínimo, cifrados ECDHE-*-AES256-GCM-SHA384. Certificado propio (sin CA externa). |
| **VeraCrypt** | Software de código abierto para cifrado completo de discos y archivos sensibles; alternativa transparente a soluciones propietarias. | Recomendado para servidores donde se alojan datos bancarios; listado en `node/data/security-tools-recommended.json`. |

---

## 2. Banca abierta y APIs cifradas

Open Banking utiliza protocolos abiertos para que aplicaciones financieras se comuniquen de forma segura.

| Estándar / protocolo | Descripción | Nota Ierahkwa |
|----------------------|-------------|---------------|
| **OAuth 2.0** | Estándar abierto de autorización; permite a una app acceder a datos bancarios sin conocer la contraseña real. | Referencia para futuras APIs de consentimiento; actualmente auth propio (JWT, sesión) en `middleware/jwt-auth.js`, `routes/platform-auth.js`. |
| **JWE (JSON Web Encryption)** | Cifrado del payload de mensajes entre bancos y aplicaciones de terceros (p. ej. Mastercard Open Finance). | Cifrado de payloads con crypto nativo (AES-256-GCM); JWE puede adoptarse para interoperabilidad sin servicios externos. |
| **ISO 20022** | Lenguaje universal para mensajes financieros; adopción para 2026 para interoperabilidad y cumplimiento. | Referenciado en `server.js` (ISO 20022 Messaging), `banking-bridge.js`, `bdet-bank.html`; SWIFT MT/MX compatibles. |

Documentación de banca abierta soberana: `docs/BANCA-ABIERTA-Y-CODIGO-REFERENCIA.md`. API de estado: `GET /api/banking/open-banking`.

---

## 3. Mensajería cifrada (tipo “agente”)

Opciones de código abierto para comunicaciones privadas de alto nivel:

| Herramienta | Descripción | Alineación TODO PROPIO |
|-------------|-------------|-------------------------|
| **Signal** | Protocolo Signal (open source); estándar de oro para cifrado extremo a extremo. | Referencia de protocolo; en plataforma: chat seguro con cifrado E2E (TweetNaCl/Signal-ready) en `server.js`, `secure-chat.html`. |
| **Wickr Me** | Enfocado en efimeridad y protección “grado militar”. | Referencia; no integrado; principio: mensajería propia o protocolos abiertos. |
| **SimpleX Chat** | No requiere identificadores (teléfono); maximiza anonimato. | Referencia; comunicaciones soberanas pueden adoptar modelos sin identificación obligatoria. |

---

## 4. Criptografía post-cuántica

En 2026 los bancos migran hacia algoritmos resistentes a computadoras cuánticas (RSA podría volverse vulnerable). Incluye **computación confidencial** para procesar datos cifrados sin descifrarlos en memoria del servidor.

| Aspecto | Implementación Ierahkwa |
|---------|-------------------------|
| **Post-cuántico** | `RuddieSolution/node/modules/quantum-encryption.js`: Kyber, NTRU, SABER, McEliece; fallback AES-256-GCM. Opcional liboqs. |
| **Defensa en profundidad** | Combinación clásica + post-cuántica; AES-256-GCM ya aporta ~128 bits de seguridad post-cuántica. |

---

## Resumen

- **Cifrado:** AES-256, TLS (certificado propio), VeraCrypt para discos.
- **APIs:** OAuth 2.0 / JWE como referencia; ISO 20022 y SWIFT MT/MX en uso.
- **Mensajería:** Signal/Wickr/SimpleX como referencia; chat E2E propio (Signal-ready).
- **Post-cuántico:** Módulo quantum-encryption (Kyber, etc.) y AES-256-GCM.

Principio: `PRINCIPIO-TODO-PROPIO.md`. Sin dependencias de servicios de cifrado de terceros; solo estándares abiertos y código propio.  
Índice: `docs/INDEX-DOCUMENTACION.md`. CryptoHost y servidores globales: `docs/CRYPTOHOST-Y-SERVIDORES-GLOBALES-REFERENCIA.md`.
