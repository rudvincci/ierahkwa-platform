#!/usr/bin/env python3
"""
create-opensource-sovereign.py
Genera 6 plataformas soberanas que reemplazan software open-source.
Sigue Pattern B (mismo formato que vpn-soberana/index.html).
"""
import os

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

PLATFORMS = [
    {
        "slug": "callcenter-soberano",
        "title": "Call Center Soberano",
        "subtitle": "Centro de Contacto Omnicanal para Naciones Soberanas",
        "icon": "📞",
        "accent": "#00bcd4",
        "description": "Centro de contacto omnicanal que reemplaza Asterisk, FreePBX y VICIdial. PBX soberano con VoIP SIP/WebRTC, IVR inteligente con IA en 14 idiomas indígenas, grabación de llamadas cifrada, marcador predictivo, enrutamiento basado en skills, analytics en tiempo real. 100% soberano, cero dependencia de Twilio/Vonage.",
        "metrics": [
            ("10K", "Agentes"),
            ("14", "Idiomas"),
            ("99.99%", "Uptime"),
            ("< 100ms", "Latencia"),
            ("1M+", "Llamadas/Día"),
            ("ACD", "Inteligente"),
        ],
        "cards": [
            ("📞", "PBX Soberano SIP/WebRTC", "Central telefónica IP soberana compatible SIP y WebRTC. Troncales ilimitadas, extensiones dinámicas, transferencias asistidas y ciegas, conferencias ad-hoc y estacionamiento de llamadas."),
            ("🤖", "IVR Inteligente con IA", "Sistema de respuesta interactiva de voz con IA conversacional en 14 idiomas indígenas. Reconocimiento de intenciones, routing contextual y resolución automática sin agente."),
            ("📊", "Marcador Predictivo Soberano", "Algoritmo predictivo que calcula el ratio óptimo de marcación según agentes disponibles, tiempo promedio de llamada y tasa de contacto. Cumple regulaciones de abandono < 3%."),
            ("🔒", "Grabación Cifrada Post-Cuántica", "Todas las llamadas grabadas con cifrado Kyber-768. Almacenamiento soberano con retención configurable, búsqueda full-text de transcripciones y cumplimiento normativo."),
            ("⚡", "Enrutamiento Basado en Skills", "Motor de distribución automática que asigna llamadas según skills del agente: idioma, producto, nivel técnico, prioridad del cliente y carga actual."),
            ("📈", "Analytics Tiempo Real", "Dashboard con métricas en vivo: llamadas en cola, tiempo de espera, nivel de servicio, tasa de abandono, productividad por agente y satisfacción CSAT."),
            ("🎵", "Cola de Espera Musical Soberana", "Música en espera con contenido cultural indígena. Mensajes informativos periódicos, posición en cola, tiempo estimado y callback automático si la espera supera umbral."),
            ("🔗", "Integración CRM Nativo", "Pantalla emergente con datos del cliente al recibir llamada. Historial completo, notas de interacción, tickets asociados y registro automático de actividad en CRM Soberano."),
            ("👂", "Whisper y Barge-In", "Supervisores pueden susurrar al agente sin que el cliente escuche, o intervenir en la llamada directamente. Monitoreo silencioso para entrenamiento y calidad."),
            ("📋", "Reportes de Calidad Automáticos", "Evaluación automática de llamadas con IA: análisis de sentimiento, cumplimiento de script, detección de silencios largos y scoring de calidad sin intervención manual."),
        ],
        "apis": [
            ("POST", "/api/v1/callcenter/calls", "Iniciar o transferir llamada. Params: from, to, queue_id, priority"),
            ("GET",  "/api/v1/callcenter/agents", "Estado de agentes: disponible, en llamada, pausa, offline. Filtros por skill y equipo."),
            ("GET",  "/api/v1/callcenter/queues", "Métricas de colas: llamadas esperando, nivel de servicio, ASA, abandono."),
            ("POST", "/api/v1/callcenter/ivr", "Configurar flujo IVR. Params: menu_tree, language, ai_enabled"),
            ("GET",  "/api/v1/callcenter/recordings", "Buscar grabaciones por fecha, agente, duración, score de calidad."),
            ("GET",  "/api/v1/callcenter/analytics", "Dashboard analytics: SLA, AHT, FCR, CSAT, ocupación por periodo."),
        ],
        "db_stores": ["callcenter-calls", "agent-sessions", "recording-cache"],
        "arch": [
            ("var(--accent)", "CLIENT", "WebRTC/SIP Phone + Softphone PWA", "Llamadas HD · 14 idiomas · WebRTC P2P"),
            ("#ffd600",        "PBX SOBERANO", "Core PBX Engine (Rust)", "SIP Proxy · Registrar · Media Server"),
            ("#7c4dff",        "IVR + ACD", "IVR AI Engine + ACD Router", "NLU · Skills routing · Queue manager"),
            ("var(--accent)", "CRM + ANALYTICS", "CRM Integration + BI Engine", "Screen pop · Recording · Dashboards"),
        ],
    },
    {
        "slug": "voip-soberano",
        "title": "VoIP Soberano",
        "subtitle": "Telefonía IP Soberana para 72 Millones de Personas",
        "icon": "📱",
        "accent": "#00bcd4",
        "description": "Sistema de telefonía IP soberano que reemplaza Twilio, Vonage y RingCentral. Protocolo SIP sobre WireGuard, códec Opus soberano, videollamadas 4K P2P, buzón de voz con transcripción IA, SMS soberano, numeración E.164 propia para 19 naciones. Sin intermediarios Big Tech.",
        "metrics": [
            ("72M", "Usuarios"),
            ("19", "Prefijos"),
            ("4K", "Video"),
            ("< 50ms", "Latencia"),
            ("E2E", "Cifrado"),
            ("SMS", "Soberano"),
        ],
        "cards": [
            ("🔒", "SIP sobre WireGuard Cifrado", "Protocolo SIP encapsulado en túnel WireGuard soberano. Señalización y media cifrados con Kyber-768, imposible interceptar o manipular. NAT traversal automático."),
            ("📹", "Videollamada 4K P2P", "Video peer-to-peer en resolución 4K con códec VP9 soberano. Compartir pantalla, fondos virtuales, reducción de ruido AI y grabación local cifrada."),
            ("📬", "Buzón de Voz con IA", "Buzón de voz con transcripción automática en 14 idiomas indígenas. Resumen por IA, clasificación de urgencia, notificación push y acceso offline."),
            ("💬", "SMS Soberano E2E", "Mensajería SMS con cifrado end-to-end. Entrega garantizada con fallback a mesh network, recibos de lectura, multimedia MMS y programación de envíos."),
            ("🔢", "Numeración E.164 Propia", "Sistema de numeración telefónica soberano bajo estándar E.164. Prefijos únicos para cada una de las 19 naciones. Portabilidad instantánea desde carriers tradicionales."),
            ("👥", "Conferencia Multi-Party", "Salas de conferencia hasta 100 participantes. Audio HD, moderación avanzada, mute selectivo, grabación, transcripción en vivo y dial-in desde PSTN."),
            ("📠", "Fax Digital Soberano", "Envío y recepción de fax digital T.38 sin hardware. Conversión PDF automática, firma digital soberana, confirmación de entrega y archivo cifrado."),
            ("📞", "Integración con Call Center", "Conexión nativa con Call Center Soberano. Transferencias inteligentes, click-to-call desde CRM, presencia unificada y directorio corporativo."),
            ("🌐", "Auto-Attendant Multilingüe", "Recepcionista virtual con menús de voz en 14 idiomas. Horarios de atención, directorio por nombre, routing por departamento y fallback configurables."),
            ("🔄", "Portabilidad Numérica Soberana", "Migración de números telefónicos existentes al ecosistema soberano. Proceso automatizado, sin downtime, verificación criptográfica de propiedad del número."),
        ],
        "apis": [
            ("POST", "/api/v1/voip/call", "Iniciar llamada VoIP. Params: from, to, codec, video_enabled"),
            ("POST", "/api/v1/voip/sms", "Enviar SMS cifrado. Params: from, to, body, priority"),
            ("GET",  "/api/v1/voip/voicemail", "Listar buzón de voz con transcripciones IA. Filtros por fecha y urgencia."),
            ("GET",  "/api/v1/voip/numbers", "Catálogo de numeración E.164 disponible por nación y tipo de número."),
            ("POST", "/api/v1/voip/conference", "Crear sala de conferencia. Params: participants, recording, transcription"),
            ("POST", "/api/v1/voip/fax", "Enviar fax digital. Params: to, document_base64, cover_page"),
        ],
        "db_stores": ["voip-config", "call-history", "voicemail-cache"],
        "arch": [
            ("var(--accent)", "SOFTPHONE", "WebRTC Softphone PWA + SIP", "Llamadas HD · Video 4K · SMS E2E"),
            ("#ffd600",        "SIP PROXY", "SIP Proxy + Registrar (Rust)", "Routing · NAT traversal · WireGuard"),
            ("#7c4dff",        "MEDIA SERVER", "Media Engine (Opus/VP9)", "Transcoding · Recording · Conference"),
            ("var(--accent)", "PSTN GATEWAY", "PSTN Gateway + SMS Gateway", "E.164 · Portability · Interconnect"),
        ],
    },
    {
        "slug": "bigdata-soberano",
        "title": "Big Data Soberano",
        "subtitle": "Procesamiento Masivo de Datos para Soberanía Digital",
        "icon": "📊",
        "accent": "#7c4dff",
        "description": "Framework de procesamiento masivo que reemplaza Apache Hadoop, Spark y Flink. Motor MapReduce soberano en Rust, procesamiento en memoria distribuido, pipeline ETL visual, data lake soberano con cifrado post-cuántico, cluster auto-escalable en 847 nodos, análisis de petabytes sin Cloud Big Tech.",
        "metrics": [
            ("847", "Nodos"),
            ("PB", "Datos"),
            ("< 500ms", "Queries"),
            ("99.99%", "Uptime"),
            ("ETL", "Visual"),
            ("0", "Cloud"),
        ],
        "cards": [
            ("⚙️", "MapReduce Soberano en Rust", "Motor de procesamiento distribuido MapReduce implementado en Rust. 10x más rápido que Hadoop Java, seguridad de memoria garantizada, zero garbage collection pauses."),
            ("🧠", "Procesamiento en Memoria Distribuido", "Engine de cómputo in-memory distribuido entre 847 nodos. Cacheo inteligente de datasets calientes, spill a disco NVMe cuando excede RAM, 100x más rápido que batch."),
            ("🔀", "Pipeline ETL Visual", "Editor visual drag-and-drop para construir pipelines de extracción, transformación y carga. 200+ conectores nativos, scheduling cron, retry automático y lineage tracking."),
            ("🔒", "Data Lake Cifrado Post-Cuántico", "Almacenamiento masivo con cifrado Kyber-768 at-rest y in-transit. Formato columnar soberano, compresión Zstd, particionamiento automático y lifecycle management."),
            ("📈", "Cluster Auto-Escalable", "Infraestructura que escala automáticamente de 1 a 847 nodos según la carga de trabajo. Provisioning en < 30 segundos, auto-healing y rebalanceo transparente."),
            ("🔍", "SQL Federado Multi-Source", "Motor SQL que consulta múltiples fuentes de datos simultáneamente: PostgreSQL, TimescaleDB, data lake, APIs. Join distribuido, push-down de predicados, query optimization."),
            ("🤖", "Machine Learning Distribuido", "Framework ML nativo para entrenar modelos sobre petabytes. Gradient descent distribuido, hyperparameter tuning automático, model registry y serving soberano."),
            ("⚡", "Streaming Analytics Real-Time", "Procesamiento de eventos en tiempo real con ventanas temporales, CEP (Complex Event Processing), alertas y dashboards de streaming con latencia < 100ms."),
            ("📋", "Data Governance Soberano", "Catálogo de datos con metadata automática, clasificación de sensibilidad, masking, auditoría de acceso, compliance GDPR equivalente soberano y data quality scoring."),
            ("💾", "Compresión Columnar Nativa", "Formato de almacenamiento columnar propio con compresión diferencial por tipo de dato. Dictionary encoding, run-length encoding y bit-packing para reducción 10:1."),
        ],
        "apis": [
            ("POST", "/api/v1/bigdata/jobs", "Enviar job MapReduce/SQL. Params: query, resources, priority, timeout"),
            ("GET",  "/api/v1/bigdata/clusters", "Estado del cluster: nodos activos, CPU, memoria, jobs en ejecución."),
            ("GET",  "/api/v1/bigdata/datasets", "Catálogo de datasets con schema, metadata, lineage y estadísticas."),
            ("POST", "/api/v1/bigdata/pipelines", "Crear pipeline ETL. Params: stages, schedule, source, destination"),
            ("POST", "/api/v1/bigdata/queries", "Ejecutar query SQL federado. Params: sql, sources, timeout, limit"),
            ("GET",  "/api/v1/bigdata/ml", "Estado de modelos ML: entrenamiento, métricas, versiones, serving."),
        ],
        "db_stores": ["bigdata-jobs", "pipeline-config", "dataset-metadata"],
        "arch": [
            ("var(--accent)", "DATA SOURCES", "APIs + DBs + Files + Streams", "200+ conectores · Ingestion continua"),
            ("#ffd600",        "ETL PIPELINE", "Visual ETL Engine (Rust)", "Transform · Validate · Enrich · Route"),
            ("#00bcd4",        "DISTRIBUTED ENGINE", "MapReduce + SQL + ML (Rust)", "847 nodos · In-memory · Auto-scale"),
            ("var(--accent)", "DATA LAKE", "Sovereign Data Lake", "Columnar · Cifrado PQ · Petabytes"),
        ],
    },
    {
        "slug": "buscador-soberano",
        "title": "Buscador Soberano",
        "subtitle": "Motor de Búsqueda Full-Text para el Ecosistema Ierahkwa",
        "icon": "🔍",
        "accent": "#7c4dff",
        "description": "Motor de búsqueda full-text que reemplaza Elasticsearch, Algolia y Solr. Indexación de 370+ plataformas soberanas, búsqueda en 14 idiomas indígenas con stemming nativo, autocompletado con IA, búsqueda semántica con embeddings, facets y filtros avanzados, cluster distribuido soberano.",
        "metrics": [
            ("370+", "Índices"),
            ("14", "Idiomas"),
            ("< 10ms", "Queries"),
            ("99.99%", "Uptime"),
            ("ML", "Ranking"),
            ("0", "Tracking"),
        ],
        "cards": [
            ("🔍", "Full-Text Search Multi-Idioma", "Búsqueda de texto completo en 14 idiomas indígenas más español, inglés y portugués. Tokenización lingüística correcta, scoring BM25+ y highlight de resultados."),
            ("🌿", "Stemming para Idiomas Indígenas", "Stemmers desarrollados con lingüistas nativos para Mohawk, Quechua, Nahuatl, Guaraní y 10 idiomas más. Lematización precisa que entiende la morfología aglutinante."),
            ("⚡", "Autocompletado con IA", "Sugerencias en tiempo real mientras el usuario escribe. Modelo de lenguaje entrenado en corpus soberano, corrección ortográfica, sinónimos y expansión de consultas."),
            ("🧠", "Búsqueda Semántica con Embeddings", "Más allá de keywords: comprende la intención de búsqueda usando embeddings vectoriales. Encuentra resultados relevantes incluso con consultas vagas o en diferente idioma."),
            ("📊", "Facets y Filtros Avanzados", "Navegación facetada con conteo dinámico por categoría, fecha, idioma, NEXUS, tipo y más. Filtros combinables, rangos numéricos y geo-spatial."),
            ("🔄", "Indexación en Tiempo Real", "Documentos indexados en < 100ms tras su creación. Webhooks desde todas las plataformas soberanas, re-indexación incremental y consistency eventual garantizada."),
            ("🌐", "Cluster Distribuido Soberano", "Cluster de búsqueda distribuido en las 19 naciones. Sharding automático, replicación para alta disponibilidad y routing inteligente al nodo más cercano."),
            ("🤖", "Relevance Tuning con ML", "Modelo de machine learning que aprende del comportamiento de búsqueda para mejorar relevancia. Click-through feedback, A/B testing de algoritmos y métricas NDCG."),
            ("📝", "Synonyms y Typo Tolerance", "Diccionario de sinónimos soberano por idioma. Tolerancia a errores tipográficos con distancia Levenshtein, corrección fonética y transliteración entre scripts."),
            ("📈", "Analytics de Búsqueda Zero-Track", "Métricas agregadas de búsqueda sin tracking individual. Queries populares, zero-result rate, click-through rate y coverage gaps, todo con privacidad diferencial."),
        ],
        "apis": [
            ("POST", "/api/v1/search/query", "Ejecutar búsqueda. Params: q, lang, filters, facets, page, size"),
            ("POST", "/api/v1/search/index", "Indexar documento. Params: id, type, content, metadata, lang"),
            ("GET",  "/api/v1/search/suggest", "Autocompletado. Params: prefix, lang, limit, context"),
            ("GET",  "/api/v1/search/facets", "Obtener facets disponibles con conteos para un query dado."),
            ("POST", "/api/v1/search/synonyms", "Gestionar sinónimos. Params: lang, synonyms_map, operation"),
            ("GET",  "/api/v1/search/analytics", "Métricas de búsqueda agregadas: top queries, zero-results, CTR."),
        ],
        "db_stores": ["search-indexes", "query-cache", "synonym-config"],
        "arch": [
            ("var(--accent)", "QUERY", "User Query (14 idiomas)", "Autocompletado · Typo correction"),
            ("#ffd600",        "NLP + STEMMING", "NLP Pipeline + Indigenous Stemmers", "Tokenize · Stem · Expand · Embed"),
            ("#00bcd4",        "INVERTED INDEX", "Distributed Index Engine (Rust)", "BM25+ · Sharding · Replication"),
            ("var(--accent)", "RESULT RANKING", "ML Relevance Engine", "Learning-to-rank · A/B test · NDCG"),
        ],
    },
    {
        "slug": "streaming-datos-soberano",
        "title": "Streaming de Datos Soberano",
        "subtitle": "Event Streaming y Message Broker Soberano",
        "icon": "⚡",
        "accent": "#00e676",
        "description": "Plataforma de event streaming que reemplaza Apache Kafka, Storm y RabbitMQ. Message broker distribuido en Rust con throughput de 2M mensajes/segundo, exactly-once delivery, particionamiento inteligente, consumer groups, dead letter queues, replay de eventos. Backbone del ecosistema Ierahkwa.",
        "metrics": [
            ("2M", "Msg/s"),
            ("Exactly", "Once"),
            ("< 1ms", "Latencia"),
            ("847", "Brokers"),
            ("30d", "Retención"),
            ("0", "Pérdida"),
        ],
        "cards": [
            ("🔀", "Message Broker Distribuido Rust", "Broker de mensajes distribuido implementado en Rust puro. Zero-copy networking, io_uring para I/O asíncrono, 2M mensajes/segundo sostenido en hardware commodity."),
            ("✅", "Exactly-Once Delivery", "Semántica exactly-once garantizada end-to-end. Idempotency keys, transacciones distribuidas, acknowledgement protocol con consensus Raft soberano."),
            ("📊", "Particionamiento Inteligente", "Distribución automática de carga entre particiones por key hashing, round-robin o custom. Rebalanceo transparente al agregar/quitar brokers sin pérdida de mensajes."),
            ("👥", "Consumer Groups Soberanos", "Grupos de consumidores con asignación automática de particiones. Offset management, heartbeat monitoring, session timeout y rebalanceo cooperativo."),
            ("💀", "Dead Letter Queue", "Cola de mensajes fallidos con retry automático configurable. Backoff exponencial, máximo de reintentos, alertas y dashboard de mensajes envenenados."),
            ("⏪", "Event Replay Temporal", "Rebobinar y reproducir eventos desde cualquier punto en el tiempo. Retention de 30 días por defecto, compaction para topics de estado, time-travel queries."),
            ("📋", "Schema Registry Nativo", "Registro de schemas Avro, Protobuf y JSON Schema. Evolución compatible, validación automática, versionado semántico y generación de código cliente."),
            ("⚡", "Stream Processing Engine", "Motor de procesamiento de streams integrado. Ventanas temporales, joins de streams, agregaciones, filtros, transformaciones y output a múltiples sinks."),
            ("📦", "Compresión LZ4/Zstd", "Compresión transparente de mensajes con LZ4 (baja latencia) o Zstd (alta compresión). Configuración por topic, batch compression y dictionary training."),
            ("📈", "Monitoring con Grafana Soberano", "Dashboards pre-configurados con métricas JMX: throughput, latencia p99, consumer lag, disk usage, replication factor y health de cada broker."),
        ],
        "apis": [
            ("POST", "/api/v1/streaming/publish", "Publicar evento. Params: topic, key, payload, headers, partition"),
            ("POST", "/api/v1/streaming/subscribe", "Suscribir consumer group. Params: topics, group_id, offset_reset"),
            ("GET",  "/api/v1/streaming/topics", "Listar topics con particiones, replication factor y configuración."),
            ("GET",  "/api/v1/streaming/consumers", "Estado de consumer groups: lag, offsets, assigned partitions."),
            ("POST", "/api/v1/streaming/replay", "Replay de eventos. Params: topic, from_timestamp, to_timestamp"),
            ("GET",  "/api/v1/streaming/health", "Health check de brokers: estado, disk, memoria, connections."),
        ],
        "db_stores": ["streaming-config", "topic-metadata", "consumer-offsets"],
        "arch": [
            ("var(--accent)", "PRODUCERS", "Event Producers (200+ platforms)", "Publish · Batch · Compress · Route"),
            ("#ffd600",        "BROKER CLUSTER", "Distributed Broker (Rust)", "847 brokers · Raft consensus · io_uring"),
            ("#7c4dff",        "PARTITIONS", "Partition + Replication Engine", "Sharding · 3x replica · Compaction"),
            ("var(--accent)", "CONSUMERS", "Consumer Groups + Stream Processing", "Exactly-once · Replay · Analytics"),
        ],
    },
    {
        "slug": "cms-soberano",
        "title": "CMS Soberano",
        "subtitle": "Sistema de Gestión de Contenidos Headless Soberano",
        "icon": "📝",
        "accent": "#e040fb",
        "description": "CMS headless que reemplaza WordPress, Drupal y Strapi. Editor visual WYSIWYG, API-first con GraphQL y REST, contenido multilingüe para 14 idiomas indígenas, media library con CDN soberano, versionado de contenido, workflows de aprobación, SEO automático, SSG y SSR nativos.",
        "metrics": [
            ("14", "Idiomas"),
            ("API", "First"),
            ("99.99%", "Uptime"),
            ("< 50ms", "TTFB"),
            ("SSG+SSR", "Nativo"),
            ("0", "Plugins Vuln"),
        ],
        "cards": [
            ("✏️", "Editor Visual WYSIWYG", "Editor de contenido visual con bloques arrastrables. Rich text, tablas, embeds, código, imágenes inline, markdown shortcuts y colaboración en tiempo real multi-autor."),
            ("🔌", "API GraphQL + REST Nativo", "Doble API automática: GraphQL para queries flexibles y REST para integraciones simples. Schema auto-generado, playground interactivo y SDK para 5 lenguajes."),
            ("🌐", "Contenido Multilingüe 14 Idiomas", "Gestión de contenido en 14 idiomas indígenas. Traducción asistida por IA, fallback chain configurable, URL localizadas y hreflang automático."),
            ("📸", "Media Library con CDN Soberano", "Biblioteca de medios con transformación de imágenes on-the-fly. Resize, crop, formato WebP/AVIF, lazy loading, CDN distribuido en 19 naciones soberanas."),
            ("📚", "Versionado de Contenido", "Historial completo de versiones para cada pieza de contenido. Diff visual, rollback instantáneo, branching de contenido y scheduled publishing."),
            ("✅", "Workflows de Aprobación", "Flujos de trabajo configurables: borrador → revisión → aprobación → publicación. Roles de editor, revisor, aprobador. Notificaciones y deadlines automáticos."),
            ("🔍", "SEO Automático Integrado", "Meta tags, Open Graph, Twitter Cards, JSON-LD, sitemap.xml y robots.txt generados automáticamente. Score SEO en tiempo real, sugerencias y canonical URLs."),
            ("⚡", "SSG y SSR Nativos", "Generación de sitios estáticos (SSG) para máximo rendimiento y server-side rendering (SSR) para contenido dinámico. Incremental Static Regeneration soberano."),
            ("🔐", "Roles y Permisos Granulares", "Sistema de permisos a nivel de campo: quién puede ver, editar o publicar cada tipo de contenido. Integración con IAM Soberano y auditoría de acciones."),
            ("🔗", "Webhooks y Event System", "Notificación automática a sistemas externos cuando el contenido cambia. Webhooks configurables, retry automático, event log y integración con Streaming Soberano."),
        ],
        "apis": [
            ("POST", "/api/v1/cms/content", "Crear/actualizar contenido. Params: type, fields, locale, status"),
            ("POST", "/api/v1/cms/media", "Subir archivo media. Params: file, alt_text, folder, transforms"),
            ("GET",  "/api/v1/cms/types", "Listar content types con schemas, campos y validaciones."),
            ("POST", "/api/v1/cms/workflows", "Gestionar workflow. Params: content_id, action, comment, assignee"),
            ("GET",  "/api/v1/cms/locales", "Idiomas configurados con progreso de traducción por content type."),
            ("POST", "/api/v1/cms/webhooks", "Registrar webhook. Params: url, events, secret, retry_policy"),
        ],
        "db_stores": ["cms-content", "media-cache", "workflow-states"],
        "arch": [
            ("var(--accent)", "EDITOR VISUAL", "WYSIWYG + Block Editor PWA", "Bloques · Colaboración · 14 idiomas"),
            ("#ffd600",        "CONTENT API", "GraphQL + REST Engine (Rust)", "Schema auto · Permissions · Cache"),
            ("#7c4dff",        "STORAGE + CDN", "Media Storage + Sovereign CDN", "Images · Video · Transforms · Edge"),
            ("var(--accent)", "SSG/SSR RENDER", "Static + Server Render Engine", "ISR · Hydration · SEO · Sitemap"),
        ],
    },
]


def generate_html(p):
    slug = p["slug"]
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    desc_short = desc[:160]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    db_stores = p["db_stores"]
    arch = p["arch"]

    # Build metric divs
    metric_html = ""
    for val, lbl in metrics:
        metric_html += f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>\n'

    # Build architecture
    arch_html = ""
    for i, (color, layer_name, layer_tech, layer_desc) in enumerate(arch):
        connector = ""
        if i == 0:
            arch_html += f'<span style="color:{color}">┌─ {layer_name} {"─" * max(1, 43 - len(layer_name))}┐</span>\n'
        else:
            arch_html += f'<span style="color:{color}">┌─ {layer_name} {"─" * max(1, 43 - len(layer_name))}┐</span>\n'
        arch_html += f'│  {layer_tech:<48}│\n'
        arch_html += f'│  {layer_desc:<48}│\n'
        arch_html += f'<span style="color:{color}">└{"─" * 18}┬{"─" * 30}┘</span>\n'
        if i < len(arch) - 1:
            arch_html += f'                   │ <span style="color:#ffd600">▼</span>\n'
    # Remove the last connector line's ┬ with ─
    arch_html = arch_html.rstrip('\n')
    # Fix last closing line to not have ┬
    arch_html = arch_html[:arch_html.rfind('┬')] + '─' + arch_html[arch_html.rfind('┬')+1:]

    # Build cards HTML
    cards_html = ""
    for c_icon, c_title, c_desc in cards:
        cards_html += f'<article class="card"><div class="card-icon" aria-hidden="true">{c_icon}</div><h4>{c_title}</h4><p>{c_desc}</p></article>\n'

    # Build API HTML
    api_html = ""
    for method, endpoint, desc_api in apis:
        color = "#ffd600" if method == "POST" else "#00FF41"
        api_html += f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div><p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{desc_api}</p>\n'

    # Build DB stores JS
    stores_js = str(db_stores).replace("'", '"')

    # Pricing items vary per platform
    plan_items = {
        "guerrero": ["✓ Acceso básico", "✓ 1 proyecto", "✓ API limitada", "✓ Soporte comunidad"],
        "cacique": ["✓ Acceso completo", "✓ 10 proyectos", "✓ API ilimitada", "✓ Soporte prioritario", "✓ Integraciones"],
        "nacion": ["✓ Enterprise", "✓ Proyectos ilimitados", "✓ API dedicada", "✓ SLA 99.99%", "✓ Soporte 24/7"],
    }

    guerrero_items = "\n".join([f'<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">{item}</li>' for item in plan_items["guerrero"]])
    cacique_items = "\n".join([f'<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">{item}</li>' for item in plan_items["cacique"]])
    nacion_items = "\n".join([f'<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">{item}</li>' for item in plan_items["nacion"]])

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{desc_short}">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} — {subtitle}">
<meta property="og:description" content="{desc[:200]}">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} — {subtitle}">
<meta name="twitter:description" content="{desc[:200]}">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} — {subtitle}</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>

<header>
<div class="logo">
<div class="logo-icon" aria-hidden="true">{icon}</div>
<h1>{title}</h1>
</div>
<nav aria-label="Navegacion principal">
<a href="#dashboard" aria-current="page">Dashboard</a>
<a href="#features">Modulos</a>
<a href="#api">API</a>
<a href="#pricing">Precios</a>
</nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">⚛️</span> Quantum-Safe</span>
</header>

<main id="main">

<!-- HERO -->
<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar Módulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<!-- DASHBOARD METRICS -->
<div class="stats" role="list" aria-label="Metricas del sistema">
{metric_html}</div>

<!-- ARCHITECTURE -->
<div class="section" id="architecture">
<h2><span aria-hidden="true">🏗️</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<!-- FEATURE CARDS -->
<div class="section-title" id="features">
<h3>Módulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>
<div class="grid">
{cards_html}</div>

<!-- API ENDPOINTS -->
<div class="section" id="api">
<h2><span aria-hidden="true">🔌</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}</div>
</div>

<!-- PRICING -->
<div class="section-title" id="pricing">
<h3>Planes Soberanos</h3>
<p>Empieza gratis. Escala soberanamente.</p>
</div>
<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Guerrero</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">0 W/mes</div>
<ul style="list-style:none;padding:0">
{guerrero_items}
</ul>
</div><div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
{cacique_items}
</ul>
</div><div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Nación</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
{nacion_items}
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">233+ plataformas soberanas &middot; 17 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem">
<span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">🛡️</span> Seguro</span>
</div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script>
/* Offline Module — {title} v1.0.0 */
(function(){{
  var DB_NAME='ierahkwa-{slug}';var DB_VER=1;
  var STORES={stores_js};
  var db=null;
  function openDB(){{
    return new Promise(function(resolve,reject){{
      var req=indexedDB.open(DB_NAME,DB_VER);
      req.onupgradeneeded=function(){{
        var d=req.result;
        STORES.forEach(function(s){{if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})}});
      }};
      req.onsuccess=function(){{db=req.result;resolve(db)}};
      req.onerror=function(){{reject(req.error)}};
    }});
  }}
  function showOfflineBanner(show){{
    var b=document.getElementById('offline-banner');
    if(!b){{
      b=document.createElement('div');b.id='offline-banner';
      b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';
      b.textContent='Modo Offline — Datos de {title} cacheados localmente para acceso sin conexión.';
      document.body.appendChild(b);
    }}
    b.style.transform=show?'translateY(0)':'translateY(100%)';
  }}
  function init(){{
    openDB().then(function(){{
      window.addEventListener('online',function(){{showOfflineBanner(false)}});
      window.addEventListener('offline',function(){{showOfflineBanner(true)}});
      if(!navigator.onLine)showOfflineBanner(true);
      console.log('[{slug}] Offline module initialized');
    }});
  }}
  init();
}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''
    return html


def main():
    created = []
    for p in PLATFORMS:
        dir_path = os.path.join(BASE, p["slug"])
        os.makedirs(dir_path, exist_ok=True)
        file_path = os.path.join(dir_path, "index.html")
        html = generate_html(p)
        with open(file_path, "w", encoding="utf-8") as f:
            f.write(html)
        line_count = html.count("\n") + 1
        created.append((p["slug"], line_count))
        print(f"✅ {p['slug']}/index.html — {line_count} líneas")

    print(f"\n🎯 {len(created)} plataformas creadas exitosamente.")
    for slug, lines in created:
        status = "OK" if lines >= 180 else "⚠️ BAJO"
        print(f"   {slug}: {lines} líneas [{status}]")


if __name__ == "__main__":
    main()
