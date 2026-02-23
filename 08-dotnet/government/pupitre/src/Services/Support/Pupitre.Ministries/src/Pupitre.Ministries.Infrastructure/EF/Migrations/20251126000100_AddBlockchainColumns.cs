using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Ministries.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockchainColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                schema: "ministries",
                table: "ministrydata",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                schema: "ministries",
                table: "ministrydata",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                schema: "ministries",
                table: "ministrydata",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "completion_date",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "credential_status",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "credential_type",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "nationality",
                schema: "ministries",
                table: "ministrydata");

            migrationBuilder.DropColumn(
                name: "program_code",
                schema: "ministries",
                table: "ministrydata");
        }
    }
}
