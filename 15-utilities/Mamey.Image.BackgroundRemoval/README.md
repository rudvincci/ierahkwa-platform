# Mamey Image Background Removal Service

AI-powered background removal service for images using rembg. This service provides REST API, CLI tool, and Python library interfaces for removing backgrounds from images while preserving foreground subjects (people, objects, clothing, etc.).

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [REST API](#rest-api)
- [CLI Tool](#cli-tool)
- [Python Library](#python-library)
- [Configuration](#configuration)
- [Docker Deployment](#docker-deployment)
- [Advanced Usage](#advanced-usage)
- [API Reference](#api-reference)
- [Development](#development)
- [Performance](#performance)
- [Troubleshooting](#troubleshooting)

## Features

- **🤖 AI-Powered Background Removal**: Uses rembg with u2net model for intelligent background detection
- **👤 Preserves Foreground Elements**: Keeps people, objects, clothing, and other foreground subjects intact
- **🌐 Multiple Interfaces**: REST API, CLI tool, and Python library
- **📦 Batch Processing**: Process multiple images at once
- **🎨 Multiple Output Formats**: PNG (with transparency) and JPEG (with white background)
- **🐳 Docker Support**: Easy deployment with Docker containers
- **⚡ High Performance**: Optimized for production use
- **🔧 Configurable**: Extensive configuration options
- **📊 Health Monitoring**: Built-in health checks and monitoring

## Installation

### Prerequisites

- Python 3.8 or higher
- pip package manager

### Install from Source

```bash
# Clone the repository
git clone <repository-url>
cd Mamey.Image.BackgroundRemoval

# Install the package
pip install -e .

# Install development dependencies (optional)
pip install -r requirements-dev.txt
```

### Install Dependencies

```bash
# Install production dependencies
pip install -r requirements.txt

# For GPU support (recommended for better performance)
pip install rembg[gpu]
```

## Quick Start

### 1. Start the API Server

```bash
# Start the server
mamey-image-bg serve

# Or with custom settings
mamey-image-bg serve --host 0.0.0.0 --port 5000 --reload
```

### 2. Process Your First Image

```bash
# Using CLI
mamey-image-bg remove input.jpg output.png

# Using API
curl -X POST "http://localhost:5000/api/remove-background" \
  -F "file=@input.jpg" \
  -F "output_format=PNG" \
  -o output.png
```

### 3. Check Service Health

```bash
# Health check
curl http://localhost:5000/api/health

# Available models
curl http://localhost:5000/api/models
```

## REST API

### Starting the Server

```bash
# Basic start
mamey-image-bg serve

# Custom configuration
mamey-image-bg serve --host 0.0.0.0 --port 8080 --reload

# With environment variables
MAMEY_IMAGE_BG_HOST=0.0.0.0 MAMEY_IMAGE_BG_PORT=8080 mamey-image-bg serve
```

### API Endpoints

#### Health Check
```http
GET /api/health
```

**Response:**
```json
{
  "status": "healthy",
  "service": "mamey-image-background-removal"
}
```

#### Available Models
```http
GET /api/models
```

**Response:**
```json
{
  "models": ["u2net", "u2net_human_seg", "u2netp", "u2net_cloth_seg", "silueta", "isnet-general-use"],
  "current_model": "u2net"
}
```

#### Remove Background (Single Image)
```http
POST /api/remove-background
Content-Type: multipart/form-data

file: [image file]
output_format: PNG|JPEG
```

**Example using curl:**
```bash
curl -X POST "http://localhost:5000/api/remove-background" \
  -F "file=@portrait.jpg" \
  -F "output_format=PNG" \
  -o portrait_no_bg.png
```

**Example using Python requests:**
```python
import requests

url = "http://localhost:5000/api/remove-background"
files = {"file": open("portrait.jpg", "rb")}
data = {"output_format": "PNG"}

response = requests.post(url, files=files, data=data)

if response.status_code == 200:
    with open("portrait_no_bg.png", "wb") as f:
        f.write(response.content)
    print("Background removed successfully!")
else:
    print(f"Error: {response.status_code} - {response.text}")
```

#### Remove Background (Raw Bytes)
```http
POST /api/remove-background/bytes
Content-Type: application/json

{
  "image_data": "base64_encoded_image_data",
  "output_format": "PNG|JPEG"
}
```

**Example using curl:**
```bash
# Convert image to base64 and send
base64_image=$(base64 -i portrait.jpg)
curl -X POST "http://localhost:5000/api/remove-background/bytes" \
  -H "Content-Type: application/json" \
  -d "{\"image_data\": \"$base64_image\", \"output_format\": \"PNG\"}" \
  -o portrait_no_bg.png
```

**Example using Python requests:**
```python
import requests
import base64

# Read and encode image
with open("portrait.jpg", "rb") as f:
    image_data = base64.b64encode(f.read()).decode('utf-8')

url = "http://localhost:5000/api/remove-background/bytes"
payload = {
    "image_data": image_data,
    "output_format": "PNG"
}

response = requests.post(url, json=payload)

if response.status_code == 200:
    with open("portrait_no_bg.png", "wb") as f:
        f.write(response.content)
    print("Background removed successfully!")
else:
    print(f"Error: {response.status_code} - {response.text}")
```

#### Remove Background (Batch Processing)
```http
POST /api/remove-background/batch
Content-Type: multipart/form-data

files: [image file 1]
files: [image file 2]
...
output_format: PNG|JPEG
```

**Example using curl:**
```bash
curl -X POST "http://localhost:5000/api/remove-background/batch" \
  -F "files=@image1.jpg" \
  -F "files=@image2.jpg" \
  -F "files=@image3.jpg" \
  -F "output_format=PNG" \
  -o processed_images.zip
```

**Example using Python requests:**
```python
import requests
import zipfile
import io

url = "http://localhost:5000/api/remove-background/batch"
files = [
    ("files", open("image1.jpg", "rb")),
    ("files", open("image2.jpg", "rb")),
    ("files", open("image3.jpg", "rb"))
]
data = {"output_format": "PNG"}

response = requests.post(url, files=files, data=data)

if response.status_code == 200:
    # Extract ZIP file
    with zipfile.ZipFile(io.BytesIO(response.content)) as zip_file:
        zip_file.extractall("processed_images")
    print("Batch processing completed!")
else:
    print(f"Error: {response.status_code} - {response.text}")
```

### API Documentation

Once the server is running, visit:
- **Swagger UI**: http://localhost:5000/docs
- **ReDoc**: http://localhost:5000/redoc

## CLI Tool

### Basic Commands

#### Remove Background from Single Image
```bash
# Basic usage
mamey-image-bg remove input.jpg output.png

# With specific format
mamey-image-bg remove input.jpg output.jpg --format JPEG

# With custom model
mamey-image-bg remove input.jpg output.png --model u2net_human_seg

# Verbose output
mamey-image-bg --verbose remove input.jpg output.png
```

#### Batch Processing
```bash
# Process all images in a directory
mamey-image-bg batch input_dir/ output_dir/

# With specific file pattern
mamey-image-bg batch input_dir/ output_dir/ --pattern "*.jpg"

# Recursive processing
mamey-image-bg batch input_dir/ output_dir/ --recursive

# With specific format
mamey-image-bg batch input_dir/ output_dir/ --format JPEG
```

#### List Available Models
```bash
# Show all available models
mamey-image-bg models

# With verbose output
mamey-image-bg --verbose models
```

#### Start API Server
```bash
# Basic server start
mamey-image-bg serve

# Custom host and port
mamey-image-bg serve --host 0.0.0.0 --port 8080

# Development mode with auto-reload
mamey-image-bg serve --reload

# With custom model
mamey-image-bg --model u2net_human_seg serve
```

### Advanced CLI Usage

#### Processing with Different Models
```bash
# For human subjects (recommended for portraits)
mamey-image-bg --model u2net_human_seg remove portrait.jpg portrait_no_bg.png

# For clothing (recommended for product photos)
mamey-image-bg --model u2net_cloth_seg remove product.jpg product_no_bg.png

# Lightweight model (faster processing)
mamey-image-bg --model u2netp remove image.jpg output.png

# High accuracy model (slower but better quality)
mamey-image-bg --model isnet-general-use remove image.jpg output.png
```

#### Batch Processing Examples
```bash
# Process only JPG files
mamey-image-bg batch photos/ processed/ --pattern "*.jpg"

# Process with specific model
mamey-image-bg --model u2net_human_seg batch portraits/ processed/

# Recursive processing with pattern
mamey-image-bg batch photos/ processed/ --pattern "*.png" --recursive

# Verbose batch processing
mamey-image-bg --verbose batch input/ output/
```

## Python Library

### Basic Usage

```python
from mamey_image_bg import BackgroundRemovalProcessor

# Initialize processor
processor = BackgroundRemovalProcessor(model_name="u2net")

# Remove background from file
output_path = processor.remove_background_from_file("input.jpg", "output.png")
print(f"Processed image saved to: {output_path}")
```

### Advanced Usage

```python
from mamey_image_bg import BackgroundRemovalProcessor
from PIL import Image
import io

# Initialize with specific model
processor = BackgroundRemovalProcessor(model_name="u2net_human_seg")

# Process from file
output_path = processor.remove_background_from_file(
    "portrait.jpg", 
    "portrait_no_bg.png",
    "PNG"
)

# Process from PIL Image
image = Image.open("input.jpg")
result_data = processor.remove_background(image, "PNG")

# Save result
with open("output.png", "wb") as f:
    f.write(result_data)

# Process from bytes
with open("input.jpg", "rb") as f:
    input_data = f.read()

result_data = processor.remove_background(input_data, "PNG")
```

### Batch Processing with Python

```python
import os
from pathlib import Path
from mamey_image_bg import BackgroundRemovalProcessor

def process_directory(input_dir, output_dir, model_name="u2net"):
    """Process all images in a directory."""
    processor = BackgroundRemovalProcessor(model_name=model_name)
    
    input_path = Path(input_dir)
    output_path = Path(output_dir)
    output_path.mkdir(exist_ok=True)
    
    # Get all image files
    image_extensions = {'.jpg', '.jpeg', '.png', '.bmp', '.tiff', '.webp'}
    image_files = [f for f in input_path.iterdir() 
                   if f.suffix.lower() in image_extensions]
    
    print(f"Found {len(image_files)} images to process")
    
    for i, image_file in enumerate(image_files, 1):
        try:
            print(f"Processing {i}/{len(image_files)}: {image_file.name}")
            
            # Generate output filename
            output_file = output_path / f"{image_file.stem}_no_bg.png"
            
            # Process image
            processor.remove_background_from_file(
                str(image_file), 
                str(output_file), 
                "PNG"
            )
            
            print(f"  ✅ Saved: {output_file}")
            
        except Exception as e:
            print(f"  ❌ Error processing {image_file.name}: {e}")

# Usage
process_directory("input_photos", "processed_photos", "u2net_human_seg")
```

### Custom Configuration

```python
from mamey_image_bg.core.config import Settings

# Create custom settings
settings = Settings(
    model_name="u2net_human_seg",
    max_image_size=2048,  # Larger images
    quality=98,           # Higher quality
    log_level="DEBUG"     # Debug logging
)

# Use with processor
processor = BackgroundRemovalProcessor(model_name=settings.model_name)
```

### Error Handling

```python
from mamey_image_bg import BackgroundRemovalProcessor

processor = BackgroundRemovalProcessor()

try:
    result = processor.remove_background_from_file("input.jpg", "output.png")
    print(f"Success: {result}")
    
except FileNotFoundError:
    print("Input file not found")
    
except ValueError as e:
    print(f"Invalid input: {e}")
    
except Exception as e:
    print(f"Processing error: {e}")
```

## Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `MAMEY_IMAGE_BG_HOST` | `0.0.0.0` | Server host address |
| `MAMEY_IMAGE_BG_PORT` | `5000` | Server port |
| `MAMEY_IMAGE_BG_DEBUG` | `false` | Enable debug mode |
| `MAMEY_IMAGE_BG_MODEL_NAME` | `u2net` | Background removal model |
| `MAMEY_IMAGE_BG_MAX_IMAGE_SIZE` | `1024` | Maximum image dimension |
| `MAMEY_IMAGE_BG_QUALITY` | `95` | Output image quality (1-100) |
| `MAMEY_IMAGE_BG_LOG_LEVEL` | `INFO` | Logging level |

### Configuration File

Create a `.env` file in the project root:

```bash
# Copy example file
cp env.example .env

# Edit configuration
nano .env
```

Example `.env` file:
```bash
# Server Configuration
MAMEY_IMAGE_BG_HOST=0.0.0.0
MAMEY_IMAGE_BG_PORT=5000
MAMEY_IMAGE_BG_DEBUG=false

# Background Removal Configuration
MAMEY_IMAGE_BG_MODEL_NAME=u2net_human_seg
MAMEY_IMAGE_BG_MAX_IMAGE_SIZE=2048
MAMEY_IMAGE_BG_QUALITY=98

# Logging
MAMEY_IMAGE_BG_LOG_LEVEL=DEBUG
```

### Available Models

| Model | Description | Best For | Speed | Quality |
|-------|-------------|----------|-------|---------|
| `u2net` | General purpose | All subjects | Medium | High |
| `u2net_human_seg` | Human subjects | Portraits, people | Medium | High |
| `u2netp` | Lightweight | Quick processing | Fast | Medium |
| `u2net_cloth_seg` | Clothing | Product photos | Medium | High |
| `silueta` | Alternative | General use | Medium | High |
| `isnet-general-use` | High accuracy | Quality critical | Slow | Very High |

## Docker Deployment

### Build Image

```bash
# Build the Docker image
docker build -t mamey-image-bg .

# Build with specific tag
docker build -t mamey-image-bg:latest .
```

### Run Container

#### API Server
```bash
# Basic run
docker run -p 5000:5000 mamey-image-bg

# With custom port
docker run -p 8080:5000 mamey-image-bg

# With volume mount
docker run -p 5000:5000 -v /path/to/images:/app/images mamey-image-bg

# With environment variables
docker run -p 5000:5000 \
  -e MAMEY_IMAGE_BG_MODEL_NAME=u2net_human_seg \
  -e MAMEY_IMAGE_BG_LOG_LEVEL=DEBUG \
  mamey-image-bg
```

#### CLI Tool
```bash
# Process single image
docker run -v /path/to/images:/images mamey-image-bg remove /images/input.jpg /images/output.png

# Batch processing
docker run -v /path/to/images:/images mamey-image-bg batch /images/input /images/output

# With custom model
docker run -v /path/to/images:/images mamey-image-bg --model u2net_human_seg remove /images/portrait.jpg /images/portrait_no_bg.png
```

### Docker Compose

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  mamey-image-bg:
    build: .
    ports:
      - "5000:5000"
    environment:
      - MAMEY_IMAGE_BG_HOST=0.0.0.0
      - MAMEY_IMAGE_BG_PORT=5000
      - MAMEY_IMAGE_BG_MODEL_NAME=u2net_human_seg
      - MAMEY_IMAGE_BG_LOG_LEVEL=INFO
    volumes:
      - ./images:/app/images
      - ./logs:/app/logs
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "python", "-c", "import requests; requests.get('http://localhost:5000/api/health')"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

Run with Docker Compose:

```bash
# Start services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## Advanced Usage

### Custom Model Integration

```python
from mamey_image_bg.core.processor import BackgroundRemovalProcessor

# Use different models for different use cases
models = {
    'portraits': 'u2net_human_seg',
    'products': 'u2net_cloth_seg',
    'general': 'u2net',
    'fast': 'u2netp',
    'quality': 'isnet-general-use'
}

def process_with_appropriate_model(image_path, use_case='general'):
    processor = BackgroundRemovalProcessor(model_name=models[use_case])
    return processor.remove_background_from_file(image_path)
```

### Performance Optimization

```python
import asyncio
from concurrent.futures import ThreadPoolExecutor
from mamey_image_bg import BackgroundRemovalProcessor

async def process_images_async(image_paths, max_workers=4):
    """Process multiple images concurrently."""
    processor = BackgroundRemovalProcessor()
    
    with ThreadPoolExecutor(max_workers=max_workers) as executor:
        loop = asyncio.get_event_loop()
        tasks = [
            loop.run_in_executor(
                executor, 
                processor.remove_background_from_file, 
                path
            )
            for path in image_paths
        ]
        results = await asyncio.gather(*tasks)
    
    return results

# Usage
image_paths = ["img1.jpg", "img2.jpg", "img3.jpg"]
results = asyncio.run(process_images_async(image_paths))
```

### Integration with Web Frameworks

#### Flask Integration
```python
from flask import Flask, request, send_file
from mamey_image_bg import BackgroundRemovalProcessor
import io

app = Flask(__name__)
processor = BackgroundRemovalProcessor()

@app.route('/remove-background', methods=['POST'])
def remove_background():
    if 'file' not in request.files:
        return {'error': 'No file provided'}, 400
    
    file = request.files['file']
    if file.filename == '':
        return {'error': 'No file selected'}, 400
    
    # Process image
    result_data = processor.remove_background(file.read(), "PNG")
    
    return send_file(
        io.BytesIO(result_data),
        mimetype='image/png',
        as_attachment=True,
        download_name=f'no_bg_{file.filename}'
    )

if __name__ == '__main__':
    app.run(debug=True)
```

#### Django Integration
```python
from django.http import HttpResponse
from django.views.decorators.csrf import csrf_exempt
from mamey_image_bg import BackgroundRemovalProcessor
import json

processor = BackgroundRemovalProcessor()

@csrf_exempt
def remove_background(request):
    if request.method != 'POST':
        return HttpResponse('Method not allowed', status=405)
    
    if 'file' not in request.FILES:
        return HttpResponse('No file provided', status=400)
    
    file = request.FILES['file']
    
    # Process image
    result_data = processor.remove_background(file.read(), "PNG")
    
    response = HttpResponse(result_data, content_type='image/png')
    response['Content-Disposition'] = f'attachment; filename="no_bg_{file.name}"'
    return response
```

## API Reference

### BackgroundRemovalProcessor

#### Constructor
```python
BackgroundRemovalProcessor(model_name: str = "u2net")
```

#### Methods

##### `remove_background(input_data, output_format="PNG")`
Remove background from image data.

**Parameters:**
- `input_data` (bytes or PIL.Image): Image data to process
- `output_format` (str): Output format ("PNG" or "JPEG")

**Returns:** bytes - Processed image data

**Example:**
```python
processor = BackgroundRemovalProcessor()
result = processor.remove_background(image_bytes, "PNG")
```

##### `remove_background_from_file(input_path, output_path=None, output_format="PNG")`
Remove background from image file.

**Parameters:**
- `input_path` (str): Path to input image file
- `output_path` (str, optional): Path for output file
- `output_format` (str): Output format ("PNG" or "JPEG")

**Returns:** str - Path to processed file

**Example:**
```python
processor = BackgroundRemovalProcessor()
output = processor.remove_background_from_file("input.jpg", "output.png")
```

##### `get_supported_models()`
Get list of supported models.

**Returns:** list - List of model names

**Example:**
```python
processor = BackgroundRemovalProcessor()
models = processor.get_supported_models()
print(models)  # ['u2net', 'u2net_human_seg', ...]
```

### Configuration Classes

#### Settings
```python
class Settings:
    host: str = "0.0.0.0"
    port: int = 5000
    debug: bool = False
    model_name: str = "u2net"
    max_image_size: int = 1024
    quality: int = 95
    log_level: str = "INFO"
```

## Development

### Setup Development Environment

```bash
# Clone repository
git clone <repository-url>
cd Mamey.Image.BackgroundRemoval

# Create virtual environment
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate

# Install in development mode
pip install -e .

# Install development dependencies
pip install -r requirements-dev.txt
```

### Running Tests

```bash
# Run all tests
pytest

# Run with coverage
pytest --cov=mamey_image_bg --cov-report=html

# Run specific test file
pytest tests/test_processor.py

# Run with verbose output
pytest -v

# Run specific test
pytest tests/test_processor.py::TestBackgroundRemovalProcessor::test_remove_background_success
```

### Code Quality

```bash
# Format code
black src/ tests/

# Sort imports
isort src/ tests/

# Lint code
flake8 src/ tests/

# Type checking
mypy src/
```

### Building and Publishing

```bash
# Build package
python setup.py sdist bdist_wheel

# Check package
twine check dist/*

# Upload to PyPI (if configured)
twine upload dist/*
```

## Performance

### Benchmarks

| Image Size | Model | Processing Time | Memory Usage |
|------------|-------|-----------------|--------------|
| 512x512 | u2net | ~1-2s | ~500MB |
| 1024x1024 | u2net | ~2-4s | ~800MB |
| 2048x2048 | u2net | ~5-8s | ~1.2GB |
| 512x512 | u2netp | ~0.5-1s | ~300MB |
| 1024x1024 | u2netp | ~1-2s | ~500MB |

### Optimization Tips

1. **Use appropriate model**: Choose model based on use case
2. **Resize large images**: Use `max_image_size` setting
3. **Enable GPU**: Install `rembg[gpu]` for better performance
4. **Batch processing**: Process multiple images together
5. **Memory management**: Process images in smaller batches

### Monitoring

```python
import time
import psutil
from mamey_image_bg import BackgroundRemovalProcessor

def benchmark_processing(image_path):
    processor = BackgroundRemovalProcessor()
    
    # Monitor memory usage
    process = psutil.Process()
    memory_before = process.memory_info().rss / 1024 / 1024  # MB
    
    start_time = time.time()
    result = processor.remove_background_from_file(image_path)
    end_time = time.time()
    
    memory_after = process.memory_info().rss / 1024 / 1024  # MB
    
    print(f"Processing time: {end_time - start_time:.2f}s")
    print(f"Memory usage: {memory_after - memory_before:.2f}MB")
    
    return result
```

## Troubleshooting

### Common Issues

#### 1. Out of Memory Error
**Problem:** `RuntimeError: CUDA out of memory`

**Solutions:**
```bash
# Use smaller model
mamey-image-bg --model u2netp remove input.jpg output.png

# Reduce image size
export MAMEY_IMAGE_BG_MAX_IMAGE_SIZE=512

# Use CPU instead of GPU
pip uninstall rembg[gpu]
pip install rembg
```

#### 2. Slow Processing
**Problem:** Images take too long to process

**Solutions:**
```bash
# Use lightweight model
mamey-image-bg --model u2netp remove input.jpg output.png

# Enable GPU support
pip install rembg[gpu]

# Process smaller images
export MAMEY_IMAGE_BG_MAX_IMAGE_SIZE=512
```

#### 3. Poor Quality Results
**Problem:** Background not removed properly

**Solutions:**
```bash
# Try different model
mamey-image-bg --model u2net_human_seg remove portrait.jpg output.png

# Use high accuracy model
mamey-image-bg --model isnet-general-use remove input.jpg output.png

# Increase image quality
export MAMEY_IMAGE_BG_QUALITY=98
```

#### 4. Service Not Starting
**Problem:** API server fails to start

**Solutions:**
```bash
# Check port availability
netstat -tulpn | grep :5000

# Use different port
mamey-image-bg serve --port 8080

# Check logs
MAMEY_IMAGE_BG_LOG_LEVEL=DEBUG mamey-image-bg serve
```

### Debug Mode

Enable debug logging for troubleshooting:

```bash
# CLI debug
MAMEY_IMAGE_BG_LOG_LEVEL=DEBUG mamey-image-bg remove input.jpg output.png

# API server debug
MAMEY_IMAGE_BG_LOG_LEVEL=DEBUG mamey-image-bg serve

# Python library debug
import logging
logging.basicConfig(level=logging.DEBUG)
```

### Log Analysis

```bash
# View logs in real-time
tail -f logs/mamey-image-bg.log

# Filter error logs
grep "ERROR" logs/mamey-image-bg.log

# Monitor performance
grep "Processing time" logs/mamey-image-bg.log
```

### Health Checks

```bash
# Check service health
curl http://localhost:5000/api/health

# Check available models
curl http://localhost:5000/api/models

# Test image processing
curl -X POST "http://localhost:5000/api/remove-background" \
  -F "file=@test.jpg" \
  -F "output_format=PNG" \
  -o test_output.png
```

## License

Proprietary - Copyright (c) 2025 Mamey.io

## Support

For support and questions:
- 📧 Email: support@mamey.io
- 📚 Documentation: [Mamey.io Docs](https://docs.mamey.io)
- 🐛 Issues: [GitHub Issues](https://github.com/Mamey-io/Mamey/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/Mamey-io/Mamey/discussions)
