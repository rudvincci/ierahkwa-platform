using NET10.Core.Interfaces;

namespace NET10.Infrastructure.Services.College;

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA SOVEREIGN COLLEGE MANAGEMENT SYSTEM
// Complete Academic Institution Management
// ═══════════════════════════════════════════════════════════════════════════════

public class StudentService : IStudentService
{
    private static readonly List<Student> _students = InitializeDemoStudents();
    private static int _studentCounter = 2025001;
    
    private static List<Student> InitializeDemoStudents()
    {
        return new List<Student>
        {
            new Student
            {
                StudentNumber = "IER-2025-0001",
                FirstName = "Kawennahere",
                LastName = "Thompson",
                Email = "kawennahere.t@ierahkwa.edu",
                Phone = "+1-777-555-0101",
                DateOfBirth = new DateTime(2005, 3, 15),
                Program = "Computer Science",
                Department = "Technology",
                CurrentSemester = 2,
                EnrollmentDate = new DateTime(2025, 8, 1),
                Status = StudentStatus.Active,
                NationAffiliation = "Mohawk Nation",
                WalletAddress = "0xIER001..."
            },
            new Student
            {
                StudentNumber = "IER-2025-0002",
                FirstName = "Tehonikonrathe",
                LastName = "White",
                Email = "tehonikonrathe.w@ierahkwa.edu",
                Phone = "+1-777-555-0102",
                DateOfBirth = new DateTime(2004, 7, 22),
                Program = "Sovereign Law",
                Department = "Law & Governance",
                CurrentSemester = 4,
                EnrollmentDate = new DateTime(2024, 8, 1),
                Status = StudentStatus.Active,
                NationAffiliation = "Seneca Nation"
            },
            new Student
            {
                StudentNumber = "IER-2025-0003",
                FirstName = "Maria",
                LastName = "Garcia",
                Email = "maria.g@ierahkwa.edu",
                Phone = "+1-777-555-0103",
                DateOfBirth = new DateTime(2005, 11, 8),
                Program = "Business Administration",
                Department = "Business",
                CurrentSemester = 2,
                EnrollmentDate = new DateTime(2025, 8, 1),
                Status = StudentStatus.Active,
                NationAffiliation = "Taino Nation"
            }
        };
    }
    
    public Task<List<Student>> GetAllAsync()
    {
        return Task.FromResult(_students.Where(s => s.Status != StudentStatus.Withdrawn).ToList());
    }
    
    public Task<Student?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_students.FirstOrDefault(s => s.Id == id));
    }
    
    public Task<Student?> GetByStudentNumberAsync(string studentNumber)
    {
        return Task.FromResult(_students.FirstOrDefault(s => 
            s.StudentNumber.Equals(studentNumber, StringComparison.OrdinalIgnoreCase)));
    }
    
    public Task<List<Student>> GetByProgramAsync(string program)
    {
        return Task.FromResult(_students
            .Where(s => s.Program.Equals(program, StringComparison.OrdinalIgnoreCase) && s.Status == StudentStatus.Active)
            .ToList());
    }
    
    public Task<List<Student>> GetActiveSemesterAsync(string semester)
    {
        return Task.FromResult(_students.Where(s => s.Status == StudentStatus.Active).ToList());
    }
    
    public Task<Student> CreateAsync(Student student)
    {
        student.Id = Guid.NewGuid();
        student.StudentNumber = $"IER-{DateTime.UtcNow.Year}-{_studentCounter++:D4}";
        student.CreatedAt = DateTime.UtcNow;
        _students.Add(student);
        return Task.FromResult(student);
    }
    
    public Task<Student> UpdateAsync(Student student)
    {
        var index = _students.FindIndex(s => s.Id == student.Id);
        if (index >= 0) _students[index] = student;
        return Task.FromResult(student);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student != null)
        {
            student.Status = StudentStatus.Withdrawn;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<StudentEnrollment> EnrollAsync(Guid studentId, EnrollmentRequest request)
    {
        var enrollment = new StudentEnrollment
        {
            StudentId = studentId,
            CourseId = request.CourseIds.FirstOrDefault(),
            Semester = request.Semester,
            Status = EnrollmentStatus.Enrolled
        };
        return Task.FromResult(enrollment);
    }
    
    public Task<List<StudentGrade>> GetGradesAsync(Guid studentId)
    {
        var grades = new List<StudentGrade>
        {
            new StudentGrade { StudentId = studentId, CourseName = "Programming I", Semester = "Fall 2025", Score = 92, Grade = "A", GradePoints = 4.0m, Credits = 3 },
            new StudentGrade { StudentId = studentId, CourseName = "Calculus I", Semester = "Fall 2025", Score = 88, Grade = "B+", GradePoints = 3.5m, Credits = 4 },
            new StudentGrade { StudentId = studentId, CourseName = "Sovereign Law", Semester = "Fall 2025", Score = 95, Grade = "A", GradePoints = 4.0m, Credits = 3 }
        };
        return Task.FromResult(grades);
    }
    
    public async Task<StudentTranscript> GetTranscriptAsync(Guid studentId)
    {
        var student = await GetByIdAsync(studentId);
        var grades = await GetGradesAsync(studentId);
        
        var totalPoints = grades.Sum(g => g.GradePoints * g.Credits);
        var totalCredits = grades.Sum(g => g.Credits);
        
        return new StudentTranscript
        {
            StudentId = studentId,
            StudentName = student?.FullName ?? "Unknown",
            StudentNumber = student?.StudentNumber ?? "",
            Program = student?.Program ?? "",
            Grades = grades,
            CGPA = totalCredits > 0 ? totalPoints / totalCredits : 0,
            TotalCredits = totalCredits
        };
    }
}

public class TeacherService : ITeacherService
{
    private static readonly List<Teacher> _teachers = InitializeDemoTeachers();
    private static int _employeeCounter = 1001;
    
    private static List<Teacher> InitializeDemoTeachers()
    {
        return new List<Teacher>
        {
            new Teacher
            {
                EmployeeId = "TCH-001",
                FirstName = "Dr. Karakwine",
                LastName = "Hill",
                Email = "k.hill@ierahkwa.edu",
                Phone = "+1-777-555-1001",
                Department = "Technology",
                Specialization = "Artificial Intelligence",
                Rank = TeacherRank.Professor,
                HireDate = new DateTime(2018, 8, 1),
                Salary = 95000,
                IsActive = true
            },
            new Teacher
            {
                EmployeeId = "TCH-002",
                FirstName = "Dr. Sarah",
                LastName = "Redbird",
                Email = "s.redbird@ierahkwa.edu",
                Phone = "+1-777-555-1002",
                Department = "Law & Governance",
                Specialization = "Indigenous Sovereignty",
                Rank = TeacherRank.AssociateProfessor,
                HireDate = new DateTime(2020, 1, 15),
                Salary = 85000,
                IsActive = true
            },
            new Teacher
            {
                EmployeeId = "TCH-003",
                FirstName = "Prof. Michael",
                LastName = "Running Deer",
                Email = "m.runningdeer@ierahkwa.edu",
                Phone = "+1-777-555-1003",
                Department = "Business",
                Specialization = "Finance & Cryptocurrency",
                Rank = TeacherRank.AssistantProfessor,
                HireDate = new DateTime(2022, 8, 1),
                Salary = 75000,
                IsActive = true
            }
        };
    }
    
    public Task<List<Teacher>> GetAllAsync()
    {
        return Task.FromResult(_teachers.Where(t => t.IsActive).ToList());
    }
    
    public Task<Teacher?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_teachers.FirstOrDefault(t => t.Id == id));
    }
    
    public Task<List<Teacher>> GetByDepartmentAsync(string department)
    {
        return Task.FromResult(_teachers
            .Where(t => t.Department.Equals(department, StringComparison.OrdinalIgnoreCase) && t.IsActive)
            .ToList());
    }
    
    public Task<Teacher> CreateAsync(Teacher teacher)
    {
        teacher.Id = Guid.NewGuid();
        teacher.EmployeeId = $"TCH-{_employeeCounter++:D3}";
        _teachers.Add(teacher);
        return Task.FromResult(teacher);
    }
    
    public Task<Teacher> UpdateAsync(Teacher teacher)
    {
        var index = _teachers.FindIndex(t => t.Id == teacher.Id);
        if (index >= 0) _teachers[index] = teacher;
        return Task.FromResult(teacher);
    }
    
    public Task<bool> DeleteAsync(Guid id)
    {
        var teacher = _teachers.FirstOrDefault(t => t.Id == id);
        if (teacher != null)
        {
            teacher.IsActive = false;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public Task<List<CourseAssignment>> GetAssignmentsAsync(Guid teacherId)
    {
        var assignments = new List<CourseAssignment>
        {
            new CourseAssignment { TeacherId = teacherId, Semester = "Spring 2026", Schedule = "MWF 9:00-10:00", Room = "Tech-101" }
        };
        return Task.FromResult(assignments);
    }
    
    public Task<CourseAssignment> AssignCourseAsync(Guid teacherId, Guid courseId, string semester)
    {
        var assignment = new CourseAssignment
        {
            TeacherId = teacherId,
            CourseId = courseId,
            Semester = semester
        };
        return Task.FromResult(assignment);
    }
}

public class AttendanceService : IAttendanceService
{
    private static readonly List<AttendanceRecord> _records = new();
    
    public Task<List<AttendanceRecord>> GetByStudentAsync(Guid studentId, DateTime? from = null, DateTime? to = null)
    {
        var query = _records.Where(r => r.StudentId == studentId);
        if (from.HasValue) query = query.Where(r => r.Date >= from.Value);
        if (to.HasValue) query = query.Where(r => r.Date <= to.Value);
        return Task.FromResult(query.OrderByDescending(r => r.Date).ToList());
    }
    
    public Task<List<AttendanceRecord>> GetByCourseAsync(Guid courseId, DateTime date)
    {
        return Task.FromResult(_records
            .Where(r => r.CourseId == courseId && r.Date.Date == date.Date)
            .ToList());
    }
    
    public Task<AttendanceRecord> RecordAsync(AttendanceRecord record)
    {
        record.Id = Guid.NewGuid();
        record.RecordedAt = DateTime.UtcNow;
        _records.Add(record);
        return Task.FromResult(record);
    }
    
    public Task<List<AttendanceRecord>> BulkRecordAsync(List<AttendanceRecord> records)
    {
        foreach (var record in records)
        {
            record.Id = Guid.NewGuid();
            record.RecordedAt = DateTime.UtcNow;
            _records.Add(record);
        }
        return Task.FromResult(records);
    }
    
    public Task<AttendanceSummary> GetSummaryAsync(Guid studentId, string semester)
    {
        var records = _records.Where(r => r.StudentId == studentId).ToList();
        var total = records.Count > 0 ? records.Count : 100; // Demo
        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        if (total == 0) { total = 100; present = 90; } // Demo data
        
        return Task.FromResult(new AttendanceSummary
        {
            StudentId = studentId,
            Semester = semester,
            TotalClasses = total,
            Present = present > 0 ? present : 90,
            Absent = 8,
            Late = 2,
            Excused = 0,
            Percentage = 90m
        });
    }
    
    public Task<decimal> GetAttendancePercentageAsync(Guid studentId, Guid courseId)
    {
        var records = _records.Where(r => r.StudentId == studentId && r.CourseId == courseId).ToList();
        if (!records.Any()) return Task.FromResult(92m); // Demo
        var present = records.Count(r => r.Status == AttendanceStatus.Present || r.Status == AttendanceStatus.Late);
        return Task.FromResult(records.Count > 0 ? (present * 100m / records.Count) : 0);
    }
}

public class FeeService : IFeeService
{
    private static readonly List<FeeStructure> _structures = InitializeFeeStructures();
    private static readonly List<StudentFee> _studentFees = new();
    private static int _receiptCounter = 100001;
    
    private static List<FeeStructure> InitializeFeeStructures()
    {
        return new List<FeeStructure>
        {
            new FeeStructure { Program = "Computer Science", Semester = "Spring 2026", TuitionFee = 5000, LabFee = 500, LibraryFee = 100, TechnologyFee = 200, ActivityFee = 150 },
            new FeeStructure { Program = "Sovereign Law", Semester = "Spring 2026", TuitionFee = 6000, LabFee = 0, LibraryFee = 200, TechnologyFee = 150, ActivityFee = 150 },
            new FeeStructure { Program = "Business Administration", Semester = "Spring 2026", TuitionFee = 4500, LabFee = 0, LibraryFee = 100, TechnologyFee = 150, ActivityFee = 150 }
        };
    }
    
    public Task<List<FeeStructure>> GetFeeStructuresAsync()
    {
        return Task.FromResult(_structures);
    }
    
    public Task<FeeStructure?> GetFeeStructureAsync(string program, string semester)
    {
        return Task.FromResult(_structures.FirstOrDefault(f => 
            f.Program.Equals(program, StringComparison.OrdinalIgnoreCase) &&
            f.Semester.Equals(semester, StringComparison.OrdinalIgnoreCase)));
    }
    
    public Task<List<StudentFee>> GetStudentFeesAsync(Guid studentId)
    {
        return Task.FromResult(_studentFees.Where(f => f.StudentId == studentId).ToList());
    }
    
    public async Task<StudentFee> GenerateFeeAsync(Guid studentId, string semester)
    {
        var structure = await GetFeeStructureAsync("Computer Science", semester) ?? _structures.First();
        
        var fee = new StudentFee
        {
            StudentId = studentId,
            Semester = semester,
            TotalAmount = structure.TotalFee,
            PaidAmount = 0,
            DueDate = DateTime.UtcNow.AddDays(30),
            Status = FeeStatus.Pending
        };
        
        _studentFees.Add(fee);
        return fee;
    }
    
    public Task<FeePayment> RecordPaymentAsync(Guid studentFeeId, FeePayment payment)
    {
        var fee = _studentFees.FirstOrDefault(f => f.Id == studentFeeId);
        if (fee != null)
        {
            payment.Id = Guid.NewGuid();
            payment.ReceiptNumber = $"RCP-{_receiptCounter++:D6}";
            payment.PaidAt = DateTime.UtcNow;
            fee.Payments.Add(payment);
            fee.PaidAmount += payment.Amount;
            fee.Status = fee.PaidAmount >= fee.TotalAmount ? FeeStatus.Paid : FeeStatus.PartiallyPaid;
        }
        return Task.FromResult(payment);
    }
    
    public Task<decimal> GetBalanceAsync(Guid studentId)
    {
        var balance = _studentFees.Where(f => f.StudentId == studentId).Sum(f => f.Balance);
        return Task.FromResult(balance);
    }
    
    public Task<List<StudentFee>> GetOverdueFeesAsync()
    {
        return Task.FromResult(_studentFees
            .Where(f => f.Status != FeeStatus.Paid && f.DueDate < DateTime.UtcNow)
            .ToList());
    }
}

public class CollegeReportService : ICollegeReportService
{
    public Task<EnrollmentReport> GetEnrollmentReportAsync(string semester)
    {
        return Task.FromResult(new EnrollmentReport
        {
            Semester = semester,
            TotalStudents = 1250,
            ByProgram = new Dictionary<string, int>
            {
                { "Computer Science", 320 },
                { "Sovereign Law", 180 },
                { "Business Administration", 280 },
                { "Indigenous Studies", 150 },
                { "Healthcare", 220 },
                { "Engineering", 100 }
            },
            ByDepartment = new Dictionary<string, int>
            {
                { "Technology", 420 },
                { "Law & Governance", 180 },
                { "Business", 280 },
                { "Arts & Culture", 150 },
                { "Health Sciences", 220 }
            },
            NewEnrollments = 285,
            Graduations = 156
        });
    }
    
    public Task<AttendanceReport> GetAttendanceReportAsync(string semester, string? department = null)
    {
        return Task.FromResult(new AttendanceReport
        {
            Semester = semester,
            AverageAttendance = 91.5m,
            ByDepartment = new Dictionary<string, decimal>
            {
                { "Technology", 93.2m },
                { "Law & Governance", 95.1m },
                { "Business", 89.8m },
                { "Arts & Culture", 88.5m },
                { "Health Sciences", 94.3m }
            },
            LowAttendanceStudents = new List<Student>()
        });
    }
    
    public Task<AcademicPerformanceReport> GetPerformanceReportAsync(string semester)
    {
        return Task.FromResult(new AcademicPerformanceReport
        {
            Semester = semester,
            AverageGPA = 3.45m,
            ByDepartment = new Dictionary<string, decimal>
            {
                { "Technology", 3.52m },
                { "Law & Governance", 3.68m },
                { "Business", 3.38m },
                { "Arts & Culture", 3.42m },
                { "Health Sciences", 3.55m }
            },
            HonorStudents = 185,
            ProbationStudents = 42
        });
    }
    
    public Task<FeeCollectionReport> GetFeeReportAsync(string semester)
    {
        return Task.FromResult(new FeeCollectionReport
        {
            Semester = semester,
            TotalBilled = 6250000,
            TotalCollected = 5687500,
            Outstanding = 562500,
            CollectionRate = 91m,
            OverdueAccounts = 89
        });
    }
    
    public Task<DepartmentReport> GetDepartmentReportAsync(string department)
    {
        return Task.FromResult(new DepartmentReport
        {
            Department = department,
            TotalStudents = 320,
            TotalTeachers = 18,
            TotalCourses = 45,
            AverageGPA = 3.52m,
            AttendanceRate = 93.2m
        });
    }
}
