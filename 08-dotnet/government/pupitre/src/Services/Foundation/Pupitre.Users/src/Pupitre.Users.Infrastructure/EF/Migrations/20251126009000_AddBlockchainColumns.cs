using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Users.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "user",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "user");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "user");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "user");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "user");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "user");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "user");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "user");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "user");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "user");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "user");
        }
    }
}
