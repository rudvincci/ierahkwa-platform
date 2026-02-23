using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Government.Modules.TravelIdentities.Core.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "travel_identities");

            migrationBuilder.CreateTable(
                name: "inbox",
                schema: "travel_identities",
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
                schema: "travel_identities",
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
                name: "travel_identities",
                schema: "travel_identities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    citizen_id = table.Column<Guid>(type: "uuid", nullable: false),
                    travel_identity_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    issued_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    pdf417_barcode = table.Column<string>(type: "text", nullable: true),
                    document_path = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revocation_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_travel_identities", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_travel_identities_citizen_id",
                schema: "travel_identities",
                table: "travel_identities",
                column: "citizen_id");

            migrationBuilder.CreateIndex(
                name: "ix_travel_identities_tenant_id",
                schema: "travel_identities",
                table: "travel_identities",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_travel_identities_travel_identity_number",
                schema: "travel_identities",
                table: "travel_identities",
                column: "travel_identity_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox",
                schema: "travel_identities");

            migrationBuilder.DropTable(
                name: "outbox",
                schema: "travel_identities");

            migrationBuilder.DropTable(
                name: "travel_identities",
                schema: "travel_identities");
        }
    }
}
