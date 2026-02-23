# Explicación de los modelos generativos

**Sovereign Government of Ierahkwa Ne Kanienke** · Referencia para IA y ciencia de datos  
Los modelos generativos son una piedra angular en IA: aprenden la distribución de los datos y generan datos nuevos con características similares. En Ierahkwa usamos **modelos locales (Ollama, etc.)** en lugar de APIs externas como GPT-4/ChatGPT.

---

## Qué son los modelos generativos

**Función principal:** Comprender y captar los **patrones o distribuciones** de un conjunto de datos y, a partir de ellos, **generar datos nuevos** que compartan características con los originales.

**Analogía:** Enseñar a alguien a dibujar animales a partir de muchas fotos: tras aprender las características, puede dibujar un animal nuevo que no ha visto, combinando lo aprendido. El modelo generativo hace lo mismo con datos (texto, imagen, audio, etc.).

---

## Generativos vs discriminativos

| Aspecto | Generativos | Discriminativos |
|--------|-------------|-----------------|
| **Objetivo** | Aprender **cómo se generan** los datos (la distribución). | Aprender **límites** que separan clases. |
| **Ejemplo (gatos/perros)** | Entienden qué hace que un gato sea gato y un perro perro; pueden **generar** imágenes nuevas de gatos o perros. | **Distinguen** entre gato y perro; no generan imágenes nuevas. |
| **Fortaleza** | **Crear** contenido nuevo, sintetizar, aumentar datos. | **Clasificar**, predecir etiquetas. |

En resumen: los discriminativos destacan en **clasificación**; los generativos en **creación** y en conocimiento profundo de la distribución de datos.

---

## Tipos de modelos generativos

| Tipo | Descripción | Uso típico |
|------|-------------|------------|
| **Redes bayesianas** | Modelos gráficos con relaciones probabilísticas entre variables. | Diagnóstico (p. ej. médico), relaciones causales. |
| **Modelos de difusión** | Modelan cómo algo se propaga o evoluciona en el tiempo. | Propagación de información, predicción epidemiológica; también **generación de imágenes** (Stable Diffusion). |
| **GAN (redes generativas adversarias)** | Dos redes: generadora (crea datos) y discriminadora (distinguir real vs generado). Entrenamiento conjunto. | Rostros, arte, generación de imágenes realistas. |
| **VAE (autocodificadores variacionales)** | Comprimen la entrada en una representación y la decodifican para generar. | Eliminación de ruido, generación de imágenes. |
| **RBM (máquinas de Boltzmann restringidas)** | Dos capas; aprenden distribución de probabilidad sobre las entradas. | Sistemas de recomendación. |
| **PixelRNN** | Generan imágenes **píxel a píxel** usando el contexto previo. | Generación secuencial de imágenes. |
| **Cadenas de Markov** | Predicen el siguiente estado solo a partir del actual. | Generación de texto (siguiente palabra). |
| **Flujos normalizadores** | Transformaciones reversibles sobre distribuciones simples para obtener distribuciones complejas. | Modelización financiera, densidad de datos. |
| **Transformers / LLM** | Modelos de lenguaje que predicen el siguiente token; pueden generar texto, código, etc. | Chatbots, código, resúmenes, RAG. En Ierahkwa: **Ollama + modelos abiertos** (ver más abajo). |

---

## Casos de uso en el mundo real

- **Creación artística:** Obras de arte, música (estilos como entrada).
- **Descubrimiento de fármacos:** Estructuras moleculares de candidatos.
- **Creación de contenidos:** Borradores, posts, textos (con IA local, sin depender de APIs externas).
- **Videojuegos:** Entornos, personajes diversos.
- **Aumento de datos:** Generar datos sintéticos cuando hay pocos datos reales.
- **Detección de anomalías:** Saber qué es “normal” y marcar outliers (p. ej. fraude).
- **Personalización:** Recomendaciones, listas, contenidos adaptados al usuario.

---

## Ventajas

- **Aumento de datos:** Generar datos adicionales cuando son escasos o caros (p. ej. imagen médica).
- **Detección de anomalías:** Identificar valores atípicos conociendo la distribución “normal”.
- **Flexibilidad:** Aprendizaje supervisado, no supervisado y semisupervisado.
- **Personalización:** Contenidos según preferencias o entradas del usuario.
- **Innovación en diseño:** Propuestas novedosas en arquitectura, producto, etc.
- **Rentabilidad:** Automatizar creación de contenido o diseños; menos coste que producción manual.

---

## Limitaciones

- **Complejidad del entrenamiento:** Recursos computacionales y tiempo elevados (sobre todo GANs y grandes LLM).
- **Control de calidad:** El contenido generado puede tener anomalías sutiles; no siempre es fácil garantizar realismo.
- **Sobreajuste:** Riesgo de poca diversidad o resultados demasiado ligados al entrenamiento.
- **Interpretabilidad:** Muchos modelos son “cajas negras”; difícil explicar por qué generan un resultado (crítico en salud, finanzas).
- **Ética:** Deep fakes, contenido falso; hace falta uso responsable.
- **Dependencia de los datos:** La calidad y sesgo de los datos de entrenamiento se reflejan en las salidas.
- **Colapso del modo (GANs):** La generadora puede producir pocas variedades; poca diversidad.

---

## Uso en ciencia de datos (con modelos locales)

En Ierahkwa, las mismas ideas se aplican usando **Ollama y modelos abiertos** en lugar de GPT-4 o ChatGPT. Ver `RuddieSolution/node/services/ai-soberano.js` y `docs/MODELOS-LLM-COMPARACION.md`.

| Tarea | Cómo ayuda un modelo generativo | En Ierahkwa |
|------|---------------------------------|-------------|
| **Exploración de datos** | Resumir y explicar conjuntos de datos, gráficos y conclusiones en lenguaje natural. | Ollama + perfil full o fast según plataforma (`getProfileForPlatform`). |
| **Generación de código** | Código para limpieza, ingeniería de características, pipelines (Python, R, SQL). | `/api/ai/code`, `/api/ai-studio/*` con ai-soberano. |
| **Redacción de informes** | Borradores que resumen conclusiones, visualizaciones y recomendaciones. | Chat/completions con modelo local. |
| **Datos sintéticos** | Generar datos de entrenamiento que respeten patrones de los reales. | Modelos generativos locales (tabulares, texto, etc.). |
| **Proyecto ML de principio a fin** | Pipelines desde preprocesamiento hasta despliegue. | Prompts a Ollama/ai-soberano; ver `docs/PROYECTOS-IA-GENERATIVA-PORTAFOLIO.md`. |

---

## Referencias en el repositorio

- **Modelos LLM y perfiles:** `MODELOS-LLM-COMPARACION.md`, `ai-soberano.js` (modelo 6 y 9, full/fast).
- **Alternativas a GPT-4 (código abierto):** `ALTERNATIVAS-GPT4-CODIGO-ABIERTO.md`.
- **Marcos y bibliotecas de IA:** `MARCOS-Y-BIBLIOTECAS-IA-REFERENCIA.md`.
- **Proyectos de portafolio:** `PROYECTOS-IA-GENERATIVA-PORTAFOLIO.md`.
- **Principio todo propio:** `PRINCIPIO-TODO-PROPIO.md`.
