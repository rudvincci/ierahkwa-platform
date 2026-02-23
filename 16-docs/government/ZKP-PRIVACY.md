# ZKP / Privacidad — Pruebas de conocimiento cero

Placeholder para integración futura con **Mamey.SICB.ZeroKnowledgeProofs**.

## Estado

- **Módulo:** `RuddieSolution/node/modules/zkp-privacy.js`
- **API:** `GET /api/v1/zkp/status`, `POST /prove`, `POST /verify` (501 hasta implementación)
- **Uso actual:** Criptografía post-cuántica disponible en módulo **quantum-encryption** (Kyber, Dilithium, SPHINCS+).

## Objetivo futuro

- Demostrar atributos (ej. “soy mayor de edad”, “tengo licencia”) sin revelar el dato subyacente.
- Integración con SICB/Mamey cuando existan las bibliotecas nativas.

## Referencias

- `RuddieSolution/node/modules/quantum-encryption.js` — Criptografía post-cuántica actual
- Roadmap: `docs/ROADMAP-ALCANCE-Y-NECESIDADES.md` (Cumplimiento · ZKP)
