# EVIDENCIA DE PRUEBAS — 100% PRODUCTION

**Sovereign Government of Ierahkwa Ne Kanienke — Office of the Prime Minister**

**Fecha de generación:** 2026-02-04T21:55:45Z

---

## Los números no mienten

Este documento se genera ejecutando las pruebas automáticas. Cada número proviene de la verificación real del código, enlaces, rutas y datos.

### Resultado de la verificación 100% production

| Prueba | Verificados | Fallos | Estado |
|--------|-------------|--------|--------|
| **Links y botones** (platform-links) | 72 | **0** | ✅ OK |
| **Plataformas HTML** (front office) | 137 | **0** | ✅ OK |
| **Rutas back office** (platform-routes) | 114 | **0** | ✅ OK |
| **Archivos data** (configuración) | 5 | **0** | ✅ OK |
| **Departamentos** (government-departments) | 41 | — | ✅ |
| **Servicios backend** (services-ports.json) | 29 | — | ✅ |

**Total de fallos en verificación:** **0**

**Conclusión:** ✅ **0 fallos.** La plataforma cumple 100% en links, rutas, páginas y data.

---

### Resumen en una tabla (evidencia)

| Métrica | Valor |
|---------|-------|
| Links OK | 72 |
| Links rotos | 0 |
| Páginas platform | 137 |
| Rutas back office OK | 114 |
| Rutas rotas | 0 |
| Archivos data OK | 5 |
| Data faltantes | 0 |
| Departamentos | 41 |
| Servicios configurados | 29 |
| **Fallos totales** | **0** |

---

### Test de salud (servicios en ejecución)

*Estos números dependen de que los servidores estén encendidos (`./start.sh`).*

| Métrica | Valor |
|---------|-------|
| Total probados | 190 |
| Saludables | 3 |
| Caídos | 186 |
| No saludables | 1 |
| Latencia media (ms) | 1 |

---

## Cómo reproducir esta evidencia

```bash
# 1. Verificación 100% production (links, rutas, data, plataformas)
node scripts/verificar-plataforma-100-production.js

# 2. Generar este documento de evidencia con los números actuales
node scripts/generar-evidencia-pruebas-100.js

# 3. (Opcional) Test de salud de APIs y páginas (requiere servidores encendidos)
node scripts/test-toda-plataforma.js
```

---

*Documento generado por `scripts/generar-evidencia-pruebas-100.js`. Los números provienen de las pruebas ejecutadas.*