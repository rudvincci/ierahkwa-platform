using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Portal.Tenant.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantActivationChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivationJson",
                table: "tenant_settings",
                type: "character varying(65535)",
                maxLength: 65535,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationJson",
                table: "tenant_settings");
        }
    }
}
