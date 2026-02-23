using System.Reactive;
using ReactiveUI;

namespace Mamey.Portal.Web.ViewModels.Citizenship;

public sealed class IntakeReviewViewModel : ReactiveObject
{
    private Guid _applicationId;
    public Guid ApplicationId
    {
        get => _applicationId;
        set => this.RaiseAndSetIfChanged(ref _applicationId, value);
    }

    private string _reviewerName = string.Empty;
    public string ReviewerName
    {
        get => _reviewerName;
        set => this.RaiseAndSetIfChanged(ref _reviewerName, value);
    }

    private DateTime? _reviewDate;
    public DateTime? ReviewDate
    {
        get => _reviewDate;
        set => this.RaiseAndSetIfChanged(ref _reviewDate, value);
    }

    // Application Completeness
    private bool _applicationComplete = false;
    public bool ApplicationComplete
    {
        get => _applicationComplete;
        set => this.RaiseAndSetIfChanged(ref _applicationComplete, value);
    }

    private bool _allDocumentsReceived = false;
    public bool AllDocumentsReceived
    {
        get => _allDocumentsReceived;
        set => this.RaiseAndSetIfChanged(ref _allDocumentsReceived, value);
    }

    private bool _identityVerified = false;
    public bool IdentityVerified
    {
        get => _identityVerified;
        set => this.RaiseAndSetIfChanged(ref _identityVerified, value);
    }

    private bool _backgroundCheckComplete = false;
    public bool BackgroundCheckComplete
    {
        get => _backgroundCheckComplete;
        set => this.RaiseAndSetIfChanged(ref _backgroundCheckComplete, value);
    }

    // Document Verification
    private bool _birthCertificateVerified = false;
    public bool BirthCertificateVerified
    {
        get => _birthCertificateVerified;
        set => this.RaiseAndSetIfChanged(ref _birthCertificateVerified, value);
    }

    private bool _photoIdVerified = false;
    public bool PhotoIdVerified
    {
        get => _photoIdVerified;
        set => this.RaiseAndSetIfChanged(ref _photoIdVerified, value);
    }

    private bool _proofOfResidenceVerified = false;
    public bool ProofOfResidenceVerified
    {
        get => _proofOfResidenceVerified;
        set => this.RaiseAndSetIfChanged(ref _proofOfResidenceVerified, value);
    }

    private bool _passportPhotoVerified = false;
    public bool PassportPhotoVerified
    {
        get => _passportPhotoVerified;
        set => this.RaiseAndSetIfChanged(ref _passportPhotoVerified, value);
    }

    private bool _signatureVerified = false;
    public bool SignatureVerified
    {
        get => _signatureVerified;
        set => this.RaiseAndSetIfChanged(ref _signatureVerified, value);
    }

    // Review Notes
    private string _completenessNotes = string.Empty;
    public string CompletenessNotes
    {
        get => _completenessNotes;
        set => this.RaiseAndSetIfChanged(ref _completenessNotes, value);
    }

    private string _documentNotes = string.Empty;
    public string DocumentNotes
    {
        get => _documentNotes;
        set => this.RaiseAndSetIfChanged(ref _documentNotes, value);
    }

    private string _additionalNotes = string.Empty;
    public string AdditionalNotes
    {
        get => _additionalNotes;
        set => this.RaiseAndSetIfChanged(ref _additionalNotes, value);
    }

    // Recommendations
    private string _recommendation = string.Empty; // "Approve", "Reject", "RequestAdditionalInfo", "Pending"
    public string Recommendation
    {
        get => _recommendation;
        set => this.RaiseAndSetIfChanged(ref _recommendation, value);
    }

    private string _recommendationReason = string.Empty;
    public string RecommendationReason
    {
        get => _recommendationReason;
        set => this.RaiseAndSetIfChanged(ref _recommendationReason, value);
    }

    public bool CanSubmit =>
        !string.IsNullOrWhiteSpace(ReviewerName) &&
        ReviewDate.HasValue &&
        !string.IsNullOrWhiteSpace(Recommendation);

    public ReactiveCommand<Unit, Unit> Submit { get; }

    public IntakeReviewViewModel()
    {
        ReviewDate = DateTime.UtcNow;
        Submit = ReactiveCommand.Create(() => { });
    }
}

