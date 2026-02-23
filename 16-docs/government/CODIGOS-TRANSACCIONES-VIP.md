# 📋 SISTEMA DE CÓDIGOS DE TRANSACCIONES
## Sovereign Government of Ierahkwa Ne Kanienke

---

## ⚠️ REGLA PRINCIPAL

| Tipo | ¿Quién pone código? | Ejemplo |
|------|---------------------|---------|
| **ENTRANTE** | El EMISOR (banco, blockchain, sistema) | UETR, TRN, TX Hash |
| **LOCAL** | NOSOTROS - Código Ierahkwa | OHWISTA-LOC-2601-001 |
| **SALIENTE** | NOSOTROS - Código Ierahkwa | WAMPUM-SAL-2601-001 |

---

## 📥 TRANSACCIONES ENTRANTES (INCOMING)
### Código viene del EMISOR - NO ponemos código

| Transacción | Código del Emisor | Dónde extraerlo |
|-------------|-------------------|-----------------|
| IBAN MT103 Deutsche | TRN + UETR | De 002-MT199.pdf campo :20: |
| STP MT103 CELOS 1B | TRN + UETR | De los 16 PDFs campo :20: |
| STP MT103 CELOS 5B | TRN + UETR | De los 14 PDFs campo :20: |
| SWIFT ACKS UBS | SWIFT Reference | Del PDF campo :20: |
| Venezuela BCV | Referencia BCV | Del PDF de BCV |
| Bonos Históricos | ISIN / CUSIP | Del bono original |
| CryptoHost | TX Hash | De blockchain |
| API to API | API Reference | De los 5 PDFs |
| IP Transfer | IP Transfer ID | Del PDF |
| WISE Port | WISE Transfer ID | De wise.com |
| Visa/Mastercard | Código autorización | Estado de cuenta |

### 📝 Ejemplo de código entrante:
```
Transacción: STP MT103 CELOS 1B
Código: El TRN que aparece en el PDF, ej: "CELOSINVAG2024092701"
UETR: El que da el banco emisor, ej: "eb6305c2-c7d1-4c5e-9542-abcd1234efgh"

¡NO le ponemos código nosotros!
```

---

## 🏠 TRANSACCIONES LOCALES (LOCAL)
### NOSOTROS ponemos código Ierahkwa

| Código Ierahkwa | Transacción | Tipo |
|-----------------|-------------|------|
| `OHWISTA-LOC-2601-001` | Rubí 3 | Activo local |
| `OHWISTA-LOC-2601-002` | Alexandrite | Activo local |

### 📝 Formato código LOCAL:
```
[PREFIJO]-LOC-[AAMM]-[SEQ]

OHWISTA-LOC-2601-001
   │      │    │    │
   │      │    │    └── Secuencia: 001
   │      │    └─────── Año/Mes: Enero 2026
   │      └──────────── Tipo: LOCAL
   └─────────────────── Prefijo: OHWISTA (riqueza/activo)
```

---

## 📤 TRANSACCIONES SALIENTES (OUTGOING)
### NOSOTROS ponemos código Ierahkwa

| Código Ierahkwa | Descripción |
|-----------------|-------------|
| `WAMPUM-SAL-2601-001` | Primera saliente financiera |
| `KANATA-SAL-2601-001` | Primera saliente gobierno |
| `KARIWIIO-SAL-2601-001` | Primera saliente crypto |

### 📝 Formato código SALIENTE:
```
[PREFIJO]-SAL-[AAMM]-[SEQ]

WAMPUM-SAL-2601-001
   │      │    │    │
   │      │    │    └── Secuencia: 001
   │      │    └─────── Año/Mes: Enero 2026
   │      └──────────── Tipo: SALIENTE
   └─────────────────── Prefijo: WAMPUM (transferencia)
```

---

## 🪶 PREFIJOS MOHAWK

| Prefijo | Significado | Uso |
|---------|-------------|-----|
| **WAMPUM** | Cinturón sagrado de valor | Transferencias financieras |
| **OHWISTA** | Riqueza, tesoro | Activos, piedras preciosas |
| **KANATA** | Territorio soberano | Gobierno, bonos soberanos |
| **KARIWIIO** | Mensaje verdadero | Digital, crypto |
| **TEKENI** | Dos, bilateral | Conexiones API |
| **ONKWEHONWE** | Pueblo original | Transacciones ciudadanos |
| **SKENNEN** | Paz | Acuerdos, tratados |

---

## 📊 RESUMEN DE TRANSACCIONES ACTUALES

### ENTRANTES (11) - Código del emisor
```
📥 IBAN MT103 Deutsche      → Extraer TRN/UETR del PDF
📥 STP MT103 CELOS 1B       → Extraer TRN de 16 PDFs
📥 STP MT103 CELOS 5B       → Extraer TRN de 14 PDFs
📥 SWIFT ACKS UBS           → Extraer SWIFT Ref del PDF
📥 Venezuela BCV            → Extraer Ref BCV del PDF
📥 Bonos Históricos         → Extraer ISIN/CUSIP de cada bono
📥 CryptoHost               → Extraer TX Hash de blockchain
📥 API to API               → Extraer Ref de 5 PDFs
📥 IP Transfer              → Extraer ID del PDF
📥 WISE Port                → Extraer WISE ID de wise.com
📥 Visa/Mastercard          → Extraer Auth Code del banco
```

### LOCALES (2) - Código Ierahkwa
```
🏠 OHWISTA-LOC-2601-001     → Rubí 3
🏠 OHWISTA-LOC-2601-002     → Alexandrite
```

### SALIENTES (0) - Código Ierahkwa
```
(Ninguna registrada todavía)

Cuando haya salientes, usar:
📤 WAMPUM-SAL-2601-001      → Primera transferencia saliente
📤 KANATA-SAL-2601-001      → Primera saliente gobierno
```

---

## ✅ PRÓXIMOS CÓDIGOS DISPONIBLES

### Para LOCALES:
```
OHWISTA-LOC-2601-003  → Próximo activo
KANATA-LOC-2601-001   → Primer documento gobierno local
WAMPUM-LOC-2601-001   → Primera reserva local
```

### Para SALIENTES:
```
WAMPUM-SAL-2601-001   → Primera transferencia saliente
KANATA-SAL-2601-001   → Primera saliente soberana
KARIWIIO-SAL-2601-001 → Primera saliente crypto
```

---

## 📁 ESTRUCTURA DE REGISTRO

```
Para cada transacción guardar:

ENTRANTE:
├── Código Original: [TRN/UETR/Hash del emisor]
├── Tipo: INCOMING
├── Fuente: [Nombre del banco/sistema emisor]
└── Extraído de: [Nombre del PDF/documento]

LOCAL:
├── Código Ierahkwa: OHWISTA-LOC-2601-XXX
├── Tipo: LOCAL
├── Creado por: [Nombre del oficial]
└── Fecha registro: [DD/MM/YYYY]

SALIENTE:
├── Código Ierahkwa: WAMPUM-SAL-2601-XXX
├── Tipo: OUTGOING
├── Destino: [Banco/cuenta destino]
├── Creado por: [Nombre del oficial]
└── Fecha registro: [DD/MM/YYYY]
```

---

*Sovereign Government of Ierahkwa Ne Kanienke*
*Sistema de Códigos v2.0 - 21 Enero 2026*
*Nia:wen (Gracias)*
