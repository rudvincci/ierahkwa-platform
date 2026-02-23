#!/bin/bash
# fix-generated-service.sh - Applies fixes to Mamey-generated microservices for Pupitre
# Usage: ./fix-generated-service.sh <SERVICE_NAME> <ENTITY_NAME>
# Example: ./fix-generated-service.sh Progress Progress

SERVICE_NAME=$1
ENTITY_NAME=$2
BASE_DIR="src/Services/Foundation/Pupitre.${SERVICE_NAME}/src"

if [ -z "$SERVICE_NAME" ] || [ -z "$ENTITY_NAME" ]; then
    echo "Usage: $0 <SERVICE_NAME> <ENTITY_NAME>"
    exit 1
fi

cd /Volumes/Barracuda/mamey-io/code-final/Pupitre

echo "Fixing Pupitre.${SERVICE_NAME}..."

# Fix csproj files - remove version attributes
for f in $(find "$BASE_DIR" -name "*.csproj" 2>/dev/null); do
    sed -i '' 's/ Version="2.0.[*0-9]*"//g' "$f" 2>/dev/null
    sed -i '' 's/ Version="9.0.[*0-9]*"//g' "$f" 2>/dev/null
done

# Fix Infrastructure csproj
cat > "$BASE_DIR/Pupitre.${SERVICE_NAME}.Infrastructure/Pupitre.${SERVICE_NAME}.Infrastructure.csproj" << EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>disable</Nullable>
    <NoWarn>CS0108;CS0168;CS8632;CS1591</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Pupitre.${SERVICE_NAME}.Application\Pupitre.${SERVICE_NAME}.Application.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Mamey" />
    <PackageReference Include="Mamey.Microservice.Infrastructure" />
    <PackageReference Include="Mamey.Persistence.PostgreSQL" />
    <PackageReference Include="Mamey.Persistence.MongoDB" />
    <PackageReference Include="Mamey.MessageBrokers.RabbitMQ" />
    <PackageReference Include="Mamey.WebApi" />
  </ItemGroup>
</Project>
EOF

# Fix Contracts/Commands/Update*.cs
UPDATE_CMD="$BASE_DIR/Pupitre.${SERVICE_NAME}.Contracts/Commands/Update${ENTITY_NAME}.cs"
if [ -f "$UPDATE_CMD" ]; then
    sed -i '' 's/public string Name { get; init; }/public string Name { get; init; } = string.Empty;/' "$UPDATE_CMD" 2>/dev/null
    sed -i '' 's/public IEnumerable<string> Tags { get; init; }/public IEnumerable<string> Tags { get; init; } = Array.Empty<string>();/' "$UPDATE_CMD" 2>/dev/null
fi

# Fix Application/Commands/Handlers/Add*Handler.cs
ADD_HANDLER="$BASE_DIR/Pupitre.${SERVICE_NAME}.Application/Commands/Handlers/Add${ENTITY_NAME}Handler.cs"
if [ -f "$ADD_HANDLER" ]; then
    sed -i '' "s/${ENTITY_NAME}.Create(command.Id, command.Name, tags: command.Tags)/${ENTITY_NAME}.Create(command.Id, command.Name ?? string.Empty, tags: command.Tags)/" "$ADD_HANDLER" 2>/dev/null
fi

# Fix Infrastructure Extension files - add using Mamey; if missing
for f in Extensions.cs EF/Extensions.cs Mongo/Extensions.cs; do
    FILE="$BASE_DIR/Pupitre.${SERVICE_NAME}.Infrastructure/$f"
    if [ -f "$FILE" ]; then
        if ! grep -q "^using Mamey;$" "$FILE" 2>/dev/null; then
            sed -i '' '1s/^/using Mamey;\n/' "$FILE" 2>/dev/null
        fi
    fi
done

# Fix ExceptionToMessageMapper
MAPPER="$BASE_DIR/Pupitre.${SERVICE_NAME}.Infrastructure/Exceptions/ExceptionToMessageMapper.cs"
if [ -f "$MAPPER" ]; then
    sed -i '' 's/public object? Map/public object Map/' "$MAPPER" 2>/dev/null
    # Fix null returns - be careful with the pattern
    sed -i '' 's/_ => null$/_ => null!/' "$MAPPER" 2>/dev/null
fi

# Fix API Routes - remove duplicate using and add null forgiving operators
ROUTES="$BASE_DIR/Pupitre.${SERVICE_NAME}.Api/${ENTITY_NAME}Routes.cs"
if [ -f "$ROUTES" ]; then
    # Remove duplicate using Mamey.Types;
    awk '!seen[$0]++' "$ROUTES" > "$ROUTES.tmp" && mv "$ROUTES.tmp" "$ROUTES"
fi

echo "Done fixing Pupitre.${SERVICE_NAME}"
