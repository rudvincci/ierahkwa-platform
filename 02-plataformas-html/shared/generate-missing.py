#!/usr/bin/env python3
"""
Generate missing platforms identified from research:
- Open source tools (Audacity, OBS, Bitwarden, ProtonVPN, etc.)
- Business software (Capterra categories: accounting, HR, ERP, etc.)
- Government services (notifications, permits, tax, etc.)
- Knowledge base (MediaWiki, BookStack, etc.)
NO DUPLICATES - all cross-checked against existing 196+ platforms.
"""
import os, pathlib

BASE = pathlib.Path(__file__).resolve().parent.parent

# (dirname, name, emoji, subtitle, accent, description, features[10])
PLATFORMS = [
    # ═══════ NEXUS ESCRITORIO (#26C6DA) ═══════
    ("audio-editor-soberano", "Audio Editor Soberano", "🎵", "Editor de Audio Profesional", "#26C6DA",
     "Editor de audio profesional con grabacion multi-pista, mezcla, efectos y produccion de podcasts. Reemplazo soberano de Audacity, Adobe Audition y GarageBand.",
     [("🎵","Grabacion Multi-Pista","Graba multiples pistas de audio simultaneamente."),
      ("🎛️","Mezcla Profesional","Mezclador con ecualizador, compresor y efectos."),
      ("🎙️","Produccion Podcast","Herramientas especializadas para podcasts."),
      ("✨","Efectos","200+ efectos: reverb, delay, chorus, distorsion."),
      ("🤖","AI Mastering","Masterizacion automatica con AI."),
      ("📦","Exportacion","Exporta a MP3, WAV, FLAC, OGG y 30+ formatos."),
      ("🔇","Reduccion Ruido","Eliminacion de ruido de fondo con AI."),
      ("🎹","MIDI","Soporte completo de instrumentos MIDI."),
      ("📱","Multiplataforma","Funciona en navegador, escritorio y tablet."),
      ("💾","Offline","Edicion completa sin conexion a internet.")]),

    ("mapas-mentales-soberano", "Mapas Mentales Soberano", "🧠", "Mapas Mentales y Diagramas", "#26C6DA",
     "Herramienta de mapas mentales, diagramas de flujo y brainstorming visual con colaboracion en tiempo real. Reemplazo soberano de FreeMind, XMind y MindMeister.",
     [("🧠","Mapas Mentales","Crea mapas mentales ilimitados con ramas."),
      ("🔀","Diagramas de Flujo","Flowcharts, organigramas y diagramas UML."),
      ("👥","Colaboracion","Edicion colaborativa en tiempo real."),
      ("🎨","Temas","50+ temas visuales y estilos personalizables."),
      ("🤖","AI Generador","Describe tu idea y AI genera el mapa."),
      ("📁","Exportacion","Exporta a PNG, SVG, PDF y Markdown."),
      ("📱","Movil","App movil con sincronizacion automatica."),
      ("🔗","Integraciones","Conecta con docs, notas y proyectos soberanos."),
      ("📊","Presentacion","Modo presentacion para reuniones."),
      ("💾","Offline","Funciona sin conexion con sync posterior.")]),

    ("wiki-soberana", "Wiki Soberana", "📚", "Base de Conocimiento Wiki", "#26C6DA",
     "Plataforma de wiki y base de conocimiento con editor WYSIWYG, busqueda semantica y gestion de documentacion. Reemplazo soberano de MediaWiki, BookStack y Confluence.",
     [("📚","Editor WYSIWYG","Editor visual sin necesidad de codigo."),
      ("📂","Organizacion","Espacios, libros, capitulos y paginas."),
      ("🔍","Busqueda AI","Busqueda semantica que entiende el contexto."),
      ("👥","Colaboracion","Edicion simultanea con historial de versiones."),
      ("🔒","Permisos","Control de acceso granular por pagina y espacio."),
      ("🌐","Multilingue","Soporte para 14 lenguas indigenas."),
      ("📊","Plantillas","Plantillas para documentacion tecnica y procesos."),
      ("🤖","AI Asistente","Resume, traduce y genera contenido con AI."),
      ("📱","Responsive","Acceso desde cualquier dispositivo."),
      ("💾","Offline","Lectura offline con sincronizacion.")]),

    ("firma-digital-soberana", "Firma Digital Soberana", "✍️", "Firma Electronica y Contratos", "#26C6DA",
     "Plataforma de firma electronica con validez legal, contratos digitales y certificados blockchain. Reemplazo soberano de DocuSign, Adobe Sign y HelloSign.",
     [("✍️","Firma Electronica","Firma documentos con validez legal."),
      ("📄","Contratos","Crea y gestiona contratos digitales."),
      ("🔗","Blockchain","Certificacion inmutable en blockchain."),
      ("📱","Firma Movil","Firma desde cualquier dispositivo."),
      ("👥","Multi-Firmante","Envia a multiples firmantes en orden."),
      ("📋","Plantillas","Templates de contratos pre-configurados."),
      ("🔔","Notificaciones","Alertas de firma pendiente por correo."),
      ("📊","Auditoria","Trail completo de cada documento."),
      ("🔒","Cifrado","E2E encryption en todos los documentos."),
      ("🤖","AI Verificacion","Verificacion de identidad con AI.")]),

    ("gestion-tiempo-soberano", "Gestion de Tiempo Soberana", "⏱️", "Control de Tiempo y Productividad", "#26C6DA",
     "Herramienta de seguimiento de tiempo, productividad y hojas de horas con reportes automaticos. Reemplazo soberano de Clockify, Toggl y Harvest.",
     [("⏱️","Cronometro","Registra tiempo con un clic."),
      ("📊","Reportes","Dashboards de productividad y horas."),
      ("📋","Hojas de Horas","Timesheets semanales y mensuales."),
      ("👥","Equipos","Gestion de tiempo por equipo y proyecto."),
      ("💰","Facturacion","Convierte horas en facturas WAMPUM."),
      ("🤖","AI Insights","AI analiza patrones de productividad."),
      ("📱","App Movil","Registra tiempo desde el celular."),
      ("🔗","Integraciones","Conecta con proyecto y calendario soberano."),
      ("🏷️","Etiquetas","Categoriza tiempo por cliente y tarea."),
      ("💾","Offline","Funciona sin conexion.")]),

    # ═══════ NEXUS VOCES (#e040fb) ═══════
    ("streaming-estudio-soberano", "Streaming Estudio Soberano", "📹", "Estudio de Streaming y Grabacion", "#e040fb",
     "Estudio de grabacion de pantalla, transmision en vivo y produccion de contenido. Reemplazo soberano de OBS Studio, Streamlabs y Camtasia.",
     [("📹","Transmision en Vivo","Streaming a multiples plataformas."),
      ("🖥️","Grabacion Pantalla","Captura pantalla en 4K con audio."),
      ("🎬","Escenas","Escenas configurables con transiciones."),
      ("🎙️","Audio Mixing","Mezcla de multiples fuentes de audio."),
      ("🤖","AI Camaras","AI para seguimiento de rostro y encuadre."),
      ("💬","Chat Overlay","Overlay de chat en la transmision."),
      ("📊","Analytics","Metricas de audiencia en tiempo real."),
      ("🎨","Overlays","Diseña overlays y alertas personalizadas."),
      ("📱","Movil","Transmite desde el celular."),
      ("💾","Grabacion Local","Grabacion offline sin streaming.")]),

    # ═══════ NEXUS ESCUDO (#f44336) ═══════
    ("vpn-soberana", "VPN Soberana", "🛡️", "Red Privada Virtual Soberana", "#f44336",
     "Red privada virtual soberana con servidores en 19 naciones, cifrado post-cuantico y zero-logs. Reemplazo soberano de ProtonVPN, NordVPN y ExpressVPN.",
     [("🛡️","Cifrado Post-Cuantico","Proteccion contra amenazas cuanticas."),
      ("🌐","19 Naciones","Servidores en las 19 naciones soberanas."),
      ("🚫","Zero Logs","Cero registros de actividad garantizado."),
      ("⚡","WireGuard","Protocolo WireGuard para maxima velocidad."),
      ("📱","Multi-Dispositivo","Apps para todos los dispositivos."),
      ("🔒","Kill Switch","Desconexion automatica si cae la VPN."),
      ("🌍","Split Tunneling","Elige que trafico va por VPN."),
      ("🤖","AI Routing","Seleccion inteligente de servidor."),
      ("👥","Familias","Plan familiar para 6 dispositivos."),
      ("💾","Offline Config","Configuracion offline disponible.")]),

    ("contrasenas-soberana", "Contrasenas Soberana", "🔑", "Gestor de Contrasenas Soberano", "#f44336",
     "Gestor de contrasenas con cifrado E2E, generador seguro y autenticacion multifactor. Reemplazo soberano de Bitwarden, 1Password y LastPass.",
     [("🔑","Vault Cifrado","Boveda cifrada E2E para contrasenas."),
      ("🔐","Generador","Genera contrasenas seguras al instante."),
      ("📱","Auto-Fill","Autocompletado en navegador y apps."),
      ("👥","Compartir","Comparte contrasenas de forma segura."),
      ("🔒","2FA/MFA","Autenticacion multifactor integrada."),
      ("🤖","AI Alertas","Detecta contrasenas comprometidas."),
      ("📋","Notas Seguras","Almacena notas, tarjetas y documentos."),
      ("🌐","Multi-Dispositivo","Sincronizacion entre todos tus dispositivos."),
      ("🏢","Empresas","Gestion de contrasenas para equipos."),
      ("💾","Offline","Acceso offline a tu boveda.")]),

    # ═══════ NEXUS TESORO (#ffd600) ═══════
    ("contabilidad-soberana", "Contabilidad Soberana", "📒", "Contabilidad y Finanzas", "#ffd600",
     "Software de contabilidad con libro mayor, cuentas por cobrar/pagar, estados financieros y reportes fiscales WAMPUM. Reemplazo soberano de QuickBooks, Xero y FreshBooks.",
     [("📒","Libro Mayor","Contabilidad de doble entrada automatica."),
      ("💰","Cuentas por Cobrar","Gestion de facturas y cobros."),
      ("📊","Estados Financieros","Balance, P&L y flujo de caja."),
      ("🏦","Conciliacion","Conciliacion bancaria automatica."),
      ("💱","Multi-Moneda","Soporte WAMPUM, fiat y cripto."),
      ("📋","Reportes Fiscales","Reportes listos para impuestos."),
      ("🤖","AI Categorizacion","AI categoriza transacciones."),
      ("📱","App Movil","Captura recibos con la camara."),
      ("👥","Multi-Empresa","Gestion de multiples empresas."),
      ("💾","Offline","Registra transacciones sin conexion.")]),

    ("nomina-soberana", "Nomina Soberana", "💵", "Gestion de Nomina y Salarios", "#ffd600",
     "Sistema de nomina con calculo de salarios, deducciones, impuestos y pagos automaticos en WAMPUM o fiat. Reemplazo soberano de Gusto, ADP y Paylocity.",
     [("💵","Calculo Automatico","Calcula salarios, horas extra y bonos."),
      ("📋","Deducciones","Impuestos, seguros y contribuciones."),
      ("💰","Pagos","Pagos automaticos en WAMPUM o fiat."),
      ("📊","Reportes","Recibos de nomina y reportes fiscales."),
      ("📅","Programacion","Nomina semanal, quincenal o mensual."),
      ("🤖","AI Compliance","Verifica cumplimiento legal automatico."),
      ("👥","Multi-Pais","Soporte para 19 legislaciones nacionales."),
      ("📱","Self-Service","Portal para empleados."),
      ("🔗","Integraciones","Conecta con contabilidad y RRHH."),
      ("🔒","Cifrado","Datos salariales cifrados E2E.")]),

    ("impuestos-soberano", "Impuestos Soberano", "🏛️", "Gestion Fiscal y Tributaria", "#ffd600",
     "Plataforma de gestion fiscal con declaraciones de impuestos, calculos automaticos y cumplimiento tributario. Reemplazo soberano de TurboTax y H&R Block.",
     [("🏛️","Declaraciones","Presenta declaraciones fiscales digitales."),
      ("📊","Calculo AI","AI calcula impuestos automaticamente."),
      ("📋","Formularios","Formularios fiscales pre-llenados."),
      ("💰","Deducciones","Identifica deducciones aplicables."),
      ("📅","Calendario Fiscal","Recordatorios de fechas limite."),
      ("🤖","AI Asistente","Asistente fiscal con AI."),
      ("📁","Documentos","Almacena comprobantes y facturas."),
      ("👥","Multi-Contribuyente","Gestion de multiples personas/empresas."),
      ("🔗","Integracion","Conecta con contabilidad soberana."),
      ("🔒","Cifrado","Informacion fiscal cifrada E2E.")]),

    # ═══════ NEXUS COMERCIO (#FF6D00) ═══════
    ("rrhh-soberano", "RRHH Soberano", "👔", "Recursos Humanos y Talento", "#FF6D00",
     "Gestion de recursos humanos con reclutamiento, evaluaciones, capacitacion y bienestar del empleado. Reemplazo soberano de BambooHR, Workday y SAP SuccessFactors.",
     [("👔","Expedientes","Expedientes digitales de empleados."),
      ("📋","Reclutamiento","Pipeline de contratacion con AI."),
      ("📊","Evaluaciones","Evaluaciones de desempeno 360."),
      ("🎓","Capacitacion","LMS integrado para entrenamiento."),
      ("📅","Vacaciones","Gestion de vacaciones y ausencias."),
      ("🤖","AI Matching","AI para matching candidato-puesto."),
      ("📱","Self-Service","Portal de empleados."),
      ("💰","Beneficios","Gestion de beneficios y seguros."),
      ("📊","Analytics","Metricas de rotacion y clima laboral."),
      ("🔒","Privacidad","Datos personales cifrados E2E.")]),

    ("soporte-soberano", "Soporte Soberano", "🎧", "Mesa de Ayuda y Soporte", "#FF6D00",
     "Sistema de soporte al cliente con tickets, chat en vivo, base de conocimiento y AI chatbot. Reemplazo soberano de Zendesk, Freshdesk e Intercom.",
     [("🎧","Tickets","Sistema de tickets con prioridades y SLA."),
      ("💬","Chat en Vivo","Chat en tiempo real con clientes."),
      ("🤖","AI Chatbot","Chatbot que resuelve consultas comunes."),
      ("📚","Knowledge Base","Base de conocimiento para autoservicio."),
      ("📧","Email","Gestion de correos de soporte."),
      ("📊","Analytics","Metricas de satisfaccion y resolucion."),
      ("👥","Equipos","Asignacion automatica por departamento."),
      ("📱","Omnicanal","Soporte por chat, email, telefono y redes."),
      ("🔗","Integraciones","Conecta con CRM y comercio soberano."),
      ("💾","Offline","Cola de tickets sin conexion.")]),

    ("inventario-soberano", "Inventario Soberano", "📦", "Gestion de Inventario y Almacen", "#FF6D00",
     "Sistema de gestion de inventario con control de stock, almacenes, codigos de barras y prediccion AI. Reemplazo soberano de TradeGecko, Cin7 y Fishbowl.",
     [("📦","Control Stock","Stock en tiempo real multi-almacen."),
      ("📊","Dashboard","Visualizacion de inventario y alertas."),
      ("📱","Scanner","Escaneo de codigos de barras con celular."),
      ("🤖","AI Prediccion","Prediccion de demanda con AI."),
      ("🔄","Reabastecimiento","Ordenes automaticas de reposicion."),
      ("📋","Lotes","Gestion de lotes y fechas de vencimiento."),
      ("💰","Valuacion","FIFO, LIFO, promedio ponderado."),
      ("🏭","Multi-Almacen","Gestion de multiples almacenes."),
      ("🔗","Integraciones","Conecta con comercio y contabilidad."),
      ("💾","Offline","Funciona sin conexion en almacen.")]),

    ("pos-soberano", "POS Soberano", "🏪", "Punto de Venta Terminal", "#FF6D00",
     "Terminal punto de venta con cobros WAMPUM/fiat, inventario, recibos y reportes de ventas. Reemplazo soberano de Square, Shopify POS y Toast.",
     [("🏪","Terminal","Interfaz tactil para punto de venta."),
      ("💳","Pagos","Acepta WAMPUM, tarjeta, efectivo y QR."),
      ("🧾","Recibos","Recibos digitales y en papel."),
      ("📦","Inventario","Inventario sincronizado con almacen."),
      ("📊","Reportes","Ventas por hora, dia, producto y empleado."),
      ("👥","Empleados","Control de caja por empleado."),
      ("🤖","AI Recomendaciones","Sugerencias de productos al cliente."),
      ("📱","Movil","POS desde tablet o celular."),
      ("🔗","Integraciones","Conecta con contabilidad y comercio."),
      ("💾","Offline","Funciona sin internet con sync.")]),

    ("erp-soberano", "ERP Soberano", "🏢", "Planificacion de Recursos Empresariales", "#FF6D00",
     "Sistema ERP integrado con finanzas, operaciones, cadena de suministro y BI. Reemplazo soberano de SAP, Oracle ERP y Microsoft Dynamics.",
     [("🏢","Modular","Modulos independientes e integrables."),
      ("💰","Finanzas","Contabilidad, presupuesto y tesoreria."),
      ("🏭","Operaciones","Produccion, manufactura y calidad."),
      ("📦","Supply Chain","Cadena de suministro end-to-end."),
      ("📊","BI","Business intelligence con dashboards."),
      ("🤖","AI Optimizacion","AI optimiza procesos y costos."),
      ("💱","Multi-Moneda","WAMPUM, fiat y conversiones."),
      ("👥","Multi-Empresa","Gestion de grupo empresarial."),
      ("📱","Movil","Aprobaciones y reportes desde celular."),
      ("🔗","API","API abierta para integraciones custom.")]),

    # ═══════ NEXUS CONSEJO (#1565c0) ═══════
    ("notificaciones-gobierno", "Notificaciones de Gobierno", "🔔", "Sistema de Notificaciones Oficiales", "#1565c0",
     "Plataforma de notificaciones oficiales del gobierno via SMS, email y push para alertas, tramites y servicios publicos. Inspirado en GOV.UK Notify.",
     [("🔔","Multi-Canal","Notificaciones por SMS, email y push."),
      ("📋","Tramites","Alertas de estado de tramites."),
      ("🏛️","Oficial","Verificacion de autenticidad gubernamental."),
      ("📊","Analytics","Metricas de entrega y lectura."),
      ("📱","App Ciudadano","App movil para notificaciones."),
      ("🤖","AI Personalizacion","Notificaciones personalizadas por AI."),
      ("🌐","Multilingue","Notificaciones en 14 lenguas nativas."),
      ("⚡","Emergencias","Canal prioritario para emergencias."),
      ("🔒","Cifrado","Comunicaciones cifradas E2E."),
      ("💾","Offline","Cola de notificaciones offline.")]),

    ("licencias-permisos-soberano", "Licencias y Permisos", "📜", "Portal de Licencias y Permisos", "#1565c0",
     "Portal digital de tramites para licencias comerciales, permisos de construccion, licencias de conducir y certificaciones oficiales.",
     [("📜","Licencias","Solicitud y renovacion de licencias."),
      ("🏗️","Permisos","Permisos de construccion y obra."),
      ("🚗","Conducir","Licencias de conducir digitales."),
      ("💼","Comercial","Licencias comerciales y sanitarias."),
      ("📊","Seguimiento","Track de estado de solicitudes."),
      ("📱","Digital","Licencias digitales en el celular."),
      ("🤖","AI Revision","Revision automatica de requisitos."),
      ("💰","Pagos","Pago de tasas en WAMPUM o fiat."),
      ("🔗","Verificacion","Verificacion publica de licencias."),
      ("💾","Offline","Consulta offline de licencias.")]),

    # ═══════ NEXUS FORJA (#00e676) ═══════
    ("virtualizacion-soberana", "Virtualizacion Soberana", "🖥️", "Maquinas Virtuales y Contenedores", "#00e676",
     "Plataforma de virtualizacion con maquinas virtuales, contenedores y cloud computing soberano. Reemplazo soberano de VirtualBox, VMware y Docker Desktop.",
     [("🖥️","Maquinas Virtuales","VMs con soporte para multiples OS."),
      ("🐳","Contenedores","Docker y OCI containers soberanos."),
      ("☁️","Cloud","Cloud computing soberano on-premise."),
      ("🔄","Snapshots","Snapshots y restauracion instantanea."),
      ("📊","Monitoring","Monitoreo de recursos en tiempo real."),
      ("🤖","AI Scaling","Auto-scaling inteligente con AI."),
      ("🔒","Isolation","Aislamiento completo entre VMs."),
      ("🌐","Networking","Red virtual configurable."),
      ("📱","Gestion Remota","Administracion desde cualquier lugar."),
      ("💾","Backup","Backup automatico de maquinas virtuales.")]),
]

TEMPLATE = """<!DOCTYPE html><html lang="es"><head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<meta name="description" content="Plataforma soberana de {name} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{dirname}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{name} — {subtitle}">
<meta property="og:description" content="Plataforma soberana de {name} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{dirname}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{name} — {subtitle}">
<meta name="twitter:description" content="Plataforma soberana de {name} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<link rel="stylesheet" href="../shared/ierahkwa.css"><title>{name} — {subtitle}</title>
<style>:root{{--accent:{accent}}}</style>
</head><body>
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header><div class="logo"><div class="logo-icon" aria-hidden="true">{initial}</div><h1>{name}</h1></div><nav aria-label="Navegacion principal"><a href="#" aria-current="page">Dashboard</a><a href="#">Servicios</a><a href="#">Red</a></nav></header>
<main id="main">
<section class="hero"><div class="badge"><span aria-hidden="true">{emoji}</span> {subtitle}</div><h2>{hero_title}</h2><p>{description}</p></section>
<div class="stats" role="list" aria-label="Estadisticas clave">{stats_html}</div>
<div class="section-title"><h3>Modulos de la Plataforma</h3><p>10 herramientas soberanas</p></div>
<div class="grid">
{cards_html}</div>
</main>
<footer><p><span aria-hidden="true">{emoji}</span> {name} &mdash; Ecosistema Digital <a href="../">Ierahkwa</a> &mdash; 72M personas, 19 naciones soberanas</p></footer>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body></html>"""

created = 0
for dirname, name, emoji, subtitle, accent, desc, features in PLATFORMS:
    d = BASE / dirname
    d.mkdir(exist_ok=True)
    initial = name[0].upper()
    hero_title = subtitle
    # Build stats
    stats = [features[0], features[1], features[4], features[9]]
    stats_html = ""
    for e, label, _ in stats:
        short = label.split()[0] if len(label.split()) > 1 else label
        stats_html += f'<div class="stat" role="listitem"><div class="val">{e}</div><div class="lbl">{short}</div></div>'
    # Build cards
    cards_html = ""
    for fe, fl, fd in features:
        cards_html += f'<article class="card"><div class="card-icon" aria-hidden="true">{fe}</div><h4>{fl}</h4><p>{fd}</p></article>\n'
    html = TEMPLATE.format(
        dirname=dirname, name=name, emoji=emoji, subtitle=subtitle,
        accent=accent, description=desc, initial=initial,
        hero_title=hero_title, stats_html=stats_html, cards_html=cards_html
    )
    (d / "index.html").write_text(html, encoding="utf-8")
    created += 1
    print(f"  Created: {dirname}/index.html ({len(html)} bytes)")

print(f"\nTotal: {created} platforms created")
