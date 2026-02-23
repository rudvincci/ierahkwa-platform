using Common.Application.DTOs;
using Receptionist.Application.DTOs;
using Receptionist.Domain.Entities;

namespace Receptionist.Application.Interfaces;

public interface IAdmissionEnquiryService
{
    Task<AdmissionEnquiryDto?> GetByIdAsync(int id);
    Task<PagedResult<AdmissionEnquiryDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<AdmissionEnquiryDto>> GetByStatusAsync(EnquiryStatus status);
    Task<AdmissionEnquiryDto> CreateAsync(CreateAdmissionEnquiryDto dto);
    Task<AdmissionEnquiryDto> UpdateAsync(int id, CreateAdmissionEnquiryDto dto);
    Task<bool> UpdateStatusAsync(int id, EnquiryStatus status);
    Task<bool> AssignAsync(int id, int userId);
    Task<bool> DeleteAsync(int id);
}

public interface IVisitorBookService
{
    Task<VisitorBookDto?> GetByIdAsync(int id);
    Task<PagedResult<VisitorBookDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<VisitorBookDto>> GetTodayVisitorsAsync();
    Task<IEnumerable<VisitorBookDto>> GetActiveVisitorsAsync();
    Task<VisitorBookDto> CheckInAsync(CreateVisitorBookDto dto);
    Task<VisitorBookDto> CheckOutAsync(int id);
    Task<bool> DeleteAsync(int id);
}

public interface IPhoneLogService
{
    Task<PhoneLogDto?> GetByIdAsync(int id);
    Task<PagedResult<PhoneLogDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<PhoneLogDto>> GetTodayLogsAsync();
    Task<PhoneLogDto> CreateAsync(int receivedBy, CreatePhoneLogDto dto);
    Task<PhoneLogDto> UpdateAsync(int id, CreatePhoneLogDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IPostalService
{
    Task<PostalDispatchDto?> GetDispatchByIdAsync(int id);
    Task<PagedResult<PostalDispatchDto>> GetAllDispatchesAsync(QueryParameters parameters);
    Task<PostalDispatchDto> CreateDispatchAsync(CreatePostalDispatchDto dto);
    Task<bool> DeleteDispatchAsync(int id);
    
    Task<PostalReceiveDto?> GetReceiveByIdAsync(int id);
    Task<PagedResult<PostalReceiveDto>> GetAllReceivesAsync(QueryParameters parameters);
    Task<PostalReceiveDto> CreateReceiveAsync(CreatePostalReceiveDto dto);
    Task<bool> DeleteReceiveAsync(int id);
}

public interface IComplainService
{
    Task<ComplainDto?> GetByIdAsync(int id);
    Task<PagedResult<ComplainDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<ComplainDto>> GetByStatusAsync(ComplainStatus status);
    Task<ComplainDto> CreateAsync(CreateComplainDto dto);
    Task<ComplainDto> UpdateAsync(int id, CreateComplainDto dto);
    Task<ComplainDto> ResolveAsync(ResolveComplainDto dto);
    Task<bool> AssignAsync(int id, int userId);
    Task<bool> DeleteAsync(int id);
}
