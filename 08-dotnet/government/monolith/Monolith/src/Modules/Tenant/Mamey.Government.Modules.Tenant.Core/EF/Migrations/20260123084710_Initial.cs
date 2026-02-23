using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Government.Modules.Tenant.Core.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenant");

            migrationBuilder.CreateTable(
                name: "inbox",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    trace_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    headers = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_invites",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_invites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_user_mappings",
                schema: "tenant",
                columns: table => new
                {
                    issuer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_user_mappings", x => new { x.issuer, x.subject });
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_settings",
                schema: "tenant",
                columns: table => new
                {
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branding_json = table.Column<string>(type: "text", nullable: false),
                    activation_json = table.Column<string>(type: "text", nullable: true),
                    document_naming_config = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_settings", x => x.tenant_id);
                    table.ForeignKey(
                        name: "fk_tenant_settings_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "tenant",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_invites_tenant_id_email",
                schema: "tenant",
                table: "tenant_invites",
                columns: new[] { "tenant_id", "email" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_user_mappings_email",
                schema: "tenant",
                table: "tenant_user_mappings",
                column: "email",
                filter: "\"email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_user_mappings_tenant_id",
                schema: "tenant",
                table: "tenant_user_mappings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_domain",
                schema: "tenant",
                table: "tenants",
                column: "domain",
                unique: true,
                filter: "\"domain\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "outbox",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "tenant_invites",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "tenant_settings",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "tenant_user_mappings",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "tenant");
        }
    }
}
