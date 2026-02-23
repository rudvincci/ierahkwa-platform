using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Educators.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "educator",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "educator",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "educator",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "educator",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "educator");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "educator");
        }
    }
}
