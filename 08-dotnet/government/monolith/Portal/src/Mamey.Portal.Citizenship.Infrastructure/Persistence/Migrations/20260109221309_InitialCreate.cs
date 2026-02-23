using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mamey.Portal.Citizenship.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "citizenship_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ApplicationNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Height = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    EyeColor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    HairColor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Sex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CountryOfOrigin = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MaritalStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PreviousNames = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    City = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Region = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AcknowledgeTreaty = table.Column<bool>(type: "boolean", nullable: false),
                    SwearAllegiance = table.Column<bool>(type: "boolean", nullable: false),
                    AffidavitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasBirthCertificate = table.Column<bool>(type: "boolean", nullable: false),
                    HasPhotoId = table.Column<bool>(type: "boolean", nullable: false),
                    HasProofOfResidence = table.Column<bool>(type: "boolean", nullable: false),
                    HasBackgroundCheck = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorizeBiometricEnrollment = table.Column<bool>(type: "boolean", nullable: false),
                    DeclareUnderstanding = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentToVerification = table.Column<bool>(type: "boolean", nullable: false),
                    ConsentToDataProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    ExtendedDataJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenship_applications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "citizenship_statuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusGrantedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    YearsCompleted = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenship_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "citizenship_issued_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageBucket = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenship_issued_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_citizenship_issued_documents_citizenship_applications_Appli~",
                        column: x => x.ApplicationId,
                        principalTable: "citizenship_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "citizenship_uploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FileName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    StorageBucket = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    UploadedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenship_uploads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_citizenship_uploads_citizenship_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "citizenship_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "intake_reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewerName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApplicationComplete = table.Column<bool>(type: "boolean", nullable: false),
                    AllDocumentsReceived = table.Column<bool>(type: "boolean", nullable: false),
                    IdentityVerified = table.Column<bool>(type: "boolean", nullable: false),
                    BackgroundCheckComplete = table.Column<bool>(type: "boolean", nullable: false),
                    BirthCertificateVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PhotoIdVerified = table.Column<bool>(type: "boolean", nullable: false),
                    ProofOfResidenceVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PassportPhotoVerified = table.Column<bool>(type: "boolean", nullable: false),
                    SignatureVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CompletenessNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DocumentNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    AdditionalNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Recommendation = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RecommendationReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_intake_reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_intake_reviews_citizenship_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "citizenship_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PaymentReference = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PaymentGateway = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_plans_citizenship_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "citizenship_applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "status_progression_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CitizenshipStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    TargetStatus = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    YearsCompletedAtApplication = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_progression_applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_status_progression_applications_citizenship_statuses_Citize~",
                        column: x => x.CitizenshipStatusId,
                        principalTable: "citizenship_statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_applications_TenantId_ApplicationNumber",
                table: "citizenship_applications",
                columns: new[] { "TenantId", "ApplicationNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_issued_documents_ApplicationId_Kind",
                table: "citizenship_issued_documents",
                columns: new[] { "ApplicationId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_issued_documents_DocumentNumber",
                table: "citizenship_issued_documents",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_statuses_ApplicationId",
                table: "citizenship_statuses",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_statuses_TenantId_Email",
                table: "citizenship_statuses",
                columns: new[] { "TenantId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_citizenship_uploads_ApplicationId_Kind",
                table: "citizenship_uploads",
                columns: new[] { "ApplicationId", "Kind" });

            migrationBuilder.CreateIndex(
                name: "IX_intake_reviews_ApplicationId",
                table: "intake_reviews",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_intake_reviews_TenantId_ApplicationId",
                table: "intake_reviews",
                columns: new[] { "TenantId", "ApplicationId" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_plans_ApplicationId",
                table: "payment_plans",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_plans_TenantId_ApplicationId",
                table: "payment_plans",
                columns: new[] { "TenantId", "ApplicationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_status_progression_applications_CitizenshipStatusId",
                table: "status_progression_applications",
                column: "CitizenshipStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_status_progression_applications_TenantId_ApplicationNumber",
                table: "status_progression_applications",
                columns: new[] { "TenantId", "ApplicationNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "citizenship_issued_documents");

            migrationBuilder.DropTable(
                name: "citizenship_uploads");

            migrationBuilder.DropTable(
                name: "intake_reviews");

            migrationBuilder.DropTable(
                name: "payment_plans");

            migrationBuilder.DropTable(
                name: "status_progression_applications");

            migrationBuilder.DropTable(
                name: "citizenship_applications");

            migrationBuilder.DropTable(
                name: "citizenship_statuses");
        }
    }
}
