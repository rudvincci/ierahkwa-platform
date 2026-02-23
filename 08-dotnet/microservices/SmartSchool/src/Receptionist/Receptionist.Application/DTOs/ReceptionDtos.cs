using Receptionist.Domain.Entities;

namespace Receptionist.Application.DTOs;

// Admission Enquiry DTOs
public class AdmissionEnquiryDto
{
    public int Id { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? GradeId { get; set; }
    public string? GradeName { get; set; }
    public DateTime EnquiryDate { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public EnquiryStatus Status { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
}

public class CreateAdmissionEnquiryDto
{
    public string StudentName { get; set; } = string.Empty;
    public string? ParentName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? GradeId { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public DateTime? FollowUpDate { get; set; }
}

// Visitor Book DTOs
public class VisitorBookDto
{
    public int Id { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IdNumber { get; set; }
    public string? Purpose { get; set; }
    public string? PersonToMeet { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Badge { get; set; }
    public string? Notes { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
}

public class CreateVisitorBookDto
{
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IdNumber { get; set; }
    public string? Purpose { get; set; }
    public string? PersonToMeet { get; set; }
    public string? Badge { get; set; }
    public string? Notes { get; set; }
    public int? StudentId { get; set; }
}

// Phone Log DTOs
public class PhoneLogDto
{
    public int Id { get; set; }
    public string CallerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CallType CallType { get; set; }
    public DateTime CallDate { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    public string? FollowUp { get; set; }
    public int? ReceivedBy { get; set; }
    public string? ReceivedByName { get; set; }
}

public class CreatePhoneLogDto
{
    public string CallerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CallType CallType { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    public string? FollowUp { get; set; }
}

// Postal DTOs
public class PostalDispatchDto
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string ToTitle { get; set; } = string.Empty;
    public string? ToAddress { get; set; }
    public string? FromTitle { get; set; }
    public DateTime DispatchDate { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public string? Attachment { get; set; }
}

public class CreatePostalDispatchDto
{
    public string ToTitle { get; set; } = string.Empty;
    public string? ToAddress { get; set; }
    public string? FromTitle { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
}

public class PostalReceiveDto
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public string FromTitle { get; set; } = string.Empty;
    public string? FromAddress { get; set; }
    public string? ToTitle { get; set; }
    public DateTime ReceiveDate { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
    public string? Attachment { get; set; }
}

public class CreatePostalReceiveDto
{
    public string FromTitle { get; set; } = string.Empty;
    public string? FromAddress { get; set; }
    public string? ToTitle { get; set; }
    public string? Type { get; set; }
    public string? Notes { get; set; }
}

// Complain DTOs
public class ComplainDto
{
    public int Id { get; set; }
    public string ComplainNumber { get; set; } = string.Empty;
    public string ComplainerName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ComplainType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ComplainDate { get; set; }
    public ComplainStatus Status { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
}

public class CreateComplainDto
{
    public string ComplainerName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public ComplainType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? StudentId { get; set; }
}

public class ResolveComplainDto
{
    public int Id { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public ComplainStatus Status { get; set; } = ComplainStatus.Resolved;
}
