# Comparación de modelos LLM abiertos para AI soberana

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO · NADA DE 3ra COMPAÑÍA  
Modelos open source para implementar con Ollama / self-hosted. Sin APIs externas.

---

## Tabla comparativa (9 modelos)

| # | Modelo | Parámetros | Contexto | Fortalezas | GPU típica | Ollama / self-host | Uso sugerido Ierahkwa |
|---|--------|------------|----------|------------|------------|--------------------|------------------------|
| 1 | **GLM 4.6** | — | 200 K | Razonamiento y codificación sólidos; supera GLM-4.5 y DeepSeek-V3.1 | Media–alta | Verificar soporte | Chat, código, ATABEY |
| 2 | **gpt-oss-120B** | 117 B | — | Modelo abierto OpenAI; CoT, niveles de razonamiento; 1 GPU | 1× 80GB (A100) | Verificar | Razonamiento, agentes |
| 3 | **Qwen3-235B-Instruct-2507** | 22 B activos (MoE) | 1 M+ | Razonamiento multilingüe; instrucciones; contexto muy largo | Alta (MoE) | Qwen suele en Ollama | RAG, docs largos, multilingüe |
| 4 | **DeepSeek-V3.2-Exp** | — | — | Atención dispersa; igual rendimiento que V3.1 con mucho menos coste | Menor que V3.1 | DeepSeek en Ollama | Sustituto eficiente de V3.1 |
| 5 | **DeepSeek-R1-0528** | — | — | Razonamiento: matemáticas, lógica, programación (ej. AIME 2025 87,5 %) | Media–alta | DeepSeek en Ollama | Support AI, App Studio, código |
| 6 | **Apriel-1.5-15B-Pensador** | 15 B | — | Multimodal (texto + imagen); vanguardia en 1 GPU | 1× 24GB | Verificar (ServiceNow) | Análisis texto+imagen, una GPU |
| 7 | **Kimi-K2-Instruct-0905** | 1 T (MoE) | 256 K | MoE 1T; flujos codificación y agenciales a largo plazo | Cluster / datacenter | Pesado | Agentes, pipelines largos (si hay cluster) |
| 8 | **Llama-3.3-Nemotron-Super-49B-v1.5** | 49 B | — | RAG y chat con herramientas (NVIDIA) | 1× 80GB o 2× 40GB | Verificar | RAG, Support AI, herramientas |
| 9 | **Modelo pequeño (recomendado)** | 1.5 B–3 B | 4 K–128 K | Rápido; bajo coste; fallback | 1× 8–24 GB | Sí (Phi-3, Qwen2.5, SmolLM) | Tareas ligeras, clasificación, fallback |

---

## Leyenda

- **Parámetros**: total o activos (MoE = Mixture of Experts).
- **Contexto**: ventana en tokens (K = miles, M = millones).
- **Ollama / self-host**: si el modelo suele estar disponible en Ollama o se puede servir en propio servidor.
- **Uso Ierahkwa**: sugerencia para plataforma soberana (ATABEY, Support AI, RAG, etc.).

---

## Cómo usar en la plataforma

- **Variables de entorno**: `OLLAMA_MODEL` o `OLLAMA_MODEL_FULL` (modelo 6, completo), `OLLAMA_MODEL_FAST` (modelo 9, rápido). Ejemplo: `OLLAMA_MODEL=llama3.2`, `OLLAMA_MODEL_FAST=phi3:mini`.
- **Servicio**: `RuddieSolution/node/services/ai-soberano.js` — una sola fuente de verdad: perfiles y asignación por plataforma.
- **API**: `GET /api/ai/profiles` devuelve full/fast; en `POST /api/ai/chat` y `/api/ai/code` se puede enviar `profile` ('full'|'fast') o `platformKey` y se resuelve solo.

## Asignación por plataforma (modelo 6 vs 9)

En `ai-soberano.js`, `getProfileForPlatform(platformKey)` define qué plataforma usa **fast** (modelo 9) y cuál **full** (modelo 6), sin duplicar lógica:

| Perfil | Uso | Plataformas (ejemplos) |
|--------|-----|------------------------|
| **fast** | Respuesta rápida al ciudadano | support-ai, citizen-portal, secure-chat, notifications, dashboard, video-call, meeting-hub |
| **full** | Código, análisis, gobierno | ai-platform, app-studio, editor, government-portal, admin, code, analyze |

El frontend puede enviar `platformKey` en el body (ej. `support-ai`, `citizen-portal`) y el backend elige el modelo; o el ciudadano elige "Rápido" / "Completo" con `profile: 'fast'` / `profile: 'full'`.

---

## Referencia

- Lista anterior: conversación sobre GLM 4.6, gpt-oss-120B, Qwen3-235B, DeepSeek-V3.2-Exp, DeepSeek-R1-0528, Apriel-1.5-15B-Pensador, Kimi-K2-Instruct-0905, Llama-3.3-Nemotron-Super-49B-v1.5.
- **12 alternativas a GPT-4 (código abierto):** ColossalChat, Alpaca-LoRA, Vicuna, GPT4ALL, Raven RWKV, OpenChatKit, OPT, Flan-T5-XXL, Baize, Koala, Dolly, Open Assistant — ver `ALTERNATIVAS-GPT4-CODIGO-ABIERTO.md`.
- Principio: `PRINCIPIO-TODO-PROPIO.md` — sin dependencias de terceros; modelos abiertos self-hosted únicamente.
