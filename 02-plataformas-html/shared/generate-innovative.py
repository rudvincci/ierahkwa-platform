#!/usr/bin/env python3
"""
Generate:
A) Standard platforms still missing from any complete digital ecosystem
B) INNOVATIVE platforms unique to indigenous sovereignty (never built before)
"""
import os, pathlib

BASE = pathlib.Path(__file__).resolve().parent.parent

PLATFORMS = [
    # ══════════════════════════════════════════════════
    # A) STANDARD MISSING PLATFORMS
    # ══════════════════════════════════════════════════

    # NEXUS ESCRITORIO (#26C6DA) - Productivity
    ("pizarra-soberana", "Pizarra Soberana", "🖊️", "Pizarra Digital Colaborativa", "#26C6DA",
     "Pizarra digital infinita con dibujo, diagramas, sticky notes y colaboracion en tiempo real. Reemplazo soberano de Miro, Excalidraw y FigJam.",
     [("🖊️","Dibujo Libre","Dibuja con lapiz, formas y texto."),
      ("📝","Sticky Notes","Notas adhesivas para brainstorming."),
      ("👥","Colaboracion","Edicion simultanea con cursores."),
      ("🔀","Diagramas","Flowcharts, wireframes y mapas."),
      ("🎨","Templates","Plantillas para retrospectivas y planning."),
      ("🤖","AI Generador","Describe y AI genera el diagrama."),
      ("📱","Touch","Soporte tactil para tablet y pen."),
      ("📁","Exportar","Exporta a PNG, SVG y PDF."),
      ("🔗","Embebido","Incrusta en docs y wikis soberanas."),
      ("💾","Offline","Funciona sin conexion.")]),

    ("modelado-3d-soberano", "Modelado 3D Soberano", "🎲", "Diseno 3D y CAD Soberano", "#26C6DA",
     "Herramienta de modelado 3D, animacion y CAD con renderizado en tiempo real. Reemplazo soberano de Blender, FreeCAD y SketchUp.",
     [("🎲","Modelado 3D","Escultura digital y modelado poligonal."),
      ("🎬","Animacion","Keyframes, rigging y motion capture."),
      ("🏗️","CAD","Diseno tecnico y planos de ingenieria."),
      ("🌅","Renderizado","Motor de render en tiempo real."),
      ("🤖","AI Generador","Genera modelos 3D desde texto o foto."),
      ("📐","Parametrico","Diseno parametrico para manufactura."),
      ("🖨️","Impresion 3D","Exporta STL para impresion 3D."),
      ("👥","Colaboracion","Edicion 3D colaborativa en la nube."),
      ("📱","WebGL","Funciona en navegador sin instalacion."),
      ("💾","Offline","Trabaja offline con archivos locales.")]),

    ("publicacion-soberana", "Publicacion Soberana", "📰", "Autoedicion y Publicacion Digital", "#26C6DA",
     "Herramienta de autoedicion para libros, revistas, carteles y material impreso con typesetting profesional. Reemplazo soberano de Scribus, InDesign y Canva Print.",
     [("📰","Layout","Maquetacion profesional multi-pagina."),
      ("📝","Tipografia","Control tipografico avanzado y kerning."),
      ("🎨","Templates","Plantillas para libros, revistas y flyers."),
      ("📄","PDF Profesional","Exporta PDF/X para imprenta."),
      ("🖼️","Imagenes","Integracion con diseno soberano."),
      ("📚","Libros","Paginacion automatica para libros."),
      ("🤖","AI Layout","AI sugiere layouts y composiciones."),
      ("🌐","Multilingue","Soporte RTL y 14 lenguas nativas."),
      ("👥","Colaboracion","Revision y aprobacion en equipo."),
      ("💾","Offline","Edicion completa sin conexion.")]),

    # NEXUS FORJA (#00e676) - Dev/Tech
    ("monitoreo-soberano", "Monitoreo Soberano", "📡", "Monitoreo de Infraestructura", "#00e676",
     "Plataforma de monitoreo de servidores, redes, aplicaciones y contenedores con alertas inteligentes. Reemplazo soberano de Nagios, Zabbix, Prometheus y Grafana.",
     [("📡","Servidores","Monitoreo de CPU, RAM, disco y red."),
      ("📊","Dashboards","Dashboards personalizables en tiempo real."),
      ("🔔","Alertas","Alertas por email, SMS y push."),
      ("🐳","Contenedores","Monitoreo de Docker y Kubernetes."),
      ("🌐","Red","SNMP, ping, traceroute y flujo de red."),
      ("🤖","AI Anomalias","Deteccion de anomalias con AI."),
      ("📈","Metricas","Series de tiempo con retencion configurable."),
      ("🔗","Integraciones","300+ plugins de recoleccion."),
      ("📱","Movil","Alertas y dashboards desde celular."),
      ("💾","Offline","Cola de metricas sin conexion.")]),

    ("testing-soberano", "Testing Soberano", "🧪", "Testing y QA Automatizado", "#00e676",
     "Plataforma de testing automatizado para web, mobile y API con AI para generacion de tests. Reemplazo soberano de Selenium, Cypress y Jest.",
     [("🧪","Test Runner","Ejecuta tests unitarios e integracion."),
      ("🌐","Browser Testing","Tests end-to-end en multiples navegadores."),
      ("📱","Mobile Testing","Tests automatizados iOS y Android."),
      ("🔌","API Testing","Tests de API REST y GraphQL."),
      ("🤖","AI Test Gen","AI genera tests desde codigo."),
      ("📊","Reportes","Reportes de cobertura y resultados."),
      ("⚡","Performance","Load testing y stress testing."),
      ("🔄","CI/CD","Integracion con DevOps pipeline."),
      ("📷","Visual Testing","Comparacion visual pixel a pixel."),
      ("💾","Offline","Ejecuta tests locales sin server.")]),

    ("base-datos-soberana", "Base de Datos Soberana", "🗄️", "Gestion de Bases de Datos", "#00e676",
     "Administrador de bases de datos con soporte SQL, NoSQL, grafos y series de tiempo. Reemplazo soberano de phpMyAdmin, DBeaver y MongoDB Compass.",
     [("🗄️","Multi-Motor","SQL, NoSQL, grafos y time-series."),
      ("📊","Query Editor","Editor de consultas con autocompletado."),
      ("🔍","Explorador","Navega tablas, colecciones y grafos."),
      ("📈","Visualizacion","Graficos de datos en tiempo real."),
      ("🤖","AI Queries","Describe en espanol y AI genera el query."),
      ("🔄","Migracion","Migra datos entre motores."),
      ("🔒","Permisos","Control de acceso granular."),
      ("📋","Backup","Backup y restauracion automatica."),
      ("📱","Web UI","Interfaz web sin instalacion."),
      ("💾","Offline","Trabaja con bases de datos locales.")]),

    # NEXUS CEREBRO (#7c4dff) - AI/Quantum
    ("observabilidad-soberana", "Observabilidad Soberana", "🔭", "Logs, Trazas y Metricas", "#7c4dff",
     "Plataforma de observabilidad unificada con logs, trazas distribuidas y metricas para microservicios soberanos. Reemplazo soberano de ELK Stack, Datadog y Splunk.",
     [("📋","Logs","Recoleccion y busqueda de logs centralizada."),
      ("🔗","Trazas","Trazado distribuido de microservicios."),
      ("📈","Metricas","Metricas de aplicacion y negocio."),
      ("📊","Dashboards","Dashboards unificados de observabilidad."),
      ("🤖","AI Root Cause","AI identifica causa raiz de problemas."),
      ("🔔","Alertas","Alertas correlacionadas multi-signal."),
      ("🗺️","Service Map","Mapa de dependencias de servicios."),
      ("📱","Movil","Incidentes y on-call desde celular."),
      ("🔒","Compliance","Retencion y audit para compliance."),
      ("💾","Offline","Cola de telemetria sin conexion.")]),

    # NEXUS SALUD (#FF5722)
    ("fitness-soberano", "Fitness Soberano", "💪", "Salud Fisica y Bienestar", "#FF5722",
     "App de fitness con rutinas de ejercicio, nutricion, meditacion y seguimiento de actividad fisica. Integra practicas ancestrales de bienestar.",
     [("💪","Rutinas","Planes de ejercicio personalizados."),
      ("🧘","Meditacion","Meditacion guiada con practicas ancestrales."),
      ("🥗","Nutricion","Plan nutricional con alimentos locales."),
      ("📊","Tracking","Seguimiento de peso, pasos y sueno."),
      ("🤖","AI Coach","Entrenador virtual con AI."),
      ("🌿","Medicina Natural","Remedios y practicas tradicionales."),
      ("👥","Comunidad","Retos grupales y comunidad."),
      ("📱","Wearables","Conecta con relojes y sensores."),
      ("🏃","Actividades","Correr, nadar, ciclismo y mas."),
      ("💾","Offline","Rutinas disponibles sin conexion.")]),

    # NEXUS ACADEMIA (#9C27B0)
    ("idiomas-soberano", "Idiomas Soberano", "🗣️", "Aprendizaje de Lenguas Indigenas", "#9C27B0",
     "App de aprendizaje de lenguas indigenas con gamificacion, AI tutor y reconocimiento de voz. Reemplazo soberano de Duolingo para las 14 lenguas nativas.",
     [("🗣️","14 Lenguas","Cursos para 14 lenguas indigenas."),
      ("🎮","Gamificacion","Niveles, puntos y rachas diarias."),
      ("🤖","AI Tutor","Tutor virtual que corrige pronunciacion."),
      ("🎤","Voz","Reconocimiento de voz para practica oral."),
      ("📚","Lecciones","Lecciones progresivas con historias."),
      ("👥","Comunidad","Practica con hablantes nativos."),
      ("🏆","Certificados","Certificacion de nivel de lengua."),
      ("📱","Movil","App movil con notificaciones diarias."),
      ("🌐","Multilingue","Aprende desde espanol, ingles o portugues."),
      ("💾","Offline","Lecciones descargables offline.")]),

    ("lector-soberano", "Lector Soberano", "📖", "Lector de eBooks y Documentos", "#9C27B0",
     "Lector de libros electronicos con soporte EPUB, PDF, MOBI y audiolibros. Integrado con Biblioteca Soberana.",
     [("📖","Multi-Formato","Lee EPUB, PDF, MOBI y CBZ."),
      ("🎧","Audiolibros","Escucha libros con narrador AI."),
      ("📝","Anotaciones","Resalta, anota y marca paginas."),
      ("🔍","Diccionario","Diccionario integrado en 14 lenguas."),
      ("📊","Progreso","Tracking de lectura y estadisticas."),
      ("🤖","AI Resumen","Resume capitulos con AI."),
      ("🌙","Modo Nocturno","Lectura nocturna sin fatiga visual."),
      ("📱","Multi-Dispositivo","Sync de progreso entre dispositivos."),
      ("📚","Biblioteca","Integrado con Biblioteca Soberana."),
      ("💾","Offline","Descarga libros para leer offline.")]),

    # ══════════════════════════════════════════════════
    # B) PLATAFORMAS INNOVADORAS (NUNCA CONSTRUIDAS)
    # ══════════════════════════════════════════════════

    # NEXUS RAICES (#00FF41) - Culture/Identity
    ("ceremonia-virtual-soberana", "Ceremonia Virtual Soberana", "🪶", "Espacio Ceremonial Digital", "#00FF41",
     "Espacio digital sagrado para ceremonias, rituales y reuniones espirituales con realidad aumentada, transmision privada y protocolos culturales. PRIMERA plataforma de su tipo en el mundo.",
     [("🪶","Espacio Sagrado","Entorno virtual inmersivo para ceremonias."),
      ("🎥","Transmision Privada","Stream cifrado E2E solo para invitados."),
      ("🌅","Ambientes AR","Realidad aumentada con paisajes ancestrales."),
      ("📿","Protocolos","Protocolos culturales configurables por nacion."),
      ("🔒","Privacidad Total","Zero grabacion, zero screenshots."),
      ("🌐","Multi-Nacion","Conecta ceremonias entre 19 naciones."),
      ("🎵","Sonido Espacial","Audio 3D inmersivo para cantos."),
      ("📅","Calendario Lunar","Calendario ceremonial con ciclos lunares."),
      ("👥","Ancianos","Rol especial para ancianos y lideres."),
      ("💾","Offline","Modo offline para areas remotas.")]),

    ("adn-ancestral-soberano", "ADN Ancestral Soberano", "🧬", "Analisis Genomico Soberano", "#00FF41",
     "Plataforma de analisis genomico y ancestria controlada por naciones indigenas. Datos geneticos NUNCA compartidos con corporaciones. PRIMERA plataforma de genomica indigena soberana.",
     [("🧬","Analisis ADN","Analisis genomico con laboratorios propios."),
      ("🌳","Arbol Ancestral","Reconstruccion de linajes ancestrales."),
      ("🗺️","Migracion","Mapa de migraciones ancestrales de pueblos."),
      ("🔒","Soberania Datos","Datos geneticos propiedad de la nacion."),
      ("🏥","Salud Genetica","Predisposiciones y medicina personalizada."),
      ("🤖","AI Ancestria","AI para analisis de haplogrupos."),
      ("👥","Consentimiento","Consentimiento comunitario, no individual."),
      ("📚","Bio-Archivo","Archivo biologico para futuras generaciones."),
      ("🌿","Etnobotanica","Relacion genetica con plantas medicinales."),
      ("🔬","Investigacion","Investigacion controlada por la comunidad.")]),

    ("semilla-soberana", "Semilla Soberana", "🌱", "Banco de Semillas y Biodiversidad", "#00FF41",
     "Banco digital de semillas, biodiversidad y conocimiento agricola ancestral con intercambio P2P entre naciones. PRIMERA plataforma de soberania alimentaria digital.",
     [("🌱","Catalogo","Catalogo de 50K+ variedades de semillas nativas."),
      ("🔄","Intercambio P2P","Intercambio de semillas entre comunidades."),
      ("📊","Biodiversidad","Indice de biodiversidad por territorio."),
      ("📅","Calendario","Calendario de siembra por zona climatica."),
      ("🤖","AI Cultivo","AI predice mejor epoca y condiciones."),
      ("📷","Identificacion","Identificacion de plantas con camara."),
      ("🧬","Genetica","Banco genetico de variedades en peligro."),
      ("🌿","Conocimiento","Sabiduria agricola ancestral documentada."),
      ("🗺️","Mapas","Mapas de distribucion de especies."),
      ("💾","Offline","Catalogo disponible sin conexion.")]),

    # NEXUS TIERRA (#43a047) - Nature
    ("oraculo-climatico-soberano", "Oraculo Climatico", "🌦️", "Prediccion Climatica Ancestral + AI", "#43a047",
     "Sistema de prediccion climatica que combina sabiduria ancestral indigena con modelos AI y datos satelitales. PRIMERA plataforma que integra conocimiento ancestral con ciencia moderna.",
     [("🌦️","Prediccion Hibrida","Ancestral + AI + satelite combinados."),
      ("🌿","Bioindicadores","Senales naturales de cambio climatico."),
      ("📡","Datos Satelite","Imagenes satelitales en tiempo real."),
      ("🤖","AI Modelos","Modelos AI entrenados con datos locales."),
      ("📊","Dashboard","Visualizacion climatica por territorio."),
      ("🔔","Alertas","Alertas tempranas para agricultores."),
      ("📅","Calendario","Calendario agricola AI + ancestral."),
      ("🗺️","Mapas","Mapas de riesgo climatico por zona."),
      ("👥","Ancianos","Validacion de ancianos en predicciones."),
      ("💾","Offline","Predicciones locales sin internet.")]),

    ("territorio-vigilante-soberano", "Territorio Vigilante", "🛰️", "Vigilancia Territorial Soberana", "#43a047",
     "Sistema de monitoreo territorial con satelites, drones, sensores IoT y reportes comunitarios para proteger tierras indigenas. PRIMERA plataforma de vigilancia territorial comunitaria.",
     [("🛰️","Satelites","Imagenes satelitales de territorios."),
      ("🤖","AI Deteccion","AI detecta deforestacion y mineria ilegal."),
      ("📱","Reportes","App para reportes de intrusiones."),
      ("🗺️","Mapas","Mapas en tiempo real de territorios."),
      ("🔔","Alertas","Alertas inmediatas de amenazas."),
      ("📊","Dashboard","Dashboard de estado territorial."),
      ("📸","Drones","Monitoreo con drones autonomos."),
      ("🌿","Biodiversidad","Seguimiento de especies en territorio."),
      ("⚖️","Legal","Evidencia legal para disputas de tierra."),
      ("💾","Offline","Funciona en areas sin internet.")]),

    # NEXUS AMPARO (#607D8B) - Social Protection
    ("intercambio-comunitario-soberano", "Intercambio Comunitario", "🤝", "Banco de Tiempo y Trueque Digital", "#607D8B",
     "Sistema de economia comunitaria con banco de tiempo, trueque digital y moneda social para comunidades donde el dinero es escaso. PRIMERA plataforma de economia solidaria indigena.",
     [("🤝","Banco de Tiempo","Intercambia horas de trabajo."),
      ("🔄","Trueque Digital","Intercambia bienes sin dinero."),
      ("💰","Moneda Social","Moneda comunitaria local digital."),
      ("📊","Reputacion","Sistema de reputacion comunitaria."),
      ("📱","App Movil","Transacciones desde el celular."),
      ("🗺️","Local","Directorio de servicios por comunidad."),
      ("🤖","AI Matching","AI conecta ofertas con necesidades."),
      ("👥","Comunidades","Red de comunidades interconectadas."),
      ("📋","Habilidades","Catalogo de habilidades locales."),
      ("💾","Offline","Funciona sin internet con sync.")]),

    # NEXUS CONSEJO (#1565c0) - Government
    ("justicia-restaurativa-soberana", "Justicia Restaurativa", "⚖️", "Justicia Restaurativa con AI", "#1565c0",
     "Plataforma de justicia restaurativa basada en tradiciones indigenas de resolucion de conflictos con asistencia AI. PRIMERA plataforma de justicia indigena digital.",
     [("⚖️","Mediacion","Proceso de mediacion digital guiado."),
      ("🤖","AI Facilitador","AI facilita dialogo entre partes."),
      ("🪶","Tradiciones","Protocolos de justicia por nacion."),
      ("👥","Consejo Ancianos","Participacion de ancianos como guias."),
      ("📋","Casos","Gestion de casos y seguimiento."),
      ("🔒","Confidencial","Sesiones privadas y cifradas."),
      ("📊","Resolucion","Metricas de resolucion y reincidencia."),
      ("🌐","Multi-Nacion","Adaptable a 574 tradiciones tribales."),
      ("📱","Acceso","Participacion remota desde cualquier lugar."),
      ("💾","Offline","Documentacion offline disponible.")]),

    # NEXUS VOCES (#e040fb) - Social/Media
    ("galeria-soberana", "Galeria Soberana", "📸", "Galeria de Fotos y Arte Digital", "#e040fb",
     "Galeria de fotos y arte digital con almacenamiento ilimitado, albums compartidos y proteccion de derechos. Reemplazo soberano de Google Photos, Flickr e Instagram.",
     [("📸","Almacenamiento","Fotos y videos en calidad original."),
      ("📂","Albums","Albums compartidos con familia y comunidad."),
      ("🤖","AI Organizacion","AI organiza por rostros, lugares y fechas."),
      ("🎨","Arte Digital","Galeria de arte indigena digital."),
      ("🔒","Derechos","Proteccion automatica de derechos de autor."),
      ("📱","Auto-Backup","Backup automatico desde celular."),
      ("🖼️","Edicion","Edicion basica de fotos integrada."),
      ("👥","Compartir","Comparte con permisos granulares."),
      ("📊","Recuerdos","Recuerdos automaticos por fecha."),
      ("💾","Offline","Galeria accesible sin conexion.")]),
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
    stats = [features[0], features[1], features[4], features[9]]
    stats_html = ""
    for e, label, _ in stats:
        short = label.split()[0] if len(label.split()) > 1 else label
        stats_html += f'<div class="stat" role="listitem"><div class="val">{e}</div><div class="lbl">{short}</div></div>'
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

print(f"\nTotal: {created} new platforms created")
print("\nBreakdown:")
print("  ESCRITORIO: pizarra, modelado-3d, publicacion (+3)")
print("  FORJA: monitoreo, testing, base-datos (+3)")
print("  CEREBRO: observabilidad (+1)")
print("  SALUD: fitness (+1)")
print("  ACADEMIA: idiomas, lector (+2)")
print("  RAICES: ceremonia-virtual, adn-ancestral, semilla (+3)")
print("  TIERRA: oraculo-climatico, territorio-vigilante (+2)")
print("  AMPARO: intercambio-comunitario (+1)")
print("  CONSEJO: justicia-restaurativa (+1)")
print("  VOCES: galeria (+1)")
print("  = 18 new platforms total")
