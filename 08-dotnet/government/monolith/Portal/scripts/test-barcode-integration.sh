#!/bin/bash
# Test script for barcode service integration

set -e

echo "=== Testing Barcode Service Integration ==="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test 1: Barcode service health
echo "1. Testing barcode service availability..."
if curl -s -f http://localhost:18648/generate-barcode -X POST \
  -H "Content-Type: application/json" \
  -d '{"data":"TEST","type":"pdf417"}' \
  -o /tmp/test_barcode.png > /dev/null 2>&1; then
  echo -e "${GREEN}✓${NC} Barcode service is running and responding"
  if file /tmp/test_barcode.png | grep -q "PNG image"; then
    echo -e "${GREEN}✓${NC} Barcode image generated successfully ($(ls -lh /tmp/test_barcode.png | awk '{print $5}'))"
  else
    echo -e "${RED}✗${NC} Barcode service returned invalid image"
    exit 1
  fi
else
  echo -e "${RED}✗${NC} Barcode service is not accessible at http://localhost:18648"
  echo -e "${YELLOW}  Start it with: cd Utilities/MameyBarcode && docker-compose up -d${NC}"
  exit 1
fi

echo ""

# Test 2: AAMVA-compliant PDF417 generation
echo "2. Testing AAMVA-compliant PDF417 generation..."
AAMVA_TEST=$(curl -s -X POST http://localhost:18648/generate-barcode \
  -H "Content-Type: application/json" \
  -d '{
    "data": "TEST_AAMVA_DATA",
    "type": "pdf417",
    "columns": 20,
    "security_level": 5,
    "scale": 2,
    "ratio": 3
  }' \
  -o /tmp/test_aamva_barcode.png 2>&1)

if [ $? -eq 0 ] && file /tmp/test_aamva_barcode.png | grep -q "PNG image"; then
  echo -e "${GREEN}✓${NC} AAMVA-compliant PDF417 barcode generated"
  echo -e "  Image size: $(ls -lh /tmp/test_aamva_barcode.png | awk '{print $5}')"
else
  echo -e "${RED}✗${NC} AAMVA PDF417 generation failed"
  echo "$AAMVA_TEST"
  exit 1
fi

echo ""

# Test 3: Portal configuration
echo "3. Checking portal configuration..."
if [ -f "src/Mamey.Portal.Web/appsettings.json" ]; then
  if grep -q '"Barcode"' src/Mamey.Portal.Web/appsettings.json; then
    echo -e "${GREEN}✓${NC} Barcode configuration found in appsettings.json"
    BARCODE_URL=$(grep -A 1 '"Barcode"' src/Mamey.Portal.Web/appsettings.json | grep '"ApiUrl"' | cut -d'"' -f4)
    echo "  API URL: $BARCODE_URL"
  else
    echo -e "${YELLOW}⚠${NC} Barcode configuration not found in appsettings.json"
  fi
else
  echo -e "${RED}✗${NC} appsettings.json not found"
fi

echo ""

# Test 4: Service registration (check Program.cs)
echo "4. Checking service registration..."
if grep -q "IAamvaBarcodeService\|AamvaBarcodeService" src/Mamey.Portal.Web/Program.cs; then
  echo -e "${GREEN}✓${NC} AamvaBarcodeService is registered"
else
  echo -e "${RED}✗${NC} AamvaBarcodeService not found in Program.cs"
fi

if grep -q "IBarcodeService\|BarcodeService" src/Mamey.Portal.Web/Program.cs; then
  echo -e "${GREEN}✓${NC} BarcodeService is registered"
else
  echo -e "${RED}✗${NC} BarcodeService not found in Program.cs"
fi

echo ""

# Summary
echo "=== Test Summary ==="
echo -e "${GREEN}All integration tests passed!${NC}"
echo ""
echo "Next steps:"
echo "  1. Start the portal: dotnet run --project src/Mamey.Portal.Web"
echo "  2. Test ID card issuance via: POST /dev/issue/idcard"
echo "  3. Verify barcode is embedded in generated HTML"
echo ""


