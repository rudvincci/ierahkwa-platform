using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.ApplicationName.Modules.Products.Core.EF.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "banking-products-module");

            migrationBuilder.CreateTable(
                name: "Inbox",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Limits",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    MaximumBalance = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DailyTransactionLimit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    WithdrawalLimit = table.Column<decimal>(type: "numeric(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outbox",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductType = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InterestRateId = table.Column<Guid>(type: "uuid", nullable: true),
                    LimitsId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountCategory = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Limits_LimitsId",
                        column: x => x.LimitsId,
                        principalSchema: "banking-products-module",
                        principalTable: "Limits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicableTaxes",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicableTaxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicableTaxes_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Benefits",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BenefitType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Benefits_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EligibilityCriterias",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: true),
                    MaxAge = table.Column<int>(type: "integer", nullable: true),
                    MinimumIncome = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Geography = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OtherCriteria = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EligibilityCriterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EligibilityCriterias_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fees",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeeType = table.Column<string>(type: "text", maxLength: 0, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Frequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fees_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterestRates",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<double>(type: "numeric(18,4)", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CompoundingFrequency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestRates_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequiredDocuments",
                schema: "banking-products-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BankingProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequiredDocuments_Products_BankingProductId",
                        column: x => x.BankingProductId,
                        principalSchema: "banking-products-module",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicableTaxes_BankingProductId",
                schema: "banking-products-module",
                table: "ApplicableTaxes",
                column: "BankingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Benefits_BankingProductId",
                schema: "banking-products-module",
                table: "Benefits",
                column: "BankingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_EligibilityCriterias_BankingProductId",
                schema: "banking-products-module",
                table: "EligibilityCriterias",
                column: "BankingProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fees_BankingProductId",
                schema: "banking-products-module",
                table: "Fees",
                column: "BankingProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestRates_BankingProductId",
                schema: "banking-products-module",
                table: "InterestRates",
                column: "BankingProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_LimitsId",
                schema: "banking-products-module",
                table: "Products",
                column: "LimitsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                schema: "banking-products-module",
                table: "Products",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequiredDocuments_BankingProductId",
                schema: "banking-products-module",
                table: "RequiredDocuments",
                column: "BankingProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicableTaxes",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Benefits",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "EligibilityCriterias",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Fees",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Inbox",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "InterestRates",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "RequiredDocuments",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "banking-products-module");

            migrationBuilder.DropTable(
                name: "Limits",
                schema: "banking-products-module");
        }
    }
}
