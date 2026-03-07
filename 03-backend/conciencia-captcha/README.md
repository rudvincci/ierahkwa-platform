# Conciencia Captcha

> Sistema CAPTCHA soberano basado en empatia: solo humanos con conciencia social pasan la verificacion.

## Descripcion

Conciencia Captcha es un sistema de verificacion de humanidad radicalmente diferente a los CAPTCHAs tradicionales. En lugar de resolver imagenes distorsionadas o puzzles visuales, presenta dilemas eticos relacionados con comunidades indigenas y soberania. El usuario debe responder con una propuesta de mediacion, y un modelo de IA (Ollama/Mistral) analiza la respuesta evaluando su nivel de empatia en una escala de 0.0 a 1.0.

El sistema bloquea bots y respuestas mecanicas porque requiere comprension contextual, empatia cultural y capacidad de mediacion. Las respuestas genericas, vacias o automatizadas reciben puntuaciones bajas (< 0.5). Solo respuestas con empatia >= 0.8 (configurable) pasan la verificacion, demostrando que el usuario posee conciencia social y comprension humana.

Los dilemas presentados abordan conflictos reales de comunidades: disputas por recursos naturales, preservacion de tradiciones orales frente a tecnologia moderna, y equilibrio entre desarrollo economico y territorios sagrados. Cada desafio es seleccionado aleatoriamente para evitar memorizacion.

## Stack Tecnico

- **Runtime**: Node.js 22
- **Framework**: Express 4.x
- **Motor IA**: Ollama (Mistral por defecto)
- **Analisis**: Evaluacion de empatia via LLM
- **Dependencias**: Solo express y cors (minimalista)
- **Puerto**: 3095

## API Endpoints

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /health | Health check con version y nombre del servicio |
| GET | /challenge | Obtener dilema etico aleatorio (id + texto) |
| POST | /validate | Validar respuesta del usuario contra dilema |

### POST /validate — Body

```json
{
  "challengeId": "d1",
  "response": "Propongo crear un calendario compartido que alterne..."
}
```

### POST /validate — Respuesta

```json
{
  "passed": true,
  "empathyScore": 0.92,
  "reason": "Respuesta mediadora que considera ambas perspectivas",
  "threshold": 0.8
}
```

## Variables de Entorno

| Variable | Descripcion | Ejemplo |
|----------|-------------|---------|
| PORT | Puerto del servicio | 3095 |
| OLLAMA_URL | URL del servidor Ollama | http://ollama:11434 |
| OLLAMA_MODEL | Modelo LLM para analisis | mistral |
| EMPATHY_THRESHOLD | Umbral minimo de empatia (0.0-1.0) | 0.8 |

## Instalacion

```bash
npm install
npm start
```

### Requisitos previos

- Servidor Ollama ejecutandose con el modelo Mistral (u otro compatible)

```bash
ollama pull mistral
ollama serve
```

## Docker

```bash
docker build -t conciencia-captcha .
docker run -p 3095:3095 \
  -e OLLAMA_URL=http://ollama:11434 \
  -e OLLAMA_MODEL=mistral \
  -e EMPATHY_THRESHOLD=0.8 \
  conciencia-captcha
```

## Arquitectura

```
Cliente ──→ GET /challenge ──→ Dilema aleatorio
         │
         └→ POST /validate ──→ Prompt de analisis ──→ Ollama (Mistral)
                                                          │
                                                    JSON { empathy, reason }
                                                          │
                                               empathy >= threshold? → PASS/FAIL
```

El flujo es stateless: el cliente recibe un dilema, el usuario escribe su mediacion, y el servicio envia un prompt estructurado a Ollama que evalua la empatia. El LLM responde con un JSON de puntuacion y razon. Si la empatia supera el umbral configurado, el captcha se considera pasado.

## Parte de

**Ierahkwa Ne Kanienke** — Plataforma Soberana Digital
