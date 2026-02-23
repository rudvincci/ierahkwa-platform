# ⚔️ PRINCIPIO SOBERANO: TODO PROPIO - NADA DE 3ra COMPAÑÍA

```
═══════════════════════════════════════════════════════════════════════
    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
    Office of the Prime Minister
    "Todo propio · Nada de terceros"
═══════════════════════════════════════════════════════════════════════
```

**De qué se trata:** Soberanía. Todo propio. No dependemos de nadie. Seguimos; no perdemos de vista esto.

---

## 🎯 REGLA FUNDAMENTAL

**TODO PROPIO. NADA DE 3ra COMPAÑÍA.**

Toda la plataforma Ierahkwa debe ser:
- **Infraestructura propia** — servidores, redes, nodos
- **Código propio** — implementaciones soberanas
- **Protocolos propios** — sin depender de empresas externas
- **Sin dependencias de terceros** — nada de Google, AWS, Signal, Meta, etc.
- **Sin certificados ajenos** — sin licencias comerciales (CodeCanyon, WoWonder, QuickDate, PlayTube, etc.), sin obligación de certificados SSL de CAs externos; PKI propia o self-signed si se requiere TLS.
- **Nuestros servicios y banco** — APIs, rutas de plataforma, estáticos y banca (BDET, wallet, forex, VIP) se sirven desde el IERAHKWA Futurehead Mamey Node (`RuddieSolution/node/server.js`); no se depende de servicios externos para operar la plataforma ni el banco. Ver `docs/SERVICIOS-NUESTRO-NODE.md`.

---

## ✅ PERMITIDO

| Tipo | Ejemplo |
|------|---------|
| Node.js runtime | crypto nativo, fs, http |
| Software open source sin empresa detrás | Algoritmos públicos, estándares |
| Código escrito en el repo | Todo en soberanos natives |
| Infraestructura self-hosted | Nuestros servidores |

---

## ❌ PROHIBIDO

| Tipo | Ejemplo |
|------|---------|
| APIs de empresas | Google, AWS, Stripe, Twilio, SendGrid |
| Servicios SaaS externos | Firebase, Auth0, etc. |
| Dependencias de empresas | Librerías que llaman a servicios de 3ros |
| Infraestructura ajena | Hosting en AWS/GCP/Azure |
| **Certificados / licencias ajenos** | Licencias CodeCanyon (WoWonder, QuickDate, PlayTube, PixelPhoto, FLAME), API keys obligatorias de terceros, certificados SSL que exijan CA comercial (usar PKI propia o self-signed) |

---

## 🔐 CRIPTOGRAFÍA

- Usar **solo** `crypto` nativo de Node.js
- Algoritmos estándar: AES-256-GCM, X25519 (si disponible)
- Sin librerías npm de terceros para cifrado

---

## 📡 TELECOM / MÓVIL

- Sistema propio de satélites y estaciones
- Cifrado E2E con crypto nativo
- Sin Signal app, WhatsApp, ni servicios externos

---

---

## 🥷 ASIMETRÍA OPERATIVA

Ver `PRINCIPIO-ELLOS-NO-NOS-ENCUENTRAN.md`:

- **Ellos no nos encuentran.** Infraestructura invisible, Ghost Mode, sin registros públicos.
- **Nosotros sí los encontramos.** Face propio, watchlist, safety link, vigilancia.

---

---

## 📜 SIN CERTIFICADOS

La plataforma opera **sin certificados ni licencias de terceros**:
- No se compran ni se dependen de licencias comerciales (WoWonder, QuickDate, PlayTube, PixelPhoto, FLAME, temas CodeCanyon, etc.).
- No es obligatorio usar certificados SSL emitidos por CAs comerciales; se puede usar PKI propia o certificados autofirmados.
- Las actualizaciones y el mantenimiento son propios; no se depende de “updates” de productos de pago.

*Recuerda: Soberanía = independencia total. Todo propio. Sin certificados ajenos.*
