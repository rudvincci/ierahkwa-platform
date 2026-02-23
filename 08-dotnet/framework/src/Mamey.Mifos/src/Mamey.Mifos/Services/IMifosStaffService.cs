using Mamey.Mifos.Entities;

namespace Mamey.Mifos.Services
{
    public interface IMifosStaffService
    {
        Task<IReadOnlyList<Staff>> RetrieveStaff();
        Task<IReadOnlyList<Staff>> RetrieveStaffByStatus(StaffStatus status);
        Task<Staff?> RetrieveStaffMember(int id);
        Task CreateStaff(Staff staff);
        Task UpdateStaff(Staff staff);
    }
}

