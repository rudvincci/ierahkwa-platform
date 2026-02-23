using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.GLEs.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "gle",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "gle",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "gle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "gle",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "gle");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "gle");
        }
    }
}
