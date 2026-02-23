# Voz AI y campañas de llamadas — TODO PROPIO (sin IntelliCall)

**Sovereign Government of Ierahkwa Ne Kanienke**  
Principio: **Todo propio · Nada de 3ra compañía**

---

## Por qué NO usamos IntelliCall AI (CodeCanyon)

IntelliCall AI es un producto comercial que depende de:

| Servicio externo | Uso en IntelliCall | Regla nuestro |
|------------------|--------------------|----------------|
| **Twilio** | WebSockets, voz, llamadas | ❌ Prohibido |
| **ElevenLabs** | TTS (voz sintética) | ❌ Prohibido |
| **Deepgram** | STT (reconocimiento de voz) | ❌ Prohibido |
| **OpenAI / OpenRouter** | LLM (lógica de conversación) | ❌ Prohibido |
| **Google OAuth** | Login | ❌ Prohibido |

El principio soberano (**PRINCIPIO-TODO-PROPIO.md**) exige: sin certificados ni licencias ajenas (incluido CodeCanyon), sin APIs de empresas (Twilio, ElevenLabs, etc.), y que **nuestros servicios y banco** estén en nuestro Node.

Por tanto **no compramos ni desplegamos IntelliCall**. Construimos la capacidad equivalente con **nuestro Node, nuestro VoIP, nuestro banco y (cuando se integre) TTS/STT/LLM propios o self-hosted**.

---

## Qué tenemos en su lugar

### Servicio propio: AI Voice Agents (agentes de voz AI)

- **Ubicación:** `RuddieSolution/node/telecom/ai-voice-agents.js`
- **API:** `/api/v1/telecom/voice-agents/*`
- **UI:** `/platform/ai-voice-agents.html` (dashboard de campañas, leads, resúmenes)

Funcionalidad:

1. **Campañas de llamadas**  
   Crear campañas, importar leads (CSV o JSON), estados (draft, running, paused, completed).

2. **Integración con VoIP propio**  
   Las llamadas se disparan contra nuestro VoIP (`/api/v1/telecom/voip`), sin Twilio.

3. **Integración con nuestro banco (BDET)**  
   - Estado de cuentas vía `/api/v1/bdet/accounts`.
   - Cargo por campaña opcional: registrar transferencia o movimiento en BDET (ej. departamento TEL / campañas).

4. **Resúmenes y scoring de leads**  
   Por llamada: resumen, TL;DR, puntuación de lead. Los datos se guardan en nuestro Node (persistencia en `node/data/` o en memoria según configuración).

5. **TTS / STT / LLM**  
   Hoy: **stub**. Cuando existan módulos propios o self-hosted (Piper, Vosk, Whisper local, LLM local), se enganchan aquí. **Cero dependencia de ElevenLabs, Deepgram u OpenAI.**

---

## Cómo encaja en el Node (sin certificado)

- El Node corre en **HTTP** en el puerto **8545** (o el que definas). No se exige certificado externo.
- Todos los servicios (plataforma, VoIP, **voice-agents**, BDET, AI Hub) se sirven desde el mismo proceso (`RuddieSolution/node/server.js`).
- `status.sh` comprueba el Node en `127.0.0.1:8545`; los voice agents son parte de ese mismo Node.

---

## Resumen

| IntelliCall (CodeCanyon) | Nuestra implementación |
|--------------------------|-------------------------|
| Twilio, ElevenLabs, Deepgram, OpenAI, Google | Node propio, VoIP propio, banco BDET propio, TTS/STT/LLM propio o stub |
| Licencia / certificado comercial | Sin certificado ajeno; código propio |
| Servicios externos | Todo en nuestro Node e integrado a nuestro banco |

Referencias: **PRINCIPIO-TODO-PROPIO.md**, **docs/SERVICIOS-NUESTRO-NODE.md**.
