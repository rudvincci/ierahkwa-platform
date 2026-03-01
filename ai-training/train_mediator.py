#!/usr/bin/env python3
"""
Ierahkwa Mediator - Script de entrenamiento fino con Unsloth + LoRA
===================================================================
Entrena un modelo Llama 3 8B como mediador soberano para la red Ierahkwa.
Usa cuantizacion de 4 bits y adaptadores LoRA para eficiencia maxima.

Requisitos:
- GPU con al menos 16GB VRAM (recomendado: NVIDIA A100, RTX 4090, o superior)
- Python 3.10+
- pip install unsloth transformers datasets trl peft bitsandbytes

Uso:
    python train_mediator.py

Salida:
    - Modelo entrenado en ./ierahkwa-mediator-lora/
    - Modelo GGUF para Ollama en ./ierahkwa-mediator-gguf/
"""

import os
import sys
import json
import logging
from pathlib import Path

# Configuracion de logging
logging.basicConfig(
    level=logging.INFO,
    format="[%(asctime)s] %(levelname)s - %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
)
logger = logging.getLogger("ierahkwa-trainer")

# --- Constantes de configuracion ---

# Modelo base para fine-tuning
MODELO_BASE = "unsloth/llama-3-8b-bnb-4bit"

# Ruta al dataset de entrenamiento
DATASET_PATH = Path(__file__).parent / "dataset_conciencia.jsonl"

# Directorio de salida para el modelo LoRA
OUTPUT_DIR = Path(__file__).parent / "ierahkwa-mediator-lora"

# Directorio de salida para el modelo GGUF (Ollama)
GGUF_OUTPUT_DIR = Path(__file__).parent / "ierahkwa-mediator-gguf"

# Hiperparametros de entrenamiento
TRAINING_CONFIG = {
    "max_seq_length": 2048,
    "lora_r": 16,                   # Rango de LoRA - balance entre capacidad y eficiencia
    "lora_alpha": 16,               # Factor de escala de LoRA
    "lora_dropout": 0.0,            # Sin dropout para datasets pequenos con supervision
    "target_modules": [             # Modulos a adaptar con LoRA
        "q_proj",
        "k_proj",
        "v_proj",
        "o_proj",
        "gate_proj",
        "up_proj",
        "down_proj",
    ],
    "num_train_epochs": 3,          # Epocas de entrenamiento
    "per_device_train_batch_size": 2,
    "gradient_accumulation_steps": 4,  # Batch efectivo = 2 * 4 = 8
    "learning_rate": 2e-4,
    "weight_decay": 0.01,
    "warmup_steps": 10,
    "lr_scheduler_type": "linear",
    "logging_steps": 1,
    "save_steps": 25,
    "seed": 42,
    "fp16": False,
    "bf16": True,                   # Usar bfloat16 si la GPU lo soporta
    "optim": "adamw_8bit",          # Optimizador eficiente en memoria
    "max_grad_norm": 0.3,
}

# Plantilla del prompt para formatear los ejemplos de entrenamiento
PROMPT_TEMPLATE = """### Instruccion:
{instruction}

### Contexto:
{context}

### Respuesta:
{response}"""


def verificar_dependencias():
    """Verifica que todas las dependencias necesarias estan instaladas."""
    dependencias = {
        "unsloth": "unsloth",
        "transformers": "transformers",
        "datasets": "datasets",
        "trl": "trl",
        "peft": "peft",
        "torch": "torch",
    }

    faltantes = []
    for nombre, paquete in dependencias.items():
        try:
            __import__(paquete)
        except ImportError:
            faltantes.append(nombre)

    if faltantes:
        logger.error(
            "Dependencias faltantes: %s", ", ".join(faltantes)
        )
        logger.error(
            "Instale con: pip install %s", " ".join(faltantes)
        )
        sys.exit(1)

    # Verificar disponibilidad de GPU
    import torch
    if not torch.cuda.is_available():
        logger.error(
            "No se detecto GPU CUDA. Este script requiere una GPU NVIDIA "
            "con al menos 16GB de VRAM para el entrenamiento."
        )
        sys.exit(1)

    gpu_name = torch.cuda.get_device_name(0)
    gpu_memory = torch.cuda.get_device_properties(0).total_mem / (1024 ** 3)
    logger.info("GPU detectada: %s (%.1f GB VRAM)", gpu_name, gpu_memory)

    if gpu_memory < 15.0:
        logger.warning(
            "La GPU tiene %.1f GB de VRAM. Se recomiendan al menos 16 GB. "
            "El entrenamiento podria fallar por falta de memoria.",
            gpu_memory,
        )


def cargar_dataset(ruta: Path) -> list[dict]:
    """
    Carga el dataset JSONL y lo formatea con la plantilla de prompt.
    Cada linea del archivo debe ser un objeto JSON con los campos:
    instruction, context, response.
    """
    if not ruta.exists():
        logger.error("No se encontro el dataset en: %s", ruta)
        sys.exit(1)

    ejemplos = []
    with open(ruta, "r", encoding="utf-8") as f:
        for num_linea, linea in enumerate(f, 1):
            linea = linea.strip()
            if not linea:
                continue
            try:
                dato = json.loads(linea)
            except json.JSONDecodeError as e:
                logger.error(
                    "Error de JSON en linea %d: %s", num_linea, e
                )
                sys.exit(1)

            # Validar campos requeridos
            campos_requeridos = {"instruction", "context", "response"}
            campos_presentes = set(dato.keys())
            if not campos_requeridos.issubset(campos_presentes):
                faltantes = campos_requeridos - campos_presentes
                logger.error(
                    "Linea %d: faltan campos requeridos: %s",
                    num_linea,
                    faltantes,
                )
                sys.exit(1)

            # Formatear con la plantilla
            texto_formateado = PROMPT_TEMPLATE.format(
                instruction=dato["instruction"],
                context=dato["context"],
                response=dato["response"],
            )
            ejemplos.append({"text": texto_formateado})

    logger.info(
        "Dataset cargado: %d ejemplos desde %s", len(ejemplos), ruta
    )
    return ejemplos


def cargar_modelo():
    """
    Carga el modelo base con cuantizacion de 4 bits y aplica adaptadores LoRA.
    Retorna el modelo y el tokenizador.
    """
    from unsloth import FastLanguageModel

    logger.info("Cargando modelo base: %s", MODELO_BASE)
    logger.info("Aplicando cuantizacion de 4 bits para eficiencia de memoria...")

    modelo, tokenizer = FastLanguageModel.from_pretrained(
        model_name=MODELO_BASE,
        max_seq_length=TRAINING_CONFIG["max_seq_length"],
        dtype=None,  # Deteccion automatica del tipo de dato
        load_in_4bit=True,
    )

    logger.info("Modelo base cargado. Aplicando adaptadores LoRA...")

    # Aplicar adaptadores LoRA a los modulos objetivo
    modelo = FastLanguageModel.get_peft_model(
        modelo,
        r=TRAINING_CONFIG["lora_r"],
        lora_alpha=TRAINING_CONFIG["lora_alpha"],
        lora_dropout=TRAINING_CONFIG["lora_dropout"],
        target_modules=TRAINING_CONFIG["target_modules"],
        bias="none",
        use_gradient_checkpointing="unsloth",  # Optimizacion de Unsloth para 30% menos VRAM
        random_state=TRAINING_CONFIG["seed"],
        use_rslora=False,
        loftq_config=None,
    )

    # Contar parametros entrenables
    total_params = sum(p.numel() for p in modelo.parameters())
    trainable_params = sum(p.numel() for p in modelo.parameters() if p.requires_grad)
    porcentaje = (trainable_params / total_params) * 100

    logger.info(
        "Parametros totales: %s | Entrenables: %s (%.2f%%)",
        f"{total_params:,}",
        f"{trainable_params:,}",
        porcentaje,
    )

    return modelo, tokenizer


def entrenar(modelo, tokenizer, dataset_formateado: list[dict]):
    """
    Ejecuta el entrenamiento fino usando SFTTrainer de TRL.
    """
    from trl import SFTTrainer
    from transformers import TrainingArguments
    from datasets import Dataset

    logger.info("Preparando entrenamiento...")
    logger.info("  Epocas: %d", TRAINING_CONFIG["num_train_epochs"])
    logger.info(
        "  Batch efectivo: %d",
        TRAINING_CONFIG["per_device_train_batch_size"]
        * TRAINING_CONFIG["gradient_accumulation_steps"],
    )
    logger.info("  Learning rate: %s", TRAINING_CONFIG["learning_rate"])
    logger.info("  Secuencia maxima: %d tokens", TRAINING_CONFIG["max_seq_length"])

    # Convertir lista de diccionarios a Dataset de HuggingFace
    hf_dataset = Dataset.from_list(dataset_formateado)

    # Configurar argumentos de entrenamiento
    training_args = TrainingArguments(
        output_dir=str(OUTPUT_DIR),
        num_train_epochs=TRAINING_CONFIG["num_train_epochs"],
        per_device_train_batch_size=TRAINING_CONFIG["per_device_train_batch_size"],
        gradient_accumulation_steps=TRAINING_CONFIG["gradient_accumulation_steps"],
        learning_rate=TRAINING_CONFIG["learning_rate"],
        weight_decay=TRAINING_CONFIG["weight_decay"],
        warmup_steps=TRAINING_CONFIG["warmup_steps"],
        lr_scheduler_type=TRAINING_CONFIG["lr_scheduler_type"],
        logging_steps=TRAINING_CONFIG["logging_steps"],
        save_steps=TRAINING_CONFIG["save_steps"],
        save_total_limit=3,
        seed=TRAINING_CONFIG["seed"],
        fp16=TRAINING_CONFIG["fp16"],
        bf16=TRAINING_CONFIG["bf16"],
        optim=TRAINING_CONFIG["optim"],
        max_grad_norm=TRAINING_CONFIG["max_grad_norm"],
        report_to="none",  # Desactivar integraciones de logging externas
        dataloader_pin_memory=True,
        remove_unused_columns=True,
    )

    # Crear el entrenador SFT
    trainer = SFTTrainer(
        model=modelo,
        tokenizer=tokenizer,
        train_dataset=hf_dataset,
        dataset_text_field="text",
        max_seq_length=TRAINING_CONFIG["max_seq_length"],
        dataset_num_proc=2,
        packing=False,  # Sin packing para mantener la integridad de cada ejemplo
        args=training_args,
    )

    # Estadisticas de memoria antes del entrenamiento
    import torch
    memoria_reservada = torch.cuda.memory_reserved(0) / (1024 ** 3)
    memoria_asignada = torch.cuda.memory_allocated(0) / (1024 ** 3)
    logger.info(
        "Memoria GPU antes de entrenar: %.2f GB reservados, %.2f GB asignados",
        memoria_reservada,
        memoria_asignada,
    )

    # Ejecutar entrenamiento
    logger.info("Iniciando entrenamiento fino...")
    resultado = trainer.train()

    # Reportar resultados
    logger.info("Entrenamiento completado.")
    logger.info("  Perdida final: %.4f", resultado.training_loss)
    logger.info("  Pasos totales: %d", resultado.global_step)
    logger.info(
        "  Tiempo total: %.1f minutos",
        resultado.metrics.get("train_runtime", 0) / 60,
    )

    # Guardar modelo LoRA
    logger.info("Guardando modelo LoRA en: %s", OUTPUT_DIR)
    modelo.save_pretrained(str(OUTPUT_DIR))
    tokenizer.save_pretrained(str(OUTPUT_DIR))
    logger.info("Modelo LoRA guardado exitosamente.")

    return modelo, tokenizer


def exportar_gguf(modelo, tokenizer):
    """
    Exporta el modelo entrenado a formato GGUF para uso con Ollama.
    Genera cuantizaciones Q4_K_M y Q8_0.
    """
    logger.info("Exportando modelo a formato GGUF para Ollama...")

    gguf_dir = str(GGUF_OUTPUT_DIR)
    os.makedirs(gguf_dir, exist_ok=True)

    # Exportar en formato GGUF con cuantizacion Q4_K_M (buena calidad, tamano reducido)
    logger.info("Generando cuantizacion Q4_K_M (recomendada para produccion)...")
    modelo.save_pretrained_gguf(
        gguf_dir,
        tokenizer,
        quantization_method="q4_k_m",
    )
    logger.info("GGUF Q4_K_M exportado en: %s", gguf_dir)

    # Tambien exportar Q8_0 para maxima calidad si hay espacio
    try:
        logger.info("Generando cuantizacion Q8_0 (maxima calidad)...")
        q8_dir = str(GGUF_OUTPUT_DIR / "q8_0")
        os.makedirs(q8_dir, exist_ok=True)
        modelo.save_pretrained_gguf(
            q8_dir,
            tokenizer,
            quantization_method="q8_0",
        )
        logger.info("GGUF Q8_0 exportado en: %s", q8_dir)
    except Exception as e:
        logger.warning(
            "No se pudo exportar Q8_0 (puede requerir mas memoria): %s", e
        )

    logger.info("Exportacion GGUF completada.")
    logger.info(
        "Para usar con Ollama, copie el archivo .gguf y el Modelfile "
        "al directorio de modelos de Ollama y ejecute:"
    )
    logger.info("  ollama create ierahkwa-mediator -f Modelfile")


def main():
    """Funcion principal que ejecuta todo el pipeline de entrenamiento."""

    logger.info("=" * 60)
    logger.info("IERAHKWA MEDIATOR - Pipeline de Entrenamiento Soberano")
    logger.info("=" * 60)

    # Paso 1: Verificar dependencias y hardware
    logger.info("[1/5] Verificando dependencias y hardware...")
    verificar_dependencias()

    # Paso 2: Cargar y preparar el dataset
    logger.info("[2/5] Cargando dataset de conciencia...")
    dataset = cargar_dataset(DATASET_PATH)

    if len(dataset) < 10:
        logger.warning(
            "El dataset tiene solo %d ejemplos. Se recomiendan al menos 50 "
            "para resultados optimos. Considere expandir dataset_conciencia.jsonl.",
            len(dataset),
        )

    # Paso 3: Cargar modelo con cuantizacion y LoRA
    logger.info("[3/5] Cargando modelo base con cuantizacion 4-bit y LoRA...")
    modelo, tokenizer = cargar_modelo()

    # Paso 4: Entrenar
    logger.info("[4/5] Ejecutando entrenamiento fino...")
    modelo, tokenizer = entrenar(modelo, tokenizer, dataset)

    # Paso 5: Exportar a GGUF
    logger.info("[5/5] Exportando a formato GGUF para Ollama...")
    exportar_gguf(modelo, tokenizer)

    logger.info("=" * 60)
    logger.info("Pipeline completado exitosamente.")
    logger.info("Modelo LoRA: %s", OUTPUT_DIR)
    logger.info("Modelo GGUF: %s", GGUF_OUTPUT_DIR)
    logger.info("")
    logger.info("Proximos pasos:")
    logger.info("  1. Copie el archivo .gguf al directorio de Ollama")
    logger.info("  2. Ejecute: ollama create ierahkwa-mediator -f Modelfile")
    logger.info("  3. Pruebe: ollama run ierahkwa-mediator")
    logger.info("=" * 60)


if __name__ == "__main__":
    main()
