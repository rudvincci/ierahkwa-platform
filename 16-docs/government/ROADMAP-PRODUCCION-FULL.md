# 🚀 ROADMAP — Producción FULL

**Gobierno Soberano de Ierahkwa Ne Kanienke**  
**Objetivo:** Sistema 100% listo para usuarios reales, dinero real, cumplimiento normativo

---

## FASE 1 — Infraestructura (4–6 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 1.1 | **HTTPS / SSL** | Reverse proxy (nginx/Caddy) con certificado Let's Encrypt | ⬜ |
| 1.2 | **Dominio y DNS** | Registrar dominio, apuntar A/AAAA al servidor | ⬜ |
| 1.3 | **Firewall / WAF** | Reglas Fortinet, DDoS, bloqueo bots | ⬜ |
| 1.4 | **Backup automático** | Cron diario + retención 30 días (scripts ya existen) | ⬜ |
| 1.5 | **Monitoreo 24/7** | Prometheus + alertas (ya hay `monitoring/`) | ⬜ |
| 1.6 | **Servidor/VPS** | Dedicado o cloud; min 4GB RAM, 2 CPU | ⬜ |

---

## FASE 2 — Seguridad crítica (6–8 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 2.1 | **JWT / Secrets** | `JWT_ACCESS_SECRET`, `JWT_REFRESH_SECRET` 32+ chars en `.env` | ⬜ |
| 2.2 | **HSM / Llaves** | HSM físico o `Mamey.SICB.TreasuryKeyCustodies` para llaves críticas | ⬜ |
| 2.3 | **Rate limiting** | Ya existe `rate-limit.js` — activar en rutas sensibles | ⬜ |
| 2.4 | **Auditoría** | AuditTrail en operaciones financieras y acceso | ⬜ |
| 2.5 | **Cifrado en reposo** | BD y backups cifrados (AES-256) | ⬜ |

---

## FASE 3 — Identidad y KYC (4–6 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 3.1 | **Biometría** | `Mamey.FWID.Identities` o proveedor soberano | ⬜ |
| 3.2 | **KYC/AML** | KYC actual + reglas AML automáticas | ⬜ |
| 3.3 | **ZKP (Zero Knowledge)** | `Mamey.SICB.ZeroKnowledgeProofs` — privacidad en tx | ⬜ |
| 3.4 | **2FA/MFA** | OTP, SMS soberano o app | ⬜ |

---

## FASE 4 — Blockchain / nodo (8–12 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 4.1 | **MameyNode (Rust)** | Clonar `Mamey-io/MameyNode` — blockchain producción | ⬜ |
| 4.2 | **MameyFramework** | Base .NET para servicios críticos | ⬜ |
| 4.3 | **Bloqueo distribuido** | `MameyLockSlot` — evitar race conditions | ⬜ |
| 4.4 | **SDKs oficiales** | TypeScript, JavaScript para integración unificada | ⬜ |

---

## FASE 5 — Tesorería SICB (6–8 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 5.1 | **TreasuryDisbursements** | Desembolsos controlados | ⬜ |
| 5.2 | **TreasuryIssuances** | Emisión Wampum/SICBDC | ⬜ |
| 5.3 | **TreatyValidators** | Cumplimiento tratados | ⬜ |
| 5.4 | **WhistleblowerReports** | Canal denuncias | ⬜ |

---

## FASE 6 — Hardening del código (2–4 semanas)

| # | Tarea | Acción | Estado |
|---|-------|--------|--------|
| 6.1 | **Tests E2E** | Tests para flujos críticos (login, pago, transferencia) | ⬜ |
| 6.2 | **Errores sin exponer** | No devolver stack traces ni paths internos | ⬜ |
| 6.3 | **Input sanitization** | Validar y sanitizar todos los inputs | ⬜ |
| 6.4 | **Dependencias** | `npm audit fix`, actualizar vulnerabilidades | ⬜ |

---

## Orden recomendado (mínimo viable FULL)

```
1. Fase 1 (Infra)     → Sin esto no hay producción
2. Fase 2 (Seguridad) → Crítico para dinero y datos
3. Fase 6 (Hardening) → Rápido, reduce riesgos
4. Fase 3 (Identidad) → KYC/MFA antes de escalar
5. Fase 4 (Blockchain)→ Escalabilidad real
6. Fase 5 (SICB)      → Nivel banco central
```

---

## Coste estimado (año 1)

| Concepto | Rango |
|----------|-------|
| Infraestructura (servidor, SSL, CDN) | $6,000 – $12,000 |
| Desarrollo (Fases 2–6) | $40,000 – $90,000 |
| Auditoría de seguridad | $15,000 – $30,000 |
| **Total** | **$61,000 – $132,000** |

---

## Repos Mamey-io a integrar (si aplica)

```bash
# Core
gh repo clone Mamey-io/MameyNode
gh repo clone Mamey-io/MameyFramework
gh repo clone Mamey-io/MameyLockSlot

# Identidad
gh repo clone Mamey-io/Mamey.Government.Identity
gh repo clone Mamey-io/Mamey.FWID.Identities
gh repo clone Mamey-io/Mamey.SICB.ZeroKnowledgeProofs

# Tesorería
gh repo clone Mamey-io/Mamey.SICB.TreasuryDisbursements
gh repo clone Mamey-io/Mamey.SICB.TreasuryIssuances
gh repo clone Mamey-io/Mamey.SICB.TreasuryKeyCustodies

# SDKs
gh repo clone Mamey-io/MameyNode.TypeScript
gh repo clone Mamey-io/MameyNode.JavaScript
```

---

*Documento generado: Febrero 2026 — IERAHKWA FUTUREHEAD*
