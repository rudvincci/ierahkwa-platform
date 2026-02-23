#\!/bin/bash
echo "Iniciando VozSoberana..."
cd "$(dirname "$0")"
npm install 2>/dev/null
echo "Abriendo http://localhost:3002"
node server.js
