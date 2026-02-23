# SOV-SPAN · Red de Asuntos Públicos Soberana

```
═══════════════════════════════════════════════════════════════════════
    SOVEREIGN GOVERNMENT OF IERAHKWA NE KANIENKE
    Office of the Prime Minister
    "Nuestro C-SPAN propio — Todo propio"
═══════════════════════════════════════════════════════════════════════
```

## Concepto

**SOV-SPAN** (Sovereign Public Affairs Network) es la red de asuntos públicos propia del Gobierno Soberano de Ierahkwa Ne Kanienke. Inspirada en el modelo C-SPAN estadounidense —cobertura sin editar, sin filtros, servicio público— pero **100% soberana**:

- Infraestructura propia (no cable industry, no YouTube TV/Hulu)
- Sin publicidad en canales principales
- Transmisiones gavel-to-gavel de sesiones de gobierno
- Archivo público de video
- Todo self-hosted, crypto nativo, nada de terceros

---

## Canales

| Canal | Nombre | Contenido Principal |
|-------|--------|---------------------|
| **SOV1** | Sovereign Council Live | Grand Council, sesiones plenarias, Prime Minister addresses |
| **SOV2** | Executive & Policy | Consejo Ejecutivo, audiencias, política pública |
| **SOV3** | History & Culture | Historia indígena, Book TV, documentales, entrevistas |

---

## Programación

### SOV1 — Sovereign Council Live
- Sesiones en vivo del Grand Council
- Discurso del Prime Minister
- Votaciones y debates
- Cuando no hay sesión: repeticiones, archivo histórico

### SOV2 — Executive & Policy
- Audiencias de política pública
- Comités y subcomités
- Briefings de ministerios
- Política y economía soberana

### SOV3 — History & Culture
- Historia de los pueblos indígenas de Norteamérica
- Entrevistas a autores (estilo Booknotes)
- Documentales culturales
- Programación histórica (Seven Generations, Haudenosaunee)

---

## Tecnología (TODO PROPIO)

- **Streaming**: HLS/DASH self-hosted o IPTV propio
- **Almacenamiento**: Nodos soberanos, sin S3/Azure/GCP
- **API**: Node.js, Express, JSON local
- **Criptografía**: `crypto` nativo de Node.js
- **Sin**: YouTube, Twitch, Vimeo, AWS, Google, etc.

---

## URLs

- **Plataforma**: `/platform/sovereign-public-affairs.html`
- **API**: `/api/v1/public-affairs/*`
- **Satelital**: `GET /api/v1/public-affairs/satelital` — Info de distribución por canal FOXTROT
- **IPTV**: Canales SOV1, SOV2, SOV3 en `/api/v1/iptv/channels` (categoría Asuntos Públicos)

## Distribución satelital

SOV-SPAN se distribuye por la **red satelital soberana**:

- **Canal**: FOXTROT (Public Broadcast)
- **Satélite**: ISB-SAT-06 (IONKWATAKARI) — GEO, BROADCAST
- **Registro**: Al arrancar el servidor, SOV1, SOV2, SOV3 se registran automáticamente en `telecom.services.broadcast`
- **API satélite**: `/api/v1/telecom/iptv-broadcast` — Lista canales distribuidos por FOXTROT

---

## Integración

- **IGT-STREAM**: Token oficial de streaming Ierahkwa
- **IPTV**: Canales SOV en la red IPTV soberana
- **Video Call**: Integración con videollamadas para sesiones en vivo
- **Americas Communication**: Anuncios y comunicados por región

---

## Principio

> **Todo propio. Nada de 3ra compañía.**  
> C-SPAN fue creado por la industria del cable estadounidense.  
> SOV-SPAN es creado por el Gobierno Soberano de Ierahkwa — para nuestro pueblo, nuestra voz, nuestro archivo.

---

*Referencia: PRINCIPIO-TODO-PROPIO.md*
