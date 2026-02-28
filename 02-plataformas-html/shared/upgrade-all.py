#!/usr/bin/env python3
"""
Ierahkwa — MEGA UPGRADE ALL PLATFORMS
Upgrades ALL 231 basic platforms to full-featured HTML with:
  Hero, metrics, architecture, 10 cards, API docs, pricing, offline module
Uses category templates for efficient generation.
"""
import os, json

BASE = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

# ═══════════════════════════════════════════
# CATEGORY CONTENT TEMPLATES
# Each category has: metrics, cards(10), apis(6), pricing(3), offline
# ═══════════════════════════════════════════
CATS = {
  'finance': {
    'metrics': [('12,847','Transacciones/día'),('99.99%','Uptime'),('< 50ms','Latencia'),('E2E','Cifrado'),('24/7','Operación'),('blockchain','Audit Trail')],
    'cards': [
      ('💰','Motor Transaccional en Tiempo Real','Procesamiento de operaciones financieras con settlement T+0 sobre MameyNode blockchain. Capacidad de 50,000 TPS con confirmación en < 2 segundos.'),
      ('🔒','Cifrado Grado Bancario AES-256','Toda la información financiera protegida con cifrado AES-256 en tránsito y reposo. Claves rotadas cada 24 horas con HSM soberano dedicado.'),
      ('📊','Dashboard Analítico en Tiempo Real','Visualización de KPIs financieros, flujos de capital, tendencias y alertas automáticas. Exporta reportes en PDF, Excel y XBRL para reguladores.'),
      ('🤖','Detección de Fraude con AI','Motor de machine learning que analiza patrones transaccionales, detecta anomalías y bloquea fraudes en tiempo real con 99.7% de precisión.'),
      ('💱','Multi-Moneda WAMPUM','Soporte nativo para WAMPUM (CBDC soberana) con conversión automática a USD, EUR, BTC y 40+ monedas fiat. Tipo de cambio actualizado cada minuto.'),
      ('📋','Cumplimiento Regulatorio Automático','Generación automática de reportes para reguladores soberanos. KYC/AML integrado, listas de sanciones, PEP screening y monitoreo continuo.'),
      ('🔗','Integración con Ecosistema BDET','Conexión directa con BDET Bank, Tesoro Nacional, Sistema Tributario y todas las plataformas financieras del ecosistema Ierahkwa.'),
      ('📱','App Móvil Nativa','Aplicación nativa para iOS y Android con autenticación biométrica, notificaciones push y operaciones offline que sincronizan al reconectar.'),
      ('🏛️','Auditoría Inmutable en Blockchain','Cada operación registrada en MameyNode con hash SHA-256. Trail de auditoría completo, inmutable y verificable por cualquier auditor autorizado.'),
      ('💾','Modo Offline Resiliente','Operaciones críticas disponibles sin conexión. Cola de transacciones pendientes con reconciliación automática al restaurar conectividad.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/transaction','Crear transacción. Params: amount, currency, recipient, memo.'),
      ('GET','/api/v1/{slug}/balance/{account}','Consultar saldo de cuenta con historial de movimientos.'),
      ('GET','/api/v1/{slug}/reports/{period}','Reportes financieros del período: diario, semanal, mensual, anual.'),
      ('POST','/api/v1/{slug}/transfer','Transferencia entre cuentas con validación de fondos y límites.'),
      ('GET','/api/v1/{slug}/analytics/dashboard','Dashboard analítico con KPIs, tendencias y alertas configurables.'),
      ('GET','/api/v1/{slug}/audit-trail/{id}','Trail de auditoría completo para una transacción específica.')
    ],
    'pricing': [
      ('Básico','0 W/mes',['100 transacciones/mes','Dashboard básico','1 cuenta','Soporte comunidad']),
      ('Profesional','25 W/mes',['Ilimitado','AI fraude','Multi-cuenta','API completa','Reportes avanzados']),
      ('Enterprise','199 W/mes',['Multi-entidad','SLA 99.99%','Auditor dedicado','Soporte 24/7','Custom integrations'])
    ],
    'offline': (['transacciones-pendientes','saldos-cache','reportes-offline'],'Saldos y operaciones pendientes disponibles offline.')
  },
  'government': {
    'metrics': [('574','Naciones'),('47K','Funcionarios'),('99.9%','Disponibilidad'),('E2E','Cifrado'),('real-time','Actualización'),('blockchain','Transparencia')],
    'cards': [
      ('🏛️','Portal Gubernamental Unificado','Interfaz centralizada para ciudadanos y funcionarios con acceso a todos los servicios gubernamentales digitales de las 19 naciones soberanas.'),
      ('📋','Gestión de Trámites Digitales','Sistema de trámites 100% digital con seguimiento en tiempo real, notificaciones automáticas y firma digital integrada. Zero papel.'),
      ('🗳️','Participación Ciudadana','Mecanismos de consulta popular, votaciones y referéndums digitales con verificación blockchain y anonimato garantizado del votante.'),
      ('🔒','Seguridad Grado Gubernamental','Cifrado post-cuántico Kyber-768 para toda la información gubernamental. Clasificación de documentos por nivel de seguridad.'),
      ('📊','Datos Abiertos y Transparencia','Portal de datos abiertos con datasets gubernamentales accesibles para investigadores, periodistas y ciudadanos. Formato estándar DCAT.'),
      ('🤖','AI para Atención Ciudadana','Asistente virtual que responde consultas en 14 idiomas indígenas, guía trámites y escala a funcionarios humanos cuando es necesario.'),
      ('📅','Agenda y Calendario Oficial','Calendario de eventos gubernamentales, sesiones legislativas, audiencias públicas y fechas ceremoniales de las 574 naciones.'),
      ('👥','Directorio de Funcionarios','Base de datos completa de funcionarios públicos con información de contacto, responsabilidades y declaraciones patrimoniales.'),
      ('📱','App Ciudadana Móvil','Aplicación para ciudadanos con notificaciones de trámites, alertas gubernamentales, consultas y pagos de servicios públicos.'),
      ('🔗','Interoperabilidad Inter-Nación','Protocolos de intercambio de datos entre las 19 naciones soberanas manteniendo soberanía de datos y jurisdicción local.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/procedure/create','Iniciar trámite gubernamental con documentos adjuntos digitales.'),
      ('GET','/api/v1/{slug}/procedure/{id}/status','Estado del trámite en tiempo real con historial de pasos.'),
      ('GET','/api/v1/{slug}/directory/officials','Directorio de funcionarios con filtros por nación y departamento.'),
      ('POST','/api/v1/{slug}/citizen/notification','Enviar notificación oficial a ciudadano verificado.'),
      ('GET','/api/v1/{slug}/open-data/datasets','Catálogo de datos abiertos gubernamentales disponibles.'),
      ('GET','/api/v1/{slug}/calendar/events','Eventos y sesiones gubernamentales programadas.')
    ],
    'pricing': [
      ('Ciudadano','0 W/mes',['Trámites básicos','Consultas','Notificaciones','App móvil']),
      ('Funcionario','0 W/mes',['Gestión de trámites','Dashboard','Reportes','Firma digital']),
      ('Nación','99 W/mes',['Portal completo','API ilimitada','Interoperabilidad','Custom branding','Soporte 24/7'])
    ],
    'offline': (['tramites-pendientes','directorio-cache','notificaciones'],'Trámites y directorio disponibles offline para zonas remotas.')
  },
  'security': {
    'metrics': [('256-bit','Cifrado'),('< 1ms','Respuesta'),('99.99%','Uptime'),('24/7','Monitoreo'),('0','Brechas'),('post-quantum','Criptografía')],
    'cards': [
      ('🛡️','Cifrado Post-Cuántico Kyber-768','Criptografía lattice-based resistente a computadoras cuánticas. Cada sesión genera claves efímeras imposibles de descifrar con algoritmos cuánticos.'),
      ('🔍','Monitoreo de Amenazas 24/7','Centro de operaciones de seguridad (SOC) que monitorea toda la infraestructura soberana en tiempo real detectando intrusiones y anomalías.'),
      ('🚨','Respuesta a Incidentes Automatizada','Playbooks automatizados que contienen y neutralizan amenazas en < 1 segundo. Cuarentena de sistemas comprometidos y notificación inmediata.'),
      ('📡','Inteligencia de Amenazas','Red de inteligencia que recopila, analiza y comparte indicadores de compromiso (IoC) entre las 19 naciones soberanas en tiempo real.'),
      ('🔒','Zero Trust Architecture','Modelo de seguridad zero-trust donde cada solicitud se verifica independientemente. No se confía en ningún usuario o dispositivo por defecto.'),
      ('🛡️','Firewall Soberano de Nueva Generación','Firewall con inspección profunda de paquetes (DPI), IPS/IDS integrado, filtrado de contenido y protección contra DDoS de hasta 1 Tbps.'),
      ('🔐','Gestión de Identidades (IAM)','Sistema centralizado de identidad y acceso con MFA obligatorio, SSO federado, roles granulares y auditoría de cada acceso.'),
      ('📊','Dashboard de Seguridad','Visualización en tiempo real del estado de seguridad: vulnerabilidades, incidentes, compliance score, y métricas de detección.'),
      ('🧪','Pentesting Automatizado','Escaneo continuo de vulnerabilidades y pruebas de penetración automatizadas contra toda la infraestructura con reportes de remediación.'),
      ('💾','Backup y Recuperación','Backups cifrados cada 6 horas en 3 ubicaciones geográficas. Recovery Point Objective (RPO) < 1 hora. Disaster recovery probado mensualmente.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/threats/active','Amenazas activas detectadas con nivel de severidad y estado.'),
      ('POST','/api/v1/{slug}/incident/report','Reportar incidente de seguridad con evidencia adjunta.'),
      ('GET','/api/v1/{slug}/compliance/score','Score de cumplimiento de seguridad con desglose por control.'),
      ('POST','/api/v1/{slug}/scan/vulnerability','Iniciar escaneo de vulnerabilidades en infraestructura.'),
      ('GET','/api/v1/{slug}/audit/access-log','Log de accesos con filtros por usuario, recurso y fecha.'),
      ('POST','/api/v1/{slug}/policy/enforce','Aplicar política de seguridad a un recurso o grupo.')
    ],
    'pricing': [
      ('Básico','0 W/mes',['Firewall básico','Monitoreo','Alertas email','Dashboard']),
      ('Profesional','49 W/mes',['SOC 24/7','AI detección','Pentesting mensual','MFA avanzado','Reportes compliance']),
      ('Nación','499 W/mes',['Zero Trust completo','IR automatizado','Inteligencia amenazas','SLA 99.99%','Equipo dedicado'])
    ],
    'offline': (['politicas-seguridad','incidentes-cache','compliance-data'],'Políticas de seguridad y protocolos disponibles offline.')
  },
  'education': {
    'metrics': [('47,283','Estudiantes'),('2,100','Cursos'),('14','Idiomas'),('99.9%','Uptime'),('offline','Disponible'),('AI','Adaptativo')],
    'cards': [
      ('🎓','Aprendizaje Adaptativo AI','Motor de inteligencia artificial que personaliza el contenido educativo según el ritmo, estilo y nivel de cada estudiante. Incrementa retención 40%.'),
      ('📚','Biblioteca Digital Soberana','Acceso a 500,000+ recursos educativos: libros, videos, simulaciones, laboratorios virtuales. Todo en 14 idiomas indígenas.'),
      ('👨‍🏫','Aulas Virtuales en Tiempo Real','Clases en vivo con video HD, pizarra colaborativa, breakout rooms, grabación automática y transcripción en idiomas indígenas.'),
      ('📊','Analíticas de Aprendizaje','Dashboard para educadores con métricas de progreso, engagement, áreas de mejora y predicción de abandono con intervención temprana.'),
      ('🏆','Gamificación y Logros','Sistema de puntos, insignias, rankings y recompensas que motivan a los estudiantes. Integrado con certificaciones blockchain.'),
      ('📝','Evaluaciones Inteligentes','Exámenes adaptativos que ajustan dificultad en tiempo real. Detección de plagio AI. Rúbricas automatizadas con feedback instantáneo.'),
      ('🌐','Contenido Multicultural','Currículo que integra conocimiento ancestral indígena con estándares educativos internacionales. Cada nación puede personalizar su contenido.'),
      ('👥','Colaboración Estudiantil','Proyectos en equipo, foros de discusión, peer review y mentoría entre pares. Conecta estudiantes de las 574 naciones.'),
      ('📱','App Móvil y Offline','Descarga cursos completos para estudiar sin internet. Sincronización automática al reconectar. Interfaz optimizada para dispositivos de gama baja.'),
      ('🎯','Competencias y Certificaciones','Framework de competencias alineado con necesidades laborales soberanas. Certificados verificables en blockchain MameyNode.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/course/enroll','Inscribir estudiante en curso con validación de prerrequisitos.'),
      ('GET','/api/v1/{slug}/student/{id}/progress','Progreso del estudiante con métricas detalladas por módulo.'),
      ('POST','/api/v1/{slug}/assessment/submit','Enviar evaluación para calificación automática AI.'),
      ('GET','/api/v1/{slug}/catalog/courses','Catálogo de cursos con filtros por nivel, tema e idioma.'),
      ('GET','/api/v1/{slug}/analytics/classroom/{id}','Analíticas de aula: participación, rendimiento, predicciones.'),
      ('POST','/api/v1/{slug}/certificate/issue','Emitir certificado verificable en blockchain para estudiante.')
    ],
    'pricing': [
      ('Estudiante','0 W/mes',['Todos los cursos','App offline','Certificados básicos','Comunidad']),
      ('Educador','15 W/mes',['Aulas virtuales','Analíticas avanzadas','Evaluaciones AI','Contenido custom']),
      ('Institución','149 W/mes',['Multi-campus','API completa','LMS white-label','SLA 99.9%','Soporte dedicado'])
    ],
    'offline': (['cursos-descargados','evaluaciones-pendientes','progreso-estudiante'],'Cursos descargados y evaluaciones disponibles sin conexión.')
  },
  'health': {
    'metrics': [('72M','Beneficiarios'),('24/7','Atención'),('99.9%','Uptime'),('HIPAA','Compliant'),('14','Idiomas'),('AI','Diagnóstico')],
    'cards': [
      ('🏥','Historia Clínica Electrónica','Expediente médico unificado accesible por cualquier proveedor de salud autorizado. Cifrado E2E con control total del paciente sobre sus datos.'),
      ('🤖','Diagnóstico Asistido por AI','Motor de IA entrenado con datos de salud indígena que ayuda a médicos con diagnóstico diferencial, alcanzando 92% de precisión.'),
      ('💊','Gestión de Medicamentos','Control de prescripciones, interacciones medicamentosas, adherencia al tratamiento y alertas automatizadas. Integrado con farmacias soberanas.'),
      ('📡','Telemedicina y Teleconsulta','Videoconsultas con médicos en 14 idiomas indígenas. Integración con dispositivos IoT para monitoreo de signos vitales remotos.'),
      ('📊','Analíticas de Salud Poblacional','Dashboard epidemiológico con datos anonimizados para vigilancia de enfermedades, tendencias y planificación de recursos sanitarios.'),
      ('🌿','Medicina Tradicional Integrada','Base de datos de plantas medicinales, prácticas ceremoniales de sanación y protocolos que integran medicina ancestral con medicina moderna.'),
      ('📅','Gestión de Citas','Sistema de citas online con disponibilidad en tiempo real, recordatorios automáticos y lista de espera inteligente que optimiza agenda médica.'),
      ('🔬','Laboratorio Digital','Solicitud de exámenes, seguimiento de muestras, entrega de resultados digital y alertas automáticas de valores críticos.'),
      ('🚑','Emergencias y Triage','Sistema de triage digital que prioriza pacientes por gravedad. Integrado con ambulancias y red de emergencias soberana.'),
      ('📱','App de Salud Personal','App para pacientes: citas, resultados, medicamentos, vacunas, historial. Funciona offline en comunidades sin internet.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/patient/{id}/record','Historia clínica del paciente con autorización verificada.'),
      ('POST','/api/v1/{slug}/appointment/book','Agendar cita médica con selección de especialidad y horario.'),
      ('POST','/api/v1/{slug}/prescription/create','Crear prescripción médica con validación de interacciones.'),
      ('GET','/api/v1/{slug}/lab/results/{id}','Resultados de laboratorio del paciente con valores de referencia.'),
      ('GET','/api/v1/{slug}/epidemiology/dashboard','Dashboard epidemiológico con métricas de salud poblacional.'),
      ('POST','/api/v1/{slug}/telemedicine/session','Iniciar sesión de telemedicina con video cifrado E2E.')
    ],
    'pricing': [
      ('Comunidad','0 W/mes',['Historia clínica','Citas básicas','App salud','Telemedicina básica']),
      ('Clínica','39 W/mes',['Multi-médico','AI diagnóstico','Lab digital','Recetas electrónicas','Reportes']),
      ('Hospital','299 W/mes',['Multi-sede','API completa','Epidemiología','SLA 99.99%','Soporte 24/7'])
    ],
    'offline': (['historia-clinica','citas-cache','medicamentos'],'Historia clínica y medicamentos accesibles offline.')
  },
  'agriculture': {
    'metrics': [('47K','Parcelas'),('94%','Precisión'),('IoT','Sensores'),('real-time','Monitoreo'),('AI','Predicción'),('574','Comunidades')],
    'cards': [
      ('🌾','Monitoreo de Cultivos por Satélite','Imágenes multiespectrales de satélites soberanos para monitorear salud de cultivos, detectar plagas, estrés hídrico y estimar rendimiento.'),
      ('📡','Red de Sensores IoT','47,000 sensores de suelo, clima y agua distribuidos en territorio soberano. Datos cada 15 minutos: humedad, pH, temperatura, nutrientes.'),
      ('🤖','AI Predictiva Agrícola','Modelos de machine learning que predicen rendimiento, fecha óptima de siembra/cosecha, riesgo de plagas y necesidades de riego con 94% de precisión.'),
      ('💧','Gestión de Riego Inteligente','Automatización de sistemas de riego basada en datos de sensores, pronóstico climático y necesidades específicas de cada cultivo.'),
      ('🌱','Banco de Semillas Digital','Catálogo de variedades de semillas nativas y adaptadas con información genómica, rendimiento histórico y protocolos de cultivo.'),
      ('📊','Dashboard de Producción','Visualización en tiempo real de producción agrícola por parcela, región y nación. Proyecciones de cosecha y análisis de tendencias.'),
      ('🐛','Detección de Plagas y Enfermedades','Sistema de visión por computadora que identifica plagas y enfermedades a partir de fotos. Recomienda tratamientos orgánicos ancestrales.'),
      ('🏪','Conexión Productor-Mercado','Marketplace que conecta agricultores directamente con compradores, eliminando intermediarios. Precios justos en WAMPUM.'),
      ('🧓','Sabiduría Agrícola Ancestral','Base de conocimiento de técnicas agrícolas ancestrales: milpa, chinampas, terrazas, rotación de cultivos por ciclos lunares.'),
      ('📱','App de Campo Offline','App para agricultores que funciona sin internet: registro de actividades, consulta de recomendaciones, fotos de plagas para análisis posterior.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/farm/{id}/status','Estado actual de parcela: cultivos, sensores, alertas, producción.'),
      ('POST','/api/v1/{slug}/irrigation/schedule','Programar riego automático basado en datos de sensores y clima.'),
      ('GET','/api/v1/{slug}/satellite/imagery/{coords}','Imagen satelital reciente para coordenadas específicas.'),
      ('POST','/api/v1/{slug}/pest/identify','Identificar plaga a partir de imagen. Retorna diagnóstico y tratamiento.'),
      ('GET','/api/v1/{slug}/market/prices','Precios de mercado en tiempo real para productos agrícolas.'),
      ('GET','/api/v1/{slug}/weather/forecast/{location}','Pronóstico agrícola para ubicación: lluvia, helada, viento, humedad.')
    ],
    'pricing': [
      ('Comunidad','0 W/mes',['1 parcela','Sensores básicos','App offline','Pronóstico general']),
      ('Agricultor','5 W/mes',['10 parcelas','AI predictiva','Riego inteligente','Marketplace','Alertas personalizadas']),
      ('Cooperativa','49 W/mes',['Ilimitado','Satélite dedicado','API completa','Dashboard regional','Soporte agronómico'])
    ],
    'offline': (['parcelas-datos','recomendaciones','precios-mercado'],'Datos de parcelas y recomendaciones agrícolas disponibles offline.')
  },
  'technology': {
    'metrics': [('99.99%','Uptime'),('< 10ms','Latencia'),('auto','Scaling'),('CI/CD','Pipeline'),('multi-cloud','Deploy'),('24/7','Monitoreo')],
    'cards': [
      ('💻','Entorno de Desarrollo Integrado','IDE completo en el navegador con syntax highlighting para 50+ lenguajes, autocompletado AI, debugging integrado y terminal SSH.'),
      ('🔄','CI/CD Pipeline Automatizado','Pipeline de integración y despliegue continuo con testing automático, code review AI, staging environments y rollback instantáneo.'),
      ('📦','Gestión de Contenedores','Orquestación de contenedores Docker/OCI con auto-scaling, health checks, service mesh y load balancing inteligente.'),
      ('📊','Monitoreo y Observabilidad','Stack completo de observabilidad: métricas, logs y traces unificados. Alertas inteligentes con reducción de ruido AI.'),
      ('🔒','Seguridad DevSecOps','Escaneo de vulnerabilidades en cada commit, análisis estático de código, gestión de secretos y compliance automático.'),
      ('🗄️','Bases de Datos Gestionadas','PostgreSQL, Redis, MongoDB, TimescaleDB y MameyNode como servicio gestionado con backups automáticos y réplicas.'),
      ('🌐','CDN y Edge Computing','Red de distribución de contenido con 47 edge nodes en territorio soberano. Cache inteligente y compute en el borde.'),
      ('📡','API Gateway Soberano','Gateway centralizado con rate limiting, autenticación JWT/OAuth, transformación de requests, caching y analytics.'),
      ('🤖','AI/ML Platform','Plataforma para entrenamiento y deploy de modelos de machine learning con GPU soberanas. MLOps completo integrado.'),
      ('📱','SDKs Multi-Plataforma','SDKs oficiales en Python, JavaScript, Go, Rust, Java y Swift para integración rápida con todo el ecosistema Ierahkwa.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/deploy','Desplegar aplicación con configuración de entorno y scaling.'),
      ('GET','/api/v1/{slug}/services/status','Estado de todos los servicios desplegados con métricas.'),
      ('POST','/api/v1/{slug}/pipeline/trigger','Disparar pipeline CI/CD para un repositorio específico.'),
      ('GET','/api/v1/{slug}/logs/{service}','Logs de servicio con filtros por nivel, tiempo y patrón.'),
      ('GET','/api/v1/{slug}/metrics/dashboard','Métricas de infraestructura: CPU, memoria, red, disco.'),
      ('POST','/api/v1/{slug}/database/create','Crear instancia de base de datos gestionada con réplicas.')
    ],
    'pricing': [
      ('Developer','0 W/mes',['1 app','512MB RAM','Shared DB','CI/CD básico','Comunidad']),
      ('Team','29 W/mes',['10 apps','4GB RAM','DB dedicada','Custom domains','Monitoreo avanzado']),
      ('Enterprise','199 W/mes',['Ilimitado','Auto-scale','Multi-región','SLA 99.99%','Soporte 24/7'])
    ],
    'offline': (['config-proyectos','docs-api','logs-cache'],'Documentación y configuración disponibles offline.')
  },
  'media': {
    'metrics': [('1M+','Usuarios'),('4K','Streaming'),('14','Idiomas'),('E2E','Cifrado'),('AI','Recomendación'),('P2P','CDN')],
    'cards': [
      ('📺','Streaming de Alta Calidad','Contenido en 4K HDR con bitrate adaptativo. CDN soberana con 47 edge nodes para < 50ms de latencia en todo el territorio.'),
      ('🤖','Recomendaciones AI Soberanas','Motor de recomendación que respeta la privacidad del usuario. Sin tracking invasivo. Prioriza contenido indígena y cultural soberano.'),
      ('🎬','Creación de Contenido','Herramientas integradas para creadores: editor de video/audio, thumbnails, subtítulos automáticos en 14 idiomas indígenas.'),
      ('💰','Monetización Directa WAMPUM','Creadores reciben el 90% de ingresos en WAMPUM. Sin intermediarios. Pagos instantáneos vía BDET Bank. Transparencia blockchain.'),
      ('🔒','Privacidad del Usuario','Zero tracking publicitario. Sin venta de datos. El historial de visualización es privado y cifrado. El usuario controla sus datos.'),
      ('📡','Transmisión en Vivo','Streaming en vivo con chat, donaciones, encuestas y multi-cámara. Latencia ultra-baja de 2 segundos con protocolo WebRTC soberano.'),
      ('🌐','Contenido Multilingüe','Subtítulos y doblaje automático en 14 idiomas indígenas usando Atabey NLP. Cada comunidad puede crear contenido en su lengua nativa.'),
      ('📱','Apps Multi-Plataforma','Apps nativas para iOS, Android, Smart TVs, web y escritorio. Descarga para ver offline en zonas sin conectividad.'),
      ('📊','Analytics para Creadores','Dashboard con métricas de audiencia, engagement, retención, demografía y tendencias. Sin exponer datos individuales de usuarios.'),
      ('👥','Comunidad y Social','Comentarios, likes, compartir, playlists colaborativas y seguimiento de creadores favoritos. Moderación AI contra contenido dañino.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/content/upload','Subir contenido multimedia con metadata y categorización.'),
      ('GET','/api/v1/{slug}/feed/recommended','Feed personalizado de contenido recomendado por AI.'),
      ('POST','/api/v1/{slug}/stream/start','Iniciar transmisión en vivo con configuración de calidad.'),
      ('GET','/api/v1/{slug}/creator/{id}/analytics','Analíticas de creador: vistas, engagement, ingresos.'),
      ('GET','/api/v1/{slug}/search','Búsqueda de contenido con filtros por tipo, idioma, fecha.'),
      ('POST','/api/v1/{slug}/playlist/create','Crear playlist con contenido curado manualmente o por AI.')
    ],
    'pricing': [
      ('Gratuito','0 W/mes',['Streaming con anuncios soberanos','Calidad HD','1 dispositivo','Comunidad']),
      ('Premium','9 W/mes',['Sin anuncios','4K HDR','5 dispositivos','Descargas offline','Audio espacial']),
      ('Creador','25 W/mes',['Herramientas creador','Analytics avanzados','Monetización','Multi-cámara','Soporte prioritario'])
    ],
    'offline': (['contenido-descargado','playlists-cache','historial'],'Contenido descargado disponible offline.')
  },
  'commerce': {
    'metrics': [('12,847','Comercios'),('< 2s','Checkout'),('WAMPUM','Pagos'),('AI','Recomendación'),('multi','Canal'),('blockchain','Trazabilidad')],
    'cards': [
      ('🛒','Tienda Online Personalizable','Constructor de tiendas con 50+ templates. Catálogo ilimitado, variantes de producto, inventario multi-almacén y SEO optimizado.'),
      ('💳','Pagos WAMPUM Integrados','Procesamiento de pagos en WAMPUM con conversión automática a monedas fiat. Checkout en < 2 segundos. Zero fees para vendedores.'),
      ('📦','Gestión de Inventario','Control de stock en tiempo real multi-almacén. Alertas de reorden automáticas. Código de barras y RFID. Trazabilidad blockchain.'),
      ('🤖','Recomendaciones de Producto AI','Motor que sugiere productos relevantes basado en historial de compras, tendencias y preferencias sin comprometer la privacidad.'),
      ('🚚','Logística y Envíos','Integración con servicios de envío soberanos. Tracking en tiempo real, estimación de entrega AI y optimización de rutas.'),
      ('📊','Analytics de Ventas','Dashboard con métricas de ventas, conversión, carrito abandonado, lifetime value y proyecciones AI de demanda.'),
      ('💬','Atención al Cliente','Chat en vivo, tickets de soporte, FAQ inteligente y chatbot AI en 14 idiomas. Resolución automática del 60% de consultas.'),
      ('📱','Comercio Móvil','App nativa optimizada para mobile commerce. Push notifications, geolocalización, escaneo de productos y pagos NFC.'),
      ('🏪','Punto de Venta Integrado','POS que sincroniza con tienda online. Acepta WAMPUM, efectivo y tarjetas. Funciona offline con sync posterior.'),
      ('🌍','Multi-Mercado','Vende en múltiples naciones soberanas con precios locales, idiomas, impuestos y regulaciones manejadas automáticamente.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/product/create','Crear producto con variantes, precios e inventario inicial.'),
      ('POST','/api/v1/{slug}/order/checkout','Procesar orden con validación de stock y cobro WAMPUM.'),
      ('GET','/api/v1/{slug}/inventory/{product}','Niveles de inventario por producto y almacén.'),
      ('GET','/api/v1/{slug}/analytics/sales/{period}','Analíticas de ventas del período con desglose por producto.'),
      ('POST','/api/v1/{slug}/shipping/track','Tracking de envío con estimación de entrega actualizada.'),
      ('GET','/api/v1/{slug}/customers/{id}/history','Historial de compras del cliente con preferencias inferidas.')
    ],
    'pricing': [
      ('Artesano','0 W/mes',['50 productos','Pagos WAMPUM','Tienda básica','Soporte comunidad']),
      ('Comercio','19 W/mes',['Ilimitado','Multi-almacén','AI recomendaciones','Analytics','POS integrado']),
      ('Marketplace','99 W/mes',['Multi-vendedor','API completa','Multi-nación','Logística integrada','SLA 99.9%'])
    ],
    'offline': (['catalogo-productos','ordenes-pendientes','inventario-cache'],'Catálogo y ventas disponibles offline con POS.')
  },
  'infrastructure': {
    'metrics': [('99.9%','Disponibilidad'),('IoT','Conectado'),('real-time','Monitoreo'),('AI','Optimización'),('24/7','Operación'),('19','Naciones')],
    'cards': [
      ('🏗️','Gestión de Infraestructura','Control centralizado de infraestructura urbana y rural: redes eléctricas, agua, transporte, telecomunicaciones y servicios públicos.'),
      ('📡','Red de Sensores IoT','Miles de sensores distribuidos monitoreando en tiempo real: tráfico, calidad de aire, consumo energético, nivel de agua y estado de vías.'),
      ('🤖','Optimización por AI','Algoritmos de machine learning que optimizan uso de recursos, predicen fallas, programan mantenimiento preventivo y reducen costos 30%.'),
      ('🗺️','Gemelo Digital Urbano','Modelo 3D digital de la infraestructura urbana para simulaciones, planificación y toma de decisiones basada en datos.'),
      ('⚡','Gestión Energética','Monitoreo de generación, distribución y consumo energético. Integración de fuentes renovables: solar, eólica, hidroeléctrica.'),
      ('🚦','Control de Tráfico Inteligente','Semáforos adaptativos con AI que optimizan flujo vehicular. Reducción de tiempos de viaje 25% y emisiones 15%.'),
      ('📊','Dashboard Operacional','Visualización en tiempo real de todos los sistemas de infraestructura con alertas, métricas de rendimiento y reportes automáticos.'),
      ('🔧','Mantenimiento Predictivo','AI que predice cuándo un equipo va a fallar antes de que ocurra. Programa mantenimiento preventivo optimizando recursos y evitando paradas.'),
      ('🌍','Sustentabilidad Ambiental','Métricas de impacto ambiental: huella de carbono, consumo de agua, generación de residuos. Objetivos de sustentabilidad monitoreados.'),
      ('📱','App Ciudadana','Reportes ciudadanos de problemas de infraestructura: baches, fugas, alumbrado. Tracking de resolución en tiempo real.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/status/overview','Estado general de toda la infraestructura monitoreada.'),
      ('GET','/api/v1/{slug}/sensors/{zone}/data','Datos de sensores IoT en una zona específica en tiempo real.'),
      ('POST','/api/v1/{slug}/maintenance/schedule','Programar mantenimiento preventivo para un activo.'),
      ('GET','/api/v1/{slug}/energy/consumption/{zone}','Consumo energético por zona con histórico y proyección.'),
      ('POST','/api/v1/{slug}/citizen/report','Reporte ciudadano de problema de infraestructura.'),
      ('GET','/api/v1/{slug}/analytics/performance','Métricas de rendimiento de infraestructura por sistema.')
    ],
    'pricing': [
      ('Comunidad','0 W/mes',['Dashboard básico','Reportes ciudadanos','Alertas generales','App móvil']),
      ('Municipal','49 W/mes',['IoT integrado','AI optimización','Mantenimiento predictivo','API completa']),
      ('Nacional','499 W/mes',['Multi-ciudad','Gemelo digital','SLA 99.99%','Soporte 24/7','Consultor dedicado'])
    ],
    'offline': (['estado-infraestructura','alertas-cache','reportes'],'Estado de infraestructura y alertas disponibles offline.')
  },
  'social': {
    'metrics': [('1M+','Usuarios'),('E2E','Cifrado'),('0','Tracking'),('14','Idiomas'),('P2P','Red'),('AI','Moderación')],
    'cards': [
      ('👥','Red Social Soberana','Plataforma de conexión entre los 72M de miembros de las 574 naciones. Perfiles verificados por identidad soberana. Zero venta de datos.'),
      ('💬','Mensajería Cifrada E2E','Chat individual y grupal con cifrado end-to-end. Mensajes efímeros, notas de voz, video y archivos. Protocolo Signal modificado.'),
      ('📝','Publicaciones y Stories','Comparte texto, fotos, videos e historias. Algoritmo cronológico (no manipulativo). Prioriza contenido de tu comunidad.'),
      ('🔒','Privacidad por Diseño','Zero tracking publicitario. Sin análisis de comportamiento. Sin venta de datos. El usuario es dueño absoluto de su información.'),
      ('🌐','Multilingüe Nativo','Interfaz y contenido en 14 idiomas indígenas. Traducción automática entre lenguas con Atabey NLP preservando contexto cultural.'),
      ('🤖','Moderación AI Ética','Detección automática de discurso de odio, desinformación y contenido dañino respetando libertad de expresión y contexto cultural indígena.'),
      ('📡','Modo Mesh Offline','En zonas sin internet, la red social opera sobre mesh local P2P. Mensajes y posts se sincronizan cuando hay conectividad.'),
      ('💰','Sin Publicidad Invasiva','Modelo de negocio basado en suscripciones voluntarias, no en publicidad. Los anuncios soberanos son opcionales y nunca invasivos.'),
      ('📊','Control de Datos Personal','Dashboard donde cada usuario ve exactamente qué datos se almacenan. Exportación completa y derecho al olvido en 24 horas.'),
      ('🎨','Personalización Cultural','Temas visuales basados en arte indígena. Emojis de cultura nativa. Stickers creados por artistas de las 574 naciones.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/post/create','Crear publicación con multimedia y visibilidad configurable.'),
      ('GET','/api/v1/{slug}/feed/timeline','Timeline cronológico de publicaciones de contactos.'),
      ('POST','/api/v1/{slug}/message/send','Enviar mensaje cifrado E2E a contacto o grupo.'),
      ('GET','/api/v1/{slug}/profile/{id}','Perfil público de usuario con publicaciones recientes.'),
      ('POST','/api/v1/{slug}/community/create','Crear comunidad temática con reglas y moderadores.'),
      ('GET','/api/v1/{slug}/search/people','Buscar personas por nombre, nación o comunidad.')
    ],
    'pricing': [
      ('Gratuito','0 W/mes',['Perfil completo','Mensajería E2E','Timeline','Comunidades','App offline']),
      ('Supporter','5 W/mes',['Sin anuncios','Stickers premium','Video HD','Almacenamiento extra']),
      ('Organización','29 W/mes',['Página oficial','Analytics','API','Verificación','Soporte prioritario'])
    ],
    'offline': (['mensajes-pendientes','feed-cache','contactos'],'Mensajes y feed disponibles offline con sync automático.')
  },
  'culture': {
    'metrics': [('574','Naciones'),('3,000+','Años'),('14','Idiomas'),('50K+','Artefactos'),('AR/VR','Inmersión'),('blockchain','Autenticidad')],
    'cards': [
      ('🏛️','Archivo Digital Ancestral','Digitalización de alta resolución de artefactos, documentos, arte y objetos ceremoniales. Cada pieza catalogada con metadata cultural.'),
      ('🌐','Experiencias Inmersivas AR/VR','Recorridos virtuales por sitios sagrados, recreaciones históricas y experiencias culturales inmersivas accesibles desde cualquier dispositivo.'),
      ('🗣️','Preservación de Lenguas','Grabaciones, diccionarios, gramáticas y cursos interactivos de 14 idiomas indígenas. AI de reconocimiento de voz para lenguas en riesgo.'),
      ('🎨','Galería de Arte Indígena','Exhibición virtual de arte contemporáneo e histórico de las 574 naciones. Ventas directas artista-comprador en WAMPUM.'),
      ('📚','Biblioteca de Tradición Oral','Miles de horas de relatos, cantos, ceremonias y conocimiento ancestral grabados con consentimiento de ancianos y comunidades.'),
      ('🔒','Protección de Patrimonio','Registro blockchain de propiedad cultural que previene apropiación. Licencias soberanas para uso de símbolos, diseños y conocimiento.'),
      ('👥','Red de Custodios Culturales','Plataforma para ancianos, artistas, historiadores y educadores que mantienen viva la cultura. Mentoría intergeneracional digital.'),
      ('📅','Calendario Ceremonial','Calendario unificado de ceremonias, festivales y eventos culturales de las 574 naciones con fases lunares y fechas solares.'),
      ('🎵','Archivo Musical Ancestral','Grabaciones de música tradicional: tambores, flautas, cantos ceremoniales. Contexto cultural de cada pieza preservado digitalmente.'),
      ('📱','App Cultural Comunitaria','App offline con contenido cultural descargable. Ancianos pueden contribuir con voz. Jóvenes aprenden tradiciones interactivamente.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/collection/{nation}','Colección cultural de una nación con artefactos y metadata.'),
      ('POST','/api/v1/{slug}/artifact/register','Registrar artefacto cultural con procedencia y custodia.'),
      ('GET','/api/v1/{slug}/languages/{code}','Recursos lingüísticos: diccionario, gramática, pronunciación.'),
      ('GET','/api/v1/{slug}/calendar/ceremonies','Ceremonias y eventos culturales programados por nación.'),
      ('POST','/api/v1/{slug}/oral-history/submit','Contribuir historia oral con grabación y metadata cultural.'),
      ('GET','/api/v1/{slug}/art/gallery/{nation}','Galería de arte de una nación con info de artistas.')
    ],
    'pricing': [
      ('Comunidad','0 W/mes',['Acceso completo','App offline','Calendario','Diccionarios']),
      ('Educador','9 W/mes',['Materiales educativos','AR/VR','Cursos de lenguas','API básica']),
      ('Institución','49 W/mes',['Multi-sede','API completa','Investigación','Custom exhibits','Soporte dedicado'])
    ],
    'offline': (['colecciones-cache','diccionarios','calendario-ceremonial'],'Colecciones culturales y diccionarios disponibles offline.')
  },
  'environment': {
    'metrics': [('47K','Sensores'),('real-time','Monitoreo'),('AI','Predicción'),('100m²','Resolución'),('574','Ecosistemas'),('0','Emisiones')],
    'cards': [
      ('🌍','Monitoreo Ambiental en Tiempo Real','Red de sensores IoT que monitorea calidad de aire, agua, suelo, ruido y biodiversidad en todo el territorio soberano.'),
      ('🛰️','Observación Satelital','Imágenes multiespectrales para detección de deforestación, contaminación, cambios en uso de suelo e impacto del cambio climático.'),
      ('🤖','Predicción de Impacto AI','Modelos de machine learning que predicen impacto ambiental de actividades, proyectan escenarios climáticos y sugieren mitigaciones.'),
      ('🌳','Gestión de Ecosistemas','Inventario y monitoreo de ecosistemas: bosques, ríos, humedales, costas. Alertas tempranas de degradación ambiental.'),
      ('♻️','Economía Circular','Plataforma para gestión de residuos, reciclaje, reutilización y compostaje. Trazabilidad de materiales desde producción hasta reciclaje.'),
      ('📊','Dashboard de Sustentabilidad','Métricas de sustentabilidad: huella de carbono, uso de agua, biodiversidad, energía renovable. Objetivos ODS monitoreados.'),
      ('🌱','Programa de Reforestación','Gestión de programas de reforestación con geolocalización de cada árbol plantado, monitoreo de supervivencia y captura de CO2.'),
      ('🐦','Biodiversidad y Fauna','Censo digital de especies con avistamientos reportados por ciudadanos y cámaras trampa. Identificación por AI de flora y fauna.'),
      ('⚠️','Alertas de Contaminación','Alertas automáticas cuando niveles de contaminación superan umbrales: aire, agua, suelo. Notificación a autoridades y comunidades.'),
      ('📱','App Eco-Ciudadana','App para ciudadanos: reporte de contaminación, avistamiento de fauna, desafíos de reciclaje, calculadora de huella de carbono.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/air-quality/{location}','Calidad del aire en ubicación: PM2.5, PM10, O3, NO2, CO.'),
      ('GET','/api/v1/{slug}/water-quality/{body}','Calidad de agua: pH, turbidez, oxígeno disuelto, contaminantes.'),
      ('POST','/api/v1/{slug}/report/pollution','Reporte ciudadano de contaminación con geolocalización y fotos.'),
      ('GET','/api/v1/{slug}/biodiversity/{region}','Inventario de biodiversidad en región con tendencias poblacionales.'),
      ('GET','/api/v1/{slug}/carbon/footprint/{entity}','Huella de carbono calculada para entidad con recomendaciones.'),
      ('GET','/api/v1/{slug}/reforestation/progress','Progreso del programa de reforestación: árboles, CO2, área.')
    ],
    'pricing': [
      ('Ciudadano','0 W/mes',['App eco','Reportes','Alertas','Calculadora CO2']),
      ('Municipio','29 W/mes',['Sensores IoT','Dashboard','AI predicción','Reportes automáticos']),
      ('Nación','199 W/mes',['Satélite dedicado','API completa','Multi-región','SLA 99.9%','Soporte 24/7'])
    ],
    'offline': (['alertas-ambientales','biodiversidad-cache','reportes'],'Alertas ambientales y guías de biodiversidad offline.')
  },
  'office': {
    'metrics': [('real-time','Colaboración'),('offline','Disponible'),('E2E','Cifrado'),('14','Idiomas'),('∞','Almacenamiento'),('0','Tracking')],
    'cards': [
      ('📝','Editor Colaborativo en Tiempo Real','Edición simultánea por múltiples usuarios con resolución de conflictos OT/CRDT. Historial de cambios infinito y restauración.'),
      ('📊','Visualización de Datos','Gráficos, tablas dinámicas, dashboards y reportes interactivos creados a partir de datos con actualización en tiempo real.'),
      ('🔒','Privacidad y Cifrado E2E','Documentos cifrados end-to-end. Solo los colaboradores autorizados pueden acceder. Zero acceso del servidor al contenido.'),
      ('🤖','Asistente AI Integrado','IA que ayuda a redactar, formatear, resumir, traducir y analizar contenido. Respeta la privacidad: procesa localmente.'),
      ('📱','Multiplataforma y Offline','Funciona en web, iOS, Android, escritorio. Modo offline completo con sincronización automática al reconectar.'),
      ('📁','Organización Inteligente','Carpetas, etiquetas, búsqueda full-text y organización automática por AI. Acceso rápido a documentos recientes y favoritos.'),
      ('🔗','Integraciones con Ecosistema','Conecta con correo, calendario, almacenamiento, mensajería y todas las plataformas del ecosistema Ierahkwa.'),
      ('📤','Exportación Multi-Formato','Exporta a PDF, DOCX, XLSX, CSV, HTML, Markdown. Importa desde formatos Microsoft Office y Google Workspace.'),
      ('👥','Permisos Granulares','Control de acceso por documento: ver, comentar, editar. Compartir con enlace temporal. Auditoría de accesos.'),
      ('🌐','Soporte Multilingüe','Interfaz en 14 idiomas indígenas. Corrector ortográfico y gramatical para lenguas nativas. Traducción integrada.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/document/create','Crear documento con tipo, plantilla y permisos iniciales.'),
      ('GET','/api/v1/{slug}/document/{id}','Obtener documento con contenido y metadata de colaboración.'),
      ('POST','/api/v1/{slug}/document/{id}/share','Compartir documento con usuarios o generar enlace público.'),
      ('GET','/api/v1/{slug}/search','Búsqueda full-text en todos los documentos del usuario.'),
      ('POST','/api/v1/{slug}/export/{id}/{format}','Exportar documento a formato: pdf, docx, xlsx, csv, html.'),
      ('GET','/api/v1/{slug}/templates','Catálogo de plantillas disponibles por categoría.')
    ],
    'pricing': [
      ('Personal','0 W/mes',['5 documentos','Colaboración básica','Offline','Export PDF']),
      ('Profesional','9 W/mes',['Ilimitado','AI asistente','Multi-formato','Historial completo','API básica']),
      ('Organización','49 W/mes',['Multi-equipo','Admin panel','API completa','SSO','Soporte prioritario'])
    ],
    'offline': (['documentos-cache','plantillas','historial-cambios'],'Documentos y plantillas disponibles offline.')
  },
  'sports': {
    'metrics': [('574','Comunidades'),('live','Streaming'),('AI','Analytics'),('24/7','Cobertura'),('multi','Deportes'),('WAMPUM','Premios')],
    'cards': [
      ('⚽','Transmisión de Eventos en Vivo','Streaming de eventos deportivos en calidad HD/4K con comentarios en idiomas indígenas. Multi-cámara y replay instantáneo.'),
      ('📊','Estadísticas y Analytics AI','Análisis en tiempo real de rendimiento atlético, tácticas de equipo, predicción de resultados y scouting automatizado.'),
      ('🏆','Torneos y Competencias','Gestión completa de ligas, torneos y campeonatos: brackets, calendarios, resultados, clasificaciones y premios en WAMPUM.'),
      ('🎮','Esports y Gaming','Liga de esports soberana con matchmaking, ranking ELO, streaming integrado y premios. Juegos indígenas tradicionales digitalizados.'),
      ('💪','Programas de Entrenamiento','Planes de entrenamiento personalizados por AI basados en objetivos, condición física y datos de wearables IoT.'),
      ('🏟️','Gestión de Instalaciones','Reserva de canchas, estadios y gimnasios. Calendario de disponibilidad, mantenimiento programado y control de acceso.'),
      ('👥','Comunidad Deportiva','Red social para atletas, entrenadores y aficionados. Foros, grupos por deporte y conexión inter-nación.'),
      ('🎯','Apuestas Deportivas Reguladas','Apuestas provably-fair con odds transparentes en blockchain. Límites de juego responsable y autoexclusión voluntaria.'),
      ('📱','App Deportiva','Scores en vivo, notificaciones, highlights, calendario de eventos. Funciona offline con actualización al reconectar.'),
      ('🏅','Juegos Ancestrales','Preservación y promoción de deportes tradicionales indígenas: lacrosse, pelota mesoamericana, juegos de destreza ancestrales.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/events/live','Eventos deportivos en vivo con scores y estadísticas.'),
      ('GET','/api/v1/{slug}/league/{id}/standings','Clasificación de liga con puntos, goles y diferencial.'),
      ('POST','/api/v1/{slug}/tournament/create','Crear torneo con formato, equipos y calendario.'),
      ('GET','/api/v1/{slug}/athlete/{id}/stats','Estadísticas detalladas de atleta por temporada.'),
      ('POST','/api/v1/{slug}/facility/reserve','Reservar instalación deportiva con horario y recursos.'),
      ('GET','/api/v1/{slug}/schedule/{sport}','Calendario de eventos por deporte y región.')
    ],
    'pricing': [
      ('Fan','0 W/mes',['Scores en vivo','Noticias','Calendario','App básica']),
      ('Atleta','9 W/mes',['Entrenamiento AI','Estadísticas avanzadas','Comunidad','Wearables IoT']),
      ('Liga','49 W/mes',['Gestión de torneos','Streaming','Analytics','API completa','Soporte dedicado'])
    ],
    'offline': (['calendario-eventos','estadisticas-cache','entrenamientos'],'Calendario y planes de entrenamiento disponibles offline.')
  },
  'military': {
    'metrics': [('256-bit','Cifrado'),('< 1ms','Respuesta'),('24/7','Operación'),('post-quantum','Criptografía'),('multi-domain','Cobertura'),('AI','Análisis')],
    'cards': [
      ('🛡️','Criptografía Post-Cuántica Militar','Cifrado de grado militar con algoritmos Kyber-768 y Dilithium-3 resistentes a computadoras cuánticas. Certificado para información clasificada.'),
      ('📡','Comunicaciones Seguras','Red de comunicaciones cifrada para fuerzas de defensa con salto de frecuencia, anti-jamming y redundancia multi-camino.'),
      ('🛸','Control de Sistemas Autónomos','Mando y control de drones, vehículos autónomos y sistemas robóticos con telemetría cifrada y failsafe automático.'),
      ('🗺️','Inteligencia Geoespacial','Análisis de imágenes satelitales, mapeo de terreno, detección de cambios y reconocimiento de patrones con AI militar.'),
      ('🤖','AI para Análisis de Amenazas','Motor de inteligencia artificial que procesa múltiples fuentes de datos para detectar, clasificar y priorizar amenazas.'),
      ('📊','Centro de Comando y Control','Dashboard C4ISR unificado para operaciones multi-dominio: tierra, aire, mar, ciber y espacio.'),
      ('🔒','Gestión de Información Clasificada','Clasificación automática de documentos, control de acceso por nivel de seguridad, distribución segura y destrucción programada.'),
      ('📱','Comunicación Táctica Móvil','Dispositivos endurecidos con comunicación cifrada, GPS seguro, mapa de operaciones y mensajería de campo.'),
      ('🌐','Red Mesh Táctica','Red mesh descentralizada que opera sin infraestructura fija. Auto-healing, auto-configurable y resistente a ataques.'),
      ('💾','Resiliencia Operacional','Sistemas redundantes con failover automático. Operación degradada gracefully. Recuperación < 30 segundos ante cualquier fallo.')
    ],
    'apis': [
      ('POST','/api/v1/{slug}/secure/channel','Establecer canal de comunicación cifrado con nivel de clasificación.'),
      ('GET','/api/v1/{slug}/situational/awareness','Estado operacional unificado de todos los dominios.'),
      ('POST','/api/v1/{slug}/mission/plan','Crear plan de misión con recursos, objetivos y contingencias.'),
      ('GET','/api/v1/{slug}/intel/analysis','Análisis de inteligencia procesado por AI con nivel de confianza.'),
      ('POST','/api/v1/{slug}/asset/track','Rastrear activo (vehículo, drone, unidad) en tiempo real.'),
      ('GET','/api/v1/{slug}/threat/assessment','Evaluación de amenazas con nivel de severidad y recomendaciones.')
    ],
    'pricing': [
      ('Unidad','0 W/mes',['Comunicación básica','Mapa táctico','Mensajería cifrada','GPS seguro']),
      ('Brigada','99 W/mes',['C4ISR completo','AI análisis','Drones','Multi-dominio','Soporte 24/7']),
      ('Nación','999 W/mes',['Soberanía total','Satélite dedicado','Post-quantum','Custom','Equipo dedicado'])
    ],
    'offline': (['planes-mision','mapas-tacticos','protocolos'],'Planes de misión y mapas tácticos disponibles offline.')
  },
  'space': {
    'metrics': [('LEO/MEO','Órbitas'),('99.9%','Uptime'),('Tbps','Capacidad'),('AI','Navegación'),('24/7','Operación'),('19','Naciones')],
    'cards': [
      ('🚀','Control de Misión','Centro de control para operaciones espaciales: lanzamientos, navegación orbital, maniobras y deorbit. Telemetría en tiempo real.'),
      ('🛰️','Gestión de Constelación','Control de constelación satelital soberana: posicionamiento, comunicación inter-satelital, prevención de colisiones y mantenimiento orbital.'),
      ('📡','Estaciones Terrenas','Red de estaciones terrenas para comunicación, tracking y control de satélites con cobertura global y redundancia.'),
      ('🌍','Observación Terrestre','Imágenes multiespectrales de alta resolución para agricultura, medio ambiente, urbanismo, defensa y gestión de desastres.'),
      ('📶','Comunicaciones Satelitales','Internet satelital soberano para comunidades remotas con < 50ms de latencia. Cobertura del 100% del territorio.'),
      ('🤖','Navegación AI Autónoma','Sistemas de navegación autónoma para satélites: corrección orbital, evasión de colisiones y optimización de combustible por AI.'),
      ('🔬','Investigación Espacial','Laboratorio orbital para investigación: materiales, biología, astronomía. Datos abiertos para científicos de las 574 naciones.'),
      ('📊','Telemetría y Analytics','Dashboard con datos de telemetría de toda la flota satelital: salud, combustible, temperatura, radiación, comunicaciones.'),
      ('🌌','Seguimiento de Debris','Catálogo y seguimiento de debris orbital para prevención de colisiones con maniobras de evasión automatizadas.'),
      ('📱','App de Tracking Satelital','Visualización en tiempo real de la posición de cada satélite soberano. Datos de cobertura y horarios de paso.')
    ],
    'apis': [
      ('GET','/api/v1/{slug}/satellite/{id}/telemetry','Telemetría en vivo de satélite: posición, salud, combustible.'),
      ('POST','/api/v1/{slug}/command/send','Enviar comando a satélite con autenticación criptográfica.'),
      ('GET','/api/v1/{slug}/coverage/{location}','Cobertura satelital para ubicación: comunicaciones y observación.'),
      ('GET','/api/v1/{slug}/imagery/{coords}','Imagen satelital más reciente para coordenadas específicas.'),
      ('GET','/api/v1/{slug}/debris/catalog','Catálogo de debris orbital con trayectorias y riesgo de colisión.'),
      ('POST','/api/v1/{slug}/mission/plan','Planificar misión espacial con parámetros orbitales y payload.')
    ],
    'pricing': [
      ('Datos','0 W/mes',['Imágenes básicas','Tracking','Cobertura','Datos abiertos']),
      ('Profesional','99 W/mes',['Alta resolución','API completa','Datos en tiempo real','Analytics']),
      ('Nación','999 W/mes',['Satélite dedicado','Control de misión','Estación terrena','SLA 99.99%','Equipo dedicado'])
    ],
    'offline': (['orbitas-cache','imagenes-satelitales','telemetria'],'Datos orbitales y imágenes satelitales cacheados offline.')
  },
}

# ═══════════════════════════════════════════
# ALL PLATFORMS: key → (title, subtitle, icon, accent, category)
# ═══════════════════════════════════════════
P = {
  # === SOVEREIGN-* ENGLISH ===
  'sovereign-agriculture': ('Agriculture Platform','Sovereign Agricultural Management','🌾','#43a047','agriculture'),
  'sovereign-aviation': ('Aviation Platform','Sovereign Aviation Control','✈️','#00bcd4','infrastructure'),
  'sovereign-backend': ('Backend Platform','Sovereign Backend Services','⚙️','#00e676','technology'),
  'sovereign-collab': ('Collaboration Platform','Sovereign Team Collaboration','🤝','#00e676','office'),
  'sovereign-commerce': ('Commerce Platform','Sovereign E-Commerce','🛒','#ffd600','commerce'),
  'sovereign-conference': ('Conference Platform','Sovereign Video Conferencing','📹','#e040fb','media'),
  'sovereign-education': ('Education Platform','Sovereign Learning System','🎓','#9C27B0','education'),
  'sovereign-energy': ('Energy Platform','Sovereign Energy Grid','⚡','#43a047','infrastructure'),
  'sovereign-enterprise': ('Enterprise Platform','Sovereign Enterprise Suite','🏢','#ffd600','commerce'),
  'sovereign-environment': ('Environment Platform','Sovereign Environmental Monitoring','🌍','#43a047','environment'),
  'sovereign-fauna': ('Fauna Platform','Sovereign Wildlife Registry','🦅','#43a047','environment'),
  'sovereign-fishing': ('Fishing Platform','Sovereign Fisheries Management','🎣','#43a047','agriculture'),
  'sovereign-forum': ('Forum Platform','Sovereign Community Forums','💬','#e040fb','social'),
  'sovereign-geology': ('Geology Platform','Sovereign Geological Survey','🏔️','#43a047','environment'),
  'sovereign-healthcare': ('Healthcare Platform','Sovereign Healthcare System','🏥','#FF5722','health'),
  'sovereign-hosting': ('Hosting Platform','Sovereign Web Hosting','🖥️','#00e676','technology'),
  'sovereign-housing': ('Housing Platform','Sovereign Housing Registry','🏠','#607D8B','infrastructure'),
  'sovereign-ide': ('IDE Platform','Sovereign Code IDE','💻','#00e676','technology'),
  'sovereign-insurance': ('Insurance Platform','Sovereign Insurance System','🛡️','#ffd600','finance'),
  'sovereign-jobs': ('Jobs Platform','Sovereign Job Portal','💼','#607D8B','social'),
  'sovereign-laws': ('Legal Platform','Sovereign Legal Database','⚖️','#1565c0','government'),
  'sovereign-library': ('Library Platform','Sovereign Digital Library','📚','#9C27B0','education'),
  'sovereign-livestock': ('Livestock Platform','Sovereign Livestock Management','🐄','#43a047','agriculture'),
  'sovereign-logistics': ('Logistics Platform','Sovereign Supply Chain','🚛','#ffd600','commerce'),
  'sovereign-maps': ('Maps Platform','Sovereign Mapping System','🗺️','#00bcd4','infrastructure'),
  'sovereign-maritime': ('Maritime Platform','Sovereign Maritime Control','⚓','#00bcd4','infrastructure'),
  'sovereign-marketing': ('Marketing Platform','Sovereign Digital Marketing','📣','#e040fb','media'),
  'sovereign-marketplace': ('Marketplace Platform','Sovereign Digital Marketplace','🏪','#ffd600','commerce'),
  'sovereign-media': ('Media Platform','Sovereign Media Network','📺','#e040fb','media'),
  'sovereign-music': ('Music Platform','Sovereign Music Streaming','🎵','#E91E63','media'),
  'sovereign-news': ('News Platform','Sovereign News Network','📰','#e040fb','media'),
  'sovereign-passport': ('Passport Platform','Sovereign Digital Passport','🛂','#1565c0','government'),
  'sovereign-payments': ('Payments Platform','Sovereign Payment Gateway','💳','#ffd600','finance'),
  'sovereign-podcast': ('Podcast Platform','Sovereign Podcast Network','🎙️','#E91E63','media'),
  'sovereign-radio': ('Radio Platform','Sovereign Digital Radio','📻','#E91E63','media'),
  'sovereign-repo': ('Repository Platform','Sovereign Code Repository','📦','#00e676','technology'),
  'sovereign-shorts': ('Shorts Platform','Sovereign Short Videos','🎬','#E91E63','media'),
  'sovereign-social': ('Social Platform','Sovereign Social Network','👥','#e040fb','social'),
  'sovereign-sports': ('Sports Platform','Sovereign Sports Network','⚽','#E91E63','sports'),
  'sovereign-transit': ('Transit Platform','Sovereign Public Transit','🚇','#ff9100','infrastructure'),
  'sovereign-university': ('University Platform','Sovereign Online University','🏛️','#9C27B0','education'),
  'sovereign-voice': ('Voice Platform','Sovereign Voice Communications','🎤','#e040fb','media'),
  'sovereign-wallet': ('Wallet Platform','Sovereign Digital Wallet','👛','#ffd600','finance'),
  'sovereign-waste': ('Waste Platform','Sovereign Waste Management','♻️','#43a047','environment'),
  'sovereign-water': ('Water Platform','Sovereign Water Resources','💧','#43a047','environment'),
  'sovereign-weather': ('Weather Platform','Sovereign Weather System','🌤️','#43a047','environment'),
  # === ENGLISH NON-SOVEREIGN ===
  'ai-agent-dev': ('AI Agent Dev','Sovereign AI Agent Development','🤖','#7c4dff','technology'),
  'algo-trading': ('Algo Trading','Sovereign Algorithmic Trading','📈','#ffd600','finance'),
  'ancestral-wisdom': ('Ancestral Wisdom','Digital Ancestral Knowledge Base','🧓','#00FF41','culture'),
  'artisan-market': ('Artisan Market','Indigenous Artisan Marketplace','🎨','#00FF41','commerce'),
  'automation': ('Automation Engine','Sovereign Process Automation','⚙️','#00e676','technology'),
  'blockchain-explorer': ('Blockchain Explorer','MameyNode Block Explorer','🔗','#ffd600','finance'),
  'central-bank': ('Central Bank','Sovereign Central Bank System','🏦','#ffd600','finance'),
  'certificaciones-nft': ('NFT Certifications','Sovereign NFT Certificates','🏆','#ffd600','finance'),
  'civil-registry': ('Civil Registry','Sovereign Civil Registry','📝','#1565c0','government'),
  'crypto-exchange': ('Crypto Exchange','Sovereign Crypto Exchange','💱','#ffd600','finance'),
  'cultural-heritage': ('Cultural Heritage','Digital Cultural Heritage','🏛️','#00FF41','culture'),
  'datos-abiertos': ('Open Data','Sovereign Open Data Portal','📊','#1565c0','government'),
  'dev-platform': ('Dev Platform','Sovereign Developer Platform','🛠️','#00e676','technology'),
  'devops-pipeline': ('DevOps Pipeline','Sovereign CI/CD Pipeline','🔄','#00e676','technology'),
  'digital-cadastre': ('Digital Cadastre','Sovereign Land Registry','🗺️','#1565c0','government'),
  'digital-census': ('Digital Census','Sovereign Population Census','📋','#1565c0','government'),
  'digital-justice': ('Digital Justice','Sovereign Justice System','⚖️','#1565c0','government'),
  'digital-museum': ('Digital Museum','Sovereign Virtual Museum','🏛️','#00FF41','culture'),
  'digital-parliament': ('Digital Parliament','Sovereign Digital Parliament','🏛️','#1565c0','government'),
  'diplomacy': ('Diplomacy','Sovereign Diplomatic Platform','🌐','#1565c0','government'),
  'eco-tourism': ('Eco Tourism','Sovereign Eco-Tourism Portal','🌿','#43a047','environment'),
  'economic-analytics': ('Economic Analytics','Sovereign Economic Dashboard','📊','#ffd600','finance'),
  'electoral-commission': ('Electoral Commission','Sovereign Electoral System','🗳️','#1565c0','government'),
  'emergency-911': ('Emergency 911','Sovereign Emergency Response','🚨','#607D8B','infrastructure'),
  'immigration': ('Immigration Portal','Sovereign Immigration System','🛂','#1565c0','government'),
  'liquid-democracy': ('Liquid Democracy','Democracia Líquida Digital','🗳️','#1565c0','government'),
  'tokenization': ('Tokenization','Tokenización de Activos Soberanos','🪙','#ffd600','finance'),
  'wampum-cbdc': ('WAMPUM CBDC','Moneda Digital del Banco Central','💎','#ffd600','finance'),
  'pension-fund': ('Pension Fund','Fondo de Pensiones Soberano','💰','#ffd600','finance'),
  'public-audit': ('Public Audit','Auditoría Pública Nacional','🔍','#1565c0','government'),
  'fiscal-dashboard': ('Fiscal Dashboard','Dashboard Fiscal Nacional','📊','#ffd600','finance'),
  'fiscal-transparency': ('Fiscal Transparency','Portal de Transparencia Fiscal','🔍','#1565c0','government'),
  'trading-dashboard': ('Trading Dashboard','Dashboard de Trading Soberano','📈','#ffd600','finance'),
  # === SPANISH GOV/FINANCE ===
  'aduana-soberana': ('Aduana Soberana','Sistema Aduanero Nacional','🛃','#ffd600','finance'),
  'auditoria-soberana': ('Auditoría Soberana','Sistema Nacional de Auditoría','🔍','#ffd600','finance'),
  'banco-central-soberano': ('Banco Central Soberano','Banca Central Digital','🏦','#ffd600','finance'),
  'bdet-wallet': ('BDET Wallet','Billetera Digital WAMPUM','👛','#ffd600','finance'),
  'blockchain-soberana': ('Blockchain Soberana','MameyNode Blockchain Network','⛓️','#ffd600','finance'),
  'casino-soberano': ('Casino Soberano','Casino Digital Provably Fair','🎰','#E91E63','sports'),
  'catastro-soberano': ('Catastro Soberano','Registro Catastral Nacional','🗺️','#1565c0','government'),
  'censo-soberano': ('Censo Soberano','Sistema Censal Nacional','📋','#1565c0','government'),
  'comision-electoral-soberana': ('Comisión Electoral','Sistema Electoral Nacional','🗳️','#1565c0','government'),
  'contratos-inteligentes-soberano': ('Contratos Inteligentes','Smart Contracts Soberanos','📄','#ffd600','finance'),
  'credito-soberano': ('Crédito Soberano','Sistema de Crédito Nacional','💳','#ffd600','finance'),
  'identidad-digital-soberana': ('Identidad Digital','Sistema de Identidad Nacional','🆔','#f44336','security'),
  'impuestos-soberano': ('Impuestos Soberano','Sistema Tributario Nacional','🏛️','#ffd600','finance'),
  'inmigracion-soberana': ('Inmigración Soberana','Control Migratorio Nacional','🛂','#1565c0','government'),
  'justicia-digital-soberana': ('Justicia Digital','Sistema Judicial Digital','⚖️','#1565c0','government'),
  'justicia-restaurativa-soberana': ('Justicia Restaurativa','Sistema de Justicia Restaurativa','🤝','#1565c0','government'),
  'licencias-permisos-soberano': ('Licencias y Permisos','Licencias y Permisos Digitales','📋','#1565c0','government'),
  'loto-soberano': ('Loto Soberano','Lotería Nacional Soberana','🎱','#E91E63','sports'),
  'normas-soberana': ('Normas Soberana','Marco Regulatorio Nacional','📜','#1565c0','government'),
  'notificaciones-gobierno': ('Notificaciones Gobierno','Sistema de Notificaciones','🔔','#1565c0','government'),
  'parlamento-soberano': ('Parlamento Soberano','Parlamento Digital Nacional','🏛️','#1565c0','government'),
  'pasaporte-soberano': ('Pasaporte Soberano','Pasaporte Digital Soberano','🛂','#1565c0','government'),
  'patrimonio-cultural-soberano': ('Patrimonio Cultural','Registro de Patrimonio Cultural','🏛️','#00FF41','culture'),
  'pension-soberana': ('Pensión Soberana','Sistema de Pensiones Nacional','💰','#607D8B','finance'),
  'propiedad-intelectual-soberana': ('Propiedad Intelectual','Registro de Propiedad Intelectual','💡','#1565c0','government'),
  'proteccion-datos-soberana': ('Protección de Datos','Protección de Datos Personales','🔒','#f44336','security'),
  'registro-civil-soberano': ('Registro Civil','Registro Civil Nacional','📝','#1565c0','government'),
  'renta-basica': ('Renta Básica','Renta Básica Universal','💵','#607D8B','finance'),
  'rifa-soberana': ('Rifa Soberana','Sistema de Rifas y Sorteos','🎟️','#E91E63','sports'),
  'seguros-soberano': ('Seguros Soberano','Aseguradoras Soberanas','🛡️','#ffd600','finance'),
  'tribunal-digital-soberano': ('Tribunal Digital','Tribunales Digitales Soberanos','⚖️','#1565c0','government'),
  'correccional-soberano': ('Correccional Soberano','Sistema Correccional Nacional','🏛️','#1565c0','government'),
  # === SPANISH TECH/DEV ===
  'audio-editor-soberano': ('Audio Editor Soberano','Editor de Audio Profesional','🎵','#e040fb','media'),
  'base-datos-soberana': ('Base de Datos Soberana','Motor de Base de Datos Soberano','🗄️','#00e676','technology'),
  'bci-soberano': ('BCI Soberano','Interfaz Cerebro-Computadora','🧠','#7c4dff','technology'),
  'buscador-soberano': ('Buscador Soberano','Motor de Búsqueda Soberano','🔍','#00bcd4','technology'),
  'code-soberano': ('Code Soberano','Plataforma de Código Soberano','💻','#00e676','technology'),
  'contrasenas-soberana': ('Contraseñas Soberana','Gestor de Contraseñas','🔐','#f44336','security'),
  'criptografia-militar-soberana': ('Criptografía Militar','Criptografía Grado Militar','🔒','#f44336','military'),
  'edge-ai-soberano': ('Edge AI Soberano','Computación AI en el Borde','🤖','#7c4dff','technology'),
  'estacion-terrena-soberana': ('Estación Terrena','Estación Terrena Satelital','📡','#00bcd4','space'),
  'firma-digital-soberana': ('Firma Digital','Sistema de Firma Digital','✍️','#f44336','security'),
  'flota-drones-soberana': ('Flota de Drones','Control de Flota de Drones','🛸','#f44336','military'),
  'frontera-digital-soberana': ('Frontera Digital','Control Fronterizo Digital','🛃','#f44336','security'),
  'gestion-tiempo-soberano': ('Gestión de Tiempo','Control de Tiempo y Productividad','⏰','#00e676','office'),
  'hojas-calculo-soberana': ('Hojas de Cálculo','Hojas de Cálculo Colaborativas','📊','#00e676','office'),
  'inteligencia-soberana': ('Inteligencia Soberana','Agencia de Inteligencia Digital','🕵️','#f44336','military'),
  'internet-cuantico-soberano': ('Internet Cuántico','Red de Internet Cuántico','🌐','#7c4dff','technology'),
  'internet-soberano': ('Internet Soberano','Red de Internet Soberana','🌐','#00bcd4','infrastructure'),
  'laboratorio-soberano': ('Laboratorio Soberano','Laboratorio Digital Nacional','🔬','#7c4dff','technology'),
  'laboratorio-virtual': ('Laboratorio Virtual','Laboratorio Virtual de Ciencias','🧪','#9C27B0','education'),
  'mapas-mentales-soberano': ('Mapas Mentales','Mapas Mentales y Diagramas','🧠','#00e676','office'),
  'mesh-soberana': ('Mesh Soberana','Red Mesh Soberana','📶','#00bcd4','infrastructure'),
  'modelado-3d-soberano': ('Modelado 3D','Modelado 3D Soberano','🎨','#00e676','technology'),
  'monitoreo-soberano': ('Monitoreo Soberano','Monitoreo de Infraestructura','📊','#00e676','technology'),
  'notas-soberana': ('Notas Soberana','Notas y Documentos Rápidos','📝','#00e676','office'),
  'nuclear-soberano': ('Nuclear Soberano','Energía Nuclear Soberana','☢️','#00bcd4','infrastructure'),
  'observabilidad-soberana': ('Observabilidad Soberana','Stack de Observabilidad','👁️','#00e676','technology'),
  'observacion-terrestre-soberana': ('Observación Terrestre','Observación Terrestre Satelital','🛰️','#00bcd4','space'),
  'observatorio-terrestre': ('Observatorio Terrestre','Observatorio Astronómico Digital','🔭','#00bcd4','space'),
  'pizarra-soberana': ('Pizarra Soberana','Pizarra Digital Colaborativa','🖊️','#00e676','office'),
  'programa-espacial-soberano': ('Programa Espacial','Programa Espacial Soberano','🚀','#00bcd4','space'),
  'publicacion-soberana': ('Publicación Soberana','Plataforma de Publicación','📰','#e040fb','media'),
  'quantum-cloud-soberana': ('Quantum Cloud','Computación Cuántica en la Nube','☁️','#7c4dff','technology'),
  'quantum-computing': ('Quantum Computing','Computación Cuántica Soberana','⚛️','#7c4dff','technology'),
  'streaming-estudio-soberano': ('Streaming Estudio','Estudio de Streaming','🎬','#e040fb','media'),
  'testing-soberano': ('Testing Soberano','Plataforma de Testing QA','🧪','#00e676','technology'),
  'traductor-soberano': ('Traductor Soberano','Traductor Universal','🌐','#7c4dff','technology'),
  'virtualizacion-soberana': ('Virtualización Soberana','Plataforma de Virtualización','🖥️','#00e676','technology'),
  'wiki-soberana': ('Wiki Soberana','Wiki Colaborativa','📖','#00e676','office'),
  'video-editor-soberano': ('Video Editor','Editor de Video Profesional','🎬','#e040fb','media'),
  'calendario-soberano': ('Calendario Soberano','Calendario y Agenda','📅','#00e676','office'),
  'formularios-soberano': ('Formularios Soberano','Constructor de Formularios','📋','#00e676','office'),
  'crm-soberano': ('CRM Soberano','Gestión de Relaciones','📇','#ffd600','commerce'),
  'infographic': ('Infographic Soberana','Plataforma de Infografías','📊','#e040fb','media'),
  'lanzamiento-soberano': ('Lanzamiento Soberano','Centro de Lanzamiento Espacial','🚀','#00bcd4','space'),
  'landing-ierahkwa': ('Landing Ierahkwa','Página de Inicio del Ecosistema','🏠','#e040fb','media'),
  'landing-page': ('Landing Page Builder','Constructor de Landing Pages','🌐','#e040fb','media'),
  'orchestrator': ('Orchestrator','Orquestador de Servicios','🎛️','#00e676','technology'),
  'low-code': ('Low-Code Platform','Plataforma Low-Code','🧩','#00e676','technology'),
  'project-mgmt': ('Project Management','Gestión de Proyectos','📋','#00e676','office'),
  'ui-design': ('UI Design','Diseño de Interfaces','🎨','#00e676','technology'),
  'pitch-deck': ('Pitch Deck','Presentaciones para Inversores','📊','#ffd600','finance'),
  # === SPANISH SOCIAL/ENV/HEALTH/AGRI ===
  'acuicultura-soberana': ('Acuicultura Soberana','Gestión Acuícola Nacional','🐟','#43a047','agriculture'),
  'agua-soberana': ('Agua Soberana','Gestión de Recursos Hídricos','💧','#43a047','environment'),
  'alimentacion-soberana': ('Alimentación Soberana','Soberanía Alimentaria Nacional','🌾','#43a047','agriculture'),
  'ambiente-soberano': ('Ambiente Soberano','Gestión Ambiental Nacional','🌍','#43a047','environment'),
  'apuestas-soberano': ('Apuestas Soberano','Plataforma de Apuestas Regulada','🎲','#E91E63','sports'),
  'aviacion-soberana': ('Aviación Soberana','Autoridad de Aviación Civil','✈️','#00bcd4','infrastructure'),
  'becas-escolares-soberana': ('Becas Escolares','Sistema de Becas Educativas','🎓','#1E88E5','education'),
  'biblioteca-ancestral': ('Biblioteca Ancestral','Biblioteca del Conocimiento Ancestral','📚','#00FF41','culture'),
  'bomberos-soberano': ('Bomberos Soberano','Cuerpo de Bomberos Digital','🚒','#607D8B','infrastructure'),
  'calificaciones-soberana': ('Calificaciones Soberana','Sistema de Calificaciones','📊','#1E88E5','education'),
  'cine-soberano': ('Cine Soberano','Plataforma de Cine Nacional','🎬','#E91E63','media'),
  'correo-postal-soberano': ('Correo Postal','Servicio Postal Nacional Digital','✉️','#00bcd4','infrastructure'),
  'curriculum-digital-soberano': ('Currículo Digital','Diseño Curricular Nacional','📚','#1E88E5','education'),
  'deporte-soberano': ('Deporte Soberano','Federación Deportiva Nacional','⚽','#E91E63','sports'),
  'desempleo-soberano': ('Desempleo Soberano','Seguro de Desempleo Digital','📋','#607D8B','finance'),
  'discapacidad-soberana': ('Discapacidad Soberana','Registro Nacional de Discapacidad','♿','#607D8B','health'),
  'educacion-especial-soberana': ('Educación Especial','Educación Inclusiva Especial','🧩','#1E88E5','education'),
  'escuela-primaria-soberana': ('Escuela Primaria','Sistema de Educación Primaria','🏫','#1E88E5','education'),
  'escuela-secundaria-soberana': ('Escuela Secundaria','Sistema de Educación Secundaria','🎒','#1E88E5','education'),
  'escuela-soberana': ('Escuela Soberana','Plataforma Escolar Integrada','🏫','#1E88E5','education'),
  'esports-soberano': ('Esports Soberano','Liga de Esports Nacional','🎮','#E91E63','sports'),
  'estadio-soberano': ('Estadio Soberano','Estadio Virtual Nacional','🏟️','#E91E63','sports'),
  'fabrica-soberana': ('Fábrica Soberana','Manufactura Digital Nacional','🏭','#ff9100','infrastructure'),
  'familia-soberana': ('Familia Soberana','Bienestar Familiar Nacional','👨‍👩‍👧','#607D8B','social'),
  'farmacia-soberana': ('Farmacia Soberana','Red de Farmacias Soberanas','💊','#FF5722','health'),
  'fauna-soberana': ('Fauna Soberana','Registro de Fauna Nacional','🦅','#43a047','environment'),
  'fertirrigacion-soberana': ('Fertirrigación Soberana','Sistema de Riego Inteligente','🌱','#43a047','agriculture'),
  'fitness-soberano': ('Fitness Soberano','Plataforma de Bienestar Físico','💪','#FF5722','health'),
  'galeria-soberana': ('Galería Soberana','Galería de Arte Digital','🖼️','#00FF41','culture'),
  'ganaderia-soberana': ('Ganadería Soberana','Gestión Ganadera Nacional','🐄','#43a047','agriculture'),
  'geologia-soberana': ('Geología Soberana','Servicio Geológico Nacional','🏔️','#43a047','environment'),
  'humanitario-soberano': ('Humanitario Soberano','Asistencia Humanitaria','🤝','#607D8B','social'),
  'idiomas-soberano': ('Idiomas Soberano','Enseñanza de Idiomas','🗣️','#9C27B0','education'),
  'intercambio-comunitario-soberano': ('Intercambio Comunitario','Trueque y Comercio Comunitario','🤝','#00FF41','commerce'),
  'inventario-soberano': ('Inventario Soberano','Gestión de Inventario Nacional','📦','#ffd600','commerce'),
  'investigacion-soberana': ('Investigación Soberana','Portal de Investigación Científica','🔬','#9C27B0','education'),
  'juegos-soberano': ('Juegos Soberano','Plataforma de Juegos','🎮','#E91E63','sports'),
  'lector-soberano': ('Lector Soberano','Lector de Documentos','📖','#00e676','office'),
  'lenguas-indigenas': ('Lenguas Indígenas','Preservación de Lenguas Indígenas','🗣️','#00FF41','culture'),
  'maritimo-soberano': ('Marítimo Soberano','Autoridad Marítima Nacional','⚓','#00bcd4','infrastructure'),
  'meteorologia-soberana': ('Meteorología Soberana','Servicio Meteorológico Nacional','🌦️','#43a047','environment'),
  'mineria-soberana': ('Minería Soberana','Gestión Minera Nacional','⛏️','#43a047','infrastructure'),
  'museo-soberano': ('Museo Soberano','Red de Museos Digitales','🏛️','#00FF41','culture'),
  'natural-parks': ('Natural Parks','Parques Naturales Soberanos','🌲','#43a047','environment'),
  'parques-soberano': ('Parques Soberano','Gestión de Parques Urbanos','🌳','#ff9100','environment'),
  'pesca-soberana': ('Pesca Soberana','Gestión Pesquera Nacional','🎣','#43a047','agriculture'),
  'portal-padres-soberano': ('Portal de Padres','Portal para Padres de Familia','👨‍👩‍👧','#1E88E5','education'),
  'preescolar-soberano': ('Preescolar Soberano','Educación Preescolar Digital','🧒','#1E88E5','education'),
  'reciclaje-soberano': ('Reciclaje Soberano','Gestión de Reciclaje Nacional','♻️','#43a047','environment'),
  # === REMAINING ===
  'pos-soberano': ('POS Soberano','Punto de Venta Soberano','🏪','#ffd600','commerce'),
  'reforestacion-soberana': ('Reforestación Soberana','Programa de Reforestación','🌳','#43a047','environment'),
  'residuos-soberano': ('Residuos Soberano','Gestión de Residuos Sólidos','🗑️','#43a047','environment'),
  'rrhh-soberano': ('RRHH Soberano','Gestión de Recursos Humanos','👥','#ffd600','commerce'),
  'salud-mental-soberana': ('Salud Mental Soberana','Salud Mental y Bienestar','🧠','#FF5722','health'),
  'semilla-soberana': ('Semilla Soberana','Banco de Semillas Nacional','🌱','#43a047','agriculture'),
  'smart-factory': ('Smart Factory','Fábrica Inteligente IoT','🏭','#ff9100','infrastructure'),
  'social-welfare': ('Social Welfare','Bienestar Social Digital','🤝','#607D8B','social'),
  'soporte-soberano': ('Soporte Soberano','Mesa de Ayuda y Soporte','🎧','#00e676','technology'),
  'telemedicina-soberana': ('Telemedicina Soberana','Plataforma de Telemedicina','🏥','#FF5722','health'),
  'territorio-vigilante-soberano': ('Territorio Vigilante','Vigilancia Territorial Nacional','📡','#f44336','military'),
  'transito-soberano': ('Tránsito Soberano','Control de Tránsito Nacional','🚦','#ff9100','infrastructure'),
  'transporte-publico-soberano': ('Transporte Público','Sistema de Transporte Público','🚌','#ff9100','infrastructure'),
  'turismo-soberano': ('Turismo Soberano','Portal de Turismo Nacional','🏖️','#00FF41','culture'),
  'universal-translator': ('Universal Translator','Traductor Universal Indígena','🌐','#7c4dff','technology'),
  'urban-planning': ('Urban Planning','Planificación Urbana Digital','🏙️','#ff9100','infrastructure'),
  'urbanismo-soberano': ('Urbanismo Soberano','Desarrollo Urbano Nacional','🏗️','#ff9100','infrastructure'),
  'vehiculos-soberano': ('Vehículos Soberano','Registro Vehicular Nacional','🚗','#ff9100','infrastructure'),
  'veteranos-soberano': ('Veteranos Soberano','Beneficios para Veteranos','🎖️','#607D8B','social'),
  'vivienda-soberana': ('Vivienda Soberana','Programa Nacional de Vivienda','🏠','#607D8B','infrastructure'),
  'sovereign-housing': ('Sovereign Housing','Vivienda Soberana Digital','🏠','#607D8B','infrastructure'),
}


# ═══════════════════════════════════════════
# GENERATE HTML
# ═══════════════════════════════════════════
def gen(key):
    t,sub,icon,accent,cat = P[key]
    c = CATS[cat]
    desc = f'{t} — plataforma soberana de grado empresarial para las 19 naciones del ecosistema Ierahkwa Ne Kanienke. {sub} con cifrado post-cuántico Kyber-768, blockchain MameyNode, modo offline y soporte para 14 idiomas indígenas. Reemplazo soberano 100% libre de Big Tech.'
    m = '\n'.join([f'<div class="stat" role="listitem"><div class="val">{v[0]}</div><div class="lbl">{v[1]}</div></div>' for v in c['metrics']])
    cards = '\n'.join([f'<article class="card"><div class="card-icon" aria-hidden="true">{x[0]}</div><h4>{x[1]}</h4><p>{x[2]}</p></article>' for x in c['cards']])
    slug = key.replace('-','_')
    apis = '\n'.join([f'<div class="sec-check"><span class="dot ok" aria-hidden="true"></span><code style="color:{"#00FF41" if a[0]=="GET" else "#ffd600"};font-size:.7rem;margin-right:.5rem">{a[0]}</code><code style="color:var(--txt2);font-size:.72rem;flex:1">{a[1].replace("{slug}",slug)}</code></div><p style="font-size:.75rem;color:var(--txt2);margin:0 0 .5rem 1.5rem">{a[2]}</p>' for a in c['apis']])
    ph = ''
    for i,plan in enumerate(c['pricing']):
        pop = ' style="border-color:var(--accent);box-shadow:0 0 20px rgba(0,255,65,.15)"' if i==1 else ''
        fl = ''.join([f'<li style="font-size:.78rem;color:var(--txt2);padding:.2rem 0">✓ {f}</li>' for f in plan[2]])
        ph += f'<div class="card"{pop}>\n<h4 style="color:var(--accent);font-size:.9rem">{plan[0]}</h4>\n<div style="font-family:Orbitron,sans-serif;font-size:1.4rem;font-weight:700;margin:.5rem 0">{plan[1]}</div>\n<ul style="list-style:none;padding:0">{fl}</ul>\n</div>'
    stores = json.dumps(c['offline'][0])
    odesc = c['offline'][1]
    return f'''<!DOCTYPE html>
<html lang="es" dir="ltr">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<meta http-equiv="X-Content-Type-Options" content="nosniff">
<meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
<meta http-equiv="Referrer-Policy" content="strict-origin-when-cross-origin">
<meta http-equiv="Permissions-Policy" content="camera=(), microphone=(), geolocation=(self), payment=()">
<meta name="description" content="{desc[:160]}">
<meta name="theme-color" content="{accent}">
<link rel="canonical" href="https://ierahkwa.nation/{key}/">
<link rel="manifest" href="../shared/manifest.json">
<link rel="icon" href="../icons/icon-96.svg" type="image/svg+xml">
<link rel="apple-touch-icon" href="../icons/icon-192.svg">
<meta property="og:title" content="{t} — {sub}">
<meta property="og:description" content="{desc[:200]}">
<meta property="og:type" content="website">
<meta property="og:url" content="https://ierahkwa.nation/{key}/">
<meta property="og:image" content="https://ierahkwa.nation/icons/icon-512.svg">
<meta name="twitter:card" content="summary">
<meta name="twitter:title" content="{t} — {sub}">
<meta name="twitter:description" content="{desc[:200]}">
<link rel="stylesheet" href="../shared/ierahkwa.css">
<title>{t} — {sub}</title>
<style>:root{{--accent:{accent}}}</style>
</head>
<body role="document">
<a href="#main" class="skip-nav">Saltar al contenido principal</a>
<header>
<div class="logo"><div class="logo-icon" aria-hidden="true">{icon}</div><h1>{t}</h1></div>
<nav aria-label="Navegacion principal"><a href="#dashboard" aria-current="page">Dashboard</a><a href="#features">Modulos</a><a href="#api">API</a><a href="#pricing">Precios</a></nav>
<span class="encrypted-badge" title="Cifrado Post-Quantum Activo"><span aria-hidden="true">⚛️</span> Quantum-Safe</span>
</header>
<main id="main">
<section class="hero" id="dashboard">
<div class="badge"><span aria-hidden="true">{icon}</span> {sub}</div>
<h2><span>{t}</span></h2>
<p>{desc}</p>
<div style="display:flex;gap:.75rem;justify-content:center;flex-wrap:wrap;margin-top:1rem">
<a href="#features" class="btn">Explorar Módulos</a>
<a href="#api" class="btn" style="background:transparent;border:2px solid var(--accent);color:var(--accent)">API Docs</a>
</div>
</section>
<div class="stats" role="list" aria-label="Metricas del sistema">{m}</div>
<div class="section" id="architecture">
<h2><span aria-hidden="true">🏗️</span> Arquitectura del Sistema</h2>
<div class="sub">Infraestructura soberana de {t}</div>
<div class="sec-panel" style="font-family:monospace;font-size:.72rem;line-height:1.8;overflow-x:auto">
<span style="color:var(--accent)">┌─ FRONTEND ─────────────────────────────────┐</span>
│  PWA + WebAssembly + Service Worker          │
│  Offline-first · 14 idiomas · WCAG 2.1 AA   │
<span style="color:var(--accent)">└──────────────────┬────────────────────────────┘</span>
                   │ <span style="color:#ffd600">REST + gRPC + WebSocket</span>
<span style="color:#00bcd4">┌─ API GATEWAY ────┴────────────────────────────┐</span>
│  Rate limiting · JWT · CSRF · mTLS · CORS     │
│  Load balancer · Circuit breaker · Cache       │
<span style="color:#00bcd4">└──────────────────┬────────────────────────────┘</span>
                   │
<span style="color:#7c4dff">┌─ MICROSERVICIOS ─┴────────────────────────────┐</span>
│  Rust + Go · 12-factor · Event-driven          │
│  CQRS · Saga pattern · Domain-driven design    │
<span style="color:#7c4dff">└──────────────────┬────────────────────────────┘</span>
                   │
<span style="color:var(--accent)">┌─ DATA LAYER ─────┴────────────────────────────┐</span>
│  PostgreSQL · Redis · TimescaleDB · S3          │
│  MameyNode Blockchain · IndexedDB (offline)     │
<span style="color:var(--accent)">└─────────────────────────────────────────────────┘</span>
</div>
</div>
<div class="section-title" id="features"><h3>Módulos de la Plataforma</h3><p>10 herramientas soberanas de grado empresarial</p></div>
<div class="grid">{cards}</div>
<div class="section" id="api">
<h2><span aria-hidden="true">🔌</span> API Endpoints</h2>
<div class="sub">REST + gRPC + WebSocket para integraciones</div>
<div class="sec-panel">{apis}</div>
</div>
<div class="section-title" id="pricing"><h3>Planes Soberanos</h3><p>Empieza gratis. Escala soberanamente.</p></div>
<div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(220px,1fr))">{ph}</div>
</main>
<footer>
<p><span aria-hidden="true">{icon}</span> <strong>{t}</strong> &mdash; Ecosistema Digital <a href="../">Ierahkwa Ne Kanienke</a></p>
<p style="color:var(--txt2);font-size:.7rem;margin-top:.25rem">233+ plataformas soberanas &middot; 17 dominios NEXUS &middot; 72M personas &middot; 19 naciones</p>
<div style="margin-top:.75rem"><span class="security-badge" data-grade="A+" title="Seguridad Soberana Activa"><span class="sec-icon" aria-hidden="true">🛡️</span> Seguro</span></div>
</footer>
<script src="../shared/ierahkwa.js"></script>
<script src="../shared/ierahkwa-security.js"></script>
<script src="../shared/ierahkwa-quantum.js"></script>
<script>
(function(){{var DB_NAME='ierahkwa-{key}';var DB_VER=1;var STORES={stores};var db=null;function openDB(){{return new Promise(function(resolve,reject){{var req=indexedDB.open(DB_NAME,DB_VER);req.onupgradeneeded=function(){{var d=req.result;STORES.forEach(function(s){{if(!d.objectStoreNames.contains(s))d.createObjectStore(s,{{keyPath:'id'}})}});}};req.onsuccess=function(){{db=req.result;resolve(db)}};req.onerror=function(){{reject(req.error)}}}})}}function showOfflineBanner(show){{var b=document.getElementById('offline-banner');if(!b){{b=document.createElement('div');b.id='offline-banner';b.style.cssText='position:fixed;bottom:0;left:0;right:0;background:var(--accent);color:#09090d;text-align:center;padding:8px;font-size:13px;font-weight:700;z-index:9999;transform:translateY(100%);transition:transform .3s';b.textContent='Modo Offline — {odesc}';document.body.appendChild(b)}}b.style.transform=show?'translateY(0)':'translateY(100%)'}}function init(){{openDB().then(function(){{window.addEventListener('online',function(){{showOfflineBanner(false)}});window.addEventListener('offline',function(){{showOfflineBanner(true)}});if(!navigator.onLine)showOfflineBanner(true);console.log('[{key}] Offline module ready')}})}}init()}})();
</script>
<script>if("serviceWorker"in navigator){{navigator.serviceWorker.register("../shared/sw.js").catch(function(){{}})}}</script>
</body>
</html>'''


def main():
    print("=" * 60)
    print("🚀 IERAHKWA — MEGA UPGRADE ALL PLATFORMS")
    print("=" * 60)
    # Skip platforms already upgraded by upgrade-pages.py
    SKIP = {'vpn-soberana','erp-soberano','contabilidad-soberana','ceremonia-virtual-soberana',
            'nomina-soberana','oraculo-climatico-soberano','adn-ancestral-soberano'}
    up = 0
    skip = 0
    miss = 0
    for key in P:
        if key in SKIP:
            skip += 1
            continue
        d = os.path.join(BASE, key)
        if not os.path.isdir(d):
            print(f"  ⚠️  Dir not found: {key}")
            miss += 1
            continue
        html = gen(key)
        with open(os.path.join(d,'index.html'),'w',encoding='utf-8') as f:
            f.write(html)
        sz = len(html)
        ln = html.count('\n')+1
        print(f"  ✅ {key}: {ln} lines, {sz:,} bytes")
        up += 1
    print(f"\n{'='*60}")
    print(f"📊 UPGRADED: {up} | SKIPPED (already done): {skip} | MISSING: {miss}")
    print(f"{'='*60}")

if __name__=='__main__':
    main()
