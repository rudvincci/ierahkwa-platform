#!/usr/bin/env python3
"""Generate missing platform HTML files for Ierahkwa Ne Kanienke."""
import os

BASE = "/Users/ruddie/Desktop/files/Soberano-Organizado/02-plataformas-html"
DOMAIN = "https://ierahkwa.nation"

# Platform definitions: (dirname, pretty_name, emoji, subtitle, accent_color, description, features)
# Features: list of (emoji, title, desc)
PLATFORMS = [
    # NEXUS ORBITAL (#00bcd4)
    ("internet-soberano", "Internet Soberano", "🌐", "Red de Internet Soberana", "#00bcd4",
     "Red de internet independiente para comunidades indigenas con infraestructura propia.",
     [("🌐","Red Backbone","Fibra optica y enlaces de microondas soberanos."),("📡","Puntos de Acceso","WiFi comunitario gratuito en plazas y centros."),("🔒","VPN Soberana","Tunel cifrado para navegacion privada."),("⚡","CDN Local","Distribucion de contenido desde servidores locales."),("📊","Monitoreo Red","Dashboard de trafico, latencia y disponibilidad."),("🛡️","Firewall Nacional","Proteccion perimetral contra ataques DDoS."),("📱","App Movil","Gestion de conexion desde dispositivos moviles."),("🔗","Peering","Acuerdos de interconexion con ISPs regionales."),("💰","Tarifas Justas","Precios accesibles para comunidades rurales."),("🌍","Cobertura","Expansion progresiva a 19 paises.")]),
    ("observatorio-terrestre", "Observatorio Terrestre", "🔭", "Observacion Satelital", "#00bcd4",
     "Sistema de observacion terrestre con imagenes satelitales para monitoreo ambiental.",
     [("🔭","Imagenes Satelitales","Captura diaria de imagenes multiespectrales."),("🌍","Cobertura Global","Monitoreo de 19 paises en tiempo real."),("🌳","Deforestacion","Deteccion automatica de cambios forestales."),("🌊","Recursos Hidricos","Seguimiento de cuerpos de agua y cuencas."),("🔥","Incendios","Alertas tempranas de incendios forestales."),("📊","Analisis AI","Clasificacion de uso de suelo con ML."),("🗺️","Mapas","Generacion de mapas tematicos actualizados."),("📡","Recepcion","Estaciones de recepcion de datos satelitales."),("💾","Archivo","Repositorio historico de imagenes desde 2020."),("🤝","API Abierta","Acceso programatico a datos geoespaciales.")]),
    ("quantum-computing", "Quantum Computing", "⚛️", "Computacion Cuantica", "#00bcd4",
     "Infraestructura de computacion cuantica para criptografia y optimizacion soberana.",
     [("⚛️","QPU Access","Acceso a procesadores cuanticos de 127 qubits."),("🔐","Criptografia","Algoritmos post-cuanticos ML-KEM-1024."),("🧮","Simulacion","Simulacion de moleculas y materiales."),("📈","Optimizacion","Problemas de optimizacion combinatoria."),("🤖","QML","Machine learning cuantico hibrido."),("🔗","Entrelazamiento","Red de distribucion de claves cuanticas."),("💻","SDK","Kit de desarrollo para algoritmos cuanticos."),("📚","Educacion","Cursos de computacion cuantica en espanol."),("🌐","Cloud","Acceso remoto a hardware cuantico."),("🛡️","QKD","Distribucion cuantica de claves criptograficas.")]),

    # NEXUS CEREBRO (#7c4dff)
    ("biblioteca-ancestral", "Biblioteca Ancestral", "📖", "Conocimiento Ancestral", "#7c4dff",
     "Biblioteca digital de conocimiento ancestral indigena con preservacion cultural.",
     [("📖","Manuscritos","Digitalizacion de textos y codices ancestrales."),("🎙️","Tradicion Oral","Grabaciones de historias y leyendas."),("🗺️","Mapas Historicos","Cartografia de territorios originarios."),("🌿","Medicina","Catalogacion de medicina tradicional."),("🎨","Arte","Galeria de arte y artesania indigena."),("🔍","Buscador","Motor de busqueda semantico multilingue."),("📱","App Movil","Acceso offline a la biblioteca."),("🤖","AI Curator","Recomendaciones personalizadas de contenido."),("👥","Colaboracion","Contribuciones de ancianos y sabios."),("🔒","Soberania","Datos almacenados en servidores soberanos.")]),
    ("escuela-soberana", "Escuela Soberana", "🏫", "Educacion Digital", "#7c4dff",
     "Plataforma educativa K-12 con curriculo indigena y tecnologia adaptativa.",
     [("🏫","Aulas Virtuales","Clases en vivo con pizarra interactiva."),("📚","Curriculo","Contenido alineado con cultura indigena."),("🌐","14 Lenguas","Material en 14 lenguas indigenas."),("🤖","Tutor AI","Asistente inteligente personalizado."),("📊","Progreso","Seguimiento de avance por estudiante."),("👨‍🏫","Docentes","Herramientas para maestros y facilitadores."),("📱","Offline","Funciona sin conexion a internet."),("🎮","Gamificacion","Aprendizaje basado en juegos."),("📋","Evaluacion","Examenes adaptativos con AI."),("🎓","Certificados","Diplomas verificables en blockchain.")]),
    ("lenguas-indigenas", "Lenguas Indigenas", "🗣️", "Preservacion Linguistica", "#7c4dff",
     "Plataforma de preservacion y ensenanza de 14 lenguas indigenas americanas.",
     [("🗣️","14 Lenguas","Nahuatl, Quechua, Maya, Guarani y mas."),("📖","Diccionarios","Diccionarios digitales bilingues."),("🎙️","Pronunciacion","Audio nativo para cada palabra."),("🤖","Traductor AI","Traduccion automatica entre lenguas."),("📱","App Movil","Aprendizaje en el celular."),("👶","Ninos","Contenido especial para primera infancia."),("📊","Progreso","Niveles de competencia linguistica."),("🎮","Juegos","Aprendizaje gamificado."),("👥","Comunidad","Foros de practica entre hablantes."),("📡","Offline","Funciona sin conexion.")]),
    ("buscador-soberano", "Buscador Soberano", "🔍", "Motor de Busqueda", "#7c4dff",
     "Motor de busqueda soberano que no rastrea usuarios ni vende datos.",
     [("🔍","Busqueda Web","Indexacion de contenido relevante."),("🔒","Privacidad","Zero tracking, zero cookies."),("🌐","Multilingue","Busqueda en 14 lenguas indigenas."),("🤖","AI Ranking","Resultados ordenados por relevancia AI."),("🗺️","Local","Resultados priorizados por region."),("📱","App","Aplicacion movil nativa."),("🖼️","Imagenes","Busqueda de imagenes soberana."),("📰","Noticias","Agregador de noticias indigenas."),("📊","Analytics","Tendencias de busqueda anonimizadas."),("⚡","Rapido","Respuestas en menos de 100ms.")]),
    ("investigacion-soberana", "Investigacion Soberana", "🔬", "Ciencia e Investigacion", "#7c4dff",
     "Plataforma de investigacion cientifica con enfoque en conocimiento indigena.",
     [("🔬","Laboratorios","Laboratorios virtuales colaborativos."),("📄","Publicaciones","Repositorio de papers y tesis."),("💰","Fondos","Gestion de becas y financiamiento."),("🤝","Colaboracion","Red de investigadores indigenas."),("📊","Datos","Datasets abiertos para investigacion."),("🧬","Biologia","Investigacion en biodiversidad nativa."),("🌿","Etnobotanica","Estudio de plantas medicinales."),("🤖","AI Tools","Herramientas de AI para analisis."),("📱","App","Acceso movil al portal."),("🔒","Etica","Protocolos de etica en investigacion.")]),
    ("laboratorio-virtual", "Laboratorio Virtual", "🧪", "Labs Virtuales", "#7c4dff",
     "Laboratorios virtuales para experimentacion cientifica y educativa.",
     [("🧪","Quimica","Simulaciones de reacciones quimicas."),("🔬","Biologia","Microscopio virtual y disecciones."),("⚡","Fisica","Experimentos de mecanica y electromagnetismo."),("🌍","Geologia","Simulaciones de procesos geologicos."),("🤖","Robotica","Programacion de robots virtuales."),("📊","Datos","Recoleccion y analisis de datos."),("👥","Colaborativo","Labs compartidos entre escuelas."),("📱","Movil","Acceso desde cualquier dispositivo."),("🎓","Certificados","Completar labs otorga credenciales."),("💾","Guardado","Progreso guardado automaticamente.")]),
    ("traductor-soberano", "Traductor Soberano", "🌐", "Traduccion AI", "#7c4dff",
     "Traductor automatico soberano con soporte para 14 lenguas indigenas.",
     [("🌐","14 Lenguas","Traduccion entre lenguas indigenas."),("🤖","AI Neural","Red neuronal entrenada con datos nativos."),("🎙️","Voz a Texto","Transcripcion de audio en lengua nativa."),("📷","Imagen","Traduccion de texto en imagenes."),("📱","App Offline","Funciona sin internet."),("💬","Chat","Traduccion en tiempo real para chat."),("📄","Documentos","Traduccion de documentos completos."),("🔒","Privacidad","Procesamiento 100% en dispositivo."),("📊","Mejora","Mejora continua con feedback comunitario."),("🌍","Espanol","Traduccion espanol-lengua indigena.")]),
    ("datos-abiertos", "Datos Abiertos", "📊", "Open Data Soberano", "#7c4dff",
     "Portal de datos abiertos para transparencia y desarrollo comunitario.",
     [("📊","Datasets","Miles de datasets en formato abierto."),("🗺️","Geoespacial","Datos geograficos y territoriales."),("📈","Economia","Indicadores economicos comunitarios."),("🏥","Salud","Estadisticas de salud publica."),("🎓","Educacion","Datos educativos por region."),("🌍","Ambiente","Datos medioambientales."),("🔗","API","Acceso programatico RESTful."),("📱","Visualizador","Dashboards interactivos."),("🔒","Gobernanza","Politicas de uso de datos."),("🤖","AI Ready","Datasets preparados para ML.")]),
    ("certificaciones-nft", "Certificaciones NFT", "🏆", "Credenciales Blockchain", "#7c4dff",
     "Sistema de certificaciones y credenciales verificables en blockchain.",
     [("🏆","Certificados","Diplomas y titulos en blockchain."),("🔗","Verificable","Verificacion instantanea por QR."),("🎓","Educacion","Credenciales academicas."),("💼","Profesional","Certificaciones de competencias."),("🌐","Portatil","Valido en 19 paises."),("🤖","AI Validacion","Deteccion de certificados falsos."),("📱","Wallet","Cartera digital de credenciales."),("🏛️","Gobierno","Licencias y permisos oficiales."),("💰","Gratuito","Sin costo para ciudadanos."),("🔒","Inmutable","Registros permanentes e inalterables.")]),

    # NEXUS TESORO (#ffd600)
    ("wampum-cbdc", "WAMPUM CBDC", "💰", "Moneda Digital Soberana", "#ffd600",
     "Moneda digital del banco central soberano con blockchain proof-of-stake.",
     [("💰","WPM Token","Moneda nativa con 10T supply maximo."),("⛓️","Blockchain","Chain ID 574, 12,847 TPS."),("🔐","Post-Quantum","Criptografia ML-DSA-65."),("🏦","Banco Central","Politica monetaria autonoma."),("💱","Forex","Conversion WPM a monedas fiat."),("📱","Wallet","Cartera digital movil."),("🔥","Burn","0.1% quema por transaccion."),("📊","Explorer","Explorador de blockchain publico."),("🤝","P2P","Transferencias persona a persona."),("🌍","19 Paises","Aceptado en todo el territorio.")]),
    ("sovereign-payments", "Sovereign Payments", "💳", "Pagos Digitales", "#ffd600",
     "Sistema de pagos digitales soberanos con comisiones minimas del 2%.",
     [("💳","Pagos","Procesamiento instantaneo de pagos."),("📱","QR","Pagos por codigo QR."),("🏪","Comercios","Terminal para comercios."),("💰","2% Fee","Comision minima del 2%."),("🔒","Seguro","Cifrado end-to-end."),("🌐","Online","Pagos web y e-commerce."),("📊","Dashboard","Panel de control para comerciantes."),("🔗","API","Integracion para desarrolladores."),("💱","Multi-moneda","WPM y monedas locales."),("📋","Recibos","Comprobantes digitales automaticos.")]),
    ("algo-trading", "Algo Trading", "📈", "Trading Algoritmico", "#ffd600",
     "Plataforma de trading algoritmico para el mercado WAMPUM.",
     [("📈","Algoritmos","Estrategias de trading automatizado."),("🤖","AI Trading","Modelos predictivos con ML."),("📊","Backtesting","Pruebas historicas de estrategias."),("⚡","Baja Latencia","Ejecucion en microsegundos."),("🔒","Seguro","Limites de riesgo automaticos."),("📱","Dashboard","Monitoreo en tiempo real."),("💰","DeFi","Integracion con protocolos DeFi."),("🔗","API","API para bots de trading."),("📋","Reportes","Analisis de rendimiento."),("🌐","24/7","Mercado abierto 24 horas.")]),
    ("sovereign-insurance", "Sovereign Insurance", "🛡️", "Seguros Soberanos", "#ffd600",
     "Sistema de seguros mutuales para comunidades indigenas.",
     [("🛡️","Seguros","Cobertura de salud, vida y propiedad."),("🤝","Mutual","Modelo cooperativo comunitario."),("💰","Bajo Costo","Primas accesibles (8% fee)."),("📱","App","Gestion de polizas desde el celular."),("🤖","AI Claims","Procesamiento automatico de reclamos."),("📊","Actuarial","Modelos actuariales con AI."),("🏥","Salud","Cobertura medica completa."),("🌾","Agricola","Seguro contra desastres naturales."),("💼","Micro","Microseguros para artesanos."),("🔗","Blockchain","Polizas verificables en blockchain.")]),
    ("sovereign-marketplace", "Sovereign Marketplace", "🏪", "Mercado Digital", "#ffd600",
     "Marketplace soberano con comision del 3% vs 15-45% de Big Tech.",
     [("🏪","Marketplace","Compra y venta de productos."),("💰","3% Fee","Comision justa del 3%."),("🌿","Artesania","Seccion especial de artesania."),("📦","Logistica","Envios integrados."),("⭐","Resenas","Sistema de calificaciones."),("🔒","Pagos Seguros","Escrow con WAMPUM."),("📱","App","Compra desde el celular."),("🌐","19 Paises","Mercado pan-americano."),("🤖","AI Search","Busqueda inteligente."),("📊","Vendedor","Dashboard para vendedores.")]),
    ("sovereign-wallet", "Sovereign Wallet", "👛", "Billetera Digital", "#ffd600",
     "Billetera digital soberana para WAMPUM y criptomonedas.",
     [("👛","Wallet","Almacenamiento seguro de WPM."),("🔐","Biometrico","Autenticacion por huella y rostro."),("💱","Exchange","Conversion entre monedas."),("📱","Movil","App nativa iOS y Android."),("🔗","DeFi","Acceso a protocolos DeFi."),("📊","Portfolio","Seguimiento de inversiones."),("💳","Tarjeta","Tarjeta virtual para pagos."),("🤝","P2P","Envios instantaneos."),("📋","Historial","Registro de transacciones."),("🔒","Cold Storage","Almacenamiento frio opcional.")]),
    ("pension-fund", "Pension Fund", "🏦", "Fondo de Pensiones", "#ffd600",
     "Fondo de pensiones soberano con gestion transparente en blockchain.",
     [("🏦","Fondo","Fondo de pensiones comunitario."),("📈","Rendimiento","Inversiones diversificadas."),("💰","Aportes","Contribuciones automaticas."),("📊","Dashboard","Seguimiento de tu fondo."),("🔗","Blockchain","Transparencia total en blockchain."),("🤖","AI Advisor","Asesor financiero AI."),("📱","App","Consulta desde el celular."),("👴","Jubilacion","Calculo de pension estimada."),("🌐","Portatil","Valido en 19 paises."),("🔒","Seguro","Fondo garantizado.")]),
    ("tokenization", "Tokenization", "🪙", "Tokenizacion de Activos", "#ffd600",
     "Plataforma de tokenizacion de activos reales en blockchain WAMPUM.",
     [("🪙","Tokens","Tokenizacion de activos reales."),("🏠","Inmuebles","Propiedad fraccionada de tierras."),("🌿","Recursos","Tokens respaldados por recursos naturales."),("📈","Trading","Mercado secundario de tokens."),("🔗","Smart Contracts","Contratos inteligentes automaticos."),("📊","Valuacion","Valuacion AI de activos."),("🔒","Legal","Marco legal en 19 jurisdicciones."),("📱","App","Trading desde el celular."),("💰","Dividendos","Distribucion automatica."),("🌐","Global","Accesible desde cualquier pais.")]),
    ("renta-basica", "Renta Basica", "💵", "Ingreso Basico Universal", "#ffd600",
     "Sistema de renta basica universal para ciudadanos indigenas.",
     [("💵","UBI","Ingreso basico mensual en WPM."),("🤖","Automatico","Distribucion por smart contract."),("📱","Wallet","Recepcion en billetera digital."),("📊","Transparencia","Todo visible en blockchain."),("👥","Elegibilidad","Verificacion por identidad FWID."),("🌐","19 Paises","Disponible en todo el territorio."),("💰","Financiacion","Fondos de tarifas de plataforma."),("📈","Ajuste","Ajuste por inflacion automatico."),("🏥","Salud","Complemento para gastos medicos."),("🎓","Educacion","Bono extra para estudiantes.")]),
    ("crypto-exchange", "Crypto Exchange", "📊", "Exchange Soberano", "#ffd600",
     "Exchange de criptomonedas soberano con pares WPM.",
     [("📊","Exchange","Trading de WPM y criptomonedas."),("💱","Pares","WPM/USD, WPM/BTC, WPM/ETH."),("📈","Charts","Graficos profesionales en tiempo real."),("🔒","Custodio","Custodia segura de fondos."),("⚡","Rapido","Matching engine de alta velocidad."),("📱","App","Trading movil."),("🤖","Bots","API para bots de trading."),("💰","Bajo Fee","Comisiones competitivas."),("📋","KYC","Verificacion con identidad FWID."),("🌐","24/7","Mercado abierto permanente.")]),
    ("central-bank", "Central Bank", "🏛️", "Banco Central Soberano", "#ffd600",
     "Banco central digital soberano con politica monetaria autonoma.",
     [("🏛️","Banco Central","Autoridad monetaria soberana."),("💰","Politica","Control de emision de WPM."),("📊","Reservas","Gestion de reservas digitales."),("📈","Tasas","Fijacion de tasas de interes."),("🔗","Blockchain","Operaciones en cadena transparentes."),("🤖","AI","Modelos macroeconomicos predictivos."),("📋","Regulacion","Marco regulatorio financiero."),("🌐","Diplomatico","Relaciones con bancos centrales."),("🔒","Seguridad","Cifrado post-cuantico."),("📱","Dashboard","Panel de indicadores economicos.")]),
    ("smart-factory", "Smart Factory", "🏭", "Fabrica Inteligente", "#ffd600",
     "Plataforma de manufactura inteligente con IoT y automatizacion.",
     [("🏭","Fabrica 4.0","Manufactura inteligente automatizada."),("🤖","Robotica","Lineas de produccion robotizadas."),("📊","IoT","Sensores en toda la planta."),("🔧","Mantenimiento","Mantenimiento predictivo con AI."),("📈","Eficiencia","Optimizacion de produccion."),("🔗","Supply Chain","Cadena de suministro blockchain."),("💡","Energia","Eficiencia energetica inteligente."),("📱","Dashboard","Monitoreo remoto."),("🎓","Capacitacion","Entrenamiento de operarios."),("🌿","Sostenible","Produccion con bajo impacto ambiental.")]),
    ("artisan-market", "Artisan Market", "🎨", "Mercado Artesanal", "#ffd600",
     "Marketplace especializado en artesania indigena con comision del 5%.",
     [("🎨","Artesania","Productos artesanales autenticos."),("💰","5% Fee","95% para el artesano."),("📷","Galeria","Fotos y videos de productos."),("📦","Envio","Logistica de envio integrada."),("🌐","Global","Compradores de todo el mundo."),("🔗","Autenticidad","Certificado blockchain de origen."),("👥","Artesanos","Perfiles de artesanos verificados."),("📱","App","Compra y venta movil."),("⭐","Resenas","Calificaciones de compradores."),("🎓","Capacitacion","Cursos de e-commerce para artesanos.")]),
    ("eco-tourism", "Eco Tourism", "🌿", "Turismo Ecologico", "#43a047",
     "Plataforma de turismo ecologico y cultural en territorios indigenas.",
     [("🌿","Eco Tours","Experiencias de turismo ecologico."),("🏕️","Hospedaje","Alojamiento comunitario."),("🗺️","Rutas","Rutas turisticas culturales."),("👥","Guias","Guias indigenas certificados."),("📱","Reservas","Sistema de reservas online."),("📷","Galeria","Fotos y videos de destinos."),("💰","Ingresos","80% para la comunidad local."),("🌍","19 Paises","Destinos en todo el territorio."),("⭐","Resenas","Opiniones de viajeros."),("🔒","Seguro","Seguro de viajero incluido.")]),

    # NEXUS VOCES (#e040fb)
    ("sovereign-social", "Sovereign Social", "💬", "Red Social Soberana", "#e040fb",
     "Red social soberana que no vende datos ni muestra publicidad.",
     [("💬","Feed","Publicaciones, fotos y videos."),("🔒","Privacidad","Zero tracking, zero ads."),("👥","Comunidades","Grupos por nacion y cultura."),("📱","App","App movil nativa."),("🌐","14 Lenguas","Interfaz en lenguas indigenas."),("🎥","Stories","Historias efimeras de 24h."),("💰","Creadores","Monetizacion para creadores."),("🤖","AI Mod","Moderacion AI de contenido."),("📊","Analytics","Estadisticas para creadores."),("🔗","Federation","Red federada descentralizada.")]),
    ("sovereign-shorts", "Sovereign Shorts", "🎬", "Videos Cortos", "#e040fb",
     "Plataforma de videos cortos soberana estilo TikTok sin tracking.",
     [("🎬","Shorts","Videos cortos de 15-60 segundos."),("🎵","Musica","Biblioteca de musica indigena."),("🤖","AI Edit","Edicion automatica con AI."),("💰","Monetizacion","Ingresos para creadores."),("📱","App","App movil fluida."),("🌐","Tendencias","Trending por region y cultura."),("👥","Duets","Colaboraciones entre creadores."),("🔒","Privacidad","Sin recoleccion de datos."),("📊","Analytics","Metricas de engagement."),("🎭","Filtros","Filtros de realidad aumentada.")]),
    ("sovereign-forum", "Sovereign Forum", "💭", "Foro Soberano", "#e040fb",
     "Foro de discusion comunitario para debate y gobernanza participativa.",
     [("💭","Foros","Temas de discusion organizados."),("🗳️","Votacion","Encuestas y votaciones."),("👥","Moderacion","Moderadores comunitarios."),("🌐","Multilingue","Traduccion automatica."),("📱","App","Acceso movil."),("🔒","Privacidad","Anonimato opcional."),("📊","Trending","Temas populares."),("🏛️","Gobierno","Foros oficiales de gobierno."),("⭐","Reputacion","Sistema de karma."),("📋","Wiki","Base de conocimiento colaborativa.")]),
    ("sovereign-music", "Sovereign Music", "🎵", "Musica Soberana", "#e040fb",
     "Plataforma de streaming de musica indigena con regalias justas.",
     [("🎵","Streaming","Musica en alta calidad."),("💰","Regalias","90% para el artista."),("🎤","Upload","Subida directa de musica."),("📱","App","Escucha offline."),("🌐","14 Lenguas","Musica en lenguas indigenas."),("📊","Analytics","Estadisticas para artistas."),("🎭","Playlists","Listas curadas por cultura."),("🤖","AI Radio","Radio personalizada con AI."),("💿","Albums","Discografia completa."),("🎪","Eventos","Conciertos en vivo streaming.")]),
    ("sovereign-media", "Sovereign Media", "📺", "Medios Soberanos", "#e040fb",
     "Plataforma de medios de comunicacion soberanos: TV, radio y prensa.",
     [("📺","TV","Canales de television soberanos."),("📻","Radio","Emisoras comunitarias."),("📰","Prensa","Periodismo independiente."),("🎥","Documentales","Produccion documental indigena."),("📱","App","Streaming movil."),("🌐","14 Lenguas","Contenido multilingue."),("🤖","AI News","Resumen de noticias con AI."),("📊","Audiencia","Metricas de alcance."),("💰","Sostenible","Modelo sin publicidad."),("👥","Ciudadano","Periodismo ciudadano.")]),
    ("sovereign-news", "Sovereign News", "📰", "Noticias Soberanas", "#e040fb",
     "Agencia de noticias soberana con cobertura de 19 paises.",
     [("📰","Noticias","Cobertura en tiempo real."),("🌐","19 Paises","Corresponsales en cada pais."),("📱","App","Alertas de noticias."),("🤖","AI Resumen","Resumenes automaticos."),("🌍","Lenguas","Noticias en 14 lenguas."),("📺","Video","Notas en video."),("📊","Datos","Periodismo de datos."),("💬","Comentarios","Debate comunitario."),("🔍","Investigacion","Periodismo investigativo."),("🔒","Fuentes","Proteccion de fuentes.")]),
    ("universal-translator", "Universal Translator", "🌍", "Traductor Universal", "#e040fb",
     "Traductor universal en tiempo real para comunicacion entre lenguas.",
     [("🌍","Tiempo Real","Traduccion simultanea."),("🎙️","Voz","Interpretacion de voz en vivo."),("📱","App","App movil."),("🌐","14+ Lenguas","Todas las lenguas indigenas."),("🤖","AI Neural","Redes neuronales de traduccion."),("💬","Chat","Traduccion en mensajeria."),("📞","Llamadas","Interpretacion en llamadas."),("📄","Documentos","Traduccion de archivos."),("🔒","Offline","Funciona sin internet."),("📊","Mejora","Aprendizaje continuo.")]),
    ("sovereign-voice", "Sovereign Voice", "🎙️", "Asistente de Voz", "#e040fb",
     "Asistente de voz soberano que habla 14 lenguas indigenas.",
     [("🎙️","Asistente","Comandos de voz inteligentes."),("🌐","14 Lenguas","Comprende lenguas indigenas."),("🤖","AI","Procesamiento de lenguaje natural."),("📱","Movil","Integrado en el telefono."),("🏠","Hogar","Control de dispositivos IoT."),("🔒","Privacidad","Procesamiento en dispositivo."),("📋","Tareas","Gestion de recordatorios."),("🌤️","Info","Clima, noticias, horarios."),("👶","Ninos","Modo seguro para ninos."),("📞","Llamadas","Hacer llamadas por voz.")]),
    ("sovereign-conference", "Sovereign Conference", "🎥", "Videoconferencia", "#e040fb",
     "Plataforma de videoconferencia soberana con cifrado end-to-end.",
     [("🎥","Video HD","Videollamadas en alta definicion."),("🔒","E2E","Cifrado end-to-end."),("👥","Salas","Hasta 500 participantes."),("📋","Agenda","Gestion de reuniones."),("🖥️","Pantalla","Compartir pantalla."),("💬","Chat","Chat en reunion."),("📱","Movil","App nativa."),("🌐","Traduccion","Subtitulos en 14 lenguas."),("📊","Grabacion","Grabacion en la nube."),("🏛️","Gobierno","Sesiones de gobierno oficiales.")]),
    ("sovereign-collab", "Sovereign Collab", "🤝", "Colaboracion Digital", "#e040fb",
     "Suite de colaboracion digital para equipos y organizaciones.",
     [("🤝","Equipos","Espacios de trabajo por equipo."),("📄","Docs","Documentos colaborativos."),("📋","Tareas","Gestion de proyectos."),("💬","Chat","Mensajeria de equipo."),("📁","Archivos","Almacenamiento compartido."),("📊","Dashboard","Panel de productividad."),("📱","App","Acceso movil."),("🔗","Integracion","Conecta con otras plataformas."),("🔒","Seguro","Cifrado end-to-end."),("🌐","Global","Equipos distribuidos.")]),
    ("sovereign-podcast", "Sovereign Podcast", "🎧", "Podcasts Soberanos", "#e040fb",
     "Plataforma de podcasts en lenguas indigenas.",
     [("🎧","Podcasts","Miles de podcasts indigenas."),("🎙️","Crear","Herramientas de grabacion."),("📱","App","Escucha offline."),("🌐","14 Lenguas","Contenido multilingue."),("💰","Monetizacion","Ingresos para creadores."),("📊","Analytics","Estadisticas de audiencia."),("🤖","AI","Transcripcion automatica."),("⭐","Curado","Recomendaciones personalizadas."),("📋","RSS","Feed RSS compatible."),("👥","Comunidad","Comentarios y resenas.")]),
    ("sovereign-radio", "Sovereign Radio", "📻", "Radio Soberana", "#e040fb",
     "Red de emisoras de radio digital indigenas en 19 paises.",
     [("📻","En Vivo","Transmision en vivo 24/7."),("🌐","Estaciones","Emisoras por region."),("🎵","Musica","Musica indigena y tradicional."),("📰","Noticias","Informativos locales."),("🌍","14 Lenguas","Programacion multilingue."),("📱","App","Radio movil."),("💬","Interaccion","Llamadas de oyentes."),("📊","Audiencia","Metricas de alcance."),("🤖","AI DJ","Programacion automatica."),("💰","Comunitaria","Gestion por la comunidad.")]),

    # NEXUS CONSEJO (#1565c0)
    ("digital-parliament", "Digital Parliament", "🏛️", "Parlamento Digital", "#1565c0",
     "Parlamento digital soberano para sesiones legislativas virtuales.",
     [("🏛️","Sesiones","Sesiones legislativas virtuales."),("🗳️","Votacion","Votacion electronica segura."),("📄","Leyes","Repositorio de legislacion."),("📊","Transparencia","Actas publicas en blockchain."),("💬","Debate","Foros de debate parlamentario."),("📱","Ciudadano","App de participacion ciudadana."),("🤖","AI","Analisis de impacto legislativo."),("🌐","Multilingue","Sesiones en 14 lenguas."),("📋","Comisiones","Gestion de comisiones."),("🔒","Seguro","Autenticacion biometrica.")]),
    ("liquid-democracy", "Liquid Democracy", "🗳️", "Democracia Liquida", "#1565c0",
     "Sistema de democracia liquida con delegacion de voto programable.",
     [("🗳️","Votacion","Voto directo o delegado."),("🤝","Delegacion","Delega tu voto a expertos."),("📊","Transparencia","Resultados en blockchain."),("📱","App","Vota desde el celular."),("🔒","Anonimo","Voto secreto criptografico."),("🏛️","Consultas","Consultas populares frecuentes."),("📋","Propuestas","Cualquier ciudadano propone."),("🌐","Federal","Sistema federal multi-nacion."),("🤖","AI","Analisis de impacto de propuestas."),("💬","Debate","Foro de discusion previo.")]),
    ("digital-justice", "Digital Justice", "⚖️", "Justicia Digital", "#1565c0",
     "Sistema de justicia digital con tribunales virtuales y mediacion AI.",
     [("⚖️","Tribunales","Tribunales virtuales en linea."),("🤖","AI Mediacion","Mediacion asistida por AI."),("📄","Expedientes","Expedientes digitales seguros."),("📊","Estadisticas","Metricas de justicia."),("👨‍⚖️","Jueces","Panel de jueces certificados."),("📱","App","Seguimiento de casos."),("🔒","Confidencial","Cifrado de expedientes."),("🌐","Multilingue","Procesos en 14 lenguas."),("💬","Audiencias","Audiencias por videoconferencia."),("📋","Sentencias","Registro publico de sentencias.")]),
    ("civil-registry", "Civil Registry", "📋", "Registro Civil Digital", "#1565c0",
     "Registro civil digital soberano con identidad verificable en blockchain.",
     [("📋","Registro","Nacimientos, matrimonios, defunciones."),("🔗","Blockchain","Registros inmutables."),("📱","App","Tramites desde el celular."),("🆔","Identidad","Vinculado a identidad FWID."),("📄","Certificados","Certificados digitales verificables."),("🌐","19 Paises","Valido en todo el territorio."),("🤖","AI","Verificacion automatica de datos."),("👥","Familia","Arbol genealogico digital."),("🔒","Privacidad","Datos personales protegidos."),("📊","Estadisticas","Datos demograficos anonimizados.")]),
    ("sovereign-passport", "Sovereign Passport", "🛂", "Pasaporte Soberano", "#1565c0",
     "Pasaporte digital soberano con verificacion biometrica y blockchain.",
     [("🛂","Pasaporte","Documento de identidad digital."),("🔐","Biometrico","Huella, iris y reconocimiento facial."),("🔗","Blockchain","Verificable en blockchain."),("📱","App","Pasaporte en el celular."),("🌐","19 Paises","Reconocido en el territorio."),("✈️","Viaje","Control migratorio digital."),("🤖","AI","Deteccion de fraude."),("📊","Historial","Registro de entradas y salidas."),("🔒","Seguro","Cifrado post-cuantico."),("💰","Gratuito","Sin costo para ciudadanos.")]),
    ("diplomacy", "Diplomacy", "🌐", "Diplomacia Digital", "#1565c0",
     "Plataforma diplomatica para relaciones entre naciones soberanas.",
     [("🌐","Relaciones","Gestion de relaciones diplomaticas."),("📄","Tratados","Repositorio de tratados y acuerdos."),("🤝","Negociacion","Salas de negociacion virtuales."),("🏛️","Embajadas","Embajadas digitales en 19 paises."),("📱","Comunicacion","Canal diplomatico seguro."),("📊","Comercio","Estadisticas de comercio bilateral."),("🔒","Confidencial","Comunicaciones cifradas."),("🌍","ONU","Coordinacion con organismos internacionales."),("💬","Protocolo","Protocolo diplomatico digital."),("📋","Agenda","Calendario de eventos diplomaticos.")]),
    ("immigration", "Immigration", "✈️", "Migracion Soberana", "#1565c0",
     "Sistema de control migratorio digital para el territorio soberano.",
     [("✈️","Control","Puntos de control migratorio digital."),("🛂","Visas","Solicitud y emision de visas."),("📊","Estadisticas","Flujos migratorios en tiempo real."),("🤖","AI","Verificacion automatica de documentos."),("📱","App","Tramites desde el celular."),("🔐","Biometrico","Verificacion biometrica."),("🔗","Blockchain","Registro inmutable de movimientos."),("🌐","19 Paises","Control en todo el territorio."),("👥","Refugio","Gestion de solicitudes de refugio."),("📋","Permisos","Permisos de residencia y trabajo.")]),
    ("electoral-commission", "Electoral Commission", "🗳️", "Comision Electoral", "#1565c0",
     "Comision electoral digital con votacion electronica verificable.",
     [("🗳️","Elecciones","Organizacion de elecciones digitales."),("🔒","Seguro","Votacion criptografica verificable."),("📊","Resultados","Conteo en tiempo real."),("📱","App","Vota desde el celular con FWID."),("🤖","AI","Deteccion de anomalias."),("📋","Candidatos","Registro de candidaturas."),("🌐","Federal","Elecciones a multiples niveles."),("👥","Observadores","Observacion electoral digital."),("📄","Actas","Actas electorales en blockchain."),("💬","Campanas","Regulacion de campanas.")]),
    ("public-audit", "Public Audit", "🔍", "Auditoria Publica", "#1565c0",
     "Sistema de auditoria publica transparente con datos en blockchain.",
     [("🔍","Auditoria","Auditoria de gastos publicos."),("📊","Dashboard","Panel de transparencia fiscal."),("🔗","Blockchain","Registros inmutables de gastos."),("🤖","AI","Deteccion de anomalias financieras."),("📱","Ciudadano","App de consulta publica."),("📄","Reportes","Informes de auditoria automaticos."),("💰","Presupuesto","Seguimiento presupuestario."),("👥","Participacion","Auditoria social participativa."),("🔒","Integridad","Datos a prueba de manipulacion."),("📋","Contratos","Registro publico de contratos.")]),
    ("sovereign-laws", "Sovereign Laws", "📜", "Legislacion Soberana", "#1565c0",
     "Repositorio digital de legislacion soberana con busqueda inteligente.",
     [("📜","Leyes","Compilacion de todas las leyes."),("🔍","Busqueda","Motor de busqueda juridico."),("🤖","AI","Analisis juridico con AI."),("📊","Estadisticas","Metricas de aplicacion."),("📱","App","Consulta movil."),("🌐","14 Lenguas","Legislacion multilingue."),("📋","Codigos","Codigos organizados por materia."),("💬","Comentarios","Anotaciones juridicas."),("📄","Jurisprudencia","Base de datos de sentencias."),("🔗","Blockchain","Registro oficial inmutable.")]),
    ("social-welfare", "Social Welfare", "🤲", "Bienestar Social", "#1565c0",
     "Sistema de gestion de programas de bienestar social digital.",
     [("🤲","Programas","Gestion de programas sociales."),("💰","Subsidios","Distribucion automatica de ayudas."),("📱","App","Solicitud desde el celular."),("📊","Dashboard","Panel de indicadores sociales."),("🤖","AI","Focalizacion inteligente de beneficiarios."),("👥","Registro","Padron de beneficiarios."),("🔗","Blockchain","Trazabilidad de fondos."),("🏥","Salud","Cobertura de salud social."),("🎓","Educacion","Becas y ayudas educativas."),("🔒","Privacidad","Datos personales protegidos.")]),
    ("emergency-911", "Emergency 911", "🆘", "Emergencias 911", "#1565c0",
     "Sistema de respuesta a emergencias con geolocalizacion y AI.",
     [("🆘","911","Numero unico de emergencias."),("📍","GPS","Geolocalizacion automatica."),("🤖","AI","Clasificacion de emergencias con AI."),("🚑","Ambulancia","Despacho de ambulancias."),("🚒","Bomberos","Coordinacion de bomberos."),("👮","Policia","Alerta a fuerzas de seguridad."),("📱","App","Boton de emergencia en app."),("📊","Dashboard","Centro de comando unificado."),("💬","Chat","Comunicacion por texto."),("🌐","Cobertura","Disponible en 19 paises.")]),

    # NEXUS TIERRA (#43a047)
    ("sovereign-agriculture", "Sovereign Agriculture", "🌾", "Agricultura Soberana", "#43a047",
     "Plataforma de gestion agricola con IoT, AI y trazabilidad blockchain.",
     [("🌾","Cultivos","Gestion de cultivos y parcelas."),("🤖","AI","Recomendaciones de siembra con AI."),("📊","IoT","Sensores de humedad y nutrientes."),("🛰️","Satelite","Imagenes satelitales de campos."),("🔗","Trazabilidad","Origen verificable en blockchain."),("💧","Riego","Riego inteligente automatizado."),("🌿","Organico","Certificacion organica digital."),("📱","App","Gestion desde el celular."),("💰","Mercado","Venta directa al consumidor."),("📈","Rendimiento","Analisis de productividad.")]),
    ("sovereign-fishing", "Sovereign Fishing", "🐟", "Pesca Soberana", "#43a047",
     "Plataforma de gestion pesquera sostenible con monitoreo satelital.",
     [("🐟","Pesca","Gestion de flotas pesqueras."),("🛰️","Satelite","Rastreo de embarcaciones."),("📊","Cuotas","Control de cuotas de pesca."),("🌊","Oceano","Monitoreo oceanografico."),("🤖","AI","Prediccion de cardumenes."),("🔗","Trazabilidad","Del mar a la mesa."),("📱","App","Gestion movil."),("💰","Mercado","Venta directa de pescado."),("🛡️","Vedas","Control de periodos de veda."),("📈","Sostenibilidad","Indicadores de sostenibilidad.")]),
    ("sovereign-livestock", "Sovereign Livestock", "🐄", "Ganaderia Soberana", "#43a047",
     "Gestion ganadera digital con trazabilidad y bienestar animal.",
     [("🐄","Ganado","Registro de cabezas de ganado."),("📊","Salud","Historial veterinario digital."),("🔗","Trazabilidad","Blockchain de la granja al plato."),("🤖","AI","Deteccion de enfermedades con AI."),("📱","App","Gestion movil."),("🛰️","GPS","Rastreo de ganado por GPS."),("💰","Mercado","Comercializacion directa."),("🌿","Bienestar","Indicadores de bienestar animal."),("📈","Produccion","Metricas de productividad."),("🧬","Genetica","Gestion genetica del hato.")]),
    ("sovereign-water", "Sovereign Water", "💧", "Agua Soberana", "#43a047",
     "Gestion integral de recursos hidricos con monitoreo IoT.",
     [("💧","Agua","Gestion de recursos hidricos."),("📊","IoT","Sensores de calidad de agua."),("🗺️","Cuencas","Mapeo de cuencas hidricas."),("🤖","AI","Prediccion de sequias."),("📱","App","Reporte ciudadano."),("🔧","Infraestructura","Gestion de acueductos."),("🌊","Rios","Monitoreo de caudales."),("💰","Tarifas","Tarifas justas de agua."),("📈","Consumo","Dashboard de consumo."),("🛡️","Proteccion","Proteccion de fuentes hidricas.")]),
    ("sovereign-energy", "Sovereign Energy", "⚡", "Energia Soberana", "#43a047",
     "Red electrica soberana con energias renovables y smart grid.",
     [("⚡","Energia","Red electrica soberana."),("☀️","Solar","Plantas solares comunitarias."),("💨","Eolica","Parques eolicos."),("📊","Smart Grid","Red inteligente con IoT."),("🔋","Almacenamiento","Baterias de almacenamiento."),("📱","App","Monitoreo de consumo."),("💰","Tarifas","Energia a precio justo."),("🤖","AI","Optimizacion de distribucion."),("🌍","Sostenible","100% renovable."),("📈","Dashboard","Panel de generacion.")]),
    ("sovereign-waste", "Sovereign Waste", "♻️", "Residuos Soberanos", "#43a047",
     "Gestion integral de residuos con reciclaje y economia circular.",
     [("♻️","Reciclaje","Programas de reciclaje comunitario."),("🗑️","Recoleccion","Rutas de recoleccion optimizadas."),("📊","Datos","Metricas de generacion de residuos."),("🤖","AI","Clasificacion automatica."),("📱","App","Reporte de residuos."),("💰","Economia Circular","Valoracion de materiales."),("🏭","Plantas","Plantas de tratamiento."),("🌍","Compostaje","Compostaje comunitario."),("📈","Reduccion","Metas de reduccion."),("👥","Educacion","Programas de concientizacion.")]),
    ("sovereign-environment", "Sovereign Environment", "🌍", "Medio Ambiente", "#43a047",
     "Monitoreo y proteccion del medio ambiente en territorios indigenas.",
     [("🌍","Monitoreo","Vigilancia ambiental continua."),("🌳","Bosques","Monitoreo de cobertura forestal."),("📊","Indicadores","Dashboard de indicadores ambientales."),("🛰️","Satelite","Imagenes satelitales."),("🤖","AI","Deteccion de amenazas ambientales."),("📱","App","Reporte ciudadano."),("⚖️","Regulacion","Normativa ambiental."),("🔗","Blockchain","Creditos de carbono."),("💰","Fondos","Fondo ambiental soberano."),("👥","Guardianes","Red de guardianes ambientales.")]),
    ("sovereign-fauna", "Sovereign Fauna", "🦅", "Fauna Soberana", "#43a047",
     "Proteccion y monitoreo de fauna silvestre en territorios indigenas.",
     [("🦅","Fauna","Registro de especies silvestres."),("📊","Censos","Censos de poblacion animal."),("🛰️","Rastreo","Rastreo GPS de animales."),("🤖","AI","Identificacion automatica por camara."),("📱","App","Reporte de avistamientos."),("🔬","Investigacion","Estudios de biodiversidad."),("🛡️","Proteccion","Programas contra la caza furtiva."),("🌿","Habitat","Conservacion de habitats."),("📈","Tendencias","Tendencias poblacionales."),("👥","Comunidad","Guardianes de la fauna.")]),
    ("sovereign-geology", "Sovereign Geology", "⛰️", "Geologia Soberana", "#43a047",
     "Gestion geologica y minera sostenible en territorios indigenas.",
     [("⛰️","Geologia","Mapeo geologico digital."),("💎","Minerales","Inventario de recursos minerales."),("📊","Datos","Base de datos geoquimicos."),("🛰️","Satelite","Exploracion por teledeteccion."),("🤖","AI","Modelado geologico con AI."),("📱","App","Trabajo de campo digital."),("⚖️","Regulacion","Consulta previa para mineria."),("🌍","Sostenible","Mineria responsable."),("💰","Regalias","Distribucion justa de regalias."),("🔬","Laboratorio","Analisis de muestras.")]),
    ("sovereign-weather", "Sovereign Weather", "🌤️", "Meteorologia Soberana", "#43a047",
     "Servicio meteorologico soberano con prediccion AI para agricultura.",
     [("🌤️","Pronostico","Prediccion a 14 dias."),("🤖","AI","Modelos predictivos con ML."),("📊","Estaciones","Red de estaciones meteorologicas."),("🛰️","Satelite","Imagenes de nubes en tiempo real."),("📱","App","Alertas en el celular."),("🌾","Agricultura","Pronostico especializado para cultivos."),("⚠️","Alertas","Alertas de fenomenos extremos."),("📈","Historico","Datos climaticos historicos."),("🌊","Maritimo","Pronostico para pescadores."),("🌍","19 Paises","Cobertura en todo el territorio.")]),
    ("natural-parks", "Natural Parks", "🏞️", "Parques Naturales", "#43a047",
     "Gestion de parques naturales y areas protegidas indigenas.",
     [("🏞️","Parques","Gestion de areas protegidas."),("🗺️","Mapas","Cartografia de parques."),("🌳","Biodiversidad","Inventario de flora y fauna."),("📱","App","Guia de visitantes."),("📊","Monitoreo","Vigilancia de ecosistemas."),("🏕️","Turismo","Turismo ecologico controlado."),("👥","Guardaparques","Gestion de personal."),("🔥","Incendios","Prevencion de incendios."),("💰","Fondos","Financiamiento de conservacion."),("📈","Indicadores","Metricas de conservacion.")]),

    # NEXUS FORJA (#00e676)
    ("sovereign-ide", "Sovereign IDE", "💻", "Entorno de Desarrollo", "#00e676",
     "IDE soberano basado en web para desarrollo de software.",
     [("💻","Editor","Editor de codigo avanzado."),("🤖","AI Copilot","Asistente AI de codigo."),("🔧","Debug","Depurador integrado."),("📦","Terminal","Terminal integrada."),("🔗","Git","Control de versiones Git."),("🌐","Web","IDE en el navegador."),("📱","Movil","Desarrollo desde tablet."),("💾","Cloud","Workspace en la nube."),("📚","Docs","Documentacion integrada."),("👥","Colaboracion","Pair programming en tiempo real.")]),
    ("ai-agent-dev", "AI Agent Dev", "🤖", "Desarrollo de Agentes AI", "#00e676",
     "Plataforma para crear y desplegar agentes de inteligencia artificial.",
     [("🤖","Agentes","Creacion de agentes AI."),("🧠","Modelos","Modelos pre-entrenados."),("📊","Training","Entrenamiento con datos propios."),("🔗","API","Despliegue via API."),("📱","Chat","Agentes conversacionales."),("💻","No-Code","Constructor visual de agentes."),("📈","Monitoreo","Metricas de rendimiento."),("🔒","Privacidad","Datos soberanos."),("💰","Marketplace","Mercado de agentes."),("📚","Docs","Documentacion y tutoriales.")]),
    ("sovereign-repo", "Sovereign Repo", "📦", "Repositorio de Codigo", "#00e676",
     "Repositorio de codigo soberano estilo GitHub con CI/CD integrado.",
     [("📦","Repos","Repositorios Git ilimitados."),("🔗","CI/CD","Integracion continua."),("📋","Issues","Gestion de incidencias."),("👥","Equipos","Organizacion por equipos."),("🔒","Privado","Repos publicos y privados."),("📊","Analytics","Metricas de desarrollo."),("📄","Wiki","Documentacion por proyecto."),("🤖","AI Review","Code review con AI."),("💻","Codespaces","Desarrollo en la nube."),("📱","App","Gestion movil de repos.")]),
    ("devops-pipeline", "DevOps Pipeline", "🔄", "Pipeline DevOps", "#00e676",
     "Plataforma de CI/CD y DevOps con despliegue automatizado.",
     [("🔄","CI/CD","Integracion y despliegue continuo."),("📦","Docker","Build de contenedores."),("⚙️","K8s","Despliegue en Kubernetes."),("📊","Monitoreo","Observabilidad completa."),("🔒","Seguridad","SAST/SCA integrados."),("📋","Pipeline","Editor visual de pipelines."),("🤖","AI","Optimizacion de builds con AI."),("💻","Runners","Runners auto-escalables."),("📈","Metricas","DORA metrics."),("🌐","Multi-Cloud","Despliegue multi-nube.")]),
    ("sovereign-backend", "Sovereign Backend", "⚙️", "Backend como Servicio", "#00e676",
     "Backend-as-a-Service soberano con API, auth y base de datos.",
     [("⚙️","BaaS","Backend listo para usar."),("🔐","Auth","Autenticacion JWT RS256."),("💾","Database","PostgreSQL y MongoDB."),("🔗","API","REST y GraphQL automaticos."),("📊","Dashboard","Panel de administracion."),("📱","SDK","SDKs para movil y web."),("🔒","Seguridad","Cifrado en reposo y transito."),("📈","Escalable","Auto-scaling horizontal."),("💰","Gratuito","Tier gratuito generoso."),("📚","Docs","Documentacion completa.")]),
    ("automation", "Automation", "⚡", "Automatizacion", "#00e676",
     "Plataforma de automatizacion de flujos de trabajo no-code.",
     [("⚡","Workflows","Flujos de trabajo automaticos."),("🔗","Conectores","500+ integraciones."),("📋","Triggers","Disparadores por evento."),("🤖","AI","Automatizacion inteligente."),("📊","Dashboard","Monitoreo de flujos."),("💻","No-Code","Constructor visual."),("📱","Movil","Gestion desde el celular."),("📈","Metricas","KPIs de automatizacion."),("🔒","Seguro","Credenciales cifradas."),("💰","Ahorro","Reduce trabajo manual 80%.")]),
    ("orchestrator", "Orchestrator", "🎼", "Orquestador de Servicios", "#00e676",
     "Orquestador de microservicios para infraestructura distribuida.",
     [("🎼","Orquestacion","Gestion de 84 microservicios."),("⚙️","K8s","Orquestacion Kubernetes."),("📊","Monitoreo","Health checks en tiempo real."),("🔄","Scaling","Auto-scaling inteligente."),("🔗","Service Mesh","Comunicacion entre servicios."),("📋","Config","Gestion de configuracion."),("🤖","AI","Balanceo de carga con AI."),("💻","Dashboard","Panel de control unificado."),("📈","Metricas","Metricas de rendimiento."),("🔒","Seguro","mTLS entre servicios.")]),
    ("low-code", "Low Code", "🧩", "Plataforma Low-Code", "#00e676",
     "Constructor visual de aplicaciones sin necesidad de programar.",
     [("🧩","Constructor","Arrastra y suelta componentes."),("📱","Apps","Crea apps moviles."),("🌐","Web","Crea sitios web."),("💾","Database","Base de datos visual."),("🔗","API","Conecta APIs externas."),("📊","Dashboard","Dashboards arrastrables."),("🤖","AI","Componentes de AI listos."),("👥","Colaboracion","Equipos de diseno."),("💰","Templates","Plantillas gratuitas."),("📈","Deploy","Publicacion con un click.")]),
    ("ui-design", "UI Design", "🎨", "Diseno de Interfaces", "#00e676",
     "Herramienta de diseno de interfaces con sistema de diseno Ierahkwa.",
     [("🎨","Diseno","Editor de interfaces visual."),("📐","Grid","Sistema de grid responsivo."),("🎭","Componentes","Libreria de componentes."),("📱","Prototipos","Prototipos interactivos."),("👥","Colaboracion","Diseno colaborativo."),("💻","Code Export","Exporta a HTML/CSS."),("📊","Design System","Sistema Ierahkwa integrado."),("🤖","AI","Generacion de UI con AI."),("📋","Assets","Biblioteca de iconos y fuentes."),("🔗","Handoff","Entrega a desarrolladores.")]),
    ("project-mgmt", "Project Mgmt", "📋", "Gestion de Proyectos", "#00e676",
     "Herramienta de gestion de proyectos con metodologias agiles.",
     [("📋","Proyectos","Gestion de proyectos y tareas."),("📊","Kanban","Tableros kanban."),("📈","Gantt","Diagramas de Gantt."),("👥","Equipos","Asignacion de equipos."),("💬","Chat","Comunicacion de equipo."),("📱","App","Gestion movil."),("🤖","AI","Estimacion de esfuerzo con AI."),("📄","Docs","Documentacion de proyecto."),("⏱️","Tiempo","Registro de horas."),("💰","Presupuesto","Control presupuestario.")]),
    ("dev-platform", "Dev Platform", "🔧", "Plataforma de Desarrollo", "#00e676",
     "Plataforma integral de desarrollo con herramientas unificadas.",
     [("🔧","Tools","Suite completa de desarrollo."),("💻","IDE","Editor de codigo integrado."),("📦","Repos","Repositorios de codigo."),("🔄","CI/CD","Pipeline automatizado."),("📊","Analytics","Metricas de desarrollo."),("👥","Equipos","Gestion de equipos."),("🤖","AI","Asistente AI de codigo."),("📱","Movil","Desarrollo movil."),("📚","Docs","Centro de documentacion."),("💰","Free Tier","Tier gratuito para startups.")]),

    # NEXUS URBE (#ff9100)
    ("digital-cadastre", "Digital Cadastre", "🗺️", "Catastro Digital", "#ff9100",
     "Sistema catastral digital con mapeo 3D y blockchain de propiedad.",
     [("🗺️","Catastro","Registro catastral digital."),("📐","3D","Modelos 3D de propiedades."),("🔗","Blockchain","Titulos de propiedad en blockchain."),("🛰️","Satelite","Imagenes satelitales."),("📱","App","Consulta desde el celular."),("📊","Valuacion","Valuacion automatica con AI."),("⚖️","Legal","Marco legal territorial."),("💰","Impuestos","Calculo de impuestos."),("📄","Certificados","Certificados catastrales."),("👥","Consulta","Consulta publica de parcelas.")]),
    ("urban-planning", "Urban Planning", "🏙️", "Planificacion Urbana", "#ff9100",
     "Plataforma de planificacion urbana con gemelo digital de ciudades.",
     [("🏙️","Planificacion","Planes maestros digitales."),("🗺️","SIG","Sistema de informacion geografica."),("🏗️","Proyectos","Gestion de proyectos urbanos."),("🤖","AI","Simulacion de impacto."),("📊","Indicadores","Dashboard de indicadores urbanos."),("📱","Ciudadano","Participacion ciudadana."),("🌍","Sostenible","Desarrollo urbano sostenible."),("🚗","Movilidad","Planificacion de transporte."),("💧","Servicios","Red de servicios publicos."),("📈","Crecimiento","Proyecciones de crecimiento.")]),
    ("sovereign-housing", "Sovereign Housing", "🏠", "Vivienda Soberana", "#ff9100",
     "Plataforma de gestion de vivienda social y construccion comunitaria.",
     [("🏠","Vivienda","Programas de vivienda social."),("📋","Solicitudes","Solicitud de vivienda digital."),("🏗️","Construccion","Seguimiento de obras."),("💰","Financiamiento","Creditos de vivienda."),("📊","Dashboard","Panel de indicadores."),("🗺️","Ubicacion","Mapeo de proyectos."),("📱","App","Tramites moviles."),("👥","Comunidad","Autoconstruccion asistida."),("🌿","Sostenible","Vivienda ecologica."),("📈","Oferta","Inventario de vivienda.")]),
    ("sovereign-transit", "Sovereign Transit", "🚌", "Transporte Soberano", "#ff9100",
     "Sistema de transporte publico inteligente con GPS y pagos WPM.",
     [("🚌","Transporte","Red de transporte publico."),("📍","GPS","Rastreo de unidades en tiempo real."),("📱","App","Horarios y rutas en app."),("💰","WPM","Pago con WAMPUM."),("📊","Dashboard","Centro de control."),("🤖","AI","Optimizacion de rutas."),("🗺️","Mapas","Mapas interactivos."),("⏱️","Tiempos","Tiempos de espera."),("👥","Accesible","Transporte inclusivo."),("📈","Metricas","KPIs de servicio.")]),
    ("sovereign-aviation", "Sovereign Aviation", "✈️", "Aviacion Soberana", "#ff9100",
     "Gestion de aviacion soberana y aeropuertos comunitarios.",
     [("✈️","Aviacion","Gestion de vuelos soberanos."),("🛫","Aeropuertos","Red de aeropuertos comunitarios."),("📊","Control","Control de trafico aereo."),("📱","App","Reserva de vuelos."),("🔒","Seguridad","Protocolos de seguridad."),("💰","Tarifas","Vuelos a precio accesible."),("🛰️","GPS","Navegacion por satelite propio."),("📋","Regulacion","Regulacion aeronautica."),("🤖","AI","Optimizacion de rutas."),("🌍","Regional","Conexion entre 19 paises.")]),
    ("sovereign-maritime", "Sovereign Maritime", "🚢", "Maritimo Soberano", "#ff9100",
     "Gestion de transporte maritimo y puertos soberanos.",
     [("🚢","Maritimo","Flota naval soberana."),("⚓","Puertos","Red de puertos comunitarios."),("📊","Monitoreo","Rastreo de embarcaciones."),("📱","App","Gestion movil."),("🌊","Oceanografia","Datos oceanograficos."),("💰","Comercio","Comercio maritimo."),("🔒","Seguridad","Seguridad maritima."),("📋","Regulacion","Normativa maritima."),("🤖","AI","Optimizacion de rutas."),("🌍","Regional","Puertos en 19 paises.")]),
    ("sovereign-logistics", "Sovereign Logistics", "📦", "Logistica Soberana", "#ff9100",
     "Plataforma logistica para cadena de suministro soberana.",
     [("📦","Logistica","Gestion de cadena de suministro."),("🚛","Transporte","Flota de transporte."),("📊","Tracking","Rastreo en tiempo real."),("🤖","AI","Optimizacion de rutas y cargas."),("📱","App","Gestion movil."),("💰","Costos","Reduccion de costos logisticos."),("🏭","Almacenes","Red de almacenes."),("🔗","Blockchain","Trazabilidad de envios."),("📋","Documentos","Documentacion digital."),("🌐","Regional","Logistica pan-americana.")]),
    ("sovereign-healthcare", "Sovereign Healthcare", "🏥", "Salud Publica", "#ff9100",
     "Sistema de salud publica digital con historia clinica electronica.",
     [("🏥","Salud","Sistema de salud publica."),("📋","Historia Clinica","Expediente electronico."),("💊","Farmacia","Recetas electronicas."),("🤖","AI","Diagnostico asistido por AI."),("📱","App","Telemedicina movil."),("📊","Epidemiologia","Vigilancia epidemiologica."),("🔬","Laboratorio","Resultados en linea."),("💰","Gratuito","Atencion sin costo."),("👥","Personal","Gestion de personal medico."),("🔒","Privacidad","Datos medicos protegidos.")]),
    ("sovereign-education", "Sovereign Education", "📚", "Educacion Publica", "#ff9100",
     "Sistema de educacion publica digital para ciudades inteligentes.",
     [("📚","Educacion","Sistema educativo digital."),("🏫","Escuelas","Red de escuelas digitales."),("👨‍🏫","Docentes","Portal para docentes."),("📱","App","Aula virtual movil."),("📊","Dashboard","Indicadores educativos."),("🤖","AI","Tutor personalizado."),("🌐","14 Lenguas","Educacion multilingue."),("📋","Curriculo","Curriculo soberano."),("🎓","Certificados","Diplomas en blockchain."),("💰","Gratuito","Educacion gratuita universal.")]),
    ("sovereign-university", "Sovereign University", "🎓", "Universidad Soberana", "#ff9100",
     "Universidad digital soberana con programas de grado y posgrado.",
     [("🎓","Universidad","Educacion superior soberana."),("📚","Programas","Grado y posgrado."),("👨‍🏫","Docentes","Plataforma para profesores."),("📱","Virtual","Campus virtual completo."),("📊","Investigacion","Portal de investigacion."),("🤖","AI","Aprendizaje adaptativo."),("🌐","Internacional","Intercambio entre 19 paises."),("📋","Acreditacion","Acreditacion soberana."),("💼","Empleo","Bolsa de trabajo."),("💰","Becas","Becas para indigenas.")]),
    ("digital-census", "Digital Census", "📊", "Censo Digital", "#ff9100",
     "Sistema de censo digital soberano con actualizacion continua.",
     [("📊","Censo","Censo poblacional digital."),("📱","App","Registro desde el celular."),("🗺️","Geografico","Datos georreferenciados."),("🤖","AI","Proyecciones demograficas."),("👥","Poblacion","72M de ciudadanos."),("🏠","Vivienda","Censo de viviendas."),("💼","Economico","Datos socioeconomicos."),("🌐","19 Paises","Cobertura pan-americana."),("🔒","Privacidad","Datos anonimizados."),("📈","Tiempo Real","Actualizacion continua.")]),
    ("sovereign-maps", "Sovereign Maps", "🗺️", "Mapas Soberanos", "#ff9100",
     "Servicio de mapas digitales soberanos con datos propios.",
     [("🗺️","Mapas","Cartografia digital soberana."),("📍","Navegacion","GPS con mapa propio."),("🌐","Territorios","Mapeo de territorios indigenas."),("📱","App","App de navegacion."),("🛰️","Satelite","Vista satelital propia."),("📊","Datos","Datos geoespaciales abiertos."),("🤖","AI","Deteccion de cambios."),("🏔️","3D","Modelos de terreno 3D."),("💰","Gratuito","Sin costo ni tracking."),("🔗","API","API para desarrolladores.")]),

    # NEXUS RAICES (#00FF41)
    ("digital-museum", "Digital Museum", "🏛️", "Museo Digital", "#00FF41",
     "Museo digital de arte y cultura indigena de las Americas.",
     [("🏛️","Museo","Galeria virtual 3D."),("🎨","Arte","Obras de arte indigena."),("📷","Fotografias","Archivo fotografico historico."),("🌐","Virtual","Recorridos virtuales 360."),("📱","App","Visita desde el celular."),("🤖","AI","Guia virtual inteligente."),("📚","Catalogos","Catalogos digitales."),("🎭","Exposiciones","Exposiciones temporales."),("💰","Gratuito","Acceso libre y gratuito."),("👥","Comunidad","Contribuciones comunitarias.")]),
    ("sovereign-library", "Sovereign Library", "📚", "Biblioteca Soberana", "#00FF41",
     "Biblioteca digital soberana con millones de libros y documentos.",
     [("📚","Libros","Millones de libros digitales."),("🔍","Busqueda","Motor de busqueda avanzado."),("🌐","14 Lenguas","Contenido multilingue."),("📱","App","Lectura offline."),("🤖","AI","Recomendaciones personalizadas."),("📰","Periodicos","Hemeroteca digital."),("📄","Documentos","Documentos historicos."),("👥","Prestamo","Prestamo digital."),("📊","Catalogo","Catalogo colectivo."),("💰","Gratuito","Acceso libre universal.")]),
    ("cultural-heritage", "Cultural Heritage", "🏺", "Patrimonio Cultural", "#00FF41",
     "Preservacion y difusion del patrimonio cultural indigena.",
     [("🏺","Patrimonio","Registro de patrimonio cultural."),("📷","Digitalizacion","Digitalizacion 3D de artefactos."),("🌐","Virtual","Recorridos virtuales."),("📚","Archivo","Archivo cultural digital."),("🤖","AI","Restauracion digital con AI."),("📱","App","Guia cultural movil."),("🗺️","Rutas","Rutas patrimoniales."),("👥","Comunidad","Preservacion comunitaria."),("📊","Inventario","Inventario nacional."),("💰","Fondos","Financiamiento de preservacion.")]),
    ("ancestral-wisdom", "Ancestral Wisdom", "🌿", "Sabiduria Ancestral", "#00FF41",
     "Repositorio de sabiduria ancestral y conocimiento tradicional.",
     [("🌿","Sabiduria","Conocimiento ancestral."),("👴","Ancianos","Ensenanzas de ancianos."),("🎙️","Oral","Tradicion oral grabada."),("🌍","Cosmovisiones","Cosmovisiones indigenas."),("📚","Textos","Textos sagrados digitalizados."),("🤖","AI","Busqueda semantica."),("📱","App","Acceso movil."),("🌿","Medicina","Medicina tradicional."),("👥","Comunidad","Contribuciones comunitarias."),("🔒","Sagrado","Proteccion de conocimiento sagrado.")]),
    ("sovereign-sports", "Sovereign Sports", "⚽", "Deportes Soberanos", "#00FF41",
     "Plataforma deportiva para juegos y deportes indigenas.",
     [("⚽","Deportes","Liga deportiva soberana."),("🏟️","Eventos","Calendario de eventos."),("📊","Estadisticas","Stats de jugadores y equipos."),("📱","App","Seguimiento en vivo."),("🎮","E-Sports","Liga de videojuegos."),("🏃","Tradicional","Juegos tradicionales indigenas."),("💰","Patrocinio","Patrocinios comunitarios."),("📺","Streaming","Transmision en vivo."),("🏆","Torneos","Organizacion de torneos."),("👥","Clubes","Gestion de clubes.")]),
    ("sovereign-enterprise", "Sovereign Enterprise", "💼", "Empresa Soberana", "#00FF41",
     "Suite empresarial para PyMEs indigenas con herramientas completas.",
     [("💼","ERP","Sistema de planificacion empresarial."),("📊","Contabilidad","Contabilidad y finanzas."),("💰","Facturacion","Facturacion electronica."),("📦","Inventario","Control de inventario."),("👥","RRHH","Gestion de personal."),("📱","App","Gestion movil."),("🤖","AI","Analisis predictivo de negocios."),("📈","Reportes","Reportes financieros."),("🔗","Integracion","Conecta con marketplace."),("🌐","Multi-pais","Operacion en 19 paises.")]),
    ("sovereign-hosting", "Sovereign Hosting", "🖥️", "Hospedaje Soberano", "#00FF41",
     "Servicio de hosting web soberano en infraestructura propia.",
     [("🖥️","Hosting","Servidores web soberanos."),("🌐","Dominios","Registro de dominios .nation."),("💾","Storage","Almacenamiento SSD."),("📊","Dashboard","Panel de control."),("🔒","SSL","Certificados SSL gratuitos."),("📱","App","Gestion movil."),("⚡","CDN","Red de distribucion de contenido."),("📧","Email","Correo electronico incluido."),("🤖","AI","Optimizacion automatica."),("💰","Accesible","Precios justos.")]),
    ("sovereign-marketing", "Sovereign Marketing", "📢", "Marketing Soberano", "#00FF41",
     "Herramientas de marketing digital etico sin tracking invasivo.",
     [("📢","Marketing","Herramientas de marketing etico."),("📧","Email","Email marketing."),("📊","Analytics","Metricas anonimizadas."),("🤖","AI","Contenido generado con AI."),("📱","Social","Gestion de redes sociales."),("🎨","Diseno","Plantillas de diseno."),("💰","SEO","Optimizacion para buscadores."),("📋","Campanas","Gestion de campanas."),("👥","CRM","Relacion con clientes."),("🔒","Etico","Marketing sin manipulacion.")]),
    ("sovereign-commerce", "Sovereign Commerce", "🛒", "Comercio Soberano", "#00FF41",
     "Plataforma de comercio electronico soberana con 3% de comision.",
     [("🛒","E-Commerce","Tienda en linea soberana."),("💰","3% Fee","Comision justa del 3%."),("📦","Logistica","Envios integrados."),("💳","Pagos","Pagos con WPM."),("📱","App","Compra movil."),("📊","Dashboard","Panel para vendedores."),("🤖","AI","Recomendaciones de productos."),("⭐","Resenas","Sistema de calificaciones."),("🌐","19 Paises","Mercado pan-americano."),("🔒","Seguro","Transacciones protegidas.")]),
    ("sovereign-jobs", "Sovereign Jobs", "💼", "Empleo Soberano", "#00FF41",
     "Bolsa de trabajo soberana conectando talento indigena con empleo.",
     [("💼","Empleos","Miles de ofertas laborales."),("📋","CV","Constructor de curriculum."),("🤖","AI","Match inteligente candidato-empleo."),("📱","App","Busca empleo desde el celular."),("🎓","Capacitacion","Cursos de preparacion."),("💰","Freelance","Trabajos independientes."),("🌐","Remoto","Trabajo remoto pan-americano."),("📊","Mercado","Analisis del mercado laboral."),("👥","Red","Networking profesional."),("🏢","Empresas","Portal para empleadores.")]),
    ("economic-analytics", "Economic Analytics", "📈", "Analitica Economica", "#00FF41",
     "Dashboard de indicadores economicos del ecosistema soberano.",
     [("📈","Economia","Dashboard macroeconomico."),("📊","PIB","PIB soberano en tiempo real."),("💰","Comercio","Balanza comercial."),("🏦","Finanzas","Indicadores financieros."),("🤖","AI","Predicciones economicas."),("📱","App","Consulta movil."),("🌐","19 Paises","Datos de todo el territorio."),("📋","Reportes","Informes periodicos."),("👥","Empleo","Estadisticas de empleo."),("🔍","Investigacion","Datos para investigadores.")]),
]

TEMPLATE = '''<!DOCTYPE html><html lang="es"><head><meta charset="UTF-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<meta name="description" content="Plataforma soberana de {pretty} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="{domain}/{dirname}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{pretty} — {subtitle}">
<meta property="og:description" content="Plataforma soberana de {pretty} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<meta property="og:type" content="website">
<meta property="og:url" content="{domain}/{dirname}/">
<meta property="og:image" content="{domain}/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{pretty} — {subtitle}">
<meta name="twitter:description" content="Plataforma soberana de {pretty} para la infraestructura digital de 574 naciones tribales - Ierahkwa Ne Kanienke">
<link rel="stylesheet" href="../shared/ierahkwa.css"><title>{pretty} — {subtitle}</title>
<style>:root{{--accent:{accent}}}</style>
</head><body>
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header><div class="logo"><div class="logo-icon" aria-hidden="true">{initial}</div><h1>{pretty}</h1></div><nav aria-label="Navegacion principal"><a href="#" aria-current="page">Dashboard</a><a href="#">Servicios</a><a href="#">Red</a></nav></header>
<main id="main">
<section class="hero"><div class="badge"><span aria-hidden="true">{emoji}</span> {subtitle}</div><h2>{desc_short}</h2><p>{description}</p></section>
<div class="stats" role="list" aria-label="Estadisticas clave"><div class="stat" role="listitem"><div class="val">574</div><div class="lbl">Naciones</div></div><div class="stat" role="listitem"><div class="val">72M</div><div class="lbl">Personas</div></div><div class="stat" role="listitem"><div class="val">100%</div><div class="lbl">Soberano</div></div><div class="stat" role="listitem"><div class="val">19</div><div class="lbl">Paises</div></div></div>
<div class="section-title"><h3>Modulos de la Plataforma</h3><p>10 herramientas soberanas</p></div>
<div class="grid">
{cards}
</div>
</main>
<footer><p><span aria-hidden="true">{emoji}</span> {pretty} &mdash; Ecosistema Digital <a href="../">Ierahkwa</a> &mdash; 72M personas, 19 naciones soberanas</p></footer>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body></html>'''

CARD = '<article class="card"><div class="card-icon" aria-hidden="true">{emoji}</div><h4>{title}</h4><p>{desc}</p></article>'

created = 0
skipped = 0

for p in PLATFORMS:
    dirname, pretty, emoji, subtitle, accent, description, features = p
    dirpath = os.path.join(BASE, dirname)
    filepath = os.path.join(dirpath, "index.html")

    if os.path.isfile(filepath):
        skipped += 1
        continue

    os.makedirs(dirpath, exist_ok=True)

    cards = "\n".join(CARD.format(emoji=f[0], title=f[1], desc=f[2]) for f in features)
    initial = pretty[0].upper()
    desc_short = subtitle

    html = TEMPLATE.format(
        pretty=pretty, dirname=dirname, emoji=emoji, subtitle=subtitle,
        accent=accent, description=description, desc_short=desc_short,
        initial=initial, cards=cards, domain=DOMAIN
    )

    with open(filepath, "w", encoding="utf-8") as f:
        f.write(html)

    created += 1
    print(f"  [OK] {dirname}")

print(f"\n=== Completado ===")
print(f"  Creados: {created}")
print(f"  Omitidos (ya existen): {skipped}")
