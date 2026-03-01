#!/usr/bin/env bash
#
# fix-dockerfiles.sh
# Fixes the empty COPY issue in Dockerfiles across 34 microservice directories.
#
# Problem:
#   COPY ["", "./"]
#   RUN dotnet restore ""
#   WORKDIR "/src/."
#
# Fix:
#   - Replace empty COPY with individual COPY lines for each .csproj file
#   - Replace empty restore with dotnet restore of the main project .csproj
#   - Replace WORKDIR "/src/." with WORKDIR pointing to the main project subdirectory
#
# The "main" project is determined by the DLL name in the ENTRYPOINT line.
#
set -euo pipefail

MICROSERVICES_DIR="$(cd "$(dirname "$0")/../08-dotnet/microservices" && pwd)"

# The 34 affected directories
SERVICES=(
  treasury
  notifications
  compliance
  WorkflowEngine
  VotingSystem
  TradeX
  TaxAuthority
  SpikeOffice
  ServiceDesk
  RnBCal
  ReportEngine
  ProjectHub
  ProcurementHub
  OutlookExtractor
  NotifyHub
  NFTCertificates
  MultichainBridge
  MeetingHub
  IDOFactory
  HRM
  GovernanceDAO
  FormBuilder
  FarmFactory
  ESignature
  DocumentFlow
  DigitalVault
  DataHub
  ContractManager
  CitizenCRM
  BudgetControl
  AuditTrail
  AssetTracker
  AppBuilder
  AIFraudDetection
)

FIXED=0
SKIPPED=0
ERRORS=0

for SERVICE in "${SERVICES[@]}"; do
  SERVICE_DIR="${MICROSERVICES_DIR}/${SERVICE}"
  DOCKERFILE="${SERVICE_DIR}/Dockerfile"

  # --- Validation ---
  if [[ ! -d "$SERVICE_DIR" ]]; then
    echo "WARNING: Directory not found: ${SERVICE_DIR} -- skipping"
    ((SKIPPED++))
    continue
  fi

  if [[ ! -f "$DOCKERFILE" ]]; then
    echo "WARNING: Dockerfile not found: ${DOCKERFILE} -- skipping"
    ((SKIPPED++))
    continue
  fi

  # Check if the Dockerfile actually has the empty COPY issue
  if ! grep -q 'COPY \["", "\./"\]' "$DOCKERFILE"; then
    echo "INFO: ${SERVICE}/Dockerfile does not have the empty COPY issue -- skipping"
    ((SKIPPED++))
    continue
  fi

  # --- Find .csproj files (exclude macOS ._ resource forks) ---
  # Collect real .csproj files: paths relative to SERVICE_DIR
  CSPROJ_FILES=()
  while IFS= read -r -d '' f; do
    # Get path relative to SERVICE_DIR
    REL_PATH="${f#${SERVICE_DIR}/}"
    # Skip macOS resource fork files (._*)
    BASENAME="$(basename "$REL_PATH")"
    if [[ "$BASENAME" == ._* ]]; then
      continue
    fi
    CSPROJ_FILES+=("$REL_PATH")
  done < <(find "$SERVICE_DIR" -name '*.csproj' -print0 2>/dev/null)

  if [[ ${#CSPROJ_FILES[@]} -eq 0 ]]; then
    echo "ERROR: No .csproj files found in ${SERVICE_DIR} -- skipping"
    ((ERRORS++))
    continue
  fi

  # --- Determine the main project from ENTRYPOINT ---
  # ENTRYPOINT line looks like: ENTRYPOINT ["dotnet", "SomeName.dll"]
  # or with ._  prefix:         ENTRYPOINT ["dotnet", "._Mamey.SICB.Treasury.dll"]
  ENTRYPOINT_LINE=$(grep '^ENTRYPOINT' "$DOCKERFILE" || true)
  if [[ -z "$ENTRYPOINT_LINE" ]]; then
    echo "ERROR: No ENTRYPOINT found in ${DOCKERFILE} -- skipping"
    ((ERRORS++))
    continue
  fi

  # Extract the DLL name (strip .dll suffix)
  # The DLL name is the second element in the JSON array
  DLL_NAME=$(echo "$ENTRYPOINT_LINE" | sed -E 's/.*"dotnet",\s*"([^"]+)".*/\1/')
  # Remove .dll suffix
  PROJECT_NAME="${DLL_NAME%.dll}"
  # Remove ._ prefix if present (macOS resource fork artifact in the ENTRYPOINT)
  PROJECT_NAME="${PROJECT_NAME#._}"

  # Find the main .csproj that matches PROJECT_NAME
  MAIN_CSPROJ=""
  for CSPROJ in "${CSPROJ_FILES[@]}"; do
    CSPROJ_BASENAME="$(basename "$CSPROJ" .csproj)"
    if [[ "$CSPROJ_BASENAME" == "$PROJECT_NAME" ]]; then
      MAIN_CSPROJ="$CSPROJ"
      break
    fi
  done

  if [[ -z "$MAIN_CSPROJ" ]]; then
    echo "ERROR: Could not find main .csproj for project '${PROJECT_NAME}' in ${SERVICE_DIR}"
    echo "       Available: ${CSPROJ_FILES[*]}"
    ((ERRORS++))
    continue
  fi

  # Determine the main project subdirectory (dirname of the main .csproj relative path)
  MAIN_PROJECT_DIR="$(dirname "$MAIN_CSPROJ")"

  # --- Build the replacement COPY lines ---
  # Each .csproj gets: COPY ["SubDir/Project.csproj", "SubDir/"]
  COPY_LINES=""
  for CSPROJ in "${CSPROJ_FILES[@]}"; do
    CSPROJ_DIR="$(dirname "$CSPROJ")"
    COPY_LINES="${COPY_LINES}COPY [\"${CSPROJ}\", \"${CSPROJ_DIR}/\"]"$'\n'
  done
  # Remove trailing newline
  COPY_LINES="${COPY_LINES%$'\n'}"

  # --- Build the replacement content ---
  # We need to replace three things in the Dockerfile:
  #   1. COPY ["", "./"]           ->  Multiple COPY lines for each .csproj
  #   2. RUN dotnet restore ""     ->  RUN dotnet restore "MainProject/MainProject.csproj"
  #   3. WORKDIR "/src/."          ->  WORKDIR "/src/MainProject"

  # Read the original Dockerfile
  ORIGINAL=$(<"$DOCKERFILE")

  # Replace COPY ["", "./"] with the proper COPY lines
  NEW_CONTENT=$(echo "$ORIGINAL" | sed '/^COPY \["", "\.\/"\]$/c\
__COPY_PLACEHOLDER__
')

  # Now replace the placeholder with actual COPY lines
  # We use a temp file approach since multi-line sed replacement is tricky
  TMPFILE=$(mktemp)
  while IFS= read -r line; do
    if [[ "$line" == "__COPY_PLACEHOLDER__" ]]; then
      echo "$COPY_LINES"
    else
      echo "$line"
    fi
  done <<< "$NEW_CONTENT" > "$TMPFILE"

  # Replace RUN dotnet restore ""
  sed -i.bak "s|^RUN dotnet restore \"\"$|RUN dotnet restore \"${MAIN_CSPROJ}\"|" "$TMPFILE"

  # Replace WORKDIR "/src/."
  sed -i.bak "s|^WORKDIR \"/src/\\.\"|WORKDIR \"/src/${MAIN_PROJECT_DIR}\"|" "$TMPFILE"

  # Also fix the ENTRYPOINT if it has the ._ prefix
  if echo "$ENTRYPOINT_LINE" | grep -q '"._'; then
    sed -i.bak "s|\"\\._${PROJECT_NAME}\\.dll\"|\"${PROJECT_NAME}.dll\"|" "$TMPFILE"
  fi

  # Write back
  cp "$TMPFILE" "$DOCKERFILE"
  rm -f "$TMPFILE" "${TMPFILE}.bak"

  echo "FIXED: ${SERVICE}/Dockerfile"
  echo "       Main project: ${PROJECT_NAME} (${MAIN_CSPROJ})"
  echo "       COPY lines: ${#CSPROJ_FILES[@]} .csproj file(s)"
  echo "       WORKDIR: /src/${MAIN_PROJECT_DIR}"
  ((FIXED++))
done

echo ""
echo "===== Summary ====="
echo "Fixed:   ${FIXED}"
echo "Skipped: ${SKIPPED}"
echo "Errors:  ${ERRORS}"
echo "Total:   ${#SERVICES[@]}"
