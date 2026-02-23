using NET10.Core.Architecture;

namespace NET10.Core.Models.College;

// ═══════════════════════════════════════════════════════════════════════════════
// COLLEGE MANAGEMENT SYSTEM - MODELS
// Complete Academic Institution Management
// ═══════════════════════════════════════════════════════════════════════════════

// ═══════════════════════════════════════════════════════════════════════════════
// INSTITUTION
// ═══════════════════════════════════════════════════════════════════════════════
public class Institution : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public InstitutionType Type { get; set; } = InstitutionType.College;
    
    // Contact
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    
    // Administration
    public string PrincipalName { get; set; } = string.Empty;
    public string EstablishedYear { get; set; } = string.Empty;
    public string AffiliatedTo { get; set; } = string.Empty;
    public string AccreditationNumber { get; set; } = string.Empty;
    
    // Branding
    public string Logo { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#1e3a8a";
    public string Motto { get; set; } = string.Empty;
    
    // Settings
    public string Currency { get; set; } = "USD";
    public string Timezone { get; set; } = "UTC";
    public string AcademicYearFormat { get; set; } = "yyyy-yyyy"; // 2025-2026
    public int SemestersPerYear { get; set; } = 2;
    
    public bool IsActive { get; set; } = true;
}

public enum InstitutionType
{
    School,
    HighSchool,
    College,
    University,
    TechnicalInstitute,
    VocationalSchool
}

// ═══════════════════════════════════════════════════════════════════════════════
// ACADEMIC YEAR & SEMESTER
// ═══════════════════════════════════════════════════════════════════════════════
public class AcademicYear : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public string Name { get; set; } = string.Empty; // "2025-2026"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    public List<Semester> Semesters { get; set; } = [];
}

public class Semester : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty; // "Fall 2025", "Spring 2026"
    public int Number { get; set; } // 1, 2, 3...
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? RegistrationStartDate { get; set; }
    public DateTime? RegistrationEndDate { get; set; }
    public DateTime? ExamStartDate { get; set; }
    public DateTime? ExamEndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

// ═══════════════════════════════════════════════════════════════════════════════
// DEPARTMENT & COURSE
// ═══════════════════════════════════════════════════════════════════════════════
public class Department : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? HeadOfDepartmentId { get; set; }
    public string HeadOfDepartmentName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty; // Building/Room
    public bool IsActive { get; set; } = true;
}

public class Course : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty; // CS101
    public string Name { get; set; } = string.Empty; // Computer Science
    public string Description { get; set; } = string.Empty;
    public CourseLevel Level { get; set; } = CourseLevel.Undergraduate;
    public int DurationYears { get; set; } = 4;
    public int TotalCredits { get; set; }
    public int TotalSemesters { get; set; }
    
    // Fees
    public decimal TuitionFeePerSemester { get; set; }
    public decimal AdmissionFee { get; set; }
    public decimal LabFee { get; set; }
    public decimal OtherFees { get; set; }
    public decimal TotalFeePerSemester => TuitionFeePerSemester + LabFee + OtherFees;
    
    // Requirements
    public string EligibilityCriteria { get; set; } = string.Empty;
    public int MaxStudents { get; set; }
    public int CurrentEnrollment { get; set; }
    public int AvailableSeats => MaxStudents - CurrentEnrollment;
    
    public bool IsActive { get; set; } = true;
}

public enum CourseLevel
{
    Diploma,
    Associate,
    Undergraduate,
    Graduate,
    Postgraduate,
    Doctorate
}

// ═══════════════════════════════════════════════════════════════════════════════
// SUBJECT
// ═══════════════════════════════════════════════════════════════════════════════
public class Subject : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    
    public string Code { get; set; } = string.Empty; // CS101-01
    public string Name { get; set; } = string.Empty; // Introduction to Programming
    public string Description { get; set; } = string.Empty;
    
    public int SemesterNumber { get; set; } // Which semester this subject belongs to
    public int Credits { get; set; }
    public int LectureHoursPerWeek { get; set; }
    public int LabHoursPerWeek { get; set; }
    public int TutorialHoursPerWeek { get; set; }
    public int TotalHoursPerWeek => LectureHoursPerWeek + LabHoursPerWeek + TutorialHoursPerWeek;
    
    public SubjectType Type { get; set; } = SubjectType.Theory;
    public bool IsMandatory { get; set; } = true;
    public bool HasLab { get; set; } = false;
    
    // Grading
    public int PassingMarks { get; set; } = 40;
    public int MaxMarks { get; set; } = 100;
    public int InternalMarks { get; set; } = 30;
    public int ExternalMarks { get; set; } = 70;
    
    public Guid? TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}

public enum SubjectType
{
    Theory,
    Practical,
    TheoryWithPractical,
    Project,
    Seminar,
    Internship
}

// ═══════════════════════════════════════════════════════════════════════════════
// STUDENT
// ═══════════════════════════════════════════════════════════════════════════════
public class Student : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public string StudentId { get; set; } = string.Empty; // Roll Number / Enrollment Number
    public string RegistrationNumber { get; set; } = string.Empty;
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Religion { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // General, OBC, SC, ST, etc.
    
    // Contact
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AlternatePhone { get; set; } = string.Empty;
    
    // Address
    public string PermanentAddress { get; set; } = string.Empty;
    public string CurrentAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // Guardian Information
    public string FatherName { get; set; } = string.Empty;
    public string FatherOccupation { get; set; } = string.Empty;
    public string FatherPhone { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public string MotherOccupation { get; set; } = string.Empty;
    public string MotherPhone { get; set; } = string.Empty;
    public string GuardianName { get; set; } = string.Empty;
    public string GuardianRelation { get; set; } = string.Empty;
    public string GuardianPhone { get; set; } = string.Empty;
    public string EmergencyContact { get; set; } = string.Empty;
    
    // Academic Information
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int CurrentSemester { get; set; } = 1;
    public int CurrentYear { get; set; } = 1;
    public DateTime AdmissionDate { get; set; }
    public string AdmissionType { get; set; } = "Regular"; // Regular, Lateral, Transfer
    public string BatchYear { get; set; } = string.Empty; // 2025-2029
    public string Section { get; set; } = string.Empty; // A, B, C
    
    // Previous Education
    public string PreviousSchool { get; set; } = string.Empty;
    public string PreviousBoard { get; set; } = string.Empty;
    public decimal PreviousPercentage { get; set; }
    public string PreviousPassingYear { get; set; } = string.Empty;
    
    // Documents
    public string PhotoUrl { get; set; } = string.Empty;
    public string IdProofType { get; set; } = string.Empty;
    public string IdProofNumber { get; set; } = string.Empty;
    
    // Status
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public DateTime? GraduationDate { get; set; }
    public string WithdrawalReason { get; set; } = string.Empty;
    
    // Financial
    public decimal TotalFeesDue { get; set; }
    public decimal TotalFeesPaid { get; set; }
    public decimal FeeBalance => TotalFeesDue - TotalFeesPaid;
    public bool HasScholarship { get; set; } = false;
    public string ScholarshipType { get; set; } = string.Empty;
    public decimal ScholarshipAmount { get; set; }
}

public enum StudentStatus
{
    Active,
    Inactive,
    Graduated,
    Withdrawn,
    Suspended,
    Expelled,
    OnLeave,
    Transferred
}

// ═══════════════════════════════════════════════════════════════════════════════
// TEACHER / FACULTY
// ═══════════════════════════════════════════════════════════════════════════════
public class Teacher : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    
    // Contact
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AlternatePhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // Professional Information
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public TeacherDesignation Designation { get; set; } = TeacherDesignation.Lecturer;
    public string Specialization { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty; // "M.Tech, PhD"
    public string HighestDegree { get; set; } = string.Empty;
    public string University { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    
    // Employment
    public DateTime JoiningDate { get; set; }
    public DateTime? LeavingDate { get; set; }
    public EmploymentType EmploymentType { get; set; } = EmploymentType.Permanent;
    public decimal Salary { get; set; }
    public string SalaryType { get; set; } = "Monthly";
    public string BankName { get; set; } = string.Empty;
    public string BankAccount { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    
    // Documents
    public string PhotoUrl { get; set; } = string.Empty;
    public string IdProofType { get; set; } = string.Empty;
    public string IdProofNumber { get; set; } = string.Empty;
    
    // Status
    public TeacherStatus Status { get; set; } = TeacherStatus.Active;
    public string ResignationReason { get; set; } = string.Empty;
    
    // Assigned Subjects
    public List<Guid> AssignedSubjectIds { get; set; } = [];
}

public enum TeacherDesignation
{
    Lecturer,
    AssistantProfessor,
    AssociateProfessor,
    Professor,
    HeadOfDepartment,
    Dean,
    Principal,
    VisitingFaculty,
    LabAssistant
}

public enum EmploymentType
{
    Permanent,
    Contract,
    PartTime,
    Visiting,
    Temporary
}

public enum TeacherStatus
{
    Active,
    OnLeave,
    Resigned,
    Retired,
    Terminated,
    Suspended
}

// ═══════════════════════════════════════════════════════════════════════════════
// ATTENDANCE
// ═══════════════════════════════════════════════════════════════════════════════
public class StudentAttendance : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentRollNumber { get; set; } = string.Empty;
    
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public Guid? TeacherId { get; set; }
    
    public DateTime Date { get; set; }
    public int Period { get; set; } // 1st period, 2nd period, etc.
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string Remarks { get; set; } = string.Empty;
    
    public string MarkedBy { get; set; } = string.Empty;
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
}

public class TeacherAttendance : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string LeaveType { get; set; } = string.Empty; // Sick, Casual, etc.
    public string Remarks { get; set; } = string.Empty;
    
    public decimal WorkHours => CheckInTime.HasValue && CheckOutTime.HasValue 
        ? (decimal)(CheckOutTime.Value - CheckInTime.Value).TotalHours 
        : 0;
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    HalfDay,
    OnLeave,
    Holiday
}

// ═══════════════════════════════════════════════════════════════════════════════
// FEES MANAGEMENT
// ═══════════════════════════════════════════════════════════════════════════════
public class FeeStructure : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public Guid AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    
    public string FeeType { get; set; } = string.Empty; // Tuition, Lab, Library, etc.
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public FeeFrequency Frequency { get; set; } = FeeFrequency.Semester;
    
    // Applicable To
    public bool AppliesNewStudents { get; set; } = true;
    public bool AppliesExistingStudents { get; set; } = true;
    public string ApplicableCategories { get; set; } = string.Empty; // Comma-separated
    
    public bool IsMandatory { get; set; } = true;
    public DateTime? DueDate { get; set; }
    public decimal LateFee { get; set; }
    public int GracePeriodDays { get; set; } = 15;
    
    public bool IsActive { get; set; } = true;
}

public enum FeeFrequency
{
    OneTime,
    Monthly,
    Quarterly,
    Semester,
    Annual
}

public class StudentFee : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentRollNumber { get; set; } = string.Empty;
    
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    
    // Fee Details
    public List<FeeItem> Items { get; set; } = [];
    public decimal Subtotal => Items.Sum(i => i.Amount);
    public decimal Discount { get; set; }
    public decimal DiscountReason { get; set; }
    public decimal Scholarship { get; set; }
    public decimal LateFee { get; set; }
    public decimal Total => Subtotal - Discount - Scholarship + LateFee;
    public decimal AmountPaid { get; set; }
    public decimal Balance => Total - AmountPaid;
    
    // Status
    public FeeStatus Status { get; set; } = FeeStatus.Pending;
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    
    public List<FeePayment> Payments { get; set; } = [];
}

public class FeeItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FeeType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class FeePayment : BaseEntity
{
    public Guid StudentFeeId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
    public string TransactionReference { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string ChequeNumber { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string ReceivedBy { get; set; } = string.Empty;
}

public enum FeeStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue,
    Waived,
    Refunded
}

public enum PaymentMode
{
    Cash,
    Cheque,
    BankTransfer,
    Card,
    UPI,
    Online,
    Crypto
}

// ═══════════════════════════════════════════════════════════════════════════════
// TEACHER SCHEDULE / TIMETABLE
// ═══════════════════════════════════════════════════════════════════════════════
public class ClassSchedule : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid SemesterId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string Section { get; set; } = string.Empty;
    
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration => (int)(EndTime - StartTime).TotalMinutes;
    
    public string Room { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    
    public ClassType ClassType { get; set; } = ClassType.Lecture;
    public bool IsActive { get; set; } = true;
}

public enum ClassType
{
    Lecture,
    Lab,
    Tutorial,
    Seminar,
    Workshop,
    Exam
}

// ═══════════════════════════════════════════════════════════════════════════════
// EXAMINATION & GRADES
// ═══════════════════════════════════════════════════════════════════════════════
public class Examination : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid SemesterId { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty; // "Mid-Term", "Final", "Unit Test 1"
    public ExamType Type { get; set; } = ExamType.Semester;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public bool IsPublished { get; set; } = false;
    public bool ResultsPublished { get; set; } = false;
}

public enum ExamType
{
    UnitTest,
    MidTerm,
    Semester,
    Final,
    Practical,
    Viva,
    Internal
}

public class ExamSchedule : BaseEntity
{
    public Guid ExaminationId { get; set; }
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }
    
    public string Room { get; set; } = string.Empty;
    public int MaxMarks { get; set; }
    public int PassingMarks { get; set; }
    
    public Guid? InvigilatorId { get; set; }
    public string InvigilatorName { get; set; } = string.Empty;
}

public class StudentGrade : BaseEntity
{
    public Guid InstitutionId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentRollNumber { get; set; } = string.Empty;
    
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credits { get; set; }
    
    public Guid SemesterId { get; set; }
    public int SemesterNumber { get; set; }
    
    // Marks
    public decimal InternalMarks { get; set; }
    public decimal ExternalMarks { get; set; }
    public decimal PracticalMarks { get; set; }
    public decimal TotalMarks => InternalMarks + ExternalMarks + PracticalMarks;
    public decimal MaxMarks { get; set; }
    public decimal Percentage => MaxMarks > 0 ? (TotalMarks / MaxMarks) * 100 : 0;
    
    // Grade
    public string Grade { get; set; } = string.Empty; // A+, A, B+, etc.
    public decimal GradePoints { get; set; }
    public bool IsPassed { get; set; }
    
    public string Remarks { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
}

// ═══════════════════════════════════════════════════════════════════════════════
// REPORTS & DASHBOARD
// ═══════════════════════════════════════════════════════════════════════════════
public class CollegeDashboard
{
    public Guid InstitutionId { get; set; }
    
    // Counts
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalDepartments { get; set; }
    
    // Current Semester
    public string CurrentSemester { get; set; } = string.Empty;
    public int NewAdmissions { get; set; }
    public int ActiveStudents { get; set; }
    
    // Attendance
    public decimal TodayStudentAttendance { get; set; }
    public decimal TodayTeacherAttendance { get; set; }
    public decimal MonthlyAverageAttendance { get; set; }
    
    // Fees
    public decimal TotalFeesCollected { get; set; }
    public decimal TotalFeesPending { get; set; }
    public int StudentsWithDues { get; set; }
    
    // By Department
    public List<DepartmentStats> DepartmentStats { get; set; } = [];
}

public class DepartmentStats
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
    public int CourseCount { get; set; }
}

public class AttendanceReport
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNumber { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Semester { get; set; }
    
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    
    public int TotalClasses { get; set; }
    public int ClassesAttended { get; set; }
    public int ClassesAbsent { get; set; }
    public decimal AttendancePercentage => TotalClasses > 0 ? (ClassesAttended * 100m / TotalClasses) : 0;
    
    public List<SubjectAttendance> SubjectWise { get; set; } = [];
}

public class SubjectAttendance
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int Attended { get; set; }
    public decimal Percentage => TotalClasses > 0 ? (Attended * 100m / TotalClasses) : 0;
}

public class FeeReport
{
    public Guid InstitutionId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    
    public decimal TotalCollected { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalOverdue { get; set; }
    
    public List<FeeCollectionSummary> DailyCollection { get; set; } = [];
    public List<CourseFeeSummary> ByCourse { get; set; } = [];
}

public class FeeCollectionSummary
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class CourseFeeSummary
{
    public string CourseName { get; set; } = string.Empty;
    public decimal Collected { get; set; }
    public decimal Pending { get; set; }
    public int PaidCount { get; set; }
    public int PendingCount { get; set; }
}
