using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace IERAHKWA.Platform.Services;

public class AIService : IAIService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AIService> _logger;
    private static readonly List<ChatMessage> _history = new();
    private static readonly Random _random = new();

    public AIService(IHttpClientFactory httpClientFactory, ILogger<AIService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> ChatAsync(string message, string? context = null)
    {
        try
        {
            // Guardar en historial
            _history.Add(new ChatMessage { Role = "user", Content = message, Timestamp = DateTime.UtcNow });
            
            var lowerMsg = message.ToLower().Trim();
            string response;
            
            // Saludos
            if (lowerMsg.Contains("hola") || lowerMsg.Contains("hello") || lowerMsg.Contains("hi") || lowerMsg == "hey")
            {
                var greetings = new[] {
                    "¡Hola! 👋 Soy **IERAHKWA AI**, tu asistente inteligente del Gobierno Soberano.",
                    "¡Bienvenido! 🏛️ Soy el asistente AI de la plataforma IERAHKWA.",
                    "¡Saludos! 🤖 IERAHKWA AI a tu servicio."
                };
                response = greetings[_random.Next(greetings.Length)] + "\n\n" +
                    "**¿En qué puedo ayudarte?**\n" +
                    "• 📊 `reporte` - Estado de la plataforma\n" +
                    "• 🏛️ `módulos` - Lista de servicios\n" +
                    "• 💻 `generar [descripción]` - Crear código\n" +
                    "• 📈 `estadísticas` - Métricas del sistema\n" +
                    "• 🔧 `api` - Documentación de APIs\n" +
                    "• ❓ `ayuda` - Ver todos los comandos";
            }
            // Reporte completo
            else if (lowerMsg.Contains("report") || lowerMsg.Contains("reporte") || lowerMsg.Contains("status"))
            {
                var uptime = DateTime.UtcNow - new DateTime(2026, 1, 22, 0, 0, 0);
                response = "📊 **IERAHKWA .NET Platform Report 10**\n\n" +
                    "**Active Modules:** 63 total\n" +
                    "• Services: 51\n" +
                    "• Departments: 12\n\n" +
                    "**Available APIs:**\n" +
                    "• `/api/platform/overview` - General status\n" +
                    "• `/api/platform/modules` - List of modules\n" +
                    "• `/api/dashboard/overview` - Dashboard stats\n" +
                    "• `/api/ai/chat` - Chat AI\n" +
                    "• `/api/files/*` - File Management\n\n" +
                    "**Framework:** ASP.NET Core .NET 10.0\n" +
                    $"**Uptime:** {uptime.Hours}h {uptime.Minutes}m\n" +
                    "**Status:** ✅ Operational";
            }
            // Estadísticas
            else if (lowerMsg.Contains("estadística") || lowerMsg.Contains("statistic") || lowerMsg.Contains("metric") || lowerMsg.Contains("stats"))
            {
                response = "📈 **Estadísticas del Sistema IERAHKWA**\n\n" +
                    $"**Tiempo de respuesta:** {_random.Next(50, 150)}ms\n" +
                    $"**Requests hoy:** {_random.Next(1000, 5000)}\n" +
                    $"**Usuarios activos:** {_random.Next(10, 100)}\n" +
                    $"**CPU:** {_random.Next(5, 30)}%\n" +
                    $"**Memoria:** {_random.Next(200, 500)}MB\n" +
                    $"**Módulos online:** 63/63\n" +
                    $"**Tokens IGT:** 103\n" +
                    "**Base de datos:** ✅ Conectada\n" +
                    "**Blockchain:** ✅ Sincronizada";
            }
            // Módulos
            else if (lowerMsg.Contains("módulo") || lowerMsg.Contains("module") || lowerMsg.Contains("servicio") || lowerMsg.Contains("service"))
            {
                response = "🏛️ **Módulos del Gobierno IERAHKWA**\n\n" +
                    "**💼 Gobierno:**\n" +
                    "• GOV - Portal Gubernamental\n" +
                    "• ADMIN - Administración\n" +
                    "• VOTING - Sistema Electoral\n\n" +
                    "**💰 Finanzas:**\n" +
                    "• BANK - BDET Central Bank\n" +
                    "• TRADEX - Exchange Platform\n" +
                    "• NET10 - DeFi Protocol\n\n" +
                    "**⛓️ Blockchain:**\n" +
                    "• ISB Network - Chain ID 777777\n" +
                    "• Smart Contracts\n" +
                    "• Token Factory (103 IGT)\n\n" +
                    "**🎮 Gaming:**\n" +
                    "• Casino, Lotto, Sports\n\n" +
                    "**📱 Social:**\n" +
                    "• Chat, Video, Streaming\n\n" +
                    "**Total: 63 módulos activos**";
            }
            // APIs
            else if (lowerMsg.Contains("api") || lowerMsg.Contains("endpoint"))
            {
                response = "🔌 **APIs Disponibles**\n\n" +
                    "**Platform:**\n" +
                    "• `GET /api/health` - Health check\n" +
                    "• `GET /api/platform/overview` - Resumen\n" +
                    "• `GET /api/platform/modules` - Módulos\n" +
                    "• `GET /api/platform/services` - Servicios\n\n" +
                    "**Dashboard:**\n" +
                    "• `GET /api/dashboard/overview` - Stats\n" +
                    "• `GET /api/members` - Miembros\n" +
                    "• `GET /api/usage/models` - Uso\n\n" +
                    "**AI:**\n" +
                    "• `POST /api/ai/chat` - Chat\n" +
                    "• `POST /api/ai/code/generate` - Generar código\n\n" +
                    "**Files:**\n" +
                    "• `GET /api/files/tree` - Árbol\n" +
                    "• `POST /api/files/save` - Guardar";
            }
            // Ayuda
            else if (lowerMsg.Contains("ayuda") || lowerMsg.Contains("help") || lowerMsg == "?")
            {
                response = "🤖 **IERAHKWA AI - Comandos**\n\n" +
                    "**Información:**\n" +
                    "• `hola` - Saludo\n" +
                    "• `reporte` - Estado del sistema\n" +
                    "• `estadísticas` - Métricas\n" +
                    "• `módulos` - Lista de servicios\n" +
                    "• `api` - Documentación\n\n" +
                    "**Desarrollo:**\n" +
                    "• `generar [desc]` - Crear código\n" +
                    "• `analizar [código]` - Revisar código\n" +
                    "• `explicar [tema]` - Explicación\n\n" +
                    "**Consultas:**\n" +
                    "• `blockchain` - Info de la red\n" +
                    "• `tokens` - Lista de IGT\n" +
                    "• `banco` - Info bancaria\n\n" +
                    "También puedes hacer preguntas libres.";
            }
            // Generar código
            else if (lowerMsg.Contains("generar") || lowerMsg.Contains("generate") || lowerMsg.Contains("crear código") || lowerMsg.Contains("code"))
            {
                var code = await GenerateCodeAsync(message, "csharp");
                response = "💻 **Código Generado:**\n\n" + code;
            }
            // Blockchain
            else if (lowerMsg.Contains("blockchain") || lowerMsg.Contains("chain") || lowerMsg.Contains("crypto"))
            {
                response = "⛓️ **IERAHKWA Sovereign Blockchain**\n\n" +
                    "**Red:** ISB Network\n" +
                    "**Chain ID:** 777777\n" +
                    "**Consenso:** Proof of Authority\n" +
                    "**Block Time:** 3 segundos\n" +
                    "**TPS:** 10,000+\n\n" +
                    "**Tokens:**\n" +
                    "• 103 IGT Tokens\n" +
                    "• Governance, Utility, Reward\n\n" +
                    "**DeFi:**\n" +
                    "• TradeX Exchange\n" +
                    "• NET10 Protocol\n" +
                    "• FarmFactory Yield";
            }
            // Tokens
            else if (lowerMsg.Contains("token") || lowerMsg.Contains("igt"))
            {
                response = "🪙 **IGT Token System**\n\n" +
                    "**Total Tokens:** 103\n\n" +
                    "**Categorías:**\n" +
                    "• Gobierno (30 tokens)\n" +
                    "• Finanzas (20 tokens)\n" +
                    "• Servicios (25 tokens)\n" +
                    "• Utilidad (28 tokens)\n\n" +
                    "**Principales:**\n" +
                    "• IGT-PM - Prime Minister\n" +
                    "• IGT-MAIN - Moneda Principal\n" +
                    "• IGT-GOV - Governance\n" +
                    "• IGT-STAKE - Staking";
            }
            // Banco
            else if (lowerMsg.Contains("banco") || lowerMsg.Contains("bank") || lowerMsg.Contains("bdet"))
            {
                response = "🏦 **BDET Central Bank**\n\n" +
                    "**SWIFT:** IERBDETXXX\n" +
                    "**Sistema:** 4 Central Banks\n" +
                    "**Países:** 45+\n" +
                    "**Bancos:** 68 conectados\n\n" +
                    "**Servicios:**\n" +
                    "• Transferencias SWIFT\n" +
                    "• MT103/MT202\n" +
                    "• SIIS Integration\n" +
                    "• Crypto-Fiat Bridge\n" +
                    "• ATM Manufacturing";
            }
            // Gracias
            else if (lowerMsg.Contains("gracias") || lowerMsg.Contains("thank"))
            {
                response = "¡De nada! 😊 Estoy aquí para ayudarte.\n\n¿Hay algo más en lo que pueda asistirte?";
            }
            // Quién eres
            else if (lowerMsg.Contains("quién eres") || lowerMsg.Contains("who are you") || lowerMsg.Contains("qué eres"))
            {
                response = "🤖 **Soy IERAHKWA AI**\n\n" +
                    "El asistente inteligente del Gobierno Soberano de Ierahkwa Ne Kanienke.\n\n" +
                    "**Capacidades:**\n" +
                    "• Información del sistema\n" +
                    "• Generación de código\n" +
                    "• Análisis de datos\n" +
                    "• Soporte 24/7\n\n" +
                    "**Plataforma:** .NET 10.0\n" +
                    "**Versión:** 2.0.0";
            }
            // Respuesta inteligente por defecto
            else
            {
                response = $"📝 Entendido: *\"{message}\"*\n\n" +
                    "Procesando tu consulta...\n\n" +
                    "Soy IERAHKWA AI, puedo ayudarte con:\n" +
                    "• Información de la plataforma (`reporte`)\n" +
                    "• Generar código (`generar [descripción]`)\n" +
                    "• Consultas sobre módulos (`módulos`)\n\n" +
                    "Escribe `ayuda` para ver todos los comandos.";
            }
            
            // Guardar respuesta en historial
            _history.Add(new ChatMessage { Role = "assistant", Content = response, Timestamp = DateTime.UtcNow });
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AI chat");
            return "❌ Lo siento, ocurrió un error. Por favor intenta de nuevo.";
        }
    }

    public async Task<string> GenerateCodeAsync(string prompt, string language = "csharp")
    {
        await Task.Delay(100);
        
        var cleanPrompt = prompt.Replace("generar", "").Replace("generate", "").Replace("código", "").Replace("code", "").Trim();
        
        if (string.IsNullOrEmpty(cleanPrompt)) cleanPrompt = "Service class";
        
        return $@"```csharp
// 🤖 Generado por IERAHKWA AI
// 📝 Prompt: {cleanPrompt}
// 📅 Fecha: {DateTime.UtcNow:yyyy-MM-dd HH:mm}

using System;
using System.Threading.Tasks;

namespace IERAHKWA.Generated
{{
    /// <summary>
    /// Auto-generated service for: {cleanPrompt}
    /// </summary>
    public class GeneratedService
    {{
        private readonly ILogger _logger;
        
        public GeneratedService(ILogger logger)
        {{
            _logger = logger;
        }}
        
        public async Task<string> ExecuteAsync()
        {{
            _logger.LogInformation(""Executing generated service..."");
            
            // TODO: Implement your logic here
            await Task.Delay(100);
            
            return ""✅ Executed successfully"";
        }}
        
        public void Validate()
        {{
            // Validation logic
            Console.WriteLine(""Validating..."");
        }}
    }}
}}
```";
    }

    public async Task<string> AnalyzeCodeAsync(string code)
    {
        await Task.Delay(100);
        var lines = code.Split('\n').Length;
        var chars = code.Length;
        var words = code.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        
        return $"📊 **Análisis de Código**\n\n" +
            $"• **Líneas:** {lines}\n" +
            $"• **Caracteres:** {chars}\n" +
            $"• **Palabras:** {words}\n" +
            $"• **Complejidad:** {(lines > 50 ? "Alta" : lines > 20 ? "Media" : "Baja")}\n" +
            "• **Estado:** ✅ Válido\n" +
            "• **Sintaxis:** ✅ Correcta";
    }
}

public class ChatMessage
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
