# Hardening del Nodo 8545 — Sentinela · Smart Contracts · Canario

Sovereign Government of Ierahkwa Ne Kanienke. El puerto 8545 es extremadamente sensible. Si alguien externo lo ve, puede intentar drenar fondos. TODO PROPIO.

---

## 1. Hardening del Nodo (Seguridad de Puerto 8545)

El puerto 8545 es extremadamente sensible. Si alguien externo lo ve, puede intentar drenar fondos.

### Consejo
- **Nunca exponer este puerto a la IP pública.**
- Usar un túnel **SSH** o **Tailscale** para que solo Takoda y su gabinete puedan interactuar con la `/platform` desde dispositivos autorizados.

### Filtro RPC
- Implementar un Proxy (p. ej. **Nginx**) con autenticación de certificado.
- Cada llamada a la API del banco debe requerir una firma digital única.

| Componente | Herramienta | Uso |
|------------|-------------|-----|
| Túnel seguro | SSH, Tailscale | Acceso solo desde dispositivos autorizados — gabinete |
| Proxy RPC | Nginx + mTLS / cert auth | Cada llamada requiere firma digital |

---

## 2. Integración con el Modo Fortaleza — Nodo Centinela

Como el nodo es localhost, ya se dio el primer paso hacia el Air-Gap.

### Acción: Nodo Centinela (Sentry Node)
- El **Nodo Centinela** da la cara al mundo.
- El nodo que maneja la plataforma de gobernanza (`localhost:8545`) se mantiene **detrás de tres capas de firewall**.
- Se comunica solo con pares (peers) de confianza en el Caribe y las Américas.

```
[ Internet ] → [ Nodo Centinela ] → [ Firewall 1 ] → [ Firewall 2 ] → [ Firewall 3 ] → [ localhost:8545 /platform ]
```

---

## 3. Smart Contracts de Gobernanza (El Códice Legal)

En la `/platform` deben desplegarse los contratos que rigen el Registro Civil:

### ERC-725 / ERC-735
- Estándares de **identidad soberana** para que el registro de los custodios sea inmutable.
- ERC-725: identidad descentralizada (claims, keys).
- ERC-735: claims verificables.

### Multi-Sig de Gabinete
- Configurar una billetera **Safe (Gnosis)** donde cualquier movimiento de fondos soberanos requiera la firma de **3 de 5** líderes de las Américas.
- Sin consenso, el nodo no se mueve.

| Componente | Estándar / Herramienta | Uso |
|------------|------------------------|-----|
| Identidad | ERC-725, ERC-735 | Registro Civil inmutable, custodios |
| Fondos | Safe (Gnosis) Multi-Sig | 3/5 líderes — sin consenso, sin movimiento |

---

## 4. Monitoreo con Atabey (Canario de Seguridad)

Conectar la `/platform` al panel de **Wazuh** o **Grafana**.

### Métrica Clave
- Si el **consumo de gas** o el **número de transacciones** en el nodo sube de forma inusual, Atabey debe activar el **Canario de Seguridad**.
- Notificación inmediata de actividad sospechosa en el libro contable.

| Métrica | Umbral | Acción |
|---------|--------|--------|
| Gas inusual | Pico anormal vs baseline | Canario rojo — alerta silenciosa |
| Transacciones inusuales | Pico anormal vs baseline | Canario rojo — alerta silenciosa |

---

## 5. Blindaje Post-Cuántico en el Nodo

Para asegurar las próximas 7 generaciones, el nodo debe migrar de firmas **ECDSA** (vulnerables) a firmas de **Criptografía Basada en Retículos** (Lattice-based).

### Acción
- Investigar implementaciones de **Quantum Resistant Ledger (QRL)** o puentes que protejan los activos contra futuros ataques cuánticos.
- Ya documentado: CRYSTALS-Dilithium, Kyber, SPHINCS+ en `quantum-encryption.js` y `PROGRESO-ESTRATEGICO-2026.md`.

| Componente | Herramienta | Uso |
|------------|-------------|-----|
| Firmas actuales | ECDSA | Vulnerables a cuántica |
| Firmas objetivo | Lattice-based (Dilithium, NTRU, QRL) | Resistentes a cuántica |

---

## Resumen por Pilar

| Pilar | Objetivo |
|-------|----------|
| 1. Hardening 8545 | Túnel SSH/Tailscale, Nginx cert auth — nunca exponer a IP pública |
| 2. Nodo Centinela | Nodo público + nodo gobernanza detrás de 3 firewalls |
| 3. Smart Contracts | ERC-725/735 identidad, Safe 3/5 Multi-Sig fondos |
| 4. Monitoreo Atabey | Wazuh/Grafana — Canario si gas o transacciones inusuales |
| 5. Post-Cuántico | Migrar ECDSA → Lattice-based (QRL, Dilithium) |

---

## Referencias

| Archivo | Descripción |
|---------|-------------|
| [MODO-FORTALEZA-AIRGAP-POSTCUANTICA.md](MODO-FORTALEZA-AIRGAP-POSTCUANTICA.md) | Air-Gap, LoRa, post-cuántica, YubiKey, Honeytokens, Wazuh |
| [PROGRESO-ESTRATEGICO-2026.md](PROGRESO-ESTRATEGICO-2026.md) | Post-quantum CRYSTALS-Dilithium, Kyber |
| [REGISTRO-CIVIL-RED-COMUNICACION-CENTRO-MANDO.md](REGISTRO-CIVIL-RED-COMUNICACION-CENTRO-MANDO.md) | Registro Civil, Nodo Bancario |
| [FIDEICOMISO-BUNKER-GUARDIAN-ECONOMIA.md](FIDEICOMISO-BUNKER-GUARDIAN-ECONOMIA.md) | Contratos sucesión, blockchain |
| [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md) | Sin dependencias externas |
