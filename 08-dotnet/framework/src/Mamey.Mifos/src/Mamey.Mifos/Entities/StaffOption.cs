namespace Mamey.Mifos.Entities
{
    public record StaffOption(int Id, string FirstName, string LastName,
        string DisplayName, int OfficeId, string OfficeName, bool IsLoanOfficer,
        bool IsActive);
}

