using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Assessments.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "assessment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "assessment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "assessment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "assessment",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "assessment");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "assessment");
        }
    }
}
