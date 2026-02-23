using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Parents.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "parent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "parent",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "parent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "parent",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "parent");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "parent");
        }
    }
}
