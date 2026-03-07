#!/usr/bin/env python3
"""
IERAHKWA NE KANIENKE — PLANOS DE SERVIDORES
Genera PDF con arquitectura completa de servidores para producción
"""

from reportlab.lib.pagesizes import letter
from reportlab.lib.units import inch, mm
from reportlab.lib.colors import HexColor, black, white, gray
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_RIGHT
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    PageBreak, KeepTogether, HRFlowable
)
from reportlab.pdfgen import canvas
from reportlab.platypus.doctemplate import PageTemplate, BaseDocTemplate, Frame
from datetime import datetime
import os

# ── Colores ──────────────────────────────────────────────────
CYAN = HexColor('#00bcd4')
DARK_BG = HexColor('#0a0e1a')
DARK_CARD = HexColor('#111827')
GREEN = HexColor('#00e676')
GOLD = HexColor('#ffd600')
RED = HexColor('#f44336')
PURPLE = HexColor('#7c4dff')
ORANGE = HexColor('#ff9100')
BLUE = HexColor('#1565c0')
LIGHT_GRAY = HexColor('#e0e0e0')
MED_GRAY = HexColor('#9e9e9e')
DARK_TEXT = HexColor('#1a1a2e')
WHITE_BG = HexColor('#ffffff')
SECTION_BG = HexColor('#f5f5f5')
BORDER_COLOR = HexColor('#00bcd4')

OUTPUT_PATH = os.path.expanduser('~/Desktop/files/Soberano-Organizado/PLANOS-SERVIDORES-SOBERANO.pdf')

# ── Estilos ──────────────────────────────────────────────────
styles = getSampleStyleSheet()

title_style = ParagraphStyle(
    'CustomTitle', parent=styles['Title'],
    fontSize=28, leading=34, textColor=DARK_TEXT,
    spaceAfter=6, alignment=TA_CENTER,
    fontName='Helvetica-Bold'
)

subtitle_style = ParagraphStyle(
    'CustomSubtitle', parent=styles['Normal'],
    fontSize=14, leading=18, textColor=MED_GRAY,
    spaceAfter=20, alignment=TA_CENTER,
    fontName='Helvetica'
)

h1_style = ParagraphStyle(
    'H1', parent=styles['Heading1'],
    fontSize=22, leading=28, textColor=DARK_TEXT,
    spaceBefore=20, spaceAfter=12,
    fontName='Helvetica-Bold',
    borderWidth=2, borderColor=CYAN, borderPadding=8,
    backColor=HexColor('#e0f7fa')
)

h2_style = ParagraphStyle(
    'H2', parent=styles['Heading2'],
    fontSize=16, leading=20, textColor=DARK_TEXT,
    spaceBefore=16, spaceAfter=8,
    fontName='Helvetica-Bold'
)

h3_style = ParagraphStyle(
    'H3', parent=styles['Heading3'],
    fontSize=13, leading=16, textColor=HexColor('#333333'),
    spaceBefore=10, spaceAfter=6,
    fontName='Helvetica-Bold'
)

body_style = ParagraphStyle(
    'Body', parent=styles['Normal'],
    fontSize=10, leading=14, textColor=HexColor('#333333'),
    spaceAfter=6, fontName='Helvetica'
)

code_style = ParagraphStyle(
    'Code', parent=styles['Normal'],
    fontSize=8.5, leading=11, textColor=HexColor('#1a1a2e'),
    fontName='Courier', backColor=HexColor('#f0f0f0'),
    borderWidth=0.5, borderColor=HexColor('#cccccc'),
    borderPadding=6, spaceAfter=8, spaceBefore=4
)

bullet_style = ParagraphStyle(
    'Bullet', parent=styles['Normal'],
    fontSize=10, leading=14, textColor=HexColor('#333333'),
    leftIndent=20, bulletIndent=8, spaceAfter=3,
    fontName='Helvetica'
)

note_style = ParagraphStyle(
    'Note', parent=styles['Normal'],
    fontSize=9, leading=12, textColor=HexColor('#555555'),
    backColor=HexColor('#fff3e0'), borderWidth=0.5,
    borderColor=ORANGE, borderPadding=8,
    spaceAfter=10, spaceBefore=6, fontName='Helvetica-Oblique'
)

# ── Funciones auxiliares ─────────────────────────────────────
def make_table(data, col_widths=None, header=True):
    """Crear tabla estilizada"""
    t = Table(data, colWidths=col_widths, repeatRows=1 if header else 0)
    style_cmds = [
        ('BACKGROUND', (0, 0), (-1, 0), CYAN),
        ('TEXTCOLOR', (0, 0), (-1, 0), white),
        ('FONTNAME', (0, 0), (-1, 0), 'Helvetica-Bold'),
        ('FONTSIZE', (0, 0), (-1, 0), 10),
        ('FONTNAME', (0, 1), (-1, -1), 'Helvetica'),
        ('FONTSIZE', (0, 1), (-1, -1), 9),
        ('ALIGN', (0, 0), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'MIDDLE'),
        ('GRID', (0, 0), (-1, -1), 0.5, HexColor('#cccccc')),
        ('ROWBACKGROUNDS', (0, 1), (-1, -1), [WHITE_BG, HexColor('#f8f8f8')]),
        ('TOPPADDING', (0, 0), (-1, -1), 5),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 5),
        ('LEFTPADDING', (0, 0), (-1, -1), 8),
        ('RIGHTPADDING', (0, 0), (-1, -1), 8),
    ]
    t.setStyle(TableStyle(style_cmds))
    return t

def section_line():
    return HRFlowable(width="100%", thickness=1, color=CYAN, spaceAfter=10, spaceBefore=5)

def bullet(text):
    return Paragraph(f"<bullet>&bull;</bullet> {text}", bullet_style)

def code_block(text):
    escaped = text.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')
    return Paragraph(escaped, code_style)

# ── Construir PDF ─────────────────────────────────────────────
def build_pdf():
    doc = SimpleDocTemplate(
        OUTPUT_PATH,
        pagesize=letter,
        topMargin=0.75*inch,
        bottomMargin=0.75*inch,
        leftMargin=0.75*inch,
        rightMargin=0.75*inch
    )

    story = []

    # ══════════════════════════════════════════════════════════
    # PORTADA
    # ══════════════════════════════════════════════════════════
    story.append(Spacer(1, 1.5*inch))
    story.append(Paragraph("IERAHKWA NE KANIENKE", title_style))
    story.append(Paragraph("Internet Satelital Soberano", subtitle_style))
    story.append(Spacer(1, 0.3*inch))
    story.append(section_line())
    story.append(Spacer(1, 0.2*inch))

    cover_title = ParagraphStyle('CoverTitle', parent=title_style, fontSize=24, textColor=CYAN)
    story.append(Paragraph("PLANOS DE SERVIDORES", cover_title))
    story.append(Paragraph("Arquitectura de Produccion", subtitle_style))
    story.append(Spacer(1, 0.5*inch))

    cover_data = [
        ['Campo', 'Valor'],
        ['Proyecto', 'WiFi Soberano + Sovereign Core'],
        ['Version', 'v5.5.0'],
        ['Fecha', datetime.now().strftime('%d/%m/%Y')],
        ['Clasificacion', 'CONFIDENCIAL — Solo equipo interno'],
        ['Infraestructura', 'Starlink + Docker + Node.js + PostgreSQL'],
        ['Territorios', '574 Naciones / 19 Paises'],
        ['Poblacion Objetivo', '72,847,291 personas'],
    ]
    story.append(make_table(cover_data, col_widths=[2.5*inch, 4.5*inch]))
    story.append(Spacer(1, 0.5*inch))

    story.append(Paragraph(
        "Documento generado automaticamente por el sistema Ierahkwa. "
        "Contiene instrucciones completas para montar servidores de produccion "
        "del sistema WiFi Soberano en cualquier ubicacion con acceso Starlink.",
        body_style
    ))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # INDICE
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("INDICE", h1_style))
    story.append(Spacer(1, 0.2*inch))

    toc_items = [
        "1. Arquitectura General del Sistema",
        "2. Requisitos de Hardware",
        "3. Diagrama de Red",
        "4. Stack Tecnologico",
        "5. Servidor 1: WiFi Soberano (Portal Cautivo)",
        "6. Servidor 2: Sovereign Core (Backend Universal)",
        "7. Base de Datos: PostgreSQL 16 + Redis 7",
        "8. Nginx: Reverse Proxy + SSL + Captive Portal",
        "9. Docker: Contenedores y Volumenes",
        "10. Firewall y Seguridad",
        "11. Monitoreo y Alertas",
        "12. Scripts de Deployment",
        "13. Flujo del Usuario WiFi",
        "14. Revenue Model — Modelo de Ingresos",
        "15. Escalamiento: De 1 Hotspot a 574 Territorios",
        "16. Checklist de Deployment",
    ]
    for item in toc_items:
        story.append(Paragraph(item, body_style))
    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 1. ARQUITECTURA GENERAL
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("1. ARQUITECTURA GENERAL DEL SISTEMA", h1_style))
    story.append(Spacer(1, 0.1*inch))

    story.append(Paragraph(
        "El sistema WiFi Soberano opera como un portal cautivo comercial que vende acceso "
        "a internet via Starlink. La arquitectura se compone de 5 capas principales:",
        body_style
    ))

    arch_diagram = """
+------------------------------------------------------------------+
|                    USUARIOS WiFi (Celulares/Laptops)              |
+------------------------------------------------------------------+
          |                    |                    |
    [Apple Detection]   [Android Detection]  [Microsoft Detection]
    /hotspot-detect     /generate_204        /connecttest.txt
          |                    |                    |
+------------------------------------------------------------------+
|                  NGINX (Puerto 80/443)                            |
|     Reverse Proxy + SSL + Rate Limiting + Captive Detection      |
+------------------------------------------------------------------+
          |                              |
+-----------------------+    +-----------------------+
|   WiFi Soberano       |    |   Sovereign Core      |
|   Puerto 3095         |    |   Puerto 3050         |
|   - Captive Portal    |    |   - Auth/Users        |
|   - Session Manager   |    |   - Payments WAMPUM   |
|   - Plans/Pricing     |    |   - Analytics         |
|   - Fleet Tracker     |    |   - WiFi Bridge       |
|   - Vigilancia        |    |   - Messaging         |
+-----------------------+    +-----------------------+
          |                              |
+------------------------------------------------------------------+
|              PostgreSQL 16 + Redis 7 (Docker)                     |
|         Datos persistentes + Cache de sesiones                    |
+------------------------------------------------------------------+
          |
+------------------------------------------------------------------+
|              STARLINK (Conexion Satelital)                        |
|         Performance Gen 1/3 + Mini + Mesh Nodes                  |
+------------------------------------------------------------------+
"""
    story.append(code_block(arch_diagram.strip()))
    story.append(Spacer(1, 0.1*inch))

    story.append(Paragraph("<b>Flujo simplificado:</b>", body_style))
    story.append(bullet("Usuario se conecta al WiFi del router Starlink"))
    story.append(bullet("iptables redirige TODO trafico HTTP al portal cautivo"))
    story.append(bullet("Nginx detecta el tipo de dispositivo (Apple/Android/Microsoft)"))
    story.append(bullet("Usuario ve la pagina de planes y precios"))
    story.append(bullet("Paga con WAMPUM (blockchain) o metodo local"))
    story.append(bullet("Backend autenticar MAC del dispositivo via ipset"))
    story.append(bullet("iptables permite trafico del MAC autenticado"))
    story.append(bullet("Sesion expira automaticamente segun el plan comprado"))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 2. REQUISITOS DE HARDWARE
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("2. REQUISITOS DE HARDWARE", h1_style))

    story.append(Paragraph("<b>Servidor Minimo (1 hotspot, hasta 50 usuarios):</b>", h2_style))
    hw_min = [
        ['Componente', 'Minimo', 'Recomendado'],
        ['CPU', '2 cores ARM/x86', '4 cores'],
        ['RAM', '2 GB', '4 GB'],
        ['Almacenamiento', '32 GB SSD', '64 GB SSD'],
        ['Red', '1x Ethernet Gigabit', '2x Ethernet'],
        ['SO', 'Ubuntu 22.04 LTS', 'Ubuntu 24.04 LTS'],
        ['Ejemplo Hardware', 'Raspberry Pi 4 (4GB)', 'Mini PC Intel N100'],
    ]
    story.append(make_table(hw_min, col_widths=[2*inch, 2.3*inch, 2.7*inch]))
    story.append(Spacer(1, 0.2*inch))

    story.append(Paragraph("<b>Servidor Regional (5-20 hotspots, hasta 500 usuarios):</b>", h2_style))
    hw_reg = [
        ['Componente', 'Especificacion'],
        ['CPU', '8+ cores (Intel i5/i7 o Ryzen 5)'],
        ['RAM', '16 GB DDR4'],
        ['Almacenamiento', '256 GB NVMe SSD'],
        ['Red', '2x Gigabit Ethernet + WiFi 6E'],
        ['SO', 'Ubuntu 22.04/24.04 Server'],
        ['Ejemplo', 'Dell OptiPlex, HP ProDesk, o similar'],
    ]
    story.append(make_table(hw_reg, col_widths=[2.5*inch, 4.5*inch]))
    story.append(Spacer(1, 0.2*inch))

    story.append(Paragraph("<b>Equipo Starlink por Hotspot:</b>", h2_style))
    starlink_data = [
        ['Kit', 'Modelo', 'Velocidad', 'Usuarios Max', 'Precio/mes'],
        ['Performance', 'Gen 1/3', '40-220 Mbps', '100+', '$120-250 USD'],
        ['Standard', 'Gen 2', '25-100 Mbps', '50', '$50-120 USD'],
        ['Mini', 'Portable', '5-50 Mbps', '20', '$30-50 USD'],
        ['Mesh Node', 'Repetidor', 'N/A (relay)', '+30/nodo', '$0 (incluido)'],
    ]
    story.append(make_table(starlink_data, col_widths=[1.2*inch, 1*inch, 1.3*inch, 1.3*inch, 1.5*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph(
        "NOTA: Cada kit Starlink se multiplica con routers mesh/repetidores WiFi. "
        "Un kit Performance puede servir 3-5 zonas con repetidores bien ubicados.",
        note_style
    ))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 3. DIAGRAMA DE RED
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("3. DIAGRAMA DE RED", h1_style))

    net_diagram = """
                        SATELITE STARLINK
                              |
                    [Antena Starlink Kit]
                              |
                     [Router Starlink]
                      192.168.1.1
                              |
              +---------------+---------------+
              |                               |
    [SERVIDOR SOBERANO]              [Switch/AP WiFi]
     192.168.1.10                    192.168.1.0/24
     - Nginx (80/443)                     |
     - WiFi Soberano (3095)    +----------+----------+
     - Sovereign Core (3050)   |          |          |
     - PostgreSQL (5432)    [Mesh 1]  [Mesh 2]  [Mesh 3]
     - Redis (6379)         .100      .101      .102
     - iptables/captive              |
                            [Usuarios WiFi]
                         192.168.1.50-250 (DHCP)
"""
    story.append(code_block(net_diagram.strip()))
    story.append(Spacer(1, 0.1*inch))

    story.append(Paragraph("<b>Configuracion de Red:</b>", h2_style))
    net_config = [
        ['Parametro', 'Valor', 'Notas'],
        ['Subred WiFi', '192.168.1.0/24', 'Subred principal para usuarios'],
        ['Gateway', '192.168.1.1', 'Router Starlink'],
        ['Servidor', '192.168.1.10', 'IP fija del servidor Soberano'],
        ['DHCP Range', '192.168.1.50-250', 'Para dispositivos usuarios'],
        ['DNS', '8.8.8.8, 1.1.1.1', 'O DNS local para portal detection'],
        ['Mesh Nodes', '192.168.1.100-110', 'IPs fijas para repetidores'],
    ]
    story.append(make_table(net_config, col_widths=[1.8*inch, 2*inch, 3.2*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Puertos Abiertos (Firewall UFW):</b>", h2_style))
    ports_data = [
        ['Puerto', 'Protocolo', 'Servicio', 'Acceso'],
        ['22', 'TCP', 'SSH', 'Solo admin (IP whitelist)'],
        ['80', 'TCP', 'HTTP', 'Publico (redirect a HTTPS)'],
        ['443', 'TCP', 'HTTPS', 'Publico (portal + API)'],
        ['3095', 'TCP', 'WiFi Soberano', 'Solo localhost'],
        ['3050', 'TCP', 'Sovereign Core', 'Solo localhost'],
        ['5432', 'TCP', 'PostgreSQL', 'Solo localhost'],
        ['6379', 'TCP', 'Redis', 'Solo localhost'],
    ]
    story.append(make_table(ports_data, col_widths=[1*inch, 1.2*inch, 1.8*inch, 3*inch]))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 4. STACK TECNOLOGICO
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("4. STACK TECNOLOGICO", h1_style))

    stack_data = [
        ['Capa', 'Tecnologia', 'Version', 'Funcion'],
        ['Runtime', 'Node.js', '22 LTS', 'Servidor de aplicacion'],
        ['Framework', 'Express.js', '4.21', 'API REST + WebSocket'],
        ['Base de Datos', 'PostgreSQL', '16 Alpine', 'Datos persistentes'],
        ['Cache/Sesiones', 'Redis', '7 Alpine', 'Sesiones WiFi + cache'],
        ['Contenedores', 'Docker', 'Latest', 'Isolation de servicios'],
        ['Reverse Proxy', 'Nginx', 'Latest', 'SSL, rate limiting, captive'],
        ['SSL', "Let's Encrypt", 'Auto-renew', 'Certificados HTTPS'],
        ['Blockchain', 'MameyNode', 'v1.0', 'Pagos WAMPUM'],
        ['Firewall', 'UFW + iptables', 'Native', 'Captive portal redirect'],
        ['Monitoreo', 'systemd + cron', 'Native', 'Health checks, auto-recovery'],
        ['CI/CD', 'GitHub Actions', 'Latest', 'Deploy automatico'],
        ['AI Security', '7 Agents', 'v5.5', 'Guardian, Anomaly, Trust...'],
    ]
    story.append(make_table(stack_data, col_widths=[1.3*inch, 1.5*inch, 1*inch, 3.2*inch]))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 5. SERVIDOR 1: WIFI SOBERANO
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("5. SERVIDOR 1: WiFi SOBERANO (Puerto 3095)", h1_style))

    story.append(Paragraph(
        "Servicio principal del portal cautivo. Gestiona sesiones WiFi, planes, pagos, "
        "fleet tracking y vigilancia.",
        body_style
    ))

    story.append(Paragraph("<b>Estructura de archivos:</b>", h2_style))
    wifi_structure = """
03-backend/wifi-soberano/
|-- package.json
|-- Dockerfile
|-- .env.example
|-- server.js              <- Express server principal
|-- routes/
|   |-- auth.js            <- Login captive portal
|   |-- plans.js           <- CRUD de planes WiFi
|   |-- sessions.js        <- Gestion de sesiones
|   |-- payments.js        <- Pagos WAMPUM
|   |-- analytics.js       <- Recoleccion de datos
|   |-- fleet.js           <- Starlink fleet management
|   |-- admin.js           <- Dashboard admin API
|-- middleware/
|   |-- bandwidth.js       <- Rate limiting por plan
|   |-- captive.js         <- Redirect captive portal
|-- models/
|   |-- migrations.sql     <- Tablas SQL
|-- workers/
|   |-- session-expiry.js  <- Worker que expira sesiones
"""
    story.append(code_block(wifi_structure.strip()))

    story.append(Paragraph("<b>API Endpoints:</b>", h2_style))
    api_data = [
        ['Metodo', 'Endpoint', 'Acceso', 'Descripcion'],
        ['GET', '/api/v1/wifi/portal', 'Publico', 'Data del portal cautivo'],
        ['GET', '/api/v1/wifi/plans', 'Publico', 'Planes disponibles + precios'],
        ['POST', '/api/v1/wifi/connect', 'Publico', 'Conectar dispositivo al WiFi'],
        ['GET', '/api/v1/wifi/session/status', 'Publico', 'Tiempo restante de sesion'],
        ['POST', '/api/v1/wifi/session/extend', 'Publico', 'Extender tiempo de sesion'],
        ['POST', '/api/v1/wifi/payment/create', 'Publico', 'Crear pago WAMPUM'],
        ['POST', '/api/v1/wifi/payment/verify', 'Publico', 'Verificar pago'],
        ['GET', '/api/v1/wifi/admin/dashboard', 'Admin', 'Metricas en tiempo real'],
        ['GET', '/api/v1/wifi/admin/sessions', 'Admin', 'Sesiones activas'],
        ['GET', '/api/v1/wifi/admin/revenue', 'Admin', 'Ingresos por periodo'],
        ['GET', '/api/v1/wifi/admin/fleet', 'Admin', 'Estado de kits Starlink'],
        ['GET', '/api/v1/wifi/admin/analytics', 'Admin', 'Analytics de usuarios'],
        ['GET', '/api/v1/wifi/admin/vigilancia', 'Admin', 'Alertas de vigilancia'],
        ['POST', '/api/v1/wifi/admin/hotspot', 'Admin', 'Agregar/editar hotspot'],
        ['POST', '/api/v1/wifi/admin/plan', 'Admin', 'Crear/editar plan'],
    ]
    story.append(make_table(api_data, col_widths=[0.7*inch, 2.5*inch, 0.8*inch, 3*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Planes WiFi (precios default):</b>", h2_style))
    plans_data = [
        ['Plan', 'Duracion', 'Precio (WAMPUM)', 'Bandwidth'],
        ['1 Hora', '1h', '9.99', '25 Mbps'],
        ['1 Dia', '24h', '24.99', '50 Mbps'],
        ['1 Semana', '168h', '99.99', '50 Mbps'],
        ['1 Mes', '720h', '249.99', '100 Mbps'],
        ['6 Meses', '4,320h', '999.99', '100 Mbps'],
        ['1 Ano', '8,760h', '1,499.99', '100 Mbps'],
    ]
    story.append(make_table(plans_data, col_widths=[1.5*inch, 1.5*inch, 2*inch, 2*inch]))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 6. SERVIDOR 2: SOVEREIGN CORE
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("6. SERVIDOR 2: SOVEREIGN CORE (Puerto 3050)", h1_style))

    story.append(Paragraph(
        "Backend universal que gestiona usuarios, autenticacion, pagos WAMPUM, "
        "messaging, voting, storage, analytics y content. Incluye el modulo "
        "wifi-bridge que conecta con WiFi Soberano.",
        body_style
    ))

    story.append(Paragraph("<b>Modulos de Sovereign Core:</b>", h2_style))
    core_modules = [
        ['Modulo', 'Path', 'Funcion'],
        ['Auth', '/v1/auth/*', 'Login, registro, JWT tokens'],
        ['Users', '/v1/users/*', 'Gestion de usuarios soberanos'],
        ['Payments', '/v1/payments/*', 'WAMPUM + BDET Bank'],
        ['Messaging', '/v1/messages/*', 'Chat soberano (WebSocket)'],
        ['Voting', '/v1/voting/*', 'Votaciones nacion soberana'],
        ['Storage', '/v1/storage/*', 'Almacenamiento soberano'],
        ['Analytics', '/v1/analytics/*', 'Datos y metricas'],
        ['Content', '/v1/content/*', 'CMS soberano'],
        ['WiFi Bridge', '/v1/wifi/*', 'Bridge a WiFi Soberano (3095)'],
    ]
    story.append(make_table(core_modules, col_widths=[1.5*inch, 2*inch, 3.5*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>WiFi Bridge Endpoints:</b>", h2_style))
    bridge_data = [
        ['Metodo', 'Endpoint', 'Funcion'],
        ['GET', '/v1/wifi/status', 'Health check del bridge'],
        ['GET', '/v1/wifi/plans', 'Proxy a planes WiFi'],
        ['GET', '/v1/wifi/dashboard', 'Dashboard agregado'],
        ['POST', '/v1/wifi/provision-user', 'Crear cuenta desde WiFi signup'],
        ['GET', '/v1/wifi/sessions', 'Sesiones WiFi activas'],
        ['GET', '/v1/wifi/revenue', 'Revenue WiFi agregado'],
        ['GET', '/v1/wifi/fleet', 'Estado fleet Starlink'],
        ['GET', '/v1/wifi/vigilancia', 'Alertas vigilancia'],
        ['POST', '/v1/wifi/alert', 'Forward alertas WiFi'],
    ]
    story.append(make_table(bridge_data, col_widths=[1*inch, 2.5*inch, 3.5*inch]))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 7. BASE DE DATOS
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("7. BASE DE DATOS: PostgreSQL 16 + Redis 7", h1_style))

    story.append(Paragraph("<b>PostgreSQL 16 (Docker)</b>", h2_style))
    story.append(Paragraph("Contenedor: postgres-soberano | Red: soberana | Puerto: 127.0.0.1:5432", body_style))

    story.append(Paragraph("<b>Tablas principales WiFi:</b>", h3_style))
    tables_data = [
        ['Tabla', 'Columnas Clave', 'Indices', 'Tamano Est.'],
        ['wifi_sessions', 'id, mac, ip, plan_id, expires_at, status', 'mac, status, expires_at', '~10GB/ano'],
        ['wifi_plans', 'id, name, duration_hours, price_wampum', 'is_active', '<1MB'],
        ['hotspots', 'id, name, lat, lng, territory, max_users', 'status, territory', '<1MB'],
        ['starlink_fleet', 'id, utid, model, account, status', 'utid, status', '<1MB'],
        ['wifi_analytics', 'id, session_id, timestamp, bytes_up/down', 'timestamp, hotspot_id', '~50GB/ano'],
        ['users', 'id, email, password_hash, role, metadata', 'email', '~1GB/ano'],
    ]
    story.append(make_table(tables_data, col_widths=[1.3*inch, 2.3*inch, 1.7*inch, 1.2*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Redis 7 (Docker)</b>", h2_style))
    story.append(Paragraph("Contenedor: redis-soberano | Red: soberana | Puerto: 127.0.0.1:6379", body_style))

    redis_data = [
        ['Key Pattern', 'TTL', 'Uso'],
        ['session:{mac}', 'Segun plan', 'Sesion WiFi activa'],
        ['rate:{ip}', '60s', 'Rate limiting por IP'],
        ['cache:plans', '300s', 'Cache de planes'],
        ['cache:dashboard', '30s', 'Cache dashboard admin'],
        ['auth:{token}', '24h', 'JWT token blacklist'],
        ['fleet:{utid}', '60s', 'Estado Starlink en cache'],
    ]
    story.append(make_table(redis_data, col_widths=[2*inch, 1.2*inch, 3.8*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Comandos Docker:</b>", h3_style))
    docker_cmds = """
# Iniciar PostgreSQL
docker run -d --name postgres-soberano --network soberana \\
  --restart always -e POSTGRES_DB=soberana \\
  -e POSTGRES_USER=soberano -e POSTGRES_PASSWORD=<GENERADO> \\
  -v pgdata-soberano:/var/lib/postgresql/data \\
  -p 127.0.0.1:5432:5432 postgres:16-alpine

# Iniciar Redis
docker run -d --name redis-soberano --network soberana \\
  --restart always -v redis-soberano:/data \\
  -p 127.0.0.1:6379:6379 \\
  redis:7-alpine redis-server --requirepass <GENERADO>

# Backup PostgreSQL
docker exec postgres-soberano pg_dump -U soberano soberana | gzip > backup.sql.gz

# Conectar a PostgreSQL
docker exec -it postgres-soberano psql -U soberano -d soberana
"""
    story.append(code_block(docker_cmds.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 8. NGINX
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("8. NGINX: Reverse Proxy + SSL + Captive Portal", h1_style))

    story.append(Paragraph(
        "Nginx actua como punto de entrada unico. Maneja SSL, rate limiting, "
        "deteccion de portal cautivo, proxy a backends y WebSocket.",
        body_style
    ))

    story.append(Paragraph("<b>Captive Portal Detection:</b>", h2_style))
    captive_data = [
        ['Sistema', 'URL que prueba', 'Respuesta Esperada', 'Nuestra Respuesta'],
        ['Apple iOS/macOS', '/hotspot-detect.html', 'HTML con Success', '302 -> /portal'],
        ['Android', '/generate_204', 'HTTP 204', '302 -> /portal'],
        ['Microsoft Windows', '/connecttest.txt', 'Microsoft Connect Test', '302 -> /portal'],
        ['Firefox', '/success.txt', 'success', '302 -> /portal'],
    ]
    story.append(make_table(captive_data, col_widths=[1.5*inch, 1.8*inch, 1.8*inch, 1.9*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Configuracion de Upstreams:</b>", h3_style))
    nginx_conf = """
upstream wifi_backend {
    server 127.0.0.1:3095;    # WiFi Soberano
    keepalive 32;
}

upstream core_backend {
    server 127.0.0.1:3050;    # Sovereign Core
    keepalive 16;
}

# Rate Limiting
limit_req_zone $binary_remote_addr zone=portal:10m rate=30r/m;
limit_req_zone $binary_remote_addr zone=api:10m rate=200r/m;
"""
    story.append(code_block(nginx_conf.strip()))

    story.append(Paragraph("<b>Rutas principales:</b>", h2_style))
    routes_data = [
        ['Location', 'Backend', 'Tipo', 'Notas'],
        ['/portal', 'Static files', 'HTML', 'Captive portal UI'],
        ['/api/v1/wifi/*', 'wifi_backend', 'API', 'Rate limited 200r/m'],
        ['/ws/wifi', 'wifi_backend', 'WebSocket', 'Dashboard real-time'],
        ['/v1/*', 'core_backend', 'API', 'Sovereign Core API'],
        ['/shared/*', 'Static files', 'Assets', 'CSS/JS, cache 7 dias'],
        ['/health', 'wifi_backend', 'Health', 'Sin access_log'],
        ['/', 'wifi_backend', 'Default', 'Catch-all'],
    ]
    story.append(make_table(routes_data, col_widths=[1.5*inch, 1.5*inch, 1.2*inch, 2.8*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>SSL/TLS:</b>", h2_style))
    story.append(bullet("Let's Encrypt via certbot (auto-renewal)"))
    story.append(bullet("TLS 1.2 + 1.3 (ECDHE ciphers)"))
    story.append(bullet("HSTS habilitado (max-age=31536000)"))
    story.append(bullet("Auto-firmado como fallback si el dominio no resuelve aun"))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 9. DOCKER
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("9. DOCKER: Contenedores y Volumenes", h1_style))

    docker_data = [
        ['Contenedor', 'Imagen', 'Puerto', 'Volumen', 'Restart'],
        ['postgres-soberano', 'postgres:16-alpine', '127.0.0.1:5432', 'pgdata-soberano', 'always'],
        ['redis-soberano', 'redis:7-alpine', '127.0.0.1:6379', 'redis-soberano', 'always'],
    ]
    story.append(make_table(docker_data, col_widths=[1.5*inch, 1.5*inch, 1.3*inch, 1.5*inch, 0.8*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Red Docker:</b>", h2_style))
    story.append(bullet("Nombre: soberana"))
    story.append(bullet("Driver: bridge"))
    story.append(bullet("Todos los contenedores conectados a esta red"))
    story.append(bullet("Los servicios Node.js corren nativos (systemd) y se conectan via localhost"))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Volumenes Persistentes:</b>", h2_style))
    volumes_data = [
        ['Volumen', 'Contenedor', 'Path Interno', 'Proposito'],
        ['pgdata-soberano', 'postgres-soberano', '/var/lib/postgresql/data', 'Datos PostgreSQL'],
        ['redis-soberano', 'redis-soberano', '/data', 'Datos Redis (AOF)'],
    ]
    story.append(make_table(volumes_data, col_widths=[1.5*inch, 1.5*inch, 2.3*inch, 1.7*inch]))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 10. FIREWALL Y SEGURIDAD
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("10. FIREWALL Y SEGURIDAD", h1_style))

    story.append(Paragraph("<b>Capas de seguridad:</b>", h2_style))
    security_layers = [
        ['Capa', 'Herramienta', 'Funcion'],
        ['1. Red', 'UFW (iptables)', 'Solo puertos 22, 80, 443 abiertos'],
        ['2. Captive Portal', 'iptables/nftables', 'Redirect HTTP no autenticado al portal'],
        ['3. Rate Limiting', 'Nginx', '30r/m portal, 200r/m API'],
        ['4. Auth', 'JWT + bcrypt', 'Tokens con expiracion, hash seguro'],
        ['5. Fail2Ban', 'fail2ban', 'Ban IPs con multiples fallos SSH/HTTP'],
        ['6. SSL', "Let's Encrypt", 'Encriptacion en transito'],
        ['7. AI Security', '7 Agents', 'Guardian, Anomaly, Trust, Shield, Forensic'],
        ['8. Vigilancia', 'Atabey AI', 'Monitor 24/7, proteccion VIP'],
    ]
    story.append(make_table(security_layers, col_widths=[1.2*inch, 1.5*inch, 4.3*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Captive Portal Redirect (iptables):</b>", h2_style))
    iptables_flow = """
FLUJO DE IPTABLES:

1. MANGLE (PREROUTING):
   - Verificar si MAC esta en ipset 'wifi_auth'
   - Si si: MARK 0x1 (autenticado)
   - Si no: sin marca

2. NAT (PREROUTING):
   - Whitelist: servidor portal, DNS, DHCP -> RETURN
   - Si MARK 0x1 (autenticado) -> RETURN
   - HTTP (80) -> DNAT a PORTAL_IP:80
   - HTTPS (443) -> DNAT a PORTAL_IP:443

3. FILTER (FORWARD):
   - ESTABLISHED,RELATED -> ACCEPT
   - DNS (53) -> ACCEPT
   - MARK 0x1 (autenticado) -> ACCEPT
   - Destino portal -> ACCEPT
   - Todo lo demas -> DROP

4. NAT (POSTROUTING):
   - MASQUERADE para salida a internet
"""
    story.append(code_block(iptables_flow.strip()))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Comandos de autenticacion MAC:</b>", h3_style))
    auth_cmds = """
# Autenticar dispositivo (darle internet)
wifi-auth-device AA:BB:CC:DD:EE:FF 86400   # 24 horas

# Desautenticar (cortarle internet)
wifi-deauth-device AA:BB:CC:DD:EE:FF

# Listar autenticados
wifi-list-auth
"""
    story.append(code_block(auth_cmds.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 11. MONITOREO
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("11. MONITOREO Y ALERTAS", h1_style))

    story.append(Paragraph("<b>Servicios monitoreados:</b>", h2_style))
    monitor_data = [
        ['Servicio', 'Check', 'Intervalo', 'Auto-Recovery'],
        ['wifi-soberano', 'HTTP /health', '5 min', 'systemctl restart'],
        ['sovereign-core', 'HTTP /health', '5 min', 'systemctl restart'],
        ['nginx', 'HTTP /','5 min', 'systemctl restart'],
        ['PostgreSQL', 'Docker status', '5 min', 'docker restart'],
        ['Redis', 'Docker status', '5 min', 'docker restart'],
        ['Disco', 'df usage', '5 min', 'Alerta si >85%'],
        ['Memoria', '/proc/meminfo', '5 min', 'Alerta si >85%'],
        ['SSL Cert', 'openssl check', '5 min', 'Alerta si <30 dias'],
    ]
    story.append(make_table(monitor_data, col_widths=[1.5*inch, 1.5*inch, 1*inch, 3*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Alertas:</b>", h2_style))
    story.append(bullet("Webhook (Slack/Discord/Telegram)"))
    story.append(bullet("API interna wifi-soberano /admin/alert"))
    story.append(bullet("Syslog (logger -t soberano-health)"))
    story.append(bullet("Auto-recovery despues de 3 fallos consecutivos"))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph("<b>Comandos:</b>", h3_style))
    monitor_cmds = """
# Check una vez
./health-monitor.sh check

# Instalar como servicio (systemd + cron)
./health-monitor.sh install

# Ver logs
journalctl -u soberano-health -f
journalctl -u wifi-soberano -f
journalctl -u sovereign-core -f
"""
    story.append(code_block(monitor_cmds.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 12. SCRIPTS DE DEPLOYMENT
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("12. SCRIPTS DE DEPLOYMENT", h1_style))

    scripts_data = [
        ['Script', 'Funcion', 'Cuando Usar'],
        ['setup-wifi-production.sh', 'Setup completo del servidor desde cero', 'Primera vez en servidor nuevo'],
        ['captive-portal-redirect.sh', 'Configurar iptables para portal cautivo', 'Despues del setup, en el router'],
        ['deploy-wifi.sh', 'Deploy de actualizaciones (git pull + restart)', 'Cada vez que hay cambios'],
        ['health-monitor.sh', 'Monitoreo continuo con auto-recovery', 'Siempre corriendo'],
        ['test-wifi-e2e.sh', 'Test end-to-end del flujo completo', 'Post-deploy para verificar'],
    ]
    story.append(make_table(scripts_data, col_widths=[2.2*inch, 2.5*inch, 2.3*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Orden de ejecucion (servidor nuevo):</b>", h2_style))

    deploy_steps = """
# PASO 1: Clonar repositorio
git clone https://github.com/rudvincci/ierahkwa-platform.git /opt/soberano

# PASO 2: Setup completo del servidor
cd /opt/soberano/07-scripts
sudo bash setup-wifi-production.sh

# PASO 3: Configurar captive portal redirect
sudo bash captive-portal-redirect.sh setup

# PASO 4: Instalar health monitor
sudo bash health-monitor.sh install

# PASO 5: Verificar con test E2E
bash test-wifi-e2e.sh

# PASO 6: Configurar DNS
# Apuntar wifi.soberano.bo -> IP del servidor

# PASO 7: Obtener SSL real
sudo certbot --nginx -d wifi.soberano.bo
"""
    story.append(code_block(deploy_steps.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 13. FLUJO DEL USUARIO
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("13. FLUJO DEL USUARIO WiFi", h1_style))

    user_flow = """
+----------------------------------------------------------------------+
|  FLUJO COMPLETO: Usuario WiFi desde conexion hasta navegacion        |
+----------------------------------------------------------------------+

 1. CONEXION
    Usuario -> Conecta al WiFi "Internet Soberano"
    Router -> Asigna IP via DHCP (192.168.1.X)

 2. DETECCION AUTOMATICA
    Dispositivo -> Prueba URLs de captive portal detection
    iptables -> Redirige HTTP al servidor portal (DNAT)
    Nginx -> Detecta endpoint y redirige a /portal

 3. PORTAL CAUTIVO
    Usuario -> Ve pagina de bienvenida "Internet Soberano"
    Portal -> Muestra planes disponibles con precios

 4. SELECCION DE PLAN
    Usuario -> Elige plan (ej: 1 Hora por W 9.99)
    Frontend -> POST /api/v1/wifi/payment/create

 5. PAGO
    Backend -> Crea transaccion WAMPUM en MameyNode
    Usuario -> Confirma pago (wallet o QR code)
    Backend -> POST /api/v1/wifi/payment/verify

 6. ACTIVACION
    Backend -> Registra sesion en PostgreSQL
    Backend -> Guarda en Redis (TTL = duracion del plan)
    Backend -> Ejecuta: wifi-auth-device {MAC} {seconds}
    ipset -> Agrega MAC a wifi_auth con timeout

 7. NAVEGACION LIBRE
    iptables -> Detecta MAC en ipset -> MARK 0x1
    FORWARD -> MAC marcado pasa -> ACCEPT
    NAT -> MASQUERADE -> Internet via Starlink

 8. EXPIRACION
    Worker session-expiry.js -> Cada 30s verifica Redis
    Redis -> TTL expira -> Key eliminada
    Worker -> Ejecuta: wifi-deauth-device {MAC}
    ipset -> Remueve MAC de wifi_auth
    iptables -> Trafico redirigido al portal de nuevo

 9. RENOVACION (opcional)
    Usuario -> Ve portal de nuevo -> Puede extender sesion
    POST /api/v1/wifi/session/extend -> Paga diferencia
"""
    story.append(code_block(user_flow.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 14. REVENUE MODEL
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("14. REVENUE MODEL — Modelo de Ingresos", h1_style))

    story.append(Paragraph("<b>WiFi Soberano — Ingresos por Hotspot:</b>", h2_style))
    revenue_data = [
        ['Metrica', 'Conservador', 'Moderado', 'Optimista'],
        ['Usuarios/dia/hotspot', '20', '50', '100'],
        ['Plan promedio', 'W 9.99/h', 'W 24.99/d', 'W 24.99/d'],
        ['Ingreso/dia/hotspot', 'W 199.80', 'W 1,249.50', 'W 2,499'],
        ['Ingreso/mes/hotspot', 'W 5,994', 'W 37,485', 'W 74,970'],
        ['Costo Starlink/mes', '-W 3,000', '-W 3,000', '-W 3,000'],
        ['Costo servidor/mes', '-W 500', '-W 500', '-W 500'],
        ['GANANCIA/mes/hotspot', 'W 2,494', 'W 33,985', 'W 71,470'],
    ]
    story.append(make_table(revenue_data, col_widths=[2*inch, 1.5*inch, 1.7*inch, 1.8*inch]))

    story.append(Spacer(1, 0.2*inch))
    story.append(Paragraph("<b>Revenue Share — Plataformas Soberanas (30% plataforma):</b>", h2_style))

    rev_share = [
        ['Plataforma', 'Creador Recibe', 'Soberano Recibe', 'Big Tech Da'],
        ['Canal Soberano (Video)', '70%', '30%', 'YouTube: 55%'],
        ['Musica Soberana', '70%', '30%', 'Spotify: 30%'],
        ['Artesania Soberana', '70%', '30%', 'Etsy: 80%'],
        ['Soberano Rides', '70%', '30%', 'Uber: 70%'],
        ['Soberano Eats', '70%', '30%', 'DoorDash: 70%'],
        ['Servicios Soberanos', '70%', '30%', 'TaskRabbit: 70%'],
        ['Freelance Soberano', '70%', '30%', 'Fiverr: 80%'],
    ]
    story.append(make_table(rev_share, col_widths=[2*inch, 1.3*inch, 1.5*inch, 2.2*inch]))

    story.append(Spacer(1, 0.1*inch))
    story.append(Paragraph(
        "NOTA: 30% para la plataforma soberana es mas que competitivo. Los creadores "
        "reciben 70% vs 55% en YouTube o 30% en Spotify. El 30% de Soberano se "
        "reinvierte en infraestructura, expansion y bienestar de las 574 naciones.",
        note_style
    ))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 15. ESCALAMIENTO
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("15. ESCALAMIENTO: De 1 Hotspot a 574 Territorios", h1_style))

    story.append(Paragraph("<b>Fases de Expansion:</b>", h2_style))
    scale_data = [
        ['Fase', 'Hotspots', 'Territorios', 'Usuarios', 'Infra Necesaria'],
        ['1. Piloto', '1-5', 'Panama', '~250', '1 servidor local'],
        ['2. Regional', '5-20', 'Panama + Colombia', '~1,000', '1 VPS + servidores locales'],
        ['3. Nacional', '20-100', 'Centroamerica', '~5,000', 'Cluster K8s regional'],
        ['4. Continental', '100-500', 'Americas', '~25,000', 'Multi-region K8s'],
        ['5. Global', '500-5,000', '574 territorios', '~72M', 'Edge + CDN soberano'],
    ]
    story.append(make_table(scale_data, col_widths=[1.2*inch, 1*inch, 1.5*inch, 1*inch, 2.3*inch]))

    story.append(Spacer(1, 0.15*inch))
    story.append(Paragraph("<b>Arquitectura de escalamiento:</b>", h2_style))
    scale_arch = """
FASE 1-2 (Actual):
  Servidor Local -> Starlink -> Internet
  (Todo en 1 maquina)

FASE 3-4:
  [Servidor Central VPS]           [Servidores Locales]
   - Dashboard global               - WiFi Soberano
   - Analytics agregados             - Captive Portal
   - User provisioning               - Session Manager
   - Pagos centralizados             - Cache local
        |                                   |
   [PostgreSQL Central]            [Redis Local]
   [Redis Central]                 [SQLite fallback]
        |
   [CDN Soberano]
   - Portal HTML statico
   - Assets compartidos

FASE 5:
  [Kubernetes Multi-Region]
   - Edge nodes en cada territorio
   - Mesh VPN entre nodos
   - Replicacion PostgreSQL
   - Redis Cluster
   - CDN P2P soberano
"""
    story.append(code_block(scale_arch.strip()))

    story.append(PageBreak())

    # ══════════════════════════════════════════════════════════
    # 16. CHECKLIST
    # ══════════════════════════════════════════════════════════
    story.append(Paragraph("16. CHECKLIST DE DEPLOYMENT", h1_style))

    story.append(Paragraph("<b>Pre-Deployment:</b>", h2_style))
    story.append(bullet("[ ] Hardware: Servidor + Starlink kit operativo"))
    story.append(bullet("[ ] Red: Subred WiFi configurada (192.168.1.0/24)"))
    story.append(bullet("[ ] DNS: Dominio apuntando al servidor (o IP directa)"))
    story.append(bullet("[ ] Acceso SSH al servidor (root o sudo)"))
    story.append(bullet("[ ] Ubuntu 22.04+ instalado y actualizado"))

    story.append(Paragraph("<b>Deployment:</b>", h2_style))
    story.append(bullet("[ ] Clonar repo: git clone ierahkwa-platform /opt/soberano"))
    story.append(bullet("[ ] Ejecutar: setup-wifi-production.sh"))
    story.append(bullet("[ ] Verificar Docker containers: docker ps"))
    story.append(bullet("[ ] Verificar servicios: systemctl status wifi-soberano"))
    story.append(bullet("[ ] Verificar servicios: systemctl status sovereign-core"))
    story.append(bullet("[ ] Configurar captive portal: captive-portal-redirect.sh"))
    story.append(bullet("[ ] Instalar monitor: health-monitor.sh install"))

    story.append(Paragraph("<b>Post-Deployment:</b>", h2_style))
    story.append(bullet("[ ] Ejecutar test E2E: test-wifi-e2e.sh"))
    story.append(bullet("[ ] Verificar SSL: curl -I https://wifi.soberano.bo"))
    story.append(bullet("[ ] Probar captive portal desde celular"))
    story.append(bullet("[ ] Probar flujo de pago completo"))
    story.append(bullet("[ ] Verificar health monitor corriendo"))
    story.append(bullet("[ ] Configurar webhooks de alertas"))
    story.append(bullet("[ ] Documentar credenciales en lugar seguro"))

    story.append(Paragraph("<b>Mantenimiento:</b>", h2_style))
    story.append(bullet("[ ] Backups automaticos (cron): backup-database.sh"))
    story.append(bullet("[ ] Renovacion SSL automatica (certbot timer)"))
    story.append(bullet("[ ] Updates: deploy-wifi.sh (git pull + restart)"))
    story.append(bullet("[ ] Monitoreo 24/7: health-monitor.sh daemon"))
    story.append(bullet("[ ] Logs: journalctl -u wifi-soberano / sovereign-core"))

    story.append(Spacer(1, 0.3*inch))
    story.append(section_line())
    story.append(Spacer(1, 0.15*inch))

    footer_style = ParagraphStyle('Footer', parent=body_style, alignment=TA_CENTER, textColor=MED_GRAY, fontSize=9)
    story.append(Paragraph(
        f"IERAHKWA NE KANIENKE — Internet Satelital Soberano",
        footer_style
    ))
    story.append(Paragraph(
        f"574 Naciones | 19 Paises | 72,847,291 Personas",
        footer_style
    ))
    story.append(Paragraph(
        f"Generado: {datetime.now().strftime('%d/%m/%Y %H:%M')} | v5.5.0",
        footer_style
    ))
    story.append(Paragraph("CONFIDENCIAL — Solo equipo interno", footer_style))

    # Build PDF
    doc.build(story)
    print(f"PDF generado: {OUTPUT_PATH}")
    return OUTPUT_PATH

if __name__ == '__main__':
    build_pdf()
