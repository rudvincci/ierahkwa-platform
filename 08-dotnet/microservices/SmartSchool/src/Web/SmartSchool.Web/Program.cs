using System.Text;
using Common.Application.Interfaces;
using Common.Domain.Interfaces;
using Common.Persistence;
using Common.Persistence.Services;
using Librarian.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using OnlineSchool.Persistence;
using Receptionist.Persistence;
using SmartAccounting.Persistence;
using SmartSchool.Web.Services;
using UserManagement.Application.Interfaces;
using UserManagement.Persistence;
using Zoom.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add NLog
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContexts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<CommonDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<UserManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<OnlineSchoolDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<SmartAccountingDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ReceptionistDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<LibrarianDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<ZoomDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Allow token from cookie for MVC views
            context.Token = context.Request.Cookies["AuthToken"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Smart School & Accounting API", 
        Version = "v1",
        Description = "API for Smart School & Accounting System"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

// Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var commonContext = services.GetRequiredService<CommonDbContext>();
        var userContext = services.GetRequiredService<UserManagementDbContext>();
        var schoolContext = services.GetRequiredService<OnlineSchoolDbContext>();
        var accountingContext = services.GetRequiredService<SmartAccountingDbContext>();
        var receptionContext = services.GetRequiredService<ReceptionistDbContext>();
        var librarianContext = services.GetRequiredService<LibrarianDbContext>();
        var zoomContext = services.GetRequiredService<ZoomDbContext>();
        
        // Create databases if they don't exist
        commonContext.Database.EnsureCreated();
        userContext.Database.EnsureCreated();
        schoolContext.Database.EnsureCreated();
        accountingContext.Database.EnsureCreated();
        receptionContext.Database.EnsureCreated();
        librarianContext.Database.EnsureCreated();
        zoomContext.Database.EnsureCreated();
        
        // Seed default data
        await SmartSchool.Web.Data.DbSeeder.SeedAsync(app.Services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();
