using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Rewards.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "reward",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "reward",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "reward",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "reward",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "reward");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "reward");
        }
    }
}
