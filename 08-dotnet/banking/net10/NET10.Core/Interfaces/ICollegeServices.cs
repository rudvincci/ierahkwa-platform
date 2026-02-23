namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════
// COLLEGE MANAGEMENT SYSTEM INTERFACES
// Ierahkwa Sovereign Education System
// ═══════════════════════════════════════════════════════════════

/// <summary>
/// Student Management Service
/// </summary>
public interface IStudentService
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(Guid id);
    Task<Student?> GetByStudentNumberAsync(string studentNumber);
    Task<List<Student>> GetByProgramAsync(string program);
    Task<List<Student>> GetActiveSemesterAsync(string semester);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(Guid id);
    Task<StudentEnrollment> EnrollAsync(Guid studentId, EnrollmentRequest request);
    Task<List<StudentGrade>> GetGradesAsync(Guid studentId);
    Task<StudentTranscript> GetTranscriptAsync(Guid studentId);
}

/// <summary>
/// Teacher/Faculty Management Service
/// </summary>
public interface ITeacherService
{
    Task<List<Teacher>> GetAllAsync();
    Task<Teacher?> GetByIdAsync(Guid id);
    Task<List<Teacher>> GetByDepartmentAsync(string department);
    Task<Teacher> CreateAsync(Teacher teacher);
    Task<Teacher> UpdateAsync(Teacher teacher);
    Task<bool> DeleteAsync(Guid id);
    Task<List<CourseAssignment>> GetAssignmentsAsync(Guid teacherId);
    Task<CourseAssignment> AssignCourseAsync(Guid teacherId, Guid courseId, string semester);
}

/// <summary>
/// Attendance Tracking Service
/// </summary>
public interface IAttendanceService
{
    Task<List<AttendanceRecord>> GetByStudentAsync(Guid studentId, DateTime? from = null, DateTime? to = null);
    Task<List<AttendanceRecord>> GetByCourseAsync(Guid courseId, DateTime date);
    Task<AttendanceRecord> RecordAsync(AttendanceRecord record);
    Task<List<AttendanceRecord>> BulkRecordAsync(List<AttendanceRecord> records);
    Task<AttendanceSummary> GetSummaryAsync(Guid studentId, string semester);
    Task<decimal> GetAttendancePercentageAsync(Guid studentId, Guid courseId);
}

/// <summary>
/// Fee Management Service
/// </summary>
public interface IFeeService
{
    Task<List<FeeStructure>> GetFeeStructuresAsync();
    Task<FeeStructure?> GetFeeStructureAsync(string program, string semester);
    Task<List<StudentFee>> GetStudentFeesAsync(Guid studentId);
    Task<StudentFee> GenerateFeeAsync(Guid studentId, string semester);
    Task<FeePayment> RecordPaymentAsync(Guid studentFeeId, FeePayment payment);
    Task<decimal> GetBalanceAsync(Guid studentId);
    Task<List<StudentFee>> GetOverdueFeesAsync();
}

/// <summary>
/// College Reports Service
/// </summary>
public interface ICollegeReportService
{
    Task<EnrollmentReport> GetEnrollmentReportAsync(string semester);
    Task<AttendanceReport> GetAttendanceReportAsync(string semester, string? department = null);
    Task<AcademicPerformanceReport> GetPerformanceReportAsync(string semester);
    Task<FeeCollectionReport> GetFeeReportAsync(string semester);
    Task<DepartmentReport> GetDepartmentReportAsync(string department);
}

// ═══════════════════════════════════════════════════════════════
// COLLEGE MODELS
// ═══════════════════════════════════════════════════════════════

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int CurrentSemester { get; set; } = 1;
    public DateTime EnrollmentDate { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? NationAffiliation { get; set; }
    public string? WalletAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum StudentStatus
{
    Active,
    OnLeave,
    Suspended,
    Graduated,
    Withdrawn
}

public class Teacher
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public TeacherRank Rank { get; set; } = TeacherRank.Instructor;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public bool IsActive { get; set; } = true;
    public string? WalletAddress { get; set; }
}

public enum TeacherRank
{
    Instructor,
    AssistantProfessor,
    AssociateProfessor,
    Professor,
    DepartmentHead,
    Dean
}

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CourseCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int MaxStudents { get; set; } = 30;
    public decimal Fee { get; set; }
}

public class CourseAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeacherId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}

public class StudentEnrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;
}

public enum EnrollmentStatus
{
    Enrolled,
    Dropped,
    Completed,
    Failed
}

public class EnrollmentRequest
{
    public List<Guid> CourseIds { get; set; } = new();
    public string Semester { get; set; } = string.Empty;
}

public class StudentGrade
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoints { get; set; }
    public int Credits { get; set; }
}

public class StudentTranscript
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public List<StudentGrade> Grades { get; set; } = new();
    public decimal CGPA { get; set; }
    public int TotalCredits { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class AttendanceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused
}

public class AttendanceSummary
{
    public Guid StudentId { get; set; }
    public string Semester { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public decimal Percentage { get; set; }
}

public class FeeStructure
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Program { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public decimal TuitionFee { get; set; }
    public decimal LabFee { get; set; }
    public decimal LibraryFee { get; set; }
    public decimal TechnologyFee { get; set; }
    public decimal ActivityFee { get; set; }
    public decimal TotalFee => TuitionFee + LabFee + LibraryFee + TechnologyFee + ActivityFee;
}

public class StudentFee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Semester { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Balance => TotalAmount - PaidAmount;
    public DateTime DueDate { get; set; }
    public FeeStatus Status { get; set; } = FeeStatus.Pending;
    public List<FeePayment> Payments { get; set; } = new();
}

public enum FeeStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Overdue,
    Waived
}

public class FeePayment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? WalletAddress { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public string? ReceiptNumber { get; set; }
}

// Report Models
public class EnrollmentReport
{
    public string Semester { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public Dictionary<string, int> ByProgram { get; set; } = new();
    public Dictionary<string, int> ByDepartment { get; set; } = new();
    public int NewEnrollments { get; set; }
    public int Graduations { get; set; }
}

public class AttendanceReport
{
    public string Semester { get; set; } = string.Empty;
    public decimal AverageAttendance { get; set; }
    public Dictionary<string, decimal> ByDepartment { get; set; } = new();
    public List<Student> LowAttendanceStudents { get; set; } = new();
}

public class AcademicPerformanceReport
{
    public string Semester { get; set; } = string.Empty;
    public decimal AverageGPA { get; set; }
    public Dictionary<string, decimal> ByDepartment { get; set; } = new();
    public int HonorStudents { get; set; }
    public int ProbationStudents { get; set; }
}

public class FeeCollectionReport
{
    public string Semester { get; set; } = string.Empty;
    public decimal TotalBilled { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal Outstanding { get; set; }
    public decimal CollectionRate { get; set; }
    public int OverdueAccounts { get; set; }
}

public class DepartmentReport
{
    public string Department { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalCourses { get; set; }
    public decimal AverageGPA { get; set; }
    public decimal AttendanceRate { get; set; }
}
