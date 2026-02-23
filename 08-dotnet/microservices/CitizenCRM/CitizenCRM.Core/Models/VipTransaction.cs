using System;
using System.Collections.Generic;

namespace CitizenCRM.Core.Models
{
    /// <summary>
    /// Sistema de Códigos de Transacciones VIP
    /// Sovereign Government of Ierahkwa Ne Kanienke
    /// 
    /// REGLA DE CODIFICACIÓN:
    /// - ENTRANTES (INCOMING): Mantienen código original (SWIFT, UETR, TRN del emisor)
    /// - LOCALES: Código indígena Ierahkwa
    /// - SALIENTES (OUTGOING): Código indígena Ierahkwa
    /// </summary>
    public class VipTransaction
    {
        public string IerahkwaCode { get; set; } = string.Empty;      // Nuestro código (solo local/saliente)
        public string OriginalCode { get; set; } = string.Empty;      // Código original (SWIFT, UETR, etc.)
        public string TransactionType { get; set; } = string.Empty;   // INCOMING, LOCAL, OUTGOING
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string Status { get; set; } = "Pending";
        public int CompletionPercentage { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string FolderPath { get; set; } = string.Empty;
        public int DocumentCount { get; set; }
        public List<string> MissingItems { get; set; } = new();
        public List<string> RequiredActions { get; set; } = new();
        public TransactionDetails Details { get; set; } = new();
    }

    public class TransactionDetails
    {
        public string UETR { get; set; } = string.Empty;
        public string TRN { get; set; } = string.Empty;
        public string SwiftReference { get; set; } = string.Empty;
        public string SenderBank { get; set; } = string.Empty;
        public string ReceiverBank { get; set; } = string.Empty;
        public string SenderAccount { get; set; } = string.Empty;
        public string ReceiverAccount { get; set; } = string.Empty;
        public DateTime? ValueDate { get; set; }
        public string MessageType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Sistema de Códigos Ierahkwa
    /// 
    /// SOLO PARA TRANSACCIONES LOCALES Y SALIENTES
    /// Las entrantes mantienen su código original (SWIFT, UETR, TRN)
    /// 
    /// FORMATO CÓDIGO INDÍGENA: [PREFIJO]-[TIPO]-[AÑO][MES]-[SECUENCIA]
    /// Ejemplo: WAMPUM-SAL-2601-001 = Saliente, Enero 2026, #001
    /// 
    /// PREFIJOS MOHAWK:
    /// - WAMPUM = Transferencias financieras (registro sagrado)
    /// - OHWISTA = Activos/Riqueza
    /// - KANATA = Gobierno/Soberano
    /// - KARIWIIO = Digital/Crypto
    /// - TEKENI = Conexiones bilaterales
    /// - ONKWEHONWE = Del pueblo
    /// 
    /// TIPOS:
    /// - SAL = Saliente (Outgoing)
    /// - LOC = Local
    /// - INT = Interno
    /// </summary>
    public static class TransactionCodes
    {
        // ============================================
        // TRANSACCIONES ENTRANTES (INCOMING)
        // Mantienen código original del emisor
        // ============================================

        // Estas transacciones YA tienen su código:
        // - MT103 tiene TRN y UETR del banco emisor
        // - SWIFT tiene referencia del emisor
        // - Crypto tiene TX Hash de blockchain

        // ============================================
        // TRANSACCIONES LOCALES Y SALIENTES
        // Reciben código indígena Ierahkwa
        // ============================================

        /// <summary>
        /// Diccionario de Palabras Mohawk
        /// </summary>
        public static Dictionary<string, string> MohawkDictionary => new()
        {
            { "WAMPUM", "Cinturón sagrado - Registro de valor y tratados" },
            { "OHWISTA", "Dinero, riqueza, tesoro" },
            { "KANATA", "Territorio soberano, nación" },
            { "KARIWIIO", "Buena palabra, mensaje verdadero" },
            { "TEKENI", "Dos - Conexión bilateral" },
            { "ONKWEHONWE", "Pueblo original" },
            { "SKENNEN", "Paz" },
            { "IERAHKWA", "Nombre del gobierno" }
        };

        /// <summary>
        /// Genera código Ierahkwa para transacción local o saliente
        /// </summary>
        public static string GenerateIerahkwaCode(string prefix, string type, int sequence)
        {
            var now = DateTime.UtcNow;
            var yearMonth = $"{now:yy}{now:MM}";
            return $"{prefix}-{type}-{yearMonth}-{sequence:D3}";
        }

        /// <summary>
        /// Obtiene todas las transacciones VIP
        /// </summary>
        public static List<VipTransaction> GetAllTransactions()
        {
            return new List<VipTransaction>
            {
                // ===== ENTRANTES (INCOMING) - Código del emisor =====
                
                new VipTransaction
                {
                    IerahkwaCode = "—",  // No aplica, es entrante
                    OriginalCode = "MT103-TRN-PENDING",  // Extraer de PDF
                    TransactionType = "INCOMING",
                    Name = "IBAN TO IBAN MT103 CASH TRANSFER STP",
                    Description = "Deutsche Bank MT103 - Código viene del banco emisor",
                    Category = "SWIFT",
                    Amount = 1_000_000_000M,
                    Currency = "USD",
                    Status = "In Progress",
                    CompletionPercentage = 90,
                    Priority = "Critical",
                    FolderPath = "trasaciones vip/IBAN TO IBAN MT103 CASH TRANSFER STP",
                    DocumentCount = 9,
                    MissingItems = new List<string> { "UETR (del emisor)", "TRN (del emisor)" },
                    RequiredActions = new List<string> 
                    { 
                        "Extraer TRN de 002 - MT199.pdf",
                        "Extraer UETR del mensaje SWIFT",
                        "El código es del BANCO EMISOR"
                    },
                    Details = new TransactionDetails
                    {
                        MessageType = "MT103",
                        ReceiverBank = "Deutsche Bank"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "MT103-TRN-PENDING",
                    TransactionType = "INCOMING",
                    Name = "STP MT103 CELOS INVEST - 1B",
                    Description = "CELOS INVEST - Código viene del banco emisor",
                    Category = "SWIFT",
                    Amount = 1_000_000_000M,
                    Currency = "USD",
                    Status = "In Progress",
                    CompletionPercentage = 85,
                    Priority = "Critical",
                    FolderPath = "trasaciones vip/STP MT103/1B",
                    DocumentCount = 16,
                    MissingItems = new List<string> { "UETR (del emisor)", "TRN (de cada PDF)" },
                    RequiredActions = new List<string>
                    {
                        "Extraer TRN de los 16 PDFs",
                        "El código es del BANCO EMISOR"
                    },
                    Details = new TransactionDetails
                    {
                        MessageType = "MT103",
                        ValueDate = new DateTime(2024, 9, 27)
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "MT103-TRN-PENDING",
                    TransactionType = "INCOMING",
                    Name = "STP MT103 CELOS INVEST - 5B",
                    Description = "CELOS INVEST - Código viene del banco emisor",
                    Category = "SWIFT",
                    Amount = 5_000_000_000M,
                    Currency = "USD",
                    Status = "In Progress",
                    CompletionPercentage = 85,
                    Priority = "Critical",
                    FolderPath = "trasaciones vip/STP MT103/5B",
                    DocumentCount = 14,
                    MissingItems = new List<string> { "UETR (del emisor)", "TRN (de cada PDF)" },
                    RequiredActions = new List<string>
                    {
                        "Extraer TRN de los 14 PDFs",
                        "El código es del BANCO EMISOR"
                    },
                    Details = new TransactionDetails
                    {
                        MessageType = "MT103",
                        ValueDate = new DateTime(2024, 10, 3)
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "SWIFT-REF-PENDING",
                    TransactionType = "INCOMING",
                    Name = "INTERNATIONAL SWIFT ACKS",
                    Description = "UBS London - Código viene del emisor UBS",
                    Category = "SWIFT",
                    Status = "In Progress",
                    CompletionPercentage = 80,
                    Priority = "High",
                    FolderPath = "trasaciones vip/INTERNATIONAL SWIFT ACKS - CUSTOMER'S COPY",
                    DocumentCount = 1,
                    MissingItems = new List<string> { "SWIFT Reference (del PDF)", "UETR (de UBS)" },
                    RequiredActions = new List<string>
                    {
                        "Extraer referencia SWIFT del PDF",
                        "El código es de UBS LONDON"
                    },
                    Details = new TransactionDetails
                    {
                        SenderBank = "UBS LONDON AG",
                        ReceiverBank = "DEUTSCHE BANK"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "BCV-REF-PENDING",
                    TransactionType = "INCOMING",
                    Name = "VENEZUELA BCV EUROCLEAR",
                    Description = "BCV Venezuela - Código viene del Banco Central",
                    Category = "Government",
                    Amount = 4_200_000_000M,
                    Currency = "USD",
                    Status = "Pending",
                    CompletionPercentage = 60,
                    Priority = "High",
                    FolderPath = "trasaciones vip/venezuea trasacion",
                    DocumentCount = 4,
                    MissingItems = new List<string> 
                    { 
                        "Referencia BCV (del PDF)", 
                        "Número cuenta EuroClear" 
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer referencia del BCV",
                        "El código es del BANCO CENTRAL DE VENEZUELA"
                    },
                    Details = new TransactionDetails
                    {
                        SenderBank = "Banco Central de Venezuela"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "ISIN/CUSIP-PENDING",
                    TransactionType = "INCOMING",
                    Name = "BONOS HISTÓRICOS",
                    Description = "Bonos - Códigos ISIN/CUSIP del emisor original",
                    Category = "Government",
                    Status = "Pending",
                    CompletionPercentage = 50,
                    Priority = "Medium",
                    FolderPath = "trasaciones vip/bonos historicos",
                    DocumentCount = 1632,
                    MissingItems = new List<string>
                    {
                        "ISIN/CUSIP de cada bono",
                        "Los códigos son del EMISOR ORIGINAL"
                    },
                    RequiredActions = new List<string>
                    {
                        "Inventariar códigos ISIN/CUSIP",
                        "Cada bono tiene su código del emisor"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "TX-HASH-PENDING",
                    TransactionType = "INCOMING",
                    Name = "CRYPTOHOST SYSTEM",
                    Description = "Crypto - TX Hash viene de la blockchain",
                    Category = "Crypto",
                    Status = "In Progress",
                    CompletionPercentage = 55,
                    Priority = "Medium",
                    FolderPath = "trasaciones vip/cryptohost system",
                    DocumentCount = 7,
                    MissingItems = new List<string>
                    {
                        "TX Hash (de blockchain)",
                        "El código es el HASH de la transacción"
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer TX Hash del certificado",
                        "Verificar en blockchain explorer"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "API-REF-PENDING",
                    TransactionType = "INCOMING",
                    Name = "API TO API",
                    Description = "API - Referencias vienen del sistema emisor",
                    Category = "API",
                    Status = "Pending",
                    CompletionPercentage = 50,
                    Priority = "Medium",
                    FolderPath = "trasaciones vip/api to api",
                    DocumentCount = 5,
                    MissingItems = new List<string>
                    {
                        "Referencias de cada PDF",
                        "Los códigos son del SISTEMA EMISOR"
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer referencias de los 5 PDFs"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "IP-TRANSFER-PENDING",
                    TransactionType = "INCOMING",
                    Name = "IP SPECIAL CASH TRANSFER",
                    Description = "IP Transfer - Código viene del emisor",
                    Category = "API",
                    Status = "Pending",
                    CompletionPercentage = 50,
                    Priority = "Medium",
                    FolderPath = "trasaciones vip/TRANSMITED BY IP:IP SPECIAL CASH TRANSFER MESSAGE",
                    DocumentCount = 1,
                    MissingItems = new List<string>
                    {
                        "IP Transfer ID (del PDF)",
                        "El código es del EMISOR"
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer IP Transfer ID del PDF"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "WISE-ID-PENDING",
                    TransactionType = "INCOMING",
                    Name = "WISE PORT",
                    Description = "WISE - Transfer ID viene de Wise.com",
                    Category = "API",
                    Status = "Pending",
                    CompletionPercentage = 45,
                    Priority = "Low",
                    FolderPath = "trasaciones vip/WISE PORT",
                    DocumentCount = 1,
                    MissingItems = new List<string>
                    {
                        "WISE Transfer ID (del PDF o dashboard)",
                        "El código es de WISE"
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer Transfer ID de WISE"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "—",
                    OriginalCode = "CARD-AUTH-PENDING",
                    TransactionType = "INCOMING",
                    Name = "VISA MASTERCARD",
                    Description = "Tarjetas - Códigos de autorización del banco",
                    Category = "Card",
                    Status = "Pending",
                    CompletionPercentage = 40,
                    Priority = "Low",
                    FolderPath = "trasaciones vip/targeta visa y mastercar",
                    DocumentCount = 10,
                    MissingItems = new List<string>
                    {
                        "Códigos de autorización",
                        "Los códigos son del BANCO EMISOR"
                    },
                    RequiredActions = new List<string>
                    {
                        "Extraer códigos de autorización del estado de cuenta"
                    }
                },

                // ===== LOCALES - Código Ierahkwa =====
                
                new VipTransaction
                {
                    IerahkwaCode = "OHWISTA-LOC-2601-001",
                    OriginalCode = "N/A",
                    TransactionType = "LOCAL",
                    Name = "RUBÍ 3",
                    Description = "Activo local - CÓDIGO IERAHKWA asignado",
                    Category = "OHWISTA",
                    Status = "In Progress",
                    CompletionPercentage = 75,
                    Priority = "High",
                    FolderPath = "trasaciones vip/rubi 3",
                    DocumentCount = 15,
                    MissingItems = new List<string>
                    {
                        "Certificación GIA/IGI",
                        "Peso en quilates",
                        "Tasación USD"
                    },
                    RequiredActions = new List<string>
                    {
                        "Este activo LOCAL tiene código IERAHKWA",
                        "Registrar en sistema con OHWISTA-LOC-2601-001"
                    }
                },

                new VipTransaction
                {
                    IerahkwaCode = "OHWISTA-LOC-2601-002",
                    OriginalCode = "N/A",
                    TransactionType = "LOCAL",
                    Name = "ALEXANDRITE",
                    Description = "Activo local - CÓDIGO IERAHKWA asignado",
                    Category = "OHWISTA",
                    Status = "Pending",
                    CompletionPercentage = 40,
                    Priority = "Medium",
                    FolderPath = "trasaciones vip/Alexandrite",
                    DocumentCount = 6,
                    MissingItems = new List<string>
                    {
                        "Certificación GIA/IGI",
                        "Peso en quilates",
                        "Tasación USD"
                    },
                    RequiredActions = new List<string>
                    {
                        "Este activo LOCAL tiene código IERAHKWA",
                        "Registrar en sistema con OHWISTA-LOC-2601-002"
                    }
                }
            };
        }

        public static TransactionSummary GetSummary()
        {
            var transactions = GetAllTransactions();
            return new TransactionSummary
            {
                TotalTransactions = transactions.Count,
                TotalValueUSD = transactions.Where(t => t.Amount.HasValue).Sum(t => t.Amount!.Value),
                IncomingCount = transactions.Count(t => t.TransactionType == "INCOMING"),
                LocalCount = transactions.Count(t => t.TransactionType == "LOCAL"),
                OutgoingCount = transactions.Count(t => t.TransactionType == "OUTGOING"),
                AverageCompletion = (int)transactions.Average(t => t.CompletionPercentage)
            };
        }
    }

    public class TransactionSummary
    {
        public int TotalTransactions { get; set; }
        public decimal TotalValueUSD { get; set; }
        public int IncomingCount { get; set; }
        public int LocalCount { get; set; }
        public int OutgoingCount { get; set; }
        public int AverageCompletion { get; set; }
    }
}
