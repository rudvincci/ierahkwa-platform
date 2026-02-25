namespace Ierahkwa.Gateway.Configuration;

public static class GatewayRoutes
{
    public static object GetServiceCatalog() => new
    {
        // NEXUS Orbital — Science & Technology (8 services)
        nexus_orbital = new
        {
            color = "#00bcd4",
            services = new[]
            {
                new { name = "SpaceService", path = "/api/space", port = 5100 },
                new { name = "TelecomService", path = "/api/telecom", port = 5101 },
                new { name = "GenomicsService", path = "/api/genomics", port = 5102 },
                new { name = "IoTRoboticsService", path = "/api/iot-robotics", port = 5103 },
                new { name = "QuantumService", path = "/api/quantum", port = 5104 },
                new { name = "AIEngineService", path = "/api/ai-engine", port = 5105 },
                new { name = "NetworkService", path = "/api/network", port = 5106 },
                new { name = "DevToolsService", path = "/api/devtools", port = 5107 }
            }
        },
        // NEXUS Escudo — Defense & Security (5 services)
        nexus_escudo = new
        {
            color = "#f44336",
            services = new[]
            {
                new { name = "MilitaryService", path = "/api/military", port = 5200 },
                new { name = "DroneService", path = "/api/drones", port = 5201 },
                new { name = "CyberSecService", path = "/api/cybersec", port = 5202 },
                new { name = "IntelligenceService", path = "/api/intelligence", port = 5203 },
                new { name = "EmergencyService", path = "/api/emergency", port = 5204 }
            }
        },
        // NEXUS Cerebro — Education & Research (4 services)
        nexus_cerebro = new
        {
            color = "#7c4dff",
            services = new[]
            {
                new { name = "EducationService", path = "/api/education", port = 5300 },
                new { name = "ResearchService", path = "/api/research", port = 5301 },
                new { name = "LanguageService", path = "/api/language", port = 5302 },
                new { name = "SearchService", path = "/api/search", port = 5303 }
            }
        },
        // NEXUS Tesoro — Economy & Finance (8 services)
        nexus_tesoro = new
        {
            color = "#ffd600",
            services = new[]
            {
                new { name = "CommerceService", path = "/api/commerce", port = 5400 },
                new { name = "BlockchainService", path = "/api/blockchain", port = 5401 },
                new { name = "BankingService", path = "/api/banking", port = 5402 },
                new { name = "InsuranceService", path = "/api/insurance", port = 5403 },
                new { name = "EmploymentService", path = "/api/employment", port = 5404 },
                new { name = "SmartFactoryService", path = "/api/smart-factory", port = 5405 },
                new { name = "ArtisanService", path = "/api/artisan", port = 5406 },
                new { name = "TourismService", path = "/api/tourism", port = 5407 }
            }
        },
        // NEXUS Voces — Culture & Communication (5 services)
        nexus_voces = new
        {
            color = "#e040fb",
            services = new[]
            {
                new { name = "MediaContentService", path = "/api/media", port = 5500 },
                new { name = "MessagingService", path = "/api/messaging", port = 5501 },
                new { name = "CultureArchiveService", path = "/api/culture", port = 5502 },
                new { name = "SportsService", path = "/api/sports", port = 5503 },
                new { name = "SocialService", path = "/api/social", port = 5504 }
            }
        },
        // NEXUS Consejo — Governance & Law (5 services)
        nexus_consejo = new
        {
            color = "#1565c0",
            services = new[]
            {
                new { name = "GovernanceService", path = "/api/governance", port = 5600 },
                new { name = "JusticeService", path = "/api/justice", port = 5601 },
                new { name = "DiplomacyService", path = "/api/diplomacy", port = 5602 },
                new { name = "CitizenService", path = "/api/citizen", port = 5603 },
                new { name = "SocialWelfareService", path = "/api/social-welfare", port = 5604 }
            }
        },
        // NEXUS Tierra — Environment & Resources (5 services)
        nexus_tierra = new
        {
            color = "#43a047",
            services = new[]
            {
                new { name = "AgricultureService", path = "/api/agriculture", port = 5700 },
                new { name = "NaturalResourceService", path = "/api/natural-resource", port = 5701 },
                new { name = "EnvironmentService", path = "/api/environment", port = 5702 },
                new { name = "WasteService", path = "/api/waste", port = 5703 },
                new { name = "EnergyService", path = "/api/energy", port = 5704 }
            }
        },
        // NEXUS Forja — Technology & Innovation (5 services)
        nexus_forja = new
        {
            color = "#00e676",
            services = new[]
            {
                new { name = "DevOpsService", path = "/api/devops", port = 5800 },
                new { name = "LowCodeDesignService", path = "/api/lowcode", port = 5801 },
                new { name = "BrowserService", path = "/api/browser", port = 5802 },
                new { name = "ProductivityService", path = "/api/productivity", port = 5803 },
                new { name = "CloudService", path = "/api/cloud", port = 5804 }
            }
        },
        // NEXUS Urbe — Infrastructure & Urban (4 services)
        nexus_urbe = new
        {
            color = "#ff9100",
            services = new[]
            {
                new { name = "UrbanService", path = "/api/urban", port = 5900 },
                new { name = "TransportService", path = "/api/transport", port = 5901 },
                new { name = "PostalMapsService", path = "/api/postal-maps", port = 5902 },
                new { name = "HousingService", path = "/api/housing", port = 5903 }
            }
        },
        // NEXUS Raíces — Identity & Heritage (4 services)
        nexus_raices = new
        {
            color = "#d4a853",
            services = new[]
            {
                new { name = "IdentityService", path = "/api/identity", port = 6000 },
                new { name = "HealthService", path = "/api/health", port = 6001 },
                new { name = "NexusAggregationService", path = "/api/nexus", port = 6002 },
                new { name = "LicensingService", path = "/api/licensing", port = 6003 }
            }
        },
        // Cross-cutting services
        platform = new
        {
            services = new[]
            {
                new { name = "Gateway", path = "/", port = 5000 },
                new { name = "FWIDIdentity", path = "/api/fwid", port = 5001 }
            }
        }
    };
}
