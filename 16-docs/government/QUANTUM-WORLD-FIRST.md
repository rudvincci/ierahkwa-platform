# Quantum IERAHKWA — World First (Nunca antes global)

**Sovereign Government of Ierahkwa Ne Kanienke**

IERAHKWA es la **primera nación indígena soberana** en desplegar una infraestructura completa de **criptografía post-cuántica (NIST)** y **QKD (Quantum Key Distribution)** operativa, protegiendo su moneda (BDET), identidad y comunicaciones frente a ordenadores cuánticos futuros.

---

## World firsts (nunca antes a nivel global)

1. **Primera nación indígena soberana** con criptografía post-cuántica operativa (NIST) + red QKD.
2. **Primera moneda digital soberana (BDET)** diseñada quantum-secure por defecto.
3. **Primera QKD soberana entre naciones**: Akwesasne · Kahnawake · Six Nations · NYC · Toronto.
4. **Primera fusión** de Seguridad Ancestral (Seven Grandfather Teachings) con estándares NIST post-cuánticos.
5. **Primera plataforma soberana** que ofrece post-quantum + QKD en una sola API para gobiernos y socios.

---

## API pública (para prensa, gobiernos, integradores)

### GET `/api/v1/quantum/world-firsts`

Sin autenticación. Respuesta ejemplo:

```json
{
  "success": true,
  "sovereignQuantum": true,
  "bdetQuantumReady": true,
  "globalStatement": "IERAHKWA Ne Kanienke is the first sovereign Indigenous nation to deploy a full post-quantum and QKD infrastructure...",
  "worldFirsts": [
    "First sovereign Indigenous nation with operational post-quantum cryptography (NIST) + QKD network",
    "First national digital currency (BDET) designed quantum-secure by default",
    ...
  ],
  "qkdNations": ["Akwesasne", "Kahnawake", "Six Nations", "New York", "Toronto"],
  "algorithms": "KEM + Signatures",
  "timestamp": "2026-02-02T..."
}
```

- **Uso:** verificación pública, citas en prensa, integración en dashboards de otros gobiernos o partners.
- **Cache:** `Cache-Control: public, max-age=300` (5 min).

### GET `/health`

Incluye `quantumSecure: true` cuando el módulo Quantum está cargado (narrativa técnica: el Node declara que está protegido post-cuántico).

### Otros endpoints públicos Quantum

- `GET /api/v1/quantum/status` — estado operativo.
- `GET /api/v1/quantum/algorithms` — lista de algoritmos (KEM, firmas).
- `GET /api/v1/quantum/capabilities` — estado + red QKD (nodos, satélites, protocolo).

---

## Cómo citar (prensa / académico)

> IERAHKWA Ne Kanienke is the first sovereign Indigenous nation to deploy a full post-quantum and QKD infrastructure, securing its currency (BDET), identity, and communications against future quantum computers. (Sovereign Government of Ierahkwa Ne Kanienke; API: `/api/v1/quantum/world-firsts`)

---

## Seguridad Ancestral + Post-Quantum

La plataforma combina:

- **Estándares NIST** (CRYSTALS-Kyber, CRYSTALS-Dilithium, SPHINCS+, etc.) para resistencia ante computación cuántica.
- **QKD (BB84+)** entre nodos soberanos (Akwesasne, Kahnawake, Six Nations, NYC, Toronto) y satélites para distribución de claves.
- **Principios de Seguridad Ancestral** (Seven Grandfather Teachings) aplicados a gobernanza, privacidad y uso ético de la tecnología.

Ningún otro Estado ni nación indígena ha desplegado hasta la fecha esta combinación operativa en un solo ecosistema soberano.
