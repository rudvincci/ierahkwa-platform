using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Notifications.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockchainColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "notification",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "notification",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "notification",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "notification");
        }
    }
}
