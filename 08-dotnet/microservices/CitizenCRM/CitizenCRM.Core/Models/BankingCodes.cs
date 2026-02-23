using System;
using System.Collections.Generic;

namespace CitizenCRM.Core.Models
{
    /// <summary>
    /// Sistema de Códigos Bancarios - Nombres Indígenas de las Américas
    /// Sovereign Government of Ierahkwa Ne Kanienke
    /// 
    /// ESTRUCTURA BANCARIA CON SÍMBOLOS INDÍGENAS:
    /// 1. BANCO CENTRAL - WAMPUM (Cinturón sagrado)
    /// 2. BANCOS REGIONALES - ÁGUILA, CÓNDOR, QUETZAL, CARIBE
    /// 3. BANCOS NACIONALES - TAÍNO y aliados
    /// 4. INSTITUCIONES - Fondos y cooperativas
    /// </summary>
    public static class BankingCodes
    {
        // ============================================
        // 1. BANCO CENTRAL - WAMPUM CENTRAL BANK
        // El Wampum es el registro sagrado de valor
        // ============================================

        public static class CentralBank
        {
            public const string Code = "WAMPUM";
            public const string FullCode = "WCB";  // Wampum Central Bank
            public const string Name = "Wampum Central Bank";
            public const string NameNative = "Teiotià:kon Ohwista'shón:'a";
            public const string SwiftBIC = "WAMPUSIE";
            
            public static BankInfo Info => new()
            {
                BankCode = Code,
                ShortCode = FullCode,
                BankName = Name,
                BankNameNative = NameNative,
                BankType = "CENTRAL",
                Symbol = "🪶",
                SwiftBIC = SwiftBIC,
                Description = "Banco Central Soberano - Guardián del Wampum sagrado",
                Functions = new List<string>
                {
                    "Emisión de moneda soberana",
                    "Reservas internacionales",
                    "Custodia de activos VIP",
                    "Política monetaria",
                    "Transacciones gobierno a gobierno"
                }
            };

            // Tipos de transacción Banco Central
            public const string TX_RESERVE = "WAMPUM-RES";    // Reserva
            public const string TX_CUSTODY = "WAMPUM-CUS";    // Custodia
            public const string TX_EMISSION = "WAMPUM-EMI";   // Emisión
            public const string TX_SOVEREIGN = "WAMPUM-SOV";  // Soberano
        }

        // ============================================
        // 2. BANCOS REGIONALES - Símbolos de las Américas
        // ============================================

        public static class RegionalBanks
        {
            // 🦅 ÁGUILA - Norte (Norteamérica)
            public static BankInfo Aguila => new()
            {
                BankCode = "AGUILA",
                ShortCode = "AGB",
                BankName = "Banco Águila",
                BankNameNative = "Eagle Bank - Ohkwá:ri Ohwista'shón:'a",
                BankType = "REGIONAL",
                Symbol = "🦅",
                SwiftBIC = "AGLAIEXX",
                Region = "NORTE",
                Territory = "Norteamérica - Turtle Island",
                Description = "Banco Regional Norte - El Águila representa la visión y el poder del Norte",
                Symbolism = "El Águila vuela más alto, ve más lejos - Mensajero entre el cielo y la tierra",
                Functions = new List<string>
                {
                    "Transacciones Norteamérica",
                    "Comercio USA/Canadá",
                    "Reservas del Norte",
                    "Naciones indígenas del Norte"
                }
            };

            // 🦅 CÓNDOR - Sur (Sudamérica)
            public static BankInfo Condor => new()
            {
                BankCode = "CONDOR",
                ShortCode = "CDB",
                BankName = "Banco Cóndor",
                BankNameNative = "Kuntur Bank - Guardián de los Andes",
                BankType = "REGIONAL",
                Symbol = "🦅",
                SwiftBIC = "CONDIEXX",
                Region = "SUR",
                Territory = "Sudamérica - Abya Yala",
                Description = "Banco Regional Sur - El Cóndor representa la sabiduría ancestral del Sur",
                Symbolism = "El Cóndor conecta el mundo de arriba con el de abajo - Símbolo sagrado Andino",
                Functions = new List<string>
                {
                    "Transacciones Sudamérica",
                    "Comercio Andino",
                    "Reservas del Sur",
                    "Naciones indígenas del Sur"
                }
            };

            // 🐦 QUETZAL - Centro (Mesoamérica)
            public static BankInfo Quetzal => new()
            {
                BankCode = "QUETZAL",
                ShortCode = "QZB",
                BankName = "Banco Quetzal",
                BankNameNative = "Quetzalcóatl Bank - Serpiente Emplumada",
                BankType = "REGIONAL",
                Symbol = "🐦",
                SwiftBIC = "QUETIEXX",
                Region = "CENTRO",
                Territory = "Mesoamérica - Maya, Azteca",
                Description = "Banco Regional Centro - El Quetzal representa la libertad y riqueza",
                Symbolism = "Ave sagrada Maya - Símbolo de libertad, no puede vivir en cautiverio",
                Functions = new List<string>
                {
                    "Transacciones Centroamérica",
                    "Comercio Mesoamericano",
                    "Reservas del Centro",
                    "Naciones Maya, Azteca, Olmeca"
                }
            };

            // 🌊 CARIBE - Islas (Caribe)
            public static BankInfo Caribe => new()
            {
                BankCode = "CARIBE",
                ShortCode = "CRB",
                BankName = "Banco Caribe",
                BankNameNative = "Kalinago Bank - Pueblo del Mar",
                BankType = "REGIONAL",
                Symbol = "🌊",
                SwiftBIC = "CARBIEXX",
                Region = "CARIBE",
                Territory = "Islas del Caribe - Antillas",
                Description = "Banco Regional Caribe - El mar une a los pueblos insulares",
                Symbolism = "Los Caribes fueron grandes navegantes - Guerreros del mar",
                Functions = new List<string>
                {
                    "Transacciones Caribe",
                    "Comercio insular",
                    "Reservas del Caribe",
                    "Naciones Kalinago, Taíno, Arawak"
                }
            };

            public static List<BankInfo> All => new() { Aguila, Condor, Quetzal, Caribe };
        }

        // ============================================
        // 3. BANCOS NACIONALES - Pueblos Indígenas
        // ============================================

        public static class NationalBanks
        {
            // 🌺 TAÍNO - Caribe ancestral
            public static BankInfo Taino => new()
            {
                BankCode = "TAINO",
                ShortCode = "TNB",
                BankName = "Banco Taíno",
                BankNameNative = "Taíno National Bank - Bohío de Oro",
                BankType = "NATIONAL",
                Symbol = "🌺",
                SwiftBIC = "TAINIEXX",
                Nation = "Pueblo Taíno",
                Description = "Banco Nacional Taíno - Los buenos y nobles del Caribe",
                Symbolism = "Taíno significa 'los buenos' - Primeros en recibir a Colón",
                Functions = new List<string>
                {
                    "Transacciones pueblo Taíno",
                    "Herencia Boricua/Quisqueya",
                    "Preservación cultural"
                }
            };

            // 🪶 HAUDENOSAUNEE - Confederación Iroquesa
            public static BankInfo Haudenosaunee => new()
            {
                BankCode = "HAUDE",
                ShortCode = "HNB",
                BankName = "Banco Haudenosaunee",
                BankNameNative = "Rotinonhsión:ni Ohwista'shón:'a",
                BankType = "NATIONAL",
                Symbol = "🪶",
                SwiftBIC = "HAUDIEXX",
                Nation = "Confederación Haudenosaunee (Seis Naciones)",
                Description = "Banco de la Confederación - La Casa Larga",
                Symbolism = "Gente de la Casa Larga - Primera democracia de América",
                Functions = new List<string>
                {
                    "Transacciones Seis Naciones",
                    "Mohawk, Oneida, Onondaga, Cayuga, Seneca, Tuscarora",
                    "Comercio confederado"
                }
            };

            // 🦬 LAKOTA - Grandes Planicies
            public static BankInfo Lakota => new()
            {
                BankCode = "LAKOTA",
                ShortCode = "LKB",
                BankName = "Banco Lakota",
                BankNameNative = "Lakota Oyate Bank - Nación del Búfalo",
                BankType = "NATIONAL",
                Symbol = "🦬",
                SwiftBIC = "LAKTIEXX",
                Nation = "Nación Lakota Sioux",
                Description = "Banco Nacional Lakota - Guerreros de las Planicies",
                Symbolism = "El búfalo provee todo - Símbolo de abundancia",
                Functions = new List<string>
                {
                    "Transacciones Lakota/Dakota/Nakota",
                    "Reservas de las Planicies",
                    "Black Hills Fund"
                }
            };

            // 🌿 MAPUCHE - Sur de Chile/Argentina
            public static BankInfo Mapuche => new()
            {
                BankCode = "MAPUCHE",
                ShortCode = "MPB",
                BankName = "Banco Mapuche",
                BankNameNative = "Mapu Che Bank - Gente de la Tierra",
                BankType = "NATIONAL",
                Symbol = "🌿",
                SwiftBIC = "MAPUIEXX",
                Nation = "Nación Mapuche",
                Description = "Banco Nacional Mapuche - Nunca conquistados",
                Symbolism = "Gente de la tierra - Resistencia inquebrantable",
                Functions = new List<string>
                {
                    "Transacciones Wallmapu",
                    "Comercio sur austral",
                    "Reservas Mapuche"
                }
            };

            // ☀️ INCA - Tawantinsuyu
            public static BankInfo Inca => new()
            {
                BankCode = "INCA",
                ShortCode = "ICB",
                BankName = "Banco Inca",
                BankNameNative = "Tawantinsuyu Bank - Hijos del Sol",
                BankType = "NATIONAL",
                Symbol = "☀️",
                SwiftBIC = "INCAIEXX",
                Nation = "Herencia Inca - Quechua/Aymara",
                Description = "Banco Nacional Inca - El gran imperio andino",
                Symbolism = "Inti - El Sol es padre de los Incas",
                Functions = new List<string>
                {
                    "Transacciones Andinas",
                    "Perú, Bolivia, Ecuador",
                    "Comercio Quechua/Aymara"
                }
            };

            // 🌙 MAYA - Mesoamérica
            public static BankInfo Maya => new()
            {
                BankCode = "MAYA",
                ShortCode = "MYB",
                BankName = "Banco Maya",
                BankNameNative = "K'iche' Bank - Guardianes del Tiempo",
                BankType = "NATIONAL",
                Symbol = "🌙",
                SwiftBIC = "MAYAIEXX",
                Nation = "Naciones Maya",
                Description = "Banco Nacional Maya - Astrónomos del cosmos",
                Symbolism = "Guardianes del calendario sagrado y las estrellas",
                Functions = new List<string>
                {
                    "Transacciones Maya",
                    "Guatemala, México, Belice, Honduras",
                    "K'iche', Yucateco, Tzotzil, etc."
                }
            };

            public static List<BankInfo> All => new() 
            { 
                Taino, Haudenosaunee, Lakota, Mapuche, Inca, Maya 
            };
        }

        // ============================================
        // 4. INSTITUCIONES FINANCIERAS
        // ============================================

        public static class Institutions
        {
            // Fondo Soberano
            public static BankInfo SovereignFund => new()
            {
                BankCode = "PACHAMAMA",
                ShortCode = "PMF",
                BankName = "Fondo Pachamama",
                BankNameNative = "Pachamama Sovereign Fund - Madre Tierra",
                BankType = "SOVEREIGN_FUND",
                Symbol = "🌍",
                Description = "Fondo Soberano - Riqueza para las generaciones futuras",
                Symbolism = "Pachamama - La Madre Tierra nos sostiene a todos"
            };

            // Tesorería
            public static BankInfo Treasury => new()
            {
                BankCode = "BOHIO",
                ShortCode = "BTR",
                BankName = "Tesorería Bohío",
                BankNameNative = "Bohío Treasury - Casa del Tesoro",
                BankType = "TREASURY",
                Symbol = "🏛️",
                Description = "Tesorería del Gobierno - Manejo de fondos públicos",
                Symbolism = "Bohío - Casa tradicional Taína"
            };

            // Cooperativa
            public static BankInfo Cooperative => new()
            {
                BankCode = "AYLLU",
                ShortCode = "AYC",
                BankName = "Cooperativa Ayllu",
                BankNameNative = "Ayllu Cooperative - Comunidad",
                BankType = "COOPERATIVE",
                Symbol = "🤝",
                Description = "Cooperativa de ahorro - Para el pueblo",
                Symbolism = "Ayllu - Sistema comunitario andino de ayuda mutua"
            };

            // Fondo de Desarrollo
            public static BankInfo Development => new()
            {
                BankCode = "BUEN-VIVIR",
                ShortCode = "BVD",
                BankName = "Fondo Buen Vivir",
                BankNameNative = "Sumak Kawsay Fund - Vida Plena",
                BankType = "DEVELOPMENT_FUND",
                Symbol = "🌱",
                Description = "Fondo de Desarrollo - Armonía con la naturaleza",
                Symbolism = "Sumak Kawsay - Filosofía andina del buen vivir"
            };

            public static List<BankInfo> All => new() 
            { 
                SovereignFund, Treasury, Cooperative, Development 
            };
        }

        // ============================================
        // GENERADOR DE CÓDIGOS
        // ============================================

        /// <summary>
        /// Genera código de transacción
        /// Formato: [BANCO]-[TIPO]-[AAMM]-[SEQ]
        /// </summary>
        public static string GenerateCode(string bankCode, string txType, int sequence)
        {
            var now = DateTime.UtcNow;
            return $"{bankCode}-{txType}-{now:yyMM}-{sequence:D4}";
        }

        /// <summary>
        /// Tipos de transacción
        /// </summary>
        public static class TxTypes
        {
            public const string DEP = "DEP";   // Depósito
            public const string WIT = "WIT";   // Retiro
            public const string TRF = "TRF";   // Transferencia
            public const string PAY = "PAY";   // Pago
            public const string RES = "RES";   // Reserva
            public const string CUS = "CUS";   // Custodia
            public const string INV = "INV";   // Inversión
            public const string LOC = "LOC";   // Local
            public const string SAL = "SAL";   // Saliente
        }

        /// <summary>
        /// Ejemplos de códigos
        /// </summary>
        public static Dictionary<string, string> ExampleCodes => new()
        {
            // Banco Central
            { "WAMPUM-CUS-2601-0001", "Banco Central - Custodia de activo" },
            { "WAMPUM-RES-2601-0001", "Banco Central - Reserva" },
            
            // Regionales
            { "AGUILA-TRF-2601-0001", "Banco Águila - Transferencia Norte" },
            { "CONDOR-TRF-2601-0001", "Banco Cóndor - Transferencia Sur" },
            { "QUETZAL-TRF-2601-0001", "Banco Quetzal - Transferencia Centro" },
            { "CARIBE-TRF-2601-0001", "Banco Caribe - Transferencia Islas" },
            
            // Nacionales
            { "TAINO-LOC-2601-0001", "Banco Taíno - Transacción local" },
            { "HAUDE-SAL-2601-0001", "Banco Haudenosaunee - Saliente" },
            
            // Instituciones
            { "PACHAMAMA-INV-2601-0001", "Fondo Pachamama - Inversión" },
            { "AYLLU-DEP-2601-0001", "Cooperativa Ayllu - Depósito" }
        };

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        public static List<BankInfo> GetAllBanks()
        {
            var banks = new List<BankInfo> { CentralBank.Info };
            banks.AddRange(RegionalBanks.All);
            banks.AddRange(NationalBanks.All);
            banks.AddRange(Institutions.All);
            return banks;
        }
    }

    /// <summary>
    /// Información de banco
    /// </summary>
    public class BankInfo
    {
        public string BankCode { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string BankNameNative { get; set; } = string.Empty;
        public string BankType { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string SwiftBIC { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Territory { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Symbolism { get; set; } = string.Empty;
        public List<string> Functions { get; set; } = new();
    }
}
