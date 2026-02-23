# Marcos y bibliotecas de IA — Referencia para Ierahkwa

**Sovereign Government of Ierahkwa Ne Kanienke** · TODO PROPIO · NADA DE 3ra COMPAÑÍA  
Referencia basada en: marcos/bibliotecas que simplifican el desarrollo de software basado en IA (rentabilidad, tiempo, flujo de desarrollo).

---

## Por qué usar marcos de IA

- **Rentables**: reducen costes al reutilizar componentes y evitar codificación manual.
- **Aceleran el flujo**: algoritmos preimplementados, manejo de datos, optimización; el equipo se centra en el problema de negocio.
- **Ahorro de tiempo**: entornos de desarrollo, depuración, pruebas, visualización; modelos preconstruidos.

## Criterios al elegir (para nuestro contexto)

| Criterio | Importancia para Ierahkwa |
|----------|---------------------------|
| **Rendimiento** | Prioridad: datos eficaces, entrenamiento e inferencia rápidos. |
| **Apoyo comunitario** | Comunidad activa, recursos y plugins; sin depender de soporte comercial. |
| **Flexibilidad** | Probar varios algoritmos; texto, imagen, audio; integrar con nuestro stack (Node, Ollama). |
| **Facilidad de aprendizaje** | Documentación y tutoriales; nivel del equipo. |
| **Código abierto vs comercial** | **Solo código abierto y self-hosted**; sin APIs de empresas (OpenAI, IBM, Google Cloud AI, etc.). |

---

## Clasificación según principio “todo propio”

### ✅ Adecuados (código abierto, sin atarse a un proveedor)

| Marco / biblioteca | Uso principal | Notas |
|--------------------|----------------|-------|
| **PyTorch** | Deep learning, prototipos, investigación | Gráfico dinámico, muy usado; comunidad fuerte. |
| **TensorFlow** | Deep learning, escalabilidad | Open source; flexibilidad; curva de aprendizaje más alta. |
| **Scikit-Learn** | ML clásico, minería de datos | Python, fácil, prototipos rápidos; no es para deep learning pesado. |
| **Keras** | API alto nivel sobre TensorFlow | Fácil de aprender; creación rápida de prototipos. |
| **Hugging Face Transformers** | NLP, modelos preentrenados | Código abierto; modelos locales; sin depender de su API en producción si usamos modelos self-hosted. |
| **LangChain** | Aplicaciones LLM, cadenas, agentes | Útil para orquestar LLMs; **usar solo con Ollama/modelos propios**, no con OpenAI. |
| **XGBoost** | Datos estructurados, clasificación/regresión | Alto rendimiento en tabulares; bien mantenido. |
| **OpenNN** | Redes neuronales (C++) | Rápido, eficiente; investigación y decisiones basadas en datos. |
| **PyBrain** | ML en Python, experimentación | Ligero, educativo; documentación limitada. |
| **MXNet** | Deep learning | Eficiente, escalable; comunidad menor que TF/PyTorch. |
| **Caffe** | Visión por ordenador | Rápido; muy orientado a visión. |
| **DL4J (Deeplearning4j)** | Deep learning en Java/Scala | Entorno JVM; escalable. |
| **Theano** | Computación numérica (Python) | Ya no se mantiene activamente; solo referencia histórica/educativa. |

### ⚠️ Usar solo la parte open source / self-hosted

| Recurso | Qué usar | Qué no usar |
|---------|----------|-------------|
| **Hugging Face** | Biblioteca Transformers, modelos descargados y ejecutados en nuestro servidor | APIs en la nube de Hugging Face que envían datos fuera. |
| **LangChain** | Integración con Ollama y modelos locales | Integración con OpenAI/Anthropic u otros APIs externos. |

### ❌ No alineados con “todo propio” (servicios/APIs de empresas)

| Recurso | Motivo |
|---------|--------|
| **OpenAI (API)** | Servicio en la nube, datos fuera, coste recurrente, dependencia de tercero. |
| **IBM Watson** | Servicios propietarios, IBM Cloud, coste. |
| **Microsoft CNTK** | Open source pero ecosistema Microsoft; valorar solo si ya se usa Azure/Windows en todo. |
| **Google Cloud AI / Vertex** | Servicios en la nube, dependencia de Google. |

Para **LLM y chat** en Ierahkwa usamos **Ollama + modelos abiertos** (ver `docs/MODELOS-LLM-COMPARACION.md` y `RuddieSolution/node/services/ai-soberano.js`), no APIs de OpenAI ni de otras empresas.

---

## Resumen rápido por tipo de tarea

| Necesidad | Opción recomendada (todo propio) |
|-----------|-----------------------------------|
| Deep learning general | PyTorch o TensorFlow (self-hosted) |
| ML clásico / tabular | Scikit-Learn, XGBoost |
| Entrada sencilla a deep learning | Keras sobre TensorFlow |
| NLP / modelos de lenguaje | Hugging Face Transformers (modelos locales) + Ollama para inferencia |
| Aplicaciones LLM (agentes, cadenas) | LangChain conectado solo a Ollama/modelos propios |
| Visión por ordenador | Caffe, o PyTorch/TensorFlow con modelos de visión |
| Java/Scala | DL4J |
| Experimentación / educación | PyBrain, Scikit-Learn |

---

## Conclusión para el proyecto

- **Marcos y bibliotecas**: preferir **código abierto**, **self-hosted** y **sin depender de APIs de empresas**.
- **Inferencia LLM en plataforma**: **Ollama** y modelos de la comparativa (modelo 6 y 9), sin OpenAI/Anthropic.
- El “mejor” marco es el que se ajusta al problema, al stack (Node, Python, JVM) y al principio **todo propio**.

Referencia de principio: `PRINCIPIO-TODO-PROPIO.md`.
