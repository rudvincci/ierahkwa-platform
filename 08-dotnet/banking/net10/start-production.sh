#!/bin/bash

# ════════════════════════════════════════════════════════════════
# IERAHKWA NET10 - PRODUCTION START SCRIPT
# Sovereign Government DeFi Platform
# ════════════════════════════════════════════════════════════════

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="$SCRIPT_DIR/NET10.API"
ENVIRONMENT=${1:-Production}
PORT=${2:-5071}

echo "╔═══════════════════════════════════════════════════════════════╗"
echo "║                                                               ║"
echo "║   🌐 IERAHKWA NET10 DEFI PLATFORM                            ║"
echo "║   Starting Production Server...                              ║"
echo "║                                                               ║"
echo "╚═══════════════════════════════════════════════════════════════╝"
echo ""

# Verificar que estamos en el directorio correcto
if [ ! -f "$API_DIR/NET10.API.csproj" ]; then
    echo "❌ Error: No se encontró el proyecto NET10.API"
    echo "   Ubicación esperada: $API_DIR"
    exit 1
fi

# Cambiar al directorio de la API
cd "$API_DIR"

# Verificar que .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "❌ Error: .NET SDK no está instalado"
    exit 1
fi

echo "✅ Verificando compilación..."
dotnet build --configuration Release --no-incremental > /dev/null 2>&1

if [ $? -ne 0 ]; then
    echo "❌ Error: La compilación falló"
    echo "   Ejecutando compilación con salida detallada..."
    dotnet build --configuration Release
    exit 1
fi

echo "✅ Compilación exitosa"
echo ""

# Configurar variables de entorno
export ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
export ASPNETCORE_URLS="http://0.0.0.0:$PORT"

echo "📊 Configuración:"
echo "   - Entorno: $ENVIRONMENT"
echo "   - Puerto: $PORT"
echo "   - URL: http://0.0.0.0:$PORT"
echo ""

# Crear directorio de logs si no existe
mkdir -p logs

echo "🚀 Iniciando servidor..."
echo "   Presiona Ctrl+C para detener"
echo ""

# Iniciar el servidor
dotnet run --configuration Release --no-build --urls "http://0.0.0.0:$PORT"
