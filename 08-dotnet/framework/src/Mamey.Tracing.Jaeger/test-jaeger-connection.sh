#!/bin/bash

# Test script to verify Jaeger connectivity and UDP port availability

echo "=== Jaeger Connectivity Test ==="
echo ""

# Check if Jaeger is running (Docker)
echo "1. Checking if Jaeger container is running..."
if docker ps | grep -q jaeger; then
    echo "   ✓ Jaeger container is running"
    docker ps | grep jaeger
else
    echo "   ✗ Jaeger container is NOT running"
    echo "   To start Jaeger, run:"
    echo "   docker run -d --name jaeger \\"
    echo "     -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 \\"
    echo "     -p 5775:5775/udp \\"
    echo "     -p 6831:6831/udp \\"
    echo "     -p 6832:6832/udp \\"
    echo "     -p 5778:5778 \\"
    echo "     -p 16686:16686 \\"
    echo "     -p 14268:14268 \\"
    echo "     -p 14250:14250 \\"
    echo "     -p 9411:9411 \\"
    echo "     jaegertracing/all-in-one:latest"
fi

echo ""

# Check if UDP port 6831 is available
echo "2. Checking UDP port 6831 availability..."
if command -v nc &> /dev/null; then
    if nc -zv -u localhost 6831 2>&1 | grep -q "succeeded\|open"; then
        echo "   ✓ UDP port 6831 is available"
    else
        echo "   ✗ UDP port 6831 is NOT available"
    fi
else
    echo "   ⚠ netcat (nc) not installed, cannot test UDP port"
fi

echo ""

# Check if Jaeger UI is accessible
echo "3. Checking Jaeger UI (HTTP)..."
if command -v curl &> /dev/null; then
    if curl -s -o /dev/null -w "%{http_code}" http://localhost:16686 | grep -q "200"; then
        echo "   ✓ Jaeger UI is accessible at http://localhost:16686"
    else
        echo "   ✗ Jaeger UI is NOT accessible"
    fi
else
    echo "   ⚠ curl not installed, cannot test HTTP"
fi

echo ""

# Check if Jaeger API is accessible
echo "4. Checking Jaeger API..."
if command -v curl &> /dev/null; then
    RESPONSE=$(curl -s http://localhost:16686/api/services)
    if [ ! -z "$RESPONSE" ]; then
        echo "   ✓ Jaeger API is accessible"
        echo "   Services found:"
        echo "$RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$RESPONSE"
    else
        echo "   ✗ Jaeger API is NOT accessible"
    fi
else
    echo "   ⚠ curl not installed, cannot test API"
fi

echo ""
echo "=== Test Complete ==="

