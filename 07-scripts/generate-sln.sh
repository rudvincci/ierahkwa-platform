#!/bin/bash
#
# 🦅 generate-sln.sh — Auto-discover all .NET projects and generate unified .sln
#
# Usage: cd ~/Desktop/Mamey-main && bash generate-sln.sh
#
set -euo pipefail

MAMEY="${1:-$(pwd)}"
SLN="$MAMEY/IerahkwaMamey.sln"
CSHARP_GUID="FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
FOLDER_GUID="2150E333-8FDC-42A3-9474-1A3956D46DE8"

echo "🦅 Scanning for .csproj files in $MAMEY..."
echo ""

# Find all .csproj files, excluding bin/obj/node_modules
PROJECTS=$(find "$MAMEY" -name "*.csproj" \
    ! -path "*/bin/*" \
    ! -path "*/obj/*" \
    ! -path "*/node_modules/*" \
    ! -path "*/.git/*" \
    2>/dev/null | sort)

COUNT=$(echo "$PROJECTS" | grep -c '.' || echo 0)
echo "Found $COUNT .csproj files"
echo ""

if [ "$COUNT" -eq 0 ]; then
    echo "⚠ No .csproj files found. Nothing to generate."
    exit 0
fi

# Categorize projects by folder
declare -A CATEGORIES
declare -A CAT_GUIDS

CAT_GUIDS[Banking]="A0000001-0000-0000-0000-000000000001"
CAT_GUIDS[Government]="A0000002-0000-0000-0000-000000000001"
CAT_GUIDS[Finance]="A0000003-0000-0000-0000-000000000001"
CAT_GUIDS[Tech]="A0000004-0000-0000-0000-000000000001"
CAT_GUIDS[Business]="A0000005-0000-0000-0000-000000000001"
CAT_GUIDS[Infra]="A0000006-0000-0000-0000-000000000001"
CAT_GUIDS[Education]="A0000007-0000-0000-0000-000000000001"
CAT_GUIDS[Core]="A0000008-0000-0000-0000-000000000001"
CAT_GUIDS[Other]="A0000009-0000-0000-0000-000000000001"

# Generate deterministic GUID from path
make_guid() {
    echo "$1" | md5sum | sed 's/^\(........\)\(....\)\(....\)\(....\)\(............\).*/\1-\2-\3-\4-\5/' | tr 'a-f' 'A-F'
}

categorize() {
    local path="$1"
    if echo "$path" | grep -qi "government"; then echo "Government"
    elif echo "$path" | grep -qi "finance\|bank\|defi\|bridge\|ido\|asset\|budget\|transaction"; then echo "Finance"
    elif echo "$path" | grep -qi "tech\|biometric\|audit\|ai\|crm\|mobile\|image\|app"; then echo "Tech"
    elif echo "$path" | grep -qi "business\|casino\|atm\|travel\|agriculture\|inventory\|product\|venturi"; then echo "Business"
    elif echo "$path" | grep -qi "infra\|deploy\|config\|data\|network\|systemd"; then echo "Infra"
    elif echo "$path" | grep -qi "education\|school"; then echo "Education"
    elif echo "$path" | grep -qi "Banking\.\|RuddieSolution"; then echo "Banking"
    elif echo "$path" | grep -qi "PlataformaFinal\|plataformas-finales"; then echo "Core"
    else echo "Other"
    fi
}

# Start writing .sln
cat > "$SLN" << 'HEADER'

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.10.0
MinimumVisualStudioVersion = 10.0.40219.1

# 🦅 Ierahkwa Mamey FutureHead — Auto-generated unified solution
# Generated: GENDATE
# Projects: PROJCOUNT

HEADER

sed -i "s/GENDATE/$(date '+%Y-%m-%d %H:%M')/" "$SLN"
sed -i "s/PROJCOUNT/$COUNT/" "$SLN"

# Write solution folders
for cat in Banking Government Finance Tech Business Infra Education Core Other; do
    guid="${CAT_GUIDS[$cat]}"
    echo "Project(\"{$FOLDER_GUID}\") = \"$cat\", \"$cat\", \"{$guid}\"" >> "$SLN"
    echo "EndProject" >> "$SLN"
done

echo "" >> "$SLN"

# Track nesting
NESTING=""

# Write each project
while IFS= read -r csproj; do
    [ -z "$csproj" ] && continue

    # Relative path from Mamey-main
    relpath="${csproj#$MAMEY/}"
    # Convert to backslash for .sln
    slnpath=$(echo "$relpath" | tr '/' '\\')
    # Project name
    name=$(basename "$csproj" .csproj)
    # Generate GUID
    guid=$(make_guid "$relpath")
    # Categorize
    cat=$(categorize "$relpath")
    cat_guid="${CAT_GUIDS[$cat]}"

    echo "  ✓ [$cat] $name"

    echo "Project(\"{$CSHARP_GUID}\") = \"$name\", \"$slnpath\", \"{$guid}\"" >> "$SLN"
    echo "EndProject" >> "$SLN"

    NESTING="$NESTING		{$guid} = {$cat_guid}
"
done <<< "$PROJECTS"

# Write Global section
cat >> "$SLN" << GLOBAL

Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
GLOBAL

# Build configs for each project
while IFS= read -r csproj; do
    [ -z "$csproj" ] && continue
    relpath="${csproj#$MAMEY/}"
    guid=$(make_guid "$relpath")
    cat >> "$SLN" << CONF
		{$guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{$guid}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{$guid}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{$guid}.Release|Any CPU.Build.0 = Release|Any CPU
CONF
done <<< "$PROJECTS"

cat >> "$SLN" << NESTED
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
$NESTING	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
NESTED

echo ""
echo "✅ Generated: $SLN"
echo "   $COUNT projects across $(echo "${!CAT_GUIDS[@]}" | wc -w) categories"
echo ""
echo "Open in Rider/VS:"
echo "   open \"$SLN\""
echo ""
