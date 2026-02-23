using Mamey.CQRS.Commands;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;

using Mamey.Contexts;
using Mamey.Exceptions;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Services;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Microservice.Infrastructure.Diagnostics;
using Microsoft.AspNetCore.Mvc;


namespace Mamey.Government.Identity.Api;

public static class UserRoutes
{
    public static IApplicationBuilder AddUserRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints=> endpoints.MapHealthEndpoint());
        app.UseEndpoints(endpoints =>
            {
                endpoints.Get($"/",
                        (ctx) => ctx.Response.WriteJsonAsync(ctx?.RequestServices?.GetService<AppOptions>()?.Name))
                    
                    // Authentication Routes
                    .Post("auth/sign-in", async (ctx) =>
                    {
                        var identityService = ctx.RequestServices.GetRequiredService<IIdentityService>();
                        var signInRequest = await ctx.ReadJsonAsync<SignInRequest>();
                        if (signInRequest == null || string.IsNullOrEmpty(signInRequest.UsernameOrEmail) || string.IsNullOrEmpty(signInRequest.Password))
                        {
                            ctx.Response.StatusCode = 400;
                            await ctx.Response.WriteJsonAsync(new { error = "Username or email and password are required" });
                            return;
                        }
                        
                        try
                        {
                            var ipAddress = ctx.Connection.RemoteIpAddress?.ToString();
                            var userAgent = ctx.Request.Headers["User-Agent"].ToString();
                            var authResult = await identityService.SignInAsync(signInRequest.UsernameOrEmail, signInRequest.Password, ipAddress, userAgent);
                            if (authResult == null)
                            {
                                ctx.Response.Unauthorized();
                                return;

                            }
                            
                            
                            await ctx.Response.Ok(authResult);
                        }
                        catch (Exception ex)
                        {
                            ctx.Response.Unauthorized();
                            return;
                        }
                    }, auth: false)
                    .Post("auth/sign-out", async (ctx) =>
                    {
                        var identityService = ctx.RequestServices.GetRequiredService<IIdentityService>();
                        var signOutRequest = await ctx.ReadJsonAsync<SignOutRequest>();
                        if (signOutRequest == null || string.IsNullOrEmpty(signOutRequest.RefreshToken))
                        {
                            ctx.Response.StatusCode = 400;
                            await ctx.Response.WriteJsonAsync(new { error = "Refresh token is required" });
                            return;
                        }
                        
                        try
                        {
                            await identityService.SignOutAsync(signOutRequest.RefreshToken);
                            ctx.Response.StatusCode = 200;
                            await ctx.Response.WriteJsonAsync(new { message = "Signed out successfully" });
                        }
                        catch (Exception ex)
                        {
                            ctx.Response.StatusCode = 400;
                            await ctx.Response.WriteJsonAsync(new { error = ex.Message });
                        }
                    }, auth: false)
                    .Post("auth/refresh-token", async (ctx) =>
                    {
                        var identityService = ctx.RequestServices.GetRequiredService<IIdentityService>();
                        var refreshRequest = await ctx.ReadJsonAsync<RefreshTokenRequest>();
                        if (refreshRequest == null || string.IsNullOrEmpty(refreshRequest.RefreshToken))
                        {
                            ctx.Response.StatusCode = 400;
                            await ctx.Response.WriteJsonAsync(new { error = "Refresh token is required" });
                            return;
                        }
                        
                        try
                        {
                            var authResult = await identityService.RefreshTokenAsync(refreshRequest.RefreshToken);
                            ctx.Response.StatusCode = 200;
                            await ctx.Response.WriteJsonAsync(authResult);
                        }
                        catch (Exception ex)
                        {
                            ctx.Response.StatusCode = 401;
                            await ctx.Response.WriteJsonAsync(new { error = ex.Message });
                        }
                    }, auth: false)
                    ;
     
            }
        );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints?
            // Subject Management Routes
            .Get<BrowseIdentity, PagedResult<Mamey.Government.Identity.Contracts.DTO.SubjectDto>>($"identity", auth: true)
            .Get<GetSubject, SubjectDetailsDto>($"identity/{{id:guid}}", auth: false)
            .Get<GetSubjectByEmail, SubjectDetailsDto>($"identity/by-email/{{email}}", auth: false)
            .Get<GetSubjectsByStatus, IEnumerable<SubjectDetailsDto>>($"identity/by-status/{{status}}", auth: false)
            .Get<GetSubjectsByRole, IEnumerable<SubjectDetailsDto>>($"identity/by-role/{{roleId:guid}}", auth: false)
            .Get<GetRecentlyAuthenticatedSubjects, IEnumerable<SubjectDetailsDto>>($"identity/recently-authenticated", auth: false)
            .Post<AddSubject>($"identity",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"identity/{cmd.Id}"), auth: false)
            .Put<UpdateSubject>($"identity/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Delete<DeleteSubject>($"identity/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // User Management Routes
            .Get<GetUser, UserDto>($"users/{{id:guid}}", auth: false)
            .Get<GetUserByEmail, UserDto>($"users/by-email/{{email}}", auth: false)
            .Get<BrowseUsers, PagedResult<UserDto>?>($"users", auth: false)
            .Get<GetUsersByStatus, IEnumerable<UserDto>>($"users/by-status/{{status}}", auth: false)
            .Get<GetUsersByRole, IEnumerable<UserDto>>($"users/by-role/{{roleId:guid}}", auth: false)
            .Get<GetUserSessions, IEnumerable<SessionDto>>($"users/{{userId:guid}}/sessions", auth: false)
            .Get<GetRecentlyActiveUsers, IEnumerable<UserDto>>($"users/recently-active", auth: false)
            .Get<GetUserStatistics, UserStatisticsDto>($"users/statistics", auth: false)
            .Post<CreateUser>($"users",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"users/{cmd.Id}"), auth: false)
            .Put<UpdateUser>($"users/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<ChangeUserPassword>($"users/{{id:guid}}/change-password",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<ActivateUser>($"users/{{id:guid}}/activate",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<DeactivateUser>($"users/{{id:guid}}/deactivate",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<LockUser>($"users/{{id:guid}}/lock",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<UnlockUser>($"users/{{id:guid}}/unlock",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Role Management Routes
            .Get<GetRole, RoleDto>($"roles/{{id:guid}}", auth: false)
            .Get<BrowseRoles, PagedResult<Mamey.Government.Identity.Contracts.DTO.RoleDto>?>($"roles", auth: false)
            .Get<SearchRoles, IEnumerable<RoleDto>>($"roles/search", auth: false)
            .Get<GetRolesByStatus, IEnumerable<RoleDto>>($"roles/by-status/{{status}}", auth: false)
            .Get<GetRolesByPermission, IEnumerable<RoleDto>>($"roles/by-permission/{{permissionId:guid}}", auth: false)
            .Get<GetRoleStatistics, RoleStatisticsDto>($"roles/statistics", auth: false)
            .Post<CreateRole>($"roles",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"roles/{cmd.Id}"), auth: false)
            .Put<UpdateRole>($"roles/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<AssignRoleToSubject>($"subjects/{{subjectId:guid}}/roles/{{roleId:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Delete<RemoveRoleFromSubject>($"subjects/{{subjectId:guid}}/roles/{{roleId:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Permission Management Routes
            .Get<GetPermission, PermissionDto>($"permissions/{{id:guid}}", auth: false)
            .Get<BrowsePermissions, PagedResult<PermissionDto>?>($"permissions", auth: false)
            .Get<SearchPermissions, IEnumerable<PermissionDto>>($"permissions/search", auth: false)
            .Get<GetPermissionsByStatus, IEnumerable<PermissionDto>>($"permissions/by-status/{{status}}", auth: false)
            .Get<GetPermissionsByRole, IEnumerable<PermissionDto>>($"permissions/by-role/{{roleId:guid}}", auth: false)
            .Get<GetPermissionsByResource, IEnumerable<PermissionDto>>($"permissions/by-resource/{{resource}}", auth: false)
            .Get<GetPermissionsByAction, IEnumerable<PermissionDto>>($"permissions/by-action/{{action}}", auth: false)
            .Get<GetPermissionStatistics, PermissionStatisticsDto>($"permissions/statistics", auth: false)
            .Post<CreatePermission>($"permissions",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"permissions/{cmd.Id}"), auth: false)
            .Put<UpdatePermission>($"permissions/{{id:guid}}",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Session Management Routes
            .Get<GetSession, SessionDto>($"sessions/{{id:guid}}", auth: false)
            .Get<GetSessionByRefreshToken, SessionDto>($"sessions/by-refresh-token/{{token}}", auth: false)
            .Get<GetSessionByAccessToken, SessionDto>($"sessions/by-access-token/{{token}}", auth: false)
            .Get<GetSessionStatistics, SessionStatisticsDto>($"sessions/statistics", auth: false)
            .Post<CreateSession>($"sessions",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"sessions/{cmd.Id}"), auth: false)
            .Post<RefreshSession>($"sessions/{{id:guid}}/refresh",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<RevokeSession>($"sessions/{{id:guid}}/revoke",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Email Confirmation Routes
            .Get<GetEmailConfirmationByCode, EmailConfirmationDto>($"email-confirmations/by-code/{{code}}", auth: false)
            .Get<GetEmailConfirmationByUserId, EmailConfirmationDto>($"email-confirmations/by-user/{{userId:guid}}", auth: false)
            .Get<GetPendingEmailConfirmations, IEnumerable<EmailConfirmationDto>>($"email-confirmations/pending", auth: false)
            .Get<GetEmailConfirmationStatistics, EmailConfirmationStatisticsDto>($"email-confirmations/statistics", auth: false)
            .Post<CreateEmailConfirmation>($"email-confirmations",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"email-confirmations/{cmd.Id}"), auth: false)
            .Post<ConfirmEmail>($"email-confirmations/confirm",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<ResendEmailConfirmation>($"email-confirmations/resend",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Two-Factor Authentication Routes
            .Get<GetTwoFactorAuthByUserId, TwoFactorAuthDto>($"two-factor-auth/by-user/{{userId:guid}}", auth: false)
            .Get<GetTwoFactorAuthStatistics, TwoFactorAuthStatisticsDto>($"two-factor-auth/statistics", auth: false)
            .Post<SetupTwoFactorAuth>($"two-factor-auth/setup",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"two-factor-auth/{cmd.Id}"), auth: false)
            .Post<ActivateTwoFactorAuth>($"two-factor-auth/activate",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<VerifyTwoFactorAuth>($"two-factor-auth/verify",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<DisableTwoFactorAuth>($"two-factor-auth/disable",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Multi-Factor Authentication Routes
            .Get<GetMultiFactorAuthByUserId, MultiFactorAuthDto>($"multi-factor-auth/by-user/{{userId:guid}}", auth: false)
            .Get<GetEnabledMfaMethods, IEnumerable<string>>($"multi-factor-auth/methods/by-user/{{userId:guid}}", auth: false)
            .Get<GetActiveMfaChallenge, MfaChallengeDto>($"multi-factor-auth/challenges/active/{{userId:guid}}", auth: false)
            .Get<GetMultiFactorAuthStatistics, MultiFactorAuthStatisticsDto>($"multi-factor-auth/statistics", auth: false)
            .Post<SetupMultiFactorAuth>($"multi-factor-auth/setup",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"multi-factor-auth/{cmd.Id}"), auth: false)
            .Post<EnableMfaMethod>($"multi-factor-auth/methods/enable",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<DisableMfaMethod>($"multi-factor-auth/methods/disable",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
            .Post<CreateMfaChallenge>($"multi-factor-auth/challenges",
                beforeDispatch: ([FromBody] cmd, ctx) => Task.CompletedTask,
                afterDispatch: ([FromBody] cmd, ctx) => ctx?.Response.Created($"multi-factor-auth/challenges/{cmd.Id}"), auth: false)
            .Post<VerifyMfaChallenge>($"multi-factor-auth/challenges/{{id:guid}}/verify",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)

            // Credential Management Routes
            .Post<RevokeCredential>($"credentials/{{id:guid}}/revoke",
                afterDispatch: (cmd, ctx) => ctx?.Response.Accepted(), auth: false)
        );
        return app;
    }
}

