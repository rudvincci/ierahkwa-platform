# Cinco proyectos de IA generativa para portafolio

**Sovereign Government of Ierahkwa Ne Kanienke** · Referencia para ML y ciencia de datos  
Proyectos que mejoran el portafolio y muestran dominio de herramientas recientes. En Ierahkwa priorizamos versiones **todo propio** (sin APIs de terceros cuando sea posible).

---

## Resumen por proyecto

| # | Proyecto | Herramientas | Versión soberana (todo propio) |
|---|----------|-------------|----------------------------------|
| 1 | **StableSAM** | Segment Anything (Meta), Stable Diffusion Inpainting, Hugging Face, Gradio | ✅ Todo open source y local; sin cambios. |
| 2 | **Alpaca-LoRA** | LLaMA, LoRA, Stanford Alpaca, Gradio / alpaca.cpp | ✅ Ya self-hosted; encaja con Ollama/modelos locales. |
| 3 | **Chat con PDF** | LangChain, embeddings, vector DB, LLM | Sustituir OpenAI por **Ollama + embeddings locales** (ver abajo). |
| 4 | **Asistente de voz** | Whisper, TTS, palabra de activación, LLM | Whisper y TTS propios ✅; LLM: **Ollama** en lugar de OpenAI/EdgeGPT. |
| 5 | **Proyecto integral (préstamos)** | ChatGPT para código + Gradio + HF Spaces | Usar **Ollama/ai-soberano** para generación de código y clasificador; deploy en nuestro servidor. |

---

## 1. StableSAM: Stable Diffusion Inpainting + Segment Anything

**Objetivo:** App que cambia fondo, cara, ropa o cualquier zona seleccionada a partir de imagen + área + prompt.

**Stack (todo abierto/local):**
- **Segment Anything (SAM)** — Meta, código abierto.
- **Stable Diffusion 2 Inpainting** — stabilityai/stable-diffusion-2-inpainting (Hugging Face).
- **Diffusers** (Hugging Face) + **Gradio** para la UI.

**Pipeline:**
1. Pipeline de inpainting con diffusers y pesos de Hugging Face; `.to("cuda")` para GPU.
2. Función de máscara con SAM Predictor: imagen + zona seleccionada + `is_background` → máscara y segmentación.
3. Función de inpainting: pipeline + imagen, máscara, prompt y prompt negativo.
4. UI Gradio: fila con tres bloques de imagen, botón de envío; imagen de entrada con selección de píxeles → máscara → inpainting.

**Versión mejorada:** StableSAM en Hugging Face con ControlNet y Runway ML Stable Diffusion Inpainting.

**Recursos:** Demo [StableSAM en Hugging Face](https://huggingface.co/spaces) · Tutorial YouTube: Stable Diffusion Inpainting con SAM.

---

## 2. Alpaca-LoRA: chatbot tipo ChatGPT con recursos mínimos

**Objetivo:** Chatbot especializado tipo ChatGPT con una sola GPU (o CPU / Raspberry Pi 4 GB con alpaca.cpp).

**Stack:**
- **LLaMA** (base) + **LoRA** + dataset **Stanford Alpaca**.
- **Gradio** o **Alpaca-LoRA-Serve** para interfaz tipo ChatGPT.

**Pasos:**
1. **Clonar:** `tloen/alpaca-lora`; `pip install -r requirements.txt` (bitsandbytes desde fuente si hace falta).
2. **Entrenamiento:** `finetune.py` con `--base_model`, `--data_path`, `--output_dir` (ajustar hiperparámetros en el repo).
3. **Inferencia:** `generate.py` con `--load_8bit`, `--base_model`, `--lora_weights`; o **alpaca.cpp** para CPU/Raspberry Pi.
4. **Servir:** Alpaca-LoRA-Serve para UI estilo ChatGPT.

**En Ierahkwa:** Usar mismo flujo con modelos y pesos locales; o exponer vía **Ollama** si hay imagen compatible. Ver `docs/ALTERNATIVAS-GPT4-CODIGO-ABIERTO.md` y `docs/MODELOS-LLM-COMPARACION.md`.

**Recursos:** GitHub tloen/alpaca-lora · Model card alpaca-lora-7b · Demo Alpaca-LoRA-Serve.

---

## 3. Automatización de interacción con PDF (tipo ChatPDF)

**Objetivo:** Chatbot que responde sobre un PDF (libro, documentación legal, etc.) usando RAG (retrieval + LLM).

**Stack original:** LangChain (PyPDFLoader, OpenAIEmbeddings, Chroma), GPT-3.5.

**Versión soberana (todo propio):**
- **Carga:** LangChain `PyPDFLoader` (igual).
- **Embeddings:** En lugar de `OpenAIEmbeddings`, usar **modelos locales**: p. ej. `sentence-transformers` (Hugging Face, local) o integración LangChain con embedding model self-hosted.
- **Vector DB:** Chroma (local) igual.
- **LLM:** En lugar de `OpenAI(model_name="gpt-3.5-turbo")`, usar **Ollama** vía LangChain (Ollama integration) o llamada directa a `ai-soberano` (ver `RuddieSolution/node/services/ai-soberano.js`).

Ejemplo de flujo local con LangChain + Ollama:
- `from langchain.llms import Ollama` (o equivalente) para el chat.
- Cadena: `ChatVectorDBChain.from_llm(Ollama(...), vectordb, ...)`.

**UI:** Gradio para exponer el chatbot.

**Recursos:** Tutorial “Hablar con un PDF” con LangChain; adaptar a Ollama/embeddings locales.

---

## 4. Asistente de voz (tipo Bing-GPT)

**Objetivo:** Asistente personal por voz: palabra de activación → transcripción → LLM → TTS → audio.

**Stack original:** OpenAI API, Whisper, TTS (p. ej. boto3/polly), EdgeGPT/ChatGPT.

**Versión soberana:**
- **Transcripción:** **Whisper** (open source) — sin cambios, local.
- **TTS:** Biblioteca de texto a voz **local** (p. ej. piper, coqui TTS, o similar) en lugar de Polly/APIs externas.
- **LLM:** **Ollama** + `ai-soberano` en lugar de OpenAI/EdgeGPT.
- **Palabra de activación y orquestación:** Código propio; sin depender de Bing/OpenAI.

Flujo: mic → Whisper → texto → si palabra de activación → prompt a Ollama → respuesta → TTS local → audio.

**Recursos:** GitHub del asistente original; reimplementar backend con Ollama y TTS local. Ver `docs/MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md` (sin APIs de terceros).

---

## 5. Proyecto integral de ciencia de datos (clasificador de préstamos)

**Objetivo:** Pipeline completo: EDA, ingeniería de características, preprocesamiento, selección de modelo, hiperparámetros, evaluación y app web (Gradio).

**Stack original:** ChatGPT para generar código + Gradio + deploy en Hugging Face Spaces.

**Versión soberana:**
- **Generación de código y sugerencias:** Usar **Ollama** (modelo 6 o 9 según `ai-soberano`) o **AI Studio** interno (`/api/ai-studio`, `/api/ai/code`) en lugar de ChatGPT.
- **Ejecución:** Python local (pandas, scikit-learn, etc.) — sin cambios.
- **App Gradio:** Igual; desplegar en **nuestro servidor** o en Hugging Face Spaces si es solo para portafolio (datos no sensibles).
- **Prompts:** Misma idea de prompts por fases (planificación, EDA, features, preprocesamiento, modelo, tuning, app); el LLM local sustituye a ChatGPT.

Fases: planificación → EDA → ingeniería de características → preprocesamiento y balanceo → selección de modelo → tuning y evaluación → app Gradio → deploy.

**Recursos:** Guías de “ChatGPT en ciencia de datos”; sustituir por Ollama/ai-soberano. Demo clasificador de préstamos en Gradio.

---

## Conclusión

- **StableSAM y Alpaca-LoRA** son ya compatibles con “todo propio” (modelos y datos locales).
- **Chat PDF, asistente de voz y proyecto integral:** sustituir OpenAI/ChatGPT/EdgeGPT por **Ollama + ai-soberano** y embeddings/TTS locales; LangChain y Gradio se mantienen.
- Herramientas útiles: **Stable Diffusion Inpainting, Segment Anything, LoRA, LangChain, Gradio, Whisper** — todas utilizables en versión open source y self-hosted.
- Referencias en el repo: `PRINCIPIO-TODO-PROPIO.md`, `MODELOS-LLM-COMPARACION.md`, `ALTERNATIVAS-GPT4-CODIGO-ABIERTO.md`, `MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md`, `RuddieSolution/node/services/ai-soberano.js`.
