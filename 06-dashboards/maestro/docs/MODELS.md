# Available Models

Maestro supports all models available in Cursor. See [Cursor Models Documentation](https://cursor.com/docs/models) for the latest information.

## Model Providers

### Anthropic

| Model | Context Window | Description |
|-------|---------------|-------------|
| **Claude 4.5 Sonnet** | 200k / 1M | Latest Claude model, best for complex tasks |
| **Claude 4.5 Haiku** | 200k | Fast and efficient |
| **Claude 4.1 Opus** | 200k | Updated Claude 4 Opus |
| **Claude 4 Opus** | 200k | Most capable Claude 4 |
| **Claude 4 Sonnet** | 200k / 1M | Balanced Claude 4 model |

**Model IDs:**
- `claude-4-5-sonnet` (default)
- `claude-4-5-sonnet-1m`
- `claude-4-5-haiku`
- `claude-4-1-opus`
- `claude-4-opus`
- `claude-4-sonnet`
- `claude-4-sonnet-1m`

### OpenAI

| Model | Context Window | Description |
|-------|---------------|-------------|
| **GPT-5** | 272k | Latest GPT model |
| **GPT-5-Pro** | 272k | Professional variant |
| **GPT-5 Fast** | 272k | Faster variant |
| **GPT-5-Codex** | 272k | Code-optimized |
| **GPT-5.1** | 272k | Updated GPT-5 |
| **GPT-5.1 Codex** | 272k | Code-optimized GPT-5.1 |
| **GPT-5 Mini** | 272k | Smaller variant |
| **GPT-5 Nano** | 272k | Smallest variant |
| **GPT 4.1** | 200k / 1M | GPT 4.1 model |
| **o3** | 200k | Reasoning model |

**Model IDs:**
- `gpt-5`
- `gpt-5-pro`
- `gpt-5-fast`
- `gpt-5-codex`
- `gpt-5-1`
- `gpt-5-1-codex`
- `gpt-5-mini`
- `gpt-5-nano`
- `gpt-5-1-codex-mini`
- `gpt-4-1`
- `gpt-4-1-1m`
- `o3`

### Google

| Model | Context Window | Description |
|-------|---------------|-------------|
| **Gemini 3 Pro** | 200k / 1M | Latest Gemini Pro |
| **Gemini 2.5 Pro** | 200k / 1M | Capable Gemini Pro |
| **Gemini 2.5 Flash** | 1M | Fast Gemini with 1M context |

**Model IDs:**
- `gemini-3-pro`
- `gemini-3-pro-1m`
- `gemini-2-5-pro`
- `gemini-2-5-pro-1m`
- `gemini-2-5-flash`

### xAI

| Model | Context Window | Description |
|-------|---------------|-------------|
| **Grok 4** | 256k | xAI Grok 4 model |
| **Grok 4 Fast** | 200k / 2M | Faster Grok variant |
| **Grok Code** | 256k | Code-optimized Grok |

**Model IDs:**
- `grok-4`
- `grok-4-fast`
- `grok-4-fast-2m`
- `grok-code`

### DeepSeek

| Model | Context Window | Description |
|-------|---------------|-------------|
| **Deepseek V3.1** | 60k | Deepseek V3.1 model |
| **Deepseek R1** | 60k | Reasoning model |

**Model IDs:**
- `deepseek-v3-1`
- `deepseek-r1-20250528`

### Cursor

| Model | Context Window | Description |
|-------|---------------|-------------|
| **Composer 1** | 200k | Cursor's custom Composer model |

**Model IDs:**
- `composer-1`

## Usage

### CLI

```bash
# Use specific model
maestro run --flow my-workflow --model claude-4-5-sonnet

# Use GPT-5
maestro run --flow my-workflow --model gpt-5

# Use Gemini 3 Pro with 1M context
maestro run --flow my-workflow --model gemini-3-pro-1m
```

### Dashboard

Select model from the dropdown in the dashboard UI. Models are grouped by provider for easy selection.

### API

```bash
# Change model for workflow
POST /api/workflows/:workflow/model
{ "modelId": "claude-4-5-sonnet" }

# Change model for specific step
POST /api/workflows/:workflow/steps/:step/model
{ "modelId": "gpt-5-codex" }
```

## Default Model

The default model is **Claude 4.5 Sonnet** (`claude-4-5-sonnet`), which provides the best balance of capability and performance.

## Context Windows

Models support different context window sizes:

- **200k**: Standard long context (most models)
- **256k**: Extended context (Grok models)
- **272k**: GPT-5 models
- **1M**: Ultra-long context (selected models)
- **2M**: Maximum context (Grok 4 Fast 2M)
- **60k**: Standard context (DeepSeek models)

Choose models with larger context windows for tasks requiring extensive codebase analysis.

## Model Selection Guide

- **Complex Analysis**: Claude 4.5 Sonnet, GPT-5-Pro, Gemini 3 Pro
- **Code Generation**: GPT-5-Codex, Grok Code, Composer 1
- **Fast Tasks**: Claude 4.5 Haiku, GPT-5 Fast, Gemini 2.5 Flash
- **Ultra-Long Context**: Models with 1M+ context windows
- **Reasoning**: o3, Deepseek R1
- **Cost-Effective**: GPT-5 Mini, GPT-5 Nano

## See Also

- [Cursor Models Documentation](https://cursor.com/docs/models)
- [Model Manager API](API_REFERENCE.md#model-management)
