namespace Mamey.Portal.Shared.Storage.DocumentNaming;

public sealed class DocumentNamingPattern
{
    public string PersonalDocumentPattern { get; set; } =
        "citizenship/{ApplicationNumber}/personal/{Unique}-{OriginalFileName}";

    public string PassportPhotoPattern { get; set; } =
        "citizenship/{ApplicationNumber}/passport-photo/{Unique}-{OriginalFileName}";

    public string SignatureImagePattern { get; set; } =
        "citizenship/{ApplicationNumber}/signature/{Unique}-{OriginalFileName}";

    public string CitizenshipCertificatePattern { get; set; } =
        "citizenship/{ApplicationNumber}/generated/certificate/{Unique}-{OriginalFileName}";

    public string BirthCertificatePattern { get; set; } =
        "citizenship/{ApplicationNumber}/generated/birth-certificate/{Unique}-{OriginalFileName}";

    public string MarriageCertificatePattern { get; set; } =
        "citizenship/{ApplicationNumber}/generated/marriage-certificate/{Unique}-{OriginalFileName}";

    public string NameChangeCertificatePattern { get; set; } =
        "citizenship/{ApplicationNumber}/generated/name-change/{Unique}-{OriginalFileName}";

    public string PassportDocumentPattern { get; set; } =
        "citizenship/{ApplicationNumber}/issued/passport/{Unique}-{OriginalFileName}";

    public string IdCardDocumentPattern { get; set; } =
        "citizenship/{ApplicationNumber}/issued/id-card/{Kind}/{Unique}-{OriginalFileName}";

    public string VehicleTagDocumentPattern { get; set; } =
        "citizenship/{ApplicationNumber}/issued/vehicle-tag/{Kind}/{Unique}-{OriginalFileName}";

    public static DocumentNamingPattern Default { get; } = new();
}

