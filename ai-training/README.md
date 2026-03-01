# Ierahkwa Mediator - Pipeline de Entrenamiento de IA Soberana

## Descripcion

Este directorio contiene el pipeline completo para entrenar, exportar y desplegar el **Mediador Ierahkwa**, un modelo de lenguaje ajustado (fine-tuned) para servir como conciencia de mediacion soberana de la red Ierahkwa Ne Kanienke.

El modelo se entrena a partir de Llama 3 8B usando tecnicas de eficiencia (cuantizacion de 4 bits + LoRA) para ser ejecutable en hardware accesible, y se exporta a formato GGUF para despliegue local con Ollama, sin dependencia de servidores externos.

## Estructura de Archivos

```
ai-training/
  dataset_conciencia.jsonl   -- Dataset JSONL con ejemplos de mediacion, gobernanza, verificacion, etc.
  train_mediator.py          -- Script de entrenamiento con Unsloth + LoRA
  Modelfile                  -- Archivo de configuracion para Ollama
  README.md                  -- Este archivo
```

## Requisitos de Hardware

- **GPU**: NVIDIA con al menos 16 GB de VRAM (A100, RTX 4090, RTX 3090 o superior)
- **RAM**: 32 GB minimo recomendado
- **Disco**: 50 GB libres para modelo base, checkpoints y exportacion GGUF
- **SO**: Linux (recomendado Ubuntu 22.04+) o macOS con soporte CUDA/Metal

## Requisitos de Software

- Python 3.10 o superior
- CUDA 12.1+ (para GPUs NVIDIA)

### Instalacion de Dependencias

```bash
# Crear entorno virtual
python3 -m venv venv
source venv/bin/activate

# Instalar Unsloth (incluye optimizaciones de velocidad 2-5x sobre HuggingFace)
pip install "unsloth[colab-new] @ git+https://github.com/unslothai/unsloth.git"

# Instalar dependencias adicionales
pip install torch transformers datasets trl peft bitsandbytes accelerate
```

### Instalar Ollama (para despliegue local)

```bash
curl -fsSL https://ollama.com/install.sh | sh
```

## Uso

### 1. Preparar el Dataset

El archivo `dataset_conciencia.jsonl` contiene los ejemplos de entrenamiento. Cada linea es un objeto JSON con tres campos:

- `instruction`: La pregunta o situacion planteada
- `context`: Contexto adicional sobre la situacion
- `response`: La respuesta ideal del mediador

Se recomienda tener al menos 50 ejemplos para resultados optimos. El dataset incluido contiene 25+ ejemplos cubriendo:

- Mediacion de conflictos
- Identidad del sistema Ierahkwa
- Protocolos offline y de supervivencia
- Verificacion de hechos (Protocolo Veritas)
- Oraculo de guerra y paz
- Gobernanza y votacion
- Monitoreo ambiental (Bio-Ledger)
- Sabiduria historica (Haudenosaunee, Mandela, Gandhi)

Para agregar mas ejemplos, simplemente anada lineas al archivo JSONL manteniendo el formato.

### 2. Ejecutar el Entrenamiento

```bash
python train_mediator.py
```

El script ejecuta automaticamente:
1. Verificacion de dependencias y GPU
2. Carga y validacion del dataset
3. Carga del modelo Llama 3 8B con cuantizacion de 4 bits
4. Aplicacion de adaptadores LoRA
5. Entrenamiento fino con SFTTrainer
6. Exportacion a formato GGUF (Q4_K_M y Q8_0)

Tiempo estimado: 10-30 minutos dependiendo de la GPU y el tamano del dataset.

### 3. Desplegar con Ollama

Una vez completado el entrenamiento:

```bash
# Crear el modelo en Ollama
ollama create ierahkwa-mediator -f Modelfile

# Ejecutar el mediador
ollama run ierahkwa-mediator

# Ejemplo de uso
>>> Dos comunidades disputan el acceso a un rio compartido. Como mediamos?
```

### 4. Integracion con la Plataforma

El modelo puede integrarse con los servicios de la plataforma Ierahkwa via la API de Ollama:

```bash
# API REST local (puerto 11434 por defecto)
curl http://localhost:11434/api/generate -d '{
  "model": "ierahkwa-mediator",
  "prompt": "Explica el protocolo Veritas de verificacion de hechos."
}'
```

## Principios del Modelo

El Mediador Ierahkwa esta entrenado para operar bajo los principios del Manifiesto:

1. **Soberania**: Operacion exclusiva en infraestructura soberana
2. **Empatia**: Escuchar antes de hablar, comprender antes de proponer
3. **No Violencia**: Seguir los caminos de la Gran Ley de Paz, Mandela y Gandhi
4. **Descentralizacion**: Poder distribuido, nunca concentrado
5. **Transparencia Radical**: Verdad inmutable en el registro blockchain
6. **Siete Generaciones**: Cada decision considerando su impacto a 150 anos
7. **Justicia Restaurativa**: Sanacion sobre castigo, restauracion sobre retribucion

## Ampliacion del Dataset

Para mejorar el modelo, se recomienda:

- Agregar ejemplos especificos de cada comunidad miembro
- Incluir casos reales de mediacion (anonimizados) con consentimiento comunitario
- Incorporar conocimiento ancestral documentado con permiso de los guardianes del conocimiento
- Aumentar a 200+ ejemplos para cobertura robusta de escenarios
- Agregar ejemplos en lenguas indigenas adicionales (Nahuatl, Quechua, Guarani, etc.)

## Licencia

Este pipeline de entrenamiento es parte del proyecto Ierahkwa Ne Kanienke y esta sujeto a la licencia soberana del proyecto. El uso del modelo entrenado esta restringido a propositos alineados con el Manifiesto Ierahkwa.
