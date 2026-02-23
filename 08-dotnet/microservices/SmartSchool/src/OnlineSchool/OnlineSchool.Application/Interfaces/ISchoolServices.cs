using Common.Application.DTOs;
using OnlineSchool.Application.DTOs;

namespace OnlineSchool.Application.Interfaces;

public interface IGradeService
{
    Task<GradeDto?> GetByIdAsync(int id);
    Task<PagedResult<GradeDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<GradeDto>> GetAllActiveAsync();
    Task<GradeDto> CreateAsync(CreateGradeDto dto);
    Task<GradeDto> UpdateAsync(UpdateGradeDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IClassRoomService
{
    Task<ClassRoomDto?> GetByIdAsync(int id);
    Task<PagedResult<ClassRoomDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<ClassRoomDto>> GetByGradeAsync(int gradeId);
    Task<ClassRoomDto> CreateAsync(CreateClassRoomDto dto);
    Task<ClassRoomDto> UpdateAsync(UpdateClassRoomDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IMaterialService
{
    Task<MaterialDto?> GetByIdAsync(int id);
    Task<PagedResult<MaterialDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<MaterialDto>> GetAllActiveAsync();
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto);
    Task<MaterialDto> UpdateAsync(UpdateMaterialDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ITeacherService
{
    Task<TeacherDto?> GetByIdAsync(int id);
    Task<TeacherDto?> GetByUserIdAsync(int userId);
    Task<PagedResult<TeacherDto>> GetAllAsync(QueryParameters parameters);
    Task<TeacherDto> CreateAsync(CreateTeacherDto dto);
    Task<TeacherDto> UpdateAsync(UpdateTeacherDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> AssignMaterialsAsync(int teacherId, IEnumerable<int> materialIds);
}

public interface IStudentService
{
    Task<StudentDto?> GetByIdAsync(int id);
    Task<StudentDto?> GetByUserIdAsync(int userId);
    Task<PagedResult<StudentDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<StudentDto>> GetByClassRoomAsync(int classRoomId);
    Task<StudentDto> CreateAsync(CreateStudentDto dto);
    Task<StudentDto> UpdateAsync(UpdateStudentDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IParentService
{
    Task<ParentDto?> GetByIdAsync(int id);
    Task<ParentDto?> GetByUserIdAsync(int userId);
    Task<PagedResult<ParentDto>> GetAllAsync(QueryParameters parameters);
    Task<ParentDto> CreateAsync(CreateParentDto dto);
    Task<ParentDto> UpdateAsync(UpdateParentDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<StudentProgressDto>> GetChildrenProgressAsync(int parentId);
}
