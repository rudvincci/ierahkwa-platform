#!/usr/bin/env python3
"""
create-space-global.py
Generates 8 platform directories with Pattern B index.html files.
Interstellar/alien communication & global communication completeness.
"""

import os

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
# BASE = /Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html

PLATFORMS = [
    {
        "dir": "seti-soberano",
        "title": "SETI Soberano",
        "subtitle": "Búsqueda de Inteligencia Extraterrestre Soberana",
        "icon": "👽",
        "accent": "#7c4dff",
        "description": "Programa soberano de búsqueda de inteligencia extraterrestre que reemplaza SETI@home y Breakthrough Listen. Red de 847 radio telescopios distribuidos en 19 naciones, análisis de señales con IA cuántica, detección de patrones anómalos en frecuencias de 1-10 GHz, base de datos de candidatos de señales, procesamiento distribuido entre comunidades indígenas.",
        "metrics": [
            ("847", "Telescopios"),
            ("1-10", "GHz"),
            ("IA", "Cuántica"),
            ("19", "Naciones"),
            ("24/7", "Scanning"),
            ("P2P", "Processing"),
        ],
        "arch_layers": [
            ("#7c4dff", "RADIO TELESCOPIOS (847)", "Antenas · Receptores · Filtros · Amplificadores", "Distribución · 19 Naciones · Calibración · Sync"),
            ("#00bcd4", "PROCESAMIENTO SEÑALES", "FFT · Demodulación · Filtrado · Bandpass", "DSP · GPU · Pipeline · Streaming"),
            ("#ffd600", "IA CUÁNTICA ANÁLISIS", "Detección · Patrones · Clasificación · Anomalías", "ML · Deep Learning · Quantum · Neural"),
            ("#7c4dff", "ALERTA + BASE DE DATOS", "Candidatos · Verificación · Archivo · Notificación", "Ranking · Prioridad · Histórico · Citizen Science"),
        ],
        "cards": [
            ("📡", "Red de 847 Radio Telescopios", "Red distribuida de 847 radio telescopios en 19 naciones indígenas para monitoreo continuo del cielo en búsqueda de señales extraterrestres."),
            ("🧠", "Análisis de Señales con IA Cuántica", "Inteligencia artificial cuántica para procesar millones de señales por segundo, identificando patrones anómalos que escapan al análisis clásico."),
            ("🔍", "Detección de Patrones Anómalos", "Algoritmos especializados en detectar patrones no-naturales en el ruido cósmico: repeticiones, modulación, estructuras matemáticas."),
            ("📻", "Frecuencia Mágica del Hidrógeno (1420 MHz)", "Monitoreo dedicado de la línea de 21cm del hidrógeno (1420.405 MHz), la frecuencia universal considerada ideal para comunicación interestelar."),
            ("🖥️", "Procesamiento Distribuido P2P", "Red de procesamiento distribuido entre comunidades indígenas. Cada nodo contribuye capacidad de cómputo para analizar datos de radio telescopios."),
            ("📊", "Base de Datos de Candidatos", "Base de datos soberana de señales candidatas con ranking de probabilidad, metadata completa, y verificación multi-telescopio."),
            ("🌊", "Waterfall Plot en Tiempo Real", "Visualización waterfall en tiempo real de todo el espectro monitoreado, con resaltado automático de señales de interés."),
            ("🔗", "Correlación Multi-Telescopio", "Correlación simultánea de señales entre múltiples telescopios para confirmar origen extraterrestre y descartar interferencia local."),
            ("🚨", "Alertas de Señales Anómalas", "Sistema de alertas en tiempo real cuando se detecta una señal que cumple criterios de posible origen artificial o inteligente."),
            ("👥", "Citizen Science Indígena", "Programa de ciencia ciudadana que permite a comunidades indígenas participar en la clasificación y análisis de señales candidatas."),
        ],
        "apis": [
            ("GET", "/api/v1/seti/signals", "Listar señales detectadas. Filtros: frecuencia, potencia, fecha, estado."),
            ("POST", "/api/v1/seti/analyze", "Analizar señal con IA cuántica. Params: signal_id, depth, algorithm."),
            ("GET", "/api/v1/seti/candidates", "Candidatos clasificados por probabilidad. Filtros: score, verificado."),
            ("GET", "/api/v1/seti/telescopes", "Estado de los 847 telescopios: online, calibración, datos."),
            ("POST", "/api/v1/seti/alert", "Crear alerta manual de señal anómala. Params: signal_data, priority."),
            ("GET", "/api/v1/seti/waterfall", "Datos de waterfall plot para rango de frecuencia y tiempo."),
        ],
        "db_name": "ierahkwa-seti-soberano",
        "db_stores": ["seti-signals", "candidate-events", "telescope-data"],
    },
    {
        "dir": "deep-space-soberano",
        "title": "Deep Space Soberano",
        "subtitle": "Red de Espacio Profundo para Comunicación Interplanetaria",
        "icon": "🛸",
        "accent": "#00bcd4",
        "description": "Red de espacio profundo soberana que reemplaza NASA DSN y ESA ESTRACK. 19 antenas de 34m distribuidas globalmente para comunicación con sondas, satélites y futuras misiones interplanetarias. Enlace láser óptico, corrección de errores cuántica, delay-tolerant networking para Marte+, tracking de objetos en órbita profunda.",
        "metrics": [
            ("19", "Antenas 34m"),
            ("Láser", "Óptico"),
            ("DTN", "Protocol"),
            ("Mars", "Ready"),
            ("Track", "Orbital"),
            ("QEC", "Quantum"),
        ],
        "arch_layers": [
            ("#00bcd4", "MISIÓN ESPACIAL", "Sondas · Satélites · Rovers · Estaciones", "Interplanetario · Orbital · Lunar · Deep Space"),
            ("#7c4dff", "ANTENAS 34m (19 NACIONES)", "Parabólicas · Láser · Feed · LNA", "Distribución Global · Cobertura 24/7 · Redundancia"),
            ("#ffd600", "PROCESAMIENTO SEÑAL", "Demodulación · Error Correction · Doppler", "Telemetría · Ranging · Delta-DOR · VLBI"),
            ("#00bcd4", "CENTRO DE CONTROL", "Comando · Monitoreo · Planning · Archivo", "Mission Control · Scheduling · Data Distribution"),
        ],
        "cards": [
            ("📡", "19 Antenas de 34 Metros", "Red global de 19 antenas parabólicas de 34 metros de diámetro, una por cada nación soberana, garantizando cobertura 24/7 del espacio profundo."),
            ("💡", "Enlace Láser Óptico", "Comunicación óptica por láser para tasas de datos 10-100x superiores a radiofrecuencia. Ideal para transmisión de video desde Marte y más allá."),
            ("🌐", "Delay-Tolerant Networking", "Protocolo DTN diseñado para las largas latencias del espacio profundo. Store-and-forward, bundle protocol, contactos intermitentes."),
            ("⚛️", "Corrección de Errores Cuántica", "Códigos de corrección de errores cuánticos para comunicación ultra-confiable a distancias interplanetarias con mínima pérdida de datos."),
            ("🔴", "Comunicación Mars-Ready", "Infraestructura lista para comunicación con Marte: latencia 4-24 min, relay orbital, ventanas de comunicación, almacenamiento en tránsito."),
            ("🔭", "Tracking de Objetos Profundos", "Seguimiento preciso de objetos en espacio profundo: sondas, asteroides, cometas. Precisión de nanorradianes con VLBI."),
            ("📈", "Doppler Navigation", "Navegación por efecto Doppler para determinación precisa de velocidad y trayectoria de sondas en espacio profundo."),
            ("📊", "Telemetría de Alta Precisión", "Recepción y decodificación de telemetría científica y de ingeniería con bit error rate < 10⁻⁶ a distancias de AU."),
            ("🎮", "Comando y Control Remoto", "Envío de comandos a sondas y rovers con verificación, time-tagged commands, secuencias autónomas y contingencias."),
            ("💾", "Archivo de Datos Espaciales", "Archivo permanente soberano de todos los datos científicos recibidos desde el espacio profundo. Acceso abierto para investigadores."),
        ],
        "apis": [
            ("POST", "/api/v1/deepspace/transmit", "Transmitir datos al espacio profundo. Params: target, data, priority, schedule."),
            ("GET", "/api/v1/deepspace/receive", "Recibir datos de misiones activas. Filtros: mission, type, date_range."),
            ("GET", "/api/v1/deepspace/tracking", "Estado de tracking de objetos. Params: object_id, ephemeris."),
            ("POST", "/api/v1/deepspace/command", "Enviar comando a misión. Params: mission_id, command, verification."),
            ("GET", "/api/v1/deepspace/telemetry", "Telemetría en tiempo real. Filtros: mission, subsystem, timespan."),
            ("GET", "/api/v1/deepspace/antennas", "Estado de las 19 antenas: azimut, elevación, target, SNR."),
        ],
        "db_name": "ierahkwa-deep-space-soberano",
        "db_stores": ["deepspace-telemetry", "tracking-objects", "command-queue"],
    },
    {
        "dir": "senales-cosmicas-soberano",
        "title": "Señales Cósmicas Soberano",
        "subtitle": "Procesamiento Avanzado de Señales del Cosmos",
        "icon": "📡",
        "accent": "#7c4dff",
        "description": "Plataforma de procesamiento de señales cósmicas que complementa SETI y Deep Space. FFT cuántica para análisis espectral, machine learning para clasificación de señales (pulsar, quasar, FRB, artificial), demodulación multi-esquema, filtrado de interferencia terrestre, correlación entre observatorios, archivo permanente de señales.",
        "metrics": [
            ("FFT", "Cuántica"),
            ("ML", "Clasificación"),
            ("Multi", "Esquema"),
            ("847", "Fuentes"),
            ("Archivo", "Permanente"),
            ("Real", "Time"),
        ],
        "arch_layers": [
            ("#7c4dff", "RECEPTORES (847)", "Radio · Óptico · Infrarrojo · Rayos X", "Antenas · Detectores · Filtros · Amplificadores"),
            ("#00bcd4", "FFT CUÁNTICA", "Transformada · Espectral · Resolución · Windowing", "GPU · QPU · Pipeline · Streaming · Real-Time"),
            ("#ffd600", "ML CLASIFICACIÓN", "Pulsar · Quasar · FRB · Artificial · Ruido", "Deep Learning · CNN · RNN · Transformer · Ensemble"),
            ("#7c4dff", "CATÁLOGO + ARCHIVO PERMANENTE", "Indexación · Metadata · Búsqueda · Acceso", "Soberano · Replicado · Inmutable · Abierto"),
        ],
        "cards": [
            ("📊", "FFT Cuántica Análisis Espectral", "Transformada rápida de Fourier ejecutada en procesadores cuánticos para análisis espectral de ultra-alta resolución en tiempo real."),
            ("🤖", "ML Clasificación de Señales", "Machine learning avanzado para clasificar automáticamente señales: pulsares, quasares, Fast Radio Bursts, interferencia, y posibles señales artificiales."),
            ("⚡", "Detección de Fast Radio Bursts", "Detección en tiempo real de Fast Radio Bursts (FRB), misteriosas ráfagas de radio de milisegundos de origen extragaláctico."),
            ("🚫", "Filtrado de Interferencia Terrestre", "Algoritmos avanzados para identificar y filtrar interferencia de origen terrestre: satélites, radares, WiFi, celular, microondas."),
            ("📻", "Demodulación Multi-Esquema", "Demodulación simultánea en múltiples esquemas: AM, FM, PSK, QAM, OFDM, buscando cualquier tipo de modulación artificial."),
            ("🔗", "Correlación Inter-Observatorio", "Correlación cruzada de datos entre observatorios distribuidos para validación y localización precisa de fuentes de señales."),
            ("🌈", "Spectrograma 3D en Tiempo Real", "Visualización 3D interactiva del espectro electromagnético: frecuencia, tiempo e intensidad con zoom y navegación temporal."),
            ("📚", "Catálogo de Fuentes Cósmicas", "Catálogo soberano de fuentes cósmicas conocidas con posición, tipo, frecuencia, variabilidad y referencias cruzadas."),
            ("💾", "Archivo Permanente de Señales", "Archivo permanente e inmutable de todas las señales grabadas, con indexación, búsqueda y acceso abierto para investigación."),
            ("🖥️", "Pipeline de Procesamiento GPU", "Pipeline de procesamiento masivamente paralelo en GPU para análisis en tiempo real de datos de 847 receptores simultáneos."),
        ],
        "apis": [
            ("POST", "/api/v1/signals/analyze", "Analizar señal cósmica. Params: signal_data, fft_resolution, classify."),
            ("GET", "/api/v1/signals/spectrum", "Espectro en tiempo real. Filtros: freq_range, resolution, source."),
            ("GET", "/api/v1/signals/catalog", "Catálogo de fuentes cósmicas. Filtros: type, position, magnitude."),
            ("POST", "/api/v1/signals/classify", "Clasificar señal con ML. Params: signal_id, models, confidence."),
            ("GET", "/api/v1/signals/frb", "Fast Radio Bursts detectados. Filtros: date, dm, fluence."),
            ("GET", "/api/v1/signals/archive", "Archivo de señales grabadas. Filtros: date, freq, source, type."),
        ],
        "db_name": "ierahkwa-senales-cosmicas-soberano",
        "db_stores": ["signal-recordings", "spectral-data", "classification-results"],
    },
    {
        "dir": "protocolo-interestelar-soberano",
        "title": "Protocolo Interestelar Soberano",
        "subtitle": "Protocolo Universal de Comunicación con Civilizaciones",
        "icon": "🌌",
        "accent": "#7c4dff",
        "description": "Protocolo de comunicación interestelar soberano inspirado en Lincos y METI. Codificación matemática universal basada en constantes fundamentales (pi, e, primos), modulación multi-banda redundante, corrección de errores para distancias de años luz, mensaje pictográfico digital, música como lenguaje universal, incluye cultura de 574 naciones tribales.",
        "metrics": [
            ("Lincos", "Based"),
            ("Math", "Universal"),
            ("Multi", "Band"),
            ("Light", "Year Range"),
            ("Picto", "Graphic"),
            ("574", "Culturas"),
        ],
        "arch_layers": [
            ("#7c4dff", "MENSAJE CULTURAL", "574 Naciones · Música · Pictogramas · Idiomas", "Arte · Ceremonias · Historia · Conocimiento"),
            ("#ffd600", "CODIFICACIÓN MATEMÁTICA", "Primos · Pi · e · Fibonacci · Constantes", "Binario · Base Universal · Redundancia · Checksums"),
            ("#00bcd4", "MODULACIÓN MULTI-BANDA", "Radio · Láser · Neutrinos · Gravitacional", "Redundante · Interleaved · Error Correction · FEC"),
            ("#7c4dff", "TRANSMISIÓN INTERESTELAR", "Antenas · Potencia · Dirección · Timing", "Beacon · Burst · Continuo · Scheduled"),
        ],
        "cards": [
            ("🔢", "Codificación Matemática Universal", "Sistema de codificación basado en matemáticas universales: números primos, operaciones básicas, constantes fundamentales comprensibles por cualquier inteligencia."),
            ("π", "Constantes Fundamentales (Pi, Primos)", "Uso de pi, e, phi, primos y Fibonacci como lenguaje base. Secuencias que cualquier civilización tecnológica reconocería como artificiales."),
            ("📻", "Modulación Multi-Banda Redundante", "Transmisión simultánea en múltiples bandas del espectro electromagnético con redundancia para sobrevivir interferencia y degradación interestelar."),
            ("✅", "Corrección de Errores Interestelar", "Códigos de corrección de errores diseñados para distancias de años luz: Reed-Solomon extendido, turbo codes, LDPC con redundancia extrema."),
            ("🎨", "Mensaje Pictográfico Digital", "Mensajes pictográficos codificados digitalmente: representaciones de anatomía, sistema solar, ADN, tabla periódica, civilización."),
            ("🎵", "Música como Lenguaje Universal", "Codificación de música de las 574 naciones tribales como forma de comunicación universal, con estructura matemática inherente."),
            ("🌍", "Representación de 574 Culturas", "Inclusión de elementos culturales de las 574 naciones tribales: idiomas, arte, ceremonias, conocimiento ancestral, cosmovisiones."),
            ("📡", "Beacon de Presencia Continua", "Señal beacon continua que anuncia la presencia de civilización, transmitida permanentemente con secuencia de primos como identificador."),
            ("🔓", "Decodificador de Respuestas", "Sistema preparado para decodificar posibles respuestas extraterrestres: análisis de patrones, correlación matemática, traducción tentativa."),
            ("🎮", "Simulador de Comunicación", "Simulador para probar protocolos de comunicación interestelar: latencia, degradación de señal, ruido, comprensibilidad del mensaje."),
        ],
        "apis": [
            ("POST", "/api/v1/interstellar/encode", "Codificar mensaje interestelar. Params: content, format, redundancy."),
            ("POST", "/api/v1/interstellar/decode", "Decodificar señal recibida. Params: signal_data, algorithms, depth."),
            ("GET", "/api/v1/interstellar/message", "Mensajes codificados disponibles. Filtros: type, culture, status."),
            ("POST", "/api/v1/interstellar/transmit", "Transmitir mensaje. Params: message_id, direction, power, bands."),
            ("GET", "/api/v1/interstellar/beacon", "Estado del beacon de presencia. Info: power, direction, uptime."),
            ("POST", "/api/v1/interstellar/simulate", "Simular comunicación. Params: distance, noise, protocol, message."),
        ],
        "db_name": "ierahkwa-protocolo-interestelar-soberano",
        "db_stores": ["interstellar-messages", "beacon-config", "decode-attempts"],
    },
    {
        "dir": "metaverso-soberano",
        "title": "Metaverso Soberano",
        "subtitle": "Mundo Virtual 3D Descentralizado para 72M Personas",
        "icon": "🌐",
        "accent": "#E91E63",
        "description": "Metaverso soberano que reemplaza Meta Horizon, Decentraland y The Sandbox. Mundo virtual 3D persistente renderizado con WebGPU, avatares personalizables con identidad soberana, territorios virtuales de las 19 naciones, economía WAMPUM integrada, educación inmersiva, ceremonias culturales 3D, reuniones de gobierno, P2P sin servidores centrales.",
        "metrics": [
            ("WebGPU", "3D"),
            ("P2P", "Desc."),
            ("19", "Territorios"),
            ("WAMPUM", "Economy"),
            ("Avatar", "Soberano"),
            ("Multi", "Player"),
        ],
        "arch_layers": [
            ("#E91E63", "USUARIO", "Registro · Login · Identidad · Perfil", "Browser · VR Headset · Mobile · Desktop"),
            ("#7c4dff", "AVATAR + IDENTIDAD", "Personalización · Soberanía · Wallet · Social", "3D Model · Animaciones · Expresiones · Vestimenta"),
            ("#00bcd4", "MUNDO 3D (WebGPU P2P)", "Renderizado · Física · Terreno · Edificios", "P2P Sync · Sharding · LOD · Streaming"),
            ("#ffd600", "ECONOMÍA WAMPUM", "Transacciones · Marketplace · NFT · Staking", "DeFi · Trading · Propiedad · Gobernanza"),
        ],
        "cards": [
            ("🌍", "Mundo 3D Persistente WebGPU", "Mundo virtual 3D persistente renderizado con WebGPU nativo. Terrenos, edificios, naturaleza y cielo con iluminación global en tiempo real."),
            ("👤", "Avatares con Identidad Soberana", "Avatares 3D personalizables vinculados a identidad soberana. Expresiones faciales, movimiento corporal, vestimenta cultural de 574 naciones."),
            ("🗺️", "19 Territorios Virtuales Nacionales", "Representación virtual de los 19 territorios nacionales con geografía, arquitectura y cultura propia. Fronteras, embajadas y zonas comunes."),
            ("💰", "Economía WAMPUM Integrada", "Economía virtual basada en WAMPUM: compra, venta, intercambio de bienes virtuales, terrenos, servicios y experiencias."),
            ("📚", "Educación Inmersiva 3D", "Aulas virtuales, laboratorios, museos y bibliotecas 3D para educación inmersiva. Aprendizaje experiencial y colaborativo."),
            ("🪶", "Ceremonias Culturales Virtuales", "Espacios sagrados virtuales para ceremonias culturales de las 574 naciones tribales. Música, danza, rituales y celebraciones."),
            ("🏛️", "Reuniones de Gobierno 3D", "Salas de gobierno virtuales para reuniones, votaciones, debates y ceremonias oficiales de las 19 naciones soberanas."),
            ("🎨", "Marketplace de Objetos NFT", "Marketplace descentralizado de objetos virtuales, vestimenta, arte y arquitectura como NFTs en la blockchain soberana."),
            ("🔨", "Construcción Colaborativa", "Herramientas de construcción 3D colaborativa para crear edificios, paisajes y experiencias en el metaverso soberano."),
            ("👥", "Social Spaces Comunitarios", "Espacios sociales para comunidades: plazas, parques, centros culturales, cafés y lugares de encuentro virtual."),
        ],
        "apis": [
            ("POST", "/api/v1/metaverse/join", "Unirse al metaverso. Params: avatar_id, territory, spawn_point."),
            ("GET", "/api/v1/metaverse/world", "Estado del mundo: territorios, usuarios, eventos, clima virtual."),
            ("POST", "/api/v1/metaverse/avatar", "Crear/actualizar avatar. Params: model, identity, customization."),
            ("GET", "/api/v1/metaverse/territories", "Territorios disponibles. Info: propietarios, uso, eventos, reglas."),
            ("POST", "/api/v1/metaverse/build", "Construir en territorio. Params: position, model, materials."),
            ("GET", "/api/v1/metaverse/events", "Eventos programados. Filtros: type, territory, date, capacity."),
        ],
        "db_name": "ierahkwa-metaverso-soberano",
        "db_stores": ["metaverse-world", "avatar-data", "territory-ownership"],
    },
    {
        "dir": "realidad-aumentada-soberana",
        "title": "Realidad Aumentada Soberana",
        "subtitle": "Plataforma AR/VR/XR para Experiencias Inmersivas",
        "icon": "🥽",
        "accent": "#E91E63",
        "description": "Plataforma AR/VR/XR soberana que reemplaza Apple Vision Pro SDK, Meta Quest SDK y Google ARCore. Framework para crear experiencias de realidad aumentada y virtual sobre territorios indígenas, reconstrucción 3D de sitios ancestrales, guías turísticas AR, educación inmersiva de cultura, telemedicina VR, entrenamiento militar XR.",
        "metrics": [
            ("AR/VR", "XR"),
            ("WebXR", "Nativo"),
            ("Sitios", "Ancestrales"),
            ("Edu", "VR"),
            ("Tele", "Medicina"),
            ("Militar", "XR"),
        ],
        "arch_layers": [
            ("#E91E63", "DISPOSITIVO XR", "Headset · Móvil · Gafas · Háptico", "Quest · Vision · Mobile AR · Desktop"),
            ("#7c4dff", "WebXR ENGINE", "Rendering · Shaders · Lighting · Particles", "Scene Graph · ECS · Physics · Audio 3D"),
            ("#00bcd4", "SLAM + TRACKING", "Planos · Objetos · Manos · Ojos", "6DOF · Anchors · Mesh · Occlusion"),
            ("#ffd600", "RENDERING 3D + SPATIAL AUDIO", "Modelos · Texturas · PBR · HDR", "Binaural · HRTF · Reverb · Distance"),
        ],
        "cards": [
            ("🖥️", "Framework WebXR Soberano", "Framework WebXR completo y soberano para crear experiencias AR/VR/XR sin dependencia de Apple, Google o Meta. Estándar abierto."),
            ("🏛️", "Reconstrucción 3D Sitios Ancestrales", "Reconstrucción fotogramétrica 3D de sitios sagrados y ancestrales de las 574 naciones tribales. Preservación digital del patrimonio."),
            ("🗺️", "Guías Turísticas AR", "Guías turísticas de realidad aumentada sobre territorios indígenas: historia, cultura, flora, fauna superpuestas al mundo real."),
            ("📚", "Educación Cultural Inmersiva", "Experiencias educativas inmersivas sobre cultura, historia y tradiciones de los pueblos indígenas. Aprendizaje experiencial 3D."),
            ("🏥", "Telemedicina VR", "Consultas médicas en realidad virtual con visualización 3D de anatomía, telecirugía asistida y diagnóstico remoto inmersivo."),
            ("🎯", "Entrenamiento Militar XR", "Simulaciones de entrenamiento militar en realidad extendida: tácticas, navegación, comunicaciones y emergencias."),
            ("📍", "SLAM y Object Tracking", "Simultaneous Localization and Mapping para posicionamiento preciso. Tracking de objetos, superficies y entornos en tiempo real."),
            ("🖐️", "Hand Tracking Nativo", "Seguimiento de manos sin controladores para interacción natural. Gestos, manipulación de objetos y escritura en el aire."),
            ("🔊", "Spatial Audio 3D", "Audio espacial 3D con HRTF personalizado. Sonidos posicionados en el espacio virtual con reverberación y oclusión realista."),
            ("🛒", "Marketplace de Experiencias", "Marketplace soberano para publicar, descubrir y distribuir experiencias AR/VR/XR creadas por comunidades indígenas."),
        ],
        "apis": [
            ("POST", "/api/v1/xr/scene", "Crear escena XR. Params: type (AR/VR/XR), assets, anchors."),
            ("GET", "/api/v1/xr/assets", "Biblioteca de assets 3D. Filtros: category, format, size, culture."),
            ("POST", "/api/v1/xr/track", "Iniciar tracking. Params: mode (SLAM/hand/eye), config."),
            ("GET", "/api/v1/xr/experiences", "Experiencias publicadas. Filtros: type, territory, rating."),
            ("POST", "/api/v1/xr/publish", "Publicar experiencia. Params: scene_id, metadata, pricing."),
            ("GET", "/api/v1/xr/devices", "Dispositivos compatibles. Info: capabilities, firmware, status."),
        ],
        "db_name": "ierahkwa-realidad-aumentada-soberana",
        "db_stores": ["xr-scenes", "asset-models", "experience-catalog"],
    },
    {
        "dir": "antivirus-soberano",
        "title": "Antivirus Soberano",
        "subtitle": "Protección Anti-Malware Post-Cuántica",
        "icon": "🦠",
        "accent": "#f44336",
        "description": "Antivirus soberano que reemplaza Norton, McAfee y ClamAV. Motor de detección con IA que identifica malware zero-day, sandbox soberana para análisis de archivos sospechosos, protección en tiempo real, actualizaciones de firmas vía blockchain (inmutables), escaneo de red, protección de endpoints, zero telemetría a Big Tech.",
        "metrics": [
            ("IA", "Zero-Day"),
            ("Sand", "Box"),
            ("Real", "Time"),
            ("Firmas", "Blockchain"),
            ("0", "Telemetría"),
            ("End", "Point"),
        ],
        "arch_layers": [
            ("#f44336", "ARCHIVO/RED", "Ficheros · Procesos · Conexiones · Tráfico", "Endpoint · Gateway · Email · USB · Download"),
            ("#ffd600", "MOTOR IA + HEURÍSTICA", "Firmas · Comportamiento · ML · Emulación", "Static · Dynamic · Neural · Ensemble"),
            ("#7c4dff", "SANDBOX ANÁLISIS", "Ejecución · Monitoreo · Syscalls · Red", "Aislado · Temporal · Snapshot · Trazas"),
            ("#f44336", "CUARENTENA/LIMPIEZA", "Aislamiento · Eliminación · Restauración · Reporte", "Alerta · Log · Forense · Actualización"),
        ],
        "cards": [
            ("🧠", "Detección Zero-Day con IA", "Inteligencia artificial entrenada para detectar malware zero-day nunca antes visto, mediante análisis de comportamiento y patrones sospechosos."),
            ("📦", "Sandbox Soberana de Análisis", "Sandbox aislada para ejecutar archivos sospechosos de forma segura, monitoreando syscalls, red y cambios en el filesystem."),
            ("⚡", "Protección en Tiempo Real", "Escaneo en tiempo real de todos los archivos, procesos y conexiones de red. Bloqueo instantáneo de amenazas detectadas."),
            ("⛓️", "Firmas vía Blockchain Inmutables", "Base de datos de firmas de malware distribuida via blockchain. Inmutable, verificable, imposible de manipular por atacantes."),
            ("🌐", "Escaneo de Red Completo", "Escaneo de todo el tráfico de red: DNS, HTTP, SMTP, archivos adjuntos, descargas, conexiones sospechosas, C2 callbacks."),
            ("🖥️", "Protección de Endpoints", "Agente ligero para protección de endpoints: escritorios, laptops, servidores, móviles. Mínimo impacto en rendimiento."),
            ("🔒", "Cuarentena Inteligente", "Sistema de cuarentena inteligente que aísla archivos sospechosos sin eliminarlos, permitiendo análisis forense y restauración."),
            ("🔍", "Análisis Heurístico Avanzado", "Análisis heurístico que detecta variantes de malware conocido mediante similitud de código, empaquetado y comportamiento."),
            ("📊", "Reportes de Amenazas", "Reportes detallados de amenazas detectadas: tipo, origen, impacto, acciones tomadas, recomendaciones de mitigación."),
            ("🔄", "Actualización P2P sin CDN", "Actualización de firmas y motor vía red P2P soberana. Sin dependencia de CDNs de Big Tech, distribución entre nodos soberanos."),
        ],
        "apis": [
            ("POST", "/api/v1/antivirus/scan", "Escanear archivo o directorio. Params: target, depth, engines."),
            ("GET", "/api/v1/antivirus/status", "Estado de protección. Info: última actualización, amenazas, escaneos."),
            ("POST", "/api/v1/antivirus/quarantine", "Gestionar cuarentena. Params: file_id, action (isolate/restore/delete)."),
            ("GET", "/api/v1/antivirus/signatures", "Base de firmas. Info: versión, count, última actualización, hash."),
            ("POST", "/api/v1/antivirus/sandbox", "Ejecutar en sandbox. Params: file, timeout, monitoring_level."),
            ("GET", "/api/v1/antivirus/threats", "Amenazas detectadas. Filtros: severity, type, date, status."),
        ],
        "db_name": "ierahkwa-antivirus-soberano",
        "db_stores": ["antivirus-signatures", "quarantine-files", "threat-log"],
    },
    {
        "dir": "accesibilidad-soberana",
        "title": "Accesibilidad Soberana",
        "subtitle": "Herramientas de Accesibilidad Universal para Todos",
        "icon": "♿",
        "accent": "#607D8B",
        "description": "Plataforma de accesibilidad que reemplaza herramientas de Apple, Google y Microsoft. Screen reader soberano en 14 idiomas indígenas, texto a voz y voz a texto, magnificación, alto contraste, navegación por teclado, lengua de señas con IA, subtítulos automáticos, Braille display soberano, WCAG 2.1 AAA para todas las 408 plataformas.",
        "metrics": [
            ("14", "Idiomas"),
            ("Screen", "Reader"),
            ("TTS", "STT"),
            ("Lengua", "Señas"),
            ("Braille", "Display"),
            ("WCAG", "AAA"),
        ],
        "arch_layers": [
            ("#607D8B", "USUARIO", "Ciego · Sordo · Motor · Cognitivo", "Preferencias · Perfil · Dispositivo · Contexto"),
            ("#7c4dff", "DETECCIÓN DE NECESIDAD", "Perfil · Sensores · Configuración · Auto-detect", "Tipo · Severidad · Preferencia · Asistencia"),
            ("#00bcd4", "MOTOR A11Y (TTS/STT/SEÑAS)", "Texto a Voz · Voz a Texto · Lengua Señas", "14 Idiomas · IA · Neural · Braille · Contraste"),
            ("#ffd600", "INTERFAZ ADAPTADA", "Layout · Colores · Fuentes · Navegación", "Responsive · Alto Contraste · Magnificación · Teclado"),
        ],
        "cards": [
            ("🔊", "Screen Reader 14 Idiomas Indígenas", "Screen reader soberano con soporte para 14 idiomas indígenas además de español e inglés. Voces neurales naturales y pronunciación correcta."),
            ("🗣️", "Texto a Voz Soberano", "Motor de texto a voz con voces neurales en idiomas indígenas. Entonación natural, velocidad ajustable, voces masculinas y femeninas."),
            ("🎙️", "Voz a Texto con IA", "Reconocimiento de voz con IA para dictado en idiomas indígenas. Puntuación automática, vocabulario especializado, modo offline."),
            ("🔍", "Magnificación Adaptativa", "Magnificación de pantalla inteligente que sigue el foco, cursor y punto de lectura. Zoom suave, anti-aliasing, sin pérdida de contexto."),
            ("🌓", "Alto Contraste Automático", "Modo de alto contraste automático con múltiples esquemas: blanco/negro, amarillo/negro, verde/negro. Detección de contraste insuficiente."),
            ("⌨️", "Navegación por Teclado Universal", "Navegación completa por teclado para todas las 408 plataformas. Focus visible, atajos de teclado, skip links, landmarks ARIA."),
            ("🤟", "Lengua de Señas con IA", "Traducción automática a lengua de señas mediante avatar 3D con IA. Soporte para múltiples variantes de señas de las Américas."),
            ("📝", "Subtítulos Automáticos", "Subtítulos automáticos en tiempo real para todo contenido de audio y video. Sincronización precisa, identificación de hablantes."),
            ("⠿", "Soporte Braille Display", "Soporte nativo para pantallas Braille. Grado 1 y 2, navegación, edición y lectura con refreshable Braille display soberano."),
            ("✅", "Auditoría WCAG 2.1 AAA", "Herramienta de auditoría automática de accesibilidad WCAG 2.1 nivel AAA para todas las plataformas del ecosistema soberano."),
        ],
        "apis": [
            ("GET", "/api/v1/a11y/audit", "Auditar accesibilidad. Params: url, level (A/AA/AAA), standards."),
            ("POST", "/api/v1/a11y/tts", "Texto a voz. Params: text, language, voice, speed, format."),
            ("POST", "/api/v1/a11y/stt", "Voz a texto. Params: audio, language, vocabulary, punctuation."),
            ("GET", "/api/v1/a11y/config", "Configuración de accesibilidad del usuario. Preferencias guardadas."),
            ("POST", "/api/v1/a11y/signlang", "Traducir a lengua de señas. Params: text, variant, format (video/3d)."),
            ("GET", "/api/v1/a11y/braille", "Convertir a Braille. Params: text, grade (1/2), language."),
        ],
        "db_name": "ierahkwa-accesibilidad-soberana",
        "db_stores": ["a11y-config", "tts-cache", "audit-results"],
    },
]


def generate_html(p):
    """Generate Pattern B index.html for a platform."""
    title = p["title"]
    subtitle = p["subtitle"]
    icon = p["icon"]
    accent = p["accent"]
    desc = p["description"]
    slug = p["dir"]
    metrics = p["metrics"]
    cards = p["cards"]
    apis = p["apis"]
    arch = p["arch_layers"]
    db_name = p["db_name"]
    db_stores = p["db_stores"]

    # Short description for twitter
    short_desc = desc[:140].rsplit(" ", 1)[0] + "."

    # Build metrics HTML
    metrics_html = ""
    for val, lbl in metrics:
        metrics_html += f'<div class="stat" role="listitem"><div class="val">{val}</div><div class="lbl">{lbl}</div></div>\n'

    # Build architecture HTML
    arch_html = ""
    for i, (color, layer_name, line1, line2) in enumerate(arch):
        connector = ""
        if i < len(arch) - 1:
            connector = """                   │
"""
        arch_html += f'''<span style="color:{color}">┌─ {layer_name} {"─" * max(1, 50 - len(layer_name))}┐</span>
│  {line1}{" " * max(1, 55 - len(line1))}│
│  {line2}{" " * max(1, 55 - len(line2))}│
<span style="color:{color}">└──────────────────┴────────────────────────────────────────┘</span>
{connector}'''

    # Build cards HTML
    cards_html = ""
    for c_icon, c_title, c_desc in cards:
        cards_html += f'''<article class="card">
<div class="card-icon" aria-hidden="true">{c_icon}</div>
<h4>{c_title}</h4>
<p>{c_desc}</p>
</article>
'''

    # Build API HTML
    api_html = ""
    for method, endpoint, api_desc in apis:
        color = "#ffd600" if method == "POST" else "#00FF41"
        api_html += f'''<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{color};font-size:.7rem;margin-right:.5rem">{method}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{endpoint}</code></div>
<p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{api_desc}</p>
'''

    # DB stores as JSON array
    stores_str = str(db_stores).replace("'", '"')

    html = f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cuántico Kyber-768, blockchain MameyNode y soberanía digital total.">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{slug}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{title} — Ierahkwa Ne Kanienke">
<meta property="og:description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {subtitle} con cifrado post-cuántico Kyber-768, blockchain MameyNode y soberanía digital total.">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{slug}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{title} — Ierahkwa Ne Kanienke">
<meta name="twitter:description" content="{title} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {short_desc}">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{title} — Ierahkwa Ne Kanienke</title>
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
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">⚛️</span> Quantum-Safe</span>
</header>

<main id="main">

<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {subtitle}</div>
<h2><span>{title}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar Módulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>

<div class="stats" role="list" aria-label="Metricas del sistema">
{metrics_html}</div>

<div class="section" id="architecture">
<h2><span aria-hidden="true">🏗️</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {title}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
{arch_html}</div>
</div>

<div class="section-title" id="features">
<h3>Módulos de la Plataforma</h3>
<p>10 herramientas soberanas de grado empresarial</p>
</div>

<div class="grid">
{cards_html}</div>

<div class="section" id="api">
<h2><span aria-hidden="true">🔌</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">
{api_html}</div>
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
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 100 operaciones/mes</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Dashboard básico</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ 1 proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte comunidad</li>
</ul>
</div>
<div class="card" style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)">
<h4 style="color:var(--accent);font-size:.9rem">Cacique</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">5 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Ilimitado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Analytics avanzados</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Multi-proyecto</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ API completa</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte prioritario</li>
</ul>
</div>
<div class="card">
<h4 style="color:var(--accent);font-size:.9rem">Nación</h4>
<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">15 W/mes</div>
<ul style="list-style:none;padding:0">
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Multi-nación</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ SLA 99.99%</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Auditor dedicado</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Soporte 24/7</li>
<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ Custom integrations</li>
</ul>
</div>
</div>

</main>

<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{title}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">200+ plataformas soberanas &middot; 15 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">🛡️</span> Seguro</span></div>
</footer>

<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script src="../shared/ierahkwa-protocols.js"></script>
<script src="../shared/ierahkwa-interconnect.js"></script>
<script>
(function(){{
  var DB_NAME='{db_name}';
  var DB_VER=1;
  var STORES={stores_str};
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
      b.textContent='Modo Offline — Datos y operaciones pendientes disponibles offline.';
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
    print("=" * 60)
    print("create-space-global.py — Generando 8 plataformas")
    print("=" * 60)

    for p in PLATFORMS:
        dir_path = os.path.join(BASE, p["dir"])
        os.makedirs(dir_path, exist_ok=True)
        html = generate_html(p)
        file_path = os.path.join(dir_path, "index.html")
        with open(file_path, "w", encoding="utf-8") as f:
            f.write(html)
        line_count = html.count("\n") + 1
        print(f"  [OK] {p['dir']}/index.html — {line_count} líneas — {p['icon']} {p['title']}")

    print()
    print(f"Total: {len(PLATFORMS)} plataformas generadas.")
    print("=" * 60)


if __name__ == "__main__":
    main()
