using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Mamey.Portal.Cms.Infrastructure.Persistence;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Library.Application.Models;
using Mamey.Portal.Library.Infrastructure.Persistence;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Storage;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Infrastructure.Persistence;
using Mamey.Portal.Web.Auth;
using Mamey.Portal.Web.Middleware;
using Mamey.Portal.Web.Realtime;
using Mamey.Portal.Web.Tenancy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Mamey.Portal.Web;

public static class PortalApplicationConfiguration
{
    public static void ConfigurePortalApp(this WebApplication app, bool useOidc)
    {
        if (app.Environment.IsDevelopment())
        {
            ApplyDevBootstrap(app);
        }
        else
        {
            ApplyMigrations(app);
        }
        ConfigureRequestPipeline(app, useOidc);
        MapPortalEndpoints(app, useOidc);

        app.UseMiddleware<CitizenApprovalMiddleware>();

        app.MapBlazorHub();
        app.MapHub<CitizenshipHub>("/hubs/citizenship");
        app.MapFallbackToPage("/_Host");
    }

    private static void ConfigureRequestPipeline(WebApplication app, bool useOidc)
    {
        if (!useOidc)
        {
            app.Use((ctx, next) =>
            {
                if (!ctx.Request.Cookies.ContainsKey(MockAuthSession.CookieName))
                {
                    ctx.Response.Cookies.Append(
                        MockAuthSession.CookieName,
                        Guid.NewGuid().ToString("N"),
                        new CookieOptions
                        {
                            HttpOnly = true,
                            IsEssential = true,
                            SameSite = SameSiteMode.Strict,
                            Secure = !ctx.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
                        });
                }

                return next();
            });
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        if (useOidc)
        {
            app.Use(async (http, next) =>
            {
                if (http.User?.Identity?.IsAuthenticated != true)
                {
                    await next();
                    return;
                }

                var path = http.Request.Path.Value ?? string.Empty;
                if (path.StartsWith("/auth/", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/_content", StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase))
                {
                    await next();
                    return;
                }

                var options = http.RequestServices.GetRequiredService<IOptions<PortalAuthOptions>>().Value;
                var issuer = OidcIssuerNormalizer.Normalize(http.User.FindFirst("iss")?.Value ?? options.Oidc.Authority);
                var subject = http.User.FindFirst("sub")?.Value ?? string.Empty;
                var email = (http.User.FindFirst("email")?.Value ?? string.Empty).Trim().ToLowerInvariant();

                var tenant = http.RequestServices.GetRequiredService<ITenantContext>().TenantId;

                if (!string.IsNullOrWhiteSpace(issuer) && !string.IsNullOrWhiteSpace(subject))
                {
                    var db = http.RequestServices.GetRequiredService<TenantDbContext>();
                    var mappedTenant = await db.UserMappings.AsNoTracking()
                        .Where(x => x.Issuer == issuer && x.Subject == subject)
                        .Select(x => x.TenantId)
                        .SingleOrDefaultAsync();

                    if (!string.IsNullOrWhiteSpace(mappedTenant))
                    {
                        tenant = ClaimsTenantContext.NormalizeTenantId(mappedTenant);
                        if (!string.IsNullOrWhiteSpace(tenant))
                        {
                            http.Items["ResolvedTenantId"] = tenant;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(email))
                    {
                        var invite = await db.UserInvites
                            .Where(x => x.Issuer == issuer && x.Email == email && x.UsedAt == null)
                            .OrderBy(x => x.CreatedAt)
                            .FirstOrDefaultAsync();

                        if (invite is not null)
                        {
                            var invitedTenant = ClaimsTenantContext.NormalizeTenantId(invite.TenantId);
                            if (!string.IsNullOrWhiteSpace(invitedTenant))
                            {
                                db.UserMappings.Add(new TenantUserMappingRow
                                {
                                    Issuer = issuer,
                                    Subject = subject,
                                    TenantId = invitedTenant,
                                    Email = email,
                                    CreatedAt = DateTimeOffset.UtcNow,
                                    UpdatedAt = DateTimeOffset.UtcNow
                                });

                                invite.UsedAt = DateTimeOffset.UtcNow;
                                invite.UpdatedAt = invite.UsedAt.Value;

                                try { await db.SaveChangesAsync(); } catch { }

                                tenant = invitedTenant;
                                http.Items["ResolvedTenantId"] = tenant;
                            }
                        }
                    }
                }

                var gateDb = http.RequestServices.GetRequiredService<TenantDbContext>();
                var exists = await gateDb.Tenants.AsNoTracking().AnyAsync(x => x.TenantId == tenant);
                if (exists)
                {
                    await next();
                    return;
                }

                var isAdmin = http.User.IsInRole("Admin");

                var accept = http.Request.Headers.Accept.ToString();
                var wantsHtml = accept.Contains("text/html", StringComparison.OrdinalIgnoreCase)
                                || string.IsNullOrWhiteSpace(accept);

                if (wantsHtml)
                {
                    if (isAdmin)
                    {
                        var prefill = Uri.EscapeDataString(tenant);
                        http.Response.Redirect($"/gov/tenants?prefillTenantId={prefill}");
                        return;
                    }

                    http.Response.Redirect("/auth/login?error=tenant_unknown");
                    return;
                }

                http.Response.StatusCode = StatusCodes.Status403Forbidden;
            });
        }

        app.UseRateLimiter();
    }

    private static void MapPortalEndpoints(WebApplication app, bool useOidc)
    {
        app.MapGet("/healthz", () => Results.Ok("ok")).AllowAnonymous();

        if (useOidc)
        {
            app.MapGet("/auth/oidc/login", (string? returnUrl, HttpContext http) =>
            {
                returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return Results.Challenge(
                    new AuthenticationProperties { RedirectUri = returnUrl },
                    new[] { OpenIdConnectDefaults.AuthenticationScheme });
            }).AllowAnonymous();

            app.MapGet("/auth/oidc/logout", async (HttpContext http) =>
            {
                await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/");
            }).AllowAnonymous();
        }

        if (app.Environment.IsDevelopment())
        {
            app.MapGet("/dev/whoami", (HttpContext http, ITenantContext tenant) =>
            {
                var user = http.User;
                return Results.Json(new
                {
                    isAuthenticated = user.Identity?.IsAuthenticated == true,
                    name = user.Identity?.Name,
                    tenant = tenant.TenantId,
                    roles = user.Claims.Where(c => c.Type is "roles" or "role" or System.Security.Claims.ClaimTypes.Role)
                        .Select(c => c.Value)
                        .Distinct()
                        .ToArray(),
                    claims = user.Claims.Select(c => new { c.Type, c.Value }).ToArray()
                });
            }).RequireAuthorization();

            var bootstrap = app.MapPost("/dev/seed/bootstrap", async (
                string? tenants,
                ITenantOnboardingService onboarding,
                CitizenshipDbContext citizenshipDb,
                CancellationToken ct) =>
            {
                static string NormalizeTenantId(string? id)
                {
                    id = (id ?? string.Empty).Trim().ToLowerInvariant();
                    if (string.IsNullOrWhiteSpace(id)) return string.Empty;

                    var cleaned = new string(id.Where(c => (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-').ToArray());
                    return cleaned.Length > 128 ? cleaned[..128] : cleaned;
                }

                static string NewApplicationNumber(string tenantId, DateTimeOffset now)
                {
                    var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
                    return $"APP-{tenantId.ToUpperInvariant()}-{now:yyyyMMdd}-{suffix}";
                }

                var now = DateTimeOffset.UtcNow;
                var tenantIds = (tenants ?? "ink,bor")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(NormalizeTenantId)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(25)
                    .ToList();

                if (tenantIds.Count == 0)
                {
                    return Results.BadRequest(new { error = "No tenant ids provided." });
                }

                var createdTenants = new List<string>();
                var createdApplications = new List<object>();

                foreach (var tid in tenantIds)
                {
                    var settings = await onboarding.GetSettingsAsync(tid, ct);
                    if (settings is null)
                    {
                        var displayName = tid switch
                        {
                            "ink" => "Ierahkwa ne Kanienke",
                            "bor" => "Borinquen",
                            _ => tid.ToUpperInvariant()
                        };

                        await onboarding.CreateTenantAsync(
                            tid,
                            displayName,
                            branding: new TenantBranding(displayName, SealLine1: null, SealLine2: null, ContactEmail: null),
                            namingPattern: DocumentNamingPattern.Default,
                            activationChecklist: null,
                            ct: ct);

                        createdTenants.Add(tid);
                    }

                    var appNo = NewApplicationNumber(tid, now);
                    var appRow = new CitizenshipApplicationRow
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tid,
                        ApplicationNumber = appNo,
                        Status = "Submitted",
                        FirstName = tid == "ink" ? "Test" : "Sample",
                        LastName = tid == "ink" ? "Kanienke" : "Applicant",
                        DateOfBirth = new DateOnly(1990, 1, 1),
                        Email = $"{tid}.applicant@example.com",
                        AddressLine1 = "123 Main St",
                        City = "Testville",
                        Region = "TS",
                        PostalCode = "00000",
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    citizenshipDb.Applications.Add(appRow);
                    await citizenshipDb.SaveChangesAsync(ct);

                    createdApplications.Add(new { tenantId = tid, applicationNumber = appNo, applicationId = appRow.Id });
                }

                return Results.Ok(new
                {
                    createdTenants,
                    createdApplications
                });
            });

            if (useOidc)
            {
                bootstrap.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
            }
            else
            {
                bootstrap.AllowAnonymous();
            }
        }

        app.MapGet("/library/files/{libraryItemId:guid}", async (
            Guid libraryItemId,
            ITenantContext tenant,
            ICurrentUserContext user,
            LibraryDbContext db,
            IObjectStorage storage,
            CancellationToken ct) =>
        {
            var tenantId = tenant.TenantId;

            var item = await db.Items.AsNoTracking()
                .Where(x => x.TenantId == tenantId && x.Id == libraryItemId)
                .Select(x => new
                {
                    x.Visibility,
                    x.Status,
                    x.FileName,
                    x.ContentType,
                    x.StorageBucket,
                    x.StorageKey
                })
                .SingleOrDefaultAsync(ct);

            if (item is null)
            {
                return Results.NotFound();
            }

            if (item.Status != LibraryContentStatus.Published)
            {
                return Results.NotFound();
            }

            var canDownload = item.Visibility switch
            {
                LibraryVisibility.Public => true,
                LibraryVisibility.Citizen => user.IsAuthenticated && user.IsInRole("Citizen"),
                LibraryVisibility.Government => user.IsAuthenticated && (
                    user.IsInRole("GovernmentAgent")
                    || user.IsInRole("Admin")),
                _ => false
            };

            if (!canDownload)
            {
                return Results.NotFound();
            }

            var obj = await storage.GetAsync(item.StorageBucket, item.StorageKey, ct);
            return Results.File(obj.Content, item.ContentType, item.FileName);
        })
        .AllowAnonymous();

        app.MapGet("/api/public/validate", async (
            string? documentNumber,
            ITenantContext tenant,
            IPublicDocumentValidationService validator,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                return Results.BadRequest(new { error = "documentNumber is required." });
            }

            var result = await validator.ValidateAsync(tenant.TenantId, documentNumber, ct);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .RequireRateLimiting("public-validate");

        app.MapPost("/api/public/scan-barcode", async (
            HttpRequest request,
            ITenantContext tenant,
            IBarcodeScanningService scanner,
            IPublicDocumentValidationService validator,
            CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest(new { error = "Request must be multipart/form-data." });
            }

            var form = await request.ReadFormAsync(ct);
            var file = form.Files["image"];

            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new { error = "Image file is required." });
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return Results.BadRequest(new { error = "Image file size must be less than 10MB." });
            }

            using var stream = file.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, ct);
            var imageBytes = memoryStream.ToArray();

            var barcodeData = await scanner.ScanBarcodeFromImageAsync(imageBytes, ct);
            if (string.IsNullOrWhiteSpace(barcodeData))
            {
                return Results.Ok(new
                {
                    success = false,
                    message = "No PDF417 barcode found in the image."
                });
            }

            var documentNumber = await scanner.ParseDocumentNumberFromAamvaDataAsync(barcodeData, ct);
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                return Results.Ok(new
                {
                    success = false,
                    message = "Barcode scanned but could not extract document number.",
                    barcodeData = barcodeData.Substring(0, Math.Min(100, barcodeData.Length))
                });
            }

            var validationResult = await validator.ValidateAsync(tenant.TenantId, documentNumber, ct);

            return Results.Ok(new
            {
                success = true,
                documentNumber,
                validation = validationResult
            });
        })
        .AllowAnonymous()
        .RequireRateLimiting("public-validate");

        app.MapGet("/citizen/files/{issuedDocumentId:guid}", async (
            Guid issuedDocumentId,
            ITenantContext tenant,
            ICurrentUserContext user,
            CitizenshipDbContext db,
            IObjectStorage storage,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || !user.IsInRole("Citizen"))
            {
                return Results.Unauthorized();
            }

            var email = user.UserName?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                return Results.Unauthorized();
            }

            var tenantId = tenant.TenantId;

            var doc = await (
                    from d in db.IssuedDocuments.AsNoTracking()
                    join a in db.Applications.AsNoTracking() on d.ApplicationId equals a.Id
                    where a.TenantId == tenantId && a.Email == email && d.Id == issuedDocumentId
                    select new { d.StorageBucket, d.StorageKey })
                .FirstOrDefaultAsync(ct);

            if (doc is null)
            {
                return Results.NotFound();
            }

            var obj = await storage.GetAsync(doc.StorageBucket, doc.StorageKey, ct);
            return Results.File(obj.Content, obj.ContentType, obj.FileName);
        })
        .AllowAnonymous();

        app.MapPost("/citizen/documents/{documentId:guid}/renew", async (
            Guid documentId,
            ICitizenshipBackofficeService backoffice,
            ICurrentUserContext user,
            ITenantContext tenant,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || !user.IsInRole("Citizen"))
            {
                return Results.Unauthorized();
            }

            try
            {
                var renewed = await backoffice.RenewDocumentAsync(documentId, ct);
                return Results.Ok(new { success = true, documentId = renewed.Id, message = "Document renewed successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { success = false, error = ex.Message });
            }
        })
        .RequireAuthorization();

        app.MapGet("/citizen/uploads/{uploadId:guid}", async (
            Guid uploadId,
            ITenantContext tenant,
            ICurrentUserContext user,
            CitizenshipDbContext db,
            IObjectStorage storage,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || !user.IsInRole("Citizen"))
            {
                return Results.Unauthorized();
            }

            var email = user.UserName?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                return Results.Unauthorized();
            }

            var tenantId = tenant.TenantId;

            var upload = await (
                    from u in db.Uploads.AsNoTracking()
                    join a in db.Applications.AsNoTracking() on u.ApplicationId equals a.Id
                    where a.TenantId == tenantId && a.Email == email && u.Id == uploadId
                    select new { u.StorageBucket, u.StorageKey })
                .FirstOrDefaultAsync(ct);

            if (upload is null)
            {
                return Results.NotFound();
            }

            var obj = await storage.GetAsync(upload.StorageBucket, upload.StorageKey, ct);
            return Results.File(obj.Content, obj.ContentType, obj.FileName);
        })
        .AllowAnonymous();

        app.MapGet("/gov/progression-applications", async (
            CitizenshipDbContext db,
            ITenantContext tenant,
            ICurrentUserContext user,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || (!user.IsInRole("Admin") && !user.IsInRole("Agent")))
            {
                return Results.Unauthorized();
            }

            var tenantId = tenant.TenantId;
            var applications = await (
                from pa in db.StatusProgressionApplications.AsNoTracking()
                join cs in db.CitizenshipStatuses.AsNoTracking() on pa.CitizenshipStatusId equals cs.Id
                where pa.TenantId == tenantId
                orderby pa.CreatedAt descending
                select new
                {
                    pa.Id,
                    pa.ApplicationNumber,
                    pa.TargetStatus,
                    pa.Status,
                    pa.YearsCompletedAtApplication,
                    pa.CreatedAt,
                    pa.UpdatedAt,
                    cs.Email,
                    CurrentStatus = cs.Status
                })
                .Take(100)
                .ToListAsync(ct);

            return Results.Ok(applications);
        })
        .RequireAuthorization();

        app.MapPost("/gov/applications/{id:guid}/process-workflow", async (
            Guid id,
            IApplicationWorkflowService workflowService,
            ITenantContext tenant,
            ICurrentUserContext user,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || (!user.IsInRole("Admin") && !user.IsInRole("Agent")))
            {
                return Results.Unauthorized();
            }

            var processed = await workflowService.ProcessNextWorkflowStepAsync(id, ct);

            if (processed)
            {
                return Results.Ok(new { success = true, message = "Workflow step processed successfully" });
            }

            return Results.BadRequest(new { success = false, message = "No workflow step to process or processing failed" });
        })
        .RequireAuthorization();

        app.MapGet("/gov/applications/{id:guid}/payment-plan", async (
            Guid id,
            IPaymentPlanService paymentService,
            ITenantContext tenant,
            ICurrentUserContext user,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || (!user.IsInRole("Admin") && !user.IsInRole("Agent")))
            {
                return Results.Unauthorized();
            }

            var plan = await paymentService.GetPaymentPlanAsync(id, ct);
            if (plan == null)
            {
                return Results.NotFound(new { error = "Payment plan not found" });
            }

            return Results.Ok(plan);
        })
        .RequireAuthorization();

        app.MapPost("/gov/applications/{id:guid}/payment-plan/confirm", async (
            Guid id,
            string paymentReference,
            IPaymentPlanService paymentService,
            ITenantContext tenant,
            ICurrentUserContext user,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || (!user.IsInRole("Admin") && !user.IsInRole("Agent")))
            {
                return Results.Unauthorized();
            }

            var confirmed = await paymentService.ConfirmPaymentAsync(id, paymentReference, ct);
            if (confirmed)
            {
                return Results.Ok(new { success = true, message = "Payment confirmed successfully" });
            }

            return Results.BadRequest(new { success = false, message = "Payment confirmation failed" });
        })
        .RequireAuthorization();

        app.MapPost("/gov/progression-applications/{id:guid}/approve", async (
            Guid id,
            IStatusProgressionService progressionService,
            ICurrentUserContext user,
            CancellationToken ct) =>
        {
            if (!user.IsAuthenticated || (!user.IsInRole("Admin") && !user.IsInRole("Agent")))
            {
                return Results.Unauthorized();
            }

            try
            {
                await progressionService.ApproveProgressionApplicationAsync(id, ct);
                return Results.Ok(new { message = "Progression application approved successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .RequireAuthorization();

        if (app.Environment.IsDevelopment() && !useOidc)
        {
            app.MapGet("/dev/mock-login", (string? tenant, string? user, string? role, MockAuthSession session) =>
            {
                session.SignIn(tenant ?? "default", user ?? "manual@example.com", role ?? "Citizen");
                return Results.Ok(new { tenant = session.Tenant, user = session.UserName, role = session.Role });
            }).AllowAnonymous();

            app.MapPost("/dev/issue/idcard", async (
                string applicationNumber,
                string? variant,
                CitizenshipDbContext db,
                ICitizenshipBackofficeService backoffice,
                ITenantContext tenant,
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(applicationNumber))
                {
                    return Results.BadRequest(new { error = "applicationNumber is required." });
                }

                var tenantId = tenant.TenantId;
                var appId = await db.Applications.AsNoTracking()
                    .Where(a => a.TenantId == tenantId && a.ApplicationNumber == applicationNumber)
                    .Select(a => a.Id)
                    .SingleOrDefaultAsync(ct);

                if (appId == Guid.Empty)
                {
                    return Results.NotFound(new { error = "Application not found." });
                }

                var doc = await backoffice.IssueIdCardAsync(appId, variant ?? "IdentificationCard", ct);
                return Results.Ok(doc);
            }).AllowAnonymous();

            app.MapPost("/dev/issue/vehicletag", async (
                string applicationNumber,
                string? variant,
                CitizenshipDbContext db,
                ICitizenshipBackofficeService backoffice,
                ITenantContext tenant,
                CancellationToken ct) =>
            {
                if (string.IsNullOrWhiteSpace(applicationNumber))
                {
                    return Results.BadRequest(new { error = "applicationNumber is required." });
                }

                var tenantId = tenant.TenantId;
                var appId = await db.Applications.AsNoTracking()
                    .Where(a => a.TenantId == tenantId && a.ApplicationNumber == applicationNumber)
                    .Select(a => a.Id)
                    .SingleOrDefaultAsync(ct);

                if (appId == Guid.Empty)
                {
                    return Results.NotFound(new { error = "Application not found." });
                }

                var doc = await backoffice.IssueVehicleTagAsync(appId, variant ?? "Standard", ct);
                return Results.Ok(doc);
            }).AllowAnonymous();

            app.MapPost("/dev/backfill/issued-documents", async (CitizenshipDbContext db, ITenantContext tenant, CancellationToken ct) =>
            {
                var tenantId = tenant.TenantId;
                var now = DateTimeOffset.UtcNow;

                var candidates = await (
                        from d in db.IssuedDocuments
                        join a in db.Applications on d.ApplicationId equals a.Id
                        where a.TenantId == tenantId
                              && (d.DocumentNumber == null || d.DocumentNumber == "")
                              && (d.Kind == "Passport" || d.Kind.StartsWith("IdCard:") || d.Kind.StartsWith("VehicleTag:"))
                        select new { Doc = d, App = a })
                    .ToListAsync(ct);

                static string SuffixFromApplicationNumber(string appNo)
                {
                    if (string.IsNullOrWhiteSpace(appNo))
                    {
                        return "UNKNOWN";
                    }

                    var parts = appNo.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    return parts.Length > 0 ? parts[^1].ToUpperInvariant() : "UNKNOWN";
                }

                static string VariantFromKind(string kind, string prefix)
                    => kind.Length > prefix.Length ? kind[prefix.Length..] : "Standard";

                var updated = 0;

                foreach (var x in candidates)
                {
                    var suffix = SuffixFromApplicationNumber(x.App.ApplicationNumber);
                    var issuedAt = x.Doc.CreatedAt == default ? now : x.Doc.CreatedAt;
                    var date = issuedAt.ToString("yyMMdd");

                    if (string.Equals(x.Doc.Kind, "Passport", StringComparison.OrdinalIgnoreCase))
                    {
                        x.Doc.DocumentNumber = $"P-{tenantId.ToUpperInvariant()}-{date}-{suffix}";
                        x.Doc.ExpiresAt ??= issuedAt.AddYears(5);
                        updated++;
                        continue;
                    }

                    if (x.Doc.Kind.StartsWith("IdCard:", StringComparison.OrdinalIgnoreCase))
                    {
                        var v = VariantFromKind(x.Doc.Kind, "IdCard:");
                        x.Doc.DocumentNumber = $"ID-{tenantId.ToUpperInvariant()}-{date}-{suffix}-{v.ToUpperInvariant()}";
                        x.Doc.ExpiresAt ??= issuedAt.AddYears(5);
                        updated++;
                        continue;
                    }

                    if (x.Doc.Kind.StartsWith("VehicleTag:", StringComparison.OrdinalIgnoreCase))
                    {
                        var v = VariantFromKind(x.Doc.Kind, "VehicleTag:");
                        x.Doc.DocumentNumber = $"TAG-{tenantId.ToUpperInvariant()}-{date}-{suffix}-{v.ToUpperInvariant()}";
                        updated++;
                    }
                }

                await db.SaveChangesAsync(ct);
                return Results.Ok(new { tenantId, updated });
            }).AllowAnonymous();

            app.MapPost("/dev/seed/citizenship", async (ICitizenshipApplicationService svc, CancellationToken ct) =>
            {
                var req = new Mamey.Portal.Citizenship.Application.Requests.SubmitCitizenshipApplicationRequest(
                    FirstName: "Test",
                    LastName: "Applicant",
                    DateOfBirth: new DateOnly(1990, 1, 1),
                    Email: "test@example.com",
                    AddressLine1: "123 Main St",
                    City: "Testville",
                    Region: "TS",
                    PostalCode: "00000");

                var pdfBytes = System.Text.Encoding.UTF8.GetBytes("%PDF-1.4\n%Fake\n%%EOF\n");
                var personalDocs = new List<Mamey.Portal.Citizenship.Application.Requests.UploadFile>
                {
                    new("identity.pdf", "application/pdf", pdfBytes.Length, new MemoryStream(pdfBytes)),
                };

                var passportMs = new MemoryStream();
                using (var img = new Image<Rgba32>(600, 600))
                {
                    await img.SaveAsync(passportMs, new PngEncoder(), ct);
                }
                passportMs.Position = 0;

                var sigMs = new MemoryStream();
                using (var img = new Image<Rgba32>(400, 120))
                {
                    await img.SaveAsync(sigMs, new PngEncoder(), ct);
                }
                sigMs.Position = 0;

                var appNo = await svc.SubmitAsync(
                    req,
                    personalDocs,
                    new("passport.png", "image/png", passportMs.Length, passportMs),
                    new("signature.png", "image/png", sigMs.Length, sigMs),
                    ct);

                return Results.Ok(new { applicationNumber = appNo });
            }).AllowAnonymous();

            app.MapPost("/dev/seed/citizenship-manual", async (ICitizenshipBackofficeService backoffice, CancellationToken ct) =>
            {
                var req = new Mamey.Portal.Citizenship.Application.Requests.SubmitCitizenshipApplicationRequest(
                    FirstName: "Manual",
                    LastName: "Entry",
                    DateOfBirth: new DateOnly(1985, 5, 5),
                    Email: "manual@example.com",
                    AddressLine1: "456 Agent St",
                    City: "Backoffice",
                    Region: "BO",
                    PostalCode: "11111");

                var pdfBytes = System.Text.Encoding.UTF8.GetBytes("%PDF-1.4\n%Manual\n%%EOF\n");
                var personalDocs = new List<Mamey.Portal.Citizenship.Application.Requests.UploadFile>
                {
                    new("manual-identity.pdf", "application/pdf", pdfBytes.Length, new MemoryStream(pdfBytes)),
                };

                var passportMs = new MemoryStream();
                using (var img = new Image<Rgba32>(600, 600))
                {
                    await img.SaveAsync(passportMs, new PngEncoder(), ct);
                }
                passportMs.Position = 0;

                var sigMs = new MemoryStream();
                using (var img = new Image<Rgba32>(400, 120))
                {
                    await img.SaveAsync(sigMs, new PngEncoder(), ct);
                }
                sigMs.Position = 0;

                var appNo = await backoffice.CreateManualApplicationAsync(
                    req,
                    personalDocs,
                    new("passport.png", "image/png", passportMs.Length, passportMs),
                    new("signature.png", "image/png", sigMs.Length, sigMs),
                    ct);

                return Results.Ok(new { applicationNumber = appNo });
            }).AllowAnonymous();

            app.MapGet("/dev/files/{bucket}/{**key}", async (string bucket, string key, IObjectStorage storage, CancellationToken ct) =>
            {
                var obj = await storage.GetAsync(bucket, key, ct);
                return Results.File(obj.Content, obj.ContentType, obj.FileName);
            }).AllowAnonymous();
        }
    }

    private static void ApplyDevBootstrap(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("PortalBootstrap");

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<CitizenshipDbContext>(),
            "20260109221309_InitialCreate",
            "citizenship_applications",
            logger,
            resetDatabase: true);

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<CmsDbContext>(),
            "20260109221340_InitialCreate",
            "cms_news",
            logger);

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<TenantDbContext>(),
            "20260109221411_InitialCreate",
            "tenants",
            logger);

        var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var now = DateTimeOffset.UtcNow;
        var exists = tenantDb.Tenants.Any(x => x.TenantId == "default");
        if (!exists)
        {
            tenantDb.Tenants.Add(new TenantRow
            {
                TenantId = "default",
                DisplayName = "Default Nation",
                CreatedAt = now,
                UpdatedAt = now
            });

            tenantDb.TenantSettings.Add(new TenantSettingsRow
            {
                TenantId = "default",
                BrandingJson = """{"displayName":"Default Nation","sealLine1":null,"sealLine2":null,"contactEmail":null}""",
                CreatedAt = now,
                UpdatedAt = now
            });

            tenantDb.DocumentNaming.Add(new TenantDocumentNamingRow
            {
                TenantId = "default",
                PatternJson = """{"personalDocumentPattern":"citizenship/{TenantId}/{ApplicationNumber}/uploads/personal/{Date:yyyyMMdd}-{Unique}-{OriginalFileName}","passportPhotoPattern":"citizenship/{TenantId}/{ApplicationNumber}/uploads/passport-photo/{Date:yyyyMMdd}-{Unique}-{OriginalFileName}","signatureImagePattern":"citizenship/{TenantId}/{ApplicationNumber}/uploads/signature/{Date:yyyyMMdd}-{Unique}-{OriginalFileName}","passportDocumentPattern":"citizenship/{TenantId}/{ApplicationNumber}/issued/passport/{Date:yyyyMMdd}-{Unique}.html","idCardDocumentPattern":"citizenship/{TenantId}/{ApplicationNumber}/issued/idcard/{Date:yyyyMMdd}-{Unique}.html","vehicleTagDocumentPattern":"citizenship/{TenantId}/{ApplicationNumber}/issued/vehicletag/{Date:yyyyMMdd}-{Unique}.html","citizenshipCertificatePattern":"citizenship/{TenantId}/{ApplicationNumber}/issued/certificate/{Date:yyyyMMdd}-{Unique}.html"}""",
                UpdatedAt = now
            });

            tenantDb.SaveChanges();
        }

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<LibraryDbContext>(),
            "20260109221440_InitialCreate",
            "library_items",
            logger);
    }

    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("PortalBootstrap");

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<CitizenshipDbContext>(),
            "20260109221309_InitialCreate",
            "citizenship_applications",
            logger);

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<CmsDbContext>(),
            "20260109221340_InitialCreate",
            "cms_news",
            logger);

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<TenantDbContext>(),
            "20260109221411_InitialCreate",
            "tenants",
            logger);

        ApplyMigrations(
            scope.ServiceProvider.GetRequiredService<LibraryDbContext>(),
            "20260109221440_InitialCreate",
            "library_items",
            logger);
    }

    private static void ApplyMigrations(
        DbContext db,
        string initialMigrationId,
        string sentinelTable,
        ILogger logger,
        bool resetDatabase = false)
    {
        try
        {
            if (resetDatabase)
            {
                db.Database.EnsureDeleted();
            }
            else
            {
                EnsureBaselineMigration(db, initialMigrationId, sentinelTable, logger);
            }

            db.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed for {ContextType}.", db.GetType().Name);
            throw;
        }
    }

    private static void EnsureBaselineMigration(
        DbContext db,
        string migrationId,
        string sentinelTable,
        ILogger logger)
    {
        if (!db.Database.CanConnect())
        {
            return;
        }

        var connection = db.Database.GetDbConnection();
        var shouldClose = connection.State == ConnectionState.Closed;
        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            if (!TableExists(connection, "__EFMigrationsHistory"))
            {
                if (!TableExists(connection, sentinelTable))
                {
                    return;
                }

                CreateHistoryTable(connection);
            }

            if (!TableExists(connection, sentinelTable))
            {
                return;
            }

            if (HistoryContainsMigration(connection, migrationId))
            {
                return;
            }

            InsertMigrationHistoryRow(connection, migrationId);
            logger.LogWarning(
                "Baselined migration history with {MigrationId} for {ContextType}.",
                migrationId,
                db.GetType().Name);
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }

    private static bool TableExists(IDbConnection connection, string tableName)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT to_regclass(@tableName) IS NOT NULL";
        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = $"public.\"{tableName}\"";
        cmd.Parameters.Add(param);
        return cmd.ExecuteScalar() is true;
    }

    private static bool HistoryContainsMigration(IDbConnection connection, string migrationId)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT 1 FROM \"__EFMigrationsHistory\" WHERE \"MigrationId\" = @migrationId LIMIT 1";
        var param = cmd.CreateParameter();
        param.ParameterName = "@migrationId";
        param.Value = migrationId;
        cmd.Parameters.Add(param);
        return cmd.ExecuteScalar() is not null;
    }

    private static void CreateHistoryTable(IDbConnection connection)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" character varying(150) NOT NULL,
                "ProductVersion" character varying(32) NOT NULL,
                CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
            );
            """;
        cmd.ExecuteNonQuery();
    }

    private static void InsertMigrationHistoryRow(IDbConnection connection, string migrationId)
    {
        var version = typeof(DbContext).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?.Split('+')[0]
            ?? typeof(DbContext).Assembly.GetName().Version?.ToString()
            ?? "9.0.0";

        using var cmd = connection.CreateCommand();
        cmd.CommandText = """
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES (@migrationId, @productVersion);
            """;
        var migrationParam = cmd.CreateParameter();
        migrationParam.ParameterName = "@migrationId";
        migrationParam.Value = migrationId;
        cmd.Parameters.Add(migrationParam);

        var versionParam = cmd.CreateParameter();
        versionParam.ParameterName = "@productVersion";
        versionParam.Value = version;
        cmd.Parameters.Add(versionParam);

        cmd.ExecuteNonQuery();
    }
}
