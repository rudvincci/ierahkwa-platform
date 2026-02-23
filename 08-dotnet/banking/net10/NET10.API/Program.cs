using NET10.Core.Interfaces;
using NET10.Core.Models;
using NET10.Infrastructure.Services;
using NET10.Infrastructure.Services.ERP;

using NET10.Infrastructure.Services.Hotel;
using NET10.Infrastructure.Services.WebERP;
using NET10.Infrastructure.Services.College;
using NET10.Infrastructure.Services.Banking;
using NET10.Infrastructure.Services.CyberCafe;
using NET10.Infrastructure.Services.Hospital;
using NET10.Infrastructure.Services.Inventory;
using NET10.Infrastructure.Services.Finance;
using NET10.Infrastructure.Services.SportsBetting;
using NET10.Infrastructure.Services.GoogleMapsScraper;
using NET10.Infrastructure.Services.Casino;
using NET10.Infrastructure.Services.Lotto;
using NET10.Infrastructure.Services.Raffle;
using NET10.Core.ERP;
using Microsoft.AspNetCore.Server.Kestrel.Core;
var builder = WebApplication.CreateBuilder(args);

// Deshabilitar HTTPS completamente - usar solo HTTP sin certificados
builder.WebHost.UseUrls("http://localhost:5071");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5071, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
});

// ========================================
// Ierahkwa NET10 DeFi Platform - .NET 10
// Sovereign Government DeFi Exchange
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Ierahkwa NET10 DeFi API",
        Version = "v1",
        Description = "Sovereign DeFi Platform API - Swap, Liquidity, Farming",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Sovereign Government of Ierahkwa Ne Kanienke",
            Url = new Uri("https://ierahkwa.gov")
        }
    });
});

// Configure NET10 Platform
var net10Config = new NET10Config
{
    PlatformName = builder.Configuration["NET10:PlatformName"] ?? "Ierahkwa NET10 DeFi",
    DefaultChainId = builder.Configuration["NET10:ChainId"] ?? "777777",
    NodeEndpoint = builder.Configuration["NET10:NodeEndpoint"] ?? "https://node.ierahkwa.gov",
    AdminFeePercent = decimal.TryParse(builder.Configuration["NET10:AdminFeePercent"], out var fee) ? fee : 0.1m,
    DefaultSwapFee = 0.003m,
    LPFeePercent = 0.2m
};

builder.Services.AddSingleton(net10Config);

// Register Services (order matters for dependencies)
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPoolService, PoolService>();
builder.Services.AddSingleton<ISwapService, SwapService>();
builder.Services.AddSingleton<IFarmService, FarmService>();
builder.Services.AddSingleton<IConfigService, ConfigService>();
builder.Services.AddSingleton<IContributionService, ContributionService>();

// ERP Services
builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IAccountingService, AccountingService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<ISupplierService, SupplierService>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<IPurchaseOrderService, PurchaseOrderService>();

// Add SignalR for real-time updates
// Geocoding Services
builder.Services.AddSingleton<IGeocodingService, GeocodingService>();

// Hotel & Real Estate Services
builder.Services.AddSingleton<IPropertyService, PropertyService>();
builder.Services.AddSingleton<IRoomService, RoomService>();
builder.Services.AddSingleton<IGuestService, GuestService>();
builder.Services.AddSingleton<IReservationService, ReservationService>();
builder.Services.AddSingleton<IRealEstateService, RealEstateService>();
builder.Services.AddSingleton<IFutureheadTrustService, FutureheadTrustService>();
builder.Services.AddSingleton<IHotelReportsService, HotelReportsService>();

// Web ERP - 3-Tier Architecture Services
// Data Access Layer (DAL)
builder.Services.AddSingleton<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddSingleton<IERPUserRepository, ERPUserRepository>();
builder.Services.AddSingleton<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<ILeadRepository, LeadRepository>();

// Business Logic Layer (BLL)
builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
builder.Services.AddSingleton<IERPUserService, ERPUserService>();
builder.Services.AddSingleton<ISalesService, SalesService>();

// College Management System
builder.Services.AddSingleton<IStudentService, StudentService>();
builder.Services.AddSingleton<ITeacherService, TeacherService>();
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
builder.Services.AddSingleton<IFeeService, FeeService>();
builder.Services.AddSingleton<ICollegeReportService, CollegeReportService>();

// Ierahkwa Sovereign Bank - Central Payment System
builder.Services.AddSingleton<IBankAccountService, BankAccountService>();
builder.Services.AddSingleton<IPaymentProcessingService, PaymentProcessingService>();
builder.Services.AddSingleton<IDepartmentPaymentService, DepartmentPaymentService>();
builder.Services.AddSingleton<ITreasuryService, TreasuryService>();
builder.Services.AddSingleton<IMultiCurrencyService, MultiCurrencyService>();

// Ierahkwa Cyber Cafe Management System
builder.Services.AddSingleton<ICafeStationService, CafeStationService>();
builder.Services.AddSingleton<ICafeCustomerService, CafeCustomerService>();
builder.Services.AddSingleton<ICafePricingService, CafePricingService>();
builder.Services.AddSingleton<ICyberCafeSessionService, CyberCafeSessionService>();
builder.Services.AddSingleton<ICafeReportService, CafeReportService>();

// Ierahkwa Hospital Records Management System
builder.Services.AddSingleton<IHospitalRoomService, HospitalRoomService>();
builder.Services.AddSingleton<IPhysicianService, PhysicianService>();
builder.Services.AddSingleton<IDiagnosisService, DiagnosisService>();
builder.Services.AddSingleton<IPatientService, PatientService>();
builder.Services.AddSingleton<IAdmissionService, AdmissionService>();
builder.Services.AddSingleton<IDiagnosticTestService, DiagnosticTestService>();
builder.Services.AddSingleton<IHospitalReportService, HospitalReportService>();

// Ierahkwa Stock Management & Point of Sale System
builder.Services.AddSingleton<IInventoryProductService, InventoryProductService>();
builder.Services.AddSingleton<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IUnitOfMeasureService, UnitOfMeasureService>();
builder.Services.AddSingleton<IStockTransactionService, StockTransactionService>();
builder.Services.AddSingleton<IInventoryCustomerService, InventoryCustomerService>();
builder.Services.AddSingleton<IPointOfSaleService, PointOfSaleService>();
builder.Services.AddSingleton<IInventoryReportService, InventoryReportService>();
builder.Services.AddSingleton<IUserService, UserService>();

// Ierahkwa Sports Betting Platform
builder.Services.AddSingleton<ISportsBettingServices, NET10.Infrastructure.Services.SportsBetting.SportsBettingServices>();

// Ierahkwa Casino Platform
builder.Services.AddSingleton<ICasinoServices, CasinoServices>();

// Ierahkwa Lotto Platform
builder.Services.AddSingleton<ILottoServices, LottoServices>();

// Ierahkwa Raffle Platform
builder.Services.AddSingleton<IRaffleServices, RaffleServices>();

// Google Maps Data Scraper PRO
builder.Services.AddSingleton<IGoogleMapsScraperService, GoogleMapsScraperService>();

// Ierahkwa GoMoney Personal Finance Management System
builder.Services.AddSingleton<IFinancialAccountService, FinancialAccountService>();
builder.Services.AddSingleton<IIncomeCategoryService, IncomeCategoryService>();
builder.Services.AddSingleton<IExpenseCategoryService, ExpenseCategoryService>();
builder.Services.AddSingleton<IFinancialTransactionService, FinancialTransactionService>();
builder.Services.AddSingleton<IPayableReceivableService, PayableReceivableService>();
builder.Services.AddSingleton<IFinancialReportService, FinancialReportService>();
builder.Services.AddSingleton<IBudgetService, BudgetService>();

builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ierahkwa NET10 DeFi API v1");
    c.RoutePrefix = "swagger";
});

// HTTPS deshabilitado - usar solo HTTP
// if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Ierahkwa NET10 DeFi Platform",
    version = "1.0.0",
    node = "Ierahkwa Futurehead Mamey Node",
    chainId = 777777,
    timestamp = DateTime.UtcNow
});

// Root endpoint - serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   🌐 IERAHKWA NET10 DEFI PLATFORM                            ║
║   Sovereign Government DeFi Exchange                          ║
║                                                               ║
║   Powered by: Ierahkwa Futurehead Mamey Node                  ║
║   Network: ierahkwa-mainnet (Chain ID: 777777)               ║
║                                                               ║
║   Features:                                                   ║
║   ✅ Token Swap (AMM)                                        ║
║   ✅ Liquidity Pools                                         ║
║   ✅ Yield Farming                                           ║
║   ✅ Smart Contract Deployment                               ║
║   ✅ Admin Panel with Fee Management                         ║
║   ✅ Multi-Chain Support (ETH, BSC, Polygon, Avalanche)     ║
║   ✅ IGT Token Integration (101 Government Tokens)          ║
║   ✅ Contribution Graph (GitHub-Style Activity Tracker)     ║
║                                                               ║
║   API: /swagger                                               ║
║   NAGADAN ERP (.NET 10):                                      ║
║   ✅ Invoicing & Billing                                     ║
║   ✅ Customers & Suppliers                                   ║
║   ✅ Products & Inventory                                    ║
║   ✅ Accounting & Journal Entries                            ║
║   ✅ Financial Reports (P&L, Balance Sheet, Trial Balance)  ║
║   ✅ Payments & Receivables                                  ║
║   ✅ Purchase Orders                                         ║
║   ✅ Multi-Company Support                                   ║
║   ✅ Tax Management (GST/VAT/IVA)                           ║
║                                                               ║
║   GEOCODER PRO:                                               ║
║   ✅ Geocoding (Address → Lat/Lng)                          ║
║   ✅ Reverse Geocoding (Lat/Lng → Address)                  ║
║   ✅ CSV Import/Export                                       ║
║   ✅ Batch Processing                                        ║
║   ✅ Google Geocoding API                                    ║
║   ✅ Multi-language Support                                  ║
║                                                               ║
║   SMART HOTEL MANAGEMENT + AIRBNB + REAL ESTATE:             ║
║   ✅ Property Management (Hotels, Villas, Apartments)        ║
║   ✅ Room Management & Room Types                            ║
║   ✅ Booking & Reservations                                  ║
║   ✅ Guest Management & Loyalty                              ║
║   ✅ Check-In / Check-Out                                    ║
║   ✅ Real Estate Listings (Buy/Rent)                         ║
║   ✅ Futurehead Trust Coin (FHT) Payments                    ║
║   ✅ Daily/Weekly/Monthly Reports                            ║
║                                                               ║
║   WEB ERP - 3-TIER ARCHITECTURE (.NET 10):                   ║
║   ✅ Layer 1: Presentation (API Controllers)                 ║
║   ✅ Layer 2: Business Logic (Services)                      ║
║   ✅ Layer 3: Data Access (Repositories)                     ║
║   ✅ ASP.NET Identity Integration                            ║
║   ✅ HR, Sales, Purchasing, Manufacturing                    ║
║   ✅ Quality Control, Projects, CRM                          ║
║   ✅ Unit of Work + Repository Pattern                       ║
║                                                               ║
║   Frontend: /index.html                                       ║
║   Dashboard: /dashboard.html                                  ║
║   ERP System: /erp.html                                       ║
║   Geocoder: /geocoder.html                                    ║
║   GoMoney: /gomoney.html                                      ║
║   Contributions: /contributions.html                          ║
║   Maps Scraper: /google-maps-scraper.html                    ║
║                                                               ║
║   API Endpoints:                                              ║
║   • /api/health - System health & metrics                    ║
║   • /api/dashboard - Real-time analytics                     ║
║   • /api/notification - Alerts & notifications               ║
║   • /api/audit - Activity logging                            ║
║   • /api/report - Financial reports                          ║
║   • /api/wallet - Wallet management                          ║
║   • /api/blockchain - Block explorer                         ║
║   • /api/governance - DAO & voting                           ║
║   • /api/identity - KYC & credentials                        ║
║   • /api/bridge - Cross-chain transfers                      ║
║                                                               ║
║   FINANCIAL SERVICES:                                          ║
║   • /api/bank - Sovereign Bank (Central Payments)            ║
║   • /api/finance - GoMoney Personal Finance                  ║
║   • /api/erp/accounting - ERP Accounting                      ║
║                                                               ║
║   HEALTH & EDUCATION:                                         ║
║   • /api/hospital - Hospital Records Management              ║
║   • /api/college - Education System                          ║
║                                                               ║
║   BUSINESS SERVICES:                                          ║
║   • /api/cybercafe - Cyber Cafe Time Management              ║
║   • /api/inventory - Stock Management & Point of Sale        ║
║   • /api/hotel - Hotel & Real Estate Management              ║
║   • /api/sportsbetting - Sports Betting Platform             ║
║   • /api/casino - Casino Games & Live Dealer                 ║
║   • /api/lotto - Lottery System                              ║
║   • /api/raffle - Raffle & Prize Draws                       ║
║   • /api/geocoding - Geocoding Services                      ║
║   • /api/googlemapsscraper - Google Maps Scraper            ║
║   Sovereign Government of Ierahkwa Ne Kanienke               ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
