# MVP — Comparativas de tasas de criminalidad

Diseño de producto mínimo viable para comparar tasas de criminalidad entre países, regiones o ciudades de las Américas.

---

## Objetivo

Permitir comparar indicadores de criminalidad (homicidios, Índice de Severidad del Delito, población carcelaria) entre países o regiones, usando **datos que el usuario introduce manualmente** desde fuentes oficiales (principio TODO PROPIO).

---

## Alcance MVP

### Funcionalidades (Fase 1)

| Funcionalidad | Descripción | Estado |
|---------------|-------------|--------|
| **Selección de entidades** | El usuario elige 2–5 países o regiones a comparar (dropdown o chips). | MVP |
| **Indicadores** | Homicidios por 100k habitantes, Índice de Severidad del Delito (Canadá), tasa de encarcelamiento. | MVP |
| **Entrada manual de datos** | Formulario para ingresar valores por entidad e indicador. Los datos vienen de fuentes oficiales que el usuario consulta (FBI CDE, Statistics Canada, UNODC, etc.). | MVP |
| **Visualización** | Tabla comparativa y/o gráfico de barras simple (HTML/CSS/JS, sin librerías externas). | MVP |
| **Enlaces a fuentes** | Cada indicador tiene un enlace a la fuente oficial correspondiente (FUENTES-OFICIALES-JUSTICIA-GLOBAL.md). | MVP |

### No incluido en MVP

- Llamadas a APIs externas (FBI, UNODC, etc.).
- Base de datos persistente (opcional: `localStorage` o archivo JSON propio).
- Gráficos avanzados (D3, Chart.js, etc.) — usar CSS/HTML simple en MVP.

---

## Flujo de usuario

1. Usuario entra en **Comparativas criminalidad** (desde Fuentes oficiales o ATABEY Cumplimiento).
2. Selecciona 2–5 países/regiones (ej. USA, Canadá, México, Colombia, Brasil).
3. Selecciona indicador (ej. Homicidios por 100k).
4. Ingresa los valores manualmente (consultados previamente en FBI CDE, UNODC, etc.).
5. La página muestra tabla comparativa y gráfico de barras.
6. Puede guardar la sesión en `localStorage` para volver después (opcional).

---

## Modelo de datos (local)

```json
{
  "comparisons": [
    {
      "id": "cmp-1",
      "entities": ["USA", "Canadá", "México"],
      "indicator": "homicidios_100k",
      "values": [
        { "entity": "USA", "value": 5.2, "year": "2024", "source": "FBI CDE" },
        { "entity": "Canadá", "value": 2.1, "year": "2024", "source": "Statistics Canada" },
        { "entity": "México", "value": 23.5, "year": "2024", "source": "UNODC" }
      ],
      "createdAt": "2026-02-04T12:00:00Z"
    }
  ]
}
```

---

## Indicadores soportados

| Indicador | Fuente sugerida | Unidad |
|-----------|-----------------|--------|
| Homicidios por 100k habitantes | FBI CDE, UNODC, Statistics Canada | tasa |
| Índice de Severidad del Delito | Statistics Canada | índice |
| Tasa de encarcelamiento | BOP, CSC, World Prison Brief | presos por 100k |
| Robos | FBI CDE, UNODC | tasa por 100k |

---

## Ubicación en la plataforma

- **Página:** `RuddieSolution/platform/comparativas-criminalidad.html`
- **Enlaces desde:**
  - `fuentes-oficiales-justicia-global.html` → "Comparativas tasas criminalidad (MVP)"
  - ATABEY → Cumplimiento → card "Comparativas criminalidad" (opcional)

---

## Referencias

- [FUENTES-OFICIALES-JUSTICIA-GLOBAL.md](FUENTES-OFICIALES-JUSTICIA-GLOBAL.md)
- [registro-personas-interes-publico.html](../RuddieSolution/platform/registro-personas-interes-publico.html)
- [PRINCIPIO-TODO-PROPIO.md](../PRINCIPIO-TODO-PROPIO.md)
