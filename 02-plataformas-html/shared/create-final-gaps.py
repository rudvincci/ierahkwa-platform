#!/usr/bin/env python3
"""
create-final-gaps.py
Genera 8 plataformas finales para cubrir los gaps de comercio e infraestructura.
Pattern B: ~250 lineas, SEO completo, header+nav+quantum badge, hero, stats 6 metrics,
architecture monospace 4 capas, 10 cards, 6 APIs, 3 pricing, footer, 5 scripts,
offline IndexedDB (3 stores), SW registration.
"""
import os

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

PLATFORMS = [
    {
        "dir": "cloud-os-soberano",
        "title": "Cloud OS Soberano",
        "subtitle": "Sistema Operativo Web para Naciones Digitales",
        "icon": "\U0001f4bb",
        "accent": "#00e676",
        "description": "Sistema operativo web soberano que reemplaza Puter, OSjs y Google ChromeOS. Desktop completo en el navegador con file manager, terminal, editor de c\u00f3digo, reproductor multimedia, aplicaciones nativas WASM, multi-usuario con roles, almacenamiento en red P2P soberana, zero cloud Big Tech.",
        "metrics": [
            ("Desktop", "Web"),
            ("WASM", "Apps"),
            ("File", "Manager"),
            ("Terminal", "Integrada"),
            ("Multi", "Usuario"),
            ("P2P", "Storage"),
        ],
        "cards": [
            ("\U0001f5a5\ufe0f", "Desktop Web Completo", "Escritorio completo en el navegador con ventanas, taskbar, men\u00fa de inicio, multi-escritorio y atajos de teclado. Experiencia nativa sin instalar nada."),
            ("\U0001f4c1", "File Manager con Drag & Drop", "Gestor de archivos con drag and drop, vista en \u00e1rbol, previsualizaci\u00f3n, b\u00fasqueda instant\u00e1nea y papelera. Almacenamiento en red P2P soberana."),
            ("\u2328\ufe0f", "Terminal Soberana Integrada", "Terminal completa con bash/zsh emulado, autocompletado, historial persistente y capacidad de ejecutar scripts MameyLang directamente."),
            ("\u270f\ufe0f", "Editor de C\u00f3digo con IA", "Editor de c\u00f3digo con syntax highlighting para 50+ lenguajes, autocompletado con IA soberana, git integrado, extensiones y terminal embebida."),
            ("\U0001f3b5", "Reproductor Multimedia", "Reproductor de audio y video con soporte para todos los formatos, playlists, ecualizador, streaming P2P y biblioteca multimedia personal."),
            ("\u2699\ufe0f", "Aplicaciones WASM Nativas", "Marketplace de aplicaciones compiladas a WebAssembly para rendimiento nativo. Sandbox de seguridad, actualizaciones autom\u00e1ticas, zero dependencias externas."),
            ("\U0001f465", "Multi-Usuario con Roles", "Sistema multi-usuario con roles granulares (admin, usuario, invitado), perfiles personalizables, cuotas de almacenamiento y auditor\u00eda de acceso."),
            ("\U0001f310", "Almacenamiento P2P", "Almacenamiento distribuido en red P2P soberana con replicaci\u00f3n autom\u00e1tica, cifrado end-to-end, versionado de archivos y zero dependencia de cloud."),
            ("\U0001f3ea", "Marketplace de Apps", "Tienda de aplicaciones soberanas con categor\u00edas, reviews, actualizaciones autom\u00e1ticas y monetizaci\u00f3n para desarrolladores en WAMPUM."),
            ("\U0001f3a8", "Temas y Personalizaci\u00f3n", "Personalizaci\u00f3n total del escritorio: temas oscuros/claros, fondos de pantalla, iconos, fuentes, layouts y configuraci\u00f3n exportable entre dispositivos."),
        ],
        "apis": [
            ("POST", "/api/v1/os/files", "Gestionar archivos: crear, mover, copiar, eliminar. Params: action, path, content."),
            ("GET", "/api/v1/os/apps", "Listar aplicaciones instaladas y disponibles en marketplace con filtros."),
            ("POST", "/api/v1/os/terminal", "Ejecutar comando en terminal. Params: command, cwd, env."),
            ("GET", "/api/v1/os/users", "Listar usuarios del sistema con roles, cuotas y \u00faltimo acceso."),
            ("POST", "/api/v1/os/install", "Instalar aplicaci\u00f3n desde marketplace. Params: app_id, permissions."),
            ("GET", "/api/v1/os/settings", "Obtener configuraci\u00f3n del sistema: tema, idioma, atajos, preferencias."),
        ],
        "db_stores": ["os-filesystem", "app-registry", "user-sessions"],
        "arch_layers": [
            ("#00e676", "NAVEGADOR", "Interfaz \u00b7 Ventanas \u00b7 Drag&Drop \u00b7 Teclado", "Desktop Web \u00b7 Taskbar \u00b7 Men\u00fa \u00b7 Multi-escritorio"),
            ("#00bcd4", "DESKTOP ENGINE (WASM)", "Rendering \u00b7 Layout \u00b7 Eventos \u00b7 Animaciones", "WebAssembly \u00b7 Canvas \u00b7 WebGL \u00b7 Workers"),
            ("#7c4dff", "APP MANAGER", "Instalaci\u00f3n \u00b7 Sandbox \u00b7 Permisos \u00b7 Updates", "Marketplace \u00b7 Versiones \u00b7 Dependencias \u00b7 Cuotas"),
            ("#00e676", "P2P STORAGE", "Archivos \u00b7 Replicaci\u00f3n \u00b7 Cifrado \u00b7 Versionado", "Red Soberana \u00b7 IPFS \u00b7 Chunks \u00b7 Redundancia"),
        ],
    },
    {
        "dir": "dns-soberano",
        "title": "DNS Soberano",
        "subtitle": "Sistema de Nombres de Dominio con Privacidad Total",
        "icon": "\U0001f310",
        "accent": "#00bcd4",
        "description": "DNS soberano que reemplaza AdGuard Home, Pi-hole y Cloudflare DNS. Resoluci\u00f3n DNS cifrada con DoH/DoT/DoQ, bloqueo de anuncios y trackers a nivel de red, dominio .nation nativo, DNSSEC integrado, cache distribuido en 847 nodos, zero logs, protecci\u00f3n contra DNS spoofing post-cu\u00e1ntica.",
        "metrics": [
            ("DoH/DoT", "DoQ"),
            ("0", "Logs"),
            (".nation", "DNS"),
            ("DNSSEC", "Integrado"),
            ("847", "Nodos"),
            ("Ad-Block", "Red"),
        ],
        "cards": [
            ("\U0001f512", "DNS-over-HTTPS/TLS/QUIC", "Resoluci\u00f3n DNS completamente cifrada con soporte DoH, DoT y DoQ. Imposible para ISPs o gobiernos interceptar o manipular tus consultas DNS."),
            ("\U0001f6ab", "Bloqueo de Ads y Trackers", "Bloqueo a nivel de red de anuncios, trackers, malware y telemetr\u00eda. Listas curadas para proteger a toda la comunidad sin instalar extensiones."),
            ("\U0001f310", "Dominio .nation Nativo", "Sistema de dominios .nation soberano, independiente de ICANN. Registro, gesti\u00f3n y resoluci\u00f3n de dominios bajo gobernanza ind\u00edgena."),
            ("\U0001f6e1\ufe0f", "DNSSEC Integrado", "Firmas criptogr\u00e1ficas en cada respuesta DNS para prevenir spoofing y envenenamiento de cache. Validaci\u00f3n autom\u00e1tica end-to-end."),
            ("\U0001f4e1", "Cache Distribuido 847 Nodos", "Cache DNS distribuido en 847 nodos soberanos mundiales. Resoluci\u00f3n ultra-r\u00e1pida con redundancia geogr\u00e1fica y failover autom\u00e1tico."),
            ("\U0001f441\ufe0f", "Zero Logs Privacy", "Pol\u00edtica estricta de cero logs: ninguna consulta DNS se almacena, registra o analiza. Privacidad verificable con auditor\u00edas p\u00fablicas."),
            ("\u269b\ufe0f", "Anti-Spoofing Post-Cu\u00e1ntico", "Protecci\u00f3n contra ataques DNS con algoritmos post-cu\u00e1nticos Kyber-768. Futureproof contra computadoras cu\u00e1nticas."),
            ("\U0001f4cb", "Listas de Bloqueo Personalizables", "Listas de bloqueo configurables por comunidad: ads, trackers, adult content, gambling. Import/export compatible con Pi-hole y AdGuard."),
            ("\U0001f4ca", "Dashboard de Estad\u00edsticas", "Panel con estad\u00edsticas en tiempo real: consultas/segundo, dominios bloqueados, latencia promedio, distribuci\u00f3n geogr\u00e1fica, top domains."),
            ("\U0001f50c", "API para Integraci\u00f3n", "API REST completa para gestionar configuraci\u00f3n DNS, listas de bloqueo, dominios .nation y estad\u00edsticas desde cualquier aplicaci\u00f3n."),
        ],
        "apis": [
            ("GET", "/api/v1/dns/resolve", "Resolver dominio. Params: domain, type (A, AAAA, MX, TXT, CNAME)."),
            ("POST", "/api/v1/dns/blocklist", "Gestionar listas de bloqueo. Params: action (add/remove), list_url."),
            ("GET", "/api/v1/dns/stats", "Estad\u00edsticas en tiempo real: queries/s, blocked, latency, top domains."),
            ("POST", "/api/v1/dns/domain", "Registrar/gestionar dominio .nation. Params: domain, records, ttl."),
            ("GET", "/api/v1/dns/cache", "Estado del cache DNS: hit rate, tama\u00f1o, distribuci\u00f3n, TTLs activos."),
            ("POST", "/api/v1/dns/config", "Actualizar configuraci\u00f3n DNS. Params: upstream, doh_enabled, blocklists."),
        ],
        "db_stores": ["dns-cache", "blocklist-rules", "query-stats"],
        "arch_layers": [
            ("#00bcd4", "QUERY DNS", "Consulta \u00b7 Dominio \u00b7 Tipo \u00b7 Cliente", "Navegador \u00b7 App \u00b7 IoT \u00b7 Sistema Operativo"),
            ("#7c4dff", "CIFRADO DoH/DoT", "TLS 1.3 \u00b7 QUIC \u00b7 Kyber-768 \u00b7 Tunnel", "Privacidad \u00b7 Anti-ISP \u00b7 Anti-Spoofing \u00b7 Integridad"),
            ("#f44336", "FILTER + CACHE", "Blocklists \u00b7 Ads \u00b7 Trackers \u00b7 Malware", "Cache Distribuido \u00b7 847 Nodos \u00b7 TTL \u00b7 Hit Rate"),
            ("#00bcd4", "RESOLUCI\u00d3N SOBERANA", "Root Servers \u00b7 .nation \u00b7 DNSSEC \u00b7 Failover", "Independiente ICANN \u00b7 Geo-routing \u00b7 Redundancia"),
        ],
    },
    {
        "dir": "game-engine-soberano",
        "title": "Game Engine Soberano",
        "subtitle": "Motor de Videojuegos para Contenido Cultural Ind\u00edgena",
        "icon": "\U0001f3ae",
        "accent": "#E91E63",
        "description": "Motor de videojuegos soberano que reemplaza Godot, Unity y Unreal. Dise\u00f1ado para crear juegos educativos de cultura ind\u00edgena, rendering 2D/3D con WebGPU, scripting en MameyLang, editor visual, physics engine, audio engine, multiplayer P2P, export a web/mobile/desktop, assets de arte ind\u00edgena.",
        "metrics": [
            ("2D/3D", "WebGPU"),
            ("Mamey", "Lang"),
            ("Editor", "Visual"),
            ("Multi", "Player"),
            ("Multi", "Platform"),
            ("Assets", "Ind\u00edgenas"),
        ],
        "cards": [
            ("\U0001f5bc\ufe0f", "Rendering 2D/3D con WebGPU", "Motor de rendering con soporte 2D y 3D usando WebGPU para rendimiento nativo en el navegador. Shaders personalizables, PBR, iluminaci\u00f3n global."),
            ("\U0001f4bb", "Scripting en MameyLang", "Lenguaje de scripting MameyLang dise\u00f1ado para game dev: tipado fuerte, hot-reload, debugging visual, documentaci\u00f3n en 14 idiomas ind\u00edgenas."),
            ("\U0001f3ac", "Editor Visual de Escenas", "Editor drag-and-drop de escenas: nodos, sprites, tilemaps, UI, particles. Previsualizaci\u00f3n en tiempo real, undo ilimitado, templates."),
            ("\u26a1", "Physics Engine Nativo", "Motor de f\u00edsica 2D/3D: colisiones, rigidbody, joints, raycasting, part\u00edculas. Optimizado para juegos educativos y simulaciones culturales."),
            ("\U0001f50a", "Audio Engine Espacial", "Audio 3D espacial con reverberaci\u00f3n, oclusi\u00f3n, mezcla din\u00e1mica. Soporte para m\u00fasica y narraci\u00f3n en idiomas ind\u00edgenas con subt\u00edtulos."),
            ("\U0001f310", "Multiplayer P2P Soberano", "Multiplayer peer-to-peer sin servidores centralizados. Matchmaking, sincronizaci\u00f3n de estado, chat de voz, lobbies y torneos comunitarios."),
            ("\U0001f4f1", "Export Web/Mobile/Desktop", "Exportaci\u00f3n a HTML5/WebGPU, Android, iOS, Windows, macOS, Linux. Un solo proyecto, m\u00faltiples plataformas, optimizaci\u00f3n autom\u00e1tica."),
            ("\U0001fab6", "Assets de Arte Ind\u00edgena", "Biblioteca de assets con arte, m\u00fasica, sonidos y animaciones de cultura ind\u00edgena. Creados por artistas nativos, licencia soberana."),
            ("\U0001f9b4", "Animaci\u00f3n Skeletal", "Sistema de animaci\u00f3n skeletal 2D/3D con blend trees, IK, state machines. Editor visual de animaciones con timeline y curvas."),
            ("\U0001f3ea", "Marketplace de Assets", "Marketplace comunitario de assets: modelos, texturas, sonidos, scripts, plugins. Monetizaci\u00f3n para creadores en WAMPUM."),
        ],
        "apis": [
            ("POST", "/api/v1/game/project", "Crear/gestionar proyecto de juego. Params: name, type (2D/3D), template."),
            ("GET", "/api/v1/game/assets", "Buscar assets en biblioteca. Params: category, culture, format, license."),
            ("POST", "/api/v1/game/build", "Compilar proyecto para plataforma. Params: project_id, target, options."),
            ("POST", "/api/v1/game/publish", "Publicar juego en marketplace. Params: project_id, price, description."),
            ("GET", "/api/v1/game/multiplayer", "Estado de sesiones multiplayer: lobbies, jugadores, latencia, regiones."),
            ("GET", "/api/v1/game/analytics", "Analytics de juegos publicados: descargas, jugadores activos, retenci\u00f3n."),
        ],
        "db_stores": ["game-projects", "asset-library", "player-sessions"],
        "arch_layers": [
            ("#E91E63", "EDITOR VISUAL", "Escenas \u00b7 Nodos \u00b7 Sprites \u00b7 UI \u00b7 Tilemaps", "Drag&Drop \u00b7 Timeline \u00b7 Inspector \u00b7 Consola"),
            ("#7c4dff", "GAME ENGINE (WASM/WebGPU)", "Rendering \u00b7 Shaders \u00b7 PBR \u00b7 Part\u00edculas", "WebAssembly \u00b7 GPU \u00b7 Workers \u00b7 SIMD"),
            ("#00bcd4", "PHYSICS + AUDIO", "Colisiones \u00b7 Rigidbody \u00b7 Audio 3D \u00b7 Mezcla", "Simulaci\u00f3n \u00b7 Raycasting \u00b7 Reverb \u00b7 Oclusi\u00f3n"),
            ("#E91E63", "MULTIPLAYER P2P", "Matchmaking \u00b7 Sync \u00b7 Chat \u00b7 Torneos", "Peer-to-Peer \u00b7 WebRTC \u00b7 Estado \u00b7 Lobbies"),
        ],
    },
    {
        "dir": "facturacion-soberana",
        "title": "Facturaci\u00f3n Soberana",
        "subtitle": "Sistema de Facturaci\u00f3n Electr\u00f3nica para 19 Naciones",
        "icon": "\U0001f9fe",
        "accent": "#ffd600",
        "description": "Sistema de facturaci\u00f3n electr\u00f3nica que reemplaza Stripe Invoicing, QuickBooks y Facturapi. Cumple normativa fiscal de las 19 naciones, XML/PDF generaci\u00f3n autom\u00e1tica, timbrado digital soberano, integraci\u00f3n con contabilidad y ERP, multi-moneda WAMPUM, reportes fiscales autom\u00e1ticos.",
        "metrics": [
            ("19", "Jurisdicciones"),
            ("XML/PDF", "Auto"),
            ("Timbrado", "Digital"),
            ("Multi", "Moneda"),
            ("Fiscal", "Compliant"),
            ("Auto", "Reportes"),
        ],
        "cards": [
            ("\U0001f4c4", "Factura Electr\u00f3nica XML/PDF", "Generaci\u00f3n autom\u00e1tica de facturas en formato XML y PDF. Cumple est\u00e1ndares UBL, CFDI y formatos locales de las 19 naciones soberanas."),
            ("\U0001f50f", "Timbrado Digital Soberano", "Timbrado con firma digital soberana reconocida por las 19 naciones. Certificados post-cu\u00e1nticos, sellado de tiempo, validaci\u00f3n instant\u00e1nea."),
            ("\u2696\ufe0f", "Compliance 19 Jurisdicciones", "Cumplimiento fiscal autom\u00e1tico para las 19 jurisdicciones soberanas. Reglas de impuestos, retenciones y formatos actualizados en tiempo real."),
            ("\U0001fa99", "Multi-Moneda WAMPUM", "Facturaci\u00f3n en WAMPUM y monedas soberanas locales. Tipo de cambio en tiempo real, conversi\u00f3n autom\u00e1tica, conciliaci\u00f3n multi-moneda."),
            ("\U0001f4ca", "Integraci\u00f3n Contabilidad/ERP", "Integraci\u00f3n nativa con Contabilidad Soberana y ERP. Asientos contables autom\u00e1ticos, conciliaci\u00f3n bancaria, flujo de caja en tiempo real."),
            ("\U0001f4dd", "Notas de Cr\u00e9dito Autom\u00e1ticas", "Generaci\u00f3n autom\u00e1tica de notas de cr\u00e9dito y d\u00e9bito. Cancelaci\u00f3n de facturas con justificaci\u00f3n, flujo de aprobaci\u00f3n configurable."),
            ("\U0001f4b0", "Recibos de Pago", "Recibos de pago electr\u00f3nicos con complemento de pago. Tracking de pagos parciales, recordatorios autom\u00e1ticos, conciliaci\u00f3n."),
            ("\U0001f4e6", "Cat\u00e1logo de Productos", "Cat\u00e1logo centralizado de productos y servicios con claves fiscales, unidades de medida, precios y descuentos configurables."),
            ("\U0001f465", "Clientes y Proveedores", "Directorio de clientes y proveedores con datos fiscales validados, historial de facturaci\u00f3n, saldos pendientes y scoring."),
            ("\U0001f4c8", "Reportes Fiscales SAT/IRS", "Reportes fiscales autom\u00e1ticos compatibles con SAT, IRS y autoridades de las 19 naciones. DIOT, ISR, IVA, declaraciones."),
        ],
        "apis": [
            ("POST", "/api/v1/invoice/create", "Crear factura electr\u00f3nica. Params: client_id, items, currency, tax_rules."),
            ("GET", "/api/v1/invoice/list", "Listar facturas con filtros: fecha, cliente, status, monto, jurisdicci\u00f3n."),
            ("POST", "/api/v1/invoice/stamp", "Timbrar factura con firma digital soberana. Params: invoice_id, cert_id."),
            ("GET", "/api/v1/invoice/pdf", "Descargar factura en PDF. Params: invoice_id, format (carta/A4)."),
            ("POST", "/api/v1/invoice/cancel", "Cancelar factura. Params: invoice_id, reason, credit_note."),
            ("GET", "/api/v1/invoice/reports", "Reportes fiscales: ingresos, impuestos, retenciones por per\u00edodo y naci\u00f3n."),
        ],
        "db_stores": ["invoice-records", "client-catalog", "fiscal-reports"],
        "arch_layers": [
            ("#ffd600", "VENTA/SERVICIO", "Orden \u00b7 Productos \u00b7 Cliente \u00b7 Precio", "Cat\u00e1logo \u00b7 Descuentos \u00b7 Impuestos \u00b7 Moneda"),
            ("#00bcd4", "GENERADOR XML", "CFDI \u00b7 UBL \u00b7 Formato Local \u00b7 Validaci\u00f3n", "Schema \u00b7 Addenda \u00b7 Complementos \u00b7 Sellado"),
            ("#7c4dff", "TIMBRADO DIGITAL", "Firma \u00b7 Certificado PQ \u00b7 Timestamp \u00b7 QR", "Autoridad Soberana \u00b7 Validaci\u00f3n \u00b7 Folio \u00b7 UUID"),
            ("#ffd600", "ENV\u00cdO + CONTABILIDAD", "Email \u00b7 Portal \u00b7 ERP \u00b7 Conciliaci\u00f3n", "Asientos \u00b7 Flujo Caja \u00b7 Reportes \u00b7 Declaraciones"),
        ],
    },
    {
        "dir": "reservaciones-soberano",
        "title": "Reservaciones Soberano",
        "subtitle": "Sistema de Booking y Reservas Multi-Industria",
        "icon": "\U0001f4c5",
        "accent": "#ff9100",
        "description": "Sistema de reservaciones que reemplaza Booking.com, Cal.com y Calendly. Reservas para hoteles, restaurantes, consultas m\u00e9dicas, turnos gubernamentales, eventos culturales, transporte. Widget embebido, confirmaci\u00f3n autom\u00e1tica, recordatorios, pagos WAMPUM integrados, multi-idioma ind\u00edgena.",
        "metrics": [
            ("Multi", "Industria"),
            ("Widget", "Embebido"),
            ("Confirm.", "Auto"),
            ("Pagos", "WAMPUM"),
            ("14", "Idiomas"),
            ("Recorda-", "torios"),
        ],
        "cards": [
            ("\U0001f3e8", "Booking Multi-Industria", "Reservas para hoteles, restaurantes, consultas m\u00e9dicas, turnos, eventos y transporte. Un solo sistema para todas las industrias soberanas."),
            ("\U0001f9e9", "Widget Embebido para Sitios", "Widget JavaScript embebible en cualquier sitio web soberano. Personalizable, responsive, con disponibilidad en tiempo real y pagos integrados."),
            ("\u2705", "Confirmaci\u00f3n Autom\u00e1tica", "Confirmaci\u00f3n instant\u00e1nea por email, SMS y notificaci\u00f3n push. Reglas configurables: auto-confirmar, lista de espera, aprobaci\u00f3n manual."),
            ("\U0001f514", "Recordatorios SMS/Email", "Recordatorios autom\u00e1ticos antes de la cita: 24h, 2h, 30min. Multi-canal (SMS, email, push, WhatsApp), multi-idioma ind\u00edgena."),
            ("\U0001f4b0", "Pagos WAMPUM Integrados", "Pagos y dep\u00f3sitos en WAMPUM al momento de reservar. Cancelaci\u00f3n con reembolso autom\u00e1tico seg\u00fan pol\u00edtica configurable del negocio."),
            ("\U0001f4c5", "Calendario de Disponibilidad", "Calendario visual de disponibilidad con vista d\u00eda/semana/mes. Bloqueo de horarios, excepciones, feriados de las 19 naciones."),
            ("\U0001f4cd", "Multi-Ubicaci\u00f3n", "Gesti\u00f3n de m\u00faltiples ubicaciones/sucursales con horarios independientes, personal asignado y capacidad por sede."),
            ("\u23f3", "Lista de Espera Inteligente", "Lista de espera con notificaci\u00f3n autom\u00e1tica cuando se libera un espacio. Prioridad configurable, tiempo l\u00edmite de confirmaci\u00f3n."),
            ("\U0001f4ca", "Reportes de Ocupaci\u00f3n", "Analytics de ocupaci\u00f3n: tasa de reserva, no-shows, horarios pico, ingreso promedio, tendencias por ubicaci\u00f3n y servicio."),
            ("\U0001f50c", "API para Integraciones", "API REST para integrar reservas con CRM, ERP, facturaci\u00f3n y otros sistemas soberanos del ecosistema Ierahkwa."),
        ],
        "apis": [
            ("POST", "/api/v1/booking/reserve", "Crear reservaci\u00f3n. Params: service_id, datetime, client, guests, payment."),
            ("GET", "/api/v1/booking/availability", "Consultar disponibilidad. Params: service_id, date_range, location."),
            ("POST", "/api/v1/booking/cancel", "Cancelar reservaci\u00f3n. Params: booking_id, reason, refund_policy."),
            ("GET", "/api/v1/booking/calendar", "Calendario de reservas con vista configurable y filtros por ubicaci\u00f3n."),
            ("POST", "/api/v1/booking/reminder", "Configurar recordatorios. Params: booking_id, channels, timing."),
            ("GET", "/api/v1/booking/reports", "Reportes de ocupaci\u00f3n, ingresos y tendencias por per\u00edodo y ubicaci\u00f3n."),
        ],
        "db_stores": ["booking-reservations", "availability-slots", "reminder-queue"],
        "arch_layers": [
            ("#ff9100", "CLIENTE", "Web \u00b7 Widget \u00b7 App \u00b7 API", "B\u00fasqueda \u00b7 Selecci\u00f3n \u00b7 Datos \u00b7 Pago"),
            ("#00bcd4", "WIDGET/APP", "Calendario \u00b7 Disponibilidad \u00b7 Formulario", "Responsive \u00b7 Multi-idioma \u00b7 Personalizable"),
            ("#7c4dff", "DISPONIBILIDAD ENGINE", "Slots \u00b7 Capacidad \u00b7 Reglas \u00b7 Excepciones", "Real-time \u00b7 Conflictos \u00b7 Overbooking \u00b7 Buffer"),
            ("#ff9100", "CONFIRMACI\u00d3N + PAGO WAMPUM", "Email \u00b7 SMS \u00b7 Push \u00b7 Recibo", "WAMPUM \u00b7 Dep\u00f3sito \u00b7 Reembolso \u00b7 Conciliaci\u00f3n"),
        ],
    },
    {
        "dir": "helpdesk-soberano",
        "title": "Helpdesk Soberano",
        "subtitle": "Sistema de Tickets y Soporte al Cliente Soberano",
        "icon": "\U0001f3ab",
        "accent": "#ff9100",
        "description": "Sistema de helpdesk que reemplaza Zendesk, Freshdesk y osTicket. Gesti\u00f3n de tickets multi-canal (email, chat, tel\u00e9fono, redes), base de conocimiento, chatbot con IA en 14 idiomas ind\u00edgenas, SLA autom\u00e1ticos, escalamiento inteligente, portal de autoservicio, satisfacci\u00f3n del cliente.",
        "metrics": [
            ("Multi", "Canal"),
            ("Chatbot", "IA"),
            ("14", "Idiomas"),
            ("SLA", "Auto"),
            ("Base", "Conocimiento"),
            ("CSAT", "Tracking"),
        ],
        "cards": [
            ("\U0001f4e9", "Tickets Multi-Canal", "Recepci\u00f3n de tickets desde email, chat en vivo, tel\u00e9fono, redes sociales y formulario web. Unificaci\u00f3n en una sola bandeja para el agente."),
            ("\U0001f916", "Chatbot IA 14 Idiomas", "Chatbot con IA soberana que atiende en 14 idiomas ind\u00edgenas y espa\u00f1ol/ingl\u00e9s. Resoluci\u00f3n autom\u00e1tica de preguntas frecuentes, escalamiento inteligente."),
            ("\U0001f4da", "Base de Conocimiento", "Wiki interna y p\u00fablica con art\u00edculos, gu\u00edas, FAQs y tutoriales. B\u00fasqueda inteligente, sugerencias autom\u00e1ticas, versionado de contenido."),
            ("\u23f1\ufe0f", "SLA Autom\u00e1ticos", "Acuerdos de nivel de servicio configurables por prioridad, categor\u00eda y cliente. Alertas de incumplimiento, m\u00e9tricas de cumplimiento en dashboard."),
            ("\U0001f4c8", "Escalamiento Inteligente", "Escalamiento autom\u00e1tico por tiempo, complejidad o sentimiento del cliente. Routing inteligente al agente m\u00e1s capacitado para cada tema."),
            ("\U0001f310", "Portal de Autoservicio", "Portal para clientes con seguimiento de tickets, base de conocimiento, foro comunitario y env\u00edo de nuevos tickets sin contactar agente."),
            ("\u2b50", "Encuestas de Satisfacci\u00f3n", "Encuestas CSAT autom\u00e1ticas al cerrar ticket. NPS peri\u00f3dico, an\u00e1lisis de sentimiento, reportes de tendencias de satisfacci\u00f3n."),
            ("\u26a1", "Macros y Respuestas R\u00e1pidas", "Templates de respuestas predefinidas, macros con acciones autom\u00e1ticas (cambiar prioridad, asignar, etiquetar) para productividad del agente."),
            ("\U0001f464", "Dashboard de Agentes", "Panel por agente: tickets asignados, tiempo de respuesta, CSAT individual, carga de trabajo, disponibilidad y rendimiento hist\u00f3rico."),
            ("\U0001f4ca", "Reportes y M\u00e9tricas", "Analytics completos: tiempo medio de resoluci\u00f3n, first response time, tickets por canal, SLA compliance, CSAT trend, backlog."),
        ],
        "apis": [
            ("POST", "/api/v1/helpdesk/ticket", "Crear ticket de soporte. Params: subject, description, channel, priority."),
            ("GET", "/api/v1/helpdesk/tickets", "Listar tickets con filtros: status, priority, agent, channel, date_range."),
            ("POST", "/api/v1/helpdesk/reply", "Responder a ticket. Params: ticket_id, message, internal_note, status."),
            ("GET", "/api/v1/helpdesk/kb", "Buscar en base de conocimiento. Params: query, category, language."),
            ("POST", "/api/v1/helpdesk/chatbot", "Interacci\u00f3n con chatbot IA. Params: message, language, session_id."),
            ("GET", "/api/v1/helpdesk/metrics", "M\u00e9tricas del helpdesk: CSAT, SLA, tiempos, volumen, agentes, tendencias."),
        ],
        "db_stores": ["helpdesk-tickets", "knowledge-base", "agent-sessions"],
        "arch_layers": [
            ("#ff9100", "CLIENTE (EMAIL/CHAT/TEL)", "Email \u00b7 Chat \u00b7 Tel\u00e9fono \u00b7 Redes \u00b7 Web", "Multi-canal \u00b7 Unificado \u00b7 Tracking \u00b7 Historial"),
            ("#00bcd4", "ROUTER MULTI-CANAL", "Clasificaci\u00f3n \u00b7 Prioridad \u00b7 Routing \u00b7 SLA", "IA \u00b7 Categorizaci\u00f3n \u00b7 Asignaci\u00f3n \u00b7 Cola"),
            ("#7c4dff", "AGENTE/CHATBOT IA", "Dashboard \u00b7 Macros \u00b7 KB \u00b7 Chatbot 14 idiomas", "Respuestas \u00b7 Notas \u00b7 Escalamiento \u00b7 Templates"),
            ("#ff9100", "RESOLUCI\u00d3N + CSAT", "Cierre \u00b7 Encuesta \u00b7 M\u00e9tricas \u00b7 Reportes", "CSAT \u00b7 NPS \u00b7 Tendencias \u00b7 Mejora Continua"),
        ],
    },
    {
        "dir": "email-marketing-soberano",
        "title": "Email Marketing Soberano",
        "subtitle": "Plataforma de Email Marketing y Newsletter Soberana",
        "icon": "\U0001f4e7",
        "accent": "#e040fb",
        "description": "Plataforma de email marketing que reemplaza Mailchimp, SendGrid y Brevo. Campa\u00f1as masivas con entrega soberana (sin depender de servidores Big Tech), editor visual drag-and-drop, segmentaci\u00f3n avanzada, A/B testing, automatizaci\u00f3n de flujos, analytics de apertura/clic, GDPR/compliance soberano.",
        "metrics": [
            ("Entrega", "Soberana"),
            ("Editor", "Visual"),
            ("A/B", "Testing"),
            ("Auto", "Flujos"),
            ("Analytics", "Detallado"),
            ("GDPR", "Compliant"),
        ],
        "cards": [
            ("\U0001f3a8", "Editor Visual Drag-and-Drop", "Editor de emails con bloques arrastrables: texto, imagen, bot\u00f3n, columnas, video, social. Preview en desktop/mobile, HTML personalizable."),
            ("\U0001f4e1", "Entrega Soberana sin Big Tech", "Infraestructura de env\u00edo propia en 847 nodos soberanos. DKIM, SPF, DMARC configurados. Sin depender de AWS SES, SendGrid ni Mailgun."),
            ("\U0001f3af", "Segmentaci\u00f3n Avanzada", "Segmentos din\u00e1micos por comportamiento, demograf\u00eda, engagement, compras, idioma y naci\u00f3n. Filtros combinables con l\u00f3gica AND/OR."),
            ("\U0001f52c", "A/B Testing Autom\u00e1tico", "Test A/B de asunto, contenido, hora de env\u00edo y remitente. Selecci\u00f3n autom\u00e1tica del ganador por apertura, clic o conversi\u00f3n."),
            ("\u2699\ufe0f", "Automatizaci\u00f3n de Flujos", "Workflows visuales: bienvenida, abandono de carrito, re-engagement, cumplea\u00f1os, onboarding. Triggers por evento, tiempo o condici\u00f3n."),
            ("\U0001f4ca", "Analytics Apertura/Clic", "M\u00e9tricas en tiempo real: tasa de apertura, clics, rebotes, unsubs, heatmap de clics, geolocalizaci\u00f3n de aperturas, dispositivos."),
            ("\U0001f4cb", "Templates Personalizables", "Galer\u00eda de templates responsivos para newsletters, promociones, transaccionales, eventos culturales. Personalizables con merge tags."),
            ("\U0001f465", "Lista de Suscriptores", "Gesti\u00f3n de listas con import/export CSV, deduplicaci\u00f3n, limpieza autom\u00e1tica de bounces, tags, campos personalizados y scoring."),
            ("\U0001f517", "Unsubscribe One-Click", "Unsub con un clic cumpliendo RFC 8058. Gesti\u00f3n de preferencias, resuscripci\u00f3n, feedback de motivo de baja."),
            ("\U0001f6e1\ufe0f", "Compliance GDPR Soberano", "Compliance GDPR y regulaci\u00f3n soberana: consentimiento expl\u00edcito, derecho al olvido, portabilidad de datos, auditor\u00eda de consentimientos."),
        ],
        "apis": [
            ("POST", "/api/v1/email/campaign", "Crear campa\u00f1a de email. Params: name, subject, template_id, list_id."),
            ("GET", "/api/v1/email/lists", "Listar listas de suscriptores con conteo, tags y fecha de creaci\u00f3n."),
            ("POST", "/api/v1/email/send", "Enviar campa\u00f1a. Params: campaign_id, schedule, ab_test_config."),
            ("GET", "/api/v1/email/analytics", "Analytics de campa\u00f1a: opens, clicks, bounces, unsubs, heatmap, geo."),
            ("POST", "/api/v1/email/template", "Crear/editar template. Params: name, html, category, merge_tags."),
            ("POST", "/api/v1/email/automate", "Crear flujo de automatizaci\u00f3n. Params: trigger, steps, conditions."),
        ],
        "db_stores": ["email-campaigns", "subscriber-lists", "analytics-cache"],
        "arch_layers": [
            ("#e040fb", "CAMPA\u00d1A", "Nombre \u00b7 Asunto \u00b7 Contenido \u00b7 Lista", "A/B Test \u00b7 Programaci\u00f3n \u00b7 Segmento \u00b7 Tags"),
            ("#00bcd4", "EDITOR VISUAL", "Bloques \u00b7 Templates \u00b7 Merge Tags \u00b7 Preview", "Drag&Drop \u00b7 Responsive \u00b7 HTML \u00b7 Mobile"),
            ("#7c4dff", "SEGMENTACI\u00d3N", "Filtros \u00b7 Comportamiento \u00b7 Engagement \u00b7 Naci\u00f3n", "Din\u00e1mico \u00b7 AND/OR \u00b7 Scoring \u00b7 Exclusiones"),
            ("#e040fb", "ENTREGA SOBERANA + ANALYTICS", "847 Nodos \u00b7 DKIM \u00b7 SPF \u00b7 DMARC", "Opens \u00b7 Clicks \u00b7 Bounces \u00b7 Heatmap \u00b7 Geo"),
        ],
    },
    {
        "dir": "backup-soberano",
        "title": "Backup Soberano",
        "subtitle": "Sistema de Respaldos Autom\u00e1ticos Cifrados",
        "icon": "\U0001f4be",
        "accent": "#00e676",
        "description": "Sistema de backups que reemplaza Veeam, Acronis y Backblaze. Respaldos autom\u00e1ticos incrementales de todas las 400+ plataformas, cifrado post-cu\u00e1ntico at-rest, almacenamiento distribuido en 847 nodos, disaster recovery, point-in-time restore, backup de blockchain y bases de datos, zero data loss.",
        "metrics": [
            ("400+", "Plataformas"),
            ("Incre-", "mental"),
            ("Cifrado", "PQ"),
            ("847", "Nodos"),
            ("Disaster", "Recovery"),
            ("0 Data", "Loss"),
        ],
        "cards": [
            ("\U0001f4e6", "Backup Incremental Autom\u00e1tico", "Respaldos incrementales autom\u00e1ticos que solo copian los cambios. Programaci\u00f3n flexible: horaria, diaria, semanal. Minimal bandwidth, m\u00e1xima eficiencia."),
            ("\U0001f510", "Cifrado Post-Cu\u00e1ntico At-Rest", "Cifrado Kyber-768 de todos los backups en reposo. Claves gestionadas con HSM soberano, rotaci\u00f3n autom\u00e1tica, zero-knowledge del operador."),
            ("\U0001f4e1", "Almacenamiento Distribuido 847 Nodos", "Backups distribuidos en 847 nodos soberanos con replicaci\u00f3n geogr\u00e1fica. M\u00ednimo 3 copias en regiones distintas, failover autom\u00e1tico."),
            ("\U0001f504", "Disaster Recovery Plan", "Plan de disaster recovery con RTO y RPO configurables. Failover autom\u00e1tico, runbooks documentados, simulacros peri\u00f3dicos automatizados."),
            ("\u23f1\ufe0f", "Point-in-Time Restore", "Restauraci\u00f3n a cualquier punto en el tiempo con granularidad de minutos. Selecci\u00f3n de archivos individuales, bases de datos o plataformas completas."),
            ("\u26d3\ufe0f", "Backup de Blockchain", "Respaldo completo de todas las blockchains del ecosistema: MameyNode, WAMPUM, contratos, NFTs. Snapshot consistente con verificaci\u00f3n de integridad."),
            ("\U0001f5c4\ufe0f", "Backup de Bases de Datos", "Hot backup de todas las bases de datos: PostgreSQL, IndexedDB, Redis, IPFS. Consistencia garantizada con WAL archiving y snapshots at\u00f3micos."),
            ("\u2705", "Verificaci\u00f3n de Integridad", "Verificaci\u00f3n autom\u00e1tica post-backup: checksums SHA-256, restauraci\u00f3n de prueba, alertas de corrupci\u00f3n, reportes de integridad semanal."),
            ("\U0001f4cb", "Retenci\u00f3n Configurable", "Pol\u00edticas de retenci\u00f3n flexibles: diario 30 d\u00edas, semanal 12 semanas, mensual 24 meses, anual ilimitado. GFS (Grandfather-Father-Son)."),
            ("\U0001f4ca", "Dashboard de Estado", "Panel con estado de todos los backups: \u00faltimo exitoso, tama\u00f1o, duraci\u00f3n, alertas, tendencias de crecimiento, predicci\u00f3n de capacidad."),
        ],
        "apis": [
            ("POST", "/api/v1/backup/create", "Crear backup manual. Params: scope (platform/db/blockchain), targets."),
            ("GET", "/api/v1/backup/status", "Estado de backups: \u00faltimo exitoso, en progreso, errores, tama\u00f1o total."),
            ("POST", "/api/v1/backup/restore", "Restaurar backup. Params: backup_id, target, point_in_time, scope."),
            ("GET", "/api/v1/backup/history", "Historial de backups con filtros: fecha, tipo, estado, plataforma."),
            ("POST", "/api/v1/backup/verify", "Verificar integridad de backup. Params: backup_id, full_check."),
            ("GET", "/api/v1/backup/schedule", "Programaci\u00f3n de backups: horarios, frecuencia, targets, retenci\u00f3n."),
        ],
        "db_stores": ["backup-manifests", "restore-points", "integrity-checks"],
        "arch_layers": [
            ("#00e676", "PLATAFORMAS (400+)", "Apps \u00b7 DBs \u00b7 Blockchain \u00b7 Archivos", "Agentes \u00b7 Snapshots \u00b7 WAL \u00b7 Consistencia"),
            ("#00bcd4", "SNAPSHOT INCREMENTAL", "Diff \u00b7 Dedup \u00b7 Compresi\u00f3n \u00b7 Delta", "Changed Blocks \u00b7 Bandwidth \u00b7 Eficiencia \u00b7 Schedule"),
            ("#7c4dff", "CIFRADO PQ", "Kyber-768 \u00b7 HSM \u00b7 Rotaci\u00f3n \u00b7 ZK", "At-Rest \u00b7 In-Transit \u00b7 Claves \u00b7 Integridad"),
            ("#00e676", "STORAGE DISTRIBUIDO 847 NODOS", "Replicaci\u00f3n \u00b7 Geo \u00b7 Failover \u00b7 Retenci\u00f3n", "3 Copias \u00b7 GFS \u00b7 Verificaci\u00f3n \u00b7 Predicci\u00f3n"),
        ],
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
    arch = p["arch_layers"]

    # Build API section
    api_lines = []
    for method, endpoint, desc_api in apis:
        method_color = "#ffd600" if method == "POST" else "#00FF41"
        api_lines.append(f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{method_color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div>')
        api_lines.append(f'<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{desc_api}</p>')
    api_html = "\n".join(api_lines)

    # Build architecture
    arch_lines = []
    for color, layer_name, line1, line2 in arch:
        pad = max(1, 47 - len(layer_name))
        arch_lines.append(f'<span style="color:{color}">\u250c\u2500 {layer_name} {"\u2500" * pad}\u2510</span>')
        arch_lines.append(f'\u2502  {line1:<55}\u2502')
        arch_lines.append(f'\u2502  {line2:<55}\u2502')
        arch_lines.append(f'<span style="color:{color}">\u2514\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2534\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2518</span>')
        arch_lines.append('                   \u2502')
    # Remove last connector
    if arch_lines:
        arch_lines.pop()
    arch_html = "\n".join(arch_lines)

    # Build cards
    card_lines = []
    for c_icon, c_title, c_desc in cards:
        card_lines.append(f'<article class="card">\n<div class="card-icon" aria-hidden="true">{c_icon}</div>\n<h4>{c_title}</h4>\n<p>{c_desc}</p>\n</article>')
    cards_html = "\n".join(card_lines)

    # Build stats
    stat_lines = []
    for val, lbl in metrics:
        stat_lines.append(f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>')
    stats_html = "\n".join(stat_lines)

    # DB stores for IndexedDB
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
<meta name="description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta property="og:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cu\u00e1ntico Kyber-768, blockchain MameyNode y soberan\u00eda digital total.">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} \u2014 Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{title} \u2014 plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle}.">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} \u2014 Ierahkwa Ne Kanienke</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header>
<div class="logo"><div class="logo-icon" aria-hidden="true">{icon}</div><h1>{title}</h1></div>
<nav aria-label="Navegacion principal">
<a href="#dashboard" aria-current="page">Dashboard</a>
<a href="#features">Modulos</a>
<a href="#api">API</a>
<a href="#pricing">Precios</a>
</nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">\u269b\ufe0f</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar M\u00f3dulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{stats_html}
</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">\U0001f3d7\ufe0f</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}
</div>
</div>

<div class="section-title" id="features">
<h3>M\u00f3dulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}
</div>

<div class="section" id="api">
<h2><span aria-hidden="true">\U0001f50c</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}
</div>
</div>

<div class="section-title" id="pricing">
<h3>Planes Soberanos</h3>
<p>Empieza gratis. Escala soberanamente.</p>
</div>

<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Guerrero</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">0 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Dashboard b\u00e1sico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Naci\u00f3n</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Multi-naci\u00f3n</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">\u2713 Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">\U0001f6e1\ufe0f</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script src="../shared/ierahkwa-interconnect.js"></script>
<script>
(function(){{
  var DB_NAME='ierahkwa-{slug}';
  var DB_VER=1;
  var STORES=[{stores_js}];
  var db=null;
  function openDB(){{
    return new Promise(function(resolve,reject){{
      var req=indexedDB.open(DB_NAME,DB_VER);
      req.onupgradeneeded=function(){{
        var d=req.result;
        STORES.forEach(function(s){{
          if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})
        }});
      }};
      req.onsuccess=function(){{db=req.result;resolve(db)}};
      req.onerror=function(){{reject(req.error)}}
    }})
  }}
  function showOfflineBanner(show){{
    var b=document.getElementById('offline-banner');
    if(!b){{
      b=document.createElement('div');
      b.id='offline-banner';
      b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';
      b.textContent='Modo Offline \u2014 Datos y operaciones pendientes disponibles offline.';
      document.body.appendChild(b)
    }}
    b.style.transform=show?'translateY(0)':'translateY(100%)'
  }}
  function init(){{
    openDB().then(function(){{
      window.addEventListener('online',function(){{showOfflineBanner(false)}});
      window.addEventListener('offline',function(){{showOfflineBanner(true)}});
      if(!navigator.onLine)showOfflineBanner(true);
      console.log('[{slug}] Offline module ready')
    }})
  }}
  init()
}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''
    return html


def main():
    created = []
    for p in PLATFORMS:
        dirpath = os.path.join(BASE, p["dir"])
        os.makedirs(dirpath, exist_ok=True)
        filepath = os.path.join(dirpath, "index.html")
        html = generate_html(p)
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(html)
        line_count = html.count("\n") + 1
        created.append((p["dir"], line_count))
        print(f"  [OK] {p['dir']}/index.html -- {line_count} lineas")

    print(f"\n{'='*60}")
    print(f"  TOTAL: {len(created)} plataformas creadas")
    print(f"{'='*60}")
    for name, lines in created:
        status = "OK" if lines >= 180 else "WARN"
        print(f"  [{status}] {name}: {lines} lineas")


if __name__ == "__main__":
    main()
