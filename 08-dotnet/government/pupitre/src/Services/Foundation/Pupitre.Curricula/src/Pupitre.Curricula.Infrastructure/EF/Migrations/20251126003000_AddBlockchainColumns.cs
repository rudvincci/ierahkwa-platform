using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Curricula.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "curriculum",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "curriculum",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "curriculum",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "curriculum",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "curriculum");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "curriculum");
        }
    }
}
