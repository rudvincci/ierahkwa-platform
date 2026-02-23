using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class ApplicationFormPdfGenerator : IApplicationFormPdfGenerator
{
    public ApplicationFormPdfGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateCit001AAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-A: Citizenship Application").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Personal Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {(!string.IsNullOrWhiteSpace(data.MiddleName) ? data.MiddleName + " " : "")}{data.LastName}");
                        column.Item().Text($"Date of Birth: {data.DateOfBirth:yyyy-MM-dd}");
                        if (!string.IsNullOrWhiteSpace(data.PlaceOfBirth))
                            column.Item().Text($"Place of Birth: {data.PlaceOfBirth}");
                        if (!string.IsNullOrWhiteSpace(data.CountryOfOrigin))
                            column.Item().Text($"Country of Origin: {data.CountryOfOrigin}");
                        if (!string.IsNullOrWhiteSpace(data.Sex))
                            column.Item().Text($"Sex: {data.Sex}");
                        if (!string.IsNullOrWhiteSpace(data.Height))
                            column.Item().Text($"Height: {data.Height}");
                        if (!string.IsNullOrWhiteSpace(data.EyeColor))
                            column.Item().Text($"Eye Color: {data.EyeColor}");
                        if (!string.IsNullOrWhiteSpace(data.HairColor))
                            column.Item().Text($"Hair Color: {data.HairColor}");
                        if (!string.IsNullOrWhiteSpace(data.MaritalStatus))
                            column.Item().Text($"Marital Status: {data.MaritalStatus}");
                        if (!string.IsNullOrWhiteSpace(data.PreviousNames))
                            column.Item().Text($"Previous Names: {data.PreviousNames}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Contact Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Email: {data.Email}");
                        if (!string.IsNullOrWhiteSpace(data.PhoneNumber))
                            column.Item().Text($"Phone: {data.PhoneNumber}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Address").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        if (!string.IsNullOrWhiteSpace(data.AddressLine1))
                        {
                            column.Item().Text(data.AddressLine1);
                            if (!string.IsNullOrWhiteSpace(data.City) || !string.IsNullOrWhiteSpace(data.Region) || !string.IsNullOrWhiteSpace(data.PostalCode))
                            {
                                column.Item().Text($"{data.City}, {data.Region} {data.PostalCode}".Trim());
                            }
                        }

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001BAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-B: Treaty Acknowledgment").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Acknowledgment").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text("I acknowledge that I have read and understand the treaties and agreements that govern citizenship in this jurisdiction.");
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Acknowledged: {(data.AcknowledgeTreaty ? "Yes" : "No")}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001CAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-C: Affidavit of Allegiance").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Affidavit").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"I, {data.FirstName} {data.LastName}, do solemnly swear (or affirm) that I will support and defend the Constitution and laws of this jurisdiction against all enemies, foreign and domestic; that I will bear true faith and allegiance to the same; and that I take this obligation freely, without any mental reservation or purpose of evasion.");
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Sworn: {(data.SwearAllegiance ? "Yes" : "No")}");
                        if (data.AffidavitDate.HasValue)
                            column.Item().Text($"Date of Affidavit: {data.AffidavitDate.Value:yyyy-MM-dd}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001DAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-D: Supporting Document Checklist").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Document Checklist").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Birth Certificate: {(data.HasBirthCertificate ? "✓" : "✗")}");
                        column.Item().Text($"Government-issued Photo ID: {(data.HasPhotoId ? "✓" : "✗")}");
                        column.Item().Text($"Proof of Residence: {(data.HasProofOfResidence ? "✓" : "✗")}");
                        column.Item().Text($"Background Check: {(data.HasBackgroundCheck ? "✓" : "✗")}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001EAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-E: Biometric Enrollment Authorization").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Authorization").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text("I authorize the collection of my biometric data (fingerprints, photograph, and/or other biometric identifiers) for the purpose of identity verification, document issuance, and security purposes. I understand that this biometric data will be stored securely and used only for authorized purposes.");
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Authorized: {(data.AuthorizeBiometricEnrollment ? "Yes" : "No")}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001GAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-G: Declaration of Understanding").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Rights and Responsibilities").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text("I declare that I understand the rights and responsibilities of citizenship, including but not limited to:");
                        column.Item().PaddingTop(3);
                        column.Item().Text("• My right to vote in elections");
                        column.Item().Text("• My right to hold public office (subject to eligibility requirements)");
                        column.Item().Text("• My responsibility to obey all laws");
                        column.Item().Text("• My responsibility to pay taxes");
                        column.Item().Text("• My responsibility to serve on juries when called");
                        column.Item().Text("• My responsibility to defend the jurisdiction if required");
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Declared: {(data.DeclareUnderstanding ? "Yes" : "No")}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GenerateCit001HAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text("CIT-001-H: Consent to Verification and Data Processing").FontSize(16).Bold();
                        column.Item().PaddingTop(10);
                        column.Item().Text($"Application Number: {data.ApplicationNumber}").FontSize(12);
                        column.Item().PaddingTop(10);

                        column.Item().Text("Applicant Information").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Name: {data.FirstName} {data.LastName}");
                        column.Item().Text($"Email: {data.Email}");

                        column.Item().PaddingTop(10);
                        column.Item().Text("Consent").FontSize(14).Bold();
                        column.Item().PaddingTop(5);
                        column.Item().Text("I consent to the verification of the information provided in this application and to the processing of my personal data for the purposes of citizenship application review, document issuance, and government record-keeping.");
                        column.Item().PaddingTop(5);
                        column.Item().Text($"Consent to Verification: {(data.ConsentToVerification ? "Yes" : "No")}");
                        column.Item().Text($"Consent to Data Processing: {(data.ConsentToDataProcessing ? "Yes" : "No")}");

                        column.Item().PaddingTop(10);
                        column.Item().Text($"Submitted: {data.SubmittedAt:yyyy-MM-dd HH:mm:ss} UTC").FontSize(8).Italic();
                    });
            });
        });

        var bytes = pdf.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public async Task<Dictionary<string, byte[]>> GenerateAllFormsAsync(ApplicationFormData data, CancellationToken ct = default)
    {
        var forms = new Dictionary<string, byte[]>
        {
            ["CIT-001-A"] = await GenerateCit001AAsync(data, ct),
            ["CIT-001-B"] = await GenerateCit001BAsync(data, ct),
            ["CIT-001-C"] = await GenerateCit001CAsync(data, ct),
            ["CIT-001-D"] = await GenerateCit001DAsync(data, ct),
            ["CIT-001-E"] = await GenerateCit001EAsync(data, ct),
            ["CIT-001-G"] = await GenerateCit001GAsync(data, ct),
            ["CIT-001-H"] = await GenerateCit001HAsync(data, ct)
        };

        return forms;
    }
}

