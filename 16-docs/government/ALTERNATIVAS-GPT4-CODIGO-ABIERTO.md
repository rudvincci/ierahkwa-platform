# 12 alternativas a GPT-4 de código abierto

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO · NADA DE 3ra COMPAÑÍA  
GPT-4 no es abierto: no hay acceso al código, arquitectura, datos ni pesos. Estas alternativas permiten chatbots y LLM self-hosted sin depender de OpenAI.

**Importante:** Algunos modelos tienen licencia **no comercial** (solo académico/investigación). Revisar siempre la licencia antes de uso en producción.

---

## Resumen rápido

| # | Modelo | Licencia | Recursos | Uso en Ierahkwa |
|---|--------|----------|----------|------------------|
| 1 | ColossalChat | Abierto | Pipeline RLHF completo, datos bilingües | Chatbot propio, entrenamiento |
| 2 | Alpaca-LoRA | Abierto | 1× RTX 4090, incluso Raspberry Pi 4 GB | Chat instruct ligero |
| 3 | Vicuna (FastChat) | Abierto | ~90 % calidad ChatGPT | Chat, servir, evaluar (FastChat) |
| 4 | GPT4ALL | Abierto | CPU/GPU, cliente Python, LangChain | Chat local, bajo recurso |
| 5 | Raven RWKV | Abierto | RNN, menos VRAM que transformers | Chat eficiente |
| 6 | OpenChatKit | Abierto | Entrenar, afinar, moderación | Chatbot + retrieval |
| 7 | OPT (Meta) | Abierto | 125M–175B params | Base LLM, investigación |
| 8 | Flan-T5-XXL | Abierto | Google Research | Instrucciones, multilingüe |
| 9 | Baize | **No comercial** | Autochat con ChatGPT (datos) | Solo investigación |
| 10 | Koala | Abierto | LLaMa + diálogo | Chat tipo ChatGPT |
| 11 | Dolly (Databricks) | Abierto; Dolly 2.0 **comercial** | 30 min entrenamiento, 6B | Instrucciones, comercial (2.0) |
| 12 | Open Assistant | Abierto | 1 GPU consumo, código/modelos/datos abiertos | Asistente soberano |

---

## 1. ColossalChat

- **Qué es:** Pipeline completo RLHF para clonar modelos tipo ChatGPT: datos bilingües, entrenamiento, demo, inferencia 4-bit.
- **Recursos:** Entrenamiento e inferencia distribuidos.
- **Licencia:** Código abierto.
- **Enlaces:** [Colossal-AI paper](https://arxiv.org/abs/2110.14883) · [Blog ColossalChat](https://colossalai.org/blog/2023/03/30/colossalchat) · [GitHub hpcaitech/ColossalAI](https://github.com/hpcaitech/ColossalAI) · [Demo](https://chat.colossalai.org).

---

## 2. Alpaca-LoRA

- **Qué es:** Instruct similar a GPT-3.5 con LoRA; corre en Raspberry Pi 4 (4 GB RAM). Entrenamiento en pocas horas con 1× RTX 4090.
- **Incluye:** Código, fine-tuning, inferencia, pesos, dataset, demo.
- **Licencia:** Código abierto (ver términos de LLaMA si aplica).
- **Enlaces:** [GitHub tloen/alpaca-lora](https://github.com/tloen/alpaca-lora) · [Model card tloen/alpaca-lora-7b](https://huggingface.co/tloen/alpaca-lora-7b) · [Demo](https://alpaca-lora.github.io).

---

## 3. Vicuna (FastChat)

- **Qué es:** Transformer afinado en diálogos (ShareGPT); ~90 % de la calidad de ChatGPT. Parte de **FastChat**: entrenar, servir y evaluar chatbots.
- **Licencia:** Abierto.
- **Enlaces:** [Blog Vicuna](https://vicuna.lmsys.org) · [GitHub lm-sys/FastChat](https://github.com/lm-sys/FastChat) · [Demo FastChat](https://chat.lmsys.org).

---

## 4. GPT4ALL

- **Qué es:** Chatbot sobre datos curados (verbal, código, historias, diálogo); basado en LLaMa. Inferencia en CPU o GPU, baja latencia. Incluye cliente Python, UI, backend LangChain.
- **Licencia:** Abierto.
- **Enlaces:** [Informe técnico GPT4All](https://arxiv.org/abs/2304.06942) · [GitHub nomic-ai/gpt4all](https://github.com/nomic-ai/gpt4all) · [UI nomic-ai/gpt4all-ui](https://github.com/nomic-ai/gpt4all-ui) · [Model card](https://huggingface.co/nomic-ai/gpt4all-lora).

---

## 5. Raven RWKV (ChatRWKV)

- **Qué es:** Modelo tipo ChatGPT basado en **RWKV (100 % RNN)**, no transformer. Menor uso de VRAM, inferencia rápida. Afinado en Stanford Alpaca, code-alpaca, etc.
- **Licencia:** Abierto.
- **Enlaces:** [GitHub BlinkDL/ChatRWKV](https://github.com/BlinkDL/ChatRWKV) · [Demo Raven RWKV 7B](https://huggingface.co/spaces/BlinkDL/Raven-RWKV-7B) · [Model card BlinkDL/rwkv-4-raven](https://huggingface.co/BlinkDL/rwkv-4-raven).

---

## 6. OpenChatKit

- **Qué es:** Kit completo para alternativa a ChatGPT: entrenar LLM ajustado a instrucciones, fine-tuning, sistema de recuperación para respuestas, moderación.
- **Licencia:** Abierto.
- **Enlaces:** [Blog OpenChatKit (Together)](https://www.together.xyz/blog/openchatkit) · [GitHub togethercomputer/OpenChatKit](https://github.com/togethercomputer/OpenChatKit) · [Demo](https://openchatkit.github.io) · [Model card GPT-NeoXT-Chat-Base-20B](https://huggingface.co/togethercomputer/GPT-NeoXT-Chat-Base-20B).

---

## 7. OPT (Open Pre-trained Transformer, Meta)

- **Qué es:** Familia de LLM 125M–175B parámetros; solo decodificador, texto autorregresivo. No iguala ChatGPT pero útil para zero/few-shot y análisis.
- **Licencia:** Abierto.
- **Enlaces:** [Paper OPT](https://arxiv.org/abs/2205.01068) · [GitHub facebookresearch/metaseq](https://github.com/facebookresearch/metaseq) · [Model card facebook/opt-1.3b](https://huggingface.co/facebook/opt-1.3b).

---

## 8. Flan-T5-XXL

- **Qué es:** T5 afinado con instrucciones (FLAN); >1000 tareas, multilingüe. Mejor rendimiento en muchas tareas respecto a T5 base.
- **Licencia:** Abierto (Google Research).
- **Enlaces:** [Paper FLAN](https://arxiv.org/abs/2210.11416) · [GitHub google-research/t5x](https://github.com/google-research/t5x) · [Model card google/flan-t5-xxl](https://huggingface.co/google/flan-t5-xxl).

---

## 9. Baize

- **Qué es:** Chat multiturno con guardrails; corpus generado con ChatGPT (autochat). Buen rendimiento en diálogo.
- **Licencia:** **No comercial** (solo investigación/académico).
- **Enlaces:** [Paper Baize](https://arxiv.org/abs/2304.01196) · [GitHub project-baize/baize-chatbot](https://github.com/project-baize/baize-chatbot) · [Demo Baize 7B](https://huggingface.co/spaces/project-baize/baize-7b) · [Model card](https://huggingface.co/project-baize/baize-lora-7B).

---

## 10. Koala

- **Qué es:** Chatbot basado en LLaMa con datos de diálogo de Internet; mejor que Alpaca en muchos casos, similar a ChatGPT en varios. Incluye código, pesos, evaluaciones humanas.
- **Licencia:** Abierto (revisar términos LLaMA).
- **Enlaces:** [Blog Koala](https://bair.berkeley.edu/blog/2023/04/03/koala) · [GitHub young-geng/EasyLM](https://github.com/young-geng/EasyLM) · [Demo FastChat/Koala](https://chat.lmsys.org).

---

## 11. Dolly (Databricks)

- **Qué es:** LLM que aprende a seguir instrucciones en ~30 min en una máquina; 6B parámetros. **Dolly 2.0** es apto para **uso comercial**.
- **Licencia:** Abierto; Dolly 2.0 explícitamente comercial.
- **Enlaces:** [Blog Hello Dolly](https://www.databricks.com/blog/2023/03/24/hello-dolly-democratizing-magic-chatgpt-open-models.html) · [GitHub databrickslabs/dolly](https://github.com/databrickslabs/dolly) · [Model card databricks/dolly-v1-6b](https://huggingface.co/databricks/dolly-v1-6b).

---

## 12. Open Assistant

- **Qué es:** Proyecto abierto de asistente conversacional: código, modelos y datos con licencia abierto. Ejecutable en una GPU de consumo. Objetivo: interactuar con sistemas, recuperar información, crear aplicaciones con lenguaje.
- **Licencia:** Código abierto.
- **Enlaces:** [Blog Open Assistant](https://open-assistant.io/blog) · [GitHub LAION-AI/Open-Assistant](https://github.com/LAION-AI/Open-Assistant) · [Demo open-assistant.io](https://open-assistant.io) · [Model card OpenAssistant/oasst-sft-1-pythia-12b](https://huggingface.co/OpenAssistant/oasst-sft-1-pythia-12b).

---

## Uso en Ierahkwa (todo propio)

- **Inferencia en plataforma:** Preferir modelos disponibles en **Ollama** o servibles en nuestro propio servidor (ver `docs/MODELOS-LLM-COMPARACION.md` y `RuddieSolution/node/services/ai-soberano.js`).
- **Alternativas de este doc** que encajan bien self-hosted: Vicuna/FastChat, GPT4ALL, Alpaca-LoRA, Raven RWKV, Dolly 2.0 (comercial), Open Assistant. ColossalChat y OpenChatKit si se quiere pipeline de entrenamiento propio.
- **Evitar en producción** modelos con licencia **solo no comercial** (ej. Baize) salvo uso estrictamente académico/investigación.
- **No usar** APIs de OpenAI/GPT-4 para datos del ciudadano; estas 12 alternativas permiten sustituir con modelos propios.

Referencias: `PRINCIPIO-TODO-PROPIO.md`, `MODELOS-LLM-COMPARACION.md`, `MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md`.
