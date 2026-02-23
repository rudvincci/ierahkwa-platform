# 🏛️ MAMEY — Sovereign Platform Unificada

**Akwesasne + Ierahkwa + Mamey = Una Sola Plataforma Soberana**  
**Chain ID: 777777 | Ierahkwa Sovereign Network**

---

## Qué es esto

Plataforma soberana completa que reúne gobierno, blockchain, identidad digital, tesorería, compliance y 60+ sistemas en una estructura organizada, documentada y protegida.

| Sistema | Función |
|---------|---------|
| **Akwesasne** | Sovereign Akwesasne Government — Office of the Prime Minister |
| **Ierahkwa** | Sovereign Government of Ierahkwa Ne Kanienke — 60+ plataformas |
| **Mamey** | Framework técnico — Blockchain, SICB, BIIS, Pupitre, Docker |

---

## Estructura

```
Sovereign Platform Unificada/
├── 00-DOCS/                    ← Toda la documentación
├── 01-PLATAFORMAS-LIMPIO/      ← 15 categorías organizadas
│   ├── 01-Gobierno/
│   ├── 02-Bancos/
│   ├── 03-Identidad/
│   ├── 04-Blockchain/
│   ├── 05-Compliance-ZKP/
│   ├── 06-Tesoreria/
│   ├── 07-AI/
│   ├── 08-Biometria/
│   ├── 09-DeFi/
│   ├── 10-ERP/
│   ├── 11-Mobile/
│   ├── 12-Educacion/
│   ├── 13-Oficina/
│   ├── 14-Infraestructura/
│   └── 15-CRM-Ciudadanos/
├── 02-SEGURIDAD/               ← Firewall, certs, auth, backups
├── 03-SCRIPTS/                 ← Start, stop, health, backup, deploy
├── 04-CONFIG/                  ← Nginx, Docker, variables de entorno
├── 05-MONITORING/              ← Prometheus, Grafana, alertas
├── Akwesasne → (enlace)
├── Ierahkwa → (enlace)
├── Mamey → (enlace)
├── EMPEZAR-AQUI.md
└── README.md (este archivo)
```

---

## Inicio rápido

```bash
# 1. Verificar que todo está bien
./03-SCRIPTS/health/verificar-todo.sh

# 2. Aplicar seguridad (primera vez)
./03-SCRIPTS/security/hardening.sh

# 3. Arrancar todo
./03-SCRIPTS/start/start-mamey.sh

# 4. Ver estado
./03-SCRIPTS/health/health-check.sh

# 5. Parar todo
./03-SCRIPTS/stop/stop-mamey.sh
```

---

## Requisitos

| Dependencia | Versión mínima | Para qué |
|-------------|---------------|----------|
| macOS | 13+ | Sistema operativo |
| .NET SDK | 8.0+ | Servicios Identity, ZKP, Treasury |
| Rust | 1.70+ | MameyNode blockchain |
| Node.js | 18+ | Servidor legacy, herramientas |
| Docker | 24+ | Infraestructura |
| nginx | 1.24+ | Reverse proxy + TLS |
| openssl | 3.0+ | Certificados |

---

## Servicios

| Servicio | Puerto interno | Acceso seguro | Función |
|----------|---------------|---------------|---------|
| MameyNode | 8545 | https://localhost/chain | Blockchain EVM |
| Identity | 5001 | https://localhost/identity | Identidad ciudadana |
| ZKP | 5002 | https://localhost/compliance | Zero-Knowledge Proofs |
| Treasury | 5003 | https://localhost/treasury | Tesorería SICB |
| Dashboard | — | https://localhost/platform | Panel de control |
| Membership | — | https://localhost/membership | Membresía |
| Banks | — | https://localhost/banks | Bancos centrales |

Todos los servicios pasan por nginx con TLS. Nunca se accede directamente a los puertos internos.

---

## Tokens

| Token | Función |
|-------|---------|
| **WAMPUM** | Token nativo de la red soberana |
| **SICBDC** | Moneda digital del banco central soberano |
| **IGT** | Token de gobierno (103 departamentos) |

---

## Documentación completa

→ [00-DOCS/](00-DOCS/) contiene toda la documentación:
- EMPEZAR-AQUI.md — Guía de inicio
- ARQUITECTURA.md — Diseño del sistema
- SEGURIDAD.md — Políticas de seguridad
- SISTEMA-BANCARIO.md — Sistema bancario indígena
- DISASTER-RECOVERY.md — Recuperación ante desastres
- AUDITORIA.md — Última auditoría

---

## Contacto

**Sovereign Government of Ierahkwa Ne Kanienke**  
Mantenedor: Ruddie  
Plataforma: Mamey Ecosystem v1.0
