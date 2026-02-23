using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "citizenship_applications");

            migrationBuilder.CreateTable(
                name: "application_tokens",
                schema: "citizenship_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_application_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "citizenship_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    name_last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone_country_code = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_extension = table.Column<string>(type: "text", nullable: true),
                    phone_is_default = table.Column<bool>(type: "boolean", nullable: true),
                    address_firm_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_line = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_line3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_urbanization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_zip5 = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    address_zip4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    address_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_province = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    address_is_default = table.Column<bool>(type: "boolean", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    step = table.Column<int>(type: "integer", nullable: false),
                    rejection_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    extended_data_json = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: true),
                    access_logs_json = table.Column<string>(type: "character varying(50000)", maxLength: 50000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_primary_application = table.Column<bool>(type: "boolean", nullable: false),
                    have_foreign_citizenship_application = table.Column<bool>(type: "boolean", nullable: true),
                    have_criminal_record = table.Column<bool>(type: "boolean", nullable: true),
                    personal_details = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    contact_information = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    foreign_identification = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    dependents = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    residency_history = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    immigration_histories = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    education_qualifications = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    employment_history = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    foreign_citizenship_applications = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    criminal_history = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    references = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    payment_transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    rejected_by = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    rush_to_citizen = table.Column<bool>(type: "boolean", nullable: false),
                    rush_to_diplomacy = table.Column<bool>(type: "boolean", nullable: false),
                    fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    identification_card_fee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox",
                schema: "citizenship_applications",
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
                schema: "citizenship_applications",
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
                name: "uploaded_documents",
                schema: "citizenship_applications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    storage_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_uploaded_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_uploaded_documents_applications_application_id",
                        column: x => x.application_id,
                        principalSchema: "citizenship_applications",
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_application_id",
                schema: "citizenship_applications",
                table: "application_tokens",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_email",
                schema: "citizenship_applications",
                table: "application_tokens",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_expires_at",
                schema: "citizenship_applications",
                table: "application_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_token_hash",
                schema: "citizenship_applications",
                table: "application_tokens",
                column: "token_hash");

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_token_hash_email",
                schema: "citizenship_applications",
                table: "application_tokens",
                columns: new[] { "token_hash", "email" });

            migrationBuilder.CreateIndex(
                name: "ix_application_tokens_used_at",
                schema: "citizenship_applications",
                table: "application_tokens",
                column: "used_at",
                filter: "\"used_at\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_application_number",
                schema: "citizenship_applications",
                table: "applications",
                column: "application_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_applications_approved_at",
                schema: "citizenship_applications",
                table: "applications",
                column: "approved_at",
                filter: "\"approved_at\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_created_at",
                schema: "citizenship_applications",
                table: "applications",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_applications_email",
                schema: "citizenship_applications",
                table: "applications",
                column: "email",
                filter: "\"email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_payment_transaction_id",
                schema: "citizenship_applications",
                table: "applications",
                column: "payment_transaction_id",
                filter: "\"payment_transaction_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_rejected_at",
                schema: "citizenship_applications",
                table: "applications",
                column: "rejected_at",
                filter: "\"rejected_at\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_status",
                schema: "citizenship_applications",
                table: "applications",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_applications_status_created_at",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_step",
                schema: "citizenship_applications",
                table: "applications",
                column: "step");

            migrationBuilder.CreateIndex(
                name: "ix_applications_submitted_at",
                schema: "citizenship_applications",
                table: "applications",
                column: "submitted_at",
                filter: "\"submitted_at\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id",
                schema: "citizenship_applications",
                table: "applications",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_created_at",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_payment_transaction_id",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "payment_transaction_id" },
                filter: "\"payment_transaction_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_reviewed_by",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "reviewed_by" },
                filter: "\"reviewed_by\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_status",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_status_created_at",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_status_step",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "status", "step" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_tenant_id_step",
                schema: "citizenship_applications",
                table: "applications",
                columns: new[] { "tenant_id", "step" });

            migrationBuilder.CreateIndex(
                name: "ix_applications_updated_at",
                schema: "citizenship_applications",
                table: "applications",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "ix_uploaded_documents_application_id",
                schema: "citizenship_applications",
                table: "uploaded_documents",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "ix_uploaded_documents_application_id_document_type",
                schema: "citizenship_applications",
                table: "uploaded_documents",
                columns: new[] { "application_id", "document_type" });

            migrationBuilder.CreateIndex(
                name: "ix_uploaded_documents_document_type",
                schema: "citizenship_applications",
                table: "uploaded_documents",
                column: "document_type");

            migrationBuilder.CreateIndex(
                name: "ix_uploaded_documents_uploaded_at",
                schema: "citizenship_applications",
                table: "uploaded_documents",
                column: "uploaded_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_tokens",
                schema: "citizenship_applications");

            migrationBuilder.DropTable(
                name: "inbox",
                schema: "citizenship_applications");

            migrationBuilder.DropTable(
                name: "outbox",
                schema: "citizenship_applications");

            migrationBuilder.DropTable(
                name: "uploaded_documents",
                schema: "citizenship_applications");

            migrationBuilder.DropTable(
                name: "applications",
                schema: "citizenship_applications");
        }
    }
}
