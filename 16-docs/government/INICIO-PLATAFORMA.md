# Cómo hacer que la plataforma trabaje

**Sovereign Government of Ierahkwa Ne Kanienke**

## Arrancar la plataforma (un solo comando)

Desde la raíz del proyecto:

```bash
./start.sh
```

Eso inicia el servidor Node (puerto **8545**), el Banking Bridge y, si tienes PM2, los procesos gestionados.

## Si no usas PM2: solo Node

```bash
cd RuddieSolution/node
node server.js
```

Deja la terminal abierta. Cuando veas algo como:

```
═══════════════════════════════════════════════════════════════════════════════
  IERAHKWA FUTUREHEAD MAMEY NODE listening on port 8545
═══════════════════════════════════════════════════════════════════════════════
```

la plataforma está en marcha.

## Abrir la plataforma en el navegador

- **Plataforma principal:** http://localhost:8545/platform/
- **Admin:** http://localhost:8545/platform/admin.html
- **Banco BDET:** http://localhost:8545/bdet-bank
- **Health:** http://localhost:8545/health

## Comprobar que todo va bien

```bash
./status.sh
```

O visita: http://localhost:8545/health

## Qué se corrigió (por si “no trabajaba”)

- **AI Hub:** El archivo `RuddieSolution/node/data/ai-hub/atabey/ai-tasks.json` tenía JSON corrupto al final (`}ailed": []}`). Se corrigió; el AI Hub ya puede iniciar sin ese error.

Si algo sigue fallando, revisa que Node.js sea 18+ (`node -v`) y que en `RuddieSolution/node` estén instaladas las dependencias (`npm install`).
