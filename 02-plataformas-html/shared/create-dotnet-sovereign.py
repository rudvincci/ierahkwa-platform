#!/usr/bin/env python3
"""
create-dotnet-sovereign.py
Creates 8 sovereign platform directories that replace Microsoft .NET components.
Each generates an index.html following Pattern B (VPN Soberana template).
"""

import os

BASE_DIR = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"

platforms = [
    {
        "dir": "runtime-soberano",
        "title": "Runtime Soberano",
        "subtitle": "Motor de Ejecución Nativo Post-Cuántico",
        "icon": "⚙️",
        "accent": "#00e676",
        "description": "Runtime nativo basado en Rust y WebAssembly que reemplaza Microsoft .NET CLR. Compilación AOT, JIT soberano con optimización para 19 naciones, garbage collector de baja latencia (<1ms), interop nativo con C/Rust/Go sin FFI overhead. Zero dependencias Microsoft.",
        "metrics": [
            ("<1ms", "GC Latencia"),
            ("19", "Targets"),
            ("99.99%", "Uptime"),
            ("0.3MB", "Base Size"),
            ("847", "WASM Modules"),
            ("12ns", "Startup"),
        ],
        "cards": [
            ("⚙️", "JIT Soberano Multi-Target", "Compilador Just-In-Time optimizado para cada una de las 19 arquitecturas soberanas. Genera código nativo con SIMD auto-vectorización y branch prediction hints específicos por CPU."),
            ("🔧", "AOT Compilation Nativa", "Compilación Ahead-Of-Time que genera binarios nativos sin dependencias. Optimización de link-time, dead code elimination y tree shaking automático para binarios mínimos."),
            ("♻️", "Garbage Collector Sub-Milisegundo", "GC generacional con pausas menores a 1ms. Concurrent marking, incremental sweeping y compactación en background. Zero-pause mode para aplicaciones de tiempo real."),
            ("🌐", "WASM Runtime Universal", "Ejecución WebAssembly con WASI preview 2. Sandboxing de memoria, capability-based security y hot-loading de módulos. Compatible con 847 módulos del ecosistema."),
            ("🔗", "Interop Rust/Go/C Zero-Cost", "FFI sin overhead entre Rust, Go y C. Marshalling automático de tipos, shared memory sin copia y calling conventions nativos. Performance indistinguible de código nativo."),
            ("🔄", "Hot Reload en Producción", "Recarga de código en caliente sin reiniciar servicios. State preservation, connection draining y rollback automático si el nuevo código falla health checks."),
            ("🛡️", "Memory Safety sin Borrow Checker", "Seguridad de memoria garantizada por el runtime sin las restricciones de Rust borrow checker. Region-based memory management con linear types opcionales."),
            ("🧵", "Thread Pool Adaptativo", "Pool de threads que se adapta automáticamente a la carga. Work-stealing scheduler, async/await nativo y green threads para millones de tareas concurrentes."),
            ("🔍", "Debug Remoto Post-Cuántico", "Depuración remota cifrada con Kyber-768. Breakpoints condicionales, time-travel debugging y snapshot de estado completo en producción sin impacto de rendimiento."),
            ("📊", "Profiler de Rendimiento Nativo", "Profiling de CPU, memoria, I/O y red integrado. Flame graphs en tiempo real, allocation tracking y sugerencias de optimización con IA."),
        ],
        "apis": [
            ("POST", "/api/v1/runtime/compile", "Compilar módulo. Params: source, target, optimization_level"),
            ("POST", "/api/v1/runtime/deploy", "Desplegar módulo compilado al cluster soberano."),
            ("GET", "/api/v1/runtime/status", "Estado del runtime: módulos activos, memoria, threads, GC stats."),
            ("GET", "/api/v1/runtime/modules", "Lista de módulos WASM cargados con métricas de rendimiento."),
            ("POST", "/api/v1/runtime/debug", "Iniciar sesión de debug remoto. Params: module_id, breakpoints"),
            ("GET", "/api/v1/runtime/metrics", "Métricas de rendimiento: CPU, GC, allocations, throughput."),
        ],
        "db_stores": ["runtime-config", "compiled-modules", "debug-sessions"],
        "arch_layers": [
            ("#00e676", "COMPILER", "Rust Compiler · LLVM Backend · Multi-Target", "AOT + JIT · Tree Shaking · SIMD Auto-Vec"),
            ("#ffd600", "JIT/AOT", "Just-In-Time · Ahead-Of-Time · Tiered", "Profile-Guided Opt · Deoptimization · OSR"),
            ("#7c4dff", "RUNTIME", "GC Sub-ms · Thread Pool · Memory Manager", "Green Threads · Work Stealing · Regions"),
            ("#00e676", "WASM", "WebAssembly · WASI · Sandboxed Execution", "847 Modules · Hot Reload · Capability-Based"),
        ],
        "arch_labels": ["COMPILER", "JIT/AOT", "RUNTIME", "WASM"],
    },
    {
        "dir": "sdk-soberano",
        "title": "SDK Soberano",
        "subtitle": "Kit de Desarrollo Completo para Naciones Digitales",
        "icon": "🧰",
        "accent": "#00e676",
        "description": "SDK completo que reemplaza Microsoft .NET SDK y Visual Studio. CLI soberano, templates para 19 tipos de proyecto, package manager WAMPUM, linter y formatter integrados, generador de código con IA, testing framework nativo. Soporte para Rust, Go, TypeScript y Python.",
        "metrics": [
            ("19", "Templates"),
            ("4", "Lenguajes"),
            ("2,847", "Paquetes"),
            ("<3s", "Build Time"),
            ("100%", "Offline"),
            ("0", "Telemetría"),
        ],
        "cards": [
            ("🧰", "CLI Soberano Ierahkwa", "Interfaz de línea de comandos unificada para todo el ecosistema. Autocompletado inteligente, scripting con YAML/TOML, plugins extensibles y output JSON para automatización."),
            ("📦", "Package Manager WAMPUM", "Gestor de paquetes soberano con resolución de dependencias SAT solver. Registry distribuido, verificación criptográfica de integridad y cache local persistente."),
            ("📋", "Template Engine Multi-Proyecto", "19 templates oficiales: microservicio, API, PWA, CLI, library, blockchain node, AI model, IoT agent y más. Personalización con variables y hooks post-generación."),
            ("✨", "Linter + Formatter Integrado", "Análisis estático y formateo automático para Rust, Go, TypeScript y Python. Reglas configurables, auto-fix seguro y pre-commit hooks integrados."),
            ("🧪", "Testing Framework Nativo", "Framework de testing con unit, integration, e2e y property-based testing. Parallel execution, snapshot testing, coverage report y mutation testing."),
            ("🤖", "Code Generator con IA", "Generación de código con IA local (sin cloud). Scaffold de CRUD, API endpoints, migrations, tests y documentación a partir de specs en lenguaje natural."),
            ("🏗️", "Scaffold de Microservicios", "Generación completa de microservicios: API, DB, events, health checks, Docker, CI/CD, monitoring y documentación. Un comando, servicio productivo."),
            ("🔄", "Hot Module Replacement", "Recarga de módulos en desarrollo sin perder estado. Preserva conexiones WebSocket, state de componentes UI y sesiones de debug activas."),
            ("📖", "Documentación Auto-Generada", "Genera documentación API (OpenAPI 3.1), guías de uso y diagramas de arquitectura automáticamente desde el código fuente y comentarios."),
            ("🚀", "CI/CD Pipeline Builder", "Constructor visual y YAML de pipelines CI/CD. Templates para GitHub Actions, GitLab CI y runners soberanos. Testing, build, deploy en un archivo."),
        ],
        "apis": [
            ("POST", "/api/v1/sdk/create", "Crear nuevo proyecto. Params: template, name, language, features"),
            ("POST", "/api/v1/sdk/build", "Compilar proyecto. Params: target, optimization, parallel"),
            ("POST", "/api/v1/sdk/test", "Ejecutar tests. Params: filter, parallel, coverage"),
            ("POST", "/api/v1/sdk/publish", "Publicar paquete al registry WAMPUM soberano."),
            ("GET", "/api/v1/sdk/packages", "Buscar paquetes en el registry. Params: query, category, sort"),
            ("POST", "/api/v1/sdk/lint", "Analizar código. Params: rules, auto_fix, format"),
        ],
        "db_stores": ["sdk-config", "project-templates", "package-cache"],
        "arch_layers": [
            ("#00e676", "CLI", "Ierahkwa CLI · Autocompletado · Plugins", "YAML/TOML Config · JSON Output · Scripting"),
            ("#ffd600", "BUILD SYSTEM", "Incremental Build · Parallel · Cache", "Multi-Target · Hot Reload · Watch Mode"),
            ("#7c4dff", "PACKAGE MANAGER", "WAMPUM Registry · SAT Solver · Verify", "2,847 Packages · Local Cache · Audit"),
            ("#00e676", "DEPLOY", "CI/CD Pipeline · Docker · K8s · Sovereign", "Blue-Green · Canary · Rollback · Monitor"),
        ],
        "arch_labels": ["CLI", "BUILD SYSTEM", "PACKAGE MANAGER", "DEPLOY"],
    },
    {
        "dir": "gateway-soberano",
        "title": "Gateway Soberano",
        "subtitle": "API Gateway de Alto Rendimiento Post-Cuántico",
        "icon": "🚪",
        "accent": "#00bcd4",
        "description": "API Gateway soberano que reemplaza Microsoft Azure API Management y Ocelot .NET. Escrito en Rust con tokio async runtime, soporta 1M+ requests/segundo, rate limiting distribuido, circuit breaker, mTLS con certificados post-cuánticos Kyber-768, load balancing inteligente con IA.",
        "metrics": [
            ("1M+", "Req/s"),
            ("<0.5ms", "Latencia"),
            ("99.999%", "Uptime"),
            ("847", "Routes"),
            ("mTLS", "Cifrado"),
            ("0", "Downtime"),
        ],
        "cards": [
            ("🚦", "Rate Limiting Distribuido", "Control de tráfico distribuido con token bucket y sliding window. Configuración por ruta, por usuario, por API key. Sincronización entre nodos con CRDT para zero-conflict."),
            ("🔌", "Circuit Breaker Inteligente", "Protección automática contra cascading failures. Half-open probing, exponential backoff y fallback responses configurables. Métricas de salud en tiempo real por servicio."),
            ("🔐", "mTLS Post-Cuántico", "Mutual TLS con certificados Kyber-768 post-cuánticos. Rotación automática de certificados, OCSP stapling soberano y certificate transparency log propio."),
            ("🤖", "Load Balancer con IA", "Distribución de carga inteligente que aprende patrones de tráfico. Weighted round-robin adaptativo, least connections con health awareness y geo-routing por nación."),
            ("🔄", "Request/Response Transform", "Transformación de requests y responses en pipeline. Header injection, body mapping, protocol translation (REST↔gRPC↔GraphQL) y content negotiation automático."),
            ("🌐", "WebSocket Proxy Nativo", "Proxy WebSocket con connection multiplexing, heartbeat management, reconnection automática y message buffering durante desconexiones temporales."),
            ("📡", "gRPC Gateway Bidireccional", "Gateway gRPC con transcoding REST automático. Streaming bidireccional, load balancing por stream y deadline propagation nativa."),
            ("💾", "Cache Layer Multi-Nivel", "Cache en L1 (in-memory), L2 (Redis distribuido) y L3 (CDN soberano). Cache invalidation por tags, stale-while-revalidate y cache warming predictivo con IA."),
            ("❤️", "Health Check Automatizado", "Health checks activos y pasivos para cada upstream. TCP, HTTP, gRPC checks con intervalos configurables. Automatic removal y re-addition de backends."),
            ("🚀", "Blue-Green Deploy Zero-Downtime", "Despliegues sin interrupción con traffic splitting gradual. Canary analysis automático, rollback instantáneo y connection draining graceful."),
        ],
        "apis": [
            ("GET", "/api/v1/gateway/routes", "Lista de rutas configuradas con upstream, middleware y métricas."),
            ("GET", "/api/v1/gateway/health", "Estado de salud de todos los backends y servicios upstream."),
            ("GET", "/api/v1/gateway/metrics", "Métricas de tráfico: req/s, latencia p50/p95/p99, error rate."),
            ("POST", "/api/v1/gateway/rate-limits", "Configurar rate limits. Params: route, limit, window, action"),
            ("POST", "/api/v1/gateway/certificates", "Gestionar certificados mTLS. Params: domain, key_type, rotation"),
            ("POST", "/api/v1/gateway/cache", "Gestionar cache. Params: route, ttl, invalidation_tags"),
        ],
        "db_stores": ["gateway-config", "route-cache", "rate-limit-counters"],
        "arch_layers": [
            ("#00bcd4", "INGRESS", "TLS Termination · DDoS Shield · WAF", "HTTP/2 · HTTP/3 QUIC · WebSocket · gRPC"),
            ("#ffd600", "RATE LIMITER", "Token Bucket · Sliding Window · CRDT", "Per-Route · Per-User · Per-API Key · Global"),
            ("#7c4dff", "ROUTER", "Pattern Matching · Middleware Pipeline", "Transform · Auth · Cache · Circuit Breaker"),
            ("#00bcd4", "BACKEND", "Load Balancer · Health Check · Failover", "847 Routes · Multi-Protocol · IA Routing"),
        ],
        "arch_labels": ["INGRESS", "RATE LIMITER", "ROUTER", "BACKEND"],
    },
    {
        "dir": "fintech-soberano",
        "title": "FinTech Soberano",
        "subtitle": "Infraestructura Financiera Digital para Naciones Soberanas",
        "icon": "💰",
        "accent": "#ffd600",
        "description": "Stack financiero completo que reemplaza la infraestructura bancaria .NET. Motor de transacciones ACID con 50,000 TPS, sistema de pagos WAMPUM, procesador de tarjetas soberano, motor de préstamos con IA, compliance AML/KYC integrado, ledger distribuido con blockchain MameyNode.",
        "metrics": [
            ("50K", "TPS"),
            ("$2.4B", "Procesados"),
            ("99.999%", "Uptime"),
            ("<15ms", "Latencia"),
            ("19", "Monedas"),
            ("AML", "Compliant"),
        ],
        "cards": [
            ("💳", "Motor de Transacciones ACID", "Procesamiento transaccional con garantías ACID completas. Serializable isolation, distributed transactions con 2PC optimizado y conflict resolution automático. 50,000 TPS sostenidos."),
            ("💸", "Sistema de Pagos WAMPUM", "Red de pagos soberana compatible con SWIFT, SEPA y ACH. Liquidación instantánea entre 19 naciones, multi-moneda con conversión FX en tiempo real y fees transparentes."),
            ("💳", "Procesador de Tarjetas Soberano", "Emisión y procesamiento de tarjetas de débito/crédito soberanas. BIN propio, tokenización PCI-DSS, 3D Secure 2.0, contactless NFC y virtual cards instantáneas."),
            ("🤖", "Motor de Préstamos con IA", "Evaluación crediticia con machine learning. Scoring alternativo para poblaciones sin historial bancario, aprobación en < 60 segundos y tasas personalizadas por perfil de riesgo."),
            ("🔍", "Compliance AML/KYC Automatizado", "Verificación de identidad con biometría, PEP screening, sanctions list checking y transaction monitoring en tiempo real. Reportes SAR automáticos a reguladores."),
            ("⛓️", "Ledger Distribuido MameyNode", "Blockchain soberana para registro inmutable de transacciones. Consenso BFT con finalidad en 2 segundos, smart contracts para escrow y atomic swaps entre monedas soberanas."),
            ("📊", "Reconciliación en Tiempo Real", "Motor de reconciliación que compara transacciones entre sistemas en tiempo real. Detección de discrepancias automática, resolución con reglas configurables y audit trail completo."),
            ("🏦", "API Open Banking PSD2", "APIs compatibles con PSD2/Open Banking. Account information, payment initiation, confirmation of funds y consent management. Sandbox para desarrolladores."),
            ("📈", "Motor de Riesgo Crediticio", "Análisis de riesgo multi-dimensional: credit risk, market risk, operational risk y liquidity risk. Stress testing automatizado y capital adequacy reporting."),
            ("📋", "Reportes Regulatorios Automáticos", "Generación automática de reportes para reguladores: IFRS 9, Basel III, MiFID II, FATCA y CRS. Templates por jurisdicción, validación pre-envío y delivery tracking."),
        ],
        "apis": [
            ("POST", "/api/v1/fintech/transfer", "Transferir fondos. Params: from, to, amount, currency, reference"),
            ("GET", "/api/v1/fintech/balance", "Consultar saldo y movimientos. Params: account_id, period"),
            ("POST", "/api/v1/fintech/loans", "Solicitar préstamo. Params: amount, term, purpose, collateral"),
            ("GET", "/api/v1/fintech/compliance", "Estado de compliance AML/KYC del usuario."),
            ("POST", "/api/v1/fintech/cards", "Emitir tarjeta soberana. Params: type, limits, features"),
            ("GET", "/api/v1/fintech/reports", "Reportes financieros y regulatorios. Params: type, period, format"),
        ],
        "db_stores": ["fintech-transactions", "loan-applications", "compliance-records"],
        "arch_layers": [
            ("#ffd600", "CLIENT", "Mobile App · Web Portal · POS Terminal", "Open Banking API · PSD2 · Sandbox Dev"),
            ("#00bcd4", "PAYMENT ENGINE", "WAMPUM Network · SWIFT · SEPA · ACH", "50K TPS · Multi-Currency · FX Real-Time"),
            ("#7c4dff", "LEDGER", "Double-Entry · ACID · Reconciliation", "Immutable Audit Trail · Real-Time Balance"),
            ("#ffd600", "BLOCKCHAIN", "MameyNode · BFT Consensus · Smart Contracts", "Atomic Swaps · Escrow · 2s Finality"),
        ],
        "arch_labels": ["CLIENT", "PAYMENT ENGINE", "LEDGER", "BLOCKCHAIN"],
    },
    {
        "dir": "gobierno-digital-soberano",
        "title": "Gobierno Digital Soberano",
        "subtitle": "Plataforma de Gobierno Electrónico para 19 Naciones",
        "icon": "🏛️",
        "accent": "#1565c0",
        "description": "Plataforma de e-government que reemplaza el módulo gubernamental .NET. Portal ciudadano unificado, trámites 100% digitales, firma electrónica soberana, votación digital con blockchain, registro civil descentralizado, transparencia fiscal en tiempo real. Cumple estándares OGP.",
        "metrics": [
            ("72M", "Ciudadanos"),
            ("847", "Trámites"),
            ("<2min", "Promedio"),
            ("100%", "Digital"),
            ("19", "Naciones"),
            ("14", "Idiomas"),
        ],
        "cards": [
            ("🏛️", "Portal Ciudadano Unificado", "Punto único de acceso a todos los servicios gubernamentales. SSO con identidad soberana, dashboard personalizado por ciudadano, historial de trámites y notificaciones proactivas."),
            ("📄", "Trámites 100% Digitales", "847 trámites gubernamentales completamente digitalizados. Formularios inteligentes con auto-completado, validación en tiempo real y firma electrónica integrada. Cero papel."),
            ("✍️", "Firma Electrónica Soberana", "Firma digital con validez legal en 19 naciones. Certificados X.509 con criptografía post-cuántica, timestamping soberano y verificación instantánea sin terceros de confianza."),
            ("🗳️", "Votación Digital Blockchain", "Sistema de votación electrónica con blockchain MameyNode. Voto secreto verificable, auditoría end-to-end, resultados en tiempo real y resistencia a manipulación cuántica."),
            ("📋", "Registro Civil Descentralizado", "Nacimientos, matrimonios, defunciones y cambios de estado civil en blockchain. Certificados digitales verificables, interoperabilidad entre 19 naciones y privacidad zero-knowledge."),
            ("💰", "Transparencia Fiscal Real-Time", "Dashboard público de gastos gubernamentales en tiempo real. Cada peso rastreable desde presupuesto hasta ejecución, contratos abiertos y rendición de cuentas automática."),
            ("📜", "Licencias y Permisos Online", "Solicitud, pago y emisión de licencias y permisos 100% online. QR verificable, renovación automática, notificaciones de vencimiento y validación inter-jurisdiccional."),
            ("🧾", "Impuestos Digitales Automáticos", "Declaración y pago de impuestos simplificado. Pre-cálculo automático, deducción inteligente, pago fraccionado y devolución express en < 48 horas."),
            ("📊", "Open Data Portal", "Portal de datos abiertos con 10,000+ datasets. API REST, formatos CSV/JSON/Parquet, visualizaciones interactivas y actualizaciones automáticas desde sistemas fuente."),
            ("🤖", "Atención Ciudadana con IA", "Chatbot multilingüe (14 idiomas) para consultas gubernamentales. Resolución automática del 85% de consultas, escalamiento a humano y seguimiento de casos."),
        ],
        "apis": [
            ("GET", "/api/v1/gobierno/tramites", "Lista de trámites disponibles por categoría y jurisdicción."),
            ("GET", "/api/v1/gobierno/ciudadano", "Perfil del ciudadano, trámites activos e historial."),
            ("POST", "/api/v1/gobierno/firma", "Firmar documento digitalmente. Params: doc_id, certificate, timestamp"),
            ("POST", "/api/v1/gobierno/votacion", "Emitir voto cifrado. Params: election_id, ballot, proof"),
            ("GET", "/api/v1/gobierno/transparencia", "Datos de transparencia fiscal: presupuesto, ejecución, contratos."),
            ("POST", "/api/v1/gobierno/registro", "Registrar acto civil. Params: type, data, witnesses, jurisdiction"),
        ],
        "db_stores": ["gobierno-tramites", "ciudadano-perfil", "votacion-registros"],
        "arch_layers": [
            ("#1565c0", "CITIZEN", "Portal Web · App Móvil · Kiosco · Chatbot", "SSO Soberano · 14 Idiomas · WCAG 2.1 AA"),
            ("#ffd600", "PORTAL", "Trámites · Firma · Votación · Registro", "847 Servicios · <2min Promedio · 100% Digital"),
            ("#7c4dff", "SERVICES", "Identity · Payments · Notifications · AI", "Microservicios · Event-Driven · CQRS"),
            ("#1565c0", "BLOCKCHAIN", "MameyNode · Registro Civil · Votación", "Zero-Knowledge · Immutable · 19 Naciones"),
        ],
        "arch_labels": ["CITIZEN", "PORTAL", "SERVICES", "BLOCKCHAIN"],
    },
    {
        "dir": "microservicios-soberano",
        "title": "Microservicios Soberano",
        "subtitle": "Framework de Microservicios Event-Driven Soberano",
        "icon": "🔗",
        "accent": "#00e676",
        "description": "Framework de microservicios que reemplaza .NET Aspire y Dapr. Orquestación con contenedores soberanos (no Docker/Kubernetes), service mesh nativo, event sourcing con CQRS, saga pattern distribuido, observabilidad completa, auto-scaling basado en IA. Escrito en Rust + Go.",
        "metrics": [
            ("10K", "Servicios"),
            ("<5ms", "Inter-Service"),
            ("99.999%", "Uptime"),
            ("0", "Vendor Lock"),
            ("Auto", "Scale"),
            ("847", "Nodos"),
        ],
        "cards": [
            ("🕸️", "Service Mesh Soberano Nativo", "Malla de servicios sin sidecar proxy. Kernel-level networking con eBPF, mTLS automático entre servicios, traffic management y observabilidad integrada. Zero overhead."),
            ("📨", "Event Sourcing + CQRS", "Patrón de event sourcing con command/query separation. Event store inmutable, proyecciones materializadas, replay de eventos y temporal queries para debugging."),
            ("🔄", "Saga Pattern Distribuido", "Orquestación de transacciones distribuidas con sagas. Compensación automática, timeout handling, dead letter processing y visualización de estado de saga."),
            ("📦", "Container Runtime Propio", "Runtime de contenedores soberano sin Docker ni Kubernetes. OCI-compatible, rootless por defecto, resource isolation con cgroups v2 y image registry soberano."),
            ("🔍", "Service Discovery Automático", "Descubrimiento de servicios con DNS-SD y gossip protocol. Health-aware routing, multi-datacenter support y failover automático entre regiones soberanas."),
            ("📊", "Observabilidad OpenTelemetry", "Traces, metrics y logs unificados con OpenTelemetry nativo. Distributed tracing end-to-end, custom dashboards y alerting inteligente con anomaly detection."),
            ("🤖", "Auto-Scaling con IA", "Escalamiento automático basado en machine learning. Predicción de carga, pre-scaling proactivo, scale-to-zero para servicios inactivos y cost optimization automático."),
            ("📡", "gRPC Inter-Service Nativo", "Comunicación inter-servicio con gRPC nativo. Protocol buffers v3, bidirectional streaming, deadline propagation y automatic retry con circuit breaker."),
            ("📬", "Dead Letter Queue Resiliente", "Cola de mensajes fallidos con retry policies configurables. Exponential backoff, poison message detection, manual replay UI y alerting por umbral."),
            ("🚀", "Canary Deployment Automático", "Despliegues canary con análisis automático de métricas. Traffic splitting gradual, rollback automático si error rate > umbral y A/B testing integrado."),
        ],
        "apis": [
            ("GET", "/api/v1/micro/services", "Lista de microservicios registrados con estado y métricas."),
            ("POST", "/api/v1/micro/events", "Publicar evento al bus. Params: topic, payload, correlation_id"),
            ("GET", "/api/v1/micro/health", "Health check agregado de todos los servicios del cluster."),
            ("POST", "/api/v1/micro/deploy", "Desplegar servicio. Params: image, replicas, strategy, config"),
            ("POST", "/api/v1/micro/scale", "Escalar servicio. Params: service_id, replicas, auto_scale"),
            ("GET", "/api/v1/micro/logs", "Logs agregados con filtros. Params: service, level, timerange"),
        ],
        "db_stores": ["micro-services", "event-store", "deployment-history"],
        "arch_layers": [
            ("#00e676", "CLIENT", "API Gateway · Load Balancer · CDN", "HTTP/2 · gRPC · WebSocket · GraphQL"),
            ("#ffd600", "MESH", "Service Mesh · mTLS · eBPF Networking", "Discovery · Routing · Circuit Breaker"),
            ("#7c4dff", "SERVICES", "Rust + Go · Event-Driven · CQRS · Saga", "10K Services · Container Runtime · Auto-Scale"),
            ("#00e676", "EVENT BUS", "Event Store · Message Queue · DLQ", "Event Sourcing · Pub/Sub · Replay · CDC"),
        ],
        "arch_labels": ["CLIENT", "MESH", "SERVICES", "EVENT BUS"],
    },
    {
        "dir": "orm-soberano",
        "title": "ORM Soberano",
        "subtitle": "Object-Relational Mapper y Migraciones Soberanas",
        "icon": "🗄️",
        "accent": "#00e676",
        "description": "ORM y sistema de migraciones que reemplaza Entity Framework y EF Migrations de .NET. Query builder type-safe, migraciones versionadas con rollback automático, soporte multi-database (PostgreSQL, TimescaleDB, CockroachDB), cache inteligente de queries, connection pooling optimizado.",
        "metrics": [
            ("50K", "Queries/s"),
            ("<2ms", "Avg Latencia"),
            ("0", "N+1 Queries"),
            ("Auto", "Migrate"),
            ("5", "Databases"),
            ("100%", "Type-Safe"),
        ],
        "cards": [
            ("🔍", "Query Builder Type-Safe", "Constructor de queries con tipado completo en tiempo de compilación. Prevención de SQL injection por diseño, composición de queries y subqueries con API fluida."),
            ("📋", "Migraciones Versionadas con Rollback", "Sistema de migraciones con versionado semántico. Diff automático de schemas, rollback seguro, migraciones idempotentes y preview de cambios antes de aplicar."),
            ("🗃️", "Multi-Database Nativo", "Soporte nativo para PostgreSQL, TimescaleDB, CockroachDB, SQLite y MameyDB. Abstracción uniforme, dialect-specific optimizations y cross-database queries."),
            ("💾", "Cache Inteligente de Queries", "Cache automático de queries con invalidación basada en tablas modificadas. Cache warming predictivo, hit rate monitoring y eviction policies configurables."),
            ("🔗", "Connection Pooling Optimizado", "Pool de conexiones con sizing automático basado en carga. Health checking, connection recycling, statement caching y prepared statement pooling."),
            ("📥", "Lazy/Eager Loading Automático", "Detección automática del patrón de loading óptimo. Prevención de N+1 con batch loading, explicit loading API y data loader pattern integrado."),
            ("🔄", "Transacciones Distribuidas", "Soporte para transacciones que abarcan múltiples databases. Two-phase commit, saga pattern para eventual consistency y compensating transactions."),
            ("📊", "Schema Diffing Visual", "Comparación visual de schemas entre ambientes. Generación automática de migration scripts, conflict detection y merge de cambios concurrentes."),
            ("🌱", "Seed Data Management", "Gestión de datos iniciales y de prueba. Factories, fixtures, conditional seeding por ambiente y data anonymization para ambientes de desarrollo."),
            ("📈", "Performance Profiler de Queries", "Profiling de queries con explain plans visuales. Detección de slow queries, index suggestions, query optimization hints y N+1 detection en desarrollo."),
        ],
        "apis": [
            ("POST", "/api/v1/orm/query", "Ejecutar query. Params: model, filters, includes, order, limit"),
            ("POST", "/api/v1/orm/migrate", "Ejecutar migraciones. Params: direction, target_version, dry_run"),
            ("GET", "/api/v1/orm/schema", "Schema actual de todas las tablas con relaciones y tipos."),
            ("GET", "/api/v1/orm/connections", "Estado del connection pool: activas, idle, waiting, stats."),
            ("POST", "/api/v1/orm/cache", "Gestionar cache de queries. Params: action, table, ttl"),
            ("POST", "/api/v1/orm/seed", "Ejecutar seed data. Params: environment, tables, truncate"),
        ],
        "db_stores": ["orm-schemas", "migration-history", "query-cache"],
        "arch_layers": [
            ("#00e676", "APP", "Models · Repositories · Services", "Type-Safe Queries · Validation · Relations"),
            ("#ffd600", "QUERY BUILDER", "Fluent API · Composable · SQL Prevention", "Joins · Subqueries · Aggregations · CTE"),
            ("#7c4dff", "CONNECTION POOL", "Auto-Sizing · Health Check · Recycling", "Prepared Statements · Transaction Mgmt"),
            ("#00e676", "DATABASE", "PostgreSQL · TimescaleDB · CockroachDB", "Migrations · Seeds · Schema Diff · Backup"),
        ],
        "arch_labels": ["APP", "QUERY BUILDER", "CONNECTION POOL", "DATABASE"],
    },
    {
        "dir": "ui-framework-soberano",
        "title": "UI Framework Soberano",
        "subtitle": "Framework de Interfaces de Usuario Multi-Plataforma",
        "icon": "🎨",
        "accent": "#e040fb",
        "description": "Framework UI que reemplaza Blazor, MAUI y WPF de .NET. Componentes reactivos compilados a WebAssembly, rendering nativo en iOS/Android/Desktop sin webview, design system Ierahkwa integrado, 200+ componentes accesibles WCAG 2.1 AA, tema oscuro soberano, animaciones GPU-accelerated.",
        "metrics": [
            ("200+", "Componentes"),
            ("5", "Plataformas"),
            ("<16ms", "Render"),
            ("60fps", "Animación"),
            ("WCAG", "AA"),
            ("0", "Webview"),
        ],
        "cards": [
            ("⚡", "Componentes Reactivos WASM", "Componentes compilados a WebAssembly con reactividad fine-grained. Virtual DOM diferencial, actualizaciones granulares sin re-renders innecesarios y hydration instantáneo."),
            ("📱", "Rendering Nativo Multi-Plataforma", "Rendering nativo en iOS (UIKit), Android (Jetpack Compose), macOS (AppKit), Windows (WinUI 3) y Linux (GTK4). Sin webview, sin Electron, 100% nativo."),
            ("🎨", "Design System Ierahkwa", "Sistema de diseño completo: tokens, componentes, patrones y guidelines. Tema oscuro soberano con variantes por NEXUS, iconografía indígena y tipografía accesible."),
            ("🧩", "200+ Componentes Accesibles", "Biblioteca de 200+ componentes WCAG 2.1 AA: buttons, forms, tables, charts, maps, editors, calendars, file uploaders y más. Todos con keyboard navigation y screen reader support."),
            ("🌙", "Tema Oscuro Soberano", "Sistema de temas con modo oscuro por defecto, modo claro y alto contraste. Custom properties CSS, temas por NEXUS y preferencia de usuario persistente."),
            ("✨", "Animaciones GPU-Accelerated", "Motor de animaciones con compositing en GPU. Spring physics, gesture-driven animations, shared element transitions y layout animations con 60fps garantizados."),
            ("📦", "State Management Integrado", "Gestión de estado reactivo con stores, computed values y effects. DevTools integrado, time-travel debugging, state persistence y hydration desde server."),
            ("🗺️", "Router Declarativo", "Router con definición declarativa de rutas. Nested routes, lazy loading, guards, transitions entre páginas y URL serialization de estado."),
            ("📝", "Form Validation Reactiva", "Sistema de validación de formularios reactivo. Schema-based validation, async validators, cross-field validation y error messages i18n."),
            ("🖱️", "Drag & Drop Nativo", "Sistema drag & drop con soporte para touch, mouse y stylus. Sortable lists, kanban boards, file drop zones y virtual scroll con drag."),
        ],
        "apis": [
            ("GET", "/api/v1/ui/components", "Catálogo de componentes disponibles con props y ejemplos."),
            ("GET", "/api/v1/ui/themes", "Lista de temas disponibles con tokens y variables CSS."),
            ("GET", "/api/v1/ui/layouts", "Templates de layout: dashboard, form, list, detail, wizard."),
            ("GET", "/api/v1/ui/icons", "Biblioteca de iconos soberanos SVG. Params: category, search"),
            ("GET", "/api/v1/ui/animations", "Presets de animaciones: transitions, micro-interactions, loading."),
            ("GET", "/api/v1/ui/accessibility", "Reporte de accesibilidad WCAG por componente y página."),
        ],
        "db_stores": ["ui-config", "theme-preferences", "component-cache"],
        "arch_layers": [
            ("#e040fb", "COMPONENT", "Reactive Components · Props · Slots · Events", "200+ Components · WCAG AA · Design Tokens"),
            ("#ffd600", "VDOM", "Virtual DOM · Diff Algorithm · Reconciler", "Fine-Grained · Batch Updates · Keyed Lists"),
            ("#7c4dff", "RENDERER", "Platform-Specific · GPU Compositing", "Web · iOS · Android · macOS · Windows · Linux"),
            ("#e040fb", "NATIVE", "UIKit · Jetpack Compose · AppKit · WinUI", "60fps · Spring Physics · Gesture-Driven"),
        ],
        "arch_labels": ["COMPONENT", "VDOM", "RENDERER", "NATIVE"],
    },
]


def generate_html(p):
    slug = p["dir"]
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    db_stores = p["db_stores"]
    arch_layers = p["arch_layers"]

    # OG description (truncated)
    og_desc = desc[:180]

    # Build metrics HTML
    metrics_html = ""
    for val, lbl in metrics:
        metrics_html += f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>\n'

    # Build architecture HTML
    arch_html = ""
    for i, (color, label, line1, line2) in enumerate(arch_layers):
        if i == 0:
            arch_html += f'<span style="color:{color}">┌─ {label} {"─" * (43 - len(label))}┐</span>\n'
        else:
            arch_html += f'<span style="color:{color}">┌─ {label} {"─" * (43 - len(label))}┐</span>\n'
        arch_html += f'│  {line1:<52}│\n'
        arch_html += f'│  {line2:<52}│\n'
        arch_html += f'<span style="color:{color}">└{"─" * 18}┬{"─" * 36}┘</span>\n'
        if i < len(arch_layers) - 1:
            arch_html += f'                   │ <span style="color:#ffd600">REST + gRPC + WebSocket</span>\n'

    # Fix last layer - no trailing connector
    # Remove the last ┬ and replace with ─
    arch_html = arch_html.rstrip('\n')
    last_line_idx = arch_html.rfind('└')
    if last_line_idx != -1:
        # Replace the last closing line
        arch_html = arch_html[:last_line_idx] + f'<span style="color:{arch_layers[-1][0]}">└{"─" * 55}┘</span>'

    # Build cards HTML
    cards_html = ""
    for c_icon, c_title, c_desc in cards:
        cards_html += f'<article class="card"><div class="card-icon" aria-hidden="true">{c_icon}</div><h4>{c_title}</h4><p>{c_desc}</p></article>\n'

    # Build API HTML
    api_html = ""
    for method, endpoint, api_desc in apis:
        color = "#00FF41" if method == "GET" else "#ffd600"
        api_html += f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div><p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{api_desc}</p>\n'

    # Build DB stores
    stores_js = ", ".join([f'"{s}"' for s in db_stores])

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{og_desc}">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} — {subtitle}">
<meta property="og:description" content="{og_desc}">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} — {subtitle}">
<meta name="twitter:description" content="{og_desc}">
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
{metrics_html}</div>

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
<ul style="list-style:none;padding:0"><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 1 proyecto</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Comunidad</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Docs básica</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ CLI básico</li></ul>
</div><div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0"><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 10 proyectos</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte prioritario</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ API completa</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Integraciones</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Analytics</li></ul>
</div><div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Nación</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0"><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Ilimitado</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ SLA 99.99%</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Dedicado</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ On-premise</li><li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ White-label</li></ul>
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
  var STORES=[{stores_js}];
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
    print("=" * 60)
    print("  Creando 8 plataformas .NET Sovereign Replacements")
    print("=" * 60)

    for p in platforms:
        dir_path = os.path.join(BASE_DIR, p["dir"])
        os.makedirs(dir_path, exist_ok=True)

        html = generate_html(p)
        file_path = os.path.join(dir_path, "index.html")

        with open(file_path, "w", encoding="utf-8") as f:
            f.write(html)

        line_count = html.count("\n") + 1
        file_size = len(html.encode("utf-8"))
        print(f"  [OK] {p['dir']}/index.html — {line_count} líneas, {file_size:,} bytes")

    print()
    print("=" * 60)
    print(f"  8 plataformas creadas exitosamente en:")
    print(f"  {BASE_DIR}")
    print("=" * 60)


if __name__ == "__main__":
    main()
