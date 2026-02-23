using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Lessons.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "lesson",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "lesson",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "lesson",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "lesson",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "lesson");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "lesson");
        }
    }
}
