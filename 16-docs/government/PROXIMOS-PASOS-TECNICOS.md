# Próximos pasos técnicos – IERAHKWA

**Fecha:** 28 de Enero de 2026  
**Gobierno Soberano de Ierahkwa Ne Kanienke**

---

## Checklist rápido

| # | Paso | Estado | Notas |
|---|------|--------|--------|
| 1 | **Node.js 18+** en el servidor donde corre Node | Pendiente | Requerido para OpenAI y logger. Ver `DIAGNOSTICO-AGENTES-2026-01-28.md`. |
| 2 | **Copiar PDF** de soberanía a `docs/legal/` | Pendiente | Ver `docs/legal/README.md`. |
| 3 | **Arrancar sistema** con `./start.sh` | Listo | PM2 o procesos en background. |
| 4 | **Conectar internet** (Cisco, Fortinet, port forwarding) | Pendiente | Ver `PLAN-LIVE-PRODUCTION-100.md`. |
| 5 | **Desplegar en servidores físicos** (opcional) | Pendiente | `./scripts/deploy-a-servidores-fisicos.sh` o manual. |
| 6 | **Backups** de agentes | Listo | `./scripts/backup-agentes.sh`. |

---

## 1. Node 18+ (importante)

En la máquina donde ejecutas `server.js` y `banking-bridge.js`:

```bash
node -v   # Debe ser v18.0.0 o superior
```

Si es menor (p. ej. 14.x), actualizar Node a 18+ (nvm, instalador oficial o package manager). Sin esto, la IA y el logger pueden fallar.

---

## 2. Arrancar todo

Desde la raíz del proyecto:

```bash
./start.sh
```

- Si tienes **PM2**: arranca `server.js` (8545) y `banking-bridge.js` (3001) con ecosystem.
- Si no: arranca en background con nohup.

Verificar:

- http://localhost:8545/health  
- http://localhost:8545/platform/index.html  

---

## 3. Live production (internet)

1. Conectar modem → Cisco → Fortinet (si aplica) → servidores.
2. Port forwarding: 80, 443, 8545, 3001 → IP del servidor Node.
3. Detalle: **PLAN-LIVE-PRODUCTION-100.md**, **PLANOS-VISUALES.md**.

---

## 4. Despliegue en servidores físicos

- Script: **scripts/deploy-a-servidores-fisicos.sh**
- Guía: **INSTALAR-EN-SERVIDORES-FISICOS.md**

---

## 5. Documentación de referencia

| Documento | Uso |
|-----------|-----|
| **MAPA-CODIGO-NODE-BLOCKCHAIN-Y-DEMAS.md** | Dónde está cada código (Node, blockchain, resto). |
| **REFERENCIA-RAPIDA-CODIGO.md** | Rutas rápidas del código propio. |
| **docs/SOBERANIA-RECONOCIMIENTO-LEGAL.md** | Soberanía, reconocimiento, visión Américas. |
| **docs/legal/README.md** | Dónde guardar el PDF de legalidad. |
| **PLAN-LIVE-PRODUCTION-100.md** | Pasos para 100% en vivo. |
| **DIAGNOSTICO-AGENTES-2026-01-28.md** | Problemas Node 14 y agentes. |

---

*Lista de próximos pasos – IERAHKWA.*
