#!/bin/bash
# ============================================================================
# WIFI SOBERANO - TEST E2E (End-to-End)
# Simula el flujo completo de un usuario WiFi:
#   Conectar → Ver planes → Pagar → Navegar → Sesión expira
# ============================================================================

set -uo pipefail

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

WIFI_URL="${WIFI_URL:-http://127.0.0.1:3095}"
CORE_URL="${CORE_URL:-http://127.0.0.1:3050}"
PASSED=0
FAILED=0
TOTAL=0

# ── Helpers ───────────────────────────────────────────────────
pass() {
  ((PASSED++))
  ((TOTAL++))
  echo -e "  ${GREEN}✓${NC} $1"
}

fail() {
  ((FAILED++))
  ((TOTAL++))
  echo -e "  ${RED}✗${NC} $1"
  [ -n "${2:-}" ] && echo -e "    ${RED}→ $2${NC}"
}

test_endpoint() {
  local name="$1"
  local method="$2"
  local url="$3"
  local expected_status="${4:-200}"
  local body="${5:-}"
  local response

  if [ "$method" = "GET" ]; then
    response=$(curl -s -w "\n%{http_code}" --max-time 10 "$url" 2>/dev/null)
  else
    response=$(curl -s -w "\n%{http_code}" --max-time 10 -X "$method" \
      -H "Content-Type: application/json" \
      -d "$body" \
      "$url" 2>/dev/null)
  fi

  local http_code
  http_code=$(echo "$response" | tail -1)
  local response_body
  response_body=$(echo "$response" | sed '$d')

  if [ "$http_code" = "$expected_status" ]; then
    pass "$name (HTTP $http_code)"
    echo "$response_body"
    return 0
  else
    fail "$name" "Expected $expected_status, got $http_code"
    echo "$response_body"
    return 1
  fi
}

# ── Header ────────────────────────────────────────────────────
echo -e "\n${CYAN}"
echo "📡 ═══════════════════════════════════════════════════════════"
echo "   WIFI SOBERANO — TEST E2E"
echo "   Simulación completa del flujo de usuario WiFi"
echo "═══════════════════════════════════════════════════════════"
echo -e "${NC}"
echo "  WiFi Service: $WIFI_URL"
echo "  Core Service: $CORE_URL"
echo ""

# ══════════════════════════════════════════════════════════════
# TEST 1: Health Checks
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [1/8] Health Checks ──${NC}"

# WiFi Soberano health
HEALTH=$(test_endpoint "WiFi Soberano /health" GET "${WIFI_URL}/health" 200) && {
  echo "$HEALTH" | jq -r '.status // empty' 2>/dev/null | grep -q "ok" && \
    pass "WiFi Soberano status: ok" || \
    warn "WiFi Soberano health response unexpected"
} || true

# Sovereign Core health
test_endpoint "Sovereign Core /health" GET "${CORE_URL}/health" 200 > /dev/null || true

# Bridge status
test_endpoint "WiFi Bridge /v1/wifi/status" GET "${CORE_URL}/v1/wifi/status" 200 > /dev/null || true

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 2: Captive Portal Detection
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [2/8] Captive Portal Detection ──${NC}"

# Apple detection
APPLE_CODE=$(curl -s -o /dev/null -w '%{http_code}' --max-time 5 "${WIFI_URL}/hotspot-detect.html" 2>/dev/null)
[ "$APPLE_CODE" = "302" ] || [ "$APPLE_CODE" = "200" ] && \
  pass "Apple captive portal detection (HTTP $APPLE_CODE)" || \
  fail "Apple captive portal detection" "HTTP $APPLE_CODE"

# Android detection
ANDROID_CODE=$(curl -s -o /dev/null -w '%{http_code}' --max-time 5 "${WIFI_URL}/generate_204" 2>/dev/null)
[ "$ANDROID_CODE" = "302" ] || [ "$ANDROID_CODE" = "204" ] && \
  pass "Android captive portal detection (HTTP $ANDROID_CODE)" || \
  fail "Android captive portal detection" "HTTP $ANDROID_CODE"

# Microsoft detection
MS_CODE=$(curl -s -o /dev/null -w '%{http_code}' --max-time 5 "${WIFI_URL}/connecttest.txt" 2>/dev/null)
[ "$MS_CODE" = "302" ] || [ "$MS_CODE" = "200" ] && \
  pass "Microsoft captive portal detection (HTTP $MS_CODE)" || \
  fail "Microsoft captive portal detection" "HTTP $MS_CODE"

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 3: WiFi Plans
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [3/8] Planes WiFi ──${NC}"

PLANS_RESPONSE=$(test_endpoint "GET /api/v1/wifi/plans" GET "${WIFI_URL}/api/v1/wifi/plans" 200) || true

if echo "$PLANS_RESPONSE" | jq -e '.plans' &>/dev/null; then
  PLAN_COUNT=$(echo "$PLANS_RESPONSE" | jq '.plans | length')
  pass "Planes disponibles: $PLAN_COUNT"

  # Guardar primer plan para test de pago
  PLAN_ID=$(echo "$PLANS_RESPONSE" | jq -r '.plans[0].id // "test-plan-1h"')
  PLAN_NAME=$(echo "$PLANS_RESPONSE" | jq -r '.plans[0].name // "1 Hora"')
  PLAN_PRICE=$(echo "$PLANS_RESPONSE" | jq -r '.plans[0].price_wampum // "9.99"')
  pass "Plan seleccionado: $PLAN_NAME (Ⓦ$PLAN_PRICE)"
else
  warn "Respuesta de planes sin formato esperado"
  PLAN_ID="test-plan-1h"
fi

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 4: WiFi Connection (Simular usuario)
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [4/8] Conexión WiFi ──${NC}"

# Simular MAC address
TEST_MAC="AA:BB:CC:DD:EE:$(printf '%02X' $((RANDOM % 256)))"
TEST_IP="192.168.1.$((RANDOM % 200 + 10))"

CONNECT_BODY="{\"mac_address\":\"${TEST_MAC}\",\"ip_address\":\"${TEST_IP}\",\"device_type\":\"test\",\"os\":\"Linux\",\"browser\":\"curl-e2e-test\"}"
CONNECT_RESPONSE=$(test_endpoint "POST /api/v1/wifi/connect" POST "${WIFI_URL}/api/v1/wifi/connect" 200 "$CONNECT_BODY") || true

if echo "$CONNECT_RESPONSE" | jq -e '.session_id' &>/dev/null; then
  SESSION_ID=$(echo "$CONNECT_RESPONSE" | jq -r '.session_id')
  pass "Sesión creada: $SESSION_ID"
else
  SESSION_ID="test-session-$(date +%s)"
  warn "Sesión simulada: $SESSION_ID"
fi

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 5: Payment Flow
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [5/8] Flujo de Pago ──${NC}"

PAYMENT_BODY="{\"plan_id\":\"${PLAN_ID}\",\"mac_address\":\"${TEST_MAC}\",\"payment_method\":\"wampum\",\"amount\":${PLAN_PRICE:-9.99}}"
PAYMENT_RESPONSE=$(test_endpoint "POST /api/v1/wifi/payment/create" POST "${WIFI_URL}/api/v1/wifi/payment/create" 200 "$PAYMENT_BODY") || \
  test_endpoint "POST /api/v1/wifi/payment/create" POST "${WIFI_URL}/api/v1/wifi/payment/create" 201 "$PAYMENT_BODY" || true

if echo "$PAYMENT_RESPONSE" | jq -e '.payment_id' &>/dev/null; then
  PAYMENT_ID=$(echo "$PAYMENT_RESPONSE" | jq -r '.payment_id')
  pass "Pago creado: $PAYMENT_ID"

  # Verificar pago
  VERIFY_BODY="{\"payment_id\":\"${PAYMENT_ID}\"}"
  test_endpoint "POST /api/v1/wifi/payment/verify" POST "${WIFI_URL}/api/v1/wifi/payment/verify" 200 "$VERIFY_BODY" > /dev/null || true
else
  warn "Flujo de pago retornó formato inesperado (puede ser simulado)"
fi

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 6: Session Status
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [6/8] Estado de Sesión ──${NC}"

SESSION_RESPONSE=$(test_endpoint "GET /api/v1/wifi/session/status" GET "${WIFI_URL}/api/v1/wifi/session/status?mac=${TEST_MAC}" 200) || true

if echo "$SESSION_RESPONSE" | jq -e '.status' &>/dev/null; then
  SESSION_STATUS=$(echo "$SESSION_RESPONSE" | jq -r '.status')
  pass "Estado de sesión: $SESSION_STATUS"

  if echo "$SESSION_RESPONSE" | jq -e '.expires_at' &>/dev/null; then
    EXPIRES=$(echo "$SESSION_RESPONSE" | jq -r '.expires_at')
    pass "Expira: $EXPIRES"
  fi

  if echo "$SESSION_RESPONSE" | jq -e '.data_used_mb' &>/dev/null; then
    DATA_USED=$(echo "$SESSION_RESPONSE" | jq -r '.data_used_mb')
    pass "Data usada: ${DATA_USED} MB"
  fi
else
  warn "Estado de sesión en formato inesperado"
fi

# Extend session
EXTEND_BODY="{\"mac_address\":\"${TEST_MAC}\",\"additional_hours\":1}"
test_endpoint "POST /api/v1/wifi/session/extend" POST "${WIFI_URL}/api/v1/wifi/session/extend" 200 "$EXTEND_BODY" > /dev/null || true

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 7: Admin Endpoints
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [7/8] Admin Endpoints ──${NC}"

test_endpoint "GET /api/v1/wifi/admin/dashboard" GET "${WIFI_URL}/api/v1/wifi/admin/dashboard" 200 > /dev/null || true
test_endpoint "GET /api/v1/wifi/admin/sessions" GET "${WIFI_URL}/api/v1/wifi/admin/sessions" 200 > /dev/null || true
test_endpoint "GET /api/v1/wifi/admin/revenue" GET "${WIFI_URL}/api/v1/wifi/admin/revenue" 200 > /dev/null || true
test_endpoint "GET /api/v1/wifi/admin/fleet" GET "${WIFI_URL}/api/v1/wifi/admin/fleet" 200 > /dev/null || true
test_endpoint "GET /api/v1/wifi/admin/analytics" GET "${WIFI_URL}/api/v1/wifi/admin/analytics" 200 > /dev/null || true

# Vigilancia
test_endpoint "GET /api/v1/wifi/admin/vigilancia" GET "${WIFI_URL}/api/v1/wifi/admin/vigilancia" 200 > /dev/null || true

echo ""

# ══════════════════════════════════════════════════════════════
# TEST 8: Bridge Endpoints (sovereign-core ↔ wifi-soberano)
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}── [8/8] Bridge Endpoints ──${NC}"

test_endpoint "Bridge: GET /v1/wifi/status" GET "${CORE_URL}/v1/wifi/status" 200 > /dev/null || true
test_endpoint "Bridge: GET /v1/wifi/plans" GET "${CORE_URL}/v1/wifi/plans" 200 > /dev/null || true
test_endpoint "Bridge: GET /v1/wifi/dashboard" GET "${CORE_URL}/v1/wifi/dashboard" 200 > /dev/null || true
test_endpoint "Bridge: GET /v1/wifi/sessions" GET "${CORE_URL}/v1/wifi/sessions" 200 > /dev/null || true
test_endpoint "Bridge: GET /v1/wifi/revenue" GET "${CORE_URL}/v1/wifi/revenue" 200 > /dev/null || true
test_endpoint "Bridge: GET /v1/wifi/fleet" GET "${CORE_URL}/v1/wifi/fleet" 200 > /dev/null || true

# User provisioning through bridge
PROVISION_BODY="{\"email\":\"test-e2e-$(date +%s)@soberano.test\",\"display_name\":\"E2E Test User\",\"mac_address\":\"${TEST_MAC}\",\"hotspot_id\":\"e2e-test\"}"
test_endpoint "Bridge: POST /v1/wifi/provision-user" POST "${CORE_URL}/v1/wifi/provision-user" 201 "$PROVISION_BODY" > /dev/null || \
  test_endpoint "Bridge: POST /v1/wifi/provision-user" POST "${CORE_URL}/v1/wifi/provision-user" 200 "$PROVISION_BODY" > /dev/null || true

# Alert forwarding
ALERT_BODY="{\"alert_type\":\"e2e-test\",\"severity\":\"low\",\"ip_address\":\"${TEST_IP}\",\"details\":\"E2E test alert — ignore\"}"
test_endpoint "Bridge: POST /v1/wifi/alert" POST "${CORE_URL}/v1/wifi/alert" 200 "$ALERT_BODY" > /dev/null || true

echo ""

# ══════════════════════════════════════════════════════════════
# RESUMEN
# ══════════════════════════════════════════════════════════════
echo -e "${CYAN}═══════════════════════════════════════════════════════════${NC}"
echo ""

if [ "$FAILED" -eq 0 ]; then
  echo -e "  ${GREEN}█████████████████████████████████████████████████${NC}"
  echo -e "  ${GREEN}  ✓ TODOS LOS TESTS PASARON: ${PASSED}/${TOTAL}${NC}"
  echo -e "  ${GREEN}█████████████████████████████████████████████████${NC}"
else
  echo -e "  ${YELLOW}  Resultados: ${GREEN}${PASSED} pasaron${NC} / ${RED}${FAILED} fallaron${NC} / ${TOTAL} total"
  if [ "$FAILED" -gt $((TOTAL / 2)) ]; then
    echo -e "  ${RED}  ⚠ Más de la mitad de los tests fallaron${NC}"
    echo -e "  ${RED}  Verificar que los servicios estén corriendo:${NC}"
    echo "    systemctl status wifi-soberano"
    echo "    systemctl status sovereign-core"
    echo "    docker ps"
  fi
fi

echo ""
echo "  Test MAC: $TEST_MAC"
echo "  Test IP:  $TEST_IP"
echo "  Fecha:    $(date)"
echo ""

exit $FAILED
