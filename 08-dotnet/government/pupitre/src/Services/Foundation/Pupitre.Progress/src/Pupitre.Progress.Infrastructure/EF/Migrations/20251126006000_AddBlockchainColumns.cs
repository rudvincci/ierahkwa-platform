using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pupitre.Progress.Infrastructure.EF.Migrations
{
    public partial class AddBlockchainColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "blockchain_account",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "blockchain_metadata",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "citizen_id",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "completion_date",
                table: "learningprogress",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_hash",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_document_id",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "credential_issued_at",
                table: "learningprogress",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_status",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "credential_type",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "government_identity_id",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ledger_transaction_id",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "learningprogress",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "program_code",
                table: "learningprogress",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blockchain_account",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "blockchain_metadata",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "citizen_id",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "completion_date",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "credential_document_hash",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "credential_document_id",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "credential_issued_at",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "credential_status",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "credential_type",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "government_identity_id",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "ledger_transaction_id",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "learningprogress");

            migrationBuilder.DropColumn(
                name: "program_code",
                table: "learningprogress");
        }
    }
}
