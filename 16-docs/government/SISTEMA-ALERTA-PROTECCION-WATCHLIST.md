# Sistema de alerta por protección y watchlist

**Gobierno Soberano Ierahkwa Ne Kanienke**

Sistema para detectar y alertar cuando una persona con **orden de protección**, **antecedentes** o en **watchlist** usa tarjetas, celular, cámaras (reconocimiento facial), IP o cualquier dispositivo/app en un lugar (ej. hotel, comercio).

---

## Escenario (ejemplo: hotel)

- Un hotel tiene cámaras, Wi‑Fi, punto de venta (tarjetas).
- Una persona con **orden de protección** contra ella, o con **antecedentes** relevantes, entra al hotel.
- Esa persona usa:
  - Tarjeta de crédito en el punto de venta
  - Celular (Wi‑Fi, app)
  - Es captada por cámaras (reconocimiento facial)
- El sistema **detecta** que esa persona está en el lugar y **alerta** de inmediato (seguridad, gobierno, persona protegida).

---

## Fuentes de detección

| Fuente | Qué detecta | Requiere |
|--------|-------------|----------|
| **Tarjeta de crédito** | Uso de tarjeta en lugar X | Acuerdo con procesador de pagos o banco propio. Datos de transacciones (últimos 4 dígitos, BIN, ubicación del comercio). |
| **Celular / app** | Dispositivo o sesión en lugar X | App propia que reporte ubicación; o Wi‑Fi/cell que identifique dispositivo. |
| **Cámaras + reconocimiento facial** | Rostro en cámara | Cámaras propias, modelo de reconocimiento propio (todo propio) o integración con sistema de videovigilancia. |
| **IP** | Conexión desde IP en lugar X | Registro de IP en Wi‑Fi o red del establecimiento. |
| **Cualquier dispositivo / app** | Uso de app o dispositivo en lugar X | App o sistema propio que envíe eventos (ubicación, identificador de sesión, etc.). |

---

## Qué es legal y qué requiere acuerdos

- **Orden de protección**: Es función legítima del Estado hacer cumplir órdenes de protección. Usar tecnología para detectar violaciones y alertar es admisible si está previsto en ley y procedimientos.
- **Watchlist / antecedentes**: El uso debe estar cubierto por ley (seguridad, prevención del delito) y procedimientos internos (quién puede registrar, consultar, actuar).
- **Tarjetas**: Ver transacciones en tiempo casi real por ubicación exige acceso a datos de pagos. Normalmente solo lo tienen:
  - Emisores de tarjetas (bancos)
  - Procesadores (Visa, Mastercard, etc.)
  - Comercio con su propio procesador
  - Gobierno con acuerdos específicos con emisores/procesadores
- **Cámaras y reconocimiento facial**: Si son **propias** (infraestructura y software del gobierno o del establecimiento bajo acuerdo), se puede diseñar sin dependencia de terceros.
- **Apps / dispositivos**: Si la app es **propia** y el usuario da consentimiento (ej. app de emergencias, app de ciudadano), es legal registrar ubicación y eventos.

---

## Arquitectura (todo propio donde sea posible)

```
[Watchlist / Órdenes de protección]
         │
         ▼
[API de eventos] ◄── Tarjeta usada en lugar X (si hay datos)
         │           Celular/app en lugar X
         │           Cara detectada en cámara X
         │           IP vista en red X
         │
         ▼
[Comparar con watchlist] ──► Si coincide ──► [Alerta]
                                    │
                                    ▼
                            [Notificar a ministros / seguridad / persona protegida]
```

- **Watchlist**: Lista de personas con orden de protección o en lista de interés, con identificadores (nombre, documento, últimos 4 dígitos de tarjeta si aplica, etc.).
- **Eventos**: Cada fuente (tarjeta, celular, cámara, IP) envía eventos al API con tipo, lugar, identificador.
- **Lógica**: Se compara el evento con la watchlist. Si hay coincidencia razonable, se genera alerta.
- **Notificación**: Igual que en vigilancia/emergencias (panel, API, aviso a ministros/operadores).

---

## Qué ya tienes (implementado)

- **API** `GET /api/v1/watchlist` — Lista personas en watchlist.
- **API** `POST /api/v1/watchlist` — Añade persona a watchlist (nombre, documento, últimos 4 dígitos tarjeta, motivo, orden de protección).
- **API** `POST /api/v1/watchlist/event` — Recibe eventos (tarjeta, cara, dispositivo, IP) y verifica contra la watchlist. Si coincide, genera alerta.
- **API** `GET /api/v1/watchlist/alerts` — Lista alertas recientes.
- **Página** `watchlist-alerta-proteccion.html` — Añadir personas, ver watchlist, ver alertas y documentación de la API.

La integración real con:
- Redes de tarjetas (Visa, Mastercard, bancos)
- Cámaras y reconocimiento facial
- Redes Wi‑Fi / operadores

se haría cuando existan **acuerdos** o **infraestructura propia** (ej. punto de venta propio, cámaras propias, app propia).

---

## Resumen

- **Escenario**: Hotel (o cualquier lugar) con tarjetas, celular, cámaras, Wi‑Fi.
- **Objetivo**: Saber si una persona con orden de protección o en watchlist está en ese lugar.
- **Fuentes**: Tarjeta, celular, app, cámaras (face), IP.
- **Acción**: Alerta inmediata cuando se detecte.
- **Base**: Watchlist + API de eventos + lógica de coincidencia + notificación.
- **Legal**: Órdenes de protección y watchlist con base legal; fuentes de datos con acuerdos o sistemas propios.
