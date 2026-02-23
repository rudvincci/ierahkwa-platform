using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenant_document_naming",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PatternJson = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_document_naming", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_document_templates",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TemplateHtml = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_document_templates", x => new { x.TenantId, x.Kind });
                });

            migrationBuilder.CreateTable(
                name: "tenant_settings",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BrandingJson = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_settings", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "tenant_user_invites",
                columns: table => new
                {
                    Issuer = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_user_invites", x => new { x.Issuer, x.Email });
                });

            migrationBuilder.CreateTable(
                name: "tenant_user_mappings",
                columns: table => new
                {
                    Issuer = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_user_mappings", x => new { x.Issuer, x.Subject });
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.TenantId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_document_templates_Kind",
                table: "tenant_document_templates",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_document_templates_TenantId",
                table: "tenant_document_templates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_user_invites_Email",
                table: "tenant_user_invites",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_user_invites_TenantId",
                table: "tenant_user_invites",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_user_mappings_Email",
                table: "tenant_user_mappings",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_user_mappings_TenantId",
                table: "tenant_user_mappings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_document_naming");

            migrationBuilder.DropTable(
                name: "tenant_document_templates");

            migrationBuilder.DropTable(
                name: "tenant_settings");

            migrationBuilder.DropTable(
                name: "tenant_user_invites");

            migrationBuilder.DropTable(
                name: "tenant_user_mappings");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
