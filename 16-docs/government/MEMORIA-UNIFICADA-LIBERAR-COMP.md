# Memoria unificada — Liberar la comp para trabajar sin tanto peso

**Sovereign Government of Ierahkwa Ne Kanienke**  
Carpetas, proyectos y plataformas están unificados en **memoria** (índice + regla Cursor). Así no dependes de tener todo abierto o de recordar cada ruta.

---

## Dónde está la memoria

| Referencia | Qué contiene |
|------------|----------------|
| **`docs/INDICE-COMPLETO-PROYECTO-SOBERANOS.md`** | Carpetas (lista por grupos), proyectos (núcleo + .NET + otros), plataformas (dónde viven + APIs), scripts, puertos, documentos clave. |
| **`.cursor/rules/indice-completo-proyecto-soberanos.mdc`** | Misma estructura en resumen: carpetas, proyectos, plataformas; reglas (no borrar, principio todo propio, Extreme Pro, Ministry of Economy, data). |

El asistente (Cursor) usa la regla siempre. Tú puedes usar solo el índice cuando necesites la lista completa.

---

## Cómo trabajar sin peso

1. **Carpetas:** No hace falta abrir o explorar decenas de carpetas. La lista única está en el índice (Núcleo, APIs .NET, Infra, Comercio, Backup, Otros). Si dudas qué es una carpeta → índice.
2. **Proyectos:** Un proyecto = una carpeta (RuddieSolution es el núcleo; el resto son servicios/carpetas). Puertos y qué corre → `docs/TODO-LO-QUE-CORRE-ONLINE.md`.
3. **Plataformas:** No listar a mano. Lista dinámica: `GET http://localhost:8545/api/platform/all-pages`. Enlaces curados: `GET /api/platform/links`. Abrir todas en Chrome: `./scripts/abrir-todas-plataformas-chrome.sh`.
4. **Data:** Inventario único en `docs/DATA-QUE-TENEMOS.md`. Banco/bonos → `docs/CONECTAR-DATA-BANCO-Y-BONOS.md`.
5. **Extreme Pro:** Todo lo que va al disco = lo que está en el índice. Pasos en `docs/MUDAR-PROYECTO-A-EXTREME-PRO.md`.

---

## Resumen

- **Un solo lugar** para carpetas, proyectos y plataformas: **índice + regla**.
- **Liberar la comp** = no abrir todo; consultar el índice o la API cuando necesites la lista.
- **No borrar nada** hasta confirmar; toda carpeta es parte del proyecto.

*Última actualización: febrero 2026.*
